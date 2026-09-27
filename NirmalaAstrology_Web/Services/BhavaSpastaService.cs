using System;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>Port of cBhavaSpastaAndChalitKundli.FnBhava_Calc / Chalit house assignment (Neelkhanth).</summary>
    public sealed class BhavaSpastaService
    {
        private static readonly string[] HouseNames =
        {
            "", "LAGAN", "SECOND", "THIRD", "FOURTH", "FIFTH", "SIXTH",
            "SEVENTH", "EIGHTH", "NINTH", "TENTH", "ELEVENTH", "TWELFTH"
        };

        private static readonly string[] PlanetNames =
        {
            "", "Lagna", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn", "Rahu", "Ketu"
        };

        /// <param name="dPlt">1=Lagna … 10=Ketu, 11=MC (Dasham). Same indexing as VB6.</param>
        public BhavaSpastaResult Calculate(double[] dPlt)
        {
            if (dPlt == null || dPlt.Length < 12)
                throw new ArgumentException("dPlt must include indices 1..11 (Lagna through MC).", "dPlt");

            var dBhavSpasta = new double[28];
            CalcBhavaSpasta(dPlt[1], dPlt[11], dBhavSpasta);

            var result = new BhavaSpastaResult
            {
                Lagna = dPlt[1],
                Midheaven = dPlt[11]
            };

            for (var z = 1; z <= 12; z++)
            {
                var madhya = dBhavSpasta[z * 2];
                var sandhi = dBhavSpasta[z * 2 + 1];
                result.Houses.Add(new BhavaHouseRow
                {
                    HouseNumber = z,
                    HouseName = HouseNames[z],
                    MadhyaDegrees = madhya,
                    SandhiDegrees = sandhi,
                    MadhyaRakv = AstroMath.FormatRakv(madhya),
                    SandhiRakv = AstroMath.FormatRakv(sandhi)
                });
            }

            var lagnaRashi0 = AstroMath.FnTgr(dPlt[1] / 30.0);
            for (var plt = 2; plt <= 10; plt++)
            {
                var hs = ResolveChalitHs(dPlt[plt], dBhavSpasta);
                var chalitHouse = AstroMath.FnTgr(AstroMath.FnR(hs + lagnaRashi0, 12)) + 1;
                var rashiHouse = ((AstroMath.FnTgr(dPlt[plt] / 30.0) - lagnaRashi0 + 12) % 12) + 1;
                result.ChalitPlanets.Add(new ChalitPlanetRow
                {
                    PlanetIndex = plt,
                    PlanetName = PlanetNames[plt],
                    Longitude = dPlt[plt],
                    ChalitHouse = chalitHouse,
                    RashiHouse = rashiHouse
                });
            }

            return result;
        }

        private static void CalcBhavaSpasta(double lagna, double mc, double[] dBhavSpasta)
        {
            var dSasthyans = AstroMath.FnR(lagna - mc, 360) / 3.0;
            double dNhouse = 10;
            while (dNhouse < 13)
            {
                var dBhav = AstroMath.FnR((dNhouse - 10) * dSasthyans + mc, 360);
                dBhavSpasta[AstroMath.FnTgr(dNhouse * 2)] = dBhav;
                var opposite = AstroMath.FnTgr((dNhouse - 6) * 2);
                dBhavSpasta[opposite] = dBhav + 180;
                if (dBhavSpasta[opposite] > 360) dBhavSpasta[opposite] -= 360;
                dNhouse += 0.5;
            }

            dSasthyans = 60 - dSasthyans;
            dNhouse = 1;
            while (dNhouse < 4)
            {
                var dBhav = AstroMath.FnR((dNhouse - 1) * dSasthyans + lagna, 360);
                dBhavSpasta[AstroMath.FnTgr(dNhouse * 2)] = dBhav;
                var opposite = AstroMath.FnTgr((dNhouse + 6) * 2);
                dBhavSpasta[opposite] = dBhavSpasta[AstroMath.FnTgr(dNhouse * 2)] + 180;
                if (dBhavSpasta[opposite] > 360) dBhavSpasta[opposite] -= 360;
                dNhouse += 0.5;
            }
        }

        private static int ResolveChalitHs(double planetLon, double[] dBhavSpasta)
        {
            var hs = 1;
            for (; hs <= 12; hs++)
            {
                var iX = 1 + hs * 2;
                dBhavSpasta[27] = dBhavSpasta[3];
                var dXdd = AstroMath.FnR(planetLon - dBhavSpasta[iX], 360);
                var dXrr = AstroMath.FnR(planetLon - dBhavSpasta[iX + 2], 360);
                if (dXrr > dXdd) break;
            }
            return hs;
        }
    }
}
