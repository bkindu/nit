using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Services;
using NirmalaAstrology_Web.Services.Reports;
using NirmalaAstrology_Web.ViewModels;

namespace NirmalaAstrology_Web.Controllers
{
    /// <summary>
    /// Public limited reports only — never exposes admin DETAILED / Lal Kitab / etc.
    /// Kundli: 2-page (basic info + chart). Numerology: basic core numbers PDF.
    /// </summary>
    [SessionState(SessionStateBehavior.Required)]
    public class PublicReportController : Controller
    {
        private readonly ReportAccessService _access = new ReportAccessService();

        /// <summary>Download the free 2-page PDF from the kundli already calculated on the result screen.</summary>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult DownloadBasicKundli()
        {
            if (!_access.CanAccess("KUNDLI_BASIC", HttpContext))
                return RedirectToAction("AccessDenied", "Auth");

            var birth = BirthDataSessionStore.Get(Session);
            if (birth == null || birth.Year < 1900)
                return RedirectToAction("KundliResult", "Home");

            try
            {
                birth.ReportChoice = "OPT_KUNDLI";
                var workflow = KundliWorkflowCache.GetWorkflow(Session);
                if (workflow == null || workflow.Calculation == null)
                {
                    var tz = birth.Timezone == 0 ? 5.30 : birth.Timezone;
                    workflow = new KundliWorkflowService(Server.MapPath("~/Scripts/Custom/kundli-reference-engine.js"))
                        .Generate("OPT_KUNDLI", new KundliWorkflowRequest
                        {
                            Day = birth.Day,
                            Month = birth.Month,
                            Year = birth.Year,
                            Hour = birth.Hours,
                            Minute = birth.Minutes,
                            Latitude = birth.Latitude,
                            Longitude = birth.Longitude,
                            TimezoneOffset = tz
                        });
                    KundliWorkflowCache.Store(Session, workflow);
                }

                var cached = KundliWorkflowCache.GetPdf(Session);
                if (cached != null && cached.Bytes != null && cached.Bytes.Length > 0)
                    return File(cached.Bytes, "application/pdf", cached.FileName);

                var doc = new PublicLimitedReportBuilder().BuildKundliTwoPage(birth, workflow);
                var bytes = new LetterheadPdfReportRenderer(Server.MapPath("~/Images/Indu_Visiting_LetterHead.png")).Render(doc);
                KundliWorkflowCache.StorePdf(Session, bytes, doc.SuggestedFileName);
                return File(bytes, "application/pdf", doc.SuggestedFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[PublicReport.DownloadBasicKundli] " + ex);
                return RedirectToAction("KundliResult", "Home");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            new CommerceSchemaService().EnsureSchemaAndSeed();
            ReportAccessService.InvalidateCache();
            var items = _access.GetAll()
                .Where(r => r.IsEnabled && r.PublicAllowed)
                .Select(r =>
                {
                    var isFree = string.Equals(r.ReportCode, "KUNDLI_BASIC", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(r.ReportCode, "NUMEROLOGY_BASIC", StringComparison.OrdinalIgnoreCase)
                        || r.PriceInr <= 0m;
                    return new PublicReportCatalogItem
                    {
                        ReportCode = r.ReportCode,
                        ReportName = r.ReportName,
                        Description = string.IsNullOrWhiteSpace(r.ShortDescription) ? r.Notes : r.ShortDescription,
                        PriceInr = isFree ? 0m : r.PriceInr,
                        PublicAllowed = true,
                        IsFree = isFree
                    };
                })
                .OrderBy(r => r.ReportName)
                .ToList();
            ViewBag.Title = "Free Short Reports";
            return View(items);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult BasicHoroscope()
        {
            ViewBag.Title = "Free Basic Kundli Report";
            if (!_access.CanAccess("KUNDLI_BASIC", HttpContext))
                return RedirectToAction("AccessDenied", "Auth");

            var def = _access.GetDefinition("KUNDLI_BASIC");
            ViewBag.PriceInr = 0m;
            ViewBag.IsFree = true;
            ViewBag.Description = def != null ? def.ShortDescription : "FREE 2-page report: basic info + kundli chart (no payment)";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BasicHoroscope(BirthDataSessionModel birth, string mode)
        {
            ViewBag.Title = "Free Basic Kundli Report";
            if (!_access.CanAccess("KUNDLI_BASIC", HttpContext))
                return RedirectToAction("AccessDenied", "Auth");

            var def = _access.GetDefinition("KUNDLI_BASIC");
            ViewBag.PriceInr = 0m;
            ViewBag.IsFree = true;
            ViewBag.Description = def != null ? def.ShortDescription : "FREE 2-page report: basic info + kundli chart (no payment)";

            if (birth == null || birth.Year < 1900 || string.IsNullOrWhiteSpace(birth.Place))
            {
                ModelState.AddModelError("", "Please enter valid birth details and place.");
                return View();
            }

            try
            {
                // Free short report — never create a payable order.
                birth.ReportChoice = "OPT_KUNDLI";
                BirthDataSessionStore.Set(Session, birth);
                var scriptPath = Server.MapPath("~/Scripts/Custom/kundli-reference-engine.js");
                var tz = birth.Timezone == 0 ? 5.30 : birth.Timezone;
                var workflow = new KundliWorkflowService(scriptPath).Generate("OPT_KUNDLI", new KundliWorkflowRequest
                {
                    Day = birth.Day,
                    Month = birth.Month,
                    Year = birth.Year,
                    Hour = birth.Hours,
                    Minute = birth.Minutes,
                    Latitude = birth.Latitude,
                    Longitude = birth.Longitude,
                    TimezoneOffset = tz
                });
                var doc = new PublicLimitedReportBuilder().BuildKundliTwoPage(birth, workflow);
                var letterhead = Server.MapPath("~/Images/Indu_Visiting_LetterHead.png");
                var bytes = new LetterheadPdfReportRenderer(letterhead).Render(doc);

                if (string.Equals(mode, "preview", StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.PdfBase64 = Convert.ToBase64String(bytes);
                    ViewBag.FileName = doc.SuggestedFileName;
                    return View("BasicHoroscopePreview");
                }

                return File(bytes, "application/pdf", doc.SuggestedFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[PublicReport.BasicHoroscope] " + ex);
                // Surface a safe, actionable message (no secrets).
                var detail = ex.Message ?? "unknown error";
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    detail = detail + " (" + ex.InnerException.Message + ")";
                if (detail.Length > 280) detail = detail.Substring(0, 277) + "...";
                ModelState.AddModelError("", "Unable to generate report: " + detail);
                return View(birth);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult BasicNumerology()
        {
            ViewBag.Title = "Free Basic Numerology Report";
            if (!_access.CanAccess("NUMEROLOGY_BASIC", HttpContext))
                return RedirectToAction("AccessDenied", "Auth");
            var def = _access.GetDefinition("NUMEROLOGY_BASIC");
            ViewBag.PriceInr = 0m;
            ViewBag.IsFree = true;
            ViewBag.Description = def != null ? def.ShortDescription : "FREE core numerology numbers PDF";
            return View(new NumerologyViewModel { DateOfBirth = new DateTime(1990, 1, 1) });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BasicNumerology(NumerologyViewModel model, string mode)
        {
            ViewBag.Title = "Free Basic Numerology Report";
            if (!_access.CanAccess("NUMEROLOGY_BASIC", HttpContext))
                return RedirectToAction("AccessDenied", "Auth");
            var def = _access.GetDefinition("NUMEROLOGY_BASIC");
            ViewBag.PriceInr = 0m;
            ViewBag.IsFree = true;
            ViewBag.Description = def != null ? def.ShortDescription : "FREE core numerology numbers PDF";

            if (model == null || string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError("", "Name and date of birth are required.");
                return View(model ?? new NumerologyViewModel());
            }

            try
            {
                var result = new LegacyNumerologyService().Calculate(model.Name, model.DateOfBirth, model.NickName, model.VehicleOrPan);
                var doc = new PublicLimitedReportBuilder().BuildNumerologyBasic(result, model.Name, model.DateOfBirth);
                var letterhead = Server.MapPath("~/Images/Indu_Visiting_LetterHead.png");
                var bytes = new LetterheadPdfReportRenderer(letterhead).Render(doc);

                if (string.Equals(mode, "preview", StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.PdfBase64 = Convert.ToBase64String(bytes);
                    ViewBag.FileName = doc.SuggestedFileName;
                    return View("BasicHoroscopePreview");
                }

                return File(bytes, "application/pdf", doc.SuggestedFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[PublicReport.BasicNumerology] " + ex);
                ModelState.AddModelError("", "Unable to generate numerology report.");
                return View(model);
            }
        }
    }
}
