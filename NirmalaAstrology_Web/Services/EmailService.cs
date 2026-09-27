using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Config-driven SMTP (no credentials in code). Failures are logged without secrets.
    /// </summary>
    public sealed class EmailService
    {
        public bool TrySend(string toEmail, string toName, string subject, string htmlBody, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                error = "Recipient email is required.";
                return false;
            }

            try
            {
                var enabled = string.Equals(ConfigurationManager.AppSettings["Email:Enabled"], "true", StringComparison.OrdinalIgnoreCase);
                if (!enabled)
                {
                    error = "Email is disabled. Set Email:Enabled=true in App_Data/smtp.local.config.";
                    System.Diagnostics.Trace.WriteLine("[EmailService] Skipped (Email:Enabled=false). To=" + toEmail + " Subject=" + subject);
                    return false;
                }

                var host = ConfigurationManager.AppSettings["Email:SmtpHost"];
                var portText = ConfigurationManager.AppSettings["Email:SmtpPort"] ?? "587";
                var from = ConfigurationManager.AppSettings["Email:FromAddress"];
                var fromName = ConfigurationManager.AppSettings["Email:FromName"] ?? "Nirmala Astrology";
                var user = ConfigurationManager.AppSettings["Email:SmtpUser"];
                var pass = ConfigurationManager.AppSettings["Email:SmtpPassword"];
                var enableSsl = !string.Equals(ConfigurationManager.AppSettings["Email:EnableSsl"], "false", StringComparison.OrdinalIgnoreCase);

                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                {
                    error = "SMTP is not configured. Fill App_Data/smtp.local.config (host, from, user, password).";
                    return false;
                }

                // Prefer real mailbox as From when FromAddress is still a placeholder.
                if (from.IndexOf("your@", StringComparison.OrdinalIgnoreCase) >= 0
                    || from.IndexOf("YOUR_", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    from = user;
                }

                int port;
                if (!int.TryParse(portText, out port)) port = 587;

                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = enableSsl;
                    smtp.Timeout = 45000;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(user, pass);

                    using (var mail = new MailMessage())
                    {
                        mail.From = new MailAddress(from.Trim(), fromName, Encoding.UTF8);
                        mail.To.Add(new MailAddress(toEmail.Trim(), toName ?? toEmail.Trim(), Encoding.UTF8));
                        mail.Subject = subject ?? "";
                        mail.SubjectEncoding = Encoding.UTF8;
                        mail.Body = htmlBody ?? "";
                        mail.BodyEncoding = Encoding.UTF8;
                        mail.IsBodyHtml = true;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (SmtpException ex)
            {
                error = "Email delivery failed (SMTP). Check host/port/SSL and mailbox password.";
                System.Diagnostics.Trace.WriteLine("[EmailService] SmtpException: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                error = "Email delivery failed.";
                System.Diagnostics.Trace.WriteLine("[EmailService] " + ex.GetType().Name + ": " + ex.Message);
                return false;
            }
        }

        /// <summary>True when Email:Enabled is true (credentials may still be missing).</summary>
        public static bool IsEnabled()
        {
            return string.Equals(ConfigurationManager.AppSettings["Email:Enabled"], "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Branded HTML wrapper with Nirmala Astrology signature (contact from app settings only).
        /// </summary>
        public static string WrapHtml(string title, string innerHtml)
        {
            var phone = ConfigurationManager.AppSettings["Email:PublicPhone"] ?? "98551-10123";
            var address = ConfigurationManager.AppSettings["Email:PublicAddress"]
                ?? "58-A/3, Sidhu Colony, Bhadson Road, Patiala";
            var website = ConfigurationManager.AppSettings["Email:PublicWebsite"] ?? "https://www.nirmalaastrology.com/";
            var fromName = ConfigurationManager.AppSettings["Email:FromName"] ?? "Nirmala Astrology";
            var logoUrl = ConfigurationManager.AppSettings["Email:LogoUrl"]
                ?? "https://www.nirmalaastrology.com/Images/logo.png";

            return "<div style='font-family:Arial,Helvetica,sans-serif;max-width:640px;margin:0 auto;color:#222;line-height:1.45;'>"
                + "<div style='border-bottom:3px solid #8B0000;padding:12px 0 16px;'>"
                + "<img src='" + HtmlEncode(logoUrl) + "' alt='Nirmala Astrology' width='72' height='72' style='display:block;border:0;' />"
                + "<div style='font-size:20px;font-weight:bold;color:#8B0000;margin-top:8px;'>" + HtmlEncode(fromName) + "</div>"
                + "</div>"
                + "<h2 style='color:#8B0000;margin:20px 0 12px;font-size:18px;'>" + HtmlEncode(title) + "</h2>"
                + (innerHtml ?? "")
                + "<div style='margin-top:28px;padding-top:16px;border-top:1px solid #ddd;font-size:13px;color:#444;'>"
                + "<p style='margin:0 0 6px;'><strong>Regards,</strong></p>"
                + "<p style='margin:0 0 4px;'><strong>" + HtmlEncode(fromName) + "</strong></p>"
                + "<p style='margin:0;color:#666;'>" + HtmlEncode(address) + "<br/>"
                + "Ph: " + HtmlEncode(phone) + "<br/>"
                + "<a href='" + HtmlEncode(website) + "' style='color:#8B0000;'>" + HtmlEncode(website) + "</a></p>"
                + "</div>"
                + "<p style='margin-top:16px;font-size:11px;color:#999;'>This is an automated message from Nirmala Astrology. Please do not reply with passwords or card details.</p>"
                + "</div>";
        }

        public static string HtmlEncode(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return System.Web.HttpUtility.HtmlEncode(value);
        }
    }
}
