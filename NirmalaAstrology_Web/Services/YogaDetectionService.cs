using System.Collections.Generic;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Expanded port of oYogasWithGrammar Particular/General staples for DETAILED:
    /// Chandra/Surya classics, Nabhas Sankhya+Aakriti, Vipreet split, Parivartan,
    /// Neechbhang, Kalsarpa, Maala/Sarpa, plus prior Mahapurusha/Raja/Kartari.
    /// </summary>
    public sealed class YogaDetectionService
    {
        // Exaltation / debilitation signs (1=Aries … 12=Pisces)
        private static readonly int[] UchSign = { 0, 0, 1, 2, 10, 6, 4, 12, 7 };
        private static readonly int[] NeechSign = { 0, 0, 7, 8, 4, 12, 10, 6, 1 };

        public YogaDetectionResult Calculate(double[] dPlt, GrahaBalaResult bala)
        {
            var iRasi = new int[11];
            var house = new int[11];
            for (var z = 1; z <= 10; z++)
            {
                iRasi[z] = AstroMath.FnTgr(dPlt[z] / 30.0) + 1;
                house[z] = AstroMath.IFnR(iRasi[z] - iRasi[1], 12) + 1;
            }

            var grhInBhav = new int[13];
            for (var z = 2; z <= 8; z++)
                grhInBhav[house[z]]++;

            var occupiedSigns = new bool[13];
            var ho = 0;
            for (var z = 2; z <= 8; z++)
            {
                if (!occupiedSigns[iRasi[z]])
                {
                    occupiedSigns[iRasi[z]] = true;
                    ho++;
                }
            }

            var result = new YogaDetectionResult();
            void Add(string name, string cat, string note)
            {
                result.Yogas.Add(new DetectedYoga { Name = name, Category = cat, Note = note });
            }

            int From(int planet, int fromPlanet)
            {
                return AstroMath.IFnR(iRasi[planet] - iRasi[fromPlanet], 12) + 1;
            }

            bool InKendraFromLagna(int planet)
            {
                var h = house[planet];
                return h == 1 || h == 4 || h == 7 || h == 10;
            }

            // ——— Chandra classics (Particular) ———
            // Classical names: Sunapha=2nd from Moon, Anapha=12th (VB print labels are swapped).
            var sunapha = false;
            var anapha = false;
            for (var p = 4; p <= 8; p++)
            {
                var h = From(p, 3);
                if (h == 2) sunapha = true;
                if (h == 12) anapha = true;
            }
            if (sunapha) Add("Sunapha Yoga", "Chandra", "Planet(s) Mars–Saturn in 2nd from Moon.");
            if (anapha) Add("Anapha Yoga", "Chandra", "Planet(s) Mars–Saturn in 12th from Moon.");
            if (sunapha && anapha) Add("Durdhara Yoga", "Chandra", "2nd and 12th from Moon both occupied.");
            if (!sunapha && !anapha) Add("Kemadruma Yoga", "Chandra", "2nd and 12th from Moon empty (Mars–Saturn).");

            if (From(5, 3) == 10 || From(6, 3) == 10 || From(7, 3) == 10)
                Add("Amala Yoga", "Chandra", "Benefic in 10th from Moon.");

            var gj = From(6, 3);
            if (gj == 4 || gj == 7 || gj == 10)
                Add("Gajakesari Yoga", "Chandra", "Jupiter in kendra from Moon (4/7/10).");

            if (!InKendraFromLagna(6) && (gj == 6 || gj == 7 || gj == 8))
                Add("Shakat Yoga (from Moon)", "Chandra", "Jupiter not in lagna-kendra and in 6/7/8 from Moon.");

            var adhi = 0;
            foreach (var p in new[] { 5, 6, 7 })
            {
                var h = From(p, 3);
                if (h == 6 || h == 7 || h == 8) adhi++;
            }
            if (adhi > 0)
                Add("Chandradhi / Adhi Yoga", "Chandra", "Benefic(s) in 6/7/8 from Moon.");

            if (house[3] == house[9] || house[3] == house[10])
                Add("Grahan Yoga", "Chandra", "Moon conjoined Rahu or Ketu.");

            // ——— Surya classics ———
            var vesi = false;
            var vasi = false;
            for (var p = 4; p <= 8; p++)
            {
                var h = From(p, 2);
                if (h == 2) vesi = true;
                if (h == 12) vasi = true;
            }
            if (vesi) Add("Vesi Yoga", "Surya", "Planet(s) in 2nd from Sun.");
            if (vasi) Add("Vasi Yoga", "Surya", "Planet(s) in 12th from Sun.");
            if (vesi && vasi) Add("Ubhayachari Yoga", "Surya", "Planets in both 2nd and 12th from Sun.");

            if (iRasi[2] == iRasi[5])
                Add("Budhaditya Yoga", "Surya", "Sun and Mercury conjoined.");

            // ——— Nabhas Sankhya ———
            switch (ho)
            {
                case 7: Add("Veena / Vallaki Yoga", "Nabhas-Sankhya", "Seven planets in seven signs."); break;
                case 6: Add("Damini Yoga", "Nabhas-Sankhya", "Seven planets in six signs."); break;
                case 5: Add("Paash Yoga", "Nabhas-Sankhya", "Seven planets in five signs."); break;
                case 4: Add("Kedar Yoga", "Nabhas-Sankhya", "Seven planets in four signs."); break;
                case 3: Add("Shool Yoga", "Nabhas-Sankhya", "Seven planets in three signs."); break;
                case 2: Add("Yug Yoga", "Nabhas-Sankhya", "Seven planets in two signs."); break;
                case 1: Add("Gol Yoga", "Nabhas-Sankhya", "Seven planets in one sign."); break;
            }

            // ——— Nabhas Aakriti ———
            if (grhInBhav[1] + grhInBhav[7] == 7)
                Add("Shakat Yoga (Nabhas-Aakriti)", "Nabhas-Aakriti", "All seven planets in 1st and/or 7th.");
            if (grhInBhav[1] + grhInBhav[4] + grhInBhav[7] + grhInBhav[10] == 7
                && (grhInBhav[1] + grhInBhav[4] == 7 || grhInBhav[4] + grhInBhav[7] == 7
                    || grhInBhav[7] + grhInBhav[10] == 7 || grhInBhav[10] + grhInBhav[1] == 7))
                Add("Gada Yoga", "Nabhas-Aakriti", "All seven in two adjacent kendras.");
            if (grhInBhav[1] + grhInBhav[5] + grhInBhav[9] == 7)
                Add("Shringatak Yoga", "Nabhas-Aakriti", "All seven in 1/5/9 only.");
            if (grhInBhav[4] + grhInBhav[10] == 7)
                Add("Pakshi / Vihag Yoga", "Nabhas-Aakriti", "All seven in 4th and 10th only.");
            if (grhInBhav[2] + grhInBhav[6] + grhInBhav[10] == 7
                || grhInBhav[3] + grhInBhav[7] + grhInBhav[11] == 7
                || grhInBhav[4] + grhInBhav[8] + grhInBhav[12] == 7)
                Add("Hal Yoga", "Nabhas-Aakriti", "All seven in a triangular house set.");
            if (grhInBhav[1] + grhInBhav[4] + grhInBhav[7] + grhInBhav[10] == 7)
                Add("Kamal Yoga", "Nabhas-Aakriti", "All seven in kendras only.");
            if (grhInBhav[1] + grhInBhav[4] + grhInBhav[7] + grhInBhav[10] == 0)
                Add("Vaapi Yoga", "Nabhas-Aakriti", "No planet in any kendra.");
            if (grhInBhav[1] + grhInBhav[2] + grhInBhav[3] + grhInBhav[4] == 7)
                Add("Yoop Yoga", "Nabhas-Aakriti", "All seven in houses 1–4.");
            if (grhInBhav[4] + grhInBhav[5] + grhInBhav[6] + grhInBhav[7] == 7)
                Add("Shar / Ishu Yoga", "Nabhas-Aakriti", "All seven in houses 4–7.");
            if (grhInBhav[7] + grhInBhav[8] + grhInBhav[9] + grhInBhav[10] == 7)
                Add("Shakti Yoga", "Nabhas-Aakriti", "All seven in houses 7–10.");
            if (grhInBhav[10] + grhInBhav[11] + grhInBhav[12] + grhInBhav[1] == 7)
                Add("Danda Yoga", "Nabhas-Aakriti", "All seven in houses 10–1.");
            if (grhInBhav[1] + grhInBhav[2] + grhInBhav[3] + grhInBhav[4] + grhInBhav[5] + grhInBhav[6] + grhInBhav[7] == 7
                && grhInBhav[1] > 0 && grhInBhav[7] > 0)
                Add("Nauka Yoga", "Nabhas-Aakriti", "All seven in continuous houses 1–7.");

            // ——— Dal ———
            if (InKendraFromLagna(3) && InKendraFromLagna(5) && InKendraFromLagna(6) && InKendraFromLagna(7))
                Add("Maala Yoga", "Nabhas-Dal", "All benefics (Moon/Mer/Jup/Ven) in kendras.");
            if (InKendraFromLagna(2) && InKendraFromLagna(4) && InKendraFromLagna(8))
                Add("Sarpa Yoga", "Nabhas-Dal", "All malefics (Sun/Mars/Sat) in kendras.");

            // ——— Vipreet split ———
            var l6 = LordOf(HouseSign(iRasi[1], 6));
            var l8 = LordOf(HouseSign(iRasi[1], 8));
            var l12 = LordOf(HouseSign(iRasi[1], 12));
            if (IsTrik(house[l6]) && IsTrik(house[l8]) && IsTrik(house[l12]))
                Add("Vipreet Rajyoga", "Raja", "All three Trik lords occupy Trik houses.");
            if (house[l6] == 6) Add("Harsha Yoga", "Vipreet", "6th lord in 6th.");
            if (house[l8] == 8) Add("Sarala Yoga", "Vipreet", "8th lord in 8th.");
            if (house[l12] == 12) Add("Vimala Yoga", "Vipreet", "12th lord in 12th.");

            // ——— Parivartan ———
            var seenPv = new HashSet<string>();
            for (var hx = 1; hx <= 12; hx++)
            {
                for (var hy = hx + 1; hy <= 12; hy++)
                {
                    var lx = LordOf(HouseSign(iRasi[1], hx));
                    var ly = LordOf(HouseSign(iRasi[1], hy));
                    if (lx == ly) continue;
                    if (house[lx] == hy && house[ly] == hx)
                    {
                        var key = hx + "-" + hy;
                        if (seenPv.Add(key))
                            Add("Parivartan Yoga (" + hx + "↔" + hy + ")", "Parivartan",
                                "Lords of houses " + hx + " and " + hy + " exchange.");
                    }
                }
            }

            // ——— Neechbhang ———
            string[] pNames = { "", "", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn" };
            for (var z = 2; z <= 8; z++)
            {
                if (iRasi[z] != NeechSign[z]) continue;
                var dispositor = LordOf(iRasi[z]);
                var exaltLord = LordOf(UchSign[z]);
                if (InKendraFromLagna(dispositor))
                    Add("Neechbhang Rajyoga (" + pNames[z] + ")", "Raja",
                        pNames[z] + " debilitated; dispositor in kendra from Lagna.");
                else if (InKendraFromLagna(exaltLord))
                    Add("Neechbhang Rajyoga (" + pNames[z] + ")", "Raja",
                        pNames[z] + " debilitated; exaltation-lord in kendra from Lagna.");
            }

            // ——— Kalsarpa ———
            var oneSide = 0;
            for (var z = 2; z <= 8; z++)
            {
                if (AstroMath.FnR(dPlt[9] - dPlt[z], 360.0) < 180.0)
                    oneSide++;
            }
            if (oneSide == 0 || oneSide == 7)
                Add("Kalsarpa Yoga", "Rahu-Ketu", "All planets on one side of the Rahu–Ketu axis.");
            else
                Add("Kalsarpa absent", "Rahu-Ketu", "Planets straddle both sides of Rahu–Ketu.");

            // ——— More Particular staples ———
            if (iRasi[3] == iRasi[4])
                Add("Chandra-Mangal Yoga", "Particular", "Moon conjoined Mars.");
            if (iRasi[6] == iRasi[4])
                Add("Guru-Mangal Yoga", "Particular", "Jupiter conjoined Mars.");
            if (From(5, 1) == 10 || From(6, 1) == 10 || From(7, 1) == 10)
                Add("Amala Yoga (from Lagna)", "Particular", "Benefic in 10th from Lagna.");

            var vasu = true;
            foreach (var p in new[] { 3, 5, 6, 7 })
            {
                var h = house[p];
                if (h != 3 && h != 6 && h != 10 && h != 11) { vasu = false; break; }
            }
            if (vasu) Add("Vasumati Yoga", "Particular", "All benefics in upachaya (3/6/10/11).");

            var lagnaAdhi = 0;
            foreach (var p in new[] { 5, 6, 7 })
            {
                var h = house[p];
                if (h == 6 || h == 7 || h == 8) lagnaAdhi++;
            }
            if (lagnaAdhi > 0)
                Add("Lagnaadhi Yoga", "Particular", "Benefic(s) in 6/7/8 from Lagna.");

            // Gandharv (General lagna-specific): Taurus/Libra + Jup+Ven in kendra
            if ((iRasi[1] == 2 || iRasi[1] == 7) && InKendraFromLagna(6) && InKendraFromLagna(7))
                Add("Gandharv Yoga", "Lagna-specific", "Vrish/Tula Lagna with Jupiter and Venus in kendra.");

            // ——— Mahapurusha ———
            TryMahapurusha(Add, "Ruchaka", 4, iRasi, house, new[] { 1, 8 });
            TryMahapurusha(Add, "Bhadra", 5, iRasi, house, new[] { 3, 6 });
            TryMahapurusha(Add, "Hamsa", 6, iRasi, house, new[] { 4, 9, 12 });
            TryMahapurusha(Add, "Malavya", 7, iRasi, house, new[] { 2, 7 });
            TryMahapurusha(Add, "Sasa", 8, iRasi, house, new[] { 10, 11 });

            // ——— Kartari lagna ———
            if (IsMaleficIn(house, 2) && IsMaleficIn(house, 12))
                Add("Paap Kartari Yoga", "Kartari", "Malefics hem the Lagna.");
            if (IsBeneficIn(house, 2) && IsBeneficIn(house, 12))
                Add("Shubh Kartari Yoga", "Kartari", "Benefics hem the Lagna.");

            // ——— Raja (kendra–trikona lord association) ———
            if (Shares(house, LordOf(HouseSign(iRasi[1], 1)), LordOf(HouseSign(iRasi[1], 5)))
                || Shares(house, LordOf(HouseSign(iRasi[1], 1)), LordOf(HouseSign(iRasi[1], 9)))
                || Shares(house, LordOf(HouseSign(iRasi[1], 4)), LordOf(HouseSign(iRasi[1], 5)))
                || Shares(house, LordOf(HouseSign(iRasi[1], 7)), LordOf(HouseSign(iRasi[1], 9)))
                || Shares(house, LordOf(HouseSign(iRasi[1], 10)), LordOf(HouseSign(iRasi[1], 5)))
                || Shares(house, LordOf(HouseSign(iRasi[1], 10)), LordOf(HouseSign(iRasi[1], 9))))
                Add("Raja Yoga", "Raja", "Kendra lord associated with trikona lord.");

            if (bala != null)
            {
                foreach (var p in bala.Planets)
                {
                    if (p.Percent >= 100)
                        Add(p.Planet + " Shadbala Purna", "Bala",
                            p.Planet + " meets/exceeds required Shadbala (" + p.Percent.ToString("0") + "%).");
                }
            }

            if (result.Yogas.Count == 0)
                Add("No listed yogas", "General", "Expanded rule set found no hits.");

            return result;
        }

        private static void TryMahapurusha(System.Action<string, string, string> add, string name, int planet, int[] iRasi, int[] house, int[] ownExaltSigns)
        {
            var kendra = house[planet] == 1 || house[planet] == 4 || house[planet] == 7 || house[planet] == 10;
            var inSign = false;
            foreach (var s in ownExaltSigns) if (iRasi[planet] == s) inSign = true;
            if (kendra && inSign) add(name + " Yoga", "Mahapurusha", name + " in own/exalt kendra.");
        }

        private static int HouseSign(int lagna, int houseNum)
        {
            return ((lagna - 1 + houseNum - 1) % 12) + 1;
        }

        private static int LordOf(int sign)
        {
            int[] lords = { 0, 4, 7, 5, 3, 2, 5, 7, 4, 6, 8, 8, 6 };
            return lords[sign];
        }

        private static bool IsTrik(int h) { return h == 6 || h == 8 || h == 12; }
        private static bool IsMaleficIn(int[] house, int h)
        {
            return house[2] == h || house[4] == h || house[8] == h || house[9] == h;
        }
        private static bool IsBeneficIn(int[] house, int h)
        {
            return house[5] == h || house[6] == h || house[7] == h || house[3] == h;
        }
        private static bool Shares(int[] house, int a, int b)
        {
            return a >= 2 && b >= 2 && house[a] == house[b];
        }
    }
}
