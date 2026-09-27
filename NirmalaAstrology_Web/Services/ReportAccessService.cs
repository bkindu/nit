using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Central report permission checks (Public vs Admin). Server-side only.
    /// </summary>
    public sealed class ReportAccessService
    {
        private static readonly object CacheLock = new object();
        private static List<ReportDefinition> _cache;
        private static DateTime _cacheAt = DateTime.MinValue;

        public bool CanAccess(string reportCode, HttpContextBase httpContext)
        {
            if (string.IsNullOrWhiteSpace(reportCode)) return false;
            var def = GetDefinition(reportCode);
            if (def == null || !def.IsEnabled) return false;

            var isAdmin = httpContext != null
                && httpContext.User != null
                && httpContext.User.Identity != null
                && httpContext.User.Identity.IsAuthenticated
                && httpContext.User.IsInRole(AuthService.RoleAdmin);

            if (isAdmin) return def.AdminAllowed;
            return def.PublicAllowed;
        }

        public ReportDefinition GetDefinition(string reportCode)
        {
            foreach (var d in GetAll())
            {
                if (string.Equals(d.ReportCode, reportCode, StringComparison.OrdinalIgnoreCase))
                    return d;
            }
            return null;
        }

        public IList<ReportDefinition> GetAll()
        {
            lock (CacheLock)
            {
                if (_cache != null && (DateTime.UtcNow - _cacheAt).TotalMinutes < 5)
                    return _cache;
            }

            var list = new List<ReportDefinition>();
            try
            {
                new AuthService().EnsureSchemaAndSeed();
                new CommerceSchemaService().EnsureSchemaAndSeed();
                using (var db = new DatabaseHelper())
                {
                    var dt = db.ExecuteDataTable(@"
SELECT report_code, report_name, public_allowed, admin_allowed, is_enabled, usage_limit_daily, notes,
       IFNULL(price_inr,0) AS price_inr, short_description
FROM tbl_report_definition
ORDER BY report_name");
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(new ReportDefinition
                        {
                            ReportCode = Convert.ToString(row["report_code"]),
                            ReportName = Convert.ToString(row["report_name"]),
                            PublicAllowed = Convert.ToInt32(row["public_allowed"]) == 1,
                            AdminAllowed = Convert.ToInt32(row["admin_allowed"]) == 1,
                            IsEnabled = Convert.ToInt32(row["is_enabled"]) == 1,
                            UsageLimitDaily = row["usage_limit_daily"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["usage_limit_daily"]),
                            Notes = Convert.ToString(row["notes"]),
                            PriceInr = row.Table.Columns.Contains("price_inr") ? Convert.ToDecimal(row["price_inr"]) : 0m,
                            ShortDescription = row.Table.Columns.Contains("short_description") ? Convert.ToString(row["short_description"]) : null
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[ReportAccessService] " + ex.Message);
            }

            lock (CacheLock)
            {
                _cache = list;
                _cacheAt = DateTime.UtcNow;
            }
            return list;
        }

        public static void InvalidateCache()
        {
            lock (CacheLock)
            {
                _cache = null;
                _cacheAt = DateTime.MinValue;
            }
        }
    }
}
