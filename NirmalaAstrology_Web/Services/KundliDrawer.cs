using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>Builds a chart representation or HTML; it never performs astronomy.</summary>
    public sealed class KundliDrawer
    {
        private readonly KundliFormatter _formatter;
        public KundliDrawer(KundliFormatter formatter = null)
        {
            _formatter = formatter ?? new KundliFormatter();
        }

        public string RenderHtml(KundliResult result, string title = "Kundli")
        {
            if (result == null) throw new ArgumentNullException("result");
            var sb = new StringBuilder();
            sb.Append("<section class=\"kundli-chart\">");
            sb.Append("<h3>").Append(_formatter.HtmlEncode(title)).Append("</h3>");
            sb.Append("<table class=\"table table-bordered\"><thead><tr><th>House</th><th>Rashi</th><th>Planets</th></tr></thead><tbody>");
            foreach (var house in result.Houses ?? new List<HouseData>())
            {
                sb.Append("<tr><th>").Append(house.HouseNumber).Append("</th><td>")
                  .Append(_formatter.HtmlEncode(house.RashiName)).Append("</td><td>")
                  .Append(_formatter.HtmlEncode(String.Join(", ", house.Planets ?? new List<string>())))
                  .Append("</td></tr>");
            }
            sb.Append("</tbody></table></section>");
            return sb.ToString();
        }

        public IDictionary<int, string> GetHouseLabels(KundliResult result)
        {
            return (result.Houses ?? new List<HouseData>()).ToDictionary(x => x.HouseNumber, x => x.RashiName);
        }
    }
}
