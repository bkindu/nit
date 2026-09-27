using System;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Port of VB6 kGrahBalaCalcPrn.Graha_Bala_Calc_Printing — core Shadbala for planets 2–8 (Sun–Saturn).
    /// </summary>
    public sealed class GrahaBalaService
    {
        private static readonly string[] PlanetNames =
        {
            "", "Lagna", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn"
        };

        // Debilitation longitudes for indices 2..8 (Sun..Saturn). Index 0 unused.
        private static readonly double[] NeechAnsha =
        {
            0, 0, 190, 213, 118, 345, 275, 177, 20
        };

        private static readonly int[] RasiLords =
        {
            0, 4, 7, 5, 3, 2, 5, 7, 4, 6, 8, 8, 6
        };

        private static readonly int[] MoolTrikona =
        {
            0, 0, 5, 2, 1, 6, 9, 7, 11
        };

        private static readonly int[] Swa1 =
        {
            0, 0, 5, 4, 1, 3, 9, 2, 10
        };

        private static readonly int[] Swa2 =
        {
            0, 0, 0, 0, 8, 6, 12, 7, 11
        };

        private static readonly double[] RequiredRupas =
        {
            0, 0, 6.5, 6.0, 5.0, 7.0, 6.5, 5.5, 5.0
        };

        private static readonly int[] SaptaVargas = { 1, 2, 3, 5, 6, 8, 9 };

        public GrahaBalaResult Calculate(
            double[] dPlt,
            int[,] iSodsVrg,
            string[,] sPanchMatri,
            double[] bhavaMadhya,
            KundliWorkflowRequest request,
            double ayanamsa)
        {
            if (dPlt == null || dPlt.Length < 11)
                throw new ArgumentException("dPlt must include indices 1..10.", "dPlt");
            if (iSodsVrg == null)
                throw new ArgumentNullException("iSodsVrg");
            if (sPanchMatri == null)
                throw new ArgumentNullException("sPanchMatri");
            if (bhavaMadhya == null || bhavaMadhya.Length < 13)
                throw new ArgumentException("bhavaMadhya must include indices 1..12.", "bhavaMadhya");
            if (request == null)
                throw new ArgumentNullException("request");

            var birthDate = new DateTime(request.Year, request.Month, request.Day);
            var birthHours = AstroMath.FnDg(request.Hour + request.Minute / 100.0);
            var sunRs = AstroMath.SunRiseSetForBirthDay(
                request.Year, request.Month, request.Day,
                request.Latitude, request.Longitude, request.TimezoneOffset);
            var sunrise = sunRs.SunRiseHours;
            var sunset = sunRs.SunSetHours;
            if (sunset < sunrise) sunset += 24.0;

            var dinmanHm = sunset - sunrise;
            if (dinmanHm <= 0) dinmanHm += 24.0;
            var ratriManHm = 24.0 - dinmanHm;

            var ishtHours = birthHours - sunrise;
            if (ishtHours < 0) ishtHours += 24.0;
            var birthIstKalGp = ishtHours * 2.5;
            var dinGhatis = dinmanHm * 2.5;
            var ratriGhatis = ratriManHm * 2.5;

            var jdf = AstroMath.Dt2Jdf(birthDate, birthHours, request.TimezoneOffset);
            var jdfSr = AstroMath.Dt2Jdf(birthDate, sunrise, request.TimezoneOffset);

            // Hindu weekday: Ravivar=1 … Shanivar=7 (.NET Sunday=0 → +1).
            var iVaarAnkHindu = (int)birthDate.DayOfWeek + 1;

            var iHsOfPlt = new int[11];
            var iRasi = new int[11];
            var lagnaRasi = AstroMath.FnTgr(dPlt[1] / 30.0) + 1;
            for (var z = 1; z <= 10; z++)
            {
                iRasi[z] = AstroMath.FnTgr(dPlt[z] / 30.0) + 1;
                iHsOfPlt[z] = AstroMath.IFnR(iRasi[z] - lagnaRasi, 12) + 1;
            }

            // --- Benefic / malefic Moon & Mercury for Drishti ---
            var sMonBenMal = "Shubha";
            var sMerBenMal = "Shubha";
            var tithiFrac = AstroMath.FnR(dPlt[3] - dPlt[2], 360.0) * 60.0 / 720.0;
            var tithiNum = AstroMath.FnTgr(tithiFrac) + 1;
            if (tithiNum >= 20 && tithiNum <= 30)
                sMonBenMal = "Papa";

            if (iHsOfPlt[5] == iHsOfPlt[2] || iHsOfPlt[5] == iHsOfPlt[4] || iHsOfPlt[5] == iHsOfPlt[8]
                || (sMonBenMal == "Papa" && iHsOfPlt[5] == iHsOfPlt[3]))
            {
                sMerBenMal = "Papa";
            }

            // --- 1. Drishti bal ---
            var dDrsti = new double[13, 13];
            var dDrstiBalOnGr = new double[13];
            for (var drsta = 2; drsta <= 8; drsta++)
            {
                for (var drist = 2; drist <= 8; drist++)
                {
                    if (drist == drsta) continue;

                    var dDiff = AstroMath.FnR(dPlt[drist] - dPlt[drsta], 360.0);
                    double dDrstiBlGr = 0;

                    if (dDiff > 30 && dDiff < 300)
                    {
                        if (dDiff > 180)
                            dDrstiBlGr = (300 - dDiff) / 2.0;
                        else if (dDiff > 150)
                            dDrstiBlGr = (dDiff - 150) * 2.0;
                        else if (dDiff > 120)
                            dDrstiBlGr = 150 - dDiff;
                        else if (dDiff > 90)
                            dDrstiBlGr = (120 - dDiff) / 2.0 + 30;
                        else if (dDiff > 60)
                            dDrstiBlGr = (dDiff - 60) + 15;
                        else if (dDiff > 30)
                            dDrstiBlGr = (dDiff - 30) / 2.0;
                    }

                    // Special aspects of Mars / Jupiter / Saturn (skip exact opposition band).
                    if (dDiff < 150 || dDiff > 180)
                    {
                        if (drsta == 4 && ((dDiff > 90 && dDiff < 120) || (dDiff > 210 && dDiff < 240)))
                            dDrstiBlGr += 15;
                        if (drsta == 6 && ((dDiff > 120 && dDiff < 250) || (dDiff > 240 && dDiff < 270)))
                            dDrstiBlGr += 30;
                        if (drsta == 8 && ((dDiff > 60 && dDiff < 90) || (dDiff > 270 && dDiff < 300)))
                            dDrstiBlGr += 45;
                    }

                    // Natural malefics negate; ksheena Moon / papa Mercury also negate.
                    if (drsta == 2 || drsta == 4 || drsta == 8)
                        dDrstiBlGr *= -1;
                    if (drsta == 3 && sMonBenMal == "Papa")
                        dDrstiBlGr *= -1;
                    if (drsta == 5 && sMerBenMal == "Papa")
                        dDrstiBlGr *= -1;

                    // Mercury & Jupiter full; others quarter (BV Raman).
                    if (drsta == 5 || drsta == 6)
                        dDrsti[drist, drsta] = dDrstiBlGr;
                    else
                        dDrsti[drist, drsta] = dDrstiBlGr / 4.0;
                }
            }

            for (var aspected = 2; aspected <= 8; aspected++)
            {
                for (var aspecting = 2; aspecting <= 8; aspecting++)
                    dDrstiBalOnGr[aspected] += dDrsti[aspected, aspecting];
            }

            // --- 2. Ucha ---
            var dUchaBal = new double[9];
            for (var z = 2; z <= 8; z++)
                dUchaBal[z] = AstroMath.PksBalEtc(dPlt[z], NeechAnsha[z]);

            // --- 3. Sapta varga ---
            var dSptVgBl = new double[10, 9];
            var dSptVrgTotBl = new double[9];
            for (var vi = 0; vi < SaptaVargas.Length; vi++)
            {
                var sptVrg = SaptaVargas[vi];
                for (var iPlt = 2; iPlt <= 8; iPlt++)
                {
                    var iGrRsi = iSodsVrg[sptVrg, iPlt];
                    if (iGrRsi < 1 || iGrRsi > 12) iGrRsi = ((iGrRsi % 12) + 12) % 12;
                    if (iGrRsi == 0) iGrRsi = 12;

                    double bal;
                    if (iGrRsi == MoolTrikona[iPlt])
                    {
                        bal = 45;
                    }
                    else if (iGrRsi == Swa1[iPlt] || iGrRsi == Swa2[iPlt])
                    {
                        bal = 30;
                    }
                    else
                    {
                        var lord = RasiLords[iGrRsi];
                        bal = FriendshipMarks(sPanchMatri, iPlt, lord);
                    }

                    dSptVgBl[sptVrg, iPlt] = bal;
                }
            }

            for (var iPlt = 2; iPlt <= 8; iPlt++)
            {
                dSptVrgTotBl[iPlt] =
                    dSptVgBl[1, iPlt] + dSptVgBl[2, iPlt] + dSptVgBl[3, iPlt]
                    + dSptVgBl[5, iPlt] + dSptVgBl[6, iPlt] + dSptVgBl[8, iPlt] + dSptVgBl[9, iPlt];
            }

            // --- 4. Oja-Yugma ---
            var dYAB = new double[9];
            for (var z = 2; z <= 8; z++)
            {
                var rasiSign = iSodsVrg[1, z];
                var navSign = iSodsVrg[6, z];
                if (z == 3 || z == 7)
                {
                    if (rasiSign % 2 == 0) dYAB[z] = 15;
                    if (navSign % 2 == 0) dYAB[z] += 15;
                }
                else
                {
                    if (rasiSign % 2 == 1) dYAB[z] = 15;
                    if (navSign % 2 == 1) dYAB[z] += 15;
                }
            }

            // --- 5. Kendra / Panaphara / Apoklima ---
            var dKPA = new double[9];
            for (var z = 2; z <= 8; z++)
            {
                var h = AstroMath.IFnR(
                    AstroMath.FnTgr(dPlt[z] / 30.0) - AstroMath.FnTgr(dPlt[1] / 30.0), 12) + 1;
                if (h == 1 || h == 4 || h == 7 || h == 10)
                    dKPA[z] = 60;
                else if (h == 2 || h == 5 || h == 8 || h == 11)
                    dKPA[z] = 30;
                else
                    dKPA[z] = 15;
            }

            // --- 6. Drekkana ---
            var dDRB = new double[9];
            for (var z = 2; z <= 8; z++)
            {
                var degInSign = AstroMath.FnR(dPlt[z], 30.0);
                if (z == 2 || z == 4 || z == 6)
                {
                    if (degInSign <= 10) dDRB[z] = 15;
                }
                else if (z == 5 || z == 8)
                {
                    if (degInSign > 10 && degInSign <= 20) dDRB[z] = 15;
                }
                else if (z == 3 || z == 7)
                {
                    if (degInSign > 20 && degInSign <= 30) dDRB[z] = 15;
                }
            }

            // --- 7. Sthana ---
            var dSthanBal = new double[9];
            for (var iPlt = 2; iPlt <= 8; iPlt++)
            {
                dSthanBal[iPlt] = dUchaBal[iPlt] + dSptVrgTotBl[iPlt] + dYAB[iPlt]
                                  + dKPA[iPlt] + dDRB[iPlt];
            }

            // --- 8. Dig (vs opposite-house madhya so max at strong house) ---
            var dDIB = new double[9];
            for (var z = 2; z <= 8; z++)
            {
                double dY;
                if (z == 2 || z == 4)
                    dY = bhavaMadhya[4];
                else if (z == 3 || z == 7)
                    dY = bhavaMadhya[10];
                else if (z == 5 || z == 6)
                    dY = bhavaMadhya[7];
                else
                    dY = bhavaMadhya[1];
                dDIB[z] = AstroMath.PksBalEtc(dPlt[z], dY);
            }

            // --- 9. Natonnata ---
            var dArdhRatriHm = (dinmanHm + ratriManHm) / 2.0;
            var dNutUnatBal = new double[9];
            for (var z = 2; z <= 8; z++)
            {
                var nut = AstroMath.FnR(birthIstKalGp - (dArdhRatriHm * 2.5), 60.0);
                if (nut > 30)
                    nut = (60 - nut) * 2;
                else
                    nut = nut * 2;

                if (z == 3 || z == 4 || z == 8)
                    nut = 60 - nut;
                dNutUnatBal[z] = nut;
            }
            dNutUnatBal[5] = 60;

            // --- 10. Paksha ---
            var dPaksaBal = new double[9];
            var pakshaX = AstroMath.PksBalEtc(dPlt[3], dPlt[2]);
            dPaksaBal[2] = 60 - pakshaX;
            dPaksaBal[3] = pakshaX;
            dPaksaBal[4] = 60 - pakshaX;
            dPaksaBal[5] = pakshaX;
            dPaksaBal[6] = pakshaX;
            dPaksaBal[7] = pakshaX;
            dPaksaBal[8] = 60 - pakshaX;

            // --- 11. Tribhaga ---
            var dDNB = new double[9];
            dDNB[6] = 60;
            if (birthIstKalGp > dinGhatis)
            {
                var tri = (birthIstKalGp - dinGhatis) / Math.Max(ratriGhatis, 1e-9) * 3.0;
                if (tri < 1) dDNB[2] = 60;
                else if (tri < 2) dDNB[7] = 60;
                else if (tri < 3) dDNB[4] = 60;
            }
            else
            {
                var tri = birthIstKalGp / Math.Max(dinGhatis, 1e-9) * 3.0;
                if (tri < 1) dDNB[5] = 60;
                else if (tri < 2) dDNB[2] = 60;
                else if (tri < 3) dDNB[8] = 60;
            }

            // --- 12–14. ISB Abda / Masa / Vara / Hora ---
            var dISB_V = new double[9];
            var dISB_M = new double[9];
            var dISB_D = new double[9];
            var dISB_H = new double[9];
            var dISB = new double[9];

            var iVarsPati = (int)(AstroMath.FnR(
                Math.Floor((jdfSr - 818.0) / 360.0) * 3.0, 7.0) + 2);
            AssignIsb(dISB_V, iVarsPati, 15);

            var iMaspati = AstroMath.FnTgr(
                AstroMath.FnR(Math.Floor((jdfSr - 818.0) / 30.0) * 2.0, 7.0) + 2);
            AssignIsb(dISB_M, iMaspati, 30);

            AssignIsb(dISB_D, iVaarAnkHindu, 45);

            int iX;
            if (birthIstKalGp < dinGhatis)
                iX = AstroMath.FnTgr(birthIstKalGp / Math.Max(dinGhatis, 1e-9) * 12.0);
            else
                iX = 12 + AstroMath.FnTgr(
                    (birthIstKalGp - dinGhatis) / Math.Max(ratriGhatis, 1e-9) * 12.0);

            var iHoraPati = AstroMath.IFnR(iX * 5 + iVaarAnkHindu + 1, 7) - 1;
            if (iHoraPati < 1) iHoraPati += 7;
            AssignIsb(dISB_H, iHoraPati, 60);

            for (var z = 2; z <= 8; z++)
                dISB[z] = dISB_V[z] + dISB_M[z] + dISB_D[z] + dISB_H[z];

            // --- 15. Ayana (Raman / Jasdeep style) ---
            var dAyanBal = new double[9];
            var ayanX = AstroMath.PksBalEtc(dPlt[2] + ayanamsa, 270.0);
            dAyanBal[2] = ayanX;
            dAyanBal[3] = 60 - ayanX;
            dAyanBal[4] = ayanX;
            dAyanBal[5] = ayanX;
            dAyanBal[6] = ayanX;
            dAyanBal[7] = ayanX;
            dAyanBal[8] = 60 - ayanX;

            // --- 16. Kala (includes Dig; Dig also counted once in Shadbala total) ---
            var kalaBal = new double[9];
            for (var iPlt = 2; iPlt <= 8; iPlt++)
            {
                kalaBal[iPlt] = dDIB[iPlt] + dNutUnatBal[iPlt] + dPaksaBal[iPlt]
                                + dDNB[iPlt] + dISB[iPlt] + dAyanBal[iPlt];
            }

            // --- 17. Cheshta ---
            var mean = AstroMath.MeanPlanets(jdf);
            var dCHB = new double[9];
            dCHB[2] = dAyanBal[2];
            dCHB[3] = dPaksaBal[3];
            for (var z = 4; z <= 8; z++)
            {
                double dX;
                if (z == 4 || z == 6 || z == 8)
                {
                    dX = (mean[z] + dPlt[z]) / 2.0 - mean[2];
                    dX = AstroMath.FnR(dX, 360.0);
                }
                else
                {
                    dX = AstroMath.FnR((mean[2] + dPlt[z]) / 2.0 - mean[z], 360.0);
                }
                dCHB[z] = AstroMath.PksBalEtc(dX, 180.0);
            }

            // --- 18. Naisarga ---
            var dNsrgikBal = new double[9];
            dNsrgikBal[2] = 60;
            dNsrgikBal[3] = 60.0 * 6.0 / 7.0;
            dNsrgikBal[4] = 60.0 * 2.0 / 7.0;
            dNsrgikBal[5] = 60.0 * 3.0 / 7.0;
            dNsrgikBal[6] = 60.0 * 4.0 / 7.0;
            dNsrgikBal[7] = 60.0 * 5.0 / 7.0;
            dNsrgikBal[8] = 60.0 / 7.0;

            // --- 19. Shadbala total (VB L613–617 component sum; Dig once) ---
            var dSadBalTot = new double[9];
            for (var z = 2; z <= 8; z++)
            {
                dSadBalTot[z] =
                    dDrstiBalOnGr[z] + dUchaBal[z] + dDIB[z] + dAyanBal[z]
                    + dCHB[z] + dPaksaBal[z] + dDRB[z] + dDNB[z]
                    + dKPA[z] + dYAB[z] + dISB[z] + dNsrgikBal[z]
                    + dNutUnatBal[z] + dSptVrgTotBl[z];
            }

            var result = new GrahaBalaResult();
            for (var z = 2; z <= 8; z++)
            {
                var rupas = dSadBalTot[z] / 60.0;
                var required = RequiredRupas[z];
                result.Planets.Add(new GrahaBalaPlanetRow
                {
                    PlanetIndex = z,
                    Planet = PlanetNames[z],
                    PlanetName = PlanetNames[z],
                    Drishti = dDrstiBalOnGr[z],
                    Ucha = dUchaBal[z],
                    SaptaVarga = dSptVrgTotBl[z],
                    OjaYugma = dYAB[z],
                    Kendra = dKPA[z],
                    Drekkana = dDRB[z],
                    Sthana = dSthanBal[z],
                    Dig = dDIB[z],
                    Natonnata = dNutUnatBal[z],
                    Paksha = dPaksaBal[z],
                    Tribhaga = dDNB[z],
                    Isb = dISB[z],
                    Ayana = dAyanBal[z],
                    Kala = kalaBal[z],
                    Cheshta = dCHB[z],
                    Naisarga = dNsrgikBal[z],
                    Shadbala = dSadBalTot[z],
                    ShadbalaTotal = dSadBalTot[z],
                    Rupas = rupas,
                    Required = required,
                    RequiredRupas = required,
                    Percent = required > 0 ? rupas / required * 100.0 : 0
                });
            }

            // --- Bhava bala (simplified) ---
            var bm1 = bhavaMadhya[1];
            var bm4 = bhavaMadhya[4];
            var bm7 = bhavaMadhya[7];
            var bm10 = bhavaMadhya[10];

            for (var hs = 1; hs <= 12; hs++)
            {
                var mid = bhavaMadhya[hs];
                var dig = HouseDigBala(mid, bm1, bm4, bm7, bm10);

                var signOfHouse = AstroMath.IFnR(lagnaRasi + hs - 2, 12) + 1;
                var lord = RasiLords[signOfHouse];
                var lordShadbala = (lord >= 2 && lord <= 8) ? dSadBalTot[lord] : 0;

                result.Bhavas.Add(new BhavaBalaRow
                {
                    House = hs,
                    LordPlanetIndex = lord,
                    LordPlanetName = (lord >= 2 && lord <= 8) ? PlanetNames[lord] : "",
                    Dig = dig,
                    LordShadbala = lordShadbala,
                    Total = lordShadbala / 4.0 + dig
                });
            }

            return result;
        }

        private static void AssignIsb(double[] target, int pati, double marks)
        {
            // Map weekday/lord index 1..7 → planet slots 2..8.
            if (pati < 1 || pati > 7) return;
            target[pati + 1] = marks;
        }

        private static double FriendshipMarks(string[,] sPanchMatri, int planet, int lord)
        {
            if (lord < 2 || lord > 8 || planet < 2 || planet > 8)
                return 0;

            string label = null;
            try
            {
                label = sPanchMatri[planet, lord];
            }
            catch (IndexOutOfRangeException)
            {
                return 0;
            }

            if (string.IsNullOrEmpty(label)) return 0;
            label = label.Trim();

            if (string.Equals(label, "G.Friend", StringComparison.OrdinalIgnoreCase)) return 22.5;
            if (string.Equals(label, "Friend", StringComparison.OrdinalIgnoreCase)) return 15;
            if (string.Equals(label, "Neutral", StringComparison.OrdinalIgnoreCase)) return 7.5;
            if (string.Equals(label, "Enemy", StringComparison.OrdinalIgnoreCase)) return 3.75;
            if (string.Equals(label, "G.Enemy", StringComparison.OrdinalIgnoreCase)) return 1.8;
            return 0;
        }

        /// <summary>
        /// Simplified house dig from VB rasi-class vs 1/4/7/10 madhya (diff/3).
        /// </summary>
        private static double HouseDigBala(double mid, double bm1, double bm4, double bm7, double bm10)
        {
            double dX;
            if ((mid <= 60)
                || (mid > 120 && mid <= 150)
                || (mid > 255 && mid <= 285))
            {
                dX = AstroMath.FnR(mid - bm4, 360.0);
            }
            else if ((mid > 60 && mid <= 90)
                     || (mid > 150 && mid <= 210)
                     || (mid > 240 && mid <= 255)
                     || (mid > 300 && mid <= 330))
            {
                dX = AstroMath.FnR(mid - bm7, 360.0);
            }
            else if (mid > 210 && mid <= 240)
            {
                dX = AstroMath.FnR(mid - bm1, 360.0);
            }
            else
            {
                dX = AstroMath.FnR(mid - bm10, 360.0);
            }

            if (dX > 180) dX = 360 - dX;
            return dX / 3.0;
        }
    }
}
