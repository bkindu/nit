using System;

namespace NirmalaAstrology_Web.Models
{
    public sealed class HoraSangyaModel
    {
        public int NakshatraNumber { get; set; }
        public int Pada { get; set; }
        public int RashiNumber { get; set; }
        public string NakshatraName { get; set; }
        public string Yoni { get; set; }
        public string Gana { get; set; }
        public string Nadi { get; set; }
        public string Yunja { get; set; }
        public string RashiName { get; set; }
        public string RashiLord { get; set; }
        public string Varna { get; set; }
        public string Vashya { get; set; }
        public string Hansak { get; set; }
        public string RashiNameFrom { get; set; }
        public string NameLetter { get; set; }
        public string VargaName { get; set; }

        public string GhatMaas { get; set; }
        public string GhatTithies { get; set; }
        public string GhatVaar { get; set; }
        public string GhatNakshatra { get; set; }
        public string GhatYog { get; set; }
        public string GhatKaran { get; set; }
        public string GhatPrahar { get; set; }
        public string GhatLagan { get; set; }
        public string GhatPlanetPositions { get; set; }
        public string GhatGrah { get; set; }
        public string GhatDhanya { get; set; }
        public string GhatJati { get; set; }
        public string GhatGender { get; set; }
        public string GhatKaruradi { get; set; }
        public string GhatCharadi { get; set; }
        public string GhatVarna { get; set; }
    }

    public sealed class MatchPersonInput
    {
        public string Name { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Timezone { get; set; }
        public string Place { get; set; }
    }

    public sealed class MatchPersonProfile
    {
        public string Name { get; set; }
        public string BirthDateText { get; set; }
        public string BirthTimeText { get; set; }
        public string Place { get; set; }
        public double MoonLongitude { get; set; }
        public int MoonRashi { get; set; }
        public int LagnaRashi { get; set; }
        public string LagnaRashiName { get; set; }
        public string LagnaRakv { get; set; }
        public string MoonRakv { get; set; }
        public string AyanamsaRak { get; set; }
        public int MarsHouse { get; set; }
        public bool IsManglik { get; set; }
        public HoraSangyaModel Hora { get; set; }
    }

    public sealed class KootScore
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public double Normal { get; set; }
        public double Modified { get; set; }
        public bool HasDosha { get; set; }
        public string Note { get; set; }
    }

    public sealed class MatchMakingResult
    {
        public MatchPersonProfile Male { get; set; }
        public MatchPersonProfile Female { get; set; }
        public string GunaString { get; set; }
        public int MatrixRow { get; set; }
        public int MatrixColumn { get; set; }
        public string BhakootName { get; set; }
        public string MaleTara { get; set; }
        public string FemaleTara { get; set; }
        public string RashiMaitri { get; set; }
        public string NavamsaMaitri { get; set; }
        public KootScore[] Koots { get; set; }
        public double TotalNormal { get; set; }
        public double TotalModified { get; set; }
        public string Verdict { get; set; }
        public string ManglikNote { get; set; }
        public string[] Cancellations { get; set; }
    }
}
