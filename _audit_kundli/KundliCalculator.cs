using System;
using System.Collections.Generic;

namespace NirmalaAstrology_Web.Services
{
    // =====================================================================
    // PlanetPosition: Holds the position details for a single celestial body
    // =====================================================================
    public class PlanetPosition
    {
        public string Name { get; set; }
        public int Rashi { get; set; }           // Sign number 1-12 (Aries=1, Taurus=2, ... Pisces=12)
        public double DegreesInRashi { get; set; } // Degrees within the sign (0-30)
        public double TotalDegrees { get; set; }   // Total sidereal longitude (0-360)
        public int House { get; set; }               // House placement (1-12)
        public bool IsRetrograde { get; set; }      // True if planet is retrograde (Vakri)

        private static readonly string[] RashiNames = {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };

        // View-compatible properties
        public string PlanetName { get { return Name; } }
        public string Retrograde { get { return IsRetrograde ? "R" : null; } }
        public string RashiName { get { return Rashi > 0 && Rashi <= 12 ? RashiNames[Rashi] : "?"; } }
        public int Degrees { get { return (int)DegreesInRashi; } }
        public int Minutes { get { return (int)((DegreesInRashi - (int)DegreesInRashi) * 60); } }
    }

    // =====================================================================
    // HouseData: Contains the sign (rashi) and planets occupying a house
    // =====================================================================
    public class HouseData
    {
        public int HouseNumber { get; set; } // 1-12
        public int Rashi { get; set; }       // Sign number 1-12
        public List<string> Planets { get; set; } // Abbreviations of planets in this house

        private static readonly string[] RashiNames = {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };

        public string RashiName
        {
            get { return Rashi > 0 && Rashi <= 12 ? RashiNames[Rashi] : "?"; }
        }
    }

    // =====================================================================
    // KundliResult: Complete result of a Kundli calculation
    // =====================================================================
    public class KundliResult
    {
        // Planet positions
        public PlanetPosition Lagna { get; set; }     // Ascendant
        public PlanetPosition Sun { get; set; }
        public PlanetPosition Moon { get; set; }
        public PlanetPosition Mars { get; set; }
        public PlanetPosition Mercury { get; set; }
        public PlanetPosition Jupiter { get; set; }
        public PlanetPosition Venus { get; set; }
        public PlanetPosition Saturn { get; set; }
        public PlanetPosition Rahu { get; set; }
        public PlanetPosition Ketu { get; set; }

        // Additional data
        public double Ayanamsa { get; set; }       // Ayanamsa value in degrees
        public double SiderealTime { get; set; }    // Sidereal time in degrees (0-360)
        public double SiderealTimeHMS { get; set; } // Sidereal time in HH.MM format
        public PlanetPosition MC { get; set; }     // Midheaven / Dasham Bhava

        // House-wise data
        public List<HouseData> Houses { get; set; }

        // Raw FNQ-format values (for debugging / compatibility)
        public Dictionary<string, double> RawPositions { get; set; }

        // View-compatible properties
        public string AscendantRashiName
        {
            get { return Lagna != null ? RashiNames[Lagna.Rashi > 0 && Lagna.Rashi <= 12 ? Lagna.Rashi : 1] : null; }
        }

        public List<PlanetPosition> PlanetPositions
        {
            get
            {
                return new List<PlanetPosition> { Sun, Moon, Mars, Mercury, Jupiter, Venus, Saturn, Rahu, Ketu };
            }
        }

        private static readonly string[] RashiNames = {
            "", "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo",
            "Libra", "Scorpio", "Sagittarius", "Capricorn", "Aquarius", "Pisces"
        };
    }

    // =====================================================================
    // KundliCalculator: Self-contained service for Vedic Kundli calculations
    //
    // Converted from JavaScript source (Indu.html). All astronomical constants
    // and calculation logic preserved exactly from the original.
    //
    // Usage:
    //   var calc = new KundliCalculator();
    //   var result = calc.CalculateKundli(8, 2, 1981, 7, 47, 30.19, 76.24);
    // =====================================================================
    public class KundliCalculator
    {
        // -----------------------------------------------------------------
        // Astronomical / Mathematical Constants
        // -----------------------------------------------------------------
        private const double RAD_TO_DEG = 57.2957795;           // 180 / PI
        private const double DEG_TO_RAD = 0.0174532925199433;  // PI / 180
        private const double TWO_PI = 6.2831853072;             // 2 * PI
        private const double TPI = 6.2831853072;                // 2 * PI (alias used in perturbation code)
        private const double PLAN = 0.01720209895;              // Kepler's planetary mean motion constant

        // Planet abbreviations used in house data (index aligns with dPlt: 2=Sun..10=Ketu)
        private static readonly string[] PlanetAbbreviations = {
            "",     // [0] unused
            "LAG",  // [1] Lagna
            "SUR",  // [2] Sun
            "CHA",  // [3] Moon
            "MGL",  // [4] Mars
            "BUD",  // [5] Mercury
            "GUR",  // [6] Jupiter
            "SUK",  // [7] Venus
            "SAN",  // [8] Saturn
            "RAH",  // [9] Rahu
            "KET"   // [10] Ketu
        };

        // -----------------------------------------------------------------
        // Helper: FNDG - Convert HH.MM format to decimal degrees
        // e.g., 7.47 -> 7 + 47/60 = 7.78333...
        // -----------------------------------------------------------------
        private static double FNDG(double dX)
        {
            return (250.0 * dX + 0.00000005
                    - 60.0 * (int)(dX + 0.00000005)
                    - (int)(100.0 * dX + 0.00000005)) / 90.0;
        }

        // -----------------------------------------------------------------
        // Helper: FNDM - Convert decimal degrees back to HH.MM format
        // (Inverse of FNDG)
        // -----------------------------------------------------------------
        private static double FNDM(double dX)
        {
            return (90.0 * dX + 0.00000005
                    + 100.0 * (int)(dX + 0.00000005)
                    + (int)(60.0 * dX + 0.00000005)) / 250.0;
        }

        // -----------------------------------------------------------------
        // Helper: FNASN - Arc Sine in degrees
        // Returns degrees for arcsin(x) where |x| <= 1
        // -----------------------------------------------------------------
        private static double FNASN(double dX)
        {
            if (dX > 1.0) dX = 1.0;
            else if (dX < -1.0) dX = -1.0;
            return RAD_TO_DEG * Math.Atan(dX / Math.Sqrt(1.0 - dX * dX));
        }

        // -----------------------------------------------------------------
        // Helper: FNRAD - Convert degrees to radians
        // -----------------------------------------------------------------
        private static double FNRAD(double dX)
        {
            return dX * DEG_TO_RAD;
        }

        // -----------------------------------------------------------------
        // Helper: FNAT - Arctangent, result in degrees
        // -----------------------------------------------------------------
        private static double FNAT(double dX)
        {
            return Math.Atan(dX) * RAD_TO_DEG;
        }

        // -----------------------------------------------------------------
        // Helper: FNTN - Tangent (input in degrees)
        // -----------------------------------------------------------------
        private static double FNTN(double dX)
        {
            return Math.Tan(dX * DEG_TO_RAD);
        }

        // -----------------------------------------------------------------
        // Helper: FNQ - Convert total degrees to sign.degrees encoded format
        // Integer part = 0-based rashi, fractional part encodes degrees
        // -----------------------------------------------------------------
        private static double FNQ(double dX)
        {
            double offset = dX + 0.00003;
            return (90.0 * offset
                    + 17500.0 * (int)(offset / 30.0)
                    + 100.0 * (int)(offset)
                    + (int)(offset * 60.0)) / 25000.0;
        }

        // -----------------------------------------------------------------
        // Helper: FNUNW - Unwrap angle to range [0, 2*PI)
        // -----------------------------------------------------------------
        private static double FNUNW(double dX)
        {
            return dX - (int)(dX / TWO_PI) * TWO_PI;
        }

        // -----------------------------------------------------------------
        // Helper: FNSN - Sine (input in degrees)
        // -----------------------------------------------------------------
        private static double FNSN(double dX)
        {
            return Math.Sin(dX * DEG_TO_RAD);
        }

        // -----------------------------------------------------------------
        // Helper: FNCS - Cosine (input in degrees)
        // -----------------------------------------------------------------
        private static double FNCS(double dX)
        {
            return Math.Cos(dX * DEG_TO_RAD);
        }

        // -----------------------------------------------------------------
        // Helper: FNR - Normalize value to range [0, dCY)
        // -----------------------------------------------------------------
        private static double FNR(double dX, double dCY)
        {
            return FNFR(FNFR(dX / dCY) + 2.0) * dCY;
        }

