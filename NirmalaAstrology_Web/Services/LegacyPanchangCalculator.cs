using System;
using System.Globalization;
using System.Linq;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// LEGACY TRACE: a2Functions_Antronomy.FnPanchang (partial port for web display).
    /// Inputs: Sun/Moon from dPlt via KundliResult; date/place from KundliWorkflowRequest.
    /// </summary>
    public sealed class LegacyPanchangCalculator
    {
        private static readonly string[] Tithis =
        {
            "", "PRATIPADA", "DVITIYA", "TRITIYA", "CHATURTHI", "PANCHAMI", "SASTHI", "SAPTAMI", "ASTAMI",
            "NAVAMI", "DASAMI", "EKADASI", "DVADASI", "TRAYODASI", "CHATURDASI", "PURNIMA",
            "PRATIPADA", "DVITIYA", "TRITIYA", "CHATURTHI", "PANCHAMI", "SASTHI", "SAPTAMI", "ASTAMI",
            "NAVAMI", "DASAMI", "EKADASI", "DVADASI", "TRAYODASI", "CHATURDASI", "AMAVASYA"
        };

        private static readonly string[] Yogas =
        {
            "VISKAMBHA", "PREETI", "AYUSMAN", "SAUBHAGYA", "SHOBHANA", "ATIGANDA", "SUKARMA", "DHRITI",
            "SHOOLA", "GANDA", "VRIDHI", "DHRUVA", "VYAGHATA", "HARSHANA", "VAJRA", "SIDDHI", "VYATIPAT",
            "VARIYAN", "PARIGHA", "SHIVA", "SIDDHA", "SADHYA", "SHUBHA", "SHUKLA", "BRAHMA", "INDRA", "VAIDHRITI"
        };

        private static readonly string[] Nakshatras =
        {
            "ASHWINI", "BHARANI", "KRITIKA", "ROHINI", "MRIGSHIRA", "ARDRA", "PUNARVASU", "PUSHYA", "ASHLESHA",
            "MAGHA", "P.PHALGUNI", "U.PHALGUNI", "HASTA", "CHITRA", "SWATI", "VISAKHA", "ANURADHA", "JYESHTHA",
            "MOOLA", "P.SHADA", "U.SHADA", "SHARAWAN", "DHANISTHA", "SHATBHISHA", "P.BHADRAPAD", "U.BHADRAPAD", "REVATI"
        };

        private static readonly string[] SaurMonths =
        {
            "VAISAKHA", "JYAISTHA", "ASADHA", "SRAVANA", "BHADRAPAD", "ASVINA",
            "KARTIKA", "MARGSHIRSA", "PAUSA", "MAGHA", "PHALGUNA", "CHAITRA"
        };

        private static readonly string[] Vaar =
        {
            "RAVIWAAR", "SOMWAAR", "MANGALWAAR", "BUDHWAAR", "GURUWAAR", "SHUKARWAAR", "SHANIWAAR"
        };

        private static readonly string[] Rashis =
        {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };

        public LegacyPanchangModel Calculate(KundliResult result, KundliWorkflowRequest request)
        {
            // Step 1: Read Sun / Moon / Lagna from kundli result
            PlanetPosition sun = Find(result, "Sun");
            PlanetPosition moon = Find(result, "Moon");
            PlanetPosition lagna = Find(result, "Lagna", "Ascendant");

            double sunLong = sun == null ? 0.0 : sun.TotalDegrees;
            double moonLong = moon == null ? 0.0 : moon.TotalDegrees;

            // Step 2: Tithi / Nakshatra / Yoga / Karana from longitudes
            double phase = Normalize(moonLong - sunLong) / 12.0;
            int tithiNo = Math.Min(30, (int)Math.Floor(phase) + 1);
            int nakNo = Math.Min(27, (int)Math.Floor(Normalize(moonLong) / (360.0 / 27.0)) + 1);
            int pada = Math.Min(4, (int)Math.Floor(Normalize(moonLong) / (360.0 / 108.0)) % 4 + 1);
            int yogaNo = Math.Min(27, (int)Math.Floor(Normalize(moonLong + sunLong) / (360.0 / 27.0)) + 1);
            int karanNo = Math.Min(60, (int)Math.Floor(phase * 2.0) + 1);

            // VB6 FnPanchang: nirayana saur month, sayana sun sign, gati-based buk/bog.
            int solarRashi = Math.Min(12, (int)Math.Floor(Normalize(sunLong) / 30.0) + 1);
            int ascRashi = lagna == null ? 0 : lagna.Rashi;
            double birthHours = request.Hour + request.Minute / 60.0;
            double tzHours = AstroMath.FnDg(request.TimezoneOffset);
            double dLatt = AstroMath.FnDg(request.Latitude);
            double dLong = AstroMath.FnDg(request.Longitude);
            double dJdf = result.JulianDayFraction > 0
                ? result.JulianDayFraction
                : AstroMath.Dt2Jdf(new DateTime(request.Year, request.Month, request.Day), birthHours, tzHours);

            double sayanaSun = Normalize(sunLong + (result.AyanamsaNutated > 0 ? result.AyanamsaNutated : result.Ayanamsa));
            int sunSynNum = Math.Min(12, (int)Math.Floor(sayanaSun / 30.0) + 1);
            string sunSign = SayanaSign(sunSynNum);

            var sunRs = AstroMath.SunRiseSetForBirthDay(
                request.Year, request.Month, request.Day,
                request.Latitude, request.Longitude, request.TimezoneOffset);
            double sunrise = sunRs.SunRiseHours;
            double sunset = sunRs.SunSetHours;
            if (sunset < sunrise) sunset += 24.0;

            int saurMonth = Math.Min(12, AstroMath.FnTgr(AstroMath.FnQ(sunLong)) + 1);
            if (saurMonth < 1) saurMonth = solarRashi;
            int vikram = request.Year + (saurMonth < 10 ? 57 : 56);
            if (saurMonth == 9 && request.Month == 1) vikram--;
            int saka = request.Year + (sunSynNum < 11 ? -78 : -79);
            if (sunSynNum == 10 && request.Month == 1) saka--;

            // VB6 LBR uses global birth dJDF on the +1 call, so panchang sun gati is ~0.
            double moonGati = Math.Abs(DailyMotionDegrees(result, "Moon")) * 60.0;
            var birthEl = PanchangElements(sunLong, moonLong, 0.0, moonGati);

            var riseCalc = new KundliCalculator().CalculateAtLocalHours(
                request.Day, request.Month, request.Year, sunrise,
                request.Longitude, request.Latitude, request.TimezoneOffset);
            double riseMoonGati = Math.Abs(DailyMotionDegrees(riseCalc, "Moon")) * 60.0;
            var riseEl = PanchangElements(riseCalc.Sun, riseCalc.Moon, 0.0, riseMoonGati);

            string moonRashiName = moon == null ? "" : Rashis[moon.Rashi];
            int moonRashi = moon == null ? 0 : moon.Rashi;
            string ayan = sunSynNum > 3 && sunSynNum < 10 ? "DAKSHIN" : "UTTAR";
            string gola = sunSynNum > 0 && sunSynNum < 7 ? "UTTAR" : "DAKSHIN";

            // VB6 Hora_Sangya(dPlt(3)) — full Avakhoda + Ghat chakram
            var hora = HoraSangyaCalculator.FromMoon(moonLong);

            DateTime date = new DateTime(request.Year, request.Month, request.Day, request.Hour, request.Minute, 0);
            double meanSun = AstroMath.FnFr((279.205588 + dJdf * 0.98564735) / 360.0 + 100.0) * 360.0;

            double meanAnomalySun = AstroMath.FnFr(2.994397 + dJdf / 365.25964) * 360.0;
            double equationOfTime =
                AstroMath.FnDg(9.520001) * AstroMath.FnSn(2.0 * meanSun)
                - AstroMath.FnDg(7.4) * AstroMath.FnSn(meanAnomalySun)
                - AstroMath.FnDg(0.05) * AstroMath.FnSn(2.0 * meanAnomalySun)
                - AstroMath.FnDg(0.13) * AstroMath.FnSn(4.0 * meanSun);
            string velantar = AstroMath.FormatRa(Math.Abs(equationOfTime)) + (equationOfTime < 0 ? "(-)" : "(+)");

            // VB6 aPanchanga Dims a local dSunSyn and never assigns it (the assignment is commented),
            // so declination and chara pal print as zero.
            string decNs = AstroMath.FormatRa(0) + " NORTH";
            string charaPal = AstroMath.FormatRa(0).Trim() + "(+)m";

            double palbha = 12.0 * AstroMath.FnTn(dLatt);
            double nirayanaMeanSun = AstroMath.FnR(meanSun - result.Ayanamsa, 360.0);
            double trueRahu = result.TrueRahu > 0 ? result.TrueRahu : (result.Rahu);
            double localJd = sunRs.JulianRise + tzHours / 24.0;
            double lmtPacked = AstroMath.FnDm(AstroMath.FnR(birthHours - (82.5 - dLong) * 4.0 / 60.0, 24.0) + 0.00005);
            double ishat = AstroMath.IshatKaalGpDec(dJdf, dLatt, dLong);
            double dinman = (sunset - sunrise) * 2.5;

            LegacyPanchangModel model = new LegacyPanchangModel();
            model.Tithi = birthEl.Tithi;
            model.Nakshatra = birthEl.Nakshatra;
            model.NakshatraPada = birthEl.Charan;
            model.Yoga = birthEl.Yoga;
            model.Karan = birthEl.Karan;
            model.Paksha = birthEl.Paksha;
            model.VaarEnglish = date.DayOfWeek.ToString().ToUpperInvariant();
            model.VaarHindu = Vaar[(int)date.DayOfWeek];
            model.Rashi = moonRashiName;
            model.SunSign = sunSign;
            model.Paya = Paya(ascRashi, moonRashi);
            model.SaurMaas = SaurMonths[Math.Max(0, saurMonth - 1)];
            model.Ayan = ayan;
            model.Gola = gola;
            model.Ritu = Ritu(sunSynNum);
            model.Samvatsar = SamvatsarName(dJdf);
            model.VikramiSamvat = vikram;
            model.SakaSamvat = saka;
            model.Sunrise = AstroMath.FormatRak(sunrise);
            model.Sunset = AstroMath.FormatRak(sunset >= 12.0 ? sunset - 12.0 : sunset);
            model.Ishtakal = AstroMath.FormatRak(ishat) + " Ghatyadi";
            model.Dinman = AstroMath.FormatRak(dinman) + " Ghatyadi";
            model.SiderealTime = AstroMath.FormatSiderealTimeHours(result.SiderealTime);
            model.Ayanamsa = AstroMath.FormatAyanamsa(result.Ayanamsa);
            // +1e-9 avoids 60*hours landing at n-epsilon (FNDM prints 02:16:60 instead of 02:17:00).
            model.Gmt = AstroMath.FormatRak(AstroMath.FnR(birthHours - tzHours, 24.0) + 1e-9);
            model.ApparentNoon = AstroMath.FormatRak((sunset - sunrise) / 2.0 + sunrise);
            model.LocalMeanTime = AstroMath.FormatRak(lmtPacked);
            model.DeclinationOfSun = decNs;

            model.TithiEnding = EndingFromSunrise(localJd, riseEl.TitBuk, riseEl.TitBog);
            model.KaranEnding = EndingFromSunrise(localJd, riseEl.KrnBuk, riseEl.KrnBog);
            model.YogaEnding = EndingFromSunrise(localJd, riseEl.YogBuk, riseEl.YogBog);
            model.NakshatraEnding = EndingFromSunrise(localJd, riseEl.NakBuk, riseEl.NakBog);
            model.VaarEnding = AstroMath.FormatRak(sunrise) + " to next sunrise";
            model.SunriseTithi = riseEl.Tithi;
            model.SunriseNakshatra = riseEl.Nakshatra;
            model.SunriseYoga = riseEl.Yoga;
            model.SunriseKaran = riseEl.Karan;

            // Hora Chakram
            model.HoraRashi = hora.RashiName;
            model.HoraRashiLord = hora.RashiLord;
            model.HoraVaran = hora.Varna;
            model.HoraVashya = hora.Vashya;
            model.HoraHansak = hora.Hansak;
            model.HoraRashiName = hora.RashiNameFrom;
            model.HoraYoni = hora.Yoni;
            model.HoraGana = hora.Gana;
            model.HoraNadi = hora.Nadi;
            model.HoraYunja = hora.Yunja;
            model.HoraNameLetter = hora.NameLetter;
            model.HoraVarag = hora.VargaName;
            model.HoraPaya = model.Paya;
            model.HoraSunSign = sunSign;

            // Ghat Chakram (from moon rashi via Hora_Sangya)
            model.GhatMaas = hora.GhatMaas;
            model.GhatTithies = hora.GhatTithies;
            model.GhatVaar = hora.GhatVaar;
            model.GhatNakshatra = hora.GhatNakshatra;
            model.GhatYog = hora.GhatYog;
            model.GhatKaran = hora.GhatKaran;
            model.GhatPrahar = hora.GhatPrahar;
            model.GhatLagan = hora.GhatLagan;
            model.GhatPlanetPositions = hora.GhatPlanetPositions;
            model.GhatGrah = hora.GhatGrah;
            model.GhatDhanya = hora.GhatDhanya;
            model.GhatJati = hora.GhatJati;
            model.GhatGender = hora.GhatGender;
            model.GhatKaruradi = hora.GhatKaruradi;
            model.GhatCharadi = hora.GhatCharadi;
            model.GhatVarna = hora.GhatVarna;

            // Miscellaneous
            model.Velantar = velantar;
            model.CharaPal = charaPal;
            model.Palbha = AstroMath.FormatRak(palbha) + " Anguladi";
            model.MeanSun = AstroMath.FormatRakv(nirayanaMeanSun);
            model.WarTimeCorrection = "NIL";
            model.TrueRahu = AstroMath.FormatRakv(trueRahu);

            model.TithiBhukat = AstroMath.FormatRa(birthEl.TitBuk);
            model.TithiBhogya = AstroMath.FormatRa(birthEl.TitBog);
            model.YogaBhukat = AstroMath.FormatRa(birthEl.YogBuk);
            model.YogaBhogya = AstroMath.FormatRa(birthEl.YogBog);
            model.KaranBhukat = AstroMath.FormatRa(birthEl.KrnBuk);
            model.KaranBhogya = AstroMath.FormatRa(birthEl.KrnBog);
            model.NakshatraBhukat = AstroMath.FormatRa(birthEl.NakBuk);
            model.NakshatraBhogya = AstroMath.FormatRa(birthEl.NakBog);
            model.NakshatraBhukatVikala = birthEl.NakBukKla.ToString("00000", CultureInfo.InvariantCulture) + " Vikala";
            model.NakshatraBhogyaVikala = birthEl.NakBogKla.ToString("00000", CultureInfo.InvariantCulture) + " Vikala";
            model.Bhabhog = "48000 Vikala";
            return model;
        }

        /// <summary>VB6 ending span from sunrise JD: JD2DT + Kla(FNDM(clock)).</summary>
        private static string EndingFromSunrise(double localJulian, double bukGp, double bogGp)
        {
            double start = localJulian - bukGp / (2.5 * 24.0);
            double end = localJulian + bogGp / (2.5 * 24.0);
            return AstroMath.Jd2Dt(start) + " " + AstroMath.FormatKlaPackedHours(AstroMath.FnFr(start) * 24.0).Trim()
                 + " to " + AstroMath.Jd2Dt(end) + " " + AstroMath.FormatKlaPackedHours(AstroMath.FnFr(end) * 24.0).Trim()
                 + " hours.";
        }

        private static double DailyMotionDegrees(KundliResult result, string name)
        {
            var p = Find(result, name);
            return p == null ? 0.0 : p.DailyMotion;
        }

        private sealed class ElementSet
        {
            public string Tithi;
            public string Yoga;
            public string Nakshatra;
            public string Karan;
            public string Paksha;
            public int Charan;
            public double TitBuk, TitBog, KrnBuk, KrnBog, YogBuk, YogBog, NakBuk, NakBog, NakBukKla, NakBogKla;
        }

        /// <summary>VB6 FnPanchang tithi/karan/yoga/nak buk-bog from nirayana longitudes and arcmin/day gati.</summary>
        private static ElementSet PanchangElements(double sunNir, double moonNir, double sunGatiArcMin, double moonGatiArcMin)
        {
            if (moonGatiArcMin < 1.0) moonGatiArcMin = 1.0;
            double gatiDiff = moonGatiArcMin - sunGatiArcMin;
            if (Math.Abs(gatiDiff) < 1.0) gatiDiff = moonGatiArcMin;

            double dTit = AstroMath.FnR(moonNir - sunNir, 360.0) * 60.0 / 720.0;
            int titNum = (int)Math.Floor(dTit) + 1;
            if (titNum < 1) titNum = 1;
            if (titNum > 30) titNum = 30;
            double titBukKla = AstroMath.FnFr(dTit) * 720.0 * 60.0;
            double titKul = 720.0 / gatiDiff * 60.0;
            double titBuk = titBukKla / gatiDiff;
            double titBog = titKul - titBuk;

            double krnBuk = titBuk > titKul / 2.0 ? titBuk - titKul / 2.0 : titBuk;
            double krnBog = titKul / 2.0 - krnBuk;
            int krnNum = (int)Math.Floor(dTit * 2.0) + 1;

            double dNak = moonNir * 60.0 / 800.0;
            double nakBukKla = AstroMath.FnFr(dNak) * 48000.0;
            double nakBogKla = 48000.0 - nakBukKla;
            int charan = (int)Math.Floor(nakBukKla / 12000.0) + 1;
            if (charan < 1) charan = 1;
            if (charan > 4) charan = 4;
            double arcLeft = AstroMath.FnFr(dNak) * 800.0;
            double nakBuk = arcLeft * 60.0 / moonGatiArcMin;
            double nakBog = (800.0 - arcLeft) * 60.0 / moonGatiArcMin;
            int nakNum = (int)Math.Floor(dNak) + 1;
            if (nakNum < 1) nakNum = 1;
            if (nakNum > 27) nakNum = 27;

            double gatiYog = (sunGatiArcMin + moonGatiArcMin) * 60.0;
            double dYog = AstroMath.FnR(moonNir + sunNir, 360.0) * 60.0 / 800.0;
            int yogNum = (int)Math.Floor(dYog) + 1;
            if (yogNum < 1) yogNum = 1;
            if (yogNum > 27) yogNum = 27;
            double yogBukKla = AstroMath.FnFr(dYog) * 800.0;
            double yogRate = gatiYog / 60.0;
            if (Math.Abs(yogRate) < 1.0) yogRate = moonGatiArcMin;
            double yogBuk = yogBukKla * 60.0 / yogRate;
            double yogBog = (800.0 - yogBukKla) * 60.0 / yogRate;

            int pks = (int)Math.Floor(titNum / 15.0) + 1;
            if (pks == 3) pks = 2;
            string[] tithis =
            {
                "", "PRATIPADA", "DVITIYA", "TRITIYA", "CHATURTHI", "PANCHAMI", "SASTHI", "SAPTAMI", "ASTAMI",
                "NAVAMI", "DASAMI", "EKADASI", "DVADASI", "TRAYODASI", "CHATURDASI", "PURNIMA",
                "PRATIPADA", "DVITIYA", "TRITIYA", "CHATURTHI", "PANCHAMI", "SASTHI", "SAPTAMI", "ASTAMI",
                "NAVAMI", "DASAMI", "EKADASI", "DVADASI", "TRAYODASI", "CHATURDASI", "AMAVASYA"
            };
            string[] naks =
            {
                "", "ASHWINI", "BHARANI", "KRITIKA", "ROHINI", "MRIGSHIRA", "ARDRA", "PUNARVASU", "PUSHYA", "ASHLESHA",
                "MAGHA", "P.PHALGUNI", "U.PHALGUNI", "HASTA", "CHITRA", "SWATI", "VISAKHA", "ANURADHA", "JYESTHA",
                "MOOLA", "P.SHADA", "U.SHADA", "SHARAWAN", "DHANISTHA", "SHATBHISHA", "P.BHADRAPAD", "U.BHADRAPAD", "REVATI"
            };
            string[] yogas =
            {
                "", "VISKAMBHA", "PREETI", "AYUSMAN", "SAUBHAGYA", "SHOBHANA", "ATIGANDA", "SUKARMA", "DHRITI",
                "SHOOLA", "GANDA", "VRIDHI", "DHRUVA", "VYAGHATA", "HARSHANA", "VAJRA", "SIDDHI", "VYATIPAT",
                "VARIYAN", "PARIGHA", "SHIVA", "SIDDHA", "SADHYA", "SHUBHA", "SHUKLA", "BRAHMA", "INDRA", "VAIDHRITI"
            };

            return new ElementSet
            {
                Tithi = tithis[titNum],
                Yoga = yogas[yogNum],
                Nakshatra = naks[nakNum],
                Karan = Karan(krnNum),
                Paksha = pks == 1 ? "SHUKLA" : "KRISHAN",
                Charan = charan,
                TitBuk = titBuk,
                TitBog = titBog,
                KrnBuk = krnBuk,
                KrnBog = krnBog,
                YogBuk = yogBuk,
                YogBog = yogBog,
                NakBuk = nakBuk,
                NakBog = nakBog,
                NakBukKla = nakBukKla,
                NakBogKla = nakBogKla
            };
        }

        private static string SayanaSign(int n)
        {
            string[] names =
            {
                "", "ARIES", "TAURUS", "GEMINI", "CANCER", "LEO", "VIRGO",
                "LIBRA", "SCORPIO", "SAGITTARIUS", "CAPRICORN", "AQUARIUS", "PISCES"
            };
            return n >= 1 && n <= 12 ? names[n] : "";
        }

        private static PlanetPosition Find(KundliResult result, params string[] names)
        {
            return result.PlanetPositions.FirstOrDefault(p =>
                names.Any(n => string.Equals(p.Name, n, StringComparison.OrdinalIgnoreCase)));
        }

        private static double Normalize(double x)
        {
            x = x % 360.0;
            if (x < 0.0)
            {
                x = x + 360.0;
            }
            return x;
        }

        private static string Ritu(int r)
        {
            if (r == 12 || r == 1) return "VASANT";
            if (r == 2 || r == 3) return "GRISHAM";
            if (r == 4 || r == 5) return "VARSHA";
            if (r == 6 || r == 7) return "SHARAD";
            if (r == 8 || r == 9) return "HEMANT";
            return "SHISHIR";
        }

        private static string SamvatsarName(double jd)
        {
            string[] names =
            {
                "PRABHAVA", "VIBHAVA", "SUKLA", "PRAMODA", "PRAJAPATI", "ANGIRAS", "SRIMUKHA", "BHAVA", "YUVAN", "DHATRI",
                "ISVARA", "BAHUDHANYA", "PRAMATHIN", "VIKRAMA", "VRISHA", "CHITRABHANU", "SUBHANU", "TARANA", "PARTHIVA", "VYAYA",
                "SARVAJIT", "SARVADHARIN", "VIRODHIN", "VIKRITA", "KHARA", "NANDANA", "VIJAYA", "JAYA", "MANMATHA", "DURMUKHA",
                "HEMALAMBA", "VILAMBA", "VIKARIN", "SARVARI", "PLAVA", "SUBHAKRIT", "SOBHANA", "KRODHIN", "VISVAVASU", "PARABHAVA",
                "PLAVANGA", "KEELAKA", "SAUMYA", "SADHARANA", "VIRODHAKRIT", "PARIDHAVIN", "PRAMADIN", "ANANDA", "RAKSHASA", "ANALA",
                "PINGALA", "KALAYUKTA", "SIDHHARTHIN", "RAUDRA", "DURMATI", "DUNDUBHI", "RUDIRODGARI", "RAKTAKSHA", "KRODHANA", "KSHAYA"
            };

            double x = (225.28676 + 3034.692024 * jd / 36525.0) / 30.0 + 39.0;
            double wrapped = ((x % 60.0) + 60.0) % 60.0;
            int n = (int)Math.Floor(wrapped);
            if (n == 0)
            {
                n = 60;
            }
            return names[Math.Min(59, n - 1)];
        }

        private static string Paya(int asc, int moon)
        {
            if (asc < 1 || moon < 1)
            {
                return "";
            }

            int d = (moon - asc + 12) % 12;
            string[] payaNames =
            {
                "SVARN", "RAJAT", "TAMAR", "LAUH", "RAJAT", "SVARN",
                "TAMAR", "LAUH", "RAJAT", "TAMAR", "SVARN", "LAUH"
            };
            return payaNames[d];
        }

        private static string Lord(int r)
        {
            if (r == 1 || r == 8) return "Mars";
            if (r == 2 || r == 7) return "Venus";
            if (r == 3 || r == 6) return "Mercury";
            if (r == 4) return "Moon";
            if (r == 5) return "Sun";
            if (r == 9 || r == 12) return "Jupiter";
            if (r == 10 || r == 11) return "Saturn";
            return "-";
        }

        private static string Karan(int n)
        {
            if (n == 1) return "KINTUGHAN";
            if (n == 58) return "SHAKUNI";
            if (n == 59) return "CHATUSPAD";
            if (n >= 58) return "NAG";

            string[] movable =
            {
                "BAVA", "BALAVA", "KAULAVA", "TAITALA", "GARA", "VANIJ", "VISHTI"
            };
            return movable[(n - 2) % 7];
        }

        private static string Ghati(double x)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
            {
                return "-";
            }
            return x.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private static string Portion(double x)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
            {
                return "-";
            }
            double minutes = Math.Max(0.0, x) * 60.0;
            return minutes.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private static string Clock(double hours)
        {
            hours = hours % 24.0;
            if (hours < 0.0)
            {
                hours = hours + 24.0;
            }

            int h = (int)Math.Floor(hours);
            int m = (int)Math.Floor((hours - h) * 60.0);
            return h.ToString("00") + ":" + m.ToString("00");
        }

        private static double Julian(DateTime d)
        {
            int y = d.Year;
            int m = d.Month;
            if (m <= 2)
            {
                y = y - 1;
                m = m + 12;
            }

            int a = y / 100;
            int b = 2 - a + a / 4;
            double jd =
                Math.Floor(365.25 * (y + 4716))
                + Math.Floor(30.6001 * (m + 1))
                + d.Day
                + b
                - 1524.5
                + d.TimeOfDay.TotalDays;
            return jd;
        }

        private static double Sunrise(DateTime date, double lat, double lon, double tz, bool rise)
        {
            int n = date.DayOfYear;

            double decl =
                23.44 * Math.Sin((360.0 / 365.0) * (n - 81) * Math.PI / 180.0);

            double cosH =
                (Math.Cos(90.833 * Math.PI / 180.0)
                 - Math.Sin(lat * Math.PI / 180.0) * Math.Sin(decl * Math.PI / 180.0))
                / (Math.Cos(lat * Math.PI / 180.0) * Math.Cos(decl * Math.PI / 180.0));

            if (cosH < -1.0) cosH = -1.0;
            if (cosH > 1.0) cosH = 1.0;

            double h = Math.Acos(cosH) * 180.0 / Math.PI;

            double eot =
                9.87 * Math.Sin(2.0 * Math.PI * (n - 81) / 364.0)
                - 7.53 * Math.Cos(2.0 * Math.PI * (n - 81) / 364.0)
                - 1.5 * Math.Sin(2.0 * Math.PI * (n - 81) / 364.0);

            double solarNoon = 12.0 - lon / 15.0 + tz - eot / 60.0;

            if (rise)
            {
                return solarNoon - h / 15.0;
            }
            return solarNoon + h / 15.0;
        }
    }
}
