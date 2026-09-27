using System;

namespace NirmalaAstrology_Web.Models
{
    public sealed class AppUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
        public string[] Roles { get; set; }

        public AppUser()
        {
            Roles = new string[0];
        }

        public bool IsInRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role) || Roles == null) return false;
            foreach (var r in Roles)
            {
                if (string.Equals(r, role, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }

    public sealed class ReportDefinition
    {
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public bool PublicAllowed { get; set; }
        public bool AdminAllowed { get; set; }
        public bool IsEnabled { get; set; }
        public int? UsageLimitDaily { get; set; }
        public string Notes { get; set; }
        public decimal PriceInr { get; set; }
        public string ShortDescription { get; set; }
    }
}
