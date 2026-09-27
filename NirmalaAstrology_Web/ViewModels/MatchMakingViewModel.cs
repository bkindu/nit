using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.ViewModels
{
    public sealed class MatchMakingViewModel
    {
        public BirthDataViewModel Male { get; set; }
        public BirthDataViewModel Female { get; set; }
        public MatchMakingResult Result { get; set; }

        public MatchMakingViewModel()
        {
            Male = new BirthDataViewModel { Gender = "M", ReportChoice = "MATCH_MAKING" };
            Female = new BirthDataViewModel { Gender = "F", ReportChoice = "MATCH_MAKING", Hour = 10, Minute = 0 };
        }
    }
}
