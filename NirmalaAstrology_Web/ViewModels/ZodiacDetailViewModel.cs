using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.ViewModels
{
    /// <summary>
    /// ViewModel for the Zodiac Detail page.
    /// </summary>
    public class ZodiacDetailViewModel
    {
        /// <summary>
        /// The zodiac sign details.
        /// </summary>
        public ZodiacModel Zodiac { get; set; }

        /// <summary>
        /// List of all zodiac signs for sidebar / navigation.
        /// </summary>
        public System.Collections.Generic.List<ZodiacModel> AllZodiacs { get; set; }

        /// <summary>
        /// Compatible signs parsed as a list.
        /// </summary>
        public string[] CompatibleSignsList
        {
            get
            {
                if (string.IsNullOrEmpty(Zodiac?.CompatibleSigns))
                    return new string[0];
                return Zodiac.CompatibleSigns.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}