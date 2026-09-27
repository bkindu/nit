using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NirmalaAstrology_Web.Models
{
    public class NumerologyInputModel
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your date of birth.")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(100)]
        [Display(Name = "Nickname")]
        public string NickName { get; set; }

        [StringLength(100)]
        [Display(Name = "Vehicle / PAN / Number to test")]
        public string VehicleOrPan { get; set; }
    }

    // Combined ViewModel for Numerology page (form + results)
    public class NumerologyViewModel
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your date of birth.")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(100)]
        [Display(Name = "Nickname")]
        public string NickName { get; set; }

        [StringLength(100)]
        [Display(Name = "Vehicle / PAN / Number to test")]
        public string VehicleOrPan { get; set; }

        // Results (null on GET, populated on POST)
        public NumerologyResultModel Result { get; set; }
        public bool HasResults { get { return Result != null; } }
    }

    public class NumerologyResultModel
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Core Numbers
        public int LifePathNumber { get; set; }
        public int DestinyNumber { get; set; }
        public int SoulUrgeNumber { get; set; }
        public int PersonalityNumber { get; set; }
        public int BirthdayNumber { get; set; }

        // Interpretations
        public string LifePathInterpretation { get; set; }
        public string DestinyInterpretation { get; set; }
        public string SoulUrgeInterpretation { get; set; }
        public string PersonalityInterpretation { get; set; }
        public string BirthdayInterpretation { get; set; }

        // Additional details
        public string LuckyColor { get; set; }
        public string LuckyDay { get; set; }
        public string LuckyGemstone { get; set; }
        public string FavorableYears { get; set; }
        public string Challenges { get; set; }
        public string Strengths { get; set; }

        // Zodiac connection
        public string RulingPlanet { get; set; }
        public string AssociatedZodiacSign { get; set; }

        // Legacy VB6 numerology output sections.
        public int CompoundNameNumber { get; set; }
        public int NameRootNumber { get; set; }
        public string NameNumberMeaning { get; set; }
        public string BirthNumberMeaning { get; set; }
        public string FullNameLetters { get; set; }
        public string FullNameDigits { get; set; }
        public int VowelCount { get; set; }
        public string Vowels { get; set; }
        public int ConsonantCount { get; set; }
        public string Consonants { get; set; }
        public string VowelEffect { get; set; }
        public string ConsonantEffect { get; set; }
        public int HighestNameNumber { get; set; }
        public string MissingNameNumbers { get; set; }
        public string HighestNumberEffect { get; set; }
        public string PyramidNumber { get; set; }
        public List<string> PyramidRows { get; set; }
        public List<LegacyNumerologyNameVariationModel> NameVariations { get; set; }
        public string FriendshipWithFullName { get; set; }
        public string FriendshipAdvice { get; set; }
        public string MasterNumberText { get; set; }
        public int EssenceNumber { get; set; }
        public List<LegacyNumerologyEssencePeriodModel> EssencePeriods { get; set; }
        public string ExpressionPlane { get; set; }
        public int MentalPlaneCount { get; set; }
        public int PhysicalPlaneCount { get; set; }
        public int EmotionalPlaneCount { get; set; }
        public int IntuitivePlaneCount { get; set; }
        public int YearNumber { get; set; }
        public int GeneralLuckyNumber { get; set; }
        public int FirstIndicatorYear { get; set; }
        public int SecondIndicatorYear { get; set; }
        public int ThirdIndicatorYear { get; set; }
        public List<LegacyNumerologyPinnacleModel> Pinnacles { get; set; }
        public List<LegacyNumerologyChallengeModel> ChallengesDetailed { get; set; }
        public string VehiclePanRootNumber { get; set; }

        public NumerologyResultModel()
        {
            PyramidRows = new List<string>();
            NameVariations = new List<LegacyNumerologyNameVariationModel>();
            EssencePeriods = new List<LegacyNumerologyEssencePeriodModel>();
            Pinnacles = new List<LegacyNumerologyPinnacleModel>();
            ChallengesDetailed = new List<LegacyNumerologyChallengeModel>();
        }
    }

    public sealed class LegacyNumerologyNameVariationModel
    {
        public string Name { get; set; }
        public int CompoundNumber { get; set; }
        public int RootNumber { get; set; }
        public string Friendship { get; set; }
    }

    public sealed class LegacyNumerologyEssencePeriodModel
    {
        public string Letter { get; set; }
        public int Number { get; set; }
        public int FromAge { get; set; }
        public int ToAge { get; set; }
        public string Reading { get; set; }
    }

    public sealed class LegacyNumerologyPinnacleModel
    {
        public int Number { get; set; }
        public string Period { get; set; }
    }

    public sealed class LegacyNumerologyChallengeModel
    {
        public int Number { get; set; }
        public string Period { get; set; }
        public string Reading { get; set; }
    }

    public static class NumerologyHelper
    {
        private static readonly Dictionary<int, string> LifePathMeanings = new Dictionary<int, string>
        {
            { 1, "Life Path Number 1 - The Leader: You are a natural-born leader with an unwavering drive to achieve and innovate. Independence is your greatest strength, and you possess the courage and determination to carve your own path in life. As a pioneer, you are meant to forge new trails and inspire others with your vision and originality. You have a strong sense of self and an innate ability to take charge of any situation. Your challenge lies in balancing your assertiveness with cooperation, and learning that true leadership often involves empowering others rather than doing everything yourself. You are most fulfilled when you are creating, initiating, and leading from the front. According to Vedic numerology, Number 1 is ruled by the Sun (Surya), granting you authority, vitality, and a radiant personality that naturally draws others to your cause." },
            { 2, "Life Path Number 2 - The Diplomat: You are the peacemaker and harmonizer, blessed with extraordinary sensitivity, empathy, and the ability to see both sides of any situation. Your life purpose revolves around creating balance, fostering cooperation, and nurturing relationships. You possess a gentle strength that can calm storms and bring warring parties together. Your intuitive abilities are remarkable, often knowing what others need before they express it. You excel in partnerships and collaborative endeavors, where your mediating skills shine brightest. According to Vedic numerology, Number 2 is ruled by the Moon (Chandra), giving you a luminous, reflective nature and deep emotional intelligence. Your challenge is to overcome indecisiveness and to trust your own needs as much as you trust those of others. You find greatest fulfillment in creating harmony in your environment and nurturing the people you love." },
            { 3, "Life Path Number 3 - The Creative Communicator: You are blessed with extraordinary creative talent, a joyful disposition, and the gift of self-expression in all its forms. Whether through words, art, music, or simply your infectious enthusiasm, you have the ability to uplift and inspire everyone around you. You possess a natural optimism and a childlike wonder that keeps you forever young at heart. Social by nature, you thrive in environments where you can share your ideas and creative vision with others. According to Vedic numerology, Number 3 is ruled by Jupiter (Guru), the planet of wisdom, expansion, and divine grace, granting you a philosophical bent and a generous spirit. Your challenge lies in maintaining focus and discipline, as your many talents can sometimes lead to scattering your energy. You are most fulfilled when you are creating, performing, teaching, or bringing joy to others through your unique gifts." },
            { 4, "Life Path Number 4 - The Builder: You are the foundation upon which great things are built. With your exceptional organizational skills, practical mindset, and tireless work ethic, you have the ability to create lasting structures and systems that endure the test of time. You are methodical, reliable, and deeply committed to turning your visions into tangible reality. According to Vedic numerology, Number 4 is ruled by Rahu (the North Node of the Moon), which gives you an unconventional approach to problem-solving and a drive that is both intense and focused. Your challenge is to avoid becoming too rigid or overly focused on details at the expense of the bigger picture. You find greatest fulfillment in building something meaningful from the ground up\u2014whether a business, a home, a family, or a body of knowledge. Your dedication to hard work and your integrity are your greatest assets." },
            { 5, "Life Path Number 5 - The Freedom Seeker: You are the adventurer and explorer of the numerological system, driven by an insatiable curiosity and a deep need for freedom, variety, and new experiences. Your life is a constant dance of change and transformation, and you embrace it with enthusiasm and courage. You are versatile, adaptable, and possess a magnetic charm that draws opportunities to you like a magnet. According to Vedic numerology, Number 5 is ruled by Mercury (Budh), the planet of intellect, communication, and versatility, granting you quick thinking and exceptional communication skills. Your challenge lies in maintaining focus and commitment, as your love of freedom can sometimes make it difficult to settle into any one path or relationship. You are most fulfilled when you are traveling, learning, communicating, and experiencing the full spectrum of what life has to offer. Your adaptability and resourcefulness are your greatest strengths." },
            { 6, "Life Path Number 6 - The Nurturer: You are the heart and soul of your community, blessed with an extraordinary capacity for love, responsibility, and service to others. Your life purpose is deeply connected to family, home, healing, and creating beauty and harmony in your environment. You possess a warm, magnetic personality and a natural talent for counseling, teaching, and caring for those in need. According to Vedic numerology, Number 6 is ruled by Venus (Shukra), the planet of love, beauty, and harmony, granting you refined aesthetic sensibilities and a deep appreciation for art, music, and the finer things in life. Your challenge is to balance your selfless nature with proper self-care, and to avoid taking on too much responsibility for others' happiness. You are most fulfilled when you are creating a beautiful home, raising a family, healing others, or contributing to your community through service and creative expression." },
            { 7, "Life Path Number 7 - The Seeker of Truth: You are the philosopher, mystic, and truth-seeker of the numerological system, blessed with a deep, analytical mind and an insatiable thirst for knowledge and understanding. Your life purpose revolves around spiritual growth, intellectual mastery, and the pursuit of hidden truths that lie beneath the surface of reality. You are naturally introspective and contemplative, often preferring the company of your own thoughts to superficial social interactions. According to Vedic numerology, Number 7 is ruled by Ketu (the South Node of the Moon), which grants you spiritual depth, psychic sensitivity, and a natural inclination towards meditation and inner exploration. Your challenge lies in avoiding excessive isolation and learning to share your wisdom with the world. You are most fulfilled when you are studying, researching, meditating, or exploring the mysteries of existence. Your analytical mind and spiritual depth are your greatest gifts." },
            { 8, "Life Path Number 8 - The Powerhouse: You are the executive and material master of the numerological system, blessed with extraordinary ambition, business acumen, and the ability to manifest abundance on a grand scale. Your life purpose involves mastering the material world and learning to use power, authority, and resources wisely for the benefit of all. You possess a natural authority and organizational ability that enables you to lead large-scale endeavors and achieve significant material success. According to Vedic numerology, Number 8 is ruled by Saturn (Shani), the planet of karma, discipline, and time, granting you patience, endurance, and the understanding that true success comes through sustained effort and integrity. Your challenge lies in balancing your material ambitions with your spiritual growth, and in learning that true power comes from service rather than domination. You are most fulfilled when you are building empires, managing organizations, and using your influence to create positive change in the world." },
            { 9, "Life Path Number 9 - The Humanitarian: You are the old soul and humanitarian of the numerological system, blessed with a broad vision, a compassionate heart, and a deep understanding of the universal human experience. Your life purpose involves serving humanity, completing karmic cycles, and using your wisdom and experience to uplift others. You possess a natural charisma and a generous spirit that inspires others to follow your example of selfless service. According to Vedic numerology, Number 9 is ruled by Mars (Mangal), the planet of courage, energy, and protection, granting you the strength and determination to fight for causes you believe in. Your challenge lies in learning to let go of attachments\u2014people, situations, and things\u2014when their purpose in your life is complete. You are most fulfilled when you are serving a cause greater than yourself, teaching, healing, or creating art that speaks to the universal human condition. Your wisdom, compassion, and generosity are your greatest legacies." },
            { 11, "Life Path Number 11 - The Master Intuitive: You carry the vibration of spiritual illumination and intuitive mastery. As a Master Number, 11 carries amplified energy that grants you extraordinary psychic sensitivity, inspirational power, and a direct connection to higher wisdom. You are meant to be a channel for divine inspiration, using your heightened awareness to guide and illuminate the path for others. Your intuitive abilities are exceptional, often receiving guidance through dreams, sudden insights, and gut feelings that prove remarkably accurate. According to Vedic numerology, Number 11 resonates with the energy of spiritual enlightenment and is associated with Lord Shiva in his form as the destroyer of ignorance. Your challenge is to manage the intensity of your sensitivity, to ground your spiritual insights in practical reality, and to overcome the self-doubt that often accompanies those who carry such elevated vibrations. You are most fulfilled when you are inspiring others, pursuing spiritual growth, and using your gifts in service of humanity's evolution." },
            { 22, "Life Path Number 22 - The Master Builder: You carry the vibration of the Master Builder, combining the vision of 11 with the practical mastery of 4 to create structures, systems, and institutions that serve humanity on a grand scale. As the most powerful of the Master Numbers, 22 grants you the ability to turn the most ambitious dreams into tangible reality. You are destined to leave a lasting legacy through your work in architecture, engineering, government, or any field that requires the marriage of grand vision with practical execution. According to Vedic numerology, Number 22 resonates with the energy of cosmic creation and is associated with the divine architect Vishwakarma. Your challenge is to manage the enormous pressure of your potential, to avoid being overwhelmed by the scope of your vision, and to remember that even the grandest achievements are built one step at a time. You are most fulfilled when you are building something that will benefit generations to come." },
            { 33, "Life Path Number 33 - The Master Teacher: You carry the rarest and most elevated vibration in numerology\u2014that of the Master Teacher or Master Healer. Number 33 combines the intuitive power of 11 with the building capacity of 22, adding the dimension of selfless love and spiritual teaching. You are here to uplift humanity through your example of unconditional love, compassion, and dedicated service. Your healing presence can transform the lives of those around you, often without you even being consciously aware of the impact you are making. According to Vedic numerology, Number 33 resonates with the energy of divine love and is associated with the supreme teacher principle (Dattatreya). Your challenge is to bear the weight of your calling without being crushed by it, to balance your selfless nature with proper boundaries, and to trust that your very presence is enough to fulfill your mission. You are most fulfilled when you are teaching, healing, nurturing, and being a living example of love and wisdom in action." }
        };

        public static int ReduceToSingleDigit(int number, bool keepMasterNumbers = true)
        {
            if (keepMasterNumbers && (number == 11 || number == 22 || number == 33))
                return number;

            int sum = 0;
            while (number > 0)
            {
                sum += number % 10;
                number /= 10;
            }

            if (keepMasterNumbers && (sum == 11 || sum == 22 || sum == 33))
                return sum;

            if (sum > 9)
                return ReduceToSingleDigit(sum, keepMasterNumbers);

            return sum;
        }

        public static int CalculateLifePathNumber(DateTime dob)
        {
            int reducedDay = ReduceToSingleDigit(dob.Day, false);
            int reducedMonth = ReduceToSingleDigit(dob.Month, false);
            int reducedYear = ReduceToSingleDigit(dob.Year, false);
            int sum = reducedDay + reducedMonth + reducedYear;
            return ReduceToSingleDigit(sum);
        }

        public static int CalculateDestinyNumber(string name)
        {
            // Pythagorean numerology chart for destiny/expression number
            var letterValues = new Dictionary<char, int>
            {
                {'A',1},{'B',2},{'C',3},{'D',4},{'E',5},{'F',6},{'G',7},{'H',8},{'I',9},
                {'J',1},{'K',2},{'L',3},{'M',4},{'N',5},{'O',6},{'P',7},{'Q',8},{'R',9},
                {'S',1},{'T',2},{'U',3},{'V',4},{'W',5},{'X',6},{'Y',7},{'Z',8}
            };

            int sum = 0;
            string cleanName = (name ?? "").ToUpper().Replace(" ", "").Replace("-", "").Replace("'", "");

            foreach (char c in cleanName)
            {
                if (letterValues.ContainsKey(c))
                    sum += letterValues[c];
            }

            return ReduceToSingleDigit(sum);
        }

        public static int CalculateSoulUrgeNumber(string name)
        {
            var vowelValues = new Dictionary<char, int>
            {
                {'A',1},{'E',5},{'I',1},{'O',6},{'U',3}
            };

            int sum = 0;
            string cleanName = (name ?? "").ToUpper().Replace(" ", "").Replace("-", "").Replace("'", "");

            foreach (char c in cleanName)
            {
                if (vowelValues.ContainsKey(c))
                    sum += vowelValues[c];
            }

            return ReduceToSingleDigit(sum);
        }

        public static int CalculatePersonalityNumber(string name)
        {
            var consonantValues = new Dictionary<char, int>
            {
                {'B',2},{'C',3},{'D',4},{'F',6},{'G',7},{'H',8},{'J',1},{'K',2},{'L',3},
                {'M',4},{'N',5},{'P',7},{'Q',8},{'R',9},{'S',1},{'T',2},{'V',4},{'W',5},
                {'X',6},{'Y',7},{'Z',8}
            };

            int sum = 0;
            string cleanName = (name ?? "").ToUpper().Replace(" ", "").Replace("-", "").Replace("'", "");

            foreach (char c in cleanName)
            {
                if (consonantValues.ContainsKey(c))
                    sum += consonantValues[c];
            }

            return ReduceToSingleDigit(sum);
        }

        public static int CalculateBirthdayNumber(DateTime dob)
        {
            return ReduceToSingleDigit(dob.Day);
        }

        public static string GetLifePathInterpretation(int number)
        {
            if (LifePathMeanings.ContainsKey(number))
                return LifePathMeanings[number];
            return "The vibrations of your life path number suggest a unique and special purpose in this lifetime.";
        }

        public static Dictionary<int, string> GetNumberDetails()
        {
            return new Dictionary<int, string>
            {
                { 1, "Sun (Surya)" },
                { 2, "Moon (Chandra)" },
                { 3, "Jupiter (Guru)" },
                { 4, "Rahu (North Node)" },
                { 5, "Mercury (Budh)" },
                { 6, "Venus (Shukra)" },
                { 7, "Ketu (South Node)" },
                { 8, "Saturn (Shani)" },
                { 9, "Mars (Mangal)" },
                { 11, "Sun + Moon (Illumination)" },
                { 22, "Jupiter + Saturn (Master Builder)" },
                { 33, "Venus + Mars (Master Healer)" }
            };
        }
    }
}