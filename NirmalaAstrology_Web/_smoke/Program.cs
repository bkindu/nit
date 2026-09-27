using System;
using System.Linq;
using NirmalaAstrology_Web.Services;

namespace KundliSmoke
{
    internal static class Program
    {
        private static int Main()
        {
            // Patiala 08-Feb-1981 07:47 — VB6 Graha Spasta reference
            var calc = new KundliCalculator();
            var r = calc.CalculateKundli(8, 2, 1981, 7, 47, 76.25, 30.19, 5.30);
            var details = new KundliDetailsBuilder().Build(r);

            Console.WriteLine("JD={0:R}  Ayan={1}",
                r.JulianDayFraction,
                AstroMath.FormatAyanamsa(r.Ayanamsa));
            Console.WriteLine("{0,-8} {1,-14} {2,7} {3,-7} {4,-8} {5}",
                "Name", "RAKV", "Gati", "Motion", "Uday", "Nak");
            foreach (var p in details.Planets)
            {
                Console.WriteLine("{0,-8} {1,-14} {2,7} {3,-7} {4,-8} {5}/{6} {7} {8} {9}",
                    p.FullName,
                    p.PositionRakv,
                    p.Gati,
                    p.Motion,
                    p.UdayAsta,
                    p.NakshatraNumber,
                    p.NakshatraPada,
                    p.NakshatraLord,
                    p.NakshatraDevta,
                    p.Dignity);
            }

            Console.WriteLine();
            Console.WriteLine("VB6 expect:");
            Console.WriteLine("Lagna  10:06:13:46  -     -       -");
            Console.WriteLine("Surya  09:25:39:28  7:03  Sighar  Sadodya   (C# Sun Gati ~60' intentional)");
            Console.WriteLine("Chandra 11:07:09:35 861:19 Sighar Sadodya");
            Console.WriteLine("Mangla 10:07:18:35  47:16 Margi   Astha");
            Console.WriteLine("Budha  10:11:18:33  0:28  Vakri   Uday");
            Console.WriteLine("Guru   05:16:28:11  2:29  Vakri   Uday");
            Console.WriteLine("Sukra  09:11:13:15  75:00 Margi   Uday");
            Console.WriteLine("Sani   05:15:49:60  2:06  Vakri   Uday");
            Console.WriteLine("Rahu   03:16:54:35  3:11  Vakri   Astha");
            Console.WriteLine("Ketu   09:16:54:35  3:11  Vakri   Astha");
            return 0;
        }
    }
}
