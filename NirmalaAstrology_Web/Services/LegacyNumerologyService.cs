using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Port of pNumerology.bas. Calculation methods intentionally retain the VB6
    /// Cheiro and Pythagorean mappings rather than using the newer simplified helper.
    /// </summary>
    public sealed class LegacyNumerologyService
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string CheiroDigits = "12345835112345781234666517";
        private const string PythagoreanDigits = "12345678912345678912345678";

        private static readonly Dictionary<int, string> NumberReadings = new Dictionary<int, string>
        {
            { 0, "The challenge of 0 is the challenge of choice: develop confidence in your own analysis, make a decision, and act." },
            { 1, "The number 1 emphasizes independence, self-reliance, initiative, leadership, and control of the ego." },
            { 2, "The number 2 emphasizes sensitivity, cooperation, patience, diplomacy, friendship, and quiet growth." },
            { 3, "The number 3 emphasizes creativity, communication, expression, social activity, and control of scattered energy." },
            { 4, "The number 4 emphasizes practical work, organization, discipline, patience, and responsibility." },
            { 5, "The number 5 emphasizes freedom, change, travel, adaptability, and controlled use of impulses." },
            { 6, "The number 6 emphasizes home, family, responsibility, service, harmony, and unconditional acceptance." },
            { 7, "The number 7 emphasizes study, solitude, analysis, spirituality, faith, and inner development." },
            { 8, "The number 8 emphasizes material responsibility, business, authority, status, and proportion." },
            { 9, "The number 9 emphasizes humanitarian service, idealism, completion, generosity, and emotional maturity." }
        };

        private static readonly Dictionary<char, string> PlaneNames = new Dictionary<char, string>
        {
            { 'A', "Mental" }, { 'H', "Mental" }, { 'J', "Mental" }, { 'N', "Mental" }, { 'P', "Mental" }, { 'G', "Mental" }, { 'L', "Mental" },
            { 'E', "Physical" }, { 'W', "Physical" }, { 'D', "Physical" }, { 'M', "Physical" },
            { 'O', "Emotional" }, { 'R', "Emotional" }, { 'I', "Emotional" }, { 'Z', "Emotional" }, { 'B', "Emotional" }, { 'S', "Emotional" }, { 'T', "Emotional" }, { 'X', "Emotional" },
            { 'K', "Intuitive" }, { 'F', "Intuitive" }, { 'Q', "Intuitive" }, { 'U', "Intuitive" }, { 'Y', "Intuitive" }, { 'C', "Intuitive" }, { 'V', "Intuitive" }
        };

        public NumerologyResultModel Calculate(string name, DateTime dateOfBirth, string nickName = null, string vehicleOrPan = null)
        {
            name = name ?? string.Empty;
            var compoundRoot = CalculateNameNumber(name);
            var result = new NumerologyResultModel
            {
                Name = name,
                DateOfBirth = dateOfBirth,
                BirthdayNumber = Root(dateOfBirth.Day),
                LifePathNumber = Root(dateOfBirth.Day + dateOfBirth.Month + dateOfBirth.Year),
                DestinyNumber = compoundRoot.Item2,
                CompoundNameNumber = compoundRoot.Item1,
                NameRootNumber = compoundRoot.Item2,
                SoulUrgeNumber = CalculateVowelNumber(name),
                PersonalityNumber = CalculateConsonantNumber(name),
                BirthNumberMeaning = Reading(Root(dateOfBirth.Day)),
                NameNumberMeaning = Reading(compoundRoot.Item2),
                LifePathInterpretation = NumerologyHelper.GetLifePathInterpretation(Root(dateOfBirth.Day + dateOfBirth.Month + dateOfBirth.Year)),
                DestinyInterpretation = Reading(compoundRoot.Item2),
                SoulUrgeInterpretation = Reading(CalculateVowelNumber(name)),
                PersonalityInterpretation = Reading(CalculateConsonantNumber(name)),
                BirthdayInterpretation = Reading(Root(dateOfBirth.Day)),
                VehiclePanRootNumber = string.IsNullOrWhiteSpace(vehicleOrPan) ? "" : Root(CalculateVehicleSum(vehicleOrPan)).ToString(CultureInfo.InvariantCulture)
            };

            PopulateNameDetails(result, name);
            PopulatePyramid(result, name);
            PopulateVariations(result, name);
            PopulateMaster(result, dateOfBirth);
            PopulateEssence(result, name);
            PopulatePlanes(result, name);
            PopulateMiscellaneous(result, dateOfBirth);
            PopulateNameMeaningSummary(result);
            result.RulingPlanet = NumberPlanet(result.LifePathNumber);
            result.AssociatedZodiacSign = NumberSign(result.LifePathNumber);
            return result;
        }

        public static int Root(long value)
        {
            if (value < 0) value = Math.Abs(value);
            while (value > 9)
            {
                long sum = 0;
                foreach (var c in value.ToString(CultureInfo.InvariantCulture)) sum += c - '0';
                value = sum;
            }
            return (int)value;
        }

        public Tuple<int, int> CalculateNameNumber(string value)
        {
            var compound = 0;
            foreach (var c in (value ?? string.Empty).ToUpperInvariant())
            {
                var index = Alphabet.IndexOf(c);
                if (index >= 0) compound += CheiroDigits[index] - '0';
            }
            return Tuple.Create(compound, Root(compound));
        }

        public string Friendship(int x, int y)
        {
            if ((x == 4 && (y == 4 || y == 8)) || (y == 4 && (x == 4 || x == 8))) return "Unfortunate";
            var friendly = new Dictionary<int, int[]>
            {
                { 1, new[] { 1, 3, 5, 6, 7 } }, { 2, new[] { 2, 7, 1, 5, 6 } },
                { 3, new[] { 3, 7, 1, 5, 6 } }, { 4, new[] { 1, 3, 5, 6, 7 } },
                { 5, new[] { 5, 3, 6, 9 } }, { 6, new[] { 6, 3, 9, 5 } },
                { 7, new[] { 7, 2, 1, 5, 6 } }, { 8, new[] { 1, 5, 6, 9 } },
                { 9, new[] { 9, 3, 6, 9 } }
            };
            return friendly.ContainsKey(x) && friendly[x].Contains(y) ? "Friendly" : "Notfriendly";
        }

        private void PopulateNameDetails(NumerologyResultModel result, string name)
        {
            var letters = new StringBuilder();
            var digits = new StringBuilder();
            var vowels = new StringBuilder();
            var consonants = new StringBuilder();
            foreach (var c in (name ?? string.Empty).ToUpperInvariant())
            {
                if (Alphabet.IndexOf(c) < 0) continue;
                letters.Append(c);
                digits.Append(CheiroDigits[Alphabet.IndexOf(c)]);
                if ("AEIOU".IndexOf(c) >= 0) vowels.Append(c); else consonants.Append(c);
            }
            result.FullNameLetters = letters.ToString();
            result.FullNameDigits = digits.ToString();
            result.Vowels = vowels.ToString(); result.VowelCount = vowels.Length;
            result.Consonants = consonants.ToString(); result.ConsonantCount = consonants.Length;
            result.VowelEffect = Reading(CalculateNameNumber(vowels.ToString()).Item2);
            result.ConsonantEffect = Reading(CalculateNameNumber(consonants.ToString()).Item2);

            var counts = Enumerable.Range(1, 9).ToDictionary(x => x, x => digits.ToString().Count(c => c - '0' == x));
            result.HighestNameNumber = counts.OrderByDescending(x => x.Value).ThenBy(x => x.Key).First().Key;
            result.MissingNameNumbers = string.Join(" ", counts.Where(x => x.Value == 0).Select(x => x.Key.ToString(CultureInfo.InvariantCulture))).Trim();
            result.HighestNumberEffect = Reading(result.HighestNameNumber);
        }

        private void PopulatePyramid(NumerologyResultModel result, string name)
        {
            var current = result.FullNameDigits;
            while (current.Length > 1)
            {
                var next = new StringBuilder();
                for (var i = 0; i < current.Length - 1; i++) next.Append(Root((current[i] - '0') + (current[i + 1] - '0')));
                current = next.ToString();
                result.PyramidRows.Add(current);
            }
            result.PyramidNumber = current;
        }

        private void PopulateVariations(NumerologyResultModel result, string name)
        {
            var words = (name ?? string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 3) return;
            var first = words[0]; var middle = words.Length == 3 ? words[1] : words[1]; var last = words[words.Length - 1];
            var variations = new[] { first.Substring(0, 1) + "." + middle.Substring(0, 1) + ". " + last, middle.Substring(0, 1) + ". " + last, middle + " " + last, first + " " + middle.Substring(0, 1) + ". " + last };
            foreach (var variation in variations)
            {
                var n = CalculateNameNumber(variation);
                result.NameVariations.Add(new LegacyNumerologyNameVariationModel { Name = variation, CompoundNumber = n.Item1, RootNumber = n.Item2, Friendship = Friendship(result.BirthdayNumber, n.Item2) });
            }
        }

        private void PopulateMaster(NumerologyResultModel result, DateTime dob)
        {
            var total = dob.Day + dob.Month + dob.Year;
            var digitSum = total.ToString(CultureInfo.InvariantCulture).Sum(c => c - '0');
            result.MasterNumberText = digitSum > 9 && digitSum < 100 && digitSum % 11 == 0
                ? "Your Master Number is " + digitSum + " which is higher octave of root number " + Root(digitSum)
                : "There is no Master Number for your date of birth.";
        }

        private void PopulateEssence(NumerologyResultModel result, string name)
        {
            var age = 0;
            foreach (var c in (name ?? string.Empty).ToUpperInvariant())
            {
                var index = Alphabet.IndexOf(c); if (index < 0) continue;
                var number = PythagoreanDigits[index] - '0';
                var from = age; age += number;
                result.EssencePeriods.Add(new LegacyNumerologyEssencePeriodModel { Letter = c.ToString(), Number = number, FromAge = from, ToAge = age, Reading = Reading(number) });
                result.EssenceNumber = number;
            }
        }

        private void PopulatePlanes(NumerologyResultModel result, string name)
        {
            foreach (var c in (name ?? string.Empty).ToUpperInvariant())
            {
                string plane;
                if (!PlaneNames.TryGetValue(c, out plane)) continue;
                if (plane == "Mental") result.MentalPlaneCount++;
                else if (plane == "Physical") result.PhysicalPlaneCount++;
                else if (plane == "Emotional") result.EmotionalPlaneCount++;
                else result.IntuitivePlaneCount++;
            }
            var values = new[] { result.MentalPlaneCount, result.PhysicalPlaneCount, result.EmotionalPlaneCount, result.IntuitivePlaneCount };
            var max = values.Max();
            result.ExpressionPlane = max == 0 ? "" : new[] { "Mental", "Physical", "Emotional", "Intuitive" }[Array.IndexOf(values, max)] + " plane of expression";
        }

        private void PopulateMiscellaneous(NumerologyResultModel result, DateTime dob)
        {
            var month = Root(dob.Month); var day = Root(dob.Day); var year = Root(dob.Year);
            var lifePath = result.LifePathNumber;
            var first = dob.Year - (lifePath - 1) + 27;
            if (dob.Year + 27 != first) first += 9;
            result.FirstIndicatorYear = first; result.SecondIndicatorYear = first + 27; result.ThirdIndicatorYear = first + 54;
            result.YearNumber = dob.Year + (dob.Day + dob.Month + dob.Year).ToString(CultureInfo.InvariantCulture).Sum(c => c - '0');
            result.GeneralLuckyNumber = NumberRootFromSolarDate(dob);
            var p1 = Root(month + day); var p2 = Root(day + year); var p3 = Root(p1 + p2); var p4 = Root(month + year);
            result.Pinnacles.Add(new LegacyNumerologyPinnacleModel { Number = p1, Period = "Birth to year " + (36 - lifePath - 1) });
            result.Pinnacles.Add(new LegacyNumerologyPinnacleModel { Number = p2, Period = "Year " + (36 - lifePath - 1) + " to " + (36 - lifePath - 1 + 9) });
            result.Pinnacles.Add(new LegacyNumerologyPinnacleModel { Number = p3, Period = "Year " + (36 - lifePath - 1 + 9) + " to " + (36 - lifePath - 1 + 18) });
            result.Pinnacles.Add(new LegacyNumerologyPinnacleModel { Number = p4, Period = "Year " + (36 - lifePath - 1 + 18) + " onwards" });
            var c1 = ModNine(day - month); var c2 = ModNine(year - day); var c3 = ModNine(c1 - c2);
            result.ChallengesDetailed.Add(new LegacyNumerologyChallengeModel { Number = c1, Period = "First challenge", Reading = Reading(c1) });
            result.ChallengesDetailed.Add(new LegacyNumerologyChallengeModel { Number = c2, Period = "Second challenge", Reading = Reading(c2) });
            result.ChallengesDetailed.Add(new LegacyNumerologyChallengeModel { Number = c3, Period = "Main challenge", Reading = Reading(c3) });
        }

        private void PopulateNameMeaningSummary(NumerologyResultModel result)
        {
            result.FriendshipWithFullName = Friendship(result.BirthdayNumber, result.NameRootNumber);
            result.FriendshipAdvice = result.FriendshipWithFullName == "Friendly" ? "Your name number is harmonious with your birth number." : result.FriendshipWithFullName == "Unfortunate" ? "Your birth and name numbers are in the unfortunate 4/8 series." : "Your birth and name numbers are not naturally friendly.";
        }

        private static int ModNine(int value) { value %= 9; return value < 0 ? value + 9 : value; }
        private static int CalculateVowelNumber(string name) { return Root(CalculateNameNumberStatic(Filter(name, true))); }
        private static int CalculateConsonantNumber(string name) { return Root(CalculateNameNumberStatic(Filter(name, false))); }
        private static string Filter(string value, bool vowels) { return new string((value ?? string.Empty).ToUpperInvariant().Where(c => Alphabet.IndexOf(c) >= 0 && ("AEIOU".IndexOf(c) >= 0) == vowels).ToArray()); }
        private static int CalculateNameNumberStatic(string value) { var total = 0; foreach (var c in value) total += CheiroDigits[Alphabet.IndexOf(c)] - '0'; return total; }
        private static string Reading(int number) { return NumberReadings.ContainsKey(number) ? NumberReadings[number] : ""; }
        private static int NumberRootFromSolarDate(DateTime dob) { return Root(dob.Month); }
        private static int CalculateVehicleSum(string value) { var total = 0; foreach (var c in (value ?? string.Empty).ToUpperInvariant()) { var i = Alphabet.IndexOf(c); total += i >= 0 ? CheiroDigits[i] - '0' : (char.IsDigit(c) ? c - '0' : 0); } return total; }
        private static string NumberPlanet(int number) { var values = new Dictionary<int, string> { { 1, "Sun (Surya)" }, { 2, "Moon (Chandra)" }, { 3, "Jupiter (Guru)" }, { 4, "Rahu" }, { 5, "Mercury (Budh)" }, { 6, "Venus (Shukra)" }, { 7, "Ketu" }, { 8, "Saturn (Shani)" }, { 9, "Mars (Mangal)" } }; return values.ContainsKey(number) ? values[number] : ""; }
        private static string NumberSign(int number) { var values = new[] { "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo", "Libra", "Scorpio", "Sagittarius" }; return number >= 1 && number <= 9 ? values[number] : ""; }
    }
}
