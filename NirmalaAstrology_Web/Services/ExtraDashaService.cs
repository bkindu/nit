using System;
using System.Collections.Generic;
using System.Globalization;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Port of eDashasAll DETAILED extras:
    /// Krittikadi/Ardradi Ashtottari, Yogini, Shodshottari, Panchottari,
    /// Shatabadika, Chaturshityabadika, Davisapatika, Sashathihayani, Sandhya, Pachak.
    /// </summary>
    public sealed class ExtraDashaService
    {
        public ExtraDashaBundle Calculate(
            double moonLongitude,
            double lagnaLongitude,
            int day, int month, int year, int hour, int minute, double timezoneOffset)
        {
            var localTime = AstroMath.FnDg(hour + minute / 100.0);
            var dJdf = AstroMath.Dt2Jdf(new DateTime(year, month, day), localTime, timezoneOffset);
            var birthLabel = AstroMath.BirthDateLabel(day, month, year);

            double krittikadiNak;
            int kritLord;
            double kritBhogya;
            ResolveKrittikadi(moonLongitude, out krittikadiNak, out kritLord, out kritBhogya);

            int ardraLord;
            double ardraBhogya;
            ResolveArdradi(krittikadiNak, out ardraLord, out ardraBhogya);

            int[] ashtoYears = { 0, 6, 15, 8, 17, 10, 19, 12, 21, 6, 15, 8, 17, 10, 19, 12, 21 };
            string[] ashtoLords =
            {
                "", "Sur", "Chn", "Mgl", "Bud", "San", "Gur", "Rah", "Suk",
                "Sur", "Chn", "Mgl", "Bud", "San", "Gur", "Rah", "Suk"
            };

            var bundle = new ExtraDashaBundle
            {
                Ashtottari = Build78(dJdf, birthLabel, kritLord, kritBhogya, ashtoYears, ashtoLords, 8, 108, "Ashtottari (Krittikadi)"),
                AshtottariArdradi = Build78(dJdf, birthLabel, ardraLord, ardraBhogya, ashtoYears, ashtoLords, 8, 108, "Ashtottari (Ardradi)"),
                Yogini = BuildNakFraction(
                    moonLongitude, -5, dJdf, birthLabel, 8, 36,
                    new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 },
                    new[] { "", "Mgl", "Png", "Dny", "Brm", "Bdr", "Ulk", "Sid", "Snk", "Mgl", "Png", "Dny", "Brm", "Bdr", "Ulk", "Sid", "Snk" },
                    "Yogini"),
                Shodshottari = BuildNakFraction(
                    moonLongitude, -7, dJdf, birthLabel, 8, 106,
                    new[] { 0, 11, 12, 13, 14, 15, 16, 17, 18, 11, 12, 13, 14, 15, 16, 17, 18 },
                    new[] { "", "Sur", "Mng", "Gur", "San", "Ket", "Cnd", "Bud", "Suk", "Sur", "Mng", "Gur", "San", "Ket", "Cnd", "Bud", "Suk" },
                    "Shodshottari"),
                Panchottari = BuildNakFraction(
                    moonLongitude, -15, dJdf, birthLabel, 8, 105,
                    new[] { 0, 12, 13, 14, 15, 16, 17, 18, 12, 13, 14, 15, 16, 17, 18, 12, 13 },
                    new[] { "", "Sur", "Bud", "San", "Mgl", "Suk", "Cnd", "Gur", "Sur", "Bud", "San", "Mgl", "Suk", "Cnd", "Gur", "Sur", "Bud" },
                    "Panchottari"),
                Shatabadika = BuildNakFraction(
                    moonLongitude, 1, dJdf, birthLabel, 7, 100,
                    new[] { 0, 5, 5, 10, 10, 20, 20, 30, 5, 5, 10, 10, 20, 20, 30, 30 },
                    new[] { "", "Sur", "Chn", "Suk", "Bud", "Gur", "Mgl", "San", "Sur", "Chn", "Suk", "Bud", "Gur", "Mgl", "San", "Sur", "Chn" },
                    "Shatabadika"),
                Chaturshityabadika = BuildNakFraction(
                    moonLongitude, -14, dJdf, birthLabel, 7, 84,
                    new[] { 0, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12 },
                    new[] { "", "Sur", "Chn", "Mgl", "Bud", "Gur", "Suk", "San", "Sur", "Chn", "Mgl", "Bud", "Gur", "Suk", "San" },
                    "Chaturshityabadika"),
                Davisapatika = BuildNakFraction(
                    moonLongitude, -18, dJdf, birthLabel, 8, 72,
                    new[] { 0, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    new[] { "", "Sur", "Chn", "Suk", "Bud", "Gur", "Mgl", "San", "Rah", "Sur", "Chn", "Suk", "Bud", "Gur", "Mgl", "San", "Rah" },
                    "Davisapatika"),
                Sashathihayani = BuildNakFraction(
                    moonLongitude, -26, dJdf, birthLabel, 8, 36,
                    new[] { 0, 10, 10, 10, 6, 6, 6, 6, 6, 10, 10, 10, 6, 6, 6, 6, 6 },
                    new[] { "", "Sur", "Chn", "Mgl", "Bud", "Gur", "Suk", "San", "Rah", "Sur", "Chn", "Mgl", "Bud", "Gur", "Suk", "San", "Rah" },
                    "Sashathihayani"),
                Sandhya = CalculateSandhya(lagnaLongitude, day, month, year),
                Pachak = CalculatePachak(dJdf, birthLabel)
            };

            bundle.Systems.Add(bundle.Ashtottari);
            bundle.Systems.Add(bundle.AshtottariArdradi);
            bundle.Systems.Add(bundle.Yogini);
            bundle.Systems.Add(bundle.Shodshottari);
            bundle.Systems.Add(bundle.Panchottari);
            bundle.Systems.Add(bundle.Shatabadika);
            bundle.Systems.Add(bundle.Chaturshityabadika);
            bundle.Systems.Add(bundle.Davisapatika);
            bundle.Systems.Add(bundle.Sashathihayani);
            bundle.Systems.Add(bundle.Sandhya);
            bundle.Systems.Add(bundle.Pachak);
            return bundle;
        }

        /// <summary>Krittikadi nak starting point (also feeds Ardradi).</summary>
        private static void ResolveKrittikadi(double moon, out double dNak, out int lord, out double bhogyaFrac)
        {
            dNak = 3.0 / 40.0 * moon + 1.0;
            dNak = 27.0 * AstroMath.FnFr((dNak - 3.0) / 27.0 + 100.0);

            if (dNak <= 3) { lord = 1; bhogyaFrac = dNak / 3.0; }
            else if (dNak <= 7) { lord = 2; bhogyaFrac = (dNak - 3) / 4.0; }
            else if (dNak <= 10) { lord = 3; bhogyaFrac = (dNak - 7) / 3.0; }
            else if (dNak <= 14) { lord = 4; bhogyaFrac = (dNak - 10) / 4.0; }
            else if (dNak <= 17) { lord = 5; bhogyaFrac = (dNak - 14) / 3.0; }
            else if (dNak <= 20) { lord = 6; bhogyaFrac = (dNak - 17) / 3.0; }
            else if (dNak <= 23) { lord = 7; bhogyaFrac = (dNak - 20) / 3.0; }
            else { lord = 8; bhogyaFrac = (dNak - 23) / 4.0; }
        }

        /// <summary>VB6 reuses Krittikadi dNak then shifts: 27*FNFR((dNak-6)/27+100).</summary>
        private static void ResolveArdradi(double krittikadiNak, out int lord, out double bhogyaFrac)
        {
            var dNak = 27.0 * AstroMath.FnFr((krittikadiNak - 6.0) / 27.0 + 100.0);

            if (dNak <= 4) { lord = 1; bhogyaFrac = dNak / 4.0; }
            else if (dNak <= 7) { lord = 2; bhogyaFrac = (dNak - 4) / 3.0; }
            else if (dNak <= 11) { lord = 3; bhogyaFrac = (dNak - 7) / 4.0; }
            else if (dNak <= 14) { lord = 4; bhogyaFrac = (dNak - 11) / 3.0; }
            else if (dNak <= 17) { lord = 5; bhogyaFrac = (dNak - 14) / 3.0; }
            else if (dNak <= 20) { lord = 6; bhogyaFrac = (dNak - 17) / 3.0; }
            else if (dNak <= 23) { lord = 7; bhogyaFrac = (dNak - 20) / 4.0; }
            else { lord = 8; bhogyaFrac = (dNak - 23) / 3.0; }
        }

        /// <summary>Port of PrintingOfDashas (Yogini / Shodshottari / …).</summary>
        private static AlternateDashaResult BuildNakFraction(
            double moon, int dna, double dJdf, string birthLabel,
            int cycle, int totalYears, int[] years, string[] lords, string name)
        {
            var frac = cycle * AstroMath.FnFr((3.0 / 40.0 * moon + dna) / cycle + 100.0);
            var lord = AstroMath.FnTgr(frac) + 1;
            var dJDinYrs = dJdf / 365.242194;
            var dBirthDasaBhogya = (1.0 - AstroMath.FnFr(frac)) * years[lord];
            int y, m, d;
            BalanceYmd(dBirthDasaBhogya, out y, out m, out d);

            var dDasn = dJDinYrs - AstroMath.FnFr(frac) * years[lord];
            var result = new AlternateDashaResult
            {
                SystemName = name,
                BirthLord = lords[lord],
                BalanceText = string.Format(CultureInfo.InvariantCulture, "{0}:- {1}-{2}-{3}", lords[lord], y, m, d)
            };

            var prev = birthLabel;
            for (var iM = 1; iM <= cycle; iM++)
            {
                for (var iA = 1; iA <= cycle; iA++)
                {
                    var q = iM + lord - 1;
                    var t = iA + q - 1;
                    while (t > cycle) t -= cycle;
                    dDasn += years[q] * years[t] / (double)totalYears;
                    var end = AstroMath.Jd2Dt(dDasn * 365.242194);
                    if (dDasn >= dJDinYrs)
                    {
                        result.Periods.Add(new AlternateDashaPeriod
                        {
                            MahaLord = lords[q],
                            AntarLord = lords[t],
                            FromDate = prev,
                            ToDate = end,
                            Label = lords[q] + "-" + lords[t] + ":" + end
                        });
                        prev = end;
                    }
                }
            }
            return result;
        }

        private static AlternateDashaResult Build78(
            double dJdf, string birthLabel, int lord, double bhogyaFrac,
            int[] years, string[] lords, int cycle, int divisor, string name)
        {
            var dJDinYrs = dJdf / 365.242194;
            var dDasn = dJDinYrs - bhogyaFrac * years[lord];
            var dBirthDasaBhogya = years[lord] - bhogyaFrac * years[lord];
            int y, m, d;
            BalanceYmd(dBirthDasaBhogya, out y, out m, out d);

            var result = new AlternateDashaResult
            {
                SystemName = name,
                BirthLord = lords[lord],
                BalanceText = string.Format(CultureInfo.InvariantCulture, "{0}:- {1}-{2}-{3}", lords[lord], y, m, d)
            };

            var prev = birthLabel;
            for (var iM = 1; iM <= cycle; iM++)
            {
                for (var iA = 1; iA <= cycle; iA++)
                {
                    var q = iM + lord - 1;
                    var t = iA + q - 1;
                    while (t > cycle) t -= cycle;
                    dDasn += years[q] * years[t] / (double)divisor;
                    var end = AstroMath.Jd2Dt(dDasn * 365.242194);
                    if (dDasn >= dJDinYrs)
                    {
                        result.Periods.Add(new AlternateDashaPeriod
                        {
                            MahaLord = lords[q],
                            AntarLord = lords[t],
                            FromDate = prev,
                            ToDate = end,
                            Label = lords[q] + "-" + lords[t] + ":" + end
                        });
                        prev = end;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Port of SandhyaDasha. VB computes a start lord from Lagna then prints fixed
        /// Sur→Ketu labels for Z=1..9 with year = birthYear + 10*Z (iLord unused in print).
        /// </summary>
        private static AlternateDashaResult CalculateSandhya(double lagnaLongitude, int day, int month, int year)
        {
            var iDasaLord = AstroMath.FnTgr(lagnaLongitude / 30.0) + 1;
            int iLord;
            if (iDasaLord == 1 || iDasaLord == 8) iLord = 3;
            else if (iDasaLord == 2 || iDasaLord == 7) iLord = 6;
            else if (iDasaLord == 3 || iDasaLord == 6) iLord = 4;
            else if (iDasaLord == 4) iLord = 2;
            else if (iDasaLord == 5) iLord = 1;
            else if (iDasaLord == 10 || iDasaLord == 11) iLord = 7;
            else iLord = 5; // 9 or 12

            string[] names =
            {
                "", "Surya", "Chandra", "Mangla", "Budha", "Guru", "Shukra", "Shani", "Rahu", "Ketu",
                "Surya", "Chandra", "Mangla", "Budha", "Guru", "Shukra", "Shani", "Rahu", "Ketu"
            };

            var result = new AlternateDashaResult
            {
                SystemName = "Sandhya",
                BirthLord = names[iLord],
                BalanceText = "Mapped start " + names[iLord] + "; printed as fixed 10y Sur→Ketu (VB6)"
            };

            // VB: iLord = iLord - 1 then loop increments (unused for label); labels use sDashaLord(Z).
            var y = year;
            var dd = day.ToString("00", CultureInfo.InvariantCulture);
            var mm = month.ToString("00", CultureInfo.InvariantCulture);
            for (var z = 1; z <= 9; z++)
            {
                y = y + 10;
                var end = string.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2}", dd, mm, y);
                result.Periods.Add(new AlternateDashaPeriod
                {
                    MahaLord = names[z],
                    AntarLord = "",
                    FromDate = "",
                    ToDate = end,
                    Label = names[z] + " Sandhya Dasha  " + end
                });
            }
            return result;
        }

        /// <summary>
        /// Port of PachakDasha house periods (days). VB6 dated from JD epoch;
        /// we anchor on birth JD so printed ends are usable (same day counts).
        /// </summary>
        private static AlternateDashaResult CalculatePachak(double dJdf, string birthLabel)
        {
            int[] days = { 0, 697, 348, 348, 349, 232, 232, 232, 232, 232, 232, 232, 233 };
            string[] houses =
            {
                "", "First", "Second", "Third", "Fourth", "Fifth", "Sixth",
                "Seventh", "Eighth", "Ninth", "Tenth", "Eleventh", "Twelfth"
            };

            var result = new AlternateDashaResult
            {
                SystemName = "Pachak",
                BirthLord = "First",
                BalanceText = "House periods in days from birth"
            };

            var prev = birthLabel;
            double dX = 0;
            for (var z = 1; z <= 12; z++)
            {
                dX += days[z];
                var end = AstroMath.Jd2Dt(dJdf + dX);
                result.Periods.Add(new AlternateDashaPeriod
                {
                    MahaLord = houses[z],
                    AntarLord = "House",
                    FromDate = prev,
                    ToDate = end,
                    Label = houses[z] + " House " + end
                });
                prev = end;
            }
            return result;
        }

        private static void BalanceYmd(double yearsFrac, out int y, out int m, out int d)
        {
            y = (int)Math.Floor(yearsFrac);
            var months = AstroMath.FnFr(yearsFrac) * 12.0;
            d = (int)Math.Floor(AstroMath.FnFr(months) * 30.0 + 0.5);
            m = (int)Math.Floor(months);
            if (d == 30) { d = 0; m++; }
            if (m == 12) { m = 0; y++; }
        }
    }
}
