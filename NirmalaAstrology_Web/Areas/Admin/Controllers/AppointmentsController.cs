using System;

using System.Web.Mvc;

using NirmalaAstrology_Web.Filters;

using NirmalaAstrology_Web.Models;

using NirmalaAstrology_Web.Services;



namespace NirmalaAstrology_Web.Areas.Admin.Controllers

{

    [AdminAuthorize]

    public class AppointmentsController : Controller

    {

        private readonly AppointmentService _appointments = new AppointmentService();

        private readonly AuthService _auth = new AuthService();

        private readonly EmailService _email = new EmailService();



        [HttpGet]

        public ActionResult Index(string status, string q, int page = 1)

        {

            ViewBag.Title = "Appointments";

            int total;

            var list = _appointments.SearchAdmin(status, q, page, 25, out total);

            ViewBag.Status = status;

            ViewBag.Query = q;

            ViewBag.Page = page;

            ViewBag.Total = total;

            ViewBag.PageSize = 25;

            return View(list);

        }



        [HttpGet]

        public ActionResult Details(int id)

        {

            ViewBag.Title = "Appointment Details";

            var appt = _appointments.GetById(id);

            if (appt == null) return HttpNotFound();

            ViewBag.Payment = _appointments.GetPaymentForAppointment(id);

            return View(appt);

        }



        [HttpPost]

        [ValidateAntiForgeryToken]

        public ActionResult VerifyPayment(int id, bool markPaid)

        {

            var admin = _auth.GetByUsername(User.Identity.Name);

            string error;

            if (!_appointments.AdminVerifyPayment(id, admin != null ? admin.UserId : 0, markPaid, out error))

            {

                TempData["ErrorMessage"] = error;

                return RedirectToAction("Details", new { id });

            }



            var appt = _appointments.GetById(id);

            var payment = _appointments.GetPaymentForAppointment(id);

            if (appt != null)

            {

                if (markPaid)

                {

                    AppointmentNotifyHelper.NotifyCustomerAndAdmin(

                        _email,

                        appt,

                        payment,

                        "Payment verified — marked Paid",

                        "Nirmala Astrology — payment confirmed (" + appt.BookingRef + ")",

                        "Payment verified by admin — " + appt.BookingRef,

                        "Your UPI payment has been verified. Appointment still needs admin approval if not already approved.");

                }

                else

                {

                    AppointmentNotifyHelper.NotifyCustomerAndAdmin(

                        _email,

                        appt,

                        payment,

                        "Payment rejected",

                        "Nirmala Astrology — payment not verified (" + appt.BookingRef + ")",

                        "Payment rejected by admin — " + appt.BookingRef,

                        "The submitted UPI reference was not accepted. Please pay again and submit a correct transaction reference.");

                }

            }



            TempData["SuccessMessage"] = markPaid ? "Payment marked as Paid." : "Payment rejected.";

            return RedirectToAction("Details", new { id });

        }



        [HttpPost]

        [ValidateAntiForgeryToken]

        public ActionResult SetStatus(int id, string status, string adminNotes)

        {

            var admin = _auth.GetByUsername(User.Identity.Name);

            string error;

            if (!_appointments.AdminSetStatus(id, admin != null ? admin.UserId : 0, status, adminNotes, out error))

            {

                TempData["ErrorMessage"] = error;

                return RedirectToAction("Details", new { id });

            }



            var appt = _appointments.GetById(id);

            var payment = _appointments.GetPaymentForAppointment(id);

            if (appt != null)

            {

                if (string.Equals(status, AppointmentStatuses.Approved, StringComparison.OrdinalIgnoreCase))

                {

                    AppointmentNotifyHelper.NotifyCustomerAndAdmin(

                        _email,

                        appt,

                        payment,

                        "Appointment approved",

                        "Nirmala Astrology — appointment approved (" + appt.BookingRef + ")",

                        "Appointment approved — " + appt.BookingRef,

                        string.IsNullOrWhiteSpace(adminNotes) ? "Your appointment is confirmed." : adminNotes);

                }

                else if (string.Equals(status, AppointmentStatuses.Rejected, StringComparison.OrdinalIgnoreCase)

                    || string.Equals(status, AppointmentStatuses.Cancelled, StringComparison.OrdinalIgnoreCase))

                {

                    AppointmentNotifyHelper.NotifyCustomerAndAdmin(

                        _email,

                        appt,

                        payment,

                        "Appointment " + status,

                        "Nirmala Astrology — appointment update (" + appt.BookingRef + ")",

                        "Appointment " + status + " — " + appt.BookingRef,

                        string.IsNullOrWhiteSpace(adminNotes) ? ("Status is now " + status + ".") : adminNotes);

                }

            }



            TempData["SuccessMessage"] = "Status updated to " + status + ".";

            return RedirectToAction("Details", new { id });

        }

    }

}


