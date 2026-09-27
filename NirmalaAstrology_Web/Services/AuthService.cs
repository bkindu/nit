using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Forms-auth backed user store in MySQL. Seeds Admin role + default admin once.
    /// </summary>
    public sealed class AuthService
    {
        public const string RoleAdmin = "Admin";
        public const string RolePublic = "Public";

        public AppUser ValidateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            EnsureSchemaAndSeed();
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT user_id, username, password_hash, password_salt, display_name, email, mobile, is_active
FROM tbl_app_user
WHERE username = @Username
LIMIT 1",
                    new MySqlParameter("@Username", username.Trim()));
                if (dt.Rows.Count == 0) return null;
                var row = dt.Rows[0];
                if (!Convert.ToBoolean(row["is_active"])) return null;
                var hash = Convert.ToString(row["password_hash"]);
                var salt = Convert.ToString(row["password_salt"]);
                if (!SlowEquals(hash, HashPassword(password, salt)))
                    return null;

                var userId = Convert.ToInt32(row["user_id"]);
                return new AppUser
                {
                    UserId = userId,
                    Username = Convert.ToString(row["username"]),
                    DisplayName = Convert.ToString(row["display_name"]),
                    Email = Convert.ToString(row["email"]),
                    Mobile = row.Table.Columns.Contains("mobile") ? Convert.ToString(row["mobile"]) : null,
                    IsActive = true,
                    Roles = LoadRoles(db, userId)
                };
            }
        }

        public string[] LoadRolesForUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return new string[0];
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(
                    "SELECT user_id FROM tbl_app_user WHERE username=@u LIMIT 1",
                    new MySqlParameter("@u", username.Trim()));
                if (dt.Rows.Count == 0) return new string[0];
                return LoadRoles(db, Convert.ToInt32(dt.Rows[0]["user_id"]));
            }
        }

        public void EnsureSchemaAndSeed()
        {
            try
            {
                using (var db = new DatabaseHelper())
                {
                    db.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS tbl_app_user (
  user_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(64) NOT NULL,
  password_hash VARCHAR(256) NOT NULL,
  password_salt VARCHAR(128) NOT NULL,
  display_name VARCHAR(120) NULL,
  email VARCHAR(150) NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY uq_app_user_username (username)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                    db.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS tbl_app_role (
  role_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  role_code VARCHAR(32) NOT NULL,
  role_name VARCHAR(64) NOT NULL,
  UNIQUE KEY uq_app_role_code (role_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                    db.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS tbl_app_user_role (
  user_id INT NOT NULL,
  role_id INT NOT NULL,
  PRIMARY KEY (user_id, role_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                    db.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS tbl_report_definition (
  report_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  report_code VARCHAR(64) NOT NULL,
  report_name VARCHAR(120) NOT NULL,
  public_allowed TINYINT(1) NOT NULL DEFAULT 0,
  admin_allowed TINYINT(1) NOT NULL DEFAULT 1,
  is_enabled TINYINT(1) NOT NULL DEFAULT 1,
  usage_limit_daily INT NULL,
  notes VARCHAR(255) NULL,
  UNIQUE KEY uq_report_code (report_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                    db.ExecuteNonQuery("INSERT IGNORE INTO tbl_app_role (role_code, role_name) VALUES ('Admin','Administrator'),('Public','Public User')");

                    try { db.ExecuteNonQuery("ALTER TABLE tbl_app_user ADD COLUMN mobile VARCHAR(20) NULL AFTER email"); }
                    catch { /* already exists */ }

                    SeedReports(db);
                    SeedDefaultAdmin(db);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[AuthService.EnsureSchema] " + ex.Message);
            }
        }

        public AppUser RegisterPublicUser(string username, string password, string displayName, string email, string mobile, out string error)
        {
            error = null;
            EnsureSchemaAndSeed();
            new CommerceSchemaService().EnsureSchemaAndSeed();

            username = (username ?? "").Trim();
            email = (email ?? "").Trim();
            displayName = (displayName ?? "").Trim();
            mobile = string.IsNullOrWhiteSpace(mobile) ? null : mobile.Trim();

            if (username.Length < 3) { error = "Username must be at least 3 characters."; return null; }
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8) { error = "Password must be at least 8 characters."; return null; }
            if (string.IsNullOrWhiteSpace(email) || email.IndexOf('@') < 1) { error = "A valid email is required."; return null; }
            if (string.IsNullOrWhiteSpace(displayName)) { error = "Name is required."; return null; }

            using (var db = new DatabaseHelper())
            {
                var exists = Convert.ToInt32(db.ExecuteScalar(
                    "SELECT COUNT(*) FROM tbl_app_user WHERE username=@u OR LOWER(email)=LOWER(@e)",
                    new MySqlParameter("@u", username),
                    new MySqlParameter("@e", email)));
                if (exists > 0)
                {
                    error = "An account with this username or email already exists.";
                    return null;
                }

                var salt = CreateSalt();
                var hash = HashPassword(password, salt);
                db.ExecuteNonQuery(@"
INSERT INTO tbl_app_user (username, password_hash, password_salt, display_name, email, mobile, is_active)
VALUES (@u, @h, @s, @d, @e, @m, 1)",
                    new MySqlParameter("@u", username),
                    new MySqlParameter("@h", hash),
                    new MySqlParameter("@s", salt),
                    new MySqlParameter("@d", displayName),
                    new MySqlParameter("@e", email),
                    new MySqlParameter("@m", (object)mobile ?? DBNull.Value));

                db.ExecuteNonQuery(@"
INSERT IGNORE INTO tbl_app_user_role (user_id, role_id)
SELECT u.user_id, r.role_id
FROM tbl_app_user u
CROSS JOIN tbl_app_role r
WHERE u.username=@u AND r.role_code='Public'",
                    new MySqlParameter("@u", username));

                return ValidateUser(username, password);
            }
        }

        public AppUser GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            EnsureSchemaAndSeed();
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT user_id, username, display_name, email, mobile, is_active
FROM tbl_app_user WHERE username=@u LIMIT 1",
                    new MySqlParameter("@u", username.Trim()));
                if (dt.Rows.Count == 0) return null;
                var row = dt.Rows[0];
                var id = Convert.ToInt32(row["user_id"]);
                return new AppUser
                {
                    UserId = id,
                    Username = Convert.ToString(row["username"]),
                    DisplayName = Convert.ToString(row["display_name"]),
                    Email = Convert.ToString(row["email"]),
                    Mobile = row.Table.Columns.Contains("mobile") ? Convert.ToString(row["mobile"]) : null,
                    IsActive = Convert.ToBoolean(row["is_active"]),
                    Roles = LoadRoles(db, id)
                };
            }
        }

        public bool UpdateProfile(int userId, string displayName, string email, string mobile, out string error)
        {
            error = null;
            displayName = (displayName ?? "").Trim();
            email = (email ?? "").Trim();
            mobile = string.IsNullOrWhiteSpace(mobile) ? null : mobile.Trim();
            if (string.IsNullOrWhiteSpace(displayName)) { error = "Name is required."; return false; }
            if (string.IsNullOrWhiteSpace(email) || email.IndexOf('@') < 1) { error = "A valid email is required."; return false; }

            using (var db = new DatabaseHelper())
            {
                var dup = Convert.ToInt32(db.ExecuteScalar(
                    "SELECT COUNT(*) FROM tbl_app_user WHERE LOWER(email)=LOWER(@e) AND user_id<>@id",
                    new MySqlParameter("@e", email),
                    new MySqlParameter("@id", userId)));
                if (dup > 0) { error = "Email is already used by another account."; return false; }

                db.ExecuteNonQuery(@"
UPDATE tbl_app_user SET display_name=@d, email=@e, mobile=@m WHERE user_id=@id",
                    new MySqlParameter("@d", displayName),
                    new MySqlParameter("@e", email),
                    new MySqlParameter("@m", (object)mobile ?? DBNull.Value),
                    new MySqlParameter("@id", userId));
                return true;
            }
        }

        public bool ChangePassword(string username, string currentPassword, string newPassword, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                error = "New password must be at least 8 characters.";
                return false;
            }
            var user = ValidateUser(username, currentPassword);
            if (user == null) { error = "Current password is incorrect."; return false; }

            var salt = CreateSalt();
            var hash = HashPassword(newPassword, salt);
            using (var db = new DatabaseHelper())
            {
                db.ExecuteNonQuery(@"
UPDATE tbl_app_user SET password_hash=@h, password_salt=@s WHERE user_id=@id",
                    new MySqlParameter("@h", hash),
                    new MySqlParameter("@s", salt),
                    new MySqlParameter("@id", user.UserId));
            }
            return true;
        }

        public AppUser FindByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT user_id, username, display_name, email, mobile, is_active
FROM tbl_app_user WHERE LOWER(email)=LOWER(@e) LIMIT 1",
                    new MySqlParameter("@e", email.Trim()));
                if (dt.Rows.Count == 0) return null;
                var row = dt.Rows[0];
                var id = Convert.ToInt32(row["user_id"]);
                return new AppUser
                {
                    UserId = id,
                    Username = Convert.ToString(row["username"]),
                    DisplayName = Convert.ToString(row["display_name"]),
                    Email = Convert.ToString(row["email"]),
                    Mobile = row.Table.Columns.Contains("mobile") ? Convert.ToString(row["mobile"]) : null,
                    IsActive = Convert.ToBoolean(row["is_active"]),
                    Roles = LoadRoles(db, id)
                };
            }
        }

        public bool ResetPasswordToTemporary(int userId, out string temporaryPassword, out string error)
        {
            temporaryPassword = Guid.NewGuid().ToString("N").Substring(0, 10);
            return SetPassword(userId, temporaryPassword, out error);
        }

        public bool SetPassword(int userId, string newPassword, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                error = "Password must be at least 6 characters.";
                return false;
            }
            var salt = CreateSalt();
            var hash = HashPassword(newPassword, salt);
            using (var db = new DatabaseHelper())
            {
                db.ExecuteNonQuery(@"
UPDATE tbl_app_user SET password_hash=@h, password_salt=@s WHERE user_id=@id",
                    new MySqlParameter("@h", hash),
                    new MySqlParameter("@s", salt),
                    new MySqlParameter("@id", userId));
            }
            return true;
        }


        private static void SeedReports(DatabaseHelper db)
        {
            var seeds = new[]
            {
                new object[] { "KUNDLI_BASIC", "Basic Kundli Chart", 1, 1 },
                new object[] { "KUNDLI_DETAILED", "Detailed Horoscope Report", 0, 1 },
                new object[] { "NUMEROLOGY_BASIC", "Basic Numerology", 1, 1 },
                new object[] { "NUMEROLOGY_FULL", "Full Numerology Report", 0, 1 },
                new object[] { "MATCH_MAKING", "Match Making", 0, 1 },
                new object[] { "LAL_KITAB", "Lal Kitab", 0, 1 },
                new object[] { "VARSH_PHAL", "Varsh Phal", 0, 1 },
                new object[] { "MUHURAT", "Muhurat / Kaal Shuddhi", 0, 1 },
                new object[] { "VIVAH_MUHURTA", "Vivah Muhurta", 0, 1 },
                new object[] { "GOCHAR", "Gochar Phal", 0, 1 },
                new object[] { "PALMISTRY", "Palmistry", 0, 1 },
                new object[] { "RECORDS", "Native Records", 0, 1 },
                new object[] { "PLACES_ADMIN", "Place Atlas Admin", 0, 1 }
            };
            foreach (var s in seeds)
            {
                db.ExecuteNonQuery(@"
INSERT IGNORE INTO tbl_report_definition
(report_code, report_name, public_allowed, admin_allowed, is_enabled)
VALUES (@c, @n, @p, @a, 1)",
                    new MySqlParameter("@c", s[0]),
                    new MySqlParameter("@n", s[1]),
                    new MySqlParameter("@p", s[2]),
                    new MySqlParameter("@a", s[3]));
            }
        }

        private static void SeedDefaultAdmin(DatabaseHelper db)
        {
            var count = Convert.ToInt32(db.ExecuteScalar("SELECT COUNT(*) FROM tbl_app_user"));
            if (count > 0) return;

            var user = ConfigurationManager.AppSettings["DefaultAdminUser"] ?? "admin";
            var pass = ConfigurationManager.AppSettings["DefaultAdminPassword"] ?? "Nirmala@1974";
            var salt = CreateSalt();
            var hash = HashPassword(pass, salt);
            db.ExecuteNonQuery(@"
INSERT INTO tbl_app_user (username, password_hash, password_salt, display_name, email, is_active)
VALUES (@u, @h, @s, @d, @e, 1)",
                new MySqlParameter("@u", user),
                new MySqlParameter("@h", hash),
                new MySqlParameter("@s", salt),
                new MySqlParameter("@d", "Indu Sharma"),
                new MySqlParameter("@e", "nirmala.astrology@gmail.com"));

            db.ExecuteNonQuery(@"
INSERT IGNORE INTO tbl_app_user_role (user_id, role_id)
SELECT u.user_id, r.role_id
FROM tbl_app_user u
CROSS JOIN tbl_app_role r
WHERE u.username = @u AND r.role_code = 'Admin'",
                new MySqlParameter("@u", user));
        }

        private static string[] LoadRoles(DatabaseHelper db, int userId)
        {
            var dt = db.ExecuteDataTable(@"
SELECT r.role_code
FROM tbl_app_user_role ur
JOIN tbl_app_role r ON r.role_id = ur.role_id
WHERE ur.user_id = @id",
                new MySqlParameter("@id", userId));
            var list = new List<string>();
            foreach (DataRow row in dt.Rows)
                list.Add(Convert.ToString(row["role_code"]));
            return list.ToArray();
        }

        public static string CreateSalt()
        {
            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using (var derive = new Rfc2898DeriveBytes(password, saltBytes, 10000))
            {
                return Convert.ToBase64String(derive.GetBytes(32));
            }
        }

        private static bool SlowEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            var diff = 0;
            for (var i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
