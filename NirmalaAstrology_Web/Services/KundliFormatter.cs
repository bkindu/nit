using System;
using System.Globalization;
using System.Net;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>Pure formatting helpers. No MVC, DOM, or database dependency.</summary>
    public sealed class KundliFormatter
    {
        public string FormatDegree(double value, int decimals = 2)
        {
            return value.ToString("F" + decimals, CultureInfo.InvariantCulture) + "°";
        }

        public string FormatDate(DateTime value, string format = "dd MMM yyyy")
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        public string FormatTime(TimeSpan value)
        {
            return value.ToString(@"hh\:mm");
        }

        public string HtmlEncode(string value)
        {
            return WebUtility.HtmlEncode(value ?? String.Empty);
        }

        public string FormatPlanet(PlanetPosition planet)
        {
            if (planet == null) return String.Empty;
            var retrograde = planet.IsRetrograde ? " R" : String.Empty;
            return String.Format(CultureInfo.InvariantCulture, "{0} {1} {2}{3}",
                planet.Name, planet.RashiName, FormatDegree(planet.DegreesInRashi), retrograde);
        }
    }
}
