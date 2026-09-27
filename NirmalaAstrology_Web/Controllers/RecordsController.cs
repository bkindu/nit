using System;
using System.Web.Mvc;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Repositories;

namespace NirmalaAstrology_Web.Controllers
{
    [AdminAuthorize]
    public class RecordsController : Controller
    {
        private readonly NativeRepository _natives = new NativeRepository();

        [HttpGet]
        public ActionResult Index(NativeSearchRequest request)
        {
            ViewBag.Title = "Search Records";
            if (request == null) request = new NativeSearchRequest();
            try
            {
                int total;
                var rows = _natives.Search(request, out total);
                ViewBag.TotalCount = total;
                ViewBag.Request = request;
                ViewBag.PageCount = Math.Max(1, (int)Math.Ceiling(total / (double)request.PageSize));
                return View(rows);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Records.Index] " + ex.Message);
                ViewBag.TotalCount = 0;
                ViewBag.Request = request;
                ViewBag.PageCount = 1;
                TempData["ErrorMessage"] = "Native records are unavailable until tbl_native is created in MySQL.";
                return View(new NativeRecordModel[0]);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _natives.Delete(id);
                TempData["SuccessMessage"] = "Record deleted.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Records.Delete] " + ex.Message);
                TempData["ErrorMessage"] = "The record could not be deleted.";
            }
            return RedirectToAction("Index");
        }
    }
}
