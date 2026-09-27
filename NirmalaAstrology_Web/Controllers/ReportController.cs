using System;
using System.Web.Mvc;
using System.Web.SessionState;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Services;
using NirmalaAstrology_Web.Services.Reports;

namespace NirmalaAstrology_Web.Controllers
{
    /// <summary>
    /// Official DETAILED report PDF — Admin only (report code KUNDLI_DETAILED).
    /// </summary>
    [AdminAuthorize]
    [ReportAuthorize("KUNDLI_DETAILED")]
    [SessionState(SessionStateBehavior.Required)]
    public class ReportController : Controller
    {
        [HttpGet]
        public ActionResult KundliPreview()
        {
            ViewBag.Title = "Report Preview";
            var birth = BirthDataSessionStore.Get(Session);
            if (birth == null || birth.Year < 1)
            {
                TempData["ErrorMessage"] = "Save birth data first, then open Report Preview.";
                return RedirectToAction("Index", "BirthData");
            }
            return View();
        }

        [HttpGet]
        public ActionResult KundliPdf(string mode)
        {
            try
            {
                string fileName;
                var bytes = BuildKundliPdf(out fileName);
                Response.Headers["Content-Disposition"] =
                    (string.Equals(mode, "inline", StringComparison.OrdinalIgnoreCase) ? "inline" : "attachment")
                    + "; filename=\"" + fileName + "\"";
                return File(bytes, "application/pdf");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Report.KundliPdf] " + ex);
                TempData["ErrorMessage"] = "Unable to generate the PDF. Please try again after saving birth data.";
                return RedirectToAction("KundliResult", "Home");
            }
        }

        [HttpGet]
        public ActionResult KundliPrint()
        {
            ViewBag.Title = "Print Report";
            var birth = BirthDataSessionStore.Get(Session);
            if (birth == null || birth.Year < 1)
            {
                TempData["ErrorMessage"] = "Save birth data first, then print the official report.";
                return RedirectToAction("Index", "BirthData");
            }
            return View();
        }

        private byte[] BuildKundliPdf(out string fileName)
        {
            var cached = KundliWorkflowCache.GetPdf(Session);
            if (cached != null && cached.Bytes != null && cached.Bytes.Length > 0)
            {
                fileName = string.IsNullOrWhiteSpace(cached.FileName) ? "NirmalaAstrology_Kundli.pdf" : cached.FileName;
                return cached.Bytes;
            }

            var birth = BirthDataSessionStore.Get(Session);
            if (birth == null || birth.Year < 1)
                throw new InvalidOperationException("Birth data session is missing.");

            var workflow = KundliWorkflowCache.GetWorkflow(Session) ?? BuildWorkflow(birth);
            var document = new KundliOfficialReportBuilder().Build(birth, workflow);
            fileName = document.SuggestedFileName;
            var letterhead = Server.MapPath("~/Images/Indu_Visiting_LetterHead.png");
            var bytes = new LetterheadPdfReportRenderer(letterhead).Render(document);
            KundliWorkflowCache.StorePdf(Session, bytes, fileName);
            return bytes;
        }

        private KundliWorkflowResult BuildWorkflow(BirthDataSessionModel birth)
        {
            var choice = string.IsNullOrWhiteSpace(birth.ReportChoice) ? "DETAILED" : birth.ReportChoice;
            var scriptPath = Server.MapPath("~/Scripts/Custom/kundli-reference-engine.js");
            var workflow = new KundliWorkflowService(scriptPath);
            return workflow.Generate(choice, new KundliWorkflowRequest
            {
                Day = birth.Day,
                Month = birth.Month,
                Year = birth.Year,
                Hour = birth.Hours,
                Minute = birth.Minutes,
                Latitude = birth.Latitude,
                Longitude = birth.Longitude,
                TimezoneOffset = birth.Timezone == 0 ? 5.5 : birth.Timezone
            });
        }
    }
}
