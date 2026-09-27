using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Repositories;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Port of nMatchMaking.Milan_Calc using the Access MatchDet 36x36 matrix.
    /// </summary>
    public sealed class MatchMakingService
    {
        private static readonly string[] TaraNames =
        {
            "", "JANAM", "SAMPAT", "VIPAT", "KSHEM", "PRATYARI", "SADHAK", "VADH", "MAITAR", "ATIMAITAR"
        };

        private static readonly string[] BhakootNames =
        {
            "", "SAMSAPTAK", "DVIRDVADAS", "TRAIKADAS", "TRAIKADAS", "CHATURDAS", "NAVPANCHAK",
            "SADASTAK", "SADASTAK", "SAMSAPTAK", "NAVPANCHAK", "CHATURDAS", "DVIRDVADAS"
        };

        private static readonly string[] KootNames =
        {
            "", "Varna", "Vasya", "Tara", "Yoni", "Rashish", "Gana", "Bhakoot", "Nadi"
        };

        private readonly MatchDetRepository _matchDet;
        private readonly KundliCalculator _calculator;

        public MatchMakingService(string astronomyScriptPath = null)
        {
            _matchDet = new MatchDetRepository();
            _calculator = new KundliCalculator();
        }

        public MatchMakingResult Compare(MatchPersonInput male, MatchPersonInput female)
        {
            if (male == null) throw new ArgumentNullException("male");
            if (female == null) throw new ArgumentNullException("female");

            var maleChart = _calculator.CalculateKundli(male.Day, male.Month, male.Year, male.Hour, male.Minute, male.Longitude, male.Latitude, male.Timezone);
            var femaleChart = _calculator.CalculateKundli(female.Day, female.Month, female.Year, female.Hour, female.Minute, female.Longitude, female.Latitude, female.Timezone);
            var maleProfile = Profile(male, maleChart);
            var femaleProfile = Profile(female, femaleChart);
            return MilanCalc(maleProfile, femaleProfile);
        }

        private static MatchPersonProfile Profile(MatchPersonInput input, KundliResult chart)
        {
            var moon = chart.PlanetPositions.First(p => p.Name == "Moon");
            var lagna = chart.PlanetPositions.First(p => p.Name == "Lagna");
            var mars = chart.PlanetPositions.First(p => p.Name == "Mars");
            var hora = HoraSangyaCalculator.FromMoon(moon.TotalDegrees);
            string[] rashiNames =
            {
                "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
                "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
            };
            return new MatchPersonProfile
            {
                Name = input.Name,
                BirthDateText = new DateTime(input.Year, input.Month, input.Day).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                BirthTimeText = new TimeSpan(input.Hour, input.Minute, 0).ToString(@"hh\:mm"),
                Place = input.Place,
                MoonLongitude = moon.TotalDegrees,
                MoonRashi = hora.RashiNumber,
                LagnaRashi = lagna.Rashi,
                LagnaRashiName = (lagna.Rashi >= 1 && lagna.Rashi <= 12) ? rashiNames[lagna.Rashi] : (chart.AscendantRashiName ?? ""),
                LagnaRakv = AstroMath.FormatRakv(lagna.TotalDegrees),
                MoonRakv = AstroMath.FormatRakv(moon.TotalDegrees),
                AyanamsaRak = AstroMath.FormatAyanamsa(chart.Ayanamsa),
                MarsHouse = mars.House,
                IsManglik = mars.House == 1 || mars.House == 4 || mars.House == 7 || mars.House == 8 || mars.House == 12,
                Hora = hora
            };
        }

        private MatchMakingResult MilanCalc(MatchPersonProfile male, MatchPersonProfile female)
        {
            // LEGACY TRACE: nMatchMaking.bas → Private Sub Milan_Calc()
            // Globals: iNakNum_M/F, iRasiNum_M/F, sGunStr, MatchDet, Rashish_Matri
            var nakM = male.Hora.NakshatraNumber;
            var nakF = female.Hora.NakshatraNumber;
            var rasiM = male.MoonRashi;
            var rasiF = female.MoonRashi;
            var charanM = male.Hora.Pada;
            var charanF = female.Hora.Pada;

            var taraF = Mod9(nakM - nakF + 1);
            var taraM = Mod9(nakF - nakM + 1);
            var bhakut = Mod12(rasiM - rasiF) + 1;
            var bhakootName = BhakootNames[bhakut];

            // Matrix index correction by rashi band (legacy Milan_Calc)
            int corrM;
            if (rasiM > 8)
            {
                corrM = 3;
            }
            else if (rasiM > 4)
            {
                corrM = 2;
            }
            else
            {
                corrM = 1;
            }

            int corrF;
            if (rasiF > 8)
            {
                corrF = 3;
            }
            else if (rasiF > 4)
            {
                corrF = 2;
            }
            else
            {
                corrF = 1;
            }

            var col = rasiM + nakM - corrM;
            var row = rasiF + nakF - corrF;
            var gunaStr = _matchDet.GetGunaString(row, col);

            var normal = new double[9];
            var modified = new double[9];
            for (var z = 1; z <= 8; z++)
            {
                var score = ParseGunChar(gunaStr, z);
                normal[z] = score;
                modified[z] = score;
            }

            var nadiParihar = new bool[5];
            var bhakutParihar = new bool[5];
            var ganaParihar = false;
            var yoniParihar = false;
            var taraParihar = false;
            var vasyaParihar = false;
            var varnaParihar = false;

            // Legacy VB6 nMatchMaking.bas Milan_Calc:
            //   iNavamsa_M = Girl moon navamsa; iNavamsa_F = Boy moon navamsa
            //   sRashish_Maitri_Nvms = Rashish_Matri(iNavamsa_M, iNavamsa_F)
            //   sRashish_Maitri = Rashish_Matri(iRasiNum_F, iRasiNum_F)  ' intentional VB6 call (female, female)
            var navamsaBoy = Navamsa(male.MoonLongitude);
            var navamsaGirl = Navamsa(female.MoonLongitude);
            var rashish = RashishMatri(rasiF, rasiF);
            var navamsaMaitri = RashishMatri(navamsaGirl, navamsaBoy);

            if (normal[8] == 0)
            {
                if (nakM == nakF && charanM != charanF)
                {
                    modified[8] = 4;
                    nadiParihar[1] = true;
                }
                if (rasiM == rasiF && nakM != nakF)
                {
                    modified[8] = 4;
                    nadiParihar[2] = true;
                }
                if (nakM == nakF && rasiM != rasiF)
                {
                    modified[8] = 4;
                    nadiParihar[3] = true;
                }
                if ((charanM == 1 && charanF != 4) || (charanM == 4 && charanF != 1) ||
                    (charanM == 2 && charanF != 3) || (charanM == 3 && charanF != 2))
                {
                    modified[8] = 4;
                    nadiParihar[4] = true;
                }
            }

            if (normal[7] == 0)
            {
                if ((rashish == "YES" || navamsaMaitri == "YES") && normal[8] > 0)
                {
                    modified[7] = 3.5;
                    bhakutParihar[1] = true;
                }
                if (bhakootName == "SADASTAK" && FriendlySadastak(rasiM, rasiF))
                {
                    modified[7] = 3.5;
                    bhakutParihar[2] = true;
                }
                if (bhakootName == "NAVPANCHAK" && Mod12(rasiF - rasiM) + 1 == 5)
                {
                    modified[7] = 3.5;
                    bhakutParihar[3] = true;
                }
                if (bhakootName == "DVIRDVADAS" && Mod12(rasiF - rasiM) + 1 == 12)
                {
                    modified[7] = 3.5;
                    bhakutParihar[4] = true;
                }
            }

            if (normal[6] < 6 && (rashish == "YES" || navamsaMaitri == "YES"))
            {
                if (normal[6] == 0)
                {
                    modified[6] = 3;
                }
                else if (modified[6] == 1)
                {
                    modified[6] = 4;
                }
                else if (modified[6] == 5)
                {
                    modified[6] = 6;
                }
                ganaParihar = true;
            }

            if (normal[5] < 5 && normal[7] == 7)
            {
                if (normal[5] == 0) modified[5] = 2.5;
                else if (normal[5] == 0.5) modified[5] = 3;
                else if (normal[5] == 1) modified[5] = 3.5;
                else if (normal[5] == 3) modified[5] = 4.5;
                else if (normal[5] == 4) modified[5] = 5;
            }

            if (normal[4] == 0 && (rashish == "YES" || navamsaMaitri == "YES"))
            {
                modified[4] = 2;
                yoniParihar = true;
            }
            if (normal[3] == 0 && (rashish == "YES" || navamsaMaitri == "YES"))
            {
                modified[3] = 1.5;
                taraParihar = true;
            }
            if (normal[2] == 0 && (rashish == "YES" || navamsaMaitri == "YES"))
            {
                modified[2] = 1;
                vasyaParihar = true;
            }
            if (normal[1] == 0 && (rashish == "YES" || navamsaMaitri == "YES"))
            {
                modified[1] = 0.5;
                varnaParihar = true;
            }

            var totalN = 0.0;
            var totalM = 0.0;
            var koots = new KootScore[8];
            for (var z = 1; z <= 8; z++)
            {
                totalN += normal[z];
                totalM += modified[z];
                koots[z - 1] = new KootScore
                {
                    Number = z,
                    Name = KootNames[z],
                    Normal = normal[z],
                    Modified = modified[z],
                    HasDosha = normal[z] == 0,
                    Note = z == 7 ? bhakootName : ""
                };
            }

            string verdict;
            if (totalN > 15.5 && totalN < 18)
            {
                verdict = "Marginal matching";
            }
            else if (totalN <= 15.5)
            {
                verdict = "Unsuitable";
            }
            else
            {
                verdict = "Suitable";
            }

            var cancellations = new List<string>();
            if (nadiParihar[1]) cancellations.Add("Nadi dosh cancelled: same nakshatra, different charan.");
            if (nadiParihar[2]) cancellations.Add("Nadi dosh cancelled: same rashi, different nakshatra.");
            if (nadiParihar[3]) cancellations.Add("Nadi dosh cancelled: same nakshatra, different rashi.");
            if (nadiParihar[4]) cancellations.Add("Nadi dosh cancelled: pada-vedh exception from Muhurta Chintamani.");
            if (bhakutParihar[1]) cancellations.Add("Bhakoot dosh cancelled by Rashish/Navamsa maitri with Nadi shuddhi.");
            if (ganaParihar) cancellations.Add("Gana dosh adjusted due to Rashish/Navamsa maitri.");
            if (yoniParihar) cancellations.Add("Yoni dosh cancelled by Rashish maitri.");
            if (taraParihar) cancellations.Add("Tara dosh cancelled by Rashish maitri.");
            if (vasyaParihar) cancellations.Add("Vasya dosh cancelled by Rashish maitri.");
            if (varnaParihar) cancellations.Add("Varna dosh reduced by Rashish maitri.");

            string manglik;
            if (male.IsManglik && female.IsManglik)
            {
                manglik = "Both charts are Manglik from Lagna; the dosha is mutually present.";
            }
            else if (male.IsManglik)
            {
                manglik = "Male chart is Manglik from Lagna (Mars in 1, 4, 7, 8 or 12).";
            }
            else if (female.IsManglik)
            {
                manglik = "Female chart is Manglik from Lagna (Mars in 1, 4, 7, 8 or 12).";
            }
            else
            {
                manglik = "Neither chart is Manglik from Lagna.";
            }

            return new MatchMakingResult
            {
                Male = male,
                Female = female,
                GunaString = gunaStr,
                MatrixRow = row,
                MatrixColumn = col,
                BhakootName = bhakootName,
                MaleTara = TaraNames[taraM],
                FemaleTara = TaraNames[taraF],
                RashiMaitri = rashish,
                NavamsaMaitri = navamsaMaitri,
                Koots = koots,
                TotalNormal = totalN,
                TotalModified = totalM,
                Verdict = verdict,
                ManglikNote = manglik,
                Cancellations = cancellations.ToArray()
            };
        }

        private static double ParseGunChar(string gunaStr, int index)
        {
            if (string.IsNullOrEmpty(gunaStr) || gunaStr.Length < index)
            {
                return 0;
            }

            string ch = gunaStr.Substring(index - 1, 1);
            if (ch == "A")
            {
                return 0.5;
            }
            if (ch == "9")
            {
                return 1.5;
            }

            double value;
            if (Double.TryParse(ch, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }
            return 0;
        }

        private static int Mod9(int value)
        {
            var n = value % 9;
            if (n < 0) n += 9;
            return n == 0 ? 9 : n;
        }

        private static int Mod12(int value)
        {
            var n = value % 12;
            return n < 0 ? n + 12 : n;
        }

        private static int Navamsa(double moon)
        {
            var reduced = moon * 9.0;
            reduced = reduced % 360.0;
            if (reduced < 0) reduced += 360.0;
            return (int)Math.Floor(reduced / 30.0) + 1;
        }

        private static string RashishMatri(int boyRasi, int girlRasi)
        {
            // LEGACY TRACE: nMatchMaking.Rashish_Matri(iBoyRsi, iGirlRsi)
            if ((boyRasi == 1 || boyRasi == 8 || boyRasi == 5 || boyRasi == 9 || boyRasi == 12) &&
                (girlRasi == 1 || girlRasi == 4 || girlRasi == 5 || girlRasi == 8 || girlRasi == 9 || girlRasi == 12))
                return "YES";
            if ((boyRasi == 2 || boyRasi == 7 || boyRasi == 10 || boyRasi == 11) &&
                (girlRasi == 2 || girlRasi == 3 || girlRasi == 6 || girlRasi == 7 || girlRasi == 10 || girlRasi == 11))
                return "YES";
            if ((boyRasi == 3 || boyRasi == 6) &&
                (girlRasi == 2 || girlRasi == 3 || girlRasi == 5 || girlRasi == 6 || girlRasi == 7 || girlRasi == 10 || girlRasi == 11))
                return "YES";
            if (boyRasi == 4 && (girlRasi == 3 || girlRasi == 4 || girlRasi == 6))
                return "YES";
            return "NO";
        }

        private static bool FriendlySadastak(int male, int female)
        {
            return (male == 1 && female == 8) || (male == 2 && female == 7) || (male == 3 && female == 10) ||
                   (male == 4 && female == 9) || (male == 5 && female == 12) || (male == 6 && female == 11) ||
                   (female == 1 && male == 8) || (female == 2 && male == 7) || (female == 3 && male == 10) ||
                   (female == 4 && male == 9) || (female == 5 && male == 12) || (female == 6 && male == 11);
        }
    }
}
