using System.Collections.Generic;

namespace NirmalaAstrology_Web.Models
{
    public sealed class VimshottariDashaResult
    {
        public string BirthLord { get; set; }
        public int BirthLordIndex { get; set; }
        public int BalanceYears { get; set; }
        public int BalanceMonths { get; set; }
        public int BalanceDays { get; set; }
        public string BalanceText { get; set; }
        public string BirthDateLabel { get; set; }
        public List<MahaDashaBlock> MahaDashas { get; set; }
        public List<AntarDashaRow> AntarDashas { get; set; }

        public VimshottariDashaResult()
        {
            MahaDashas = new List<MahaDashaBlock>();
            AntarDashas = new List<AntarDashaRow>();
        }
    }

    public sealed class MahaDashaBlock
    {
        public int Sequence { get; set; }
        public string Lord { get; set; }
        public int Years { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AgeLabel { get; set; }
    }

    public sealed class AntarDashaRow
    {
        public int MahaSequence { get; set; }
        public string MahaLord { get; set; }
        public string AntarLord { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Label { get; set; }
    }
}
