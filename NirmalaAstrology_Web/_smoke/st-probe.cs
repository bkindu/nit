using System;
using NirmalaAstrology_Web.Services;
class S {
  static double FnFr(double x) { return x - Math.Floor(x); }
  static double FnR(double x, double m) { var r = FnFr(FnFr(x/m)+2)*m; return r; }
  static void Main() {
    double jd = 29624.095138888864;
    double lon = AstroMath.FnDg(76.25);
    double st = FnR(99.195 + 360*FnFr(jd) + lon + jd*0.98564735, 360);
    Console.WriteLine("ST lagna formula deg={0:R} rak={1}", st, AstroMath.FormatSiderealTimeHours(st));
    double st2 = FnR(16.6981583332 + 15*(FnFr(jd)*24 + 5.5) + lon + jd*0.985647335386 + jd*jd/3446481700000.0, 360);
    Console.WriteLine("ST ishat formula deg={0:R} rak={1}", st2, AstroMath.FormatSiderealTimeHours(st2));
    // search constant delta to hit 16:35:05
    for (double d = -2; d <= 2; d += 0.01) {
      var p = AstroMath.FormatSiderealTimeHours(st + d);
      if (p.Contains("16:35:0")) Console.WriteLine("delta={0:F3} print={1}", d, p);
    }
  }
}
