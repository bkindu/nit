using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    public sealed class ArticleService
    {
        private static readonly HashSet<string> AllowedImageExt =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ArticleService()
        {
            EnsureSchema();
        }

        public void EnsureSchema()
        {
            try
            {
                using (var db = new DatabaseHelper())
                {
                    db.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS tbl_article (
  article_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  title VARCHAR(200) NOT NULL,
  slug VARCHAR(220) NOT NULL,
  summary VARCHAR(500) NULL,
  body_html MEDIUMTEXT NOT NULL,
  image_url VARCHAR(255) NULL,
  is_published TINYINT(1) NOT NULL DEFAULT 0,
  published_at DATETIME NULL,
  created_by INT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY uq_article_slug (slug),
  KEY ix_article_published (is_published, published_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[ArticleService.Schema] " + ex.Message);
            }
        }

        public IList<ArticleRecord> GetPublished(int take = 50)
        {
            take = Math.Max(1, Math.Min(100, take));
            var list = new List<ArticleRecord>();
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT * FROM tbl_article
WHERE is_published=1
ORDER BY COALESCE(published_at, created_at) DESC
LIMIT @lim",
                    new MySqlParameter("@lim", take));
                foreach (DataRow row in dt.Rows)
                    list.Add(Map(row));
            }
            return list;
        }

        public IList<ArticleRecord> GetAllAdmin()
        {
            var list = new List<ArticleRecord>();
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT * FROM tbl_article
ORDER BY updated_at DESC, article_id DESC");
                foreach (DataRow row in dt.Rows)
                    list.Add(Map(row));
            }
            return list;
        }

        public ArticleRecord GetById(int id)
        {
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable("SELECT * FROM tbl_article WHERE article_id=@id LIMIT 1",
                    new MySqlParameter("@id", id));
                if (dt.Rows.Count == 0) return null;
                return Map(dt.Rows[0]);
            }
        }

        public ArticleRecord GetBySlug(string slug, bool publishedOnly)
        {
            slug = (slug ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(slug)) return null;
            using (var db = new DatabaseHelper())
            {
                var sql = "SELECT * FROM tbl_article WHERE slug=@s";
                if (publishedOnly) sql += " AND is_published=1";
                sql += " LIMIT 1";
                var dt = db.ExecuteDataTable(sql, new MySqlParameter("@s", slug));
                if (dt.Rows.Count == 0) return null;
                return Map(dt.Rows[0]);
            }
        }

        public bool Save(ArticleEditModel model, int? adminUserId, string uploadRootPhysical, string uploadVirtualRoot, out string error)
        {
            error = null;
            if (model == null) { error = "Invalid article."; return false; }
            model.Title = (model.Title ?? "").Trim();
            model.Summary = (model.Summary ?? "").Trim();
            model.BodyHtml = (model.BodyHtml ?? "").Trim();
            model.Slug = string.IsNullOrWhiteSpace(model.Slug) ? Slugify(model.Title) : Slugify(model.Slug);

            if (string.IsNullOrWhiteSpace(model.Title)) { error = "Title is required."; return false; }
            if (string.IsNullOrWhiteSpace(model.BodyHtml)) { error = "Content is required."; return false; }
            if (string.IsNullOrWhiteSpace(model.Slug)) { error = "Slug could not be generated."; return false; }

            string imageUrl = model.ImageUrl;
            if (model.ImageFile != null && model.ImageFile.ContentLength > 0)
            {
                string saved;
                if (!TrySaveImage(model.ImageFile, uploadRootPhysical, uploadVirtualRoot, out saved, out error))
                    return false;
                imageUrl = saved;
            }

            using (var db = new DatabaseHelper())
            {
                var clash = Convert.ToInt32(db.ExecuteScalar(
                    "SELECT COUNT(*) FROM tbl_article WHERE slug=@s AND article_id<>@id",
                    new MySqlParameter("@s", model.Slug),
                    new MySqlParameter("@id", model.ArticleId)));
                if (clash > 0)
                {
                    error = "Another article already uses this slug.";
                    return false;
                }

                if (model.ArticleId <= 0)
                {
                    db.ExecuteNonQuery(@"
INSERT INTO tbl_article
(title, slug, summary, body_html, image_url, is_published, published_at, created_by)
VALUES (@t,@s,@sum,@b,@img,@pub,IF(@pub=1,NOW(),NULL),@by)",
                        new MySqlParameter("@t", model.Title),
                        new MySqlParameter("@s", model.Slug),
                        new MySqlParameter("@sum", string.IsNullOrWhiteSpace(model.Summary) ? (object)DBNull.Value : model.Summary),
                        new MySqlParameter("@b", model.BodyHtml),
                        new MySqlParameter("@img", string.IsNullOrWhiteSpace(imageUrl) ? (object)DBNull.Value : imageUrl),
                        new MySqlParameter("@pub", model.IsPublished ? 1 : 0),
                        new MySqlParameter("@by", adminUserId.HasValue ? (object)adminUserId.Value : DBNull.Value));
                    model.ArticleId = Convert.ToInt32(db.ExecuteScalar("SELECT LAST_INSERT_ID()"));
                }
                else
                {
                    db.ExecuteNonQuery(@"
UPDATE tbl_article
SET title=@t, slug=@s, summary=@sum, body_html=@b, image_url=@img,
    is_published=@pub,
    published_at=CASE
      WHEN @pub=1 AND (published_at IS NULL OR is_published=0) THEN NOW()
      WHEN @pub=0 THEN NULL
      ELSE published_at
    END,
    updated_at=NOW()
WHERE article_id=@id",
                        new MySqlParameter("@t", model.Title),
                        new MySqlParameter("@s", model.Slug),
                        new MySqlParameter("@sum", string.IsNullOrWhiteSpace(model.Summary) ? (object)DBNull.Value : model.Summary),
                        new MySqlParameter("@b", model.BodyHtml),
                        new MySqlParameter("@img", string.IsNullOrWhiteSpace(imageUrl) ? (object)DBNull.Value : imageUrl),
                        new MySqlParameter("@pub", model.IsPublished ? 1 : 0),
                        new MySqlParameter("@id", model.ArticleId));
                }
            }
            model.ImageUrl = imageUrl;
            return true;
        }

        public bool Delete(int articleId, out string error)
        {
            error = null;
            using (var db = new DatabaseHelper())
            {
                var n = db.ExecuteNonQuery("DELETE FROM tbl_article WHERE article_id=@id",
                    new MySqlParameter("@id", articleId));
                if (n == 0) { error = "Article not found."; return false; }
            }
            return true;
        }

        public static string Slugify(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            var s = input.Trim().ToLowerInvariant();
            s = Regex.Replace(s, @"[^a-z0-9\s-]", "");
            s = Regex.Replace(s, @"\s+", "-");
            s = Regex.Replace(s, @"-+", "-").Trim('-');
            if (s.Length > 200) s = s.Substring(0, 200).Trim('-');
            return s;
        }

        private static bool TrySaveImage(HttpPostedFileBase file, string uploadRootPhysical, string uploadVirtualRoot, out string virtualPath, out string error)
        {
            virtualPath = null;
            error = null;
            if (file == null || file.ContentLength <= 0) { error = "Empty image."; return false; }
            if (file.ContentLength > 2 * 1024 * 1024) { error = "Image must be 2 MB or smaller."; return false; }

            var ext = Path.GetExtension(file.FileName ?? "");
            if (!AllowedImageExt.Contains(ext))
            {
                error = "Allowed image types: jpg, png, gif, webp.";
                return false;
            }

            // Reject non-image content types when provided
            var ct = (file.ContentType ?? "").ToLowerInvariant();
            if (!string.IsNullOrEmpty(ct) && !ct.StartsWith("image/", StringComparison.Ordinal))
            {
                error = "File must be an image.";
                return false;
            }

            Directory.CreateDirectory(uploadRootPhysical);
            var name = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                + "-" + Guid.NewGuid().ToString("N").Substring(0, 8) + ext.ToLowerInvariant();
            var physical = Path.Combine(uploadRootPhysical, name);
            file.SaveAs(physical);
            virtualPath = (uploadVirtualRoot.TrimEnd('/') + "/" + name).Replace('\\', '/');
            return true;
        }

        private static ArticleRecord Map(DataRow row)
        {
            return new ArticleRecord
            {
                ArticleId = Convert.ToInt32(row["article_id"]),
                Title = row["title"] as string,
                Slug = row["slug"] as string,
                Summary = row["summary"] == DBNull.Value ? null : row["summary"] as string,
                BodyHtml = row["body_html"] as string,
                ImageUrl = row["image_url"] == DBNull.Value ? null : row["image_url"] as string,
                IsPublished = Convert.ToInt32(row["is_published"]) == 1,
                PublishedAt = row["published_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["published_at"]),
                CreatedBy = row["created_by"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["created_by"]),
                CreatedAt = Convert.ToDateTime(row["created_at"]),
                UpdatedAt = Convert.ToDateTime(row["updated_at"])
            };
        }
    }
}
