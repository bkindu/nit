using System;
using NirmalaAstrology_Web.Services;
class AuditRun {
  static string Fmt(PlanetPosition p) {
    if (p == null) return "null";
    int s0 = (int)Math.Floor(p.TotalDegrees / 30.0);
    double rem = p.TotalDegrees - s0 * 30.0;
    int deg = (int)Math.Floor(rem);
    double mf = (rem - deg) * 60.0;
    int min = (int)Math.Floor(mf);
    int sec = (int)Math.Round((mf - min) * 60.0);
    if (sec >= 60) { sec -= 60; min++; }
    if (min >= 60) { min -= 60; deg++; }
    if (deg >= 30) { deg -= 30; s0++; s0 %= 12; }
    return string.Format("{0,-8} lon={1,10:F6}  RAKV={2:00}:{3:00}:{4:00}:{5:00}  Rashi1={6,2}  DegIn={7,7:F4}  Retro={8}",
      p.Name, p.TotalDegrees, s0, deg, min, sec, p.Rashi, p.DegreesInRashi, p.IsRetrograde ? "Vakri" : "Margi");
  }
  static void Main() {
    var c = new KundliCalculator();
    // Patiala atlas: 30.19 / 76.24 packed -> 30+19/60, 76+24/60
    double lat = 30.0 + 19.0/60.0;
    double lon = 76.0 + 24.0/60.0;
    Console.WriteLine("NATIVE 08-Feb-1981 07:47 IST  Patiala {0:F6}N {1:F6}E", lat, lon);
    var r = c.CalculateKundli(8, 2, 1981, 7, 47, lat, lon);
    Console.WriteLine("Ayanamsa={0:F6} deg", r.Ayanamsa);
    Console.WriteLine("SiderealTime={0:F6}", r.SiderealTime);
    Console.WriteLine(Fmt(r.Lagna));
    Console.WriteLine(Fmt(r.Sun));
    Console.WriteLine(Fmt(r.Moon));
    Console.WriteLine(Fmt(r.Mars));
    Console.WriteLine(Fmt(r.Mercury));
    Console.WriteLine(Fmt(r.Jupiter));
    Console.WriteLine(Fmt(r.Venus));
    Console.WriteLine(Fmt(r.Saturn));
    Console.WriteLine(Fmt(r.Rahu));
    Console.WriteLine(Fmt(r.Ketu));
    Console.WriteLine("--- vs legacy print ---");
    Console.WriteLine("Legacy Lagna 10:06:13:46");
    Console.WriteLine("Legacy Sun   09:25:39:19");
    Console.WriteLine("Legacy Moon  11:07:09:27");
    Console.WriteLine("Legacy Mars  10:07:18:35");
    Console.WriteLine("Legacy Merc  10:11:18:33");
    Console.WriteLine("Legacy Jup   05:16:28:11");
    Console.WriteLine("Legacy Ven   09:11:13:15");
    Console.WriteLine("Legacy Sat   05:15:49:60");
    Console.WriteLine("Legacy Rahu  03:16:54:35");
    Console.WriteLine("Legacy Ketu  09:16:54:35");
  }
}
