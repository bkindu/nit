using System.Collections.Generic;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Port of h16VrgTableGrahAvsthasCalcPrn + Uchadi_Avasthas (a2Functions_Antronomy):
    /// Baladi, Jagradadi, Deeptadi, Charadi, Arohi, Pravas.
    /// </summary>
    public sealed class GrahaAvasthaService
    {
        private static readonly string[] Names =
        {
            "", "", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn", "Rahu"
        };

        private static readonly string[] CharadiNames =
        {
            "", "", "Atma", "Amatya", "Bhratri", "Matri", "Putra", "Gnati", "Dara", "Pitri"
        };

        private static readonly string[] PravasNames =
        {
            "", "Pravas", "Nasta", "Mrit", "Jaya", "Hasya", "Rati", "Mud", "Supat", "Bhukat", "Javar", "Kamp", "Sthir"
        };

        // Uchadi tables from Uchadi_Avasthas
        private static readonly int[] Uch = { 0, 0, 1, 2, 10, 6, 4, 12, 7, 0 };
        private static readonly int[] Neech = { 0, 0, 7, 8, 4, 12, 10, 6, 1, 0 };
        private static readonly int[] Swa = { 0, 0, 5, 4, 1, 3, 9, 2, 10, 0 };
        private static readonly int[] Swa1 = { 0, 0, 0, 0, 8, 6, 12, 7, 11, 0 };

        public GrahaAvasthaResult Calculate(double[] dPlt)
        {
            var iRasi = new int[11];
            for (var z = 1; z <= 10; z++)
                iRasi[z] = AstroMath.FnTgr(dPlt[z] / 30.0) + 1;

            var uchadi = BuildUchadi(iRasi);

            // Charadi: rank planets 2–9 by absolute longitude (highest → Atma)
            var order = new List<int>();
            for (var z = 2; z <= 9; z++) order.Add(z);
            order.Sort((a, b) => dPlt[b].CompareTo(dPlt[a]));
            var karakOf = new string[11];
            for (var i = 0; i < order.Count; i++)
                karakOf[order[i]] = CharadiNames[i + 2];

            var result = new GrahaAvasthaResult();
            for (var z = 2; z <= 9; z++)
            {
                var iX = AstroMath.FnR(dPlt[z], 60.0);
                result.Rows.Add(new GrahaAvasthaRow
                {
                    Planet = Names[z],
                    Baladi = BaladiFrom60(iX),
                    Jagradadi = JagradFrom60(iX),
                    Deeptadi = DeeptadiFor(z, iRasi, uchadi),
                    CharadiKarak = karakOf[z] ?? "-",
                    Arohi = ArohiFor(z, dPlt[z]),
                    Pravas = PravasFor(dPlt[z])
                });
            }
            return result;
        }

        private static string BaladiFrom60(double iX)
        {
            if (iX <= 4) return "Balya";
            if (iX <= 12) return "Kumar";
            if (iX <= 18) return "Yuva";
            if (iX <= 24) return "Vridh";
            if (iX <= 30) return "Mrita";
            if (iX <= 36) return "Mrita";
            if (iX <= 42) return "Vridh";
            if (iX <= 48) return "Yuva";
            if (iX <= 54) return "Kumar";
            return "Balya";
        }

        private static string JagradFrom60(double iX)
        {
            var ix = (int)iX;
            if (ix <= 10) return "Jagrat";
            if (ix <= 20) return "Swapna";
            if (ix <= 30) return "Suspat";
            if (ix <= 40) return "Suspat";
            if (ix <= 50) return "Swapna";
            return "Jagrat";
        }

        private static string[] BuildUchadi(int[] iRasi)
        {
            var s = new string[11];
            for (var z = 2; z <= 9; z++)
            {
                if (z == 9) { s[z] = "Sama"; continue; } // Rahu: no Uchadi row in VB table for 9 the same way; treat Sama
                if (iRasi[z] == Uch[z]) s[z] = "Ucha";
                else if (iRasi[z] == Neech[z]) s[z] = "Neecha";
                else if (iRasi[z] == Swa[z] || iRasi[z] == Swa1[z]) s[z] = "Swa";
                else if (IsMitra(z, iRasi[z])) s[z] = "Mitra";
                else if (IsSatru(z, iRasi[z])) s[z] = "Satru";
                else s[z] = "Sama";
            }
            return s;
        }

        private static bool IsMitra(int z, int r)
        {
            // Compact port of iMitra* tables
            switch (z)
            {
                case 2: return r == 1 || r == 4 || r == 8 || r == 9 || r == 12;
                case 3: return r == 3 || r == 5 || r == 6;
                case 4: return r == 4 || r == 5 || r == 9 || r == 12;
                case 5: return r == 2 || r == 5 || r == 7;
                case 6: return r == 1 || r == 4 || r == 5 || r == 8;
                case 7: return r == 3 || r == 6 || r == 10 || r == 11;
                case 8: return r == 2 || r == 3 || r == 6 || r == 7;
                default: return false;
            }
        }

        private static bool IsSatru(int z, int r)
        {
            switch (z)
            {
                case 2: return r == 2 || r == 7 || r == 10 || r == 11;
                case 4: return r == 3 || r == 6;
                case 5: return r == 4;
                case 6: return r == 2 || r == 3 || r == 6 || r == 7;
                case 7: return r == 4 || r == 5;
                case 8: return r == 1 || r == 4 || r == 5 || r == 8;
                default: return false;
            }
        }

        private static string DeeptadiFor(int z, int[] iRasi, string[] uchadi)
        {
            string dip;
            // Khal if in Mars/Saturn signs; else Shant in other (except Sun-ruled handled below)
            if (iRasi[z] == 1 || iRasi[z] == 8 || iRasi[z] == 10 || iRasi[z] == 11)
                dip = "Khal";
            else
                dip = "Shant";

            switch (uchadi[z])
            {
                case "Ucha": dip = "Dipat"; break;
                case "Neecha": dip = "Deen"; break;
                case "Swa": dip = "Swast"; break;
                case "Mitra": dip = "Harshit"; break;
                case "Satru": dip = "AtiDuki"; break;
            }

            // Vikal if joins Mars or Saturn (except Mars/Saturn themselves)
            if (z != 4 && z != 8)
            {
                if (iRasi[z] == iRasi[4] || iRasi[z] == iRasi[8])
                    dip = "Vikal";
            }
            return dip;
        }

        private static string ArohiFor(int z, double lon)
        {
            // VB Select Case exalt longitudes (with known Venus typo using dPlt(6) — we use Venus lon)
            switch (z)
            {
                case 2: return (lon < 10 || lon > 190) ? "Arohi" : "Avrohi";
                case 3: return (lon < 33 || lon > 213) ? "Arohi" : "Avrohi";
                case 4: return (lon < 298 || lon > 118) ? "Arohi" : "Avrohi";
                case 5: return (lon < 165 || lon > 345) ? "Arohi" : "Avrohi";
                case 6: return (lon < 95 || lon > 275) ? "Arohi" : "Avrohi";
                case 7: return (lon < 357 || lon > 177) ? "Arohi" : "Avrohi";
                case 8: return (lon < 200 || lon > 20) ? "Arohi" : "Avrohi";
                case 9: return (lon < 90 || lon > 270) ? "Arohi" : "Avrohi";
                default: return "-";
            }
        }

        private static string PravasFor(double lon)
        {
            var iX = (int)AstroMath.FnR(AstroMath.FnFr(lon / 30.0) * 30.0 / 2.5 + 1.0, 12.0);
            if (iX == 0) iX = 12;
            return PravasNames[iX];
        }
    }
}
