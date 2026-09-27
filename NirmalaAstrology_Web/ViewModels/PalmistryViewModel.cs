using System.Collections.Generic;
using System.Web.Mvc;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.ViewModels
{
    public sealed class PalmistryViewModel
    {
        public string NativeName { get; set; }
        public string[] SelectedIds { get; set; }
        public string MountCode { get; set; }
        public string LuckyCode { get; set; }
        public string RemedyCode { get; set; }

        public List<PalmistryOption> Options { get; set; }
        public IEnumerable<SelectListItem> MountOptions { get; set; }
        public IEnumerable<SelectListItem> LuckyOptions { get; set; }
        public IEnumerable<SelectListItem> RemedyOptions { get; set; }

        public PalmistryResult Result { get; set; }

        public PalmistryViewModel()
        {
            SelectedIds = new string[0];
            Options = new List<PalmistryOption>();
            MountOptions = new List<SelectListItem>();
            LuckyOptions = new List<SelectListItem>();
            RemedyOptions = new List<SelectListItem>();
        }
    }
}
