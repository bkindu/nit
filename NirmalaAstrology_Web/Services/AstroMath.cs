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

        /// <summary>VB6 Format_RAKV(FNQ(lon)) — 0-based rashi display "RR:DD:MM:SS" (can show :60).</summary>
        public static string FormatRakv(double longitudeDegrees)
        {
            return FormatRakvEncoded(FnQ(longitudeDegrees));
        }

        /// <summary>
        /// VB6 FNDM — decimal degrees/hours → packed DD.MMSS / HH.MMSS
        /// (a2Functions_Antronomy.FNDM). Used before Format_RA / Format_RAK print.
        /// </summary>
        public static double FnDm(double dX)
        {
            var sign = Math.Sign(dX);
            if (sign == 0) sign = 1;
            dX = Math.Abs(dX);
            // Match VB6: (90*dX + 100*Int(dX) + Int(60*dX)) / 250
            var packed = (90.0 * dX + 100.0 * Math.Floor(dX) + Math.Floor(60.0 * dX)) / 250.0;
            return packed * sign;
        }

        /// <summary>VB6 Format_RA — "DD:MM" from packed or decimal via FNDM path.</summary>
        public static string FormatRa(double decimalOrPacked, bool alreadyPacked = false)
        {
            var packed = alreadyPacked ? decimalOrPacked : FnDm(decimalOrPacked);
            if (Math.Abs(packed) < 1e-12) return " 0:00";
            var sign = packed < 0 ? "-" : "";
            var raw = Math.Abs(packed).ToString("00.000000", CultureInfo.InvariantCulture);
            var padded = "00" + raw + "00.000000";
            var iPos = padded.IndexOf('.');
            if (iPos < 2) return " 0:00";
            return sign + padded.Substring(iPos - 2, 2) + ":" + padded.Substring(iPos + 1, 2);
        }

        /// <summary>
        /// VB6 Format_RAK — "DD:MM:SS" / "HH:MM:SS".
        /// Legacy ayanamsa print: Format_RAK(FNDM(dAyanamsa)) → e.g. 23:35:23.
        /// </summary>
        public static string FormatRak(double decimalOrPacked, bool alreadyPacked = false)
        {
            var packed = alreadyPacked ? decimalOrPacked : FnDm(decimalOrPacked);
            if (Math.Abs(packed) < 1e-12) return " 0:00:00";
            var sign = packed < 0 ? "-" : "";
            var raw = Math.Abs(packed).ToString("00.000000", CultureInfo.InvariantCulture);
            var padded = "00" + raw + "00.000000";
            var iPos = padded.IndexOf('.');
            if (iPos < 2) return " 0:00:00";
            return sign
                 + padded.Substring(iPos - 2, 2) + ":"
                 + padded.Substring(iPos + 1, 2) + ":"
                 + padded.Substring(iPos + 3, 2);
        }

        /// <summary>VB6 miscellaneous AYANAMSA column: Format_RAK(FNDM(dAyanamsa)).</summary>
        public static string FormatAyanamsa(double ayanamsaDegrees)
        {
            return FormatRak(ayanamsaDegrees, alreadyPacked: false);
        }

        /// <summary>VB6 SIDEREAL TIME column: Format_RAK(FNDM(dSiderealTime/15)).</summary>
        public static string FormatSiderealTimeHours(double siderealTimeDegrees)
        {
            return FormatRak(siderealTimeDegrees / 15.0, alreadyPacked: false);
        }

        /// <summary>
        /// VB6 Gati print: store path uses FNDM(Abs(Δλ°)*60) [+ FNR for non-Moon],
        /// print path is Kala(dGati) which applies FNDM again then Mid → "MMM:SS".
        /// </summary>
        /// <param name="dailyMotionDegrees">Signed daily motion in degrees (dPlt_next − dPlt).</param>
        /// <param name="isMoon">VB6 skips FNR(...,360) for Moon (Z=3).</param>
        public static string FormatGatiKala(double dailyMotionDegrees, bool isMoon = false)
        {
            var arcMinutes = Math.Abs(dailyMotionDegrees) * 60.0;
            if (!isMoon)
                arcMinutes = FnR(arcMinutes, 360.0);
            if (arcMinutes < 1e-12) return "0:00";

            // aBasic_Astronomy: dGati = FNDM(arcMinutes)
            var stored = FnDm(arcMinutes);
            // a4Functions_Print_RTF.Kala: FNDM again, then 3 digits before '.' and 2 after
            return FormatKalaPacked(FnDm(Math.Abs(stored)));
        }

        /// <summary>VB6 Kala() Mid formatting from an already FNDM-packed magnitude.</summary>
        public static string FormatKalaPacked(double packedAbs)
        {
            if (Math.Abs(packedAbs) < 1e-12) return "0:00";
            // VB6: sTxt = "     " & Trim(Str(FNDM(Abs(dX)))); Mid(iPos-3,3) & ":" & Mid(iPos+1,2)
            var raw = Math.Abs(packedAbs).ToString(CultureInfo.InvariantCulture);
            // Ensure a decimal point exists for Mid logic
            if (raw.IndexOf('.') < 0) raw = raw + ".00";
            var padded = "     " + raw + "00";
            var iPos = padded.IndexOf('.');
            if (iPos < 3) return "0:00";
            var mm = padded.Substring(iPos - 3, 3).TrimStart();
            if (string.IsNullOrEmpty(mm)) mm = "0";
            var ss = padded.Substring(iPos + 1, 2);
            return mm + ":" + ss;
        }

        /// <summary>VB6 <c>dCorr1</c> — tiny epsilon for Int/FNQ floor edge cases.</summary>
        public const double Corr1 = 0.00000000000001;

        /// <summary>VB6 FNQ — packed Rashi.AnshaKalaVikala encoding (print uses Format_RAKV).</summary>
        public static double FnQ(double dX)
        {
            var v = dX + Corr1;
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

        /// <summary>
        /// Atlas timezone is stored as HH.MM (e.g. 5.30 = 5h30m). Converts with <see cref="FnDg"/>.
        /// </summary>
        public static double NormalizeAtlasTimezone(double packedHhMm)
        {
            if (Math.Abs(packedHhMm) < 1e-12) return 5.5;
            return FnDg(packedHhMm);
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

        /// <summary>VB6 FNASN — arcsin in degrees.</summary>
        public static double FnAsn(double dX)
        {
            if (dX > 1.0) dX = 1.0;
            else if (dX < -1.0) dX = -1.0;
            if (Math.Abs(dX) >= 1.0) return dX > 0 ? 90.0 : -90.0;
            return Math.Atan(dX / Math.Sqrt(1.0 - dX * dX)) * 57.2957795;
        }

        /// <summary>VB6 SunBriefSyn — brief sayana sun used by SunRiseSetTimeZST.</summary>
        public static double SunBriefSyn(double dJDFx)
        {
            var dblM = FnFr(2.994397 + dJDFx / 365.25964) * 360.0;
            var dX = 1.9142998 * FnSn(dblM) + FnSn(2.0 * dblM) / 50.0290666
                     + FnSn(3.0 * dblM) / 3455.396;
            var dY = FnFr((279.205588 + dJDFx * 0.98564735 + dX) / 360.0 + 100.0) * 360.0;
            return FnR(dY, 360.0);
        }

        /// <summary>
        /// VB6 SunRiseSetTimeZST — iterative sunrise/sunset in zone standard time.
        /// Returns local clock hours (rise/set) and JD fractions (sr/ss).
        /// Lat/long/timezone must already be decimal (after FNDG on atlas HH.MM packs).
        /// </summary>
        public static SunRiseSetResult SunRiseSetTimeZst(
            double dJDFx, double dLatt, double dLong, double dTimeZone)
        {
            // --- Sunrise (5 iterations; dIK starts 0 like VB6 Double default) ---
            var dJDFsr = dJDFx;
            double dIK = 0.0;
            double dJDT = dJDFsr;
            for (var z = 1; z <= 5; z++)
            {
                dJDT = dJDFsr - dIK / 360.98564735;
                dJDFsr = dJDT;
                var dSun = SunBriefSyn(dJDFsr);
                var dAy = FnAt(0.9174 * FnTn(dSun)) + dSun - FnAt(FnTn(dSun));
                var dBy = FnAsn(FnTn(dLatt) * FnSn(dAy) * 0.4338);
                var dOa = dAy - dBy - 90.0;
                // VB6 hard-codes +5.5 in ST for this routine (legacy India path)
                var dSt = 16.6981583332 + 15.0 * (FnFr(dJDFsr) * 24.0 + 5.5)
                          + dLong + dJDFsr * 0.985647335386
                          + dJDFsr * dJDFsr / 3446481700000.0;
                dSt = FnR(dSt, 360.0);
                dIK = FnR(dSt - dOa, 360.0);
                if (Math.Abs(dIK - 360.0) < 1e-12) dIK = 0.0;
            }
            var dSunRiseTime = FnFr(dJDT + dTimeZone / 24.0) * 24.0;
            dJDFsr = dJDT;

            // --- Sunset ---
            var dJDFss = dJDFsr + 1.0;
            for (var z = 1; z <= 5; z++)
            {
                dJDT = dJDFss - dIK / 360.98564735;
                dJDFss = dJDT;
                var dSun = SunBriefSyn(dJDFss) + 180.0;
                var dAy = FnAt(0.9174 * FnTn(dSun)) + dSun - FnAt(FnTn(dSun));
                var dBy = FnAsn(FnTn(dLatt) * FnSn(dAy) * 0.4338);
                var dOa = dAy - dBy - 90.0;
                var dSt = 16.6981583332 + 15.0 * (FnFr(dJDFss) * 24.0 + 5.5)
                          + dLong + dJDFss * 0.985647335386
                          + dJDFss * dJDFss / 3446481700000.0;
                dSt = FnR(dSt, 360.0);
                dIK = FnR(dSt - dOa, 360.0);
                if (Math.Abs(dIK - 360.0) < 1e-12) dIK = 0.0;
            }
            var dSunSetTime = FnFr(dJDT + dTimeZone / 24.0) * 24.0;
            dJDFss = dJDT;

            return new SunRiseSetResult
            {
                SunRiseHours = dSunRiseTime,
                SunSetHours = dSunSetTime,
                JulianRise = dJDFsr,
                JulianSet = dJDFss
            };
        }

        /// <summary>
        /// Atlas-friendly wrapper: FNDG(lat/long/tz packs) + DT2JDF noon + SunRiseSetTimeZST.
        /// </summary>
        public static SunRiseSetResult SunRiseSetForBirthDay(
            int year, int month, int day,
            double atlasLatPacked, double atlasLonPacked, double atlasTzPacked)
        {
            var lat = FnDg(atlasLatPacked);
            var lon = FnDg(atlasLonPacked);
            var tz = NormalizeAtlasTimezone(atlasTzPacked);
            var jdf = Dt2Jdf(new DateTime(year, month, day), 12.0, tz);
            return SunRiseSetTimeZst(jdf, lat, lon, tz);
        }

        /// <summary>VB6 IshatKaalGPdec — ishatkaal in decimal ghatis.</summary>
        public static double IshatKaalGpDec(double dJd, double dLatt, double dLong)
        {
            var dY = SunBriefSyn(dJd);
            var dAy = FnAt(0.9174 * FnTn(dY)) + dY - FnAt(FnTn(dY));
            var dBy = FnAsn(FnTn(dLatt) * FnSn(dAy) * 0.4338);
            var dOa = dAy - dBy - 90.0;
            var dSt = 16.6981583332 + 15.0 * (FnFr(dJd) * 24.0 + 5.5)
                      + dLong + dJd * 0.985647335386
                      + dJd * dJd / 3446481700000.0;
            dSt = FnR(dSt, 360.0);
            var deg = FnR(dSt - dOa, 360.0);
            if (Math.Abs(deg - 360.0) < 1e-9) deg = 0.0;
            return deg / 6.0;
        }

        /// <summary>VB6 Kla(FNDM(hours)) — packed clock as "hh.mm" (ending-time print).</summary>
        public static string FormatKlaPackedHours(double decimalHours)
        {
            var packed = FnDm(Math.Abs(decimalHours));
            var rounded = Math.Round(packed, 2, MidpointRounding.AwayFromZero);
            if (Math.Abs(rounded) < 1e-12) return "  0.00";
            var raw = rounded.ToString("0.00", CultureInfo.InvariantCulture);
            var dot = raw.IndexOf('.');
            var padded = "   " + raw;
            var iPos = padded.IndexOf('.');
            var left = padded.Substring(iPos - 3, 3);
            var right = padded.Substring(iPos + 1, 2);
            var kla = left + "." + right;
            if (kla.Length >= 3 && kla[2] == ' ') kla = "  0" + kla.Trim();
            return kla;
        }

        public sealed class SunRiseSetResult
        {
            public double SunRiseHours { get; set; }
            public double SunSetHours { get; set; }
            public double JulianRise { get; set; }
            public double JulianSet { get; set; }
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
