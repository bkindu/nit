using System;
using NirmalaAstrology_Web.Services;

namespace KundliSmoke
{
    internal static class PanchangProbe
    {
        private static int Main()
        {
            var calc = new KundliCalculator();
            var r = calc.CalculateKundli(8, 2, 1981, 7, 47, 76.25, 30.19, 5.30);
            var req = new KundliWorkflowRequest
            {
                Day = 8, Month = 2, Year = 1981, Hour = 7, Minute = 47,
                Longitude = 76.25, Latitude = 30.19, TimezoneOffset = 5.30
            };
            var p = new LegacyPanchangCalculator().Calculate(r, req);
            var rs = AstroMath.SunRiseSetForBirthDay(1981, 2, 8, 30.19, 76.25, 5.30);
            double dJdf = r.JulianDayFraction;
            double dLatt = AstroMath.FnDg(30.19);
            double dLong = AstroMath.FnDg(76.25);
            double ishat = AstroMath.IshatKaalGpDec(dJdf, dLatt, dLong);
            double birth = 7.0 + 47.0 / 60.0;
            double lmtDec = AstroMath.FnR(birth - (82.5 - dLong) * 4.0 / 60.0, 24.0) + 0.00005;
            Console.WriteLine("JD={0:R} rise={1:R} set={2:R} jRise={3:R}", dJdf, rs.SunRiseHours, rs.SunSetHours, rs.JulianRise);
            Console.WriteLine("ishatDec={0:R} packed={1} dinmanHours={2:R}", ishat, AstroMath.FormatRak(ishat), rs.SunSetHours - rs.SunRiseHours);
            Console.WriteLine("LMT dec={0:R} FNDM={1:R} print={2}", lmtDec, AstroMath.FnDm(lmtDec), AstroMath.FormatRak(AstroMath.FnDm(lmtDec)));
            Console.WriteLine("ST deg={0:R} print={1}", r.SiderealTime, AstroMath.FormatSiderealTimeHours(r.SiderealTime));
            Console.WriteLine("Samvatsar={0} Saka={1} Vikram={2} SunSign={3}", p.Samvatsar, p.SakaSamvat, p.VikramiSamvat, p.SunSign);
            Console.WriteLine("Isht={0} Dinman={1}", p.Ishtakal, p.Dinman);
            Console.WriteLine("TitEnd={0}", p.TithiEnding);
            Console.WriteLine("KrnEnd={0}", p.KaranEnding);
            Console.WriteLine("YogEnd={0}", p.YogaEnding);
            Console.WriteLine("NakEnd={0}", p.NakshatraEnding);
            Console.WriteLine("Tit B/B {0} {1}", p.TithiBhukat, p.TithiBhogya);
            Console.WriteLine("Yog B/B {0} {1}", p.YogaBhukat, p.YogaBhogya);
            Console.WriteLine("Krn B/B {0} {1}", p.KaranBhukat, p.KaranBhogya);
            Console.WriteLine("Nak B/B {0} {1}", p.NakshatraBhukat, p.NakshatraBhogya);
            Console.WriteLine("Nak vik {0} {1}", p.NakshatraBhukatVikala, p.NakshatraBhogyaVikala);
            Console.WriteLine("LMT={0} ST={1} Chara={2} MeanSun={3} GMT={4} Noon={5} Palbha={6}",
                p.LocalMeanTime, p.SiderealTime, p.CharaPal, p.MeanSun, p.Gmt, p.ApparentNoon, p.Palbha);
            Console.WriteLine("Moon gati deg={0:R}", r.PlanetPositions.Find(x => x.Name == "Moon").DailyMotion);
            Console.WriteLine("Sun gati deg={0:R}", r.PlanetPositions.Find(x => x.Name == "Sun").DailyMotion);
            return 0;
        }
    }
}
