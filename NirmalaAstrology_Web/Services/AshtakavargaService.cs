using System;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Port of lAstakVarg.Astak_Varg (Non_Parasar + TrikonSodhan_NorthIndian defaults).
    /// Indices 2..8 = Sun..Saturn, 9 = Lagna (VB6 overwrites Rahu slot).
    /// </summary>
    public sealed class AshtakavargaService
    {
        private static readonly string[] PlanetNames =
        {
            "", "Lagna", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn", "Lagna"
        };

        // Mesh-adi rekha strings from Hora-Ratnam / Jatak-Parijat (Non_Parasar defaults).
        private static readonly string[,] Rekha = CreateRekhaTable();

        private static readonly int[] RasiMultipliers =
        {
            0, 7, 10, 8, 4, 10, 5, 7, 8, 9, 5, 11, 12
        };

        private static readonly int[] GrahaMultipliers =
        {
            0, 0, 5, 5, 8, 5, 10, 7, 5
        };

        public AshtakavargaResult Calculate(double[] dPlt)
        {
            if (dPlt == null || dPlt.Length < 11)
                throw new ArgumentException("dPlt must include indices 1..10.", "dPlt");

            var iRasi = new int[10];
            for (var z = 1; z <= 8; z++)
                iRasi[z] = AstroMath.FnTgr(dPlt[z] / 30.0) + 1;
            iRasi[9] = iRasi[1]; // Lagna occupies slot 9 for AV contributors

            var rgy = new int[13];
            for (var z = 1; z <= 12; z++)
            {
                if (z == iRasi[2] || z == iRasi[3] || z == iRasi[4] || z == iRasi[5]
                    || z == iRasi[6] || z == iRasi[7] || z == iRasi[8])
                    rgy[z] = 1;
            }

            var result = new AshtakavargaResult();
            var iTot8Vgen = new int[10, 13];

            for (var iPlt = 2; iPlt <= 9; iPlt++)
            {
                for (var iRs = 1; iRs <= 12; iRs++)
                {
                    var sum = 0;
                    for (var iGr = 2; iGr <= 9; iGr++)
                    {
                        var rotated = RotateMeshAdi(Rekha[iPlt, iGr], iRasi[iGr]);
                        sum += rotated[iRs - 1] - '0';
                    }
                    iTot8Vgen[iPlt, iRs] = sum;
                }

                var bindus = CopyRow(iTot8Vgen, iPlt);
                var trikona = TrikonaSodhanNorthIndian(bindus);
                var ekadhi = EkadhipatyaNonParasar(trikona, rgy);

                var total = 0;
                for (var s = 1; s <= 12; s++) total += bindus[s];

                result.Charts.Add(new AshtakavargaChart
                {
                    PlanetIndex = iPlt,
                    PlanetName = PlanetNames[iPlt],
                    BindusBySign = bindus,
                    TrikonaBySign = trikona,
                    EkadhipatyaBySign = ekadhi,
                    TotalBindus = total
                });
            }

            for (var iRs = 1; iRs <= 12; iRs++)
            {
                var sarva = 0;
                var tri = 0;
                var ek = 0;
                for (var iPlt = 2; iPlt <= 9; iPlt++)
                {
                    var chart = result.Charts[iPlt - 2];
                    sarva += chart.BindusBySign[iRs];
                    tri += chart.TrikonaBySign[iRs];
                    ek += chart.EkadhipatyaBySign[iRs];
                }
                result.SarvaBySign[iRs] = sarva;
                result.TrikonaSarvaBySign[iRs] = tri;
                result.EkadhipatyaSarvaBySign[iRs] = ek;
            }

            for (var iGr = 2; iGr <= 8; iGr++)
            {
                var ek = result.Charts[iGr - 2].EkadhipatyaBySign;
                var rasiPind = 0;
                var grahaPind = 0;
                for (var iRs = 1; iRs <= 12; iRs++)
                {
                    rasiPind += ek[iRs] * RasiMultipliers[iRs];
                    for (var g = 2; g <= 8; g++)
                    {
                        if (iRasi[g] == iRs)
                            grahaPind += ek[iRs] * GrahaMultipliers[g];
                    }
                }
                result.Pindas.Add(new AshtakavargaPindaRow
                {
                    PlanetName = PlanetNames[iGr],
                    RasiPinda = rasiPind,
                    GrahaPinda = grahaPind,
                    TotalPinda = rasiPind + grahaPind
                });
            }

            return result;
        }

        private static string RotateMeshAdi(string meshAdi, int occupiedSign)
        {
            var iX = occupiedSign - 1;
            if (iX <= 0) return meshAdi;
            return meshAdi.Substring(12 - iX) + meshAdi.Substring(0, 12 - iX);
        }

        private static int[] CopyRow(int[,] source, int planet)
        {
            var row = new int[13];
            for (var s = 1; s <= 12; s++) row[s] = source[planet, s];
            return row;
        }

        private static int[] TrikonaSodhanNorthIndian(int[] bindus)
        {
            var result = new int[13];
            for (var group = 1; group <= 4; group++)
            {
                var a = bindus[group];
                var b = bindus[group + 4];
                var c = bindus[group + 8];
                var lowest = Math.Min(a, Math.Min(b, c));
                result[group] = a - lowest;
                result[group + 4] = b - lowest;
                result[group + 8] = c - lowest;
            }
            return result;
        }

        private static int[] EkadhipatyaNonParasar(int[] trikona, int[] rgy)
        {
            var iTk = new int[13];
            var iEk = new int[13];
            for (var i = 1; i <= 12; i++)
            {
                iTk[i] = trikona[i];
                iEk[i] = trikona[i];
            }

            int[] xs = { 0, 1, 2, 3, 9, 10 };
            int[] ys = { 0, 8, 7, 6, 12, 11 };

            for (var z = 1; z <= 5; z++)
            {
                var x = xs[z];
                var y = ys[z];

                if (iTk[x] == 0 || iTk[y] == 0)
                {
                    iEk[x] = iTk[x];
                    iEk[y] = iTk[y];
                }
                else if (rgy[x] == 0 && rgy[y] == 0)
                {
                    if (iTk[x] == iTk[y])
                    {
                        iEk[x] = 0;
                        iEk[y] = 0;
                    }
                    else if (iTk[x] < iTk[y])
                    {
                        iEk[x] = iTk[x];
                        iEk[y] = iTk[x];
                    }
                    else
                    {
                        iEk[x] = iTk[y];
                        iEk[y] = iTk[y];
                    }
                }
                else if (rgy[x] == 1 && rgy[y] == 1)
                {
                    iEk[x] = iTk[x];
                    iEk[y] = iTk[y];
                }
                else if ((rgy[x] == 1 && rgy[y] == 0) || (rgy[x] == 0 && rgy[y] == 1))
                {
                    // Port VB6 Non_Parasar branches exactly (including the dead RGY(X)=RGY(Y) check).
                    if (rgy[x] == 1 && rgy[x] == rgy[y])
                    {
                        iEk[x] = iTk[x];
                        iEk[y] = 0;
                    }
                    else if (rgy[y] == 1 && iTk[x] == iTk[y])
                    {
                        iEk[x] = 0;
                        iEk[y] = iTk[y];
                    }
                    else if (rgy[x] == 1 && iTk[x] < iTk[y])
                    {
                        iEk[x] = iTk[x];
                        iEk[y] = iTk[x];
                    }
                    else if (rgy[y] == 1 && iTk[y] < iTk[x])
                    {
                        iEk[y] = iTk[y];
                        iEk[x] = iTk[y];
                    }
                    else if (rgy[x] == 0 && iTk[x] < iTk[y])
                    {
                        iEk[x] = 0;
                        iEk[y] = iTk[y];
                    }
                    else if (rgy[y] == 0 && iTk[y] < iTk[x])
                    {
                        iEk[y] = 0;
                        iEk[x] = iTk[x];
                    }
                }

                iEk[4] = iTk[4];
                iEk[5] = iTk[5];
            }

            return iEk;
        }

        private static string[,] CreateRekhaTable()
        {
            var s = new string[10, 10];
            // Sun (2)
            s[2, 2] = "110100111110";
            s[2, 3] = "001001000110";
            s[2, 4] = "110100111110";
            s[2, 5] = "001011001111";
            s[2, 6] = "000011001010";
            s[2, 7] = "000001100001";
            s[2, 8] = "110100111110";
            s[2, 9] = "001101000111";
            // Moon (3)
            s[3, 2] = "001001110110";
            s[3, 3] = "101001100110";
            s[3, 4] = "011011001110";
            s[3, 5] = "101110110110";
            s[3, 6] = "100100110111";
            s[3, 7] = "001110101110";
            s[3, 8] = "001011000010";
            s[3, 9] = "001001000110";
            // Mars (4)
            s[4, 2] = "001011000110";
            s[4, 3] = "001001000010";
            s[4, 4] = "110100110110";
            s[4, 5] = "001011000010";
            s[4, 6] = "000001000111";
            s[4, 7] = "000001010011";
            s[4, 8] = "100100111110";
            s[4, 9] = "101001000110";
            // Mercury (5)
            s[5, 2] = "000011001011";
            s[5, 3] = "010101010110";
            s[5, 4] = "110100111110";
            s[5, 5] = "101011001111";
            s[5, 6] = "000001010011";
            s[5, 7] = "111110011010";
            s[5, 8] = "110100111110";
            s[5, 9] = "110101010110";
            // Jupiter (6)
            s[6, 2] = "111100111110";
            s[6, 3] = "010010101010";
            s[6, 4] = "110100110110";
            s[6, 5] = "110111001110";
            s[6, 6] = "111100110110";
            s[6, 7] = "010011001110";
            s[6, 8] = "001011000001";
            s[6, 9] = "110111101110";
            // Venus (7)
            s[7, 2] = "000000010011";
            s[7, 3] = "111110011011";
            s[7, 4] = "001011001011";
            s[7, 5] = "001011001010";
            s[7, 6] = "000010011110";
            s[7, 7] = "111110011110";
            s[7, 8] = "001110011110";
            s[7, 9] = "111110011010";
            // Saturn (8)
            s[8, 2] = "110100110110";
            s[8, 3] = "001001000010";
            s[8, 4] = "001011000111";
            s[8, 5] = "000001011111";
            s[8, 6] = "000011000011";
            s[8, 7] = "000001000011";
            s[8, 8] = "001011000010";
            s[8, 9] = "101101000110";
            // Lagna (9)
            s[9, 2] = "001101000111";
            s[9, 3] = "001001000110";
            s[9, 4] = "101001000110";
            s[9, 5] = "110101010110";
            s[9, 6] = "110111101110";
            s[9, 7] = "111110011010";
            s[9, 8] = "101101000110";
            s[9, 9] = "001001000110";
            return s;
        }
    }
}
