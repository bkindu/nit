using System;
using System.Collections.Generic;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>Port of f16Vrg_Kundlies_CalcPrn.FnShodashVargCalc / SodsVrgStor.</summary>
    public sealed class Varga16Service
    {
        private static readonly string[] PlanetNames =
        {
            "", "Lagna", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn", "Rahu", "Ketu"
        };

        private static readonly string[] RashiNames =
        {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };

        // Display names match Varg16_Table_Printing (RASHI first). Chart pair titles use CHANDER for D-1 display.
        private static readonly string[] VargaNames =
        {
            "", "RASHI", "HORA", "DRESHKANA", "CHATURTHAMSHA", "SAPTAMSHA", "NAVAMSHA",
            "DASMAMSHA", "DWADSHAMSHA", "TRISHAMSA", "SODSHAMSHA", "VIMSHAMSHA", "SIDHAMSA",
            "BHAMSHAMSHA", "KHAVEDAMSHA", "AKASVEDAMSHA", "SHASTAYAMSHA"
        };

        private static readonly string[] VargaCodes =
        {
            "", "D-1", "D-2", "D-3", "D-4", "D-7", "D-9",
            "D-10", "D-12", "D-30", "D-16", "D-20", "D-24",
            "D-27", "D-40", "D-45", "D-60"
        };

        private static readonly string[] VargaPurposes =
        {
            "", "Physique / Rashi", "Wealth", "Co-Borns", "Fortunes", "Children/Grand", "Spouse",
            "Power/Position", "Parents", "Evil Effects", "Conveyances", "Science/Worship", "Learning",
            "Strength/Weakness", "Auspicious/Inauspicious", "All Indications", "All Indications"
        };

        /// <param name="dPlt">1=Lagna … 10=Ketu (MC unused).</param>
        public Varga16Result Calculate(double[] dPlt)
        {
            if (dPlt == null || dPlt.Length < 11)
                throw new ArgumentException("dPlt must include indices 1..10.", "dPlt");

            var iRasi = new int[11];
            var iSodsVrg = new int[17, 11];
            var iSodasV = 0;

            Action store = () =>
            {
                iSodasV++;
                for (var iGrh = 1; iGrh <= 10; iGrh++)
                    iSodsVrg[iSodasV, iGrh] = iRasi[iGrh];
            };

            // 1. Rashi
            for (var z = 1; z <= 10; z++)
                iRasi[z] = AstroMath.FnTgr(dPlt[z] / 30.0) + 1;
            store();

            // 2. Hora
            for (var z = 1; z <= 10; z++)
            {
                var iX = AstroMath.FnTgr((dPlt[z] + 15.0) / 30.0);
                iRasi[z] = -AstroMath.IFnR(iX, 2) + 5;
            }
            store();

            // 3. Dreshkana
            for (var z = 1; z <= 10; z++)
            {
                var iX = AstroMath.IFnR(AstroMath.FnTgr(dPlt[z] / 10.0), 3) * 4
                         + AstroMath.FnTgr(dPlt[z] / 30.0) + 1;
                iRasi[z] = AstroMath.IFnR(iX, 12);
                if (iRasi[z] == 0) iRasi[z] = 12;
            }
            store();

            // 4. Chaturthamsa
            for (var z = 1; z <= 10; z++)
            {
                var iX = AstroMath.IFnR(AstroMath.FnTgr(dPlt[z] / 7.5), 4) * 3
                         + AstroMath.FnTgr(dPlt[z] / 30.0) + 1;
                iRasi[z] = AstroMath.IFnR(iX, 12);
                if (iRasi[z] == 0) iRasi[z] = 12;
            }
            store();

            // 5. Saptamsha
            for (var z = 1; z <= 10; z++)
                iRasi[z] = AstroMath.FnTgr(AstroMath.FnR(7.0 * dPlt[z], 360.0) / 30.0) + 1;
            store();

            // 6. Navamsha
            for (var z = 1; z <= 10; z++)
                iRasi[z] = AstroMath.FnTgr(AstroMath.FnR(9.0 * dPlt[z], 360.0) / 30.0) + 1;
            store();

            // 7. Dasamamsha
            for (var z = 1; z <= 10; z++)
            {
                var iX = AstroMath.FnTgr(
                    AstroMath.FnC2(10.0, dPlt[z])
                    + AstroMath.FnTgr(AstroMath.FnQ(dPlt[z]))
                    + 8.0 * AstroMath.FnC3(dPlt[z])
                    + 1.0);
                iRasi[z] = AstroMath.IFnR(iX, 12);
                if (iRasi[z] == 0) iRasi[z] = 12;
            }
            store();

            // 8. Dwadshamsha
            for (var z = 1; z <= 10; z++)
            {
                var iX = AstroMath.FnTgr(dPlt[z] / 30.0) + AstroMath.FnTgr(dPlt[z] / 2.5) + 1;
                iRasi[z] = AstroMath.IFnR(iX, 12);
                if (iRasi[z] == 0) iRasi[z] = 12;
            }
            store();

            // 9. Trishamsa (D-30)
            for (var z = 1; z <= 10; z++)
            {
                var dX = AstroMath.FnR(dPlt[z], 60.0);
                if (dX <= 5) iRasi[z] = 1;
                else if (dX <= 10) iRasi[z] = 11;
                else if (dX <= 18) iRasi[z] = 9;
                else if (dX <= 25) iRasi[z] = 3;
                else if (dX <= 30) iRasi[z] = 7;
                else if (dX <= 35) iRasi[z] = 2;
                else if (dX <= 42) iRasi[z] = 6;
                else if (dX <= 50) iRasi[z] = 12;
                else if (dX <= 55) iRasi[z] = 10;
                else iRasi[z] = 8;
            }
            store();

            // 10. Sodashamsha
            for (var z = 1; z <= 10; z++)
                iRasi[z] = AstroMath.FnTgr(AstroMath.FnR(16.0 * dPlt[z], 360.0) / 30.0) + 1;
            store();

            // 11. Vimshamsha
            for (var z = 1; z <= 10; z++)
                iRasi[z] = AstroMath.FnTgr(AstroMath.FnR(20.0 * dPlt[z], 360.0) / 30.0) + 1;
            store();

            // 12. Sidhamsa
            for (var z = 1; z <= 10; z++)
            {
                var iX = AstroMath.FnTgr(AstroMath.FnC2(24.0, dPlt[z]) - AstroMath.FnC3(dPlt[z]) + 5.0);
                iRasi[z] = AstroMath.IFnR(iX, 12);
                if (iRasi[z] == 0) iRasi[z] = 12;
            }
            store();

            // 13. Bhamshamsha
            for (var z = 1; z <= 10; z++)
                iRasi[z] = AstroMath.FnTgr(AstroMath.FnR(27.0 * dPlt[z], 360.0) / 30.0) + 1;
            store();

            // 14. Khavedamsa
            for (var z = 1; z <= 10; z++)
            {
                var iX = AstroMath.FnTgr(AstroMath.FnC2(40.0, dPlt[z]) + AstroMath.FnC3(dPlt[z]) * 6.0 + 1.0);
                iRasi[z] = AstroMath.IFnR(iX, 12);
                if (iRasi[z] == 0) iRasi[z] = 12;
            }
            store();

            // 15. Akasvedamsha
            for (var z = 1; z <= 10; z++)
            {
                var iX = AstroMath.FnTgr(
                    AstroMath.FnC2(45.0, dPlt[z])
                    + 4.0 * Math.Floor(AstroMath.FnR(dPlt[z], 90.0) / 30.0));
                iRasi[z] = AstroMath.IFnR(iX, 12) + 1;
            }
            store();

            // 16. Shashtayamsha
            for (var z = 1; z <= 10; z++)
            {
                var iX = AstroMath.FnTgr(dPlt[z] / 30.0) + AstroMath.FnTgr(dPlt[z] / 0.5) + 1;
                iRasi[z] = AstroMath.IFnR(AstroMath.IFnR(iX, 60), 12);
                if (iRasi[z] == 0) iRasi[z] = 12;
            }
            store();

            var result = new Varga16Result { Signs = iSodsVrg };
            for (var v = 1; v <= 16; v++)
            {
                var signs = new int[11];
                for (var p = 1; p <= 10; p++) signs[p] = iSodsVrg[v, p];

                result.TableRows.Add(new VargaTableRow
                {
                    Index = v,
                    Name = VargaNames[v],
                    Code = VargaCodes[v],
                    PlanetSigns = signs
                });

                var lagnaSign = iSodsVrg[v, 1];
                var chart = new VargaChartRow
                {
                    Index = v,
                    Code = VargaCodes[v],
                    Name = VargaNames[v],
                    Purpose = VargaPurposes[v],
                    LagnaSign = lagnaSign,
                    LagnaSignName = RashiNames[lagnaSign]
                };
                for (var p = 1; p <= 10; p++)
                {
                    var sign = iSodsVrg[v, p];
                    chart.Planets.Add(new VargaPlanetSign
                    {
                        PlanetName = PlanetNames[p],
                        Sign = sign,
                        SignName = RashiNames[sign],
                        HouseFromLagna = ((sign - lagnaSign + 12) % 12) + 1
                    });
                }
                result.Charts.Add(chart);
            }

            result.Deities = BuildDeities(dPlt, iSodsVrg);
            return result;
        }

        private static List<VargaDeityRow> BuildDeities(double[] dPlt, int[,] iSodsVrg)
        {
            var list = new List<VargaDeityRow>();
            string[] shortNames = { "", "Lag", "Sun", "Mon", "Mar", "Mer", "Jup", "Ven", "Sat" };
            for (var iVrg = 2; iVrg <= 9; iVrg++)
            {
                for (var iPlt = 2; iPlt <= 8; iPlt++)
                {
                    var dX = AstroMath.FnFr(dPlt[iPlt] / 30.0) * 30.0;
                    var iY = iSodsVrg[iVrg, iPlt] % 2;
                    var deity = ResolveDeity(iVrg, dX, iY, dPlt[iPlt], iSodsVrg[iVrg, iPlt]);
                    if (string.IsNullOrEmpty(deity)) continue;
                    list.Add(new VargaDeityRow
                    {
                        VargaCode = VargaCodes[iVrg],
                        VargaName = VargaNames[iVrg],
                        PlanetName = shortNames[iPlt],
                        Deity = deity
                    });
                }
            }
            return list;
        }

        private static string ResolveDeity(int iVrg, double dX, int iY, double lon, int vargaSign)
        {
            int iX;
            switch (iVrg)
            {
                case 2:
                    if (iY == 0) iX = dX <= 15 ? 1 : 2;
                    else iX = dX > 15 ? 1 : 2;
                    return Pick(iX, "Pitar", "Deviyan");
                case 3:
                    iX = (int)(dX / 10.0 + 0.5);
                    return Pick(iX, "Narad", "Agastya", "Durvasa");
                case 4:
                    iX = (int)(dX / 7.5 + 0.5);
                    return Pick(iX, "Sanak", "Sanandan", "SanatKumar", "Sanatan");
                case 5:
                    iX = (int)(dX / 4.28563888888889 + 0.5);
                    return Pick(iX, "Ksar", "Ksheer", "Dadhi", "Aajya", "Iksuras", "Madya", "ShudhJal");
                case 6:
                    iX = (int)(dX / 3.33333333333333 + 0.5);
                    return Pick(iX, "Dev", "Nar", "Rakshas", "Dev", "Nar", "Rakshas", "Dev", "Nar", "Rakshas");
                case 7:
                    iX = (int)(dX / 3.0 + 0.5);
                    if (iY == 0) iX = 10 - iX + 1;
                    return Pick(iX, "Inder", "Agani", "Yama", "Rakshas", "Varun", "Maruta", "Kuber", "Ishan", "Brahma", "Anant");
                case 8:
                    iX = (int)(dX / 2.5 + 0.5);
                    return Pick(iX, "Ganesh", "Aswini", "Yam", "Ahi", "Ganesh", "Aswini", "Yam", "Ahi", "Ganesh", "Aswini", "Yam", "Ahi");
                case 9:
                    var dY = AstroMath.FnR(lon, 60.0);
                    if (dY <= 5 || dY > 55) return "Vanhi";
                    if (dY <= 10 || dY > 50) return "Vayu";
                    if (dY <= 18 || dY > 42) return "Inder";
                    if (dY <= 25 || dY > 35) return "Dhanad";
                    return "Jalad";
                default:
                    return null;
            }
        }

        private static string Pick(int iX, params string[] names)
        {
            if (iX < 1) iX = 1;
            if (iX > names.Length) iX = names.Length;
            return names[iX - 1];
        }
    }
}
