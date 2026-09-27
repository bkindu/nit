using System;
using System.Collections.Generic;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Port of gSpl_LgnKundlies + jUpGrahLgnCalcPrn:
    /// Dhoomadi, Mantreshwar Gulikadi, Brihat Parasari Gulikadi.
    /// </summary>
    public sealed class SpecialLagnaUpGrahaService
    {
        private static readonly string[] Rashis =
        {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };

        public SpecialLagnaResult CalculateSpecialLagnas(double[] dPlt, int[,] iSodsVrg, double ishtakalGp)
        {
            var iRasi = new int[11];
            for (var z = 1; z <= 10; z++)
                iRasi[z] = AstroMath.FnTgr(dPlt[z] / 30.0) + 1;

            var bhavLon = AstroMath.FnR(dPlt[2] + ishtakalGp * 6.0, 360.0);
            var ghatiLon = AstroMath.FnR(dPlt[2] + ishtakalGp * 30.0, 360.0);
            var horaLon = AstroMath.FnR(dPlt[2] + ishtakalGp * 12.0, 360.0);
            var vrnada = AstroMath.FnR(dPlt[1] + horaLon, 360.0);
            if (((AstroMath.FnTgr(vrnada / 30.0) + 1) % 2) == 0)
                vrnada = AstroMath.FnR(360.0 - vrnada, 360.0);

            var padLagnesh = LagneshSign(iRasi[1], iRasi);
            var pad = AstroMath.IFnR((padLagnesh - iRasi[1]) * 2 + iRasi[1], 12);
            if (pad == 0) pad = 12;

            var upBase = iRasi[1] % 2 == 0
                ? AstroMath.IFnR(iRasi[1] - 1, 12)
                : AstroMath.IFnR(iRasi[1] + 1, 12);
            if (upBase == 0) upBase = 12;
            var upLagnesh = LagneshSign(upBase, iRasi);
            var uppad = AstroMath.IFnR((upLagnesh - upBase) * 2 + upBase, 12);
            if (uppad == 0) uppad = 12;

            var highest = 0.0;
            var atma = 2;
            for (var z = 2; z <= 8; z++)
            {
                var deg = AstroMath.FnR(dPlt[z], 30.0);
                if (deg > highest) { highest = deg; atma = z; }
            }
            var karkamsha = iSodsVrg[6, atma];

            var prana = AstroMath.FnR(ishtakalGp * 120.0 + dPlt[2] - 20.0 * Math.Floor(AstroMath.FnQ(dPlt[2])), 360.0);

            var result = new SpecialLagnaResult();
            Add(result, "Bhava Lagna", bhavLon);
            Add(result, "Ghati Lagna", ghatiLon);
            Add(result, "Hora Lagna", horaLon);
            Add(result, "Varnada Lagna", vrnada);
            result.Lagnas.Add(new NamedSign { Name = "Pada Lagna", Sign = pad, SignName = Rashis[pad], Longitude = (pad - 1) * 30.0 });
            result.Lagnas.Add(new NamedSign { Name = "Uppada Lagna", Sign = uppad, SignName = Rashis[uppad], Longitude = (uppad - 1) * 30.0 });
            result.Lagnas.Add(new NamedSign { Name = "Karakamsha Lagna", Sign = karkamsha, SignName = Rashis[karkamsha], Longitude = (karkamsha - 1) * 30.0 });
            Add(result, "Pranapada", prana);
            return result;
        }

        public UpGrahaResult CalculateUpGrahas(double[] dPlt)
        {
            return CalculateUpGrahas(dPlt, null, 0, 0, 0, 0, 5.5, 0);
        }

        public UpGrahaResult CalculateUpGrahas(
            double[] dPlt,
            KundliWorkflowRequest request,
            double ishtakalGp,
            double dinmaanGp,
            double ayanamsa,
            double astronomyJdf,
            double timezone,
            double latitude)
        {
            var result = new UpGrahaResult();

            // Dhoomadi (sun-based)
            var sun = dPlt[2];
            AddNamed(result.Dhoomadi, "Dhoom", AstroMath.FnR(sun + 133.333333, 360.0));
            AddNamed(result.Dhoomadi, "Indrachap", AstroMath.FnR(result.Dhoomadi[0].Longitude + 180.0, 360.0));
            AddNamed(result.Dhoomadi, "Vyatipat", AstroMath.FnR(360.0 - result.Dhoomadi[0].Longitude, 360.0));
            AddNamed(result.Dhoomadi, "Parivesh", AstroMath.FnR(result.Dhoomadi[2].Longitude + 180.0, 360.0));
            AddNamed(result.Dhoomadi, "Shikhi", AstroMath.FnR(sun - 30.0, 360.0));
            result.Points.AddRange(result.Dhoomadi);

            if (request == null || dinmaanGp <= 0)
                return result;

            var ratriGp = 60.0 - dinmaanGp;
            var localTime = AstroMath.FnDg(request.Hour + request.Minute / 100.0);
            var st = AstroMath.SiderealTimeDegrees(astronomyJdf, localTime, request.Longitude);
            // Hindu weekday Sun=1 … Sat=7 (matches .NET DayOfWeek+1)
            var iVaar = ((int)new DateTime(request.Year, request.Month, request.Day).DayOfWeek) + 1;
            var lat = latitude;

            // Mantreshwar Gulikadi (jUpGrah: 30ths of day/night)
            // Names from comment: KAAL…MANDI,Nireesh (Choose Z=8 is "-" but calc runs for 8)
            string[] mantNames = { "", "Kaal", "Paridhi", "Mrityu", "Ardh-Prahar", "Yama-Kantak", "Kodand", "Mandi", "Nireesh" };
            for (var z = 1; z <= 8; z++)
            {
                double dF1;
                if (ishtakalGp < dinmaanGp)
                    dF1 = AstroMath.FnR(-4 * iVaar + 4 * z - 28, 30) * dinmaanGp / 30.0;
                else
                    dF1 = AstroMath.FnR(-4 * iVaar + 4 * z - 14, 30) * ratriGp / 30.0 + dinmaanGp;

                var sidTimU = st + AstroMath.FnR(dF1 - ishtakalGp, 60.0) * 6.0;
                var dx = AscFromSidereal(sidTimU, lat, ayanamsa);
                AddNamed(result.Mantreshwar, mantNames[z], dx);
            }

            // Brihat Parasari Gulikadi (eighths of day/night)
            string[] bpNames = { "", "Kodand", "Gulika", "Nireesh", "Kaal", "Paridhi", "Mrityu", "Ardh-Prahar", "Yama-Ghantak" };
            for (var z = 1; z <= 8; z++)
            {
                double dF1;
                if (ishtakalGp < dinmaanGp)
                    dF1 = AstroMath.FnR(z - iVaar - 2, 8) * dinmaanGp / 8.0;
                else
                    dF1 = AstroMath.FnR(z - 4 - iVaar, 8) * ratriGp / 8.0 + dinmaanGp;

                // VB: (FNR(...)/2.5)*15 ≡ FNR(...)*6
                var sidTimU = st + (AstroMath.FnR(dF1 - ishtakalGp, 60.0) / 2.5) * 15.0;
                var dx = AscFromSidereal(sidTimU, lat, ayanamsa);
                AddNamed(result.Parasara, bpNames[z], dx);
            }

            result.Points.AddRange(result.Parasara);
            return result;
        }

        private static double AscFromSidereal(double sidTimU, double latitudeDeg, double ayanamsa)
        {
            var dx = AstroMath.FnAt(0.9174 * AstroMath.FnTn(sidTimU) + 0.398 * AstroMath.FnTn(latitudeDeg) / AstroMath.FnCs(sidTimU))
                     + 90 + sidTimU - AstroMath.FnAt(AstroMath.FnTn(sidTimU)) - ayanamsa;
            return AstroMath.FnR(dx, 360.0);
        }

        private static int LagneshSign(int lagnaSign, int[] iRasi)
        {
            if (lagnaSign == 1 || lagnaSign == 8) return iRasi[4];
            if (lagnaSign == 2 || lagnaSign == 7) return iRasi[7];
            if (lagnaSign == 3 || lagnaSign == 6) return iRasi[5];
            if (lagnaSign == 4) return iRasi[3];
            if (lagnaSign == 5) return iRasi[2];
            if (lagnaSign == 9 || lagnaSign == 12) return iRasi[6];
            return iRasi[8];
        }

        private static void Add(SpecialLagnaResult result, string name, double lon)
        {
            AddNamed(result.Lagnas, name, lon);
        }

        private static void AddNamed(List<NamedSign> list, string name, double lon)
        {
            var sign = AstroMath.FnTgr(lon / 30.0) + 1;
            list.Add(new NamedSign { Name = name, Longitude = lon, Sign = sign, SignName = Rashis[sign] });
        }
    }
}
