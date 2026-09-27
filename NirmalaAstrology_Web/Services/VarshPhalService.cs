using System;
using System.Globalization;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>MVP port of qVarshPhal.VarshPhalTajik — Parvesh, Muntha, Mudda, core Sahams.</summary>
    public sealed class VarshPhalService
    {
        private static readonly string[] Rashis =
        {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };

        private static readonly string[] Nakshatras =
        {
            "", "ASHWINI", "BHARANI", "KRITIKA", "ROHINI", "MRIGSHIRA", "ARDRA", "PUNARVASU", "PUSHYA", "ASHLESHA",
            "MAGHA", "P.PHALGUNI", "U.PHALGUNI", "HASTA", "CHITRA", "SWATI", "VISAKHA", "ANURADHA", "JYESTHA", "MOOLA",
            "P.SHADA", "U.SHADA", "SHARAWAN", "DHANISTHA", "SHATBHISHA", "P.BHADRAPAD", "U.BHADRAPAD", "REVATI"
        };

        private static readonly int[] MuddaYears =
        {
            0, 7, 20, 6, 10, 7, 18, 16, 19, 17, 7, 20, 6, 10, 7, 18, 16, 19, 17, 7
        };

        private static readonly string[] MuddaLords =
        {
            "", "KET", "SUK", "SUR", "CHN", "MAN", "RAH", "GUR", "SAN", "BUD",
            "KET", "SUK", "SUR", "CHN", "MAN", "RAH", "GUR", "SAN", "BUD", "KET"
        };

        public VarshPhalService(string kundliScriptPath = null)
        {
        }

        public VarshPhalResult Calculate(BirthDataSessionModel birth, int targetYear)
        {
            if (birth == null) throw new ArgumentNullException("birth");
            if (targetYear < birth.Year || targetYear > birth.Year + 120)
                throw new ArgumentOutOfRangeException("targetYear");

            var calc = new KundliCalculator();
            var birthChart = calc.CalculateKundli(
                birth.Day, birth.Month, birth.Year, birth.Hours, birth.Minutes,
                birth.Longitude, birth.Latitude, birth.Timezone);

            var birthLocal = AstroMath.FnDg(birth.Hours + birth.Minutes / 100.0);
            var dJdfBirth = AstroMath.Dt2Jdf(new DateTime(birth.Year, birth.Month, birth.Day), birthLocal, birth.Timezone);

            var yearStart = AstroMath.Dt2Jdf(new DateTime(targetYear, 1, 1), 0.0, birth.Timezone);
            var completed = (int)Math.Floor((yearStart - dJdfBirth) / 365.2563628) + 1;
            if (completed < 1) completed = 1;

            var dJdfVarsh = completed * 365.2563628 + dJdfBirth;
            int vDay, vMonth, vYear, vHour, vMinute;
            LocalFromJdf(dJdfVarsh, birth.Timezone, out vDay, out vMonth, out vYear, out vHour, out vMinute);

            var varshChart = calc.CalculateKundli(
                vDay, vMonth, vYear, vHour, vMinute,
                birth.Longitude, birth.Latitude, birth.Timezone);

            var birthLagnaSign = AstroMath.FnTgr(birthChart.Lagna / 30.0) + 1;
            var birthNak = AstroMath.FnTgr(AstroMath.FnR(birthChart.Moon * 3.0 / 40.0, 27.0)) + 1;
            var varshNak = AstroMath.FnTgr(AstroMath.FnR(varshChart.Moon * 3.0 / 40.0, 27.0)) + 1;
            var varshLagnaSign = AstroMath.FnTgr(varshChart.Lagna / 30.0) + 1;

            // Muntha: iFNR(completed + birthLagnaSignIndex + 1, 12) then -1 in print — VB uses FNTGR(dPltS_Birth(1))+1 as rasi
            var muntha = AstroMath.IFnR(completed + birthLagnaSign, 12);
            if (muntha == 0) muntha = 12;

            var dPlt = new double[11];
            dPlt[1] = varshChart.Lagna;
            dPlt[2] = varshChart.Sun;
            dPlt[3] = varshChart.Moon;
            dPlt[4] = varshChart.Mars;
            dPlt[5] = varshChart.Mercury;
            dPlt[6] = varshChart.Jupiter;
            dPlt[7] = varshChart.Venus;
            dPlt[8] = varshChart.Saturn;
            dPlt[9] = varshChart.Rahu;
            dPlt[10] = varshChart.Ketu;

            var result = new VarshPhalResult
            {
                NativeName = birth.Name ?? "Native",
                TargetYear = targetYear,
                CompletedYears = completed,
                AgeYearLabel = completed + 1,
                ParveshDateText = string.Format(CultureInfo.InvariantCulture, "{0:00}.{1:00}.{2:0000}", vDay, vMonth, vYear),
                ParveshTimeText = string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", vHour, vMinute),
                MunthaSign = muntha,
                MunthaSignName = Rashis[muntha],
                BirthNakshatra = Nakshatras[birthNak],
                VarshNakshatra = Nakshatras[varshNak],
                BirthLagnaSignName = Rashis[birthLagnaSign],
                VarshLagnaSignName = Rashis[varshLagnaSign]
            };

            string[] names = { "", "Lagna", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn", "Rahu", "Ketu" };
            for (var z = 1; z <= 10; z++)
            {
                var sign = AstroMath.FnTgr(dPlt[z] / 30.0) + 1;
                var house = AstroMath.IFnR(sign - varshLagnaSign, 12) + 1;
                result.VarshPlanets.Add(new NamedLongitude
                {
                    Name = names[z],
                    Longitude = dPlt[z],
                    Sign = sign,
                    SignName = Rashis[sign],
                    House = house
                });
            }

            // Mudda dasha
            var lordNum = AstroMath.IFnR(completed + birthNak - 2, 9);
            if (lordNum == 0) lordNum = 9;
            var dDasn = dJdfVarsh / 365.242194;
            for (var iMud = 1; iMud <= 9; iMud++)
            {
                var q = iMud + lordNum + 1;
                dDasn += MuddaYears[q] * (365.242194 / 120.0) / 365.242194;
                var end = AstroMath.Jd2Dt(dDasn * 365.242194);
                result.Mudda.Add(new MuddaPeriod
                {
                    Lord = MuddaLords[q],
                    EndDate = end,
                    Label = MuddaLords[q] + " : " + end
                });
            }

            // Sahams (core set)
            var night = AstroMath.FnR(dPlt[2] - dPlt[1], 360.0) < 180.0;
            double punya = 0, vidya = 0;
            AddSaham(result, "PUNYA", dPlt[3], dPlt[2], dPlt[1], night, out punya);
            AddSaham(result, "VIDYA", dPlt[2], dPlt[3], dPlt[1], night, out vidya);
            double yash;
            AddSaham(result, "YASH", dPlt[6], punya, dPlt[1], night, out yash);
            double tmp;
            AddSaham(result, "MAHATMYA", punya, dPlt[4], dPlt[1], night, out tmp);
            AddSaham(result, "ASHA", dPlt[8], dPlt[7], dPlt[1], night, out tmp);
            AddSaham(result, "PITRI", dPlt[8], dPlt[2], dPlt[1], night, out tmp);
            AddSaham(result, "MATRI", dPlt[3], dPlt[7], dPlt[1], night, out tmp);
            AddSaham(result, "PUTAR", dPlt[6], dPlt[3], dPlt[1], night, out tmp);
            AddSaham(result, "JEEVA", dPlt[8], dPlt[6], dPlt[1], night, out tmp);
            AddSaham(result, "KARMA", dPlt[4], dPlt[5], dPlt[1], night, out tmp);
            AddSaham(result, "ROG", dPlt[1], dPlt[3], dPlt[1], night, out tmp);
            AddSaham(result, "KALAH", dPlt[6], dPlt[4], dPlt[1], night, out tmp);
            AddSaham(result, "BANDHU", dPlt[5], dPlt[3], dPlt[1], night, out tmp);
            AddSaham(result, "VANIK", dPlt[3], dPlt[5], dPlt[1], night, out tmp);
            AddSaham(result, "VIVAH", dPlt[7], dPlt[8], dPlt[1], night, out tmp);
            AddSaham(result, "SHATRU", dPlt[4], dPlt[8], dPlt[1], night, out tmp);

            result.Notes.Add("Varsh Parvesh JD = completedYears × 365.2563628 + birth JD (qVarshPhal).");
            result.Notes.Add(night
                ? "Saham night-birth swap applied (Sun–Lagna arc < 180° at Parvesh)."
                : "Saham day formula (no night swap).");
            result.Notes.Add("Mudda lords cycle Ket–Bud; year parts sum to 120.");

            return result;
        }

        private static void AddSaham(
            VarshPhalResult result, string name, double a, double b, double lagna, bool night, out double value)
        {
            var x1 = a;
            var y1 = b;
            if (night)
            {
                x1 = b;
                y1 = a;
            }
            var iAa = AstroMath.FnR(x1 - y1, 360.0) < AstroMath.FnR(x1 - lagna, 360.0) ? 30.0 : 0.0;
            value = AstroMath.FnR(x1 - y1 + lagna + iAa, 360.0);
            result.Sahams.Add(new SahamRow
            {
                Name = name,
                Longitude = value,
                Rakv = AstroMath.FormatRakv(value)
            });
        }

        private static void LocalFromJdf(double dJdf, double tz, out int day, out int month, out int year, out int hour, out int minute)
        {
            var label = AstroMath.Jd2Dt(dJdf);
            var parts = label.Split('.');
            day = int.Parse(parts[0], CultureInfo.InvariantCulture);
            month = int.Parse(parts[1], CultureInfo.InvariantCulture);
            year = int.Parse(parts[2], CultureInfo.InvariantCulture);
            var localHours = AstroMath.FnR(24.0 * AstroMath.FnFr(dJdf) + tz, 24.0);
            hour = (int)Math.Floor(localHours + 1e-9);
            minute = (int)Math.Floor((localHours - hour) * 60.0 + 0.5);
            if (minute >= 60) { minute = 0; hour++; }
            if (hour >= 24) hour -= 24;
        }
    }
}
