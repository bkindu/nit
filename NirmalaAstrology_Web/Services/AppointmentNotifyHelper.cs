using System;
using System.Configuration;
using System.Globalization;
using System.Text;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Builds appointment/payment email bodies and notifies customer + admin.
    /// </summary>
    public static class AppointmentNotifyHelper
    {
        public static string AdminNotifyAddress
        {
            get
            {
                var admin = ConfigurationManager.AppSettings["Email:AdminNotify"];
                if (!string.IsNullOrWhiteSpace(admin)) return admin.Trim();
                var contact = ConfigurationManager.AppSettings["Email:ContactTo"];
                if (!string.IsNullOrWhiteSpace(contact)) return contact.Trim();
                return (ConfigurationManager.AppSettings["Email:FromAddress"] ?? "").Trim();
            }
        }

        public static string BuildDetailsHtml(AppointmentRecord appt, PaymentRecord payment, string extraNote)
        {
            var sb = new StringBuilder();
            sb.Append("<table style='border-collapse:collapse;width:100%;font-size:14px;'>");
            Row(sb, "Booking ref", appt.BookingRef);
            Row(sb, "Customer", appt.CustomerName);
            Row(sb, "Email", appt.CustomerEmail);
            Row(sb, "Mobile", appt.CustomerMobile ?? "—");
            Row(sb, "Service", appt.ServiceName);
            Row(sb, "Date", appt.AppointmentDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            Row(sb, "Time", AppointmentService.FormatTime(appt.AppointmentTime));
            Row(sb, "Amount", "₹" + appt.Amount.ToString("0.00", CultureInfo.InvariantCulture) + " " + (appt.Currency ?? "INR"));
            Row(sb, "Appointment status", appt.AppointmentStatus);
            Row(sb, "Payment status", appt.PaymentStatus);
            if (payment != null)
            {
                Row(sb, "Order ID", payment.OrderId);
                Row(sb, "Gateway", payment.Gateway);
                Row(sb, "Payment method", payment.PaymentMethod ?? "UPI");
                Row(sb, "UPI ID", CommerceSchemaService.UpiId);
                Row(sb, "Transaction ref", string.IsNullOrWhiteSpace(payment.TransactionRef) ? "— (not submitted yet)" : payment.TransactionRef);
                Row(sb, "Paid amount", "₹" + payment.Amount.ToString("0.00", CultureInfo.InvariantCulture));
            }
            if (!string.IsNullOrWhiteSpace(appt.Notes))
                Row(sb, "Customer notes", appt.Notes);
            sb.Append("</table>");
            if (!string.IsNullOrWhiteSpace(extraNote))
                sb.Append("<p style='margin-top:16px;'>").Append(EmailService.HtmlEncode(extraNote)).Append("</p>");
            return sb.ToString();
        }

        public static void NotifyCustomerAndAdmin(
            EmailService email,
            AppointmentRecord appt,
            PaymentRecord payment,
            string title,
            string customerSubject,
            string adminSubject,
            string extraNote)
        {
            if (appt == null || email == null) return;
            var inner = BuildDetailsHtml(appt, payment, extraNote);
            var body = EmailService.WrapHtml(title, inner);
            string err;
            if (!string.IsNullOrWhiteSpace(appt.CustomerEmail))
                email.TrySend(appt.CustomerEmail, appt.CustomerName, customerSubject, body, out err);

            var adminTo = AdminNotifyAddress;
            if (!string.IsNullOrWhiteSpace(adminTo))
            {
                var adminBody = EmailService.WrapHtml("[Admin] " + title,
                    "<p><strong>Internal notification</strong> — a booking/payment event occurred.</p>" + inner);
                email.TrySend(adminTo, "Nirmala Admin", adminSubject, adminBody, out err);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("[AppointmentNotify] Admin email skipped — set Email:AdminNotify or Email:ContactTo.");
            }
        }

        private static void Row(StringBuilder sb, string label, string value)
        {
            sb.Append("<tr>")
                .Append("<td style='padding:6px 8px;border-bottom:1px solid #eee;color:#666;width:38%;'>")
                .Append(EmailService.HtmlEncode(label))
                .Append("</td>")
                .Append("<td style='padding:6px 8px;border-bottom:1px solid #eee;'><strong>")
                .Append(EmailService.HtmlEncode(value ?? "—"))
                .Append("</strong></td>")
                .Append("</tr>");
        }
    }
}
