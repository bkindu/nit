using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using MySql.Data.MySqlClient;
using NirmalaAstrology_Web.DAL;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    public sealed class AppointmentService
    {
        private static readonly TimeSpan[] DefaultSlots =
        {
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            TimeSpan.FromHours(12),
            TimeSpan.FromHours(15),
            TimeSpan.FromHours(16),
            TimeSpan.FromHours(17),
            TimeSpan.FromHours(18)
        };

        public AppointmentService()
        {
            new CommerceSchemaService().EnsureSchemaAndSeed();
        }

        public IList<AppServiceCatalogItem> GetActiveServices()
        {
            var list = new List<AppServiceCatalogItem>();
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT service_id, service_code, service_name, description, duration_minutes, price_inr, is_active
FROM tbl_service WHERE is_active=1 ORDER BY sort_order, service_name");
                foreach (DataRow row in dt.Rows)
                    list.Add(MapService(row));
            }
            return list;
        }

        public AppServiceCatalogItem GetService(int serviceId)
        {
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT service_id, service_code, service_name, description, duration_minutes, price_inr, is_active
FROM tbl_service WHERE service_id=@id LIMIT 1",
                    new MySqlParameter("@id", serviceId));
                if (dt.Rows.Count == 0) return null;
                return MapService(dt.Rows[0]);
            }
        }

        public IList<string> GetAvailableTimeSlots(DateTime date)
        {
            var booked = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT appointment_time FROM tbl_appointment
WHERE appointment_date=@d
  AND appointment_status IN (@s1,@s2,@s3)",
                    new MySqlParameter("@d", date.Date),
                    new MySqlParameter("@s1", AppointmentStatuses.PendingPayment),
                    new MySqlParameter("@s2", AppointmentStatuses.PendingApproval),
                    new MySqlParameter("@s3", AppointmentStatuses.Approved));
                foreach (DataRow row in dt.Rows)
                {
                    var t = (TimeSpan)row["appointment_time"];
                    booked.Add(FormatTime(t));
                }
            }

            return DefaultSlots
                .Select(FormatTime)
                .Where(t => !booked.Contains(t))
                .ToList();
        }

        public bool IsSlotAvailable(DateTime date, TimeSpan time, int? excludeAppointmentId = null)
        {
            using (var db = new DatabaseHelper())
            {
                var sql = @"
SELECT COUNT(*) FROM tbl_appointment
WHERE appointment_date=@d AND appointment_time=@t
  AND appointment_status IN (@s1,@s2,@s3)";
                if (excludeAppointmentId.HasValue)
                    sql += " AND appointment_id<>@ex";

                var parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@d", date.Date),
                    new MySqlParameter("@t", time),
                    new MySqlParameter("@s1", AppointmentStatuses.PendingPayment),
                    new MySqlParameter("@s2", AppointmentStatuses.PendingApproval),
                    new MySqlParameter("@s3", AppointmentStatuses.Approved)
                };
                if (excludeAppointmentId.HasValue)
                    parameters.Add(new MySqlParameter("@ex", excludeAppointmentId.Value));

                return Convert.ToInt32(db.ExecuteScalar(sql, parameters.ToArray())) == 0;
            }
        }

        public AppointmentRecord CreateBooking(
            int userId,
            int serviceId,
            DateTime date,
            TimeSpan time,
            string customerName,
            string customerEmail,
            string customerMobile,
            string notes,
            out string error)
        {
            error = null;
            var service = GetService(serviceId);
            if (service == null || !service.IsActive) { error = "Selected service is not available."; return null; }
            if (date.Date < DateTime.Today) { error = "Appointment date cannot be in the past."; return null; }
            if (!IsSlotAvailable(date, time)) { error = "That date/time is already booked. Please choose another slot."; return null; }

            var bookingRef = "NA-A-" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                + "-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpperInvariant();

            using (var db = new DatabaseHelper())
            {
                // Re-check inside insert path for race
                if (!IsSlotAvailable(date, time))
                {
                    error = "That date/time was just taken. Please choose another slot.";
                    return null;
                }

                db.ExecuteNonQuery(@"
INSERT INTO tbl_appointment
(booking_ref, user_id, service_id, appointment_date, appointment_time, amount, currency,
 payment_status, appointment_status, customer_name, customer_email, customer_mobile, notes)
VALUES
(@r,@u,@s,@d,@t,@a,'INR',@ps,@as,@cn,@ce,@cm,@n)",
                    new MySqlParameter("@r", bookingRef),
                    new MySqlParameter("@u", userId),
                    new MySqlParameter("@s", serviceId),
                    new MySqlParameter("@d", date.Date),
                    new MySqlParameter("@t", time),
                    new MySqlParameter("@a", service.PriceInr),
                    new MySqlParameter("@ps", PaymentStatuses.Pending),
                    new MySqlParameter("@as", AppointmentStatuses.PendingPayment),
                    new MySqlParameter("@cn", (customerName ?? "").Trim()),
                    new MySqlParameter("@ce", (customerEmail ?? "").Trim()),
                    new MySqlParameter("@cm", string.IsNullOrWhiteSpace(customerMobile) ? (object)DBNull.Value : customerMobile.Trim()),
                    new MySqlParameter("@n", string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes.Trim()));

                var id = Convert.ToInt32(db.ExecuteScalar("SELECT appointment_id FROM tbl_appointment WHERE booking_ref=@r", new MySqlParameter("@r", bookingRef)));

                var orderId = "ORD-" + bookingRef;
                db.ExecuteNonQuery(@"
INSERT INTO tbl_payment
(user_id, appointment_id, amount, currency, gateway, order_id, payment_status, payment_method)
VALUES (@u,@a,@amt,'INR',@g,@o,@ps,'UPI')",
                    new MySqlParameter("@u", userId),
                    new MySqlParameter("@a", id),
                    new MySqlParameter("@amt", service.PriceInr),
                    new MySqlParameter("@g", CommerceSchemaService.PaymentGateway),
                    new MySqlParameter("@o", orderId),
                    new MySqlParameter("@ps", PaymentStatuses.Pending));

                return GetById(id);
            }
        }

        public AppointmentRecord GetById(int appointmentId)
        {
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT a.*, s.service_name
FROM tbl_appointment a
JOIN tbl_service s ON s.service_id=a.service_id
WHERE a.appointment_id=@id LIMIT 1",
                    new MySqlParameter("@id", appointmentId));
                if (dt.Rows.Count == 0) return null;
                return MapAppointment(dt.Rows[0]);
            }
        }

        public AppointmentRecord GetByBookingRef(string bookingRef)
        {
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT a.*, s.service_name
FROM tbl_appointment a
JOIN tbl_service s ON s.service_id=a.service_id
WHERE a.booking_ref=@r LIMIT 1",
                    new MySqlParameter("@r", bookingRef));
                if (dt.Rows.Count == 0) return null;
                return MapAppointment(dt.Rows[0]);
            }
        }

        public IList<AppointmentRecord> GetForUser(int userId)
        {
            var list = new List<AppointmentRecord>();
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT a.*, s.service_name
FROM tbl_appointment a
JOIN tbl_service s ON s.service_id=a.service_id
WHERE a.user_id=@u
ORDER BY a.appointment_date DESC, a.appointment_time DESC",
                    new MySqlParameter("@u", userId));
                foreach (DataRow row in dt.Rows)
                    list.Add(MapAppointment(row));
            }
            return list;
        }

        public IList<AppointmentRecord> SearchAdmin(string status, string keyword, int page, int pageSize, out int total)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, Math.Min(100, pageSize));
            var list = new List<AppointmentRecord>();
            using (var db = new DatabaseHelper())
            {
                var where = "WHERE 1=1";
                var parameters = new List<MySqlParameter>();
                if (!string.IsNullOrWhiteSpace(status))
                {
                    where += " AND a.appointment_status=@st";
                    parameters.Add(new MySqlParameter("@st", status.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += " AND (a.booking_ref LIKE @k OR a.customer_name LIKE @k OR a.customer_email LIKE @k OR a.customer_mobile LIKE @k)";
                    parameters.Add(new MySqlParameter("@k", "%" + keyword.Trim() + "%"));
                }

                total = Convert.ToInt32(db.ExecuteScalar(
                    "SELECT COUNT(*) FROM tbl_appointment a " + where,
                    parameters.ToArray()));

                parameters.Add(new MySqlParameter("@lim", pageSize));
                parameters.Add(new MySqlParameter("@off", (page - 1) * pageSize));
                var dt = db.ExecuteDataTable(@"
SELECT a.*, s.service_name
FROM tbl_appointment a
JOIN tbl_service s ON s.service_id=a.service_id
" + where + @"
ORDER BY a.created_at DESC
LIMIT @lim OFFSET @off",
                    parameters.ToArray());
                foreach (DataRow row in dt.Rows)
                    list.Add(MapAppointment(row));
            }
            return list;
        }

        public PaymentRecord GetPaymentForAppointment(int appointmentId)
        {
            using (var db = new DatabaseHelper())
            {
                var dt = db.ExecuteDataTable(@"
SELECT * FROM tbl_payment WHERE appointment_id=@id ORDER BY payment_id DESC LIMIT 1",
                    new MySqlParameter("@id", appointmentId));
                if (dt.Rows.Count == 0) return null;
                return MapPayment(dt.Rows[0]);
            }
        }

        /// <summary>
        /// User submits UPI transaction reference. Idempotent on same order+txn.
        /// Does NOT mark Paid until Admin verifies (server-side), but moves appointment to PendingApproval.
        /// </summary>
        public bool SubmitUpiPayment(int appointmentId, int userId, string transactionRef, out string error)
        {
            error = null;
            transactionRef = (transactionRef ?? "").Trim();
            if (!IsValidTransactionRef(transactionRef))
            {
                error = "Enter a valid UPI / bank transaction reference (6–64 letters, numbers, - _ / .).";
                return false;
            }

            var appt = GetById(appointmentId);
            if (appt == null || appt.UserId != userId) { error = "Appointment not found."; return false; }

            // Already paid — never accept another client "paid" claim
            if (string.Equals(appt.PaymentStatus, PaymentStatuses.Paid, StringComparison.OrdinalIgnoreCase))
            {
                error = "This appointment is already marked Paid.";
                return false;
            }

            if (appt.AppointmentStatus != AppointmentStatuses.PendingPayment
                && appt.AppointmentStatus != AppointmentStatuses.PendingApproval)
            {
                error = "This appointment cannot accept payment in its current status.";
                return false;
            }

            using (var db = new DatabaseHelper())
            {
                var existingTxn = Convert.ToInt32(db.ExecuteScalar(
                    "SELECT COUNT(*) FROM tbl_payment WHERE transaction_ref=@t AND appointment_id<>@a",
                    new MySqlParameter("@t", transactionRef),
                    new MySqlParameter("@a", appointmentId)));
                if (existingTxn > 0)
                {
                    error = "This transaction reference was already used.";
                    return false;
                }

                var pay = GetPaymentForAppointment(appointmentId);
                if (pay == null) { error = "Payment record missing."; return false; }

                // Amount always from server-side payment row — never from client
                if (pay.Amount != appt.Amount)
                {
                    error = "Payment amount mismatch. Contact support.";
                    return false;
                }

                if (pay.PaymentStatus == PaymentStatuses.Paid
                    || pay.PaymentStatus == PaymentStatuses.AwaitingVerification)
                {
                    // Idempotent repeat submit
                    return true;
                }

                // Moves to AwaitingVerification only — never Paid from this path
                db.ExecuteNonQuery(@"
UPDATE tbl_payment
SET transaction_ref=@t, payment_status=@ps, payment_method='UPI', updated_at=NOW()
WHERE payment_id=@id AND payment_status<>@paid",
                    new MySqlParameter("@t", transactionRef),
                    new MySqlParameter("@ps", PaymentStatuses.AwaitingVerification),
                    new MySqlParameter("@paid", PaymentStatuses.Paid),
                    new MySqlParameter("@id", pay.PaymentId));

                db.ExecuteNonQuery(@"
UPDATE tbl_appointment
SET payment_status=@ps, appointment_status=@as, updated_at=NOW()
WHERE appointment_id=@id AND payment_status<>@paid",
                    new MySqlParameter("@ps", PaymentStatuses.AwaitingVerification),
                    new MySqlParameter("@as", AppointmentStatuses.PendingApproval),
                    new MySqlParameter("@paid", PaymentStatuses.Paid),
                    new MySqlParameter("@id", appointmentId));
            }
            return true;
        }

        public static bool IsValidTransactionRef(string transactionRef)
        {
            if (string.IsNullOrWhiteSpace(transactionRef)) return false;
            if (transactionRef.Length < 6 || transactionRef.Length > 64) return false;
            foreach (var c in transactionRef)
            {
                if (!(char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '/' || c == '.'))
                    return false;
            }
            return true;
        }

        public bool AdminVerifyPayment(int appointmentId, int adminUserId, bool markPaid, out string error)
        {
            error = null;
            var appt = GetById(appointmentId);
            if (appt == null) { error = "Appointment not found."; return false; }
            var pay = GetPaymentForAppointment(appointmentId);
            if (pay == null) { error = "Payment not found."; return false; }

            using (var db = new DatabaseHelper())
            {
                if (markPaid)
                {
                    db.ExecuteNonQuery(@"
UPDATE tbl_payment SET payment_status=@ps, gateway_response_ref=@g, updated_at=NOW() WHERE payment_id=@id",
                        new MySqlParameter("@ps", PaymentStatuses.Paid),
                        new MySqlParameter("@g", "verified-by-" + adminUserId),
                        new MySqlParameter("@id", pay.PaymentId));
                    db.ExecuteNonQuery(@"
UPDATE tbl_appointment SET payment_status=@ps, updated_at=NOW() WHERE appointment_id=@id",
                        new MySqlParameter("@ps", PaymentStatuses.Paid),
                        new MySqlParameter("@id", appointmentId));
                }
                else
                {
                    db.ExecuteNonQuery(@"
UPDATE tbl_payment SET payment_status=@ps, updated_at=NOW() WHERE payment_id=@id",
                        new MySqlParameter("@ps", PaymentStatuses.Failed),
                        new MySqlParameter("@id", pay.PaymentId));
                    db.ExecuteNonQuery(@"
UPDATE tbl_appointment SET payment_status=@ps, appointment_status=@as, updated_at=NOW() WHERE appointment_id=@id",
                        new MySqlParameter("@ps", PaymentStatuses.Failed),
                        new MySqlParameter("@as", AppointmentStatuses.PendingPayment),
                        new MySqlParameter("@id", appointmentId));
                }
            }
            return true;
        }

        public bool AdminSetStatus(int appointmentId, int adminUserId, string newStatus, string adminNotes, out string error)
        {
            error = null;
            if (!AppointmentStatuses.IsKnown(newStatus)) { error = "Invalid status."; return false; }
            var appt = GetById(appointmentId);
            if (appt == null) { error = "Appointment not found."; return false; }

            using (var db = new DatabaseHelper())
            {
                if (string.Equals(newStatus, AppointmentStatuses.Approved, StringComparison.OrdinalIgnoreCase))
                {
                    db.ExecuteNonQuery(@"
UPDATE tbl_appointment
SET appointment_status=@as, admin_notes=@n, approved_at=NOW(), approved_by=@by, updated_at=NOW()
WHERE appointment_id=@id",
                        new MySqlParameter("@as", AppointmentStatuses.Approved),
                        new MySqlParameter("@n", (object)adminNotes ?? DBNull.Value),
                        new MySqlParameter("@by", adminUserId),
                        new MySqlParameter("@id", appointmentId));
                }
                else
                {
                    db.ExecuteNonQuery(@"
UPDATE tbl_appointment
SET appointment_status=@as, admin_notes=@n, updated_at=NOW()
WHERE appointment_id=@id",
                        new MySqlParameter("@as", newStatus),
                        new MySqlParameter("@n", (object)adminNotes ?? DBNull.Value),
                        new MySqlParameter("@id", appointmentId));
                }
            }
            return true;
        }

        private static AppServiceCatalogItem MapService(DataRow row)
        {
            return new AppServiceCatalogItem
            {
                ServiceId = Convert.ToInt32(row["service_id"]),
                ServiceCode = Convert.ToString(row["service_code"]),
                ServiceName = Convert.ToString(row["service_name"]),
                Description = Convert.ToString(row["description"]),
                DurationMinutes = Convert.ToInt32(row["duration_minutes"]),
                PriceInr = Convert.ToDecimal(row["price_inr"]),
                IsActive = Convert.ToInt32(row["is_active"]) == 1
            };
        }

        private static AppointmentRecord MapAppointment(DataRow row)
        {
            return new AppointmentRecord
            {
                AppointmentId = Convert.ToInt32(row["appointment_id"]),
                BookingRef = Convert.ToString(row["booking_ref"]),
                UserId = Convert.ToInt32(row["user_id"]),
                ServiceId = Convert.ToInt32(row["service_id"]),
                ServiceName = row.Table.Columns.Contains("service_name") ? Convert.ToString(row["service_name"]) : null,
                AppointmentDate = Convert.ToDateTime(row["appointment_date"]),
                AppointmentTime = (TimeSpan)row["appointment_time"],
                Amount = Convert.ToDecimal(row["amount"]),
                Currency = Convert.ToString(row["currency"]),
                PaymentStatus = Convert.ToString(row["payment_status"]),
                AppointmentStatus = Convert.ToString(row["appointment_status"]),
                CustomerName = Convert.ToString(row["customer_name"]),
                CustomerEmail = Convert.ToString(row["customer_email"]),
                CustomerMobile = Convert.ToString(row["customer_mobile"]),
                Notes = Convert.ToString(row["notes"]),
                AdminNotes = Convert.ToString(row["admin_notes"]),
                ApprovedAt = row["approved_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["approved_at"]),
                ApprovedBy = row["approved_by"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["approved_by"]),
                CreatedAt = Convert.ToDateTime(row["created_at"]),
                UpdatedAt = Convert.ToDateTime(row["updated_at"])
            };
        }

        private static PaymentRecord MapPayment(DataRow row)
        {
            return new PaymentRecord
            {
                PaymentId = Convert.ToInt32(row["payment_id"]),
                UserId = Convert.ToInt32(row["user_id"]),
                AppointmentId = row["appointment_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["appointment_id"]),
                ReportOrderId = row.Table.Columns.Contains("report_order_id") && row["report_order_id"] != DBNull.Value
                    ? Convert.ToInt32(row["report_order_id"]) : (int?)null,
                Amount = Convert.ToDecimal(row["amount"]),
                Currency = Convert.ToString(row["currency"]),
                Gateway = Convert.ToString(row["gateway"]),
                OrderId = Convert.ToString(row["order_id"]),
                TransactionRef = Convert.ToString(row["transaction_ref"]),
                PaymentStatus = Convert.ToString(row["payment_status"]),
                PaymentMethod = Convert.ToString(row["payment_method"]),
                GatewayResponseRef = Convert.ToString(row["gateway_response_ref"]),
                CreatedAt = Convert.ToDateTime(row["created_at"]),
                UpdatedAt = Convert.ToDateTime(row["updated_at"])
            };
        }

        public static string FormatTime(TimeSpan t)
        {
            var dt = DateTime.Today.Add(t);
            return dt.ToString("hh:mm tt", CultureInfo.InvariantCulture);
        }

        public static bool TryParseTime(string text, out TimeSpan time)
        {
            time = default(TimeSpan);
            if (string.IsNullOrWhiteSpace(text)) return false;
            DateTime dt;
            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                time = dt.TimeOfDay;
                return true;
            }
            if (DateTime.TryParse(text, out dt))
            {
                time = dt.TimeOfDay;
                return true;
            }
            return false;
        }
    }
}
