using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NirmalaAstrology_Web.Helpers
{
    /// <summary>
    /// Custom HTML helper methods for reusable UI components.
    /// </summary>
    public static class CustomHtmlHelpers
    {
        /// <summary>
        /// Renders a Font Awesome icon as an <i> tag.
        /// Usage: @Html.FontAwesome("fas fa-star")
        /// </summary>
        public static IHtmlString FontAwesome(this HtmlHelper helper, string iconClass, string extraCss = "")
        {
            string css = string.IsNullOrEmpty(extraCss) ? iconClass : $"{iconClass} {extraCss}";
            return new MvcHtmlString($"<i class=\"{css}\"></i>");
        }

        /// <summary>
        /// Renders a section heading with decorative line.
        /// </summary>
        public static IHtmlString SectionHeading(this HtmlHelper helper, string title, string subtitle = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"section-heading text-center mb-5\">");
            sb.Append($"<h2 class=\"section-title\">{HttpUtility.HtmlEncode(title)}</h2>");
            if (!string.IsNullOrEmpty(subtitle))
            {
                sb.Append($"<p class=\"section-subtitle\">{HttpUtility.HtmlEncode(subtitle)}</p>");
            }
            sb.Append("<div class=\"heading-line\"><span></span></div>");
            sb.Append("</div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Truncates text to a specified length and appends "...".
        /// </summary>
        public static string Truncate(this HtmlHelper helper, string text, int maxLength = 100)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length > maxLength ? text.Substring(0, maxLength) + "..." : text;
        }
    }
}