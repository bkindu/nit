using System;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Repositories;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// MVC equivalent of VB6 DataEntry -> sChoice -> Control_Panel ganit chain.
    /// DETAILED/MEDIUM: Bhava, Dasha, Vargas, Ashtakavarga, Panchadha, Graha Bala,
    /// Special Lagnas, UpGrahas, Avasthas, Kinshukadi, Yogas.
    /// LEGACY TRACE: astronomy = aBasic_Astronomy.Basic_Astronomy_Calc via KundliCalculator.
    /// </summary>
    public sealed class KundliWorkflowService
    {
        private readonly KundliCalculator _calculator;
        private readonly KundliDetailsBuilder _detailsBuilder;
        private readonly KundliReportRepository _reportRepository;
        private readonly BhavaSpastaService _bhavaService;
        private readonly VimshottariDashaService _dashaService;
        private readonly Varga16Service _vargaService;
        private readonly AshtakavargaService _ashtakavargaService;
        private readonly PanchadhaKinshukService _panchadhaService;
        private readonly GrahaBalaService _grahaBalaService;
        private readonly SpecialLagnaUpGrahaService _specialLagnaService;
        private readonly GrahaAvasthaService _avasthaService;
        private readonly YogaDetectionService _yogaService;
        private readonly ExtraDashaService _extraDashaService;

        /// <param name="astronomyScriptPath">Ignored — kept for call-site compatibility. Calc is pure C#.</param>
        public KundliWorkflowService(string astronomyScriptPath = null)
        {
            _calculator = new KundliCalculator();
            _detailsBuilder = new KundliDetailsBuilder();
            _reportRepository = new KundliReportRepository();
            _bhavaService = new BhavaSpastaService();
            _dashaService = new VimshottariDashaService();
            _vargaService = new Varga16Service();
            _ashtakavargaService = new AshtakavargaService();
            _panchadhaService = new PanchadhaKinshukService();
            _grahaBalaService = new GrahaBalaService();
            _specialLagnaService = new SpecialLagnaUpGrahaService();
            _avasthaService = new GrahaAvasthaService();
            _yogaService = new YogaDetectionService();
            _extraDashaService = new ExtraDashaService();
        }

        public KundliWorkflowResult Generate(string choice, KundliWorkflowRequest request)
        {
            bool isOptKundli = String.Equals(choice, "OPT_KUNDLI", StringComparison.OrdinalIgnoreCase);
            bool isKundali = String.Equals(choice, "KUNDALI", StringComparison.OrdinalIgnoreCase);
            bool isDetailed = String.Equals(choice, "DETAILED", StringComparison.OrdinalIgnoreCase);
            bool isMedium = String.Equals(choice, "MEDIUM", StringComparison.OrdinalIgnoreCase);

            if (!isOptKundli && !isKundali && !isDetailed && !isMedium)
            {
                throw new NotSupportedException("Supported report choices: Opt_Kundli, KUNDALI, MEDIUM, DETAILED.");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            // Step 1: Basic astronomy (legacy Basic_Astronomy_Calc)
            KundliResult calculation = _calculator.CalculateKundli(
                request.Day, request.Month, request.Year, request.Hour, request.Minute,
                request.Longitude, request.Latitude, request.TimezoneOffset);

            // Step 2: Screen details + DB reading enrichment
            KundliDetailsModel details = _detailsBuilder.Build(calculation, request);
            try
            {
                _reportRepository.Enrich(details);
            }
            catch (Exception ex)
            {
                details.PercentageNote = details.PercentageNote + " Database report content was not loaded: " + ex.Message;
            }

            BhavaSpastaResult bhava = null;
            VimshottariDashaResult dasha = null;
            Varga16Result vargas = null;
            AshtakavargaResult ashtakavarga = null;
            GanitChainResult ganit = null;

            // Step 3: DETAILED / MEDIUM ganit chain (VB6 Control_Panel path)
            if (isDetailed || isMedium)
            {
                double[] dPlt = BuildPlanetArray(calculation);
                bhava = _bhavaService.Calculate(dPlt);
                dasha = _dashaService.Calculate(
                    calculation.Moon, request.Day, request.Month, request.Year,
                    request.Hour, request.Minute, request.TimezoneOffset);
                vargas = _vargaService.Calculate(dPlt);
                ashtakavarga = _ashtakavargaService.Calculate(dPlt);

                var panchadha = _panchadhaService.CalculatePanchadha(dPlt);
                double[] madhya = new double[13];
                foreach (var h in bhava.Houses)
                {
                    madhya[h.HouseNumber] = h.MadhyaDegrees;
                }

                DateTime date = new DateTime(request.Year, request.Month, request.Day);
                var sunRs = AstroMath.SunRiseSetForBirthDay(
                    request.Year, request.Month, request.Day,
                    request.Latitude, request.Longitude, request.TimezoneOffset);
                double sunrise = sunRs.SunRiseHours;
                double sunset = sunRs.SunSetHours;
                if (sunset < sunrise) sunset += 24.0;
                double birthHours = AstroMath.FnDg(request.Hour + request.Minute / 100.0);
                double ishtGp = (birthHours - sunrise) * 2.5;
                if (ishtGp < 0)
                {
                    ishtGp += 60;
                }
                double dinmaanGp = Math.Max(0.1, (sunset - sunrise) * 2.5);
                double localPacked = AstroMath.FnDg(request.Hour + request.Minute / 100.0);
                double astronomyJdf = AstroMath.JulDay(
                    request.Day, request.Month, request.Year, localPacked - request.TimezoneOffset);

                var grahaBala = _grahaBalaService.Calculate(dPlt, vargas.Signs, panchadha.Labels, madhya, request, calculation.AyanamsaNutated > 0 ? calculation.AyanamsaNutated : calculation.Ayanamsa);
                var special = _specialLagnaService.CalculateSpecialLagnas(dPlt, vargas.Signs, ishtGp);
                var upgrahas = _specialLagnaService.CalculateUpGrahas(
                    dPlt, request, ishtGp, dinmaanGp, calculation.AyanamsaNutated > 0 ? calculation.AyanamsaNutated : calculation.Ayanamsa, astronomyJdf, request.TimezoneOffset, request.Latitude);
                var avasthas = _avasthaService.Calculate(dPlt);
                var kinshuk = _panchadhaService.CalculateKinshukadi(vargas.Signs, panchadha.Labels);
                var yogas = _yogaService.Calculate(dPlt, grahaBala);

                ExtraDashaBundle extraDashas = null;
                if (isDetailed)
                {
                    extraDashas = _extraDashaService.Calculate(
                        calculation.Moon, calculation.Lagna, request.Day, request.Month, request.Year,
                        request.Hour, request.Minute, request.TimezoneOffset);
                }

                ganit = new GanitChainResult
                {
                    Panchadha = panchadha,
                    GrahaBala = grahaBala,
                    SpecialLagnas = special,
                    UpGrahas = upgrahas,
                    Avasthas = avasthas,
                    Kinshukadi = kinshuk,
                    Yogas = yogas,
                    ExtraDashas = extraDashas
                };
            }

            string choiceLabel;
            if (isOptKundli)
            {
                choiceLabel = "OPT_KUNDLI";
            }
            else if (isDetailed)
            {
                choiceLabel = "DETAILED";
            }
            else if (isMedium)
            {
                choiceLabel = "MEDIUM";
            }
            else
            {
                choiceLabel = "KUNDALI";
            }

            return new KundliWorkflowResult
            {
                Choice = choiceLabel,
                Calculation = calculation,
                Details = details,
                Bhava = bhava,
                Dasha = dasha,
                Vargas = vargas,
                Ashtakavarga = ashtakavarga,
                Ganit = ganit,
                DisplayMode = "SCREEN"
            };
        }

        private static double[] BuildPlanetArray(KundliResult calc)
        {
            // LEGACY TRACE: copy into dPlt(1..11) for ganit chain (same indices as zVariables)
            double[] dPlt = new double[12];
            dPlt[1] = calc.Lagna;
            dPlt[2] = calc.Sun;
            dPlt[3] = calc.Moon;
            dPlt[4] = calc.Mars;
            dPlt[5] = calc.Mercury;
            dPlt[6] = calc.Jupiter;
            dPlt[7] = calc.Venus;
            dPlt[8] = calc.Saturn;
            dPlt[9] = calc.Rahu;
            dPlt[10] = calc.Ketu;
            dPlt[11] = calc.Midheaven;
            return dPlt;
        }
    }

    public sealed class KundliWorkflowRequest
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double TimezoneOffset { get; set; }
    }

    public sealed class KundliWorkflowResult
    {
        public string Choice { get; set; }
        public string DisplayMode { get; set; }
        public KundliResult Calculation { get; set; }
        public KundliDetailsModel Details { get; set; }
        public BhavaSpastaResult Bhava { get; set; }
        public VimshottariDashaResult Dasha { get; set; }
        public Varga16Result Vargas { get; set; }
        public AshtakavargaResult Ashtakavarga { get; set; }
        public GanitChainResult Ganit { get; set; }
    }
}
