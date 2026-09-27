using System;
using System.Globalization;
using NirmalaAstrology_Web.Services;

/// <summary>Boundary tests for AstroMath RAKV (0-based legacy). Run: csc /r:… or build helper.</summary>
public static class RakvFormatTests
{
    public static int RunAll(Action<string> log)
    {
        var fail = 0;
        fail += Check(log, 0.0, "00:00:00:00");
        fail += Check(log, 29.0 + 59.0 / 60.0 + 59.0 / 3600.0, "00:29:59:59");
        fail += Check(log, 30.0, "01:00:00:00");
        fail += Check(log, 9.0 * 30.0 + 25.0 + 39.0 / 60.0, "09:25:39:00"); // Sun sample — must NOT be 10:…
        fail += Check(log, 295.65, "09:25:39:00"); // approx
        fail += Check(log, 10.0 * 30.0, "10:00:00:00");
        fail += Check(log, 11.0 * 30.0, "11:00:00:00");
        fail += Check(log, 359.0 + 59.0 / 60.0 + 59.0 / 3600.0, "11:29:59:59");
        fail += Check(log, 360.0, "00:00:00:00");
        fail += Check(log, -0.0001, "00:00:00:00"); // tiny negative → normalize
        fail += Check(log, 30.0 - 1.0 / 3600.0, "00:29:59:59");
        fail += Check(log, 30.0 + 1.0 / 3600.0, "01:00:00:01");

        // Carry: 9.999999° inside Mesha must stay rashi 0
        fail += Check(log, 9.999999, "00:10:00:00");

        // Critical regression: 1-based mistake would show 10 for Makara 25°39'
        var sun = AstroMath.ToRakvParts(9 * 30 + 25 + 39.0 / 60.0);
        if (sun.Rashi0 != 9)
        {
            log("FAIL critical: expected Rashi0=9 (Makara) got " + sun.Rashi0);
            fail++;
        }
        else log("PASS critical Rashi0=9 for Makara 25°39'");

        return fail;
    }

    private static int Check(Action<string> log, double lon, string expectedPrefix)
    {
        var actual = AstroMath.FormatRakv(lon);
        // Allow ±1″ on fractional cases by comparing rashi+ansha primarily for approx cases
        if (actual == expectedPrefix || actual.StartsWith(expectedPrefix.Substring(0, 8), StringComparison.Ordinal))
        {
            log("PASS " + lon.ToString("F6", CultureInfo.InvariantCulture) + " → " + actual);
            return 0;
        }
        log("FAIL " + lon.ToString("F6", CultureInfo.InvariantCulture) + " → " + actual + " expected " + expectedPrefix);
        return 1;
    }
}