        // -----------------------------------------------------------------
        // Helper: FNTGR - Truncate to integer (equivalent to parseInt)
        // -----------------------------------------------------------------
        private static int FNTGR(double dX)
        {
            return (int)(dX + 0.0000005);
        }

        // -----------------------------------------------------------------
        // Helper: FNFR - Fractional part (always non-negative)
        // -----------------------------------------------------------------
        private static double FNFR(double dX)
        {
            return dX - Math.Floor(dX);
        }

        // -----------------------------------------------------------------
        // Helper: julday - Compute Julian Day Number from calendar date and UT hour
        // -----------------------------------------------------------------
        private static double julday(int day, int month, int year, double hour)
        {
            if (month < 3)
            {
                year -= 1;
                month += 12;
            }
            int leep = (int)Math.Floor((double)year / 100.0);
            leep = 2 - leep + (int)Math.Floor((double)leep / 4.0);

            double jd = Math.Floor(365.2501 * year)
                        + Math.Floor((month + 1) * 30.6001)
                        - 694025.0 + leep + day + hour / 24.0;
            return jd;
        }

        // -----------------------------------------------------------------
        // getmoon - Calculate Moon's Sayana longitude from Julian Day
        // Uses Chao's perturbation method (1978) with Brown's theory terms
        // -----------------------------------------------------------------
        private double GetMoon(double dJDF)
        {
            double TQQ = (dJDF - 0.5) / 36525.0;
            double DJDQQ = dJDF - 0.5;
            double T2QQ = TQQ * TQQ;

            double M1QQ = 27.32158213;
            double M2QQ = 365.2596407;
            double M3QQ = 27.55455094;
            double M4QQ = 29.53058868;
            double M5QQ = 27.21222039;
            double M6QQ = 6798.363307;

            M1QQ = DJDQQ / M1QQ;
            M2QQ = DJDQQ / M2QQ;
            M3QQ = DJDQQ / M3QQ;
            M4QQ = DJDQQ / M4QQ;
            M5QQ = DJDQQ / M5QQ;
            M6QQ = DJDQQ / M6QQ;

            M1QQ = 360.0 * (M1QQ - (int)M1QQ);
            M2QQ = 360.0 * (M2QQ - (int)M2QQ);
            M3QQ = 360.0 * (M3QQ - (int)M3QQ);
            M4QQ = 360.0 * (M4QQ - (int)M4QQ);
            M5QQ = 360.0 * (M5QQ - (int)M5QQ);
            M6QQ = 360.0 * (M6QQ - (int)M6QQ);

            double LDQQ = 270.434164 + M1QQ - (0.001133 - 0.0000019 * TQQ) * T2QQ;
            double MSQQ = 358.475833 + M2QQ - (0.00015 + 0.0000033 * TQQ) * T2QQ;
            double MDQQ = 296.104608 + M3QQ + (0.009192 + 0.0000144 * TQQ) * T2QQ;
            double DEQQ = 350.737486 + M4QQ - (0.001436 - 0.0000019 * TQQ) * T2QQ;
            double FQQ = 11.250889 + M5QQ - (0.003211 + 0.0000003 * TQQ) * T2QQ;
            double NQQ = 258.193275 - M6QQ + (0.002078 + 0.000022 * TQQ) * T2QQ;

            double AQQ = FNRAD(51.2 + 20.2 * TQQ);
            double SAQQ = Math.Sin(AQQ);
            double SNQQ = Math.Sin(FNRAD(NQQ));
            double BQQ = 346.56 + (132.87 - 0.0091731 * TQQ) * TQQ;
            double SBQQ = 0.003964 * Math.Sin(FNRAD(BQQ));
            double CQQ = FNRAD(NQQ + 275.05 - 2.3 * TQQ);
            double SCQQ = Math.Sin(CQQ);

            LDQQ = LDQQ + 0.000233 * SAQQ + SBQQ + 0.001964 * SNQQ;
            MSQQ = MSQQ - 0.001778 * SAQQ;
            MDQQ = MDQQ + 0.000817 * SAQQ + SBQQ + 0.002541 * SNQQ;
            FQQ = FQQ + SBQQ - 0.024691 * SNQQ - 0.004328 * SCQQ;
            DEQQ = DEQQ + 0.002011 * SAQQ + SBQQ + 0.001964 * SNQQ;

            double EQQ = 1.0 - (0.002495 + 0.00000752 * TQQ) * TQQ;
            double E2QQ = EQQ * EQQ;

            LDQQ = FNRAD(LDQQ);
            MSQQ = FNRAD(MSQQ);
            NQQ = FNRAD(NQQ);
            DEQQ = FNRAD(DEQQ);
            FQQ = FNRAD(FQQ);
            MDQQ = FNRAD(MDQQ);

            // Main longitude terms (Brown's lunar theory)
            double LQQ = 6.28875 * Math.Sin(MDQQ)
                         + 1.274018 * Math.Sin(2.0 * DEQQ - MDQQ)
                         + 0.658309 * Math.Sin(2.0 * DEQQ)
                         + 0.213616 * Math.Sin(2.0 * MDQQ)
                         - EQQ * 0.185596 * Math.Sin(MSQQ)
                         - 0.114336 * Math.Sin(2.0 * FQQ)
                         + 0.058793 * Math.Sin(2.0 * (DEQQ - MDQQ))
                         + 0.057212 * EQQ * Math.Sin(2.0 * DEQQ - MSQQ - MDQQ)
                         + 0.05332 * Math.Sin(2.0 * DEQQ + MDQQ)
                         + 0.045874 * EQQ * Math.Sin(2.0 * DEQQ - MSQQ)
                         + 0.041024 * EQQ * Math.Sin(MDQQ - MSQQ)
                         - 0.034718 * Math.Sin(DEQQ)
                         - EQQ * 0.030465 * Math.Sin(MSQQ + MDQQ)
                         + 0.015326 * Math.Sin(2.0 * (DEQQ - FQQ))
                         - 0.012528 * Math.Sin(2.0 * FQQ + MDQQ)
                         - 0.01098 * Math.Sin(FQQ - MDQQ)
                         + 0.010674 * Math.Sin(4.0 * DEQQ - MDQQ)
                         + 0.010034 * Math.Sin(3.0 * MDQQ)
                         + 0.008548 * Math.Sin(4.0 * DEQQ - 2.0 * MDQQ)
                         - EQQ * 0.00791 * Math.Sin(MSQQ - MDQQ + 2.0 * DEQQ)
                         - EQQ * 0.006783 * Math.Sin(2.0 * DEQQ + MSQQ)
                         + 0.005162 * Math.Sin(MDQQ - DEQQ)
                         + EQQ * 0.005 * Math.Sin(MSQQ + DEQQ)
                         + 0.003862 * Math.Sin(4.0 * DEQQ)
                         + EQQ * 0.004049 * Math.Sin(MDQQ - MSQQ + 2.0 * DEQQ)
                         + 0.003996 * Math.Sin(2.0 * (MDQQ + DEQQ))
                         + 0.003665 * Math.Sin(2.0 * DEQQ - 3.0 * MDQQ)
                         + EQQ * 0.002695 * Math.Sin(2.0 * MDQQ - MSQQ)
                         + 0.002602 * Math.Sin(MDQQ - 2.0 * (FQQ + DEQQ))
                         + EQQ * 0.002396 * Math.Sin(2.0 * (DEQQ - MDQQ) - MSQQ)
                         - 0.002349 * Math.Sin(MDQQ + DEQQ)
                         + E2QQ * 0.002249 * Math.Sin(2.0 * (DEQQ - MSQQ))
                         - EQQ * 0.002125 * Math.Sin(2.0 * MDQQ + MSQQ)
                         - E2QQ * 0.002079 * Math.Sin(2.0 * MSQQ)
                         + E2QQ * 0.002059 * Math.Sin(2.0 * (DEQQ - MSQQ) - MDQQ)
                         - 0.001773 * Math.Sin(MDQQ + 2.0 * (DEQQ - FQQ))
                         - 0.001595 * Math.Sin(2.0 * (FQQ + DEQQ))
                         + EQQ * 0.00122 * Math.Sin(4.0 * DEQQ - MSQQ - MDQQ)
                         - 0.00111 * Math.Sin(2.0 * (MDQQ + FQQ))
                         + 0.000892 * Math.Sin(MDQQ - 3.0 * DEQQ)
                         - EQQ * 0.000811 * Math.Sin(MSQQ + MDQQ + 2.0 * DEQQ)
                         + EQQ * 0.000761 * Math.Sin(4.0 * DEQQ - MSQQ - 2.0 * MDQQ)
                         + E2QQ * 0.000704 * Math.Sin(MDQQ - 2.0 * (MSQQ + DEQQ))
                         + EQQ * 0.000693 * Math.Sin(MSQQ - 2.0 * (MDQQ - DEQQ))
                         + EQQ * 0.000598 * Math.Sin(2.0 * (DEQQ - FQQ) - MSQQ)
                         + 0.00055 * Math.Sin(MDQQ + 4.0 * DEQQ)
                         + 0.000538 * Math.Sin(4.0 * MDQQ)
                         + EQQ * 0.000521 * Math.Sin(4.0 * DEQQ - MSQQ)
                         + 0.000486 * Math.Sin(2.0 * MDQQ - DEQQ)
                         + E2QQ * 0.000717 * Math.Sin(MDQQ - 2.0 * MSQQ);

            double LAMQQ = LDQQ + FNRAD(LQQ);
            double TPIQQ = TWO_PI;

            // Normalize LAMQQ to [0, 2*PI)
            while (LAMQQ < 0) LAMQQ += TPIQQ;
            while (LAMQQ > TPIQQ) LAMQQ -= TPIQQ;

            double moonLong = FNR(LAMQQ * 360.0 / TPIQQ + _abbqq + _nutqq, 360.0);
            return moonLong;
        }

