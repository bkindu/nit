using System;
using NirmalaAstrology_Web.Services;
class Program {
  static void Main() {
    var fails = RakvFormatTests.RunAll(Console.WriteLine);
    // Explicit user case
    Console.WriteLine("Sun lon 295.65 => " + AstroMath.FormatRakv(295.65));
    Console.WriteLine("Wrong 1-based would be 10:25:39:00");
    Console.WriteLine("Moon gati 14.3553 deg/day => [" + AstroMath.FormatGatiKala(14.3553) + "]");
    Console.WriteLine("FAILS=" + fails);
    Environment.Exit(fails == 0 ? 0 : 1);
  }
}
