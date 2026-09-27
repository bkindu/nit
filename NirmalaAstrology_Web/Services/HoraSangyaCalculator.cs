using System;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Port of a2Functions_Antronomy.Hora_Sangya / Cal_Nak_N_Charan (Avakhoda + Ghat chakram).
    /// </summary>
    public static class HoraSangyaCalculator
    {
        private static readonly string[] Nak =
        {
            "", "ASHWINI", "BHARANI", "KRITIKA", "ROHINI", "MRIGSHIRA", "ARDRA", "PUNARVASU",
            "PUSHYA", "ASHLESHA", "MAGHA", "P.PHALGUNI", "U.PHALGUNI", "HAST", "CHITRA", "SWATI",
            "VISHAKHA", "ANURADHA", "JYESTHA", "MOOLA", "P.SHADA", "U.SHADA", "SHRAWAN",
            "DHANISTHA", "SHATBHISHA", "P.BHADRAPAD", "U.BHADRAPAD", "REVATI"
        };

        private static readonly string[] Yoni =
        {
            "", "ASHAV", "GAJ", "MESH", "SARAP", "SARAP", "SHWAN", "MARJAR", "MESH", "MARJAR",
            "MUSHAK", "MUSHAK", "GAU", "MAHIS", "VYAGHAR", "MAHIS", "VYAGHAR", "MRIG", "MRIG",
            "SHWAN", "VANAR", "NAKUL", "VANAR", "SIMHA", "ASHAV", "SIMHA", "GAU", "GAJ"
        };

        private static readonly string[] Gana =
        {
            "", "DEV", "NAR", "RAKSHAS", "NAR", "DEV", "NAR", "DEV", "DEV", "RAKSHAS",
            "RAKSHAS", "NAR", "DEV", "RAKSHAS", "DEV", "RAKSHAS", "DEV", "RAKSHAS", "DEV",
            "RAKSHAS", "RAKSHAS", "NAR", "DEV", "RAKSHAS", "RAKSHAS", "NAR", "NAR", "RAKSHAS"
        };

        private static readonly string[] Nadi =
        {
            "", "ADI", "MADHYA", "ANTYA", "ANTYA", "MADHYA", "ADI", "ADI", "MADHYA", "ANTYA",
            "ANTYA", "MADHYA", "ADI", "ADI", "MADHYA", "ANTYA", "ANTYA", "ANTYA", "ADI",
            "ADI", "MADHYA", "ANTYA", "ANTYA", "MADHYA", "ADI", "ADI", "MADHYA", "ANTYA"
        };

        private static readonly string[] Yunja =
        {
            "", "POORVA", "POORVA", "POORVA", "POORVA", "POORVA", "MADHYA", "MADHYA", "MADHYA", "MADHYA",
            "MADHYA", "MADHYA", "MADHYA", "MADHYA", "MADHYA", "MADHYA", "MADHYA", "ANTYA", "ANTYA",
            "ANTYA", "ANTYA", "ANTYA", "ANTYA", "ANTYA", "ANTYA", "ANTYA", "ANTYA", "POORVA"
        };

        // VB6 Choose(iNak108Num, ...) — 108 name-start syllables
        private static readonly string[] NameLetters =
        {
            "",
            "CHU", "CHE", "CHO", "LA", "LEE", "LU", "LE", "LO", "AA", "EE", "OO", "AEY",
            "O", "VA", "VEE", "VU", "VEY", "VO", "KAA", "KEE", "KOO", "GHA", "ANJAY", "CHH",
            "KEY", "KO", "HAA", "HEE", "HOO", "HEY", "HO", "DAA", "DEE", "DOO", "DEY", "DO",
            "MAA", "MEE", "MOO", "MEY", "MO", "TAA", "TEE", "TOO", "TEY", "TO", "PAA", "PEE",
            "PU", "SHA", "ANA", "THH", "PEY", "PO", "RA", "REE", "ROO", "REY", "RO", "TA(TA)",
            "TEE(T)", "TOO", "TEY", "TO", "NAA", "NEE", "NU", "NEY", "NO", "YAA", "YEE", "YOO",
            "YEY", "YO", "BHA", "BHEE", "BHOO", "DHA", "FA", "DHH", "BHEY", "BHO", "JAA", "JEE",
            "KHEE", "KHOO", "KHEY", "KHOY", "GAA", "GEE", "GOO", "GEY", "GO", "SAA", "SEE", "SOO",
            "SEY", "SO", "DAA(DA)", "DEE(D)", "DOO", "THA", "JHAA", "ANJA", "DEY(D)", "DO(D)", "CHAA", "CHEE"
        };

        // VB6 Choose(iNak108Num, ...) — 108 Varag animals
        private static readonly string[] Vargas =
        {
            "",
            "SIMHA", "SIMHA", "SIMHA", "MRIG", "MRIG", "MRIG", "MRIG", "MRIG",
            "GARUDA", "GARUDA", "GARUDA", "GARUDA", "GARUDA", "MRIG", "MRIG", "MRIG", "MRIG", "MRIG",
            "MARJAR", "MARJAR", "MARJAR", "MARJAR", "MARJAR", "SIMHA", "MARJAR", "MARJAR",
            "MENDHA", "MENDHA", "MENDHA", "MENDHA", "MENDHA", "SWAN", "SWAN", "SWAN", "SWAN", "SWAN",
            "MUSAK", "MUSAK", "MUSAK", "MUSAK", "MUSAK", "SWAN", "SWAN", "SWAN", "SWAN", "SWAN",
            "MUSAK", "MUSAK", "MUSAK", "MENDHA", "SWAN", "SWAN", "MUSAK", "MUSAK",
            "MRIG", "MRIG", "MRIG", "MRIG", "MRIG", "SARPA", "SARPA", "SARPA", "SARPA", "SARPA",
            "SARPA", "SARPA", "SARPA", "SARPA", "SARPA", "MRIG", "MRIG", "MRIG", "MRIG", "MRIG",
            "MUSAK", "MUSAK", "MUSAK", "SARPA", "MUSAK", "SWAN", "MUSAK", "MUSAK",
            "SIMHA", "SIMHA", "MARJAR", "MARJAR", "MARJAR", "MARJAR", "MARJAR", "MARJAR", "MARJAR", "MARJAR", "MARJAR",
            "MENDHA", "MENDHA", "MENDHA", "MENDHA", "MENDHA", "SARPA", "SARPA", "SARPA", "SARPA",
            "SIMHA", "SIMHA", "SARPA", "SARPA", "SIMHA", "SIMHA"
        };

        // Per moon-rashi (1..12): Rashi, Lord, Varna, Vashya, Hansak, RashiNameFrom,
        // GhatMas, Tit, Var, Nak, Yog, Krn, Prhr, Lgn
        private static readonly string[][] RashiRows =
        {
            null,
            new[] { "MESHA", "MANGAL", "KSATRIYA", "CHATUSPAD", "AGANI", "L, A", "KARTIKA", "1,6,11", "RAVI", "MAGHA", "VISHKAMBHA", "BAVA", "FIRST", "MESHA" },
            new[] { "VRISHABHA", "SUKRA", "VAISHYA", "CHATUSPAD", "PRITHAVI", "V,O,I,U", "MARGSIRSA", "5,10,15", "SANI", "HASTA", "SUKLA", "SAKUNI", "FOURTH", "MITHUNA" },
            new[] { "MITHUNA", "BUDHA", "SOODRA", "DVIPADA", "VAYU", "K, HA", "ASAD", "2,7,12", "SOM", "SVATI", "PARIGHA", "KAULVA", "THIRD", "KANYA" },
            new[] { "KARKA", "CHANDRA", "VIPRA", "JALACHARA", "JALA", "D,HI,HE", "PAUSA", "2,7,12", "BUDHA", "ANURADHA", "VYAGHAT", "NAAG", "FIRST", "MAKAR" },
            new[] { "SIMHA", "SURYA", "KSHATRIYA", "CHATUSPAD", "AGANI", "M, T", "JYESTHA", "3,8,13", "SANI", "MULA", "DHARITI", "BAVA", "FIRST", "MESHA" },
            new[] { "KANYA", "BUDHA", "VAISHYA", "DVIPADA", "PRITHAVI", "P, TE", "BHADRAPAD", "5,10,15", "SHANI", "SHRAWAN", "SUKLA", "KAULAV", "FIRST", "SIMHA" },
            new[] { "TULA", "SUKRA", "SOODRA", "DVIPADA", "VAYU", "R, TTA", "MAGHA", "4,9,14", "GURU", "SATBHISA", "SUKLA", "TAITILA", "FOURTH", "MEENA" },
            new[] { "VRISCHIKA", "MANGALA", "VIPRA", "KEETA", "JALA", "N, Y", "ASVINA", "1,6,11", "SUKRA", "REVATI", "VYATIPAT", "GARA", "FIRST", "MITHUNA" },
            new[] { "DHANU", "GURU", "KSHATRIYA", "DVIPADA", "AGANI", "BH,DH,YO,F", "SRAVANA", "3,8,13", "SUKRA", "BHARANI", "VAJRA", "TAITILA", "FIRST", "SIMHA" },
            new[] { "MAKAR", "SANI", "VAISHYA", "JALCHARA", "PRITHAVI", "J,GA,GI,,BHO", "VAISAKHA", "4,9,14", "MANGALA", "ROHINI", "VAIDHARITI", "SAKUNI", "FOURTH", "VRISCHIK" },
            new[] { "KUMBHA", "SANI", "SOODRA", "MANAVA", "VAYU", "S,GU,GE,GO", "CHAITRA", "3,8,13", "GURU", "GURU", "GANDA", "KINSTUGHAN", "THIRD", "MESHA" },
            new[] { "MEENA", "GURU", "VIPRA", "JALACHARA", "JALA", "CHA,CHI,D", "PHALGUNA", "5,10,15", "SUKRA", "ASHLESA", "VAJRA", "CHATUSPAD", "FOURTH", "SIMHA" }
        };

        // Prn_HoroPanchang Select Case iRsiNum — planet ghat rashi numbers + attributes (moon rashi keyed)
        private static readonly string[][] GhatPlanetRows =
        {
            null,
            // Sur,Cha,Man,Bud,Gur,Suk,Sha,Rah,Ket, Grah, Dhanya, Jati, Gender, Karuradi, Charadi, Varna
            new[] { "04", "01", "05", "02", "06", "07", "03", "08", "08", "MANGAL", "PI.", "KSHATRIYA", "NAR", "KARUR", "CHAR", "RAKAT" },
            new[] { "08", "05", "09", "06", "10", "10", "07", "07", "07", "SHUKAR", "VA.", "VAISYA", "ISTRI", "SAUMYA", "SATHIR", "SHWET" },
            new[] { "12", "09", "01", "10", "02", "03", "11", "06", "10", "BUDH", "SA.", "SHOODRA", "NAR", "KARUR", "DVISWABHAV", "HARIT" },
            new[] { "07", "02", "06", "03", "07", "08", "04", "04", "01", "CHANDRA", "KA.", "VIPRA", "ISTRI", "SAUMYA", "CHAR", "PANDU" },
            new[] { "09", "06", "10", "07", "11", "12", "08", "05", "11", "SURYA", "PI.", "KSHATRIYA", "NAR", "KARUR", "SATHIR", "DHOOMRA" },
            new[] { "01", "10", "02", "11", "03", "04", "12", "10", "05", "BUDH", "VA.", "VAISYA", "ISTRI", "SAUMYA", "DVISWABHAV", "CHITKABRA" },
            new[] { "06", "03", "07", "04", "08", "09", "05", "12", "06", "SHUKAR", "SA.", "SHOODRA", "NAR", "KARUR", "CHAR", "KRISHNA" },
            new[] { "10", "07", "01", "08", "12", "01", "09", "01", "04", "MANGAL", "KA.", "VIPRA", "ISTRI", "SAUMYA", "SATHIR", "LAK." },
            new[] { "07", "04", "08", "05", "09", "10", "06", "02", "02", "GURU", "PI.", "KSHATRIYA", "NAR", "KARUR", "DVISWABHAV", "PEET" },
            new[] { "11", "08", "12", "09", "01", "02", "10", "09", "03", "SHANI", "VA.", "VAISYA", "ISTRI", "SAUMYA", "CHAR", "KRISHNA" },
            new[] { "02", "11", "03", "12", "04", "05", "01", "11", "12", "SHANI", "SA.", "SHOODRA", "NAR", "KARUR", "SATHIR", "DHOOMRA" },
            new[] { "03", "12", "04", "01", "05", "06", "02", "03", "09", "GURU", "KA.", "VIPRA", "ISTRI", "SAUMYA", "DVISWABHAV", "SHAWET" }
        };

        public static HoraSangyaModel FromMoon(double moonLongitude)
        {
            var x = moonLongitude % 360.0;
            if (x < 0) x += 360.0;
            var nakDecimal = 60.0 * x / 800.0;
            var nak = Math.Min(27, Math.Max(1, (int)Math.Floor(nakDecimal) + 1));
            var pada = Math.Min(4, Math.Max(1, (int)Math.Floor((nakDecimal - Math.Floor(nakDecimal)) * 4.0) + 1));
            var rashi = Math.Min(12, Math.Max(1, (int)Math.Floor(x / 30.0) + 1));
            var nak108 = Math.Min(108, Math.Max(1, (nak - 1) * 4 + pada));
            var row = RashiRows[rashi];
            var ghatPlanets = GhatPlanetRows[rashi];

            return new HoraSangyaModel
            {
                NakshatraNumber = nak,
                Pada = pada,
                RashiNumber = rashi,
                NakshatraName = Nak[nak],
                Yoni = Yoni[nak],
                Gana = Gana[nak],
                Nadi = Nadi[nak],
                Yunja = Yunja[nak],
                RashiName = row[0],
                RashiLord = row[1],
                Varna = row[2],
                Vashya = row[3],
                Hansak = row[4],
                RashiNameFrom = row[5],
                NameLetter = NameLetters[nak108],
                VargaName = Vargas[nak108],
                GhatMaas = row[6],
                GhatTithies = row[7],
                GhatVaar = row[8],
                GhatNakshatra = row[9],
                GhatYog = row[10],
                GhatKaran = row[11],
                GhatPrahar = row[12],
                GhatLagan = row[13],
                GhatPlanetPositions = string.Format(
                    "Surya={0} Chander={1} Mangal={2} Budha={3} Guru={4} Sukar={5} Shani={6} Rahu={7} Ketu={8}",
                    ghatPlanets[0], ghatPlanets[1], ghatPlanets[2], ghatPlanets[3], ghatPlanets[4],
                    ghatPlanets[5], ghatPlanets[6], ghatPlanets[7], ghatPlanets[8]),
                GhatGrah = ghatPlanets[9],
                GhatDhanya = ghatPlanets[10],
                GhatJati = ghatPlanets[11],
                GhatGender = ghatPlanets[12],
                GhatKaruradi = ghatPlanets[13],
                GhatCharadi = ghatPlanets[14],
                GhatVarna = ghatPlanets[15]
            };
        }
    }
}
