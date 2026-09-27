using System;

using System.Globalization;

using System.Linq;

using System.Text.RegularExpressions;

using System.Web.Mvc;

using NirmalaAstrology_Web.Models;

using NirmalaAstrology_Web.Services;

using NirmalaAstrology_Web.ViewModels;



namespace NirmalaAstrology_Web.Controllers

{

    public class AppointmentController : Controller

    {

        private readonly AppointmentService _appointments = new AppointmentService();

        private readonly AuthService _auth = new AuthService();

        private readonly EmailService _email = new EmailService();



        [HttpGet]

        [AllowAnonymous]

        public ActionResult Index()

        {

            ViewBag.Title = "Book Appointment";

            var vm = BuildBookModel();

            return View(vm);

        }



        [HttpPost]

        [Authorize]

        [ValidateAntiForgeryToken]

        public ActionResult Index(AppointmentBookViewModel model)

        {

            ViewBag.Title = "Book Appointment";

            model = model ?? new AppointmentBookViewModel();

            model.Services = _appointments.GetActiveServices();



            TimeSpan time;

            if (!_appointments.GetActiveServices().Any(s => s.ServiceId == model.ServiceId))

                ModelState.AddModelError("ServiceId", "Select a service.");

            if (!AppointmentService.TryParseTime(model.AppointmentTime, out time))

                ModelState.AddModelError("AppointmentTime", "Select a valid time slot.");

            if (string.IsNullOrWhiteSpace(model.CustomerName))

                ModelState.AddModelError("CustomerName", "Name is required.");

            if (string.IsNullOrWhiteSpace(model.CustomerEmail))

                ModelState.AddModelError("CustomerEmail", "Email is required.");



            var service = _appointments.GetService(model.ServiceId);

            if (service != null)

            {

                model.SelectedPrice = service.PriceInr;

                model.SelectedServiceName = service.ServiceName;

            }



            if (!ModelState.IsValid) return View(model);



            var user = _auth.GetByUsername(User.Identity.Name);

            if (user == null) return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index") });



            string error;

            var appt = _appointments.CreateBooking(

                user.UserId,

                model.ServiceId,

                model.AppointmentDate.Date,

                time,

                model.CustomerName,

                model.CustomerEmail,

                model.CustomerMobile,

                model.Notes,

                out error);



            if (appt == null)

            {

                ModelState.AddModelError("", error ?? "Unable to create booking.");

                return View(model);

            }



            var payment = _appointments.GetPaymentForAppointment(appt.AppointmentId);

            AppointmentNotifyHelper.NotifyCustomerAndAdmin(

                _email,

                appt,

                payment,

                "Booking received — payment pending",

                "Nirmala Astrology — booking created (" + appt.BookingRef + ")",

                "New appointment booking — " + appt.BookingRef,

                "Please complete UPI payment and submit your transaction reference. Appointment is not confirmed until admin verifies payment and approves.");



            return RedirectToAction("Pay", new { id = appt.AppointmentId });

        }



        [HttpGet]

        [Authorize]

        public ActionResult Pay(int id)

        {

            ViewBag.Title = "Appointment Payment";

            var user = _auth.GetByUsername(User.Identity.Name);

            var appt = _appointments.GetById(id);

            if (user == null || appt == null || appt.UserId != user.UserId)

                return new HttpNotFoundResult();



            // Never trust client for amount — always from DB

            return View(new AppointmentPaymentViewModel

            {

                Appointment = appt,

                Payment = _appointments.GetPaymentForAppointment(id),

                UpiId = CommerceSchemaService.UpiId

            });

        }



        [HttpPost]

        [Authorize]

        [ValidateAntiForgeryToken]

        public ActionResult Pay(int id, string transactionRef)

        {

            ViewBag.Title = "Appointment Payment";

            var user = _auth.GetByUsername(User.Identity.Name);

            var appt = _appointments.GetById(id);

            if (user == null || appt == null || appt.UserId != user.UserId)

                return new HttpNotFoundResult();



            transactionRef = SanitizeTxnRef(transactionRef);



            string error;

            if (!_appointments.SubmitUpiPayment(id, user.UserId, transactionRef, out error))

            {

                ModelState.AddModelError("", error ?? "Payment submission failed.");

                return View(new AppointmentPaymentViewModel

                {

                    Appointment = appt,

                    Payment = _appointments.GetPaymentForAppointment(id),

                    UpiId = CommerceSchemaService.UpiId,

                    TransactionRef = transactionRef

                });

            }



            appt = _appointments.GetById(id);

            var payment = _appointments.GetPaymentForAppointment(id);

            AppointmentNotifyHelper.NotifyCustomerAndAdmin(

                _email,

                appt,

                payment,

                "Payment submitted — pending admin verification",

                "Nirmala Astrology — payment submitted (" + appt.BookingRef + ")",

                "UPI payment awaiting verification — " + appt.BookingRef,

                "Payment is Awaiting Verification. Status is not Paid until an admin confirms the UPI transaction.");



            TempData["SuccessMessage"] = "Payment reference submitted. Awaiting admin verification and approval.";

            return RedirectToAction("Details", new { id });

        }



        [HttpGet]

        [Authorize]

        public ActionResult Details(int id)

        {

            ViewBag.Title = "Appointment Details";

            var user = _auth.GetByUsername(User.Identity.Name);

            var appt = _appointments.GetById(id);

            if (user == null || appt == null) return new HttpNotFoundResult();

            if (appt.UserId != user.UserId && !User.IsInRole(AuthService.RoleAdmin))

                return new HttpUnauthorizedResult();



            ViewBag.Payment = _appointments.GetPaymentForAppointment(id);

            return View(appt);

        }



        [HttpGet]

        [AllowAnonymous]

        public JsonResult AvailableSlots(string date)

        {

            DateTime d;

            if (!DateTime.TryParse(date, out d))

                return Json(new string[0], JsonRequestBehavior.AllowGet);

            var slots = _appointments.GetAvailableTimeSlots(d.Date);

            return Json(slots, JsonRequestBehavior.AllowGet);

        }



        private AppointmentBookViewModel BuildBookModel()

        {

            var vm = new AppointmentBookViewModel { Services = _appointments.GetActiveServices() };

            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)

            {

                var user = _auth.GetByUsername(User.Identity.Name);

                if (user != null)

                {

                    vm.CustomerName = user.DisplayName;

                    vm.CustomerEmail = user.Email;

                    vm.CustomerMobile = user.Mobile;

                }

            }

            if (vm.Services.Count > 0)

            {

                vm.ServiceId = vm.Services[0].ServiceId;

                vm.SelectedPrice = vm.Services[0].PriceInr;

                vm.SelectedServiceName = vm.Services[0].ServiceName;

            }

            return vm;

        }



        /// <summary>Allow only safe UPI/bank reference characters; strip spaces.</summary>

        private static string SanitizeTxnRef(string raw)

        {

            if (string.IsNullOrWhiteSpace(raw)) return "";

            var t = raw.Trim();

            t = Regex.Replace(t, @"\s+", "");

            if (t.Length > 64) t = t.Substring(0, 64);

            return t;

        }

    }

}


