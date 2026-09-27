using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Bootstraps public commerce tables (services, appointments, payments) and pricing columns.
    /// </summary>
    public sealed class CommerceSchemaService
    {
        public void EnsureSchemaAndSeed()
        {
            try
            {
                using (var db = new DatabaseHelper())
                {
                    TryAlter(db, "ALTER TABLE tbl_app_user ADD COLUMN mobile VARCHAR(20) NULL AFTER email");
                    TryAlter(db, "ALTER TABLE tbl_report_definition ADD COLUMN price_inr DECIMAL(10,2) NOT NULL DEFAULT 0 AFTER notes");
                    TryAlter(db, "ALTER TABLE tbl_report_definition ADD COLUMN short_description VARCHAR(255) NULL AFTER price_inr");

                    db.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS tbl_service (
  service_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  service_code VARCHAR(64) NOT NULL,
  service_name VARCHAR(120) NOT NULL,
  description VARCHAR(500) NULL,
  duration_minutes INT NOT NULL DEFAULT 30,
  price_inr DECIMAL(10,2) NOT NULL DEFAULT 0,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  sort_order INT NOT NULL DEFAULT 0,
  UNIQUE KEY uq_service_code (service_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                    db.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS tbl_appointment (
  appointment_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  booking_ref VARCHAR(32) NOT NULL,
  user_id INT NOT NULL,
  service_id INT NOT NULL,
  appointment_date DATE NOT NULL,
  appointment_time TIME NOT NULL,
  amount DECIMAL(10,2) NOT NULL,
  currency CHAR(3) NOT NULL DEFAULT 'INR',
  payment_status VARCHAR(32) NOT NULL,
  appointment_status VARCHAR(32) NOT NULL,
  customer_name VARCHAR(120) NOT NULL,
  customer_email VARCHAR(150) NOT NULL,
  customer_mobile VARCHAR(20) NULL,
  notes VARCHAR(500) NULL,
  admin_notes VARCHAR(500) NULL,
  approved_at DATETIME NULL,
  approved_by INT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY uq_booking_ref (booking_ref),
  KEY ix_appt_user (user_id),
  KEY ix_appt_date_time (appointment_date, appointment_time),
  KEY ix_appt_status (appointment_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                    db.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS tbl_payment (
  payment_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  appointment_id INT NULL,
  report_order_id INT NULL,
  amount DECIMAL(10,2) NOT NULL,
  currency CHAR(3) NOT NULL DEFAULT 'INR',
  gateway VARCHAR(32) NOT NULL DEFAULT 'ManualUpi',
  order_id VARCHAR(64) NOT NULL,
  transaction_ref VARCHAR(128) NULL,
  payment_status VARCHAR(32) NOT NULL,
  payment_method VARCHAR(64) NULL,
  gateway_response_ref VARCHAR(255) NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY uq_payment_order (order_id),
  KEY ix_pay_user (user_id),
  KEY ix_pay_appt (appointment_id),
  KEY ix_pay_txn (transaction_ref)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                    db.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS tbl_public_report_order (
  report_order_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  order_ref VARCHAR(32) NOT NULL,
  user_id INT NULL,
  report_code VARCHAR(64) NOT NULL,
  amount DECIMAL(10,2) NOT NULL DEFAULT 0,
  currency CHAR(3) NOT NULL DEFAULT 'INR',
  payment_status VARCHAR(32) NOT NULL,
  birth_payload_json MEDIUMTEXT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY uq_report_order_ref (order_ref),
  KEY ix_pro_user (user_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                    SeedServices(db);
                    SeedReportPrices(db);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[CommerceSchema] " + ex.Message);
            }
        }

        private static void TryAlter(DatabaseHelper db, string sql)
        {
            try { db.ExecuteNonQuery(sql); }
            catch (MySqlException) { /* column already exists */ }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine("[CommerceSchema.Alter] " + ex.Message); }
        }

        private static void SeedServices(DatabaseHelper db)
        {
            var rows = new[]
            {
                new object[] { "CONSULT_GENERAL", "Astrology Consultation", "Personal consultation with Indu Sharma (pre-appointment).", 45, 1100m, 1 },
                new object[] { "CONSULT_MATCH", "Match Making Consultation", "Ashtakoot / compatibility discussion.", 45, 1500m, 2 },
                new object[] { "CONSULT_MUHURAT", "Muhurat Consultation", "Auspicious timing guidance.", 30, 900m, 3 }
            };
            foreach (var r in rows)
            {
                db.ExecuteNonQuery(@"
INSERT IGNORE INTO tbl_service
(service_code, service_name, description, duration_minutes, price_inr, is_active, sort_order)
VALUES (@c,@n,@d,@m,@p,1,@o)",
                    new MySqlParameter("@c", r[0]),
                    new MySqlParameter("@n", r[1]),
                    new MySqlParameter("@d", r[2]),
                    new MySqlParameter("@m", r[3]),
                    new MySqlParameter("@p", r[4]),
                    new MySqlParameter("@o", r[5]));
            }
        }

        private static void SeedReportPrices(DatabaseHelper db)
        {
            // Short/basic public reports are FREE — no payment gateway.
            db.ExecuteNonQuery("UPDATE tbl_report_definition SET price_inr=0.00, short_description='FREE 2-page PDF: basic birth info + kundli chart (no payment)' WHERE report_code='KUNDLI_BASIC'");
            db.ExecuteNonQuery("UPDATE tbl_report_definition SET price_inr=0.00, short_description='FREE basic core numerology PDF (no payment)' WHERE report_code='NUMEROLOGY_BASIC'");
        }

        public static string UpiId
        {
            get { return ConfigurationManager.AppSettings["Payment:UpiId"] ?? "nirmala.astrology@okicici"; }
        }

        public static string PaymentGateway
        {
            get { return ConfigurationManager.AppSettings["Payment:Gateway"] ?? "ManualUpi"; }
        }
    }
}
