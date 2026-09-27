using System.Web.Mvc;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ArticlesController : Controller
    {
        private readonly ArticleService _articles = new ArticleService();
        private readonly AuthService _auth = new AuthService();

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Articles";
            return View(_articles.GetAllAdmin());
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Title = "New Article";
            return View("Edit", new ArticleEditModel { IsPublished = true });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var a = _articles.GetById(id);
            if (a == null) return HttpNotFound();
            ViewBag.Title = "Edit Article";
            return View(new ArticleEditModel
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Slug = a.Slug,
                Summary = a.Summary,
                BodyHtml = a.BodyHtml,
                ImageUrl = a.ImageUrl,
                IsPublished = a.IsPublished
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(ArticleEditModel model)
        {
            ViewBag.Title = model != null && model.ArticleId > 0 ? "Edit Article" : "New Article";
            model = model ?? new ArticleEditModel();

            if (!ModelState.IsValid)
                return View(model);

            var admin = _auth.GetByUsername(User.Identity.Name);
            var physical = Server.MapPath("~/Content/uploads/articles");
            const string storeRoot = "/Content/uploads/articles";

            string error;
            if (!_articles.Save(model, admin != null ? (int?)admin.UserId : null, physical, storeRoot, out error))
            {
                ModelState.AddModelError("", error ?? "Unable to save article.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Article saved.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            string error;
            if (!_articles.Delete(id, out error))
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Article deleted.";
            return RedirectToAction("Index");
        }
    }
}
