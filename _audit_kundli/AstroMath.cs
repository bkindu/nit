using System;
using System.Globalization;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>VB6 a2Functions_Antronomy / a4Functions_Print_RTF math helpers used by ganit modules.</summary>
    public static class AstroMath
    {
        public static double FnFr(double dX)
        {
            return dX - Math.Floor(dX);
        }

        public static double FnR(double dX, double dCY)
        {
            return FnFr(FnFr(dX / dCY) + 2.0) * dCY;
        }

        public static int FnTgr(double dX)
        {
            return (int)(dX + 0.0000005);
        }

        /// <summary>VB6 iFNR — integer reduce into [0, iY).</summary>
        public static int IFnR(int iX, int iY)
        {
            return FnTgr(FnFr(FnFr((double)iX / iY) + 2.0) * iY);
        }

        /// <summary>VB6 FNC2 — used by D-10 / D-24 / D-40 / D-45 vargas.</summary>
        public static double FnC2(double divisor, double longitude)
        {
            return divisor * FnR(longitude, 30.0) / 30.0;
        }

        /// <summary>VB6 FNC3 — odd/even 30° block within 60°.</summary>
        public static double FnC3(double longitude)
        {
            return Math.Floor(FnR(longitude, 60.0) / 30.0);
        }

        /// <summary>
        /// Legacy VB6 RAKV components from continuous nirayana longitude.
        /// Rashi is <b>0-based</b> (0=Mesha … 11=Meena), matching FNQ / Format_RAKV.
        /// Astrology 1-based rashi for dignity/houses = Rashi0 + 1.
        /// </summary>
        public struct RakvParts
        {
            public int Rashi0;
            public int Ansha;
            public int Kala;
            public int Vikala;
            public double NormalizedLongitude;
        }

        /// <summary>
        /// Convert continuous longitude → Rashi0:Ansha:Kala:Vikala with proper carry.
        /// Rounds to the nearest vikala (arc-second) first so 29°59'59.6" can carry into the next rashi.
        /// </summary>
        public static RakvParts ToRakvParts(double longitudeDegrees)
        {
            var lon = FnR(longitudeDegrees, 360.0);
            // Work in integer arc-seconds to avoid float carry bugs.
            var totalSeconds = (long)Math.Floor(lon * 3600.0 + 0.5);
            const long secondsPerCircle = 360L * 3600L;
            const long secondsPerRashi = 30L * 3600L;
            if (totalSeconds >= secondsPerCircle) totalSeconds = 0;
            if (totalSeconds < 0) totalSeconds += secondsPerCircle;

            var rashi0 = (int)(totalSeconds / secondsPerRashi);
            var rem = totalSeconds % secondsPerRashi;
            var ansha = (int)(rem / 3600L);
            rem %= 3600L;
            var kala = (int)(rem / 60L);
            var vikala = (int)(rem % 60L);

            return new RakvParts
            {
                Rashi0 = rashi0,
                Ansha = ansha,
                Kala = kala,
                Vikala = vikala,
                NormalizedLongitude = totalSeconds / 3600.0
            };
        }

        /// <summary>VB6 Format_RAKV(FNQ(lon)) — 0-based rashi display "RR:DD:MM:SS".</summary>
        public static string FormatRakv(double longitudeDegrees)
        {
            var p = ToRakvParts(longitudeDegrees);
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}:{3:00}",
                p.Rashi0, p.Ansha, p.Kala, p.Vikala);
        }

        /// <summary>
        /// Legacy Gati print (Kala()): absolute daily motion in arc-minutes as MMM:SS style
        /// matching VB6 FNDM(Abs(Δλ°)*60) then Kala formatting (e.g. Moon 861:19).
        /// </summary>
        public static string FormatGatiKala(double dailyMotionDegrees)
        {
            var arcMinutes = Math.Abs(dailyMotionDegrees) * 60.0;
            if (arcMinutes < 1e-9) return "  0:00";
            // Pack via same FNDM idea: integer minutes + fractional → seconds
            var whole = (int)Math.Floor(arcMinutes + 1e-9);
            var frac = arcMinutes - whole;
            var secs = (int)Math.Floor(frac * 60.0 + 0.5);
            if (secs >= 60) { secs = 0; whole++; }
            return string.Format(CultureInfo.InvariantCulture, "{0,3}:{1:00}", whole, secs).TrimStart().PadLeft(6);
        }

        /// <summary>Legacy FNQ packed encoding (kept for callers that still need the packed double).</summary>
        public static double FnQ(double dX)
        {
            var v = dX + 0.00003;
            return (90.0 * v
                    + 17500.0 * Math.Floor(v / 30.0)
                    + 100.0 * Math.Floor(v)
                    + Math.Floor(v * 60.0)) / 25000.0;
        }

        /// <summary>Format already-FNQ-encoded value (VB6 Format_RAKV(dPltS)).</summary>
        public static string FormatRakvEncoded(double fnqEncoded)
        {
            if (Math.Abs(fnqEncoded) < 1e-12) return " 0:00:00:00";
            var raw = Math.Abs(fnqEncoded).ToString("00.000000", CultureInfo.InvariantCulture);
            var padded = "00" + raw + "00.000000";
            var iPos = padded.IndexOf('.');
            if (iPos < 2) return " 0:00:00:00";
            return padded.Substring(iPos - 2, 2) + ":"
                 + padded.Substring(iPos + 1, 2) + ":"
                 + padded.Substring(iPos + 3, 2) + ":"
                 + padded.Substring(iPos + 5, 2);
        }

        /// <summary>FNDG: converts HH.MM / DD.MM packed form to decimal hours/degrees.</summary>
        public static double FnDg(double packed)
        {
            return (250.0 * packed + 0.00000005
                    - 60.0 * Math.Floor(packed + 0.00000005)
                    - Math.Floor(100.0 * packed + 0.00000005)) / 90.0;
        }

        /// <summary>VB6 DT2JDF without summer-time corrections (India default path).</summary>
        public static double Dt2Jdf(DateTime date, double localTimeHours, double timeZoneHours)
        {
            var epoch = new DateTime(1899, 12, 31);
            var days = (date.Date - epoch).TotalDays;
            return days + (localTimeHours - timeZoneHours) / 24.0;
        }

        /// <summary>VB6 JD2DT — returns dd.MM.yyyy from Julian-day-fraction (1899 epoch style).</summary>
        public static string Jd2Dt(double dX)
        {
            var jdz = (long)Math.Floor(dX) + 2415020L;
            const long g = 1;
            var q = g * (long)Math.Floor((jdz / 36524.25) - 51.12264);
            var r = jdz + g + q - (long)Math.Floor(q / 4.0);
            var s = r + 1524;
            var t = (long)Math.Floor((s / 365.25) - 0.3343);
            var u = (long)Math.Floor(t * 365.25);
            var v = (long)Math.Floor((s - u) / 30.61);
            var d = s - u - (long)Math.Floor(v * 30.61);
            long m = v > 13.5 ? v - 13 : v - 1;
            long y = m > 2.5 ? t - 4716 : t - 4715;
            return string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:0000}", d, m, y);
        }

        public static double PksBalEtc(double dX, double dY)
        {
            var diff = Math.Abs(dX - dY);
            if (diff > 180) diff = 360 - diff;
            return diff / 3.0;
        }

        /// <summary>Approximate sunrise/sunset local hours (same approach as LegacyPanchangCalculator).</summary>
        public static double ApproxSunEventHours(DateTime date, double lat, double lon, double tz, bool rise)
        {
            var n = date.DayOfYear;
            var decl = 23.44 * Math.Sin((360.0 / 365.0) * (n - 81) * Math.PI / 180.0);
            var cosH = (Math.Cos(90.833 * Math.PI / 180.0) - Math.Sin(lat * Math.PI / 180.0) * Math.Sin(decl * Math.PI / 180.0))
                       / (Math.Cos(lat * Math.PI / 180.0) * Math.Cos(decl * Math.PI / 180.0));
            cosH = Math.Max(-1, Math.Min(1, cosH));
            var h = Math.Acos(cosH) * 180.0 / Math.PI;
            var eot = 9.87 * Math.Sin(2 * Math.PI * (n - 81) / 364.0)
                      - 7.53 * Math.Cos(2 * Math.PI * (n - 81) / 364.0)
                      - 1.5 * Math.Sin(2 * Math.PI * (n - 81) / 364.0);
            var solarNoon = 12 - lon / 15.0 + tz - eot / 60.0;
            return solarNoon + (rise ? -h / 15.0 : h / 15.0);
        }

        public static double[] MeanPlanets(double dJdf)
        {
            var mean = new double[10];
            double[] meanP = { 0, 0, 256.7433333, 241.3855556, 271.0275, 153.6722222, 215.5472222, 319.5055556, 244.0875, 236.747696777777 };
            double[] period = { 0, 0, 365.25636, 27.321661, 686.97982, 87.969256, 4332.589, 224.7008, 10759.23, 6793.47 };
            for (var z = 2; z <= 9; z++)
            {
                if (z == 9)
                    mean[z] = FnR(-FnFr(dJdf / period[z]) * 360.0 + meanP[z], 360.0);
                else
                    mean[z] = FnR(FnFr(dJdf / period[z]) * 360.0 + meanP[z], 360.0);
            }
            return mean;
        }

        public static double FnSn(double deg)
        {
            return Math.Sin(deg / 57.2957795);
        }

        public static double FnCs(double deg)
        {
            return Math.Cos(deg / 57.2957795);
        }

        public static double FnTn(double deg)
        {
            return Math.Tan(deg / 57.2957795);
        }

        public static double FnAt(double x)
        {
            return Math.Atan(x) * 57.2957795;
        }

        /// <summary>JS/VB julday used by kundli-reference-engine (astronomy epoch).</summary>
        public static double JulDay(int day, int month, int year, double hourDecimal)
        {
            if (month < 3) { year -= 1; month += 12; }
            var leep = (int)Math.Floor(year / 100.0);
            leep = 2 - leep + (int)Math.Floor(leep / 4.0);
            return Math.Floor(365.2501 * year) + Math.Floor((month + 1) * 30.6001) - 694025.0 + leep + day + hourDecimal / 24.0;
        }

        public static double SiderealTimeDegrees(double astronomyJdf, double localTimeHours, double longitude)
        {
            return FnR(16.6981583332 + 15.0 * localTimeHours + longitude
                       + astronomyJdf * 0.985647335386
                       + astronomyJdf * astronomyJdf / 3446481700000.0, 360.0);
        }

        public static string BirthDateLabel(int day, int month, int year)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:0000}", day, month, year);
        }
    }
}
