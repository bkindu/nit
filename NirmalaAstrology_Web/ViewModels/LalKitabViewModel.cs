using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.ViewModels
{
    public sealed class LalKitabViewModel
    {
        public bool HasBirthSession { get; set; }
        public string BirthSummary { get; set; }
        public string NativeName { get; set; }
        public int AgeYear { get; set; }
        /// <summary>Janam or Varsh — matches Opt_LalKitabJanam / Opt_LalKitabVarsh.</summary>
        public string RemedyMode { get; set; }
        public LalKitabResult Result { get; set; }

        public LalKitabViewModel()
        {
            RemedyMode = "Janam";
            AgeYear = 0;
        }
    }
}
