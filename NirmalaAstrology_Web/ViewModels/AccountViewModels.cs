using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NirmalaAstrology_Web.ViewModels
{
    public sealed class RegisterViewModel
    {
        [Required, StringLength(120)]
        [Display(Name = "Full name")]
        public string DisplayName { get; set; }

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        [Required, StringLength(64, MinimumLength = 3)]
        public string Username { get; set; }

        [Required, StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
    }

    public sealed class ProfileViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        [Required, StringLength(120)]
        public string DisplayName { get; set; }

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        public string[] Roles { get; set; }
    }

    public sealed class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; }

        [Required, StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm new password")]
        public string ConfirmPassword { get; set; }
    }

    public sealed class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }

    public sealed class AppointmentBookViewModel
    {
        public IList<Models.AppServiceCatalogItem> Services { get; set; }
        public int ServiceId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public string Notes { get; set; }
        public decimal SelectedPrice { get; set; }
        public string SelectedServiceName { get; set; }

        public AppointmentBookViewModel()
        {
            Services = new List<Models.AppServiceCatalogItem>();
            AppointmentDate = DateTime.Today.AddDays(1);
        }
    }

    public sealed class AppointmentPaymentViewModel
    {
        public Models.AppointmentRecord Appointment { get; set; }
        public Models.PaymentRecord Payment { get; set; }
        public string UpiId { get; set; }
        public string TransactionRef { get; set; }
    }

    public sealed class PublicReportCatalogItem
    {
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public string Description { get; set; }
        public decimal PriceInr { get; set; }
        public bool PublicAllowed { get; set; }
        /// <summary>True for short/basic public reports — no payment.</summary>
        public bool IsFree { get; set; }
    }
}
