using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NirmalaAstrology_Web.Models
{
    public class ContactForm
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your phone number.")]
        [RegularExpression(@"^[0-9+\-\s\(\)]{7,15}$", ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter a subject.")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter your message.")]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters.")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters long.")]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }

    public class ContactResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public DateTime SubmittedOn { get; set; }
    }
}