using System.Web.Mvc;
using NirmalaAstrology_Web.Services;

namespace NirmalaAstrology_Web.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ArticleService _articles = new ArticleService();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Title = "Articles";
            return View(_articles.GetPublished(50));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Detail(string slug)
        {
            var article = _articles.GetBySlug(slug, publishedOnly: true);
            if (article == null) return HttpNotFound();
            ViewBag.Title = article.Title;
            return View(article);
        }
    }
}
