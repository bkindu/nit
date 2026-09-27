using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    /// <summary>DETAILED extras from VB6 eDashasAll (after Vimshottari).</summary>
    public sealed class ExtraDashaBundle
    {
        public AlternateDashaResult Ashtottari { get; set; }
        public AlternateDashaResult AshtottariArdradi { get; set; }
        public AlternateDashaResult Yogini { get; set; }
        public AlternateDashaResult Shodshottari { get; set; }
        public AlternateDashaResult Panchottari { get; set; }
        public AlternateDashaResult Shatabadika { get; set; }
        public AlternateDashaResult Chaturshityabadika { get; set; }
        public AlternateDashaResult Davisapatika { get; set; }
        public AlternateDashaResult Sashathihayani { get; set; }
        public AlternateDashaResult Sandhya { get; set; }
        public AlternateDashaResult Pachak { get; set; }

        /// <summary>Display order matching eDashasAll DETAILED chain.</summary>
        public List<AlternateDashaResult> Systems { get; set; }

        public ExtraDashaBundle()
        {
            Systems = new List<AlternateDashaResult>();
        }
    }

    public sealed class AlternateDashaResult
    {
        public string SystemName { get; set; }
        public string BirthLord { get; set; }
        public string BalanceText { get; set; }
        public List<AlternateDashaPeriod> Periods { get; set; }

        public AlternateDashaResult()
        {
            Periods = new List<AlternateDashaPeriod>();
        }
    }

    public sealed class AlternateDashaPeriod
    {
        public string MahaLord { get; set; }
        public string AntarLord { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Label { get; set; }
    }

    public sealed class VargaDeityRow
    {
        public string VargaCode { get; set; }
        public string VargaName { get; set; }
        public string PlanetName { get; set; }
        public string Deity { get; set; }
    }
}
