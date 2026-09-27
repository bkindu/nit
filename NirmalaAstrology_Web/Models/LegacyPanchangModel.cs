namespace NirmalaAstrology_Web.Models
{
    public sealed class LegacyPanchangModel
    {
        public string Tithi { get; set; }
        public string Nakshatra { get; set; }
        public string Yoga { get; set; }
        public string Karan { get; set; }
        public string Paksha { get; set; }
        public string VaarEnglish { get; set; }
        public string VaarHindu { get; set; }
        public int NakshatraPada { get; set; }
        public string Rashi { get; set; }
        public string SunSign { get; set; }
        public string Paya { get; set; }
        public string SaurMaas { get; set; }
        public string Ayan { get; set; }
        public string Gola { get; set; }
        public string Ritu { get; set; }
        public string Samvatsar { get; set; }
        public int VikramiSamvat { get; set; }
        public int SakaSamvat { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string Ishtakal { get; set; }
        public string Dinman { get; set; }
        public string SiderealTime { get; set; }
        public string Ayanamsa { get; set; }
        public string Gmt { get; set; }
        public string ApparentNoon { get; set; }
        public string LocalMeanTime { get; set; }
        public string DeclinationOfSun { get; set; }
        public string HoraRashiLord { get; set; }

        // Sunrise-specific Panchang values and ending-time fields.
        public string SunriseTithi { get; set; }
        public string SunriseNakshatra { get; set; }
        public string SunriseYoga { get; set; }
        public string SunriseKaran { get; set; }
        public string TithiEnding { get; set; }
        public string VaarEnding { get; set; }
        public string KaranEnding { get; set; }
        public string YogaEnding { get; set; }
        public string NakshatraEnding { get; set; }
        public string NakshatraBhukatVikala { get; set; }
        public string NakshatraBhogyaVikala { get; set; }

        // Hora Chakram fields.
        public string HoraRashi { get; set; }
        public string HoraVaran { get; set; }
        public string HoraVashya { get; set; }
        public string HoraHansak { get; set; }
        public string HoraRashiName { get; set; }
        public string HoraYoni { get; set; }
        public string HoraGana { get; set; }
        public string HoraNadi { get; set; }
        public string HoraYunja { get; set; }
        public string HoraNameLetter { get; set; }
        public string HoraVarag { get; set; }
        public string HoraPaya { get; set; }
        public string HoraSunSign { get; set; }

        // Ghat Chakram fields.
        public string GhatMaas { get; set; }
        public string GhatTithies { get; set; }
        public string GhatYog { get; set; }
        public string GhatKaran { get; set; }
        public string GhatPrahar { get; set; }
        public string GhatPlanetPositions { get; set; }
        public string GhatGrah { get; set; }
        public string GhatDhanya { get; set; }
        public string GhatJati { get; set; }
        public string GhatGender { get; set; }
        public string GhatKaruradi { get; set; }
        public string GhatCharadi { get; set; }
        public string GhatVarna { get; set; }

        // Miscellaneous astronomy fields.
        public string Velantar { get; set; }
        public string CharaPal { get; set; }
        public string Palbha { get; set; }
        public string MeanSun { get; set; }
        public string WarTimeCorrection { get; set; }
        public string TrueRahu { get; set; }

        public string GhatLagan { get; set; }
        public string GhatVaar { get; set; }
        public string GhatNakshatra { get; set; }
        public string TithiBhukat { get; set; }
        public string TithiBhogya { get; set; }
        public string YogaBhukat { get; set; }
        public string YogaBhogya { get; set; }
        public string KaranBhukat { get; set; }
        public string KaranBhogya { get; set; }
        public string NakshatraBhukat { get; set; }
        public string NakshatraBhogya { get; set; }
        public string Bhabhog { get; set; }
    }
}
