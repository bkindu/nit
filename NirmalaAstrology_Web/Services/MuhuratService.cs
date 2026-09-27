using System;
using System.Globalization;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// MVP port of tTodayzKaalShudhi Vara_Vibhajan subset:
    /// Abhijit, Chatur-Ghatika, Rahu Kaal (+ short Kaal Shuddhi notes).
    /// </summary>
    public sealed class MuhuratService
    {
        private static readonly string[] Weekdays =
        {
            "", "Ravivaar", "Somvaar", "Mangalvaar", "Budhvaar", "Guruvaar", "Shukravaar", "Shanivaar"
        };

        private static readonly string[][] ChaturDayNight =
        {
            null,
            // Sun day 1-8, night 9-16
            new[] { "", "UDVEGA", "CHARA", "LABHA", "AMRITA", "KALA", "SHUBHA", "ROGA", "UDVEGA",
                "SHUBHA", "AMRITA", "CHARA", "ROGA", "KALA", "LABHA", "UDVEGA", "SHUBHA" },
            new[] { "", "AMRITA", "KALA", "SHUBHA", "ROGA", "UDVEGA", "CHARA", "LABHA", "AMRITA",
                "CHARA", "ROGA", "KALA", "LABHA", "UDVEGA", "SHUBHA", "AMRITA", "CHARA" },
            new[] { "", "ROGA", "UDVEGA", "CHARA", "LABHA", "AMRITA", "KALA", "SHUBHA", "ROGA",
                "KALA", "LABHA", "UDVEGA", "SHUBHA", "AMRITA", "CHARA", "ROGA", "KALA" },
            new[] { "", "LABHA", "AMRITA", "KALA", "SHUBHA", "ROGA", "UDVEGA", "CHARA", "LABHA",
                "UDVEGA", "SHUBHA", "AMRITA", "CHARA", "ROGA", "KALA", "LABHA", "UDVEGA" },
            new[] { "", "SHUBHA", "ROGA", "UDVEGA", "CHARA", "LABHA", "AMRITA", "KALA", "SHUBHA",
                "AMRITA", "CHARA", "ROGA", "KALA", "LABHA", "UDVEGA", "SHUBHA", "AMRITA" },
            new[] { "", "CHARA", "LABHA", "AMRITA", "KALA", "SHUBHA", "ROGA", "UDVEGA", "CHARA",
                "ROGA", "KALA", "LABHA", "UDVEGA", "SHUBHA", "AMRITA", "CHARA", "ROGA" },
            new[] { "", "KALA", "SHUBHA", "ROGA", "UDVEGA", "CHARA", "LABHA", "AMRITA", "KALA",
                "LABHA", "UDVEGA", "SHUBHA", "AMRITA", "CHARA", "ROGA", "KALA", "LABHA" }
        };

        // Day eighth for Rahu Kaal: Sun..Sat → part index 1..8
        private static readonly int[] RahuPart = { 0, 8, 2, 7, 5, 6, 4, 3 };

        public MuhuratResult Calculate(DateTime date, double latitude, double longitude, double timezone, string placeLabel)
        {
            var sunrise = AstroMath.ApproxSunEventHours(date, latitude, longitude, timezone, true);
            var sunset = AstroMath.ApproxSunEventHours(date, latitude, longitude, timezone, false);
            if (sunset <= sunrise) sunset = sunrise + 12.0;

            var iVaar = ((int)date.DayOfWeek) + 1; // Sun=1 … Sat=7
            var dayLen = sunset - sunrise;
            var nightLen = 24.0 - dayLen;
            var day8 = dayLen / 8.0;
            var night8 = nightLen / 8.0;

            var result = new MuhuratResult
            {
                DateLabel = date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                PlaceLabel = placeLabel ?? "",
                WeekdayHindu = Weekdays[iVaar],
                SunriseText = FormatHours(sunrise),
                SunsetText = FormatHours(sunset)
            };

            // Abhijit — mid-day ± half of (day/15); Wednesday inauspicious
            var mid = sunrise + dayLen / 2.0;
            var halfMuhurta = dayLen / 30.0; // half of day/15
            if (iVaar == 4)
            {
                result.AbhijitNote = "Abhijit is inauspicious today (Wednesday / Budhvaar).";
                result.AbhijitWindow = "-";
            }
            else
            {
                result.AbhijitNote = "Abhijit (Vijaya) muhurta — auspicious for most work except southward journey.";
                result.AbhijitWindow = FormatHours(mid - halfMuhurta) + " to " + FormatHours(mid + halfMuhurta);
            }

            // Rahu Kaal — 8th-part of day by weekday
            var part = RahuPart[iVaar];
            var rkFrom = sunrise + day8 * (part - 1);
            var rkTo = sunrise + day8 * part;
            result.RahuKaalWindow = FormatHours(rkFrom) + " to " + FormatHours(rkTo);

            // Chatur Ghatika — 8 day + 8 night
            var names = ChaturDayNight[iVaar];
            for (var z = 1; z <= 8; z++)
            {
                var from = sunrise + day8 * (z - 1);
                var to = sunrise + day8 * z;
                result.ChaturGhatika.Add(new MuhuratSegment
                {
                    Period = "Day " + z,
                    Name = names[z],
                    FromText = FormatHours(from),
                    ToText = FormatHours(to)
                });
            }
            for (var z = 1; z <= 8; z++)
            {
                var from = sunset + night8 * (z - 1);
                var to = sunset + night8 * z;
                if (to >= 24) to -= 24;
                result.ChaturGhatika.Add(new MuhuratSegment
                {
                    Period = "Night " + z,
                    Name = names[z + 8],
                    FromText = FormatHours(from % 24),
                    ToText = FormatHours(to)
                });
            }

            result.KaalShuddhiNotes.Add("Prefer Amrita / Shubha / Labha segments; avoid Kala / Roga / Udvega when possible.");
            result.KaalShuddhiNotes.Add("Avoid Rahu Kaal for journeys and new ventures.");
            if (iVaar != 4)
                result.KaalShuddhiNotes.Add("Abhijit window is a strong general-purpose muhurta (except Wednesday).");
            else
                result.KaalShuddhiNotes.Add("Skip Abhijit on Wednesday; use alternate Shubha/Amrita day parts.");

            return result;
        }

        private static string FormatHours(double hours)
        {
            hours = AstroMath.FnR(hours, 24.0);
            var h = (int)Math.Floor(hours + 1e-9);
            var m = (int)Math.Floor((hours - h) * 60.0 + 0.5);
            if (m >= 60) { m = 0; h++; }
            if (h >= 24) h -= 24;
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", h, m);
        }
    }
}
