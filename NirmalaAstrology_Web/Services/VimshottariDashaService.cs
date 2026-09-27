using System;
using System.Globalization;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>Port of eDashasAll.DashasCalcAndPrinting (Vimshottari MD/AD ending dates).</summary>
    public sealed class VimshottariDashaService
    {
        private static readonly int[] DasaYears =
        {
            0, 7, 20, 6, 10, 7, 18, 16, 19, 17,
            7, 20, 6, 10, 7, 18, 16, 19, 17
        };

        private static readonly string[] DasaLords =
        {
            "", "Ketu", "Sukra", "Surya", "Chandra", "Mangla", "Rahu", "Guru", "Sani", "Budha",
            "Ketu", "Sukra", "Surya", "Chandra", "Mangla", "Rahu", "Guru", "Sani", "Budha"
        };

        public VimshottariDashaResult Calculate(
            double moonLongitude,
            int day,
            int month,
            int year,
            int hour,
            int minute,
            double timezoneOffset)
        {
            var localTime = AstroMath.FnDg(hour + minute / 100.0);
            var dJdf = AstroMath.Dt2Jdf(new DateTime(year, month, day), localTime, timezoneOffset);
            var dNak = 3.0 / 40.0 * AstroMath.FnR(moonLongitude, 120.0);
            var iBirthDashaLord = AstroMath.FnTgr(dNak);
            var dBhogyaFraction = -dNak + AstroMath.FnTgr(dNak);

            var iDasaVarsh = DasaYears[iBirthDashaLord + 1];
            var dBirthDasaBhogya = (1.0 - AstroMath.FnFr(dNak)) * iDasaVarsh;
            var balYears = (int)Math.Floor(dBirthDasaBhogya);
            var dX = AstroMath.FnFr(dBirthDasaBhogya) * 12.0;
            var balMonths = (int)Math.Floor(dX);
            var balDays = (int)Math.Floor(AstroMath.FnFr(dX) * 30.0 + 0.5);

            var dJDinYrs = dJdf / 365.242194;
            var dDasn = dJDinYrs + dBhogyaFraction * DasaYears[iBirthDashaLord + 1];

            var birthLabel = AstroMath.BirthDateLabel(day, month, year);
            var result = new VimshottariDashaResult
            {
                BirthLordIndex = iBirthDashaLord,
                BirthLord = DasaLords[iBirthDashaLord + 1].Trim(),
                BalanceYears = balYears,
                BalanceMonths = balMonths,
                BalanceDays = balDays,
                BalanceText = string.Format(CultureInfo.InvariantCulture,
                    "{0}:- {1} year {2} month {3} days",
                    DasaLords[iBirthDashaLord + 1].Trim(), balYears, balMonths, balDays),
                BirthDateLabel = birthLabel
            };

            var sDashaString = new string[10, 10];
            var sDasaDtPrev = "";

            for (var iMDasa = 1; iMDasa <= 9; iMDasa++)
            {
                for (var iADasa = 1; iADasa <= 9; iADasa++)
                {
                    var q = iMDasa + iBirthDashaLord;
                    var t = iADasa + q - 1;
                    if (t > 9) t -= 9;

                    dDasn = dDasn + DasaYears[q] * DasaYears[t] / 120.0;
                    var sDasaDt = AstroMath.Jd2Dt(dDasn * 365.242194);

                    if (dDasn >= dJDinYrs)
                    {
                        sDashaString[iMDasa, iADasa] = sDasaDtPrev + " to " + sDasaDt
                            + " (" + DasaLords[q].Trim() + " - " + DasaLords[t].Trim() + ")";
                        result.AntarDashas.Add(new AntarDashaRow
                        {
                            MahaSequence = iMDasa,
                            MahaLord = DasaLords[q].Trim(),
                            AntarLord = DasaLords[t].Trim(),
                            FromDate = string.IsNullOrEmpty(sDasaDtPrev) ? birthLabel : sDasaDtPrev,
                            ToDate = sDasaDt,
                            Label = sDashaString[iMDasa, iADasa]
                        });
                        sDasaDtPrev = sDasaDt;
                    }
                }
            }

            // Align print-order Q used for mahadasha headers (VB6: If Q > 9 Then Q = Q - 9 after loops).
            var headerQ = 9 + iBirthDashaLord;
            if (headerQ > 9) headerQ -= 9;

            var ageYears = balYears;
            var ageSuffix = string.Format(CultureInfo.InvariantCulture, "y{0}m{1}d)", balMonths, balDays);
            for (var i = 1; i <= 9; i++)
            {
                var lordIndex = headerQ + i;
                if (i == 1)
                {
                    // first age already balYears
                }
                else
                {
                    ageYears += DasaYears[headerQ + i];
                }

                var fromDate = i == 1
                    ? birthLabel
                    : MidEndDate(sDashaString, i - 1);
                var toDate = MidEndDate(sDashaString, i);

                result.MahaDashas.Add(new MahaDashaBlock
                {
                    Sequence = i,
                    Lord = DasaLords[lordIndex].Trim(),
                    Years = DasaYears[lordIndex],
                    FromDate = fromDate,
                    ToDate = toDate,
                    AgeLabel = "(Upto age " + ageYears + ageSuffix
                });
            }

            return result;
        }

        private static string MidEndDate(string[,] sDashaString, int mahaIndex)
        {
            if (mahaIndex < 1 || mahaIndex > 9) return "";
            var raw = sDashaString[mahaIndex, 9] ?? "";
            // "... to DD.MM.YYYY (Lord - Lord)" — ending date starts at index 15 in VB Mid(...,15,10)
            if (raw.Length >= 25)
            {
                var marker = " to ";
                var pos = raw.IndexOf(marker, StringComparison.Ordinal);
                if (pos >= 0 && raw.Length >= pos + 14)
                    return raw.Substring(pos + marker.Length, 10);
            }
            return "";
        }
    }
}
