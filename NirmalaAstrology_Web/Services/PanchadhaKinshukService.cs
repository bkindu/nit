using System;
using System.Collections.Generic;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>Port of iPanchadhaKinshukadi Panchadha_Matri_Calc + KinshukAdi_Sangya.</summary>
    public sealed class PanchadhaKinshukService
    {
        private static readonly string[] Names = { "", "", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn" };
        private static readonly string[] ShodashaNames =
        {
            "", "Bhedak", "Kusum", "Nagpushp", "Kanduk", "Kerala", "Kalpavriksha", "Chandanvan", "Poornachandra",
            "Uchchaisrava", "Dhanvantari", "Suryakant", "Vidrum", "Shakrasimhasan", "Paravat", "Devalok", "Srivallabh"
        };
        private static readonly string[] DasaNames =
        {
            "", "Parijat", "Uttam", "Gopur", "Simhasan", "Paravat", "Devalok", "Brahmalok", "Sairalok", "Sridham", "Shrivallabh"
        };
        private static readonly string[] SaptaNames =
        {
            "", "Kinshuk", "Vyanjan", "Chamar", "Chatra", "Kundal", "Keval", "Mukut"
        };

        public PanchadhaResult CalculatePanchadha(double[] dPlt)
        {
            var nais = new int[9, 9];
            // Natural (Parasara)
            int[] sr = { 0, 0, 0, 1, 1, 0, 1, -1, -1 };
            int[] cn = { 0, 0, 1, 0, 0, 1, 0, 0, 0 };
            int[] mg = { 0, 0, 1, 1, 0, -1, 1, 0, 0 };
            int[] bd = { 0, 0, 1, -1, 0, 0, 0, 1, 0 };
            int[] gr = { 0, 0, 1, 1, 1, -1, 0, -1, 0 };
            int[] sk = { 0, 0, -1, -1, 0, 1, 0, 0, 1 };
            int[] sn = { 0, 0, -1, -1, -1, 1, 0, 1, 0 };
            for (var z = 2; z <= 8; z++)
            {
                if (z != 2) nais[2, z] = sr[z];
                if (z != 3) nais[3, z] = cn[z];
                if (z != 4) nais[4, z] = mg[z];
                if (z != 5) nais[5, z] = bd[z];
                if (z != 6) nais[6, z] = gr[z];
                if (z != 7) nais[7, z] = sk[z];
                if (z != 8) nais[8, z] = sn[z];
            }

            var iRasi = new int[9];
            for (var z = 2; z <= 8; z++)
                iRasi[z] = AstroMath.FnTgr(dPlt[z] / 30.0) + 1;

            var tat = new int[9, 9];
            for (var z = 2; z <= 8; z++)
            {
                for (var y = 2; y <= 8; y++)
                {
                    var iX = AstroMath.IFnR(iRasi[y] - iRasi[z], 12);
                    tat[z, y] = ((iX >= 1 && iX <= 3) || (iX >= 10 && iX <= 12)) ? 1 : -1;
                }
            }

            var result = new PanchadhaResult();
            for (var z = 2; z <= 8; z++)
            {
                var row = new PanchadhaRow { Planet = Names[z], Relations = new List<string>() };
                for (var y = 2; y <= 8; y++)
                {
                    var v = z == y ? 0 : nais[z, y] + tat[z, y];
                    var label = Label(v);
                    result.Labels[z, y] = label;
                    row.Relations.Add(z == y ? "-" : label);
                }
                result.Rows.Add(row);
            }
            return result;
        }

        public KinshukadiResult CalculateKinshukadi(int[,] iSodsVrg, string[,] sPanchMatri)
        {
            int[] own1 = { 0, 0, 5, 4, 1, 3, 9, 2, 10 };
            int[] own2 = { 0, 0, 0, 0, 8, 6, 12, 7, 11 };
            int[] uch = { 0, 0, 1, 2, 10, 6, 4, 12, 7 };
            int[] lord = { 0, 4, 7, 5, 3, 2, 5, 7, 4, 6, 8, 8, 6 };

            var result = new KinshukadiResult();
            for (var iPt = 2; iPt <= 8; iPt++)
            {
                var bhed = 0;
                var parijat = 0;
                var kinshuk = 0;
                for (var v = 1; v <= 16; v++)
                {
                    var sign = iSodsVrg[v, iPt];
                    var good = sign == uch[iPt] || sign == own1[iPt] || sign == own2[iPt]
                               || string.Equals(sPanchMatri[iPt, lord[sign]], "G.Friend", StringComparison.OrdinalIgnoreCase);
                    if (good) bhed++;
                    if (!(v == 4 || v == 11 || v == 12 || v == 13 || v == 14 || v == 15) && good) parijat++;
                    if (!(v == 4 || v == 7 || v == 10 || v == 11 || v == 12 || v == 13 || v == 14 || v == 15 || v == 16) && good) kinshuk++;
                }
                result.Rows.Add(new KinshukadiRow
                {
                    Planet = Names[iPt],
                    ShodashaCount = bhed,
                    DasaCount = parijat,
                    SaptaCount = kinshuk,
                    Shodasha = NameAt(ShodashaNames, bhed),
                    DasaVarga = NameAt(DasaNames, parijat),
                    SaptaVarga = NameAt(SaptaNames, kinshuk)
                });
            }
            return result;
        }

        private static string Label(int v)
        {
            if (v >= 2) return "G.Friend";
            if (v == 1) return "Friend";
            if (v == 0) return "Neutral";
            if (v == -1) return "Enemy";
            return "G.Enemy";
        }

        private static string NameAt(string[] names, int count)
        {
            if (count < 1) return "-";
            if (count >= names.Length) return names[names.Length - 1];
            return names[count];
        }
    }
}