        // Instance fields to pass ABBQQ_d and NUTQQ_d to GetMoon
        private double _abbqq;
        private double _nutqq;

        // =================================================================
        // MAIN PUBLIC METHOD
        // CalculateKundli - Computes complete Kundli data
        //
        // Parameters:
        //   day      : Birth day (1-31)
        //   month    : Birth month (1-12)
        //   year     : Birth year (e.g., 1981)
        //   hour     : Birth hour in IST (0-23)
        //   minute   : Birth minute (0-59)
        //   latitude : Geographic latitude in decimal degrees (e.g., 30.19)
        //   longitude: Geographic longitude in decimal degrees (e.g., 76.24)
        //
        // Returns: KundliResult with all planetary positions, houses, etc.
        // =================================================================
        public KundliResult CalculateKundli(int day, int month, int year, int hour, int minute, double latitude, double longitude)
        {
            // ---------------------------------------------------------
            // Arrays for planetary positions (index 0..10, 0 unused)
            // Index mapping (matching original JS):
            //   [1]=Lagna  [2]=Sun  [3]=Moon  [4]=Mars  [5]=Mercury
            //   [6]=Jupiter [7]=Venus [8]=Saturn [9]=Rahu [10]=Ketu [11]=MC
            // ---------------------------------------------------------
            double[] dPlt = new double[12];   // Total longitude (nirayana)
            double[] dPltS = new double[12];  // FNQ-encoded position

            // Planet-specific orbital elements (stored for MAP usage)
            double[] M1X = new double[6]; // Mean anomaly (planet index 1..5)
            double[] MAP = new double[6]; // Mean anomaly mapping (planet index 1..5)

            double dAyanamsa = 0.0;
            double ABBQQ_d = 0.0; // Nutation aberration constant
            double NUTQQ_d = 0.0; // Nutation correction for moon

            // Birth time in IST (decimal hours)
            double dBirthTimeIST = hour + minute / 60.0;
            // Convert to UT (GMT) by subtracting IST offset of 5.5 hours
            double dBirthTimeUT = dBirthTimeIST - 5.5;

            // Julian Day from UT
            double dJDF = julday(day, month, year, dBirthTimeUT);

            // Geographic coordinates in decimal degrees
            double dLong = longitude;
            double dLatt = latitude;

            // ======== RAHU AND AYANAMSA ========
            double RHM_d = FNR((259.2097527 - 1934.138634 * dJDF / 36525.0), 360.0);
            dAyanamsa = 22.462056 + dJDF / 26163.259 - 17.23 / 3600.0 * Math.Sin(RHM_d)
                        + 0.21 / 3600.0 * Math.Sin(2.0 * RHM_d) - 3.142 / 3600.0;

            // ======== SUN CALCULATION ========
            double BQQ = dJDF / 36525.0;
            double MSNQQ = 279.2038555 + (129602768.13 / 3600.0) * BQQ + 1.089 / 3600.0 * BQQ * BQQ;
            double MSAQQ = 358.9830444 + (129596579.1 / 3600.0) * BQQ
                           - (0.541 / 3600.0) * BQQ * BQQ - (0.012 / 3600.0) * BQQ * BQQ * BQQ;
            double RHUQQ = 259.2097527 - (6962911.23 / 3600.0) * BQQ + 7.48 / 3600.0 * BQQ * BQQ;

            ABBQQ_d = 1.0 * (-20.5 - 0.34 * FNCS(MSAQQ)) / 3600.0;
            NUTQQ_d = -1.27 * FNSN(2.0 * MSNQQ) + 0.13 * FNSN(MSNQQ)
                      - 0.05 * FNSN(3.0 * MSNQQ + 79.0)
                      - 17.23 * FNSN(RHUQQ) + 0.21 * FNSN(2.0 * RHUQQ);
            NUTQQ_d = 1.0 * NUTQQ_d / 3600.0;

            double MAS = MSAQQ;
            double MQQ = FNR(MSAQQ, 360.0);
            MAS = MQQ;

            double XQQ = 6910.057 * FNSN(MQQ) + 72.338 * FNSN(2.0 * MQQ)
                         + 1.054 * FNSN(3.0 * MQQ) + 0.018 * FNSN(4.0 * MQQ);
            double XLQQ = -17.24 * (BQQ + 0.003 * BQQ * BQQ) * FNSN(MQQ)
                          - 0.361 * BQQ * FNSN(2.0 * MQQ);
            XQQ = (XQQ + XLQQ) / 3600.0;

            double YQQ = MSNQQ + XQQ;

            // ======== PLANET PERTURBATION PRE-CALCULATIONS ========
            BQQ = dJDF / 36525.0;
            double PSLQQ = 6.4 * FNSN(231.19 + 20.2 * BQQ)
                           + (1.882 - 0.016 * BQQ) * FNSN(57.24 + 150.27 * BQQ)
                           + 0.266 * FNSN(31.8 + 119.0 * BQQ)
                           + 0.202 * FNSN(315.6 + 893.0 * BQQ);

            double VQQ = FNDG(62.4544361) + (81066394.75 / 3600.0) * BQQ
                         + (0.02581 / 3600.0) * BQQ * BQQ;

            double QVQQ = VQQ + 180.0;

            double PSVQQ = 4.84 * FNSN(QVQQ) - 5.53 * FNSN(2.0 * QVQQ)
                           - 0.67 * FNSN(3.0 * QVQQ) - 0.21 * FNSN(4.0 * QVQQ)
                           - 0.12 * FNSN(2.0 * QVQQ + MSAQQ)
                           - 2.5 * FNSN(MSAQQ + 12.0 - 2.0 * QVQQ)
                           - 1.56 * FNSN(MSAQQ + 12.0 - 3.0 * QVQQ)
                           + 0.14 * FNSN(MSAQQ + 12.0 - 4.0 * QVQQ)
                           - 1.02 * FNSN(2.0 * MSAQQ + 40.0 - 3.0 * QVQQ);
            PSVQQ = PSVQQ - 0.15 * FNSN(2.0 * MSAQQ + 40.0 - 4.0 * QVQQ)
                    + 0.12 * FNSN(2.0 * MSAQQ + 40.0 - 5.0 * QVQQ)
                    - 0.15 * FNSN(MSAQQ + 56.0 - 5.0 * QVQQ);

            double QJQQ = 180.0 - 41.114619 - (118672080.98 / 3600.0) * BQQ
                          + 0.11613 * BQQ * BQQ;

            double PSJQQ = 7.21 * FNSN(QJQQ - 1.083333)
                           - 2.73 * FNSN(12.0 * QJQQ - 0.25)
                           - 0.16 * FNSN(3.0 * QJQQ + 4.85)
                           + 2.6 * FNSN(QJQQ + MSAQQ + 85.7667)
                           - 1.67 * FNSN(2.0 * QJQQ + MSAQQ - 22.6);

            double QMQQ = 195.8056 + 191.4048157 * BQQ * 100.0 - MSAQQ + 5.29;

            double PSMQQ = 2.04 * FNSN(2.0 * QMQQ + 0.25)
                           + 0.27 * FNSN(QMQQ - 0.6)
                           - 1.77 * FNSN(2.0 * QMQQ + MSAQQ - 36.25167)
                           - 0.58 * FNSN(4.0 * QMQQ + 2.0 * MSAQQ + 84.0)
                           - 0.5 * FNSN(4.0 * QMQQ + MSAQQ - 47.0)
                           - 0.43 * FNSN(3.0 * QMQQ + MSAQQ - 47.7);
            PSMQQ = PSMQQ - 0.43 * FNSN(3.0 * QMQQ + MSAQQ - 47.7);

            double QCQQ = FNDG(344.38316) + (1602961611.18 / 3600.0) * BQQ
                          - (5.169 / 3600.0) * BQQ * BQQ
                          + (0.0068 / 3600.0) * BQQ * BQQ * BQQ;

            double PSCQQ = 6.44 * FNSN(QCQQ) - 0.42 * FNSN(QCQQ - MSAQQ);

            double DLQQ = (PSLQQ + PSMQQ + PSJQQ + PSVQQ + PSCQQ) / 3600.0;

            double DRVQQ = -1.12 * FNCS(QVQQ) + 3.25 * FNCS(2.0 * QVQQ + 0.1167)
                           + 0.5 * FNCS(3.0 * QVQQ - 1.0833)
                           + 0.18 * FNCS(4.0 * QVQQ - 0.24)
                           + 0.08 * FNCS(5.0 * QVQQ - 3.0);

            double DRJQQ = 3.36 * FNCS(QJQQ - 1.1) - 1.91 * FNCS(2.0 * QJQQ - 0.2167)
                           - 0.13 * FNCS(3.0 * QJQQ + 4.35);

            double DRCQQ = 6.35 * FNCS(QCQQ);

            double DRQQ = DRVQQ + DRJQQ + DRCQQ;
            DRQQ = DRQQ / (RAD_TO_DEG * 3600.0);

            YQQ = YQQ + NUTQQ_d + ABBQQ_d + DLQQ;
            YQQ = FNR(YQQ, 360.0);

            // ======== SUN POSITION (Equation of Center) ========
            double Y_d = YQQ;
            double M_d = FNFR(2.994397 + dJDF / 365.25964) * 360.0;

            double X_d = 1.9142998 * FNSN(M_d)
                         + FNSN(2.0 * M_d) / 50.0290666
                         + FNSN(3.0 * M_d) / 3455.396;

            double XX_d = X_d;
            double MM_d = M_d;

            dPlt[2] = FNR(Y_d, 360.0); // Sayana Sun longitude

            // Sun-Earth distance in AU
            double R_d = 0.99972091 / (1.0 + 0.016706 * FNCS(MM_d + XX_d)) + DRQQ;
            double H_d = Y_d;
            double J_d = R_d;
            double a_d = M_d;
            double B_d = (dJDF - 0.5) / 36525.0;

            // ==========================================================
            // PASS 1: Compute all planet orbital elements (M1X values)
            // ==========================================================
            bool[] isRetrograde = new bool[12];
            double[] stored_m_d = new double[6];
            double[] stored_p_d = new double[6];
            double[] stored_k_d = new double[6];
            double[] stored_l_d = new double[6];
            double[] stored_n_d = new double[6];
            double[] stored_q_d = new double[6];

            for (int Panchgrahi = 1; Panchgrahi <= 5; Panchgrahi++)
            {
                // Planet orbital elements (epoch J2000.0)
                double m_d, m1_d, m2_d, m3_d;
                double p_d, p1_d, p2_d, p3_d;
                double k_d, k1_d, k2_d, k3_d;
                double l_d, l1_d, l2_d, l3_d;
                double n_d, n1_d, n2_d, n3_d;
                double q_d;

                switch (Panchgrahi)
                {
                    case 1: // Mars
                        m_d = 293.737334; m1_d = 53.17137642; m2_d = 0.0003107; m3_d = 0;
                        p_d = 334.218203; p1_d = 1.8407584; p2_d = 0.0001299; p3_d = -0.00000119;
                        k_d = 0.0933129; k1_d = 0.000092064; k2_d = 0.000000077; k3_d = 0;
                        l_d = 1.850333; l1_d = -0.000675; l2_d = 0.0000126; l3_d = 0;
                        n_d = 48.786442; n1_d = 0.7709917; n2_d = -0.0000014; n3_d = -0.00000533;
                        q_d = 1.5236883;
                        break;
                    case 2: // Mercury
                        m_d = 178.179078; m1_d = 415.2057519; m2_d = 0.0003011; m3_d = 0;
                        p_d = 75.899697; p1_d = 1.5554889; p2_d = 0.0002947; p3_d = 0;
                        k_d = 0.20561421; k1_d = 0.00002046; k2_d = 0.00000003; k3_d = 0;
                        l_d = 7.002881; l1_d = 0.0018608; l2_d = -0.0000183; l3_d = 0;
                        n_d = 47.145944; n1_d = 1.1852083; n2_d = 0.0001739; n3_d = 0;
                        q_d = 0.3870986;
                        break;
                    case 3: // Jupiter
                        m_d = 238.049257; m1_d = 8.434172183; m2_d = 0.0003347; m3_d = -0.00000165;
                        p_d = 12.720972; p1_d = 1.6099617; p2_d = 0.00105627; p3_d = -0.00000343;
                        k_d = 0.04833475; k1_d = 0.00016418; k2_d = -0.0000004676; k3_d = -0.0000000017;
                        l_d = 1.308736; l1_d = -0.0056961; l2_d = 0.0000039; l3_d = 0;
                        n_d = 99.443414; n1_d = 1.01053; n2_d = 0.00035222; n3_d = -0.00000851;
                        q_d = 5.202561;
                        break;
                    case 4: // Venus
                        m_d = 342.767053; m1_d = 162.5533664; m2_d = 0.0003097; m3_d = 0;
                        p_d = 130.163833; p1_d = 1.4080361; p2_d = -0.0009764; p3_d = 0;
                        k_d = 0.00682069; k1_d = -0.00004774; k2_d = 0.00000009099999; k3_d = 0;
                        l_d = 3.393631; l1_d = 0.0010058; l2_d = -0.000001; l3_d = 0;
                        n_d = 75.779647; n1_d = 0.89985; n2_d = 0.00041; n3_d = 0;
                        q_d = 0.7233316;
                        break;
                    case 5: // Saturn
                        m_d = 266.564377; m1_d = 3.398638567; m2_d = 0.0003245; m3_d = -0.0000058;
                        p_d = 91.098214; p1_d = 1.9584158; p2_d = 0.00082636; p3_d = 0.00000461;
                        k_d = 0.05589232; k1_d = -0.0003455; k2_d = -0.000000728; k3_d = 0;
                        l_d = 0.492519; l1_d = -0.0039189; l2_d = -0.00001549; l3_d = 0.00000004;
                        n_d = 112.790414; n1_d = 0.8731951; n2_d = -0.00015218; n3_d = -0.00000531;
                        q_d = 9.554747;
                        break;
                    default:
                        m_d = 0; m1_d = 0; m2_d = 0; m3_d = 0;
                        p_d = 0; p1_d = 0; p2_d = 0; p3_d = 0;
                        k_d = 0; k1_d = 0; k2_d = 0; k3_d = 0;
                        l_d = 0; l1_d = 0; l2_d = 0; l3_d = 0;
                        n_d = 0; n1_d = 0; n2_d = 0; n3_d = 0;
                        q_d = 0;
                        break;
                }

                // Compute time-adjusted orbital elements
                m_d = (m_d - p_d)
                      + B_d * (360.0 * m1_d - p1_d)
                      + B_d * B_d * (m2_d - p2_d)
                      + B_d * B_d * B_d * (m3_d - p3_d);

                M1X[Panchgrahi] = m_d;

                p_d = (p_d - n_d)
                      + B_d * (p1_d - n1_d)
                      + B_d * B_d * (p2_d - n2_d)
                      + B_d * B_d * B_d * (p3_d - n3_d);

                n_d = n_d + n1_d * B_d + n2_d * B_d * B_d + n3_d * B_d * B_d * B_d;
                k_d = k_d + k1_d * B_d + k2_d * B_d * B_d + k3_d * B_d * B_d * B_d;
                l_d = l_d + l1_d * B_d + l2_d * B_d * B_d + l3_d * B_d * B_d * B_d;

                stored_m_d[Panchgrahi] = m_d;
                stored_p_d[Panchgrahi] = p_d;
                stored_k_d[Panchgrahi] = k_d;
                stored_l_d[Panchgrahi] = l_d;
                stored_n_d[Panchgrahi] = n_d;
                stored_q_d[Panchgrahi] = q_d;
            }

            // MAP mapping: [1]=Mercury, [2]=Venus, [3]=Mars, [4]=Jupiter, [5]=Saturn
            // Now all M1X values are available
            MAP[1] = M1X[2]; MAP[2] = M1X[4]; MAP[3] = M1X[1];
            MAP[4] = M1X[3]; MAP[5] = M1X[5];

            // ==========================================================
            // PASS 2: Compute perturbations, Kepler equation, positions
            // ==========================================================
            for (int Panchgrahi = 1; Panchgrahi <= 5; Panchgrahi++)
            {
                double m_d = stored_m_d[Panchgrahi];
                double p_d = stored_p_d[Panchgrahi];
                double k_d = stored_k_d[Panchgrahi];
                double l_d = stored_l_d[Panchgrahi];
                double n_d = stored_n_d[Panchgrahi];
                double q_d = stored_q_d[Panchgrahi];

                double DL_pur = 0, DML_pur = 0, DM_pur = 0, DHL_pur = 0, DA_pur = 0;
                double DR_pur = 0, DS_pur = 0, DP_pur = 0;

                // ======== PERTURBATION CALCULATIONS ========
                switch (Panchgrahi)
                {
                    case 1: // Mars Perturbations
                        {
                            double a_pur = 3.0 * MAP[4] - 8.0 * MAP[3] + 4.0 * MAS;
                            double SA_pur = Math.Sin(a_pur);
                            double CA_pur = Math.Cos(a_pur);
                            DML_pur = -1.0 * (0.01133 * SA_pur + 0.00933 * CA_pur);
                            DML_pur = FNRAD(DML_pur);
                            DM_pur = DML_pur;

                            DL_pur = 0.00705 * Math.Cos(MAP[4] - MAP[3] - 0.85448);
                            DL_pur += 0.00607 * Math.Cos(2.0 * MAP[4] - MAP[3] - 0.85448);
                            DL_pur += 0.00445 * Math.Cos(2.0 * MAP[4] - 2.0 * MAP[3] - 3.3492);
                            DL_pur += 0.00388 * Math.Cos(MAS - 2.0 * MAP[3] + 0.35771);
                            DL_pur += 0.00238 * Math.Cos(MAS - MAP[3] + 0.61256);
                            DL_pur += 0.00204 * Math.Cos(2.0 * MAS - 3.0 * MAP[3] + 2.7688);
                            DL_pur += 0.00177 * Math.Cos(3.0 * MAP[3] - MAP[2] - 1.0053);
                            DL_pur += 0.00136 * Math.Cos(2.0 * MAS - 4.0 * MAP[3] + 2.6894);
                            DL_pur += 0.00104 * Math.Cos(MAP[4] + 0.30749);

                            DR_pur = 0.000053227 * Math.Cos(MAP[4] - MAP[3] + 0.00717864);
                            DR_pur += 0.000050989 * Math.Cos(2.0 * MAP[4] - 2.0 * MAP[3] - 1.77997);
                            DR_pur += 0.000038278 * Math.Cos(2.0 * MAP[4] - MAP[3] - 1.71617);
                            DR_pur += 0.000015996 * Math.Cos(MAS - MAP[3] - 0.969618);
                            DR_pur += 0.000014764 * Math.Cos(2.0 * MAS - 3.0 * MAP[3] + 1.16768);
                            DR_pur += 0.000008966 * Math.Cos(MAP[4] - 2.0 * MAP[3] + 0.761225);
                            DR_pur += 0.000007914 * Math.Cos(3.0 * MAP[4] - 2.0 * MAP[3] - 2.43887);
                            DR_pur += 0.000007004 * Math.Cos(2.0 * MAP[4] - 3.0 * MAP[3] - 1.79573);
                            DR_pur += 0.00000662 * Math.Cos(MAS - 2.0 * MAP[3] + 1.97575);
                            DR_pur += 0.00000493 * Math.Cos(3.0 * MAP[4] - 3.0 * MAP[3] - 1.33069);
                            DR_pur += 0.000004693 * Math.Cos(3.0 * MAS - 5.0 * MAP[3] + 3.32665);
                            DR_pur += 0.000004571 * Math.Cos(2.0 * MAS - 4.0 * MAP[3] + 4.27086);
                            DR_pur += 0.000004409 * Math.Cos(3.0 * MAP[4] - MAP[3] - 2.02158);
                        }
                        break;

                    case 2: // Mercury Perturbations
                        {
                            DL_pur = 0.00204 * Math.Cos(5.0 * MAP[2] - 2.0 * MAP[1] + 0.21328);
                            DL_pur += 0.00103 * Math.Cos(2.0 * MAP[2] - MAP[1] - 2.8046);
                            DL_pur += 0.00091 * Math.Cos(2.0 * MAP[4] - MAP[1] - 0.64582);
                            DL_pur += 0.00078 * Math.Cos(5.0 * MAP[2] - 3.0 * MAP[1] + 0.17692);

                            DR_pur = 0.000007525 * Math.Cos(2.0 * MAP[4] - MAP[1] + 0.925251);
                            DR_pur += 0.000006802 * Math.Cos(5.0 * MAP[2] - 3.0 * MAP[1] - 4.53642);
                            DR_pur += 0.000005457 * Math.Cos(2.0 * MAP[2] - 2.0 * MAP[1] - 1.24246);
                            DR_pur += 0.000003569 * Math.Cos(5.0 * MAP[2] - MAP[1] - 1.35699);
                        }
                        break;

                    case 3: // Jupiter Perturbations (Guru)
                        {
                            double T_pur = dJDF / 36525.0;
                            double PLAN_pur = ((-0.0000000017 * T_pur - 0.0000004676) * T_pur + 0.00016418) * T_pur + 0.04833475;
                            double J1 = T_pur / 5.0 + 0.1;
                            double J2 = FNUNW(4.14473 + 52.9691 * T_pur);
                            double J3 = FNUNW(4.641118 + 21.32991 * T_pur);
                            double J4 = FNUNW(4.250177 + 7.478172 * T_pur);
                            double J5 = 5.0 * J3 - 2.0 * J2;
                            double J6 = 2.0 * J2 - 6.0 * J3 + 3.0 * J4;
                            double J7 = J3 - J2;

                            double SJ3 = Math.Sin(J3), CJ3 = Math.Cos(J3);
                            double S2J3 = Math.Sin(2.0 * J3), C2J3 = Math.Cos(2.0 * J3);
                            double SJ5 = Math.Sin(J5), CJ5 = Math.Cos(J5);
                            double S2J5 = Math.Sin(2.0 * J5), SJ6 = Math.Sin(J6);
                            double SJ7 = Math.Sin(J7), CJ7 = Math.Cos(J7);
                            double S2J7 = Math.Sin(2.0 * J7), C2J7 = Math.Cos(2.0 * J7);
                            double S3J7 = Math.Sin(3.0 * J7), C3J7 = Math.Cos(3.0 * J7);
                            double S4J7 = Math.Sin(4.0 * J7), C4J7 = Math.Cos(4.0 * J7);
                            double C5J7 = Math.Cos(5.0 * J7);

                            DML_pur = (0.331364 - (0.010281 + 0.004692 * J1) * J1) * SJ5;
                            DML_pur += (0.003228 - (0.06443601 - 0.002075 * J1) * J1) * CJ5;
                            DML_pur -= (0.003083 + (0.000275 - 0.000489 * J1) * J1) * S2J5;
                            DML_pur += 0.002472 * SJ6 + 0.013619 * SJ7 + 0.018472 * S2J7;
                            DML_pur += 0.006717 * S3J7 + 0.002775 * S4J7;
                            DML_pur += 0.006417 * S2J7 * SJ3;
                            DML_pur += (0.007275 - 0.001253 * J1) * SJ7 * SJ3;
                            DML_pur += 0.002439 * S3J7 * SJ3;
                            DML_pur -= (0.035681 + 0.001208 * J1) * SJ7 * CJ3;
                            DML_pur -= 0.003767 * C2J7 * SJ3;
                            DML_pur -= (0.033839 + 0.001125 * J1) * CJ7 * SJ3;
                            DML_pur -= 0.004261 * S2J7 * CJ3;
                            DML_pur += (0.001161 * J1 - 0.006333) * CJ7 * CJ3;
                            DML_pur += 0.002178 * CJ3 - 0.006675 * C2J7 * CJ3;
                            DML_pur -= 0.002664 * C3J7 * CJ3 - 0.002572 * SJ7 * S2J3;
                            DML_pur -= 0.003567 * S2J7 * S2J3 + 0.002094 * CJ7 * C2J3;
                            DML_pur += 0.003342 * C2J7 * C2J3;
                            DML_pur = FNRAD(DML_pur);

                            DS_pur = (3606.0 + (130.0 - 43.0 * J1) * J1) * SJ5;
                            DS_pur += (1289.0 - 580.0 * J1) * CJ5 - 6764.0 * SJ7 * SJ3;
                            DS_pur -= 1110.0 * S2J7 * SJ3 - 224.0 * S3J7 - SJ3 - 204.0 * SJ3;
                            DS_pur += (1284.0 + 116.0 * J1) * CJ7 * SJ3 + 188.0 * C2J7 * SJ3;
                            DS_pur += (1460.0 + 130.0 * J1) * SJ7 * CJ3 + 224.0 * S2J7 * CJ3;
                            DS_pur -= 817.0 * CJ3 + 6074.0 * CJ3 * CJ7 + 992.0 * C2J7 * CJ3;
                            DS_pur += 508.0 * C3J7 * CJ3 + 230.0 * C4J7 * CJ3 + 108.0 * C5J7 * CJ3;
                            DS_pur -= (956.0 + 73.0 * J1) * SJ7 * S2J3 + 448.0 * S2J7 * S2J3;
                            DS_pur += 137.0 * S3J7 * S2J3 + (108.0 * J1 - 997.0) * CJ7 * S2J3;
                            DS_pur += 480.0 * C2J7 * S2J3 + 148.0 * C3J7 * S2J3;
                            DS_pur += (99.0 * J1 - 956.0) * SJ7 * C2J3 + 490.0 * S2J7 * C2J3;
                            DS_pur += 158.0 * S3J7 * C2J3 + 179.0 * C2J3;
                            DS_pur += (1024.0 + 75.0 * J1) * CJ7 * C2J3 - 437.0 * C2J7 * C2J3;
                            DS_pur -= 132.0 * C3J7 * C2J3;
                            DS_pur *= 0.0000001;

                            DP_pur = (0.007192 - 0.003147 * J1) * SJ5;
                            DP_pur -= 0.004344 * SJ3;
                            DP_pur += (J1 * (0.000197 * J1 - 0.000675) - 0.020428) * CJ5;
                            DP_pur += 0.034036 * CJ7 * SJ3;
                            DP_pur += (0.007269 + 0.000672 * J1) * SJ7 * SJ3;
                            DP_pur += 0.005614 * C2J7 * SJ3 + 0.002964 * C3J7 * SJ3;
                            DP_pur += 0.037761 * SJ7 * CJ3 + 0.006158 * S2J7 * CJ3;
                            DP_pur -= 0.006603 * CJ7 * CJ3 - 0.005356 * SJ7 * S2J3;
                            DP_pur += 0.002722 * S2J7 * S2J3 + 0.004483 * CJ7 * S2J3;
                            DP_pur -= 0.002642 * C2J7 * S2J3 + 0.004403 * SJ7 * C2J3;
                            DP_pur -= 0.002536 * S2J7 * C2J3 + 0.005547 * CJ7 * C2J3;
                            DP_pur -= 0.002689 * C2J7 * C2J3;
                            DM_pur = DML_pur - FNRAD(DP_pur) / PLAN_pur;

                            DA_pur = 205.0 * CJ7 - 263.0 * CJ5 + 693.0 * C2J7 + 312.0 * C3J7;
                            DA_pur += 147.0 * C4J7 + 299.0 * SJ7 * SJ3 + 181.0 * C2J7 * SJ3;
                            DA_pur += 204.0 * S2J7 * CJ3 + 111.0 * S3J7 * CJ3 - 337.0 * CJ7 * CJ3;
                            DA_pur -= 111.0 * C2J7 * CJ3;
                            DA_pur *= 0.000001;
                        }
                        break;

                    case 4: // Venus Perturbations
                        {
                            DML_pur = 0.00077 * Math.Sin(4.1406 + B_d * 2.6227);
                            DML_pur = FNRAD(DML_pur);
                            DM_pur = DML_pur;

                            DL_pur = 0.00313 * Math.Cos(2.0 * MAS - 2.0 * MAP[2] - 2.587);
                            DL_pur += 0.00136 * Math.Cos(MAS - MAP[2] - 2.0788);
                            DL_pur += 0.00096 * Math.Cos(3.0 * MAS - 2.0 * MAP[2] - 2.3721);
                            DL_pur += 0.00082 * Math.Cos(MAP[4] - MAP[2] - 3.6318);

                            DR_pur = 0.000022501 * Math.Cos(2.0 * MAS - 2.0 * MAP[2] - 1.01592);
                            DR_pur += 0.000019045 * Math.Cos(3.0 * MAS - 3.0 * MAP[2] + 1.61577);
                            DR_pur += 0.000006887 * Math.Cos(MAP[4] - MAP[2] - 2.06106);
                            DR_pur += 0.000005172 * Math.Cos(MAS - MAP[2] - 0.508065);
                            DR_pur += 0.00000362 * Math.Cos(5.0 * MAS - 4.0 * MAP[2] - 1.81877);
                            DR_pur += 0.000003283 * Math.Cos(4.0 * MAS - 4.0 * MAP[2] + 1.10851);
                            DR_pur += 0.000003074 * Math.Cos(2.0 * MAP[4] - 2.0 * MAP[2] - 0.962846);
                        }
                        break;

                    case 5: // Saturn Perturbations (Shani)
                        {
                            double T_pur = dJDF / 36525.0;
                            double PLAN_pur = ((0.00000000074 * T_pur - 0.000000728) * T_pur - 0.0003455) * T_pur + 0.05589232;

                            double J1 = T_pur / 5.0 + 0.1;
                            double J2 = FNUNW(4.14473 + 52.9691 * T_pur);
                            double J3 = FNUNW(4.641118 + 21.32991 * T_pur);
                            double J4 = FNUNW(4.250177 + 7.478172 * T_pur);
                            double J5 = 5.0 * J3 - 2.0 * J2;
                            double J6 = 2.0 * J2 - 6.0 * J3 + 3.0 * J4;
                            double J7 = J3 - J2;

                            double SJ3 = Math.Sin(J3), CJ3 = Math.Cos(J3);
                            double S2J3 = Math.Sin(2.0 * J3), C2J3 = Math.Cos(2.0 * J3);
                            double S3J3 = Math.Sin(3.0 * J3), C3J3 = Math.Cos(3.0 * J3);
                            double S4J3 = Math.Sin(4.0 * J3), C4J3 = Math.Cos(4.0 * J3);
                            double SJ5 = Math.Sin(J5), CJ5 = Math.Cos(J5);
                            double C2J5 = Math.Cos(2.0 * J5), S2J5 = Math.Sin(2.0 * J5);
                            double SJ6 = Math.Sin(J6);
                            double SJ7 = Math.Sin(J7), CJ7 = Math.Cos(J7);
                            double S2J7 = Math.Sin(2.0 * J7), C2J7 = Math.Cos(2.0 * J7);
                            double S3J7 = Math.Sin(3.0 * J7), C3J7 = Math.Cos(3.0 * J7);
                            double S4J7 = Math.Sin(4.0 * J7), C4J7 = Math.Cos(4.0 * J7);
                            double C5J7 = Math.Cos(5.0 * J7), S5J7 = Math.Sin(5.0 * J7);
                            double J8 = J4 - J3;
                            double S2J8 = Math.Sin(2.0 * J8), C2J8 = Math.Cos(2.0 * J8);
                            double S3J8 = Math.Sin(3.0 * J8), C3J8 = Math.Cos(3.0 * J8);

                            DML_pur = 0.007581 * S2J5 - 0.007986 * SJ6;
                            DML_pur -= 0.148811 * SJ7 - 0.040786 * S2J7;
                            DML_pur -= (0.814181 - (0.01815 - 0.016714 * J1) * J1) * SJ5;
                            DML_pur -= (0.010497 - (0.160906 - 0.0041 * J1) * J1) * CJ5;
                            DML_pur -= 0.015208 * S3J7 - 0.006339 * S4J7;
                            DML_pur -= 0.006244 * SJ3 - 0.0165 * S2J7 * SJ3;
                            DML_pur += (0.008931 + 0.002728 * J1) * SJ7 * SJ3;
                            DML_pur -= 0.005775 * S3J7 * SJ3;
                            DML_pur += (0.081344 + 0.003206 * J1) * CJ7 * SJ3;
                            DML_pur += 0.015019 * C2J7 * SJ3;
                            DML_pur += (0.085581 + 0.002494 * J1) * SJ7 * CJ3;
                            DML_pur += 0.014394 * C2J7 * CJ3;
                            DML_pur += (0.025328 - 0.003117 * J1) * CJ7 * CJ3;
                            DML_pur += 0.006319 * C3J7 * CJ3 + 0.006369 * SJ7 * S2J3;
                            DML_pur += 0.009156 * S2J7 * S2J3 + 0.007525 * S3J8 * S2J3;
                            DML_pur -= 0.005236 * CJ7 * C2J3 - 0.007736 * C2J7 * C2J3;
                            DML_pur -= 0.007528 * C3J8 * C2J3;
                            DML_pur = FNRAD(DML_pur);

                            DS_pur = (-7927.0 + (2548.0 + 91.0 * J1) * J1) * SJ5;
                            DS_pur += (13381.0 + (1226.0 - 253.0 * J1) * J1) * CJ5;
                            DS_pur += (248.0 - 121.0 * J1) * S2J5;
                            DS_pur -= (305.0 + 91.0 * J1) * C2J5 + 412.0 * S2J7;
                            DS_pur += 12415.0 * SJ3 + (390.0 - 617.0 * J1) * SJ7 * SJ3;
                            DS_pur += (165.0 - 204.0 * J1) * S2J7 * SJ3;
                            DS_pur += 26599.0 * CJ7 * SJ3 - 4687.0 * C2J7 * SJ3;
                            DS_pur -= 1870.0 * C3J7 * SJ3 - 821.0 * C4J7 * SJ3;
                            DS_pur -= 377.0 * C5J7 * SJ3 + 497.0 * C2J8 * SJ3;
                            DS_pur += (163.0 - 611.0 * J1) * CJ3 - 12696.0 * SJ7 * CJ3;
                            DS_pur -= 4200.0 * S2J7 * CJ3 - 1503.0 * S3J7 * CJ3;
                            DS_pur -= 619.0 * S4J7 * CJ3 - 268.0 * S5J7 * CJ3;
                            DS_pur -= (282.0 + 1306.0 * J1) * CJ7 * CJ3;
                            DS_pur += (-86.0 + 230.0 * J1) * C2J7 * CJ3;
                            DS_pur += 461.0 * S2J8 + CJ3 - 350.0 * S2J3;
                            DS_pur += (2211.0 - 286.0 * J1) * SJ7 * S2J3;
                            DS_pur -= 2208.0 * S2J7 * S2J3 - 568.0 * S3J7 * S2J3;
                            DS_pur -= 346.0 * S4J7 * S2J3 - (2780.0 + 222.0 * J1) * CJ7 * S2J3;
                            DS_pur += (2022.0 + 263.0 * J1) * C2J7 * S2J3 + 248.0 * C3J7 * S2J3;
                            DS_pur += 242.0 * S3J8 * S2J3 + 467.0 * C3J8 * S2J3 - 490.0 * C2J3;
                            DS_pur -= (2842.0 - 279.0 * J1) * SJ7 * C2J3;
                            DS_pur += (128.0 + 226.0 * J1) * S2J7 * C2J3 + 224.0 * S3J7 * C2J3;
                            DS_pur += (-1594.0 + 282.0 * J1) * CJ7 * C2J3;
                            DS_pur += (2162.0 - 207.0 * J1) * C2J7 * C2J3;
                            DS_pur += 561.0 * C3J7 * C2J3 + 343.0 * C4J7 * C2J3;
                            DS_pur += 469.0 * S3J8 * C2J3 - 242.0 * C3J8 * C2J3;
                            DS_pur -= 205.0 * SJ7 * S3J3 + 262.0 * S3J7 * S3J3 + 208.0 * CJ7 * C3J3;
                            DS_pur -= 271.0 * C3J7 * C3J3 - 382.0 * C3J7 * S4J3 - 376.0 * S3J7 * C4J3;
                            DS_pur *= 0.0000001;

                            DA_pur = 572.0 * SJ5 - 1590.0 * S2J7 * CJ3 + 2933.0 * CJ5;
                            DA_pur -= 647.0 * S3J7 * CJ3 + 33629.0 * CJ7 - 344.0 * S4J7 * CJ3;
                            DA_pur -= 3081.0 * C2J7 + 2885.0 * CJ7 * CJ3 - 1423.0 * C3J7;
                            DA_pur += (2172.0 + 102.0 * J1) * C2J7 * CJ3 - 671.0 * C4J7;
                            DA_pur += 296.0 * C3J7 * CJ3 - 320.0 * C5J7 - 267.0 * S2J7 * S2J3;
                            DA_pur += 1098.0 * SJ3 - 778.0 * CJ7 * S2J3 - 2812.0 * SJ7 * SJ3;
                            DA_pur += 495.0 * C2J7 * S2J3 + 688.0 * S2J7 * SJ3;
                            DA_pur += 250.0 * C3J7 * S2J3 - 393.0 * S3J7 * SJ3;
                            DA_pur -= 856.0 * SJ7 * C2J3 - 228.0 * S4J7 * SJ3 + 441.0 * S2J7 * C2J3;
                            DA_pur += 2138.0 * CJ7 * SJ3 + 296.0 * C2J7 * C2J3 - 999.0 * C2J7 * SJ3;
                            DA_pur -= 325.0 * C4J7 * SJ3 + 398.0 * S3J7 * S3J3 - 890.0 * CJ3;
                            DA_pur += 211.0 * C3J7 * C2J3 - 642.0 * C3J7 * SJ3 - 427.0 * SJ7 * S3J3;
                            DA_pur += 344.0 * CJ7 * C3J3 + 2206.0 * SJ7 * CJ3 - 427.0 * C3J7 * C3J3;
                            DA_pur *= 0.000001;

                            DP_pur = (0.07710801 + (0.007186 - 0.001533 * J1) * J1) * SJ5 - 0.007075 * SJ7;
                            DP_pur += (0.045803 - (0.014766 + 0.000536 * J1) * J1) * CJ5 - 0.072586 * CJ3;
                            DP_pur -= 0.075825 * SJ7 * SJ3 - 0.024839 * S2J7 * SJ3;
                            DP_pur -= 0.008631 * S3J7 * SJ3 - 0.150383 * CJ7 * CJ3;
                            DP_pur += 0.026897 * C2J7 * CJ3 + 0.010053 * C3J7 * CJ3;
                            DP_pur -= (0.013597 + 0.001719 * J1) * SJ7 * S2J3;
                            DP_pur += 0.011981 * S2J7 * C2J3;
                            DP_pur -= (0.007742 - 0.001517 * J1) * CJ7 * S2J3;
                            DP_pur += (0.013586 - 0.001375 * J1) * C2J7 * C2J3;
                            DP_pur -= (0.013667 - 0.001239 * J1) * SJ7 * C2J3;
                            DP_pur += (0.014861 + 0.001136 * J1) * CJ7 * C2J3;
                            DP_pur -= (0.013064 + 0.001628 * J1) * C2J7 * C2J3;
                            DM_pur = DML_pur - FNRAD(DP_pur) / PLAN_pur;

                            DHL_pur = 0.000747 * CJ7 * SJ3 + 0.001069 * CJ7 * CJ3;
                            DHL_pur += 0.002108 * S2J7 * S2J3 + 0.001261 * C2J7 * S2J3;
                            DHL_pur += 0.001236 * S2J7 * C2J3 - 0.002075 * C2J7 * C2J3;
                            DHL_pur = FNRAD(DHL_pur);
                        }
                        break;
                }

                // ======== APPLY PERTURBATIONS & SOLVE KEPLER EQUATION ========
                m_d = m_d + DM_pur * RAD_TO_DEG;
                p_d = p_d + (DML_pur - DM_pur) * RAD_TO_DEG;
                k_d = k_d + DS_pur;
                q_d = q_d + DA_pur;
                double O_d = 0.0;

                // Solve Kepler equation by iteration (6 iterations)
                for (int Y = 1; Y <= 6; Y++)
                {
                    O_d = m_d + k_d * FNSN(O_d) * 57.2957795;
                }

                // Heliocentric longitude and geocentric conversion
                double V_d = 2.0 * FNAT(Math.Sqrt((1.0 + k_d) / (1.0 - k_d)) * FNTN(O_d / 2.0)) + p_d;
                double U_d = FNAT(FNTN(V_d) * FNCS(l_d)) + dLong;
                U_d = FNFR(U_d / 360.0 + 100.0) * 360.0;

                double YY_d = Math.Abs(FNFR((U_d - V_d) / 360.0) - 0.5);
                if (YY_d <= 0.25)
                {
                    U_d += 180.0;
                }
                U_d += n_d;

                double WW_d = q_d * (1.0 - k_d * FNCS(O_d));
                double W_d = q_d * (1.0 - k_d * FNCS(O_d))
                            * FNCS(FNASN(FNSN(l_d) * FNSN(V_d) + DHL_pur * RAD_TO_DEG));

                X_d = H_d + FNAT(FNSN(U_d - H_d) * W_d / (J_d + W_d * FNCS(U_d - H_d)));
                X_d = FNR(X_d, 360.0);

                double TS_d = FNCS(U_d - H_d)
                             + (WW_d * J_d + (WW_d + J_d) * Math.Sqrt(WW_d * J_d) * W_d / WW_d)
                               / (WW_d * WW_d + J_d * J_d + WW_d * J_d);

                YY_d = Math.Abs(FNFR((X_d - U_d) / 360.0) - 0.5);
                if (YY_d <= 0.25)
                {
                    X_d += 180.0;
                }

                // Determine if planet is retrograde
                if (TS_d > 0)
                {
                    isRetrograde[Panchgrahi + 3] = false;
                }
                else
                {
                    isRetrograde[Panchgrahi + 3] = true;
                }

                // Geocentric longitude (nirayana)
                dPlt[Panchgrahi + 3] = FNR(X_d - dAyanamsa, 360.0);
                dPltS[Panchgrahi + 3] = FNQ(dPlt[Panchgrahi + 3]);

                // ======== LAGNA & SIDEREAL TIME (computed inside the loop, after last planet) ========
                if (Panchgrahi == 5)
                {
                    double dSiderealTime = 16.6981583332
                                           + 15.0 * dBirthTimeIST
                                           + dLong
                                           + dJDF * 0.985647335386
                                           + dJDF * dJDF / 3446481700000.0;
                    dSiderealTime = FNR(dSiderealTime, 360.0);
                    double dSiderealTimHMS = FNDM(dSiderealTime / 15.0);

                    // Ascendant (Lagna) - Nirayana
                    dPlt[1] = FNAT(0.9174 * FNTN(dSiderealTime)
                                    + 0.398 * FNTN(dLatt) / FNCS(dSiderealTime))
                              + 90.0 + dSiderealTime - FNAT(FNTN(dSiderealTime));
                    dPlt[1] = FNR(dPlt[1] - dAyanamsa, 360.0);

                    // Midheaven (MC / Dasham Bhava) - Nirayana
                    dPlt[11] = FNAT(1.09 * FNTN(dSiderealTime))
                               + dSiderealTime - FNAT(FNTN(dSiderealTime));
                    dPlt[11] = FNR(dPlt[11] - dAyanamsa, 360.0);

                    dPltS[1] = FNQ(dPlt[1]);
                    dPltS[11] = FNQ(dPlt[11]);

                    // Store sidereal time for result
                    _siderealTime = dSiderealTime;
                    _siderealTimeHMS = dSiderealTimHMS;
                }
            } // End for loop (Panchgrahi 1..5)

            // ======== MOON POSITION ========
            _abbqq = ABBQQ_d;
            _nutqq = NUTQQ_d;
            dPlt[3] = GetMoon(dJDF);
            dPlt[3] = FNR(dPlt[3] - dAyanamsa, 360.0);
            dPltS[3] = FNQ(dPlt[3]);

            // ======== SUN (Nirayana) ========
            dPlt[2] = FNR(dPlt[2] - dAyanamsa, 360.0);
            dPltS[2] = FNQ(dPlt[2]);

            // ======== RAHU (Nirayana) ========
            dPlt[9] = FNR(RHM_d - dAyanamsa, 360.0);
            dPltS[9] = FNQ(dPlt[9]);

            // ======== KETU (Nirayana, always opposite Rahu) ========
            dPlt[10] = FNR(dPlt[9] + 180.0, 360.0);
            dPltS[10] = FNQ(dPlt[10]);

            // ======== BUILD RESULT ========
            return BuildResult(dPlt, dPltS, dAyanamsa, isRetrograde);
        }

        // Instance fields for sidereal time (set during calculation)
        private double _siderealTime;
        private double _siderealTimeHMS;

        // -----------------------------------------------------------------
        // BuildResult: Convert raw arrays into a structured KundliResult
        // -----------------------------------------------------------------
        private KundliResult BuildResult(double[] dPlt, double[] dPltS, double dAyanamsa, bool[] isRetrograde)
        {
            var result = new KundliResult();
            result.Ayanamsa = Math.Round(dAyanamsa, 6);
            result.SiderealTime = Math.Round(_siderealTime, 6);
            result.SiderealTimeHMS = Math.Round(_siderealTimeHMS, 6);

            // Planet names (index 1..10)
            string[] names = { "", "Lagna", "Sun", "Moon", "Mars", "Mercury", "Jupiter", "Venus", "Saturn", "Rahu", "Ketu" };

            // Compute rashi signs for each planet (index 1..10)
            int[] iRasi = new int[12];
            for (int i = 1; i <= 10; i++)
            {
                iRasi[i] = FNTGR(dPltS[i]) + 1;
            }

            // Build PlanetPosition for each celestial body
            result.Lagna = MakePlanetPosition("Lagna", dPlt[1], dPltS[1], iRasi[1], false, iRasi);
            result.Sun = MakePlanetPosition("Sun", dPlt[2], dPltS[2], iRasi[2], false, iRasi);
            result.Moon = MakePlanetPosition("Moon", dPlt[3], dPltS[3], iRasi[3], false, iRasi);
            result.Mars = MakePlanetPosition("Mars", dPlt[4], dPltS[4], iRasi[4], isRetrograde[4], iRasi);
            result.Mercury = MakePlanetPosition("Mercury", dPlt[5], dPltS[5], iRasi[5], isRetrograde[5], iRasi);
            result.Jupiter = MakePlanetPosition("Jupiter", dPlt[6], dPltS[6], iRasi[6], isRetrograde[6], iRasi);
            result.Venus = MakePlanetPosition("Venus", dPlt[7], dPltS[7], iRasi[7], isRetrograde[7], iRasi);
            result.Saturn = MakePlanetPosition("Saturn", dPlt[8], dPltS[8], iRasi[8], isRetrograde[8], iRasi);
            result.Rahu = MakePlanetPosition("Rahu", dPlt[9], dPltS[9], iRasi[9], false, iRasi);
            result.Ketu = MakePlanetPosition("Ketu", dPlt[10], dPltS[10], iRasi[10], false, iRasi);
            result.MC = MakePlanetPosition("MC", dPlt[11], dPltS[11], FNTGR(dPltS[11]) + 1, false, iRasi);

            // ======== HOUSE DATA (equivalent to Kundli_Graph) ========
            result.Houses = new List<HouseData>();
            int lagRasi = iRasi[1]; // Lagna's rashi (1-12)
            int currentSign = lagRasi;

            for (int H = 1; H <= 12; H++)
            {
                var house = new HouseData
                {
                    HouseNumber = H,
                    Rashi = currentSign,
                    Planets = new List<string>()
                };

                // Check which planets are in this house's sign
                for (int planetIdx = 2; planetIdx <= 10; planetIdx++)
                {
                    if (iRasi[planetIdx] == currentSign)
                    {
                        house.Planets.Add(PlanetAbbreviations[planetIdx]);
                    }
                }

                // For house 1, also add "LAG" (Lagna is always in house 1)
                if (H == 1)
                {
                    house.Planets.Insert(0, "LAG");
                }

                result.Houses.Add(house);

                // Advance to next sign
                currentSign++;
                if (currentSign == 13) currentSign = 1;
            }

            // Raw FNQ positions for debugging/compatibility
            result.RawPositions = new Dictionary<string, double>
            {
                { "Lagna", Math.Round(dPltS[1], 6) },
                { "Sun", Math.Round(dPltS[2], 6) },
                { "Moon", Math.Round(dPltS[3], 6) },
                { "Mars", Math.Round(dPltS[4], 6) },
                { "Mercury", Math.Round(dPltS[5], 6) },
                { "Jupiter", Math.Round(dPltS[6], 6) },
                { "Venus", Math.Round(dPltS[7], 6) },
                { "Saturn", Math.Round(dPltS[8], 6) },
                { "Rahu", Math.Round(dPltS[9], 6) },
                { "Ketu", Math.Round(dPltS[10], 6) }
            };

            return result;
        }

        // -----------------------------------------------------------------
        // MakePlanetPosition: Helper to create a PlanetPosition from raw data
        // -----------------------------------------------------------------
        private PlanetPosition MakePlanetPosition(string name, double totalDegrees, double fnqValue, int rashi, bool retrograde, int[] allRasis)
        {
            // Compute degrees within rashi
            double degreesInRashi = totalDegrees % 30.0;
            if (degreesInRashi < 0) degreesInRashi += 30.0;

            // Compute house placement relative to Lagna
            int lagnaRasi = allRasis[1]; // Lagna's rashi number
            int house = rashi - lagnaRasi + 1;
            if (house <= 0) house += 12;

            return new PlanetPosition
            {
                Name = name,
                Rashi = rashi,
                DegreesInRashi = Math.Round(degreesInRashi, 6),
                TotalDegrees = Math.Round(totalDegrees, 6),
                House = house,
                IsRetrograde = retrograde
            };
        }
    }
}
