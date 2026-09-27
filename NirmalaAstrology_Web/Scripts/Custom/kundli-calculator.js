/**
 * NIRMALA ASTROLOGY - Kundli Calculation Engine
 * Extracted from Indu.html - All astronomical calculations preserved exactly.
 * Calculates: Sun, Moon, Mars, Mercury, Jupiter, Venus, Saturn, Rahu, Ketu, Ascendant
 * Uses Kepler equations, perturbations, and Julian Day calculations.
 *
 * Global outputs after calling calculateKundli(day, month, year, hours, minutes, longitude, latitude):
 *   window.kundliData = {
 *     dPltS: [null, lagna, sun, moon, mars, mercury, jupiter, venus, saturn, rahu, ketu],
 *     dPlt: [null, ...degrees...],
 *     houses: [{rasi: number, planets: [{abbr, name, vakri}]}],
 *     ayanamsa: number
 *   }
 */

// ============================================================
// GLOBAL STATE (preserved from original)
// ============================================================
var dPltS = new Array(11);
var dPlt = new Array(11);
var dAyanamsa = 0;
var ABBQQ_d = 0;
var NUTQQ_d = 0;

// ============================================================
// MATH HELPER FUNCTIONS (preserved exactly from original)
// ============================================================
var dRCN = 57.2957795;
var dRADS = 0.0174532925199433;

function FNSN(dX) { return Math.sin(dX / 57.2957795); }
function FNCS(dX) { return Math.cos(dX / 57.2957795); }
function FNTN(dX) { return Math.tan(dX / 57.2957795); }
function FNAT(dX) { return Math.atan(dX) * 57.2957795; }
function FNRAD(dX) { return dX * 0.01745329252; }
function FNASN(dX) { return 57.2957795 * Math.atan(dX / Math.sqrt(1 - dX * dX)); }

function FNFR(dX) { return dX - Math.floor(dX); }

function FNR(dX, dCY) { return FNFR(FNFR(dX / dCY) + 2) * dCY; }

function FNDG(dX) { return (250 * dX + 0.00000005 - 60 * parseInt(dX + 0.00000005) - parseInt(100 * dX + 0.00000005)) / 90; }
function FNDM(dX) { return (90 * dX + 0.00000005 + 100 * parseInt(dX + 0.00000005) + parseInt(60 * dX + 0.00000005)) / 250; }
function FNTGR(dX) { return parseInt(dX + 0.0000005); }

function FNQ(dX) { return (90 * (dX + 0.00003) + 17500 * parseInt((dX + 0.00003) / 30) + 100 * parseInt((dX + 0.00003)) + parseInt((dX + 0.00003) * 60)) / 25000; }
function FNUNW(dX) { dX = dX - parseInt(dX / 6.2831853072) * 6.2831853072; return dX; }

function julday(day, month, year, hour) {
    if (month < 3) year -= 1;
    if (month < 3) month += 12;
    var leep = Math.floor(year / 100);
    leep = 2 - leep + Math.floor(leep / 4);
    return Math.floor(365.2501 * year) + Math.floor((month + 1) * 30.6001) - 694025 + leep + day + (hour) / 24;
}

function roundNumber(num, dec) { return Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec); }

// ============================================================
// MOON CALCULATION (preserved exactly)
// ============================================================
function getmoon(dJDF) {
    var TQQ_d = (dJDF - 0.5) / 36525;
    var DJDQQ_d = dJDF - 0.5;
    var T2QQ_d = TQQ_d * TQQ_d;
    var M1QQ_d = 27.32158213;
    var M2QQ_d = 365.2596407;
    var M3QQ_d = 27.55455094;
    var M4QQ_d = 29.53058868;
    var M5QQ_d = 27.21222039;
    var M6QQ_d = 6798.363307;
    M1QQ_d = DJDQQ_d / 27.32158213;
    M2QQ_d = DJDQQ_d / 365.2596407;
    M3QQ_d = DJDQQ_d / 27.55455094;
    M4QQ_d = DJDQQ_d / 29.53058868;
    M5QQ_d = DJDQQ_d / 27.21222039;
    M6QQ_d = DJDQQ_d / 6798.363307;
    M1QQ_d = 360 * (M1QQ_d - parseInt(M1QQ_d));
    M2QQ_d = 360 * (M2QQ_d - parseInt(M2QQ_d));
    M3QQ_d = 360 * (M3QQ_d - parseInt(M3QQ_d));
    M4QQ_d = 360 * (M4QQ_d - parseInt(M4QQ_d));
    M5QQ_d = 360 * (M5QQ_d - parseInt(M5QQ_d));
    M6QQ_d = 360 * (M6QQ_d - parseInt(M6QQ_d));
    var LDQQ_d = 270.434164 + M1QQ_d - (0.001133 - 0.0000019 * TQQ_d) * T2QQ_d;
    var MSQQ_d = 358.475833 + M2QQ_d - (0.00015 + 0.0000033 * TQQ_d) * T2QQ_d;
    var MDQQ_d = 296.104608 + M3QQ_d + (0.009192 + 0.0000144 * TQQ_d) * T2QQ_d;
    var DEQQ_d = 350.737486 + M4QQ_d - (0.001436 - 0.0000019 * TQQ_d) * T2QQ_d;
    var FQQ_d = 11.250889 + M5QQ_d - (0.003211 + 0.0000003 * TQQ_d) * T2QQ_d;
    var NQQ_d = 259.183275 - M6QQ_d + (0.002078 + 0.000022 * TQQ_d) * T2QQ_d;
    var AQQ_d = FNRAD(51.2 + 20.2 * TQQ_d);
    var SAQQ_d = Math.sin(AQQ_d);
    var SNQQ_d = Math.sin(FNRAD(NQQ_d));
    var BQQ_d = 346.56 + (132.87 - 0.0091731 * TQQ_d) * TQQ_d;
    var SBQQ_d = 0.003964 * Math.sin(FNRAD(BQQ_d));
    var CQQ_d = FNRAD(NQQ_d + 275.05 - 2.3 * TQQ_d);
    var SCQQ_d = Math.sin(CQQ_d);
    LDQQ_d = LDQQ_d + 0.000233 * SAQQ_d + SBQQ_d + 0.001964 * SNQQ_d;
    MSQQ_d = MSQQ_d - 0.001778 * SAQQ_d;
    MDQQ_d = MDQQ_d + 0.000817 * SAQQ_d + SBQQ_d + 0.002541 * SNQQ_d;
    FQQ_d = FQQ_d + SBQQ_d - 0.024691 * SNQQ_d - 0.004328 * SCQQ_d;
    DEQQ_d = DEQQ_d + 0.002011 * SAQQ_d + SBQQ_d + 0.001964 * SNQQ_d;
    var EQQ_d = 1 - (0.002495 + 0.00000752 * TQQ_d) * TQQ_d;
    var E2QQ_d = EQQ_d * EQQ_d;
    LDQQ_d = FNRAD(LDQQ_d);
    MSQQ_d = FNRAD(MSQQ_d);
    NQQ_d = FNRAD(NQQ_d);
    DEQQ_d = FNRAD(DEQQ_d);
    FQQ_d = FNRAD(FQQ_d);
    MDQQ_d = FNRAD(MDQQ_d);
    var LQQ_d = 6.28875 * Math.sin(MDQQ_d) + 1.274018 * Math.sin(2 * DEQQ_d - MDQQ_d);
    LQQ_d = LQQ_d + 0.658309 * Math.sin(2 * DEQQ_d) + 0.213616 * Math.sin(2 * MDQQ_d) - EQQ_d * 0.185596 * Math.sin(MSQQ_d) - 0.114336 * Math.sin(2 * FQQ_d) + 0.058793 * Math.sin(2 * (DEQQ_d - MDQQ_d)) + 0.057212 * EQQ_d * Math.sin(2 * DEQQ_d - MSQQ_d - MDQQ_d) + 0.05332 * Math.sin(2 * DEQQ_d + MDQQ_d);
    LQQ_d = LQQ_d + 0.045874 * EQQ_d * Math.sin(2 * DEQQ_d - MSQQ_d) + 0.041024 * EQQ_d * Math.sin(MDQQ_d - MSQQ_d) - 0.034718 * Math.sin(DEQQ_d) - EQQ_d * 0.030465 * Math.sin(MSQQ_d + MDQQ_d);
    LQQ_d = LQQ_d + 0.015326 * Math.sin(2 * (DEQQ_d - FQQ_d)) - 0.012528 * Math.sin(2 * FQQ_d + MDQQ_d) - 0.01098 * Math.sin(FQQ_d - MDQQ_d);
    LQQ_d = LQQ_d + 0.010674 * Math.sin(4 * DEQQ_d - MDQQ_d) + 0.010034 * Math.sin(3 * MDQQ_d) + 0.008548 * Math.sin(4 * DEQQ_d - 2 * MDQQ_d);
    LQQ_d = LQQ_d - EQQ_d * 0.00791 * Math.sin(MSQQ_d - MDQQ_d + 2 * DEQQ_d) - EQQ_d * 0.006783 * Math.sin(2 * DEQQ_d + MSQQ_d);
    LQQ_d = LQQ_d + 0.005162 * Math.sin(MDQQ_d - DEQQ_d) + EQQ_d * 0.005 * Math.sin(MSQQ_d + DEQQ_d) + 0.003862 * Math.sin(4 * DEQQ_d);
    LQQ_d = LQQ_d + EQQ_d * 0.004049 * Math.sin(MDQQ_d - MSQQ_d + 2 * DEQQ_d) + 0.003996 * Math.sin(2 * (MDQQ_d + DEQQ_d));
    LQQ_d = LQQ_d + 0.003665 * Math.sin(2 * DEQQ_d - 3 * MDQQ_d) + EQQ_d * 0.002695 * Math.sin(2 * MDQQ_d - MSQQ_d);
    LQQ_d = LQQ_d + 0.002602 * Math.sin(MDQQ_d - 2 * (FQQ_d + DEQQ_d)) + EQQ_d * 0.002396 * Math.sin(2 * (DEQQ_d - MDQQ_d) - MSQQ_d);
    LQQ_d = LQQ_d - 0.002349 * Math.sin(MDQQ_d + DEQQ_d) + E2QQ_d * 0.002249 * Math.sin(2 * (DEQQ_d - MSQQ_d)) - EQQ_d * 0.002125 * Math.sin(2 * MDQQ_d + MSQQ_d);
    LQQ_d = LQQ_d - E2QQ_d * 0.002079 * Math.sin(2 * MSQQ_d) + E2QQ_d * 0.002059 * Math.sin(2 * (DEQQ_d - MSQQ_d) - MDQQ_d);
    LQQ_d = LQQ_d - 0.001773 * Math.sin(MDQQ_d + 2 * (DEQQ_d - FQQ_d)) - 0.001595 * Math.sin(2 * (FQQ_d + DEQQ_d));
    LQQ_d = LQQ_d + EQQ_d * 0.00122 * Math.sin(4 * DEQQ_d - MSQQ_d - MDQQ_d) - 0.00111 * Math.sin(2 * (MDQQ_d + FQQ_d));
    LQQ_d = LQQ_d + 0.000892 * Math.sin(MDQQ_d - 3 * DEQQ_d) - EQQ_d * 0.000811 * Math.sin(MSQQ_d + MDQQ_d + 2 * DEQQ_d);
    LQQ_d = LQQ_d + EQQ_d * 0.000761 * Math.sin(4 * DEQQ_d - MSQQ_d - 2 * MDQQ_d) + E2QQ_d * 0.000704 * Math.sin(MDQQ_d - 2 * (MSQQ_d + DEQQ_d));
    LQQ_d = LQQ_d + EQQ_d * 0.000693 * Math.sin(MSQQ_d - 2 * (MDQQ_d - DEQQ_d)) + EQQ_d * 0.000598 * Math.sin(2 * (DEQQ_d - FQQ_d) - MSQQ_d);
    LQQ_d = LQQ_d + 0.00055 * Math.sin(MDQQ_d + 4 * DEQQ_d) + 0.000538 * Math.sin(4 * MDQQ_d);
    LQQ_d = LQQ_d + EQQ_d * 0.000521 * Math.sin(4 * DEQQ_d - MSQQ_d) + 0.000486 * Math.sin(2 * MDQQ_d - DEQQ_d);
    LQQ_d = LQQ_d + E2QQ_d * 0.000717 * Math.sin(MDQQ_d - 2 * MSQQ_d);
    var LAMQQ_d = LDQQ_d + FNRAD(LQQ_d);
    var TPIQQ_d = 6.283185308;
    while (LAMQQ_d < 0) { LAMQQ_d = LAMQQ_d + TPIQQ_d; }
    while (LAMQQ_d > TPIQQ_d) { LAMQQ_d = LAMQQ_d - TPIQQ_d; }
    dPlt[3] = FNR(LAMQQ_d * 360 / TPIQQ_d + ABBQQ_d + NUTQQ_d, 360);
    return dPlt[3];
}

// ============================================================
// MAIN KEPLER CALCULATION (preserved exactly from original)
// ============================================================
function Kepler(Dey, Manth, Yeer, dBirthTime, dLong, dLatt) {
    var dJDF = julday(Dey, Manth, Yeer, dBirthTime);
    var RHM_d, YQQ_d, MAS, BQQ_d, MSNQQ_d, RHUQQ_d;
    var MQQ_d, XQQ_d, XLQQ_d, MMQQ_d, XXQQ_d, DLQQ_d;
    var PSLQQ_d, VQQ_d, QVQQ_d, PSVQQ_d;
    var QJQQ_d, PSJQQ_d, PSMQQ_d;
    var QMQQ_d, DRJQQ_d, QCQQ_d, DRCQQ_d, DRQQ_d;
    var MSAQQ_d, DRVQQ_d, PSCQQ_d;
    var MAP = new Array(6);
    var M1X_d = new Array(6);
    var iVM = new Array(10);
    var sVakMar = new Array();
    var dLTP = new Array(12);

    for (var i = 0; i <= 5; i++) {
        MAP[i] = 0; dPlt[i] = 0; dPltS[i] = 0; M1X_d[i] = 0; dPltS[i] = 0; dLTP[i] = 0; iVM[i] = 0; sVakMar[i] = 0;
    }

    var M_d, P_d, M1_d, P1_d, B_d, M2_d, M3_d, N_d, N2_d, N3_d;
    var K_d, K1_d, K2_d, K3_d, L_d, L1_d, L2_d, L3_d;
    var Z, R_d, H_d, J_d, a_d, Q_d, O_d;
    var DML_pur, DM_pur, DL_pur, DR_pur, DS_pur, DP_pur, DA_pur, DHL_pur;

    // Calculate Rahu and Ayanamsa
    RHM_d = FNR((259.2097527 - 1934.138634 * dJDF / 36525), 360);
    dAyanamsa = 22.462056 + dJDF / 26163.259 - 17.23 / 3600 * FNSN(RHM_d) + 0.21 / 3600 * FNSN(2 * RHM_d) - 3.142 / 3600;

    BQQ_d = dJDF / 36525.0;
    MSNQQ_d = 279.2038555 + (129602768.13 / 3600) * BQQ_d + 1.089 / 3600 * BQQ_d * BQQ_d;
    MSAQQ_d = 357.9830444 + (129596579.1 / 3600) * BQQ_d - (0.541 / 3600) * BQQ_d * BQQ_d - (0.012 / 3600) * BQQ_d * BQQ_d * BQQ_d;
    RHUQQ_d = 259.2097527 - (6962911.23 / 3600) * BQQ_d + 7.48 / 3600 * BQQ_d * BQQ_d;
    ABBQQ_d = 1 * (-20.5 - 0.34 * FNCS(MSAQQ_d)) / 3600;
    NUTQQ_d = -1.27 * FNSN(2 * MSNQQ_d) + 0.13 * FNSN(MSAQQ_d) - 0.05 * FNSN(3 * MSNQQ_d + 79) - 17.23 * FNSN(RHUQQ_d) + 0.21 * FNSN(2 * RHUQQ_d);
    NUTQQ_d = 1 * NUTQQ_d / 3600;
    MAS = MSAQQ_d;

    MQQ_d = FNR(MSAQQ_d, 360);
    MAS = MQQ_d;
    XQQ_d = 6910.057 * FNSN(MQQ_d) + 72.338 * FNSN(2 * MQQ_d) + 1.054 * FNSN(3 * MQQ_d) + 0.018 * FNSN(4 * MQQ_d);
    XLQQ_d = -17.24 * (BQQ_d + 0.003 * BQQ_d * BQQ_d) * FNSN(MQQ_d) - 0.361 * BQQ_d * FNSN(2 * MQQ_d);
    XQQ_d = (XQQ_d + XLQQ_d) / 3600;
    MMQQ_d = MQQ_d;
    XXQQ_d = XQQ_d;
    YQQ_d = MSNQQ_d + XQQ_d;

    BQQ_d = dJDF / 36525;
    PSLQQ_d = 6.4 * FNSN(231.19 + 20.2 * BQQ_d) + (1.882 - 0.016 * BQQ_d) * FNSN(57.24 + 150.27 * BQQ_d) + 0.266 * FNSN(31.8 + 119 * BQQ_d) + 0.202 * FNSN(315.6 + 893 * BQQ_d);
    VQQ_d = FNDG(62.4544361) + (81066394.75 / 3600) * BQQ_d + (0.02581 / 3600) * BQQ_d * BQQ_d;
    QVQQ_d = VQQ_d + 180;
    PSVQQ_d = 4.84 * FNSN(QVQQ_d) - 5.53 * FNSN(2 * QVQQ_d) - 0.67 * FNSN(3 * QVQQ_d) - 0.21 * FNSN(4 * QVQQ_d) - 0.12 * FNSN(2 * QVQQ_d + MSAQQ_d) - 2.5 * FNSN(MSAQQ_d + 12 - 2 * QVQQ_d) - 1.56 * FNSN(MSAQQ_d + 12 - 3 * QVQQ_d) + 0.14 * FNSN(MSAQQ_d + 12 - 4 * QVQQ_d) - 1.02 * FNSN(2 * MSAQQ_d + 40 - 3 * QVQQ_d);
    PSVQQ_d = PSVQQ_d - 0.15 * FNSN(2 * MSAQQ_d + 40 - 4 * QVQQ_d) + 0.12 * FNSN(2 * MSAQQ_d + 40 - 5 * QVQQ_d) - 0.15 * FNSN(MSAQQ_d + 56 - 5 * QVQQ_d);
    QJQQ_d = 180 - 41.114619 - (118672080.98 / 3600) * BQQ_d + (0.11613) * BQQ_d * BQQ_d;
    PSJQQ_d = 7.21 * FNSN(QJQQ_d - 1.083333) - 2.73 * FNSN(12 * QJQQ_d - 0.25) - 0.16 * FNSN(3 * QJQQ_d + 4.85) + 2.6 * FNSN(QJQQ_d + MSAQQ_d + 85.7667) - 1.67 * FNSN(2 * QJQQ_d + MSAQQ_d - 22.6);
    QMQQ_d = 195.8056 + 191.4048157 * BQQ_d * 100 - MSAQQ_d + 5.29;
    PSMQQ_d = 2.04 * FNSN(2 * QMQQ_d + 0.25) + 0.27 * FNSN(QMQQ_d - 0.6) - 1.77 * FNSN(2 * QMQQ_d + MSAQQ_d - 36.25167) - 0.58 * FNSN(4 * QMQQ_d + 2 * MSAQQ_d + 84) - 0.5 * FNSN(4 * QMQQ_d + MSAQQ_d - 47) - 0.43 * FNSN(3 * QMQQ_d + MSAQQ_d - 47.7);
    PSMQQ_d = PSMQQ_d - 0.43 * FNSN(3 * QMQQ_d + MSAQQ_d - 47.7);
    QCQQ_d = FNDG(344.38316) + (1602961611.18 / 3600) * BQQ_d - (5.169 / 3600) * BQQ_d * BQQ_d + (0.0068 / 3600) * BQQ_d * BQQ_d * BQQ_d;
    PSCQQ_d = 6.44 * FNSN(QCQQ_d) - 0.42 * FNSN(QCQQ_d - MSAQQ_d);
    DLQQ_d = (PSLQQ_d + PSMQQ_d + PSJQQ_d + PSVQQ_d + PSCQQ_d) / 3600;
    DRVQQ_d = -1.12 * FNCS(QVQQ_d) + 3.25 * FNCS(2 * QVQQ_d + 0.1167) + 0.5 * FNCS(3 * QVQQ_d - 1.0833) + 0.18 * FNCS(4 * QVQQ_d - 0.24) + 0.08 * FNCS(5 * QVQQ_d - 3);
    DRJQQ_d = 3.36 * FNCS(QJQQ_d - 1.1) - 1.91 * FNCS(2 * QJQQ_d - 0.2167) - 0.13 * FNCS(3 * QJQQ_d + 4.35);
    DRCQQ_d = 6.35 * FNCS(QCQQ_d);
    DRQQ_d = DRVQQ_d + DRJQQ_d + DRCQQ_d;
    DRQQ_d = DRQQ_d / (dRCN * 3600);
    YQQ_d = YQQ_d + NUTQQ_d + ABBQQ_d + DLQQ_d;
    YQQ_d = FNR(YQQ_d, 360);

    Y_d = YQQ_d;
    M_d = FNFR(2.994397 + dJDF / 365.25964) * 360;
    X_d = 1.9142998 * FNSN(M_d) + FNSN(2 * M_d) / 50.0290666 + FNSN(3 * M_d) / 3455.396;
    XX_d = X_d;
    MM_d = M_d;
    dPlt[2] = FNR(Y_d, 360);

    R_d = 0.99972091 / (1 + 0.016706 * FNCS(MM_d + XX_d)) + DRQQ_d;
    H_d = Y_d; J_d = R_d; a_d = M_d;
    B_d = (dJDF - 0.5) / 36525;

    // Loop through 5 planets: Mars(1), Mercury(2), Jupiter(3), Venus(4), Saturn(5)
    for (var Panchgrahi = 1; Panchgrahi <= 5; Panchgrahi++) {
        switch (Panchgrahi) {
            case 1: M_d = 293.737334; M1_d = 53.17137642; M2_d = 0.0003107; M3_d = 0; P_d = 334.218203; P1_d = 1.8407584; P2_d = 0.0001299; P3_d = -0.00000119; K_d = 0.0933129; K1_d = 0.000092064; K2_d = 0.000000077; K3_d = 0; L_d = 1.850333; L1_d = -0.000675; L2_d = 0.0000126; L3_d = 0; N_d = 48.786442; N1_d = 0.7709917; N2_d = -0.0000014; N3_d = -0.00000533; Q_d = 1.5236883; break;
            case 2: M_d = 178.179078; M1_d = 415.2057519; M2_d = 0.0003011; M3_d = 0; P_d = 75.899697; P1_d = 1.5554889; P2_d = 0.0002947; P3_d = 0; K_d = 0.20561421; K1_d = 0.00002046; K2_d = 0.00000003; K3_d = 0; L_d = 7.002881; L1_d = 0.0018608; L2_d = -0.0000183; L3_d = 0; N_d = 47.145944; N1_d = 1.1852083; N2_d = 0.0001739; N3_d = 0; Q_d = 0.3870986; break;
            case 3: M_d = 238.049257; M1_d = 8.434172183; M2_d = 0.0003347; M3_d = -0.00000165; P_d = 12.720972; P1_d = 1.6099617; P2_d = 0.00105627; P3_d = -0.00000343; K_d = 0.04833475; K1_d = 0.00016418; K2_d = -0.0000004676; K3_d = -0.0000000017; L_d = 1.308736; L1_d = -0.0056961; L2_d = 0.0000039; L3_d = 0; N_d = 99.443414; N1_d = 1.01053; N2_d = 0.00035222; N3_d = -0.00000851; Q_d = 5.202561; break;
            case 4: M_d = 342.767053; M1_d = 162.5533664; M2_d = 0.0003097; M3_d = 0; P_d = 130.163833; P1_d = 1.4080361; P2_d = -0.0009764; P3_d = 0; K_d = 0.00682069; K1_d = -0.00004774; K2_d = 0.00000009099999; K3_d = 0; L_d = 3.393631; L1_d = 0.0010058; L2_d = -0.000001; L3_d = 0; N_d = 75.779647; N1_d = 0.89985; N2_d = 0.00041; N3_d = 0; Q_d = 0.7233316; break;
            case 5: M_d = 266.564377; M1_d = 3.398638567; M2_d = 0.0003245; M3_d = -0.0000058; P_d = 91.098214; P1_d = 1.9584158; P2_d = 0.00082636; P3_d = 0.00000461; K_d = 0.05589232; K1_d = -0.0003455; K2_d = -0.000000728; K3_d = 0; L_d = 0.492519; L1_d = -0.0039189; L2_d = -0.00001549; L3_d = 0.00000004; N_d = 112.790414; N1_d = 0.8731951; N2_d = -0.00015218; N3_d = -0.00000531; Q_d = 9.554747; break;
        }
        M_d = (M_d - P_d) + B_d * (360 * M1_d - P1_d) + B_d * B_d * (M2_d - P2_d) + B_d * B_d * B_d * (M3_d - P3_d);
        M1X_d[Panchgrahi] = M_d;
        P_d = (P_d - N_d) + B_d * (P1_d - N1_d) + B_d * B_d * (P2_d - N2_d) + B_d * B_d * B_d * (P3_d - N3_d);
        N_d = N_d + N1_d * B_d + N2_d * B_d * B_d + N3_d * B_d * B_d * B_d;
        K_d = K_d + K1_d * B_d + K2_d * B_d * B_d + K3_d * B_d * B_d * B_d;
        L_d = L_d + L1_d * B_d + L2_d * B_d * B_d + L3_d * B_d * B_d * B_d;
        MAP[1] = M1X_d[2]; MAP[2] = M1X_d[4]; MAP[3] = M1X_d[1]; MAP[4] = M1X_d[3]; MAP[5] = M1X_d[5];
        DL_pur = 0; DML_pur = 0; DM_pur = 0; DHL_pur = 0; DA_pur = 0; DR_pur = 0; DS_pur = 0;

        switch (Panchgrahi) {
            case 1: // Mars perturbations
                a = 3 * MAP[4] - 8 * MAP[3] + 4 * MAS; SA = Math.sin(a); CA = Math.cos(a);
                DML_pur = -1 * (0.01133 * SA + 0.00933 * CA); DML_pur = FNRAD(DML_pur); DM_pur = DML_pur;
                DL_pur = 0.00705 * Math.cos(MAP[4] - MAP[3] - 0.85448) + 0.00607 * Math.cos(2 * MAP[4] - MAP[3] - 0.85448) + 0.00445 * Math.cos(2 * MAP[4] - 2 * MAP[3] - 3.3492) + 0.00388 * Math.cos(MAS - 2 * MAP[3] + 0.35771) + 0.00238 * Math.cos(MAS - MAP[3] + 0.61256) + 0.00204 * Math.cos(2 * MAS - 3 * MAP[3] + 2.7688) + 0.00177 * Math.cos(3 * MAP[3] - MAP[2] - 1.0053) + 0.00136 * Math.cos(2 * MAS - 4 * MAP[3] + 2.6894) + 0.00104 * Math.cos(MAP[4] + 0.30749);
                DR_pur = 0.000053227 * Math.cos(MAP[4] - MAP[3] + 0.00717864) + 0.000050989 * Math.cos(2 * MAP[4] - 2 * MAP[3] - 1.77997) + 0.000038278 * Math.cos(2 * MAP[4] - MAP[3] - 1.71617) + 0.000015996 * Math.cos(MAS - MAP[3] - 0.969618) + 0.000014764 * Math.cos(2 * MAS - 3 * MAP[3] + 1.16768) + 0.000008966 * Math.cos(MAP[4] - 2 * MAP[3] + 0.761225) + 0.000007914 * Math.cos(3 * MAP[4] - 2 * MAP[3] - 2.43887) + 0.000007004 * Math.cos(2 * MAP[4] - 3 * MAP[3] - 1.79573) + 0.00000662 * Math.cos(MAS - 2 * MAP[3] + 1.97575) + 0.00000493 * Math.cos(3 * MAP[4] - 3 * MAP[3] - 1.33069) + 0.000004693 * Math.cos(3 * MAS - 5 * MAP[3] + 3.32665) + 0.000004571 * Math.cos(2 * MAS - 4 * MAP[3] + 4.27086) + 0.000004409 * Math.cos(3 * MAP[4] - MAP[3] - 2.02158);
                break;
            case 2: // Mercury perturbations
                DL_pur = 0.00204 * Math.cos(5 * MAP[2] - 2 * MAP[1] + 0.21328) + 0.00103 * Math.cos(2 * MAP[2] - MAP[1] - 2.8046) + 0.00091 * Math.cos(2 * MAP[4] - MAP[1] - 0.64582) + 0.00078 * Math.cos(5 * MAP[2] - 3 * MAP[1] + 0.17692);
                DR_pur = 0.000007525 * Math.cos(2 * MAP[4] - MAP[1] + 0.925251) + 0.000006802 * Math.cos(5 * MAP[2] - 3 * MAP[1] - 4.53642) + 0.000005457 * Math.cos(2 * MAP[2] - 2 * MAP[1] - 1.24246) + 0.000003569 * Math.cos(5 * MAP[2] - MAP[1] - 1.35699);
                break;
            case 3: // Jupiter perturbations
                var T = dJDF / 36525, PLAN = ((-0.0000000017 * T - 0.0000004676) * T + 0.00016418) * T + 0.04833475, TPI = 6.2831853072;
                var J1 = T / 5 + 0.1, J2 = FNUNW(4.14473 + 52.9691 * T), J3 = FNUNW(4.641118 + 21.32991 * T), J4 = FNUNW(4.250177 + 7.478172 * T);
                var J5 = 5 * J3 - 2 * J2, J6 = 2 * J2 - 6 * J3 + 3 * J4, J7 = J3 - J2;
                var SJ3 = Math.sin(J3), CJ3 = Math.cos(J3), S2J3 = Math.sin(2 * J3), C2J3 = Math.cos(2 * J3);
                var SJ5 = Math.sin(J5), CJ5 = Math.cos(J5), S2J5 = Math.sin(2 * J5), SJ6 = Math.sin(J6);
                var SJ7 = Math.sin(J7), CJ7 = Math.cos(J7), S2J7 = Math.sin(2 * J7), C2J7 = Math.cos(2 * J7);
                var S3J7 = Math.sin(3 * J7), C3J7 = Math.cos(3 * J7), S4J7 = Math.sin(4 * J7), C4J7 = Math.cos(4 * J7), C5J7 = Math.cos(5 * J7);
                DML_pur = (0.331364 - (0.010281 + 0.004692 * J1) * J1) * SJ5 + (0.003228 - (0.06443601 - 0.002075 * J1) * J1) * CJ5 - (0.003083 + (0.000275 - 0.000489 * J1) * J1) * S2J5 + 0.002472 * SJ6 + 0.013619 * SJ7 + 0.018472 * S2J7 + 0.006717 * S3J7 + 0.002775 * S4J7 + 0.006417 * S2J7 * SJ3 + (0.007275 - 0.001253 * J1) * SJ7 * SJ3 + 0.002439 * S3J7 * SJ3 - (0.035681 + 0.001208 * J1) * SJ7 * CJ3 - 0.003767 * C2J7 * SJ3 - (0.033839 + 0.001125 * J1) * CJ7 * SJ3 - 0.004261 * S2J7 * CJ3 + (0.001161 * J1 - 0.006333) * CJ7 * CJ3 + 0.002178 * CJ3 - 0.006675 * C2J7 * CJ3 - 0.002664 * C3J7 * CJ3 - 0.002572 * SJ7 * S2J3 - 0.003567 * S2J7 * S2J3 + 0.002094 * CJ7 * C2J3 + 0.003342 * C2J7 * C2J3;
                DML_pur = FNRAD(DML_pur);
                DS_pur = (3606 + (130 - 43 * J1) * J1) * SJ5 + (1289 - 580 * J1) * CJ5 - 6764 * SJ7 * SJ3 - 1110 * S2J7 * SJ3 - 224 * S3J7 - SJ3 - 204 * SJ3 + (1284 + 116 * J1) * CJ7 * SJ3 + 188 * C2J7 * SJ3 + (1460 + 130 * J1) * SJ7 * CJ3 + 224 * S2J7 * CJ3 - 817 * CJ3 + 6074 * CJ3 * CJ7 + 992 * C2J7 * CJ3 + 508 * C3J7 * CJ3 + 230 * C4J7 * CJ3 + 108 * C5J7 * CJ3 - (956 + 73 * J1) * SJ7 * S2J3 + 448 * S2J7 * S2J3 + 137 * S3J7 * S2J3 + (108 * J1 - 997) * CJ7 * S2J3 + 480 * C2J7 * S2J3 + 148 * C3J7 * S2J3 + (99 * J1 - 956) * SJ7 * C2J3 + 490 * S2J7 * C2J3 + 158 * S3J7 * C2J3 + 179 * C2J3 + (1024 + 75 * J1) * CJ7 * C2J3 - 437 * C2J7 * C2J3 - 132 * C3J7 * C2J3;
                DS_pur = DS_pur * 0.0000001;
                DP_pur = (0.007192 - 0.003147 * J1) * SJ5 - 0.004344 * SJ3 + (J1 * (0.000197 * J1 - 0.000675) - 0.020428) * CJ5 + 0.034036 * CJ7 * SJ3 + (0.007269 + 0.000672 * J1) * SJ7 * SJ3 + 0.005614 * C2J7 * SJ3 + 0.002964 * C3J7 * SJ3 + 0.037761 * SJ7 * CJ3 + 0.006158 * S2J7 * CJ3 - 0.006603 * CJ7 * CJ3 - 0.005356 * SJ7 * S2J3 + 0.002722 * S2J7 * S2J3 + 0.004483 * CJ7 * S2J3 - 0.002642 * C2J7 * S2J3 + 0.004403 * SJ7 * C2J3 - 0.002536 * S2J7 * C2J3 + 0.005547 * CJ7 * C2J3 - 0.002689 * C2J7 * C2J3;
                DM_pur = DML_pur - FNRAD(DP_pur) / PLAN;
                DA_pur = 205 * CJ7 - 263 * CJ5 + 693 * C2J7 + 312 * C3J7 + 147 * C4J7 + 299 * SJ7 * SJ3 + 181 * C2J7 * SJ3 + 204 * S2J7 * CJ3 + 111 * S3J7 * CJ3 - 337 * CJ7 * CJ3 - 111 * C2J7 * CJ3;
                DA_pur = DA_pur * 0.000001;
                break;
            case 4: // Venus perturbations
                DML_pur = 0.00077 * Math.sin(4.1406 + T * 2.6227); DML_pur = FNRAD(DML_pur); DM_pur = DML_pur;
                DL_pur = 0.00313 * Math.cos(2 * MAS - 2 * MAP[2] - 2.587) + 0.00136 * Math.cos(MAS - MAP[2] - 2.0788) + 0.00096 * Math.cos(3 * MAS - 2 * MAP[2] - 2.3721) + 0.00082 * Math.cos(MAP[4] - MAP[2] - 3.6318);
                DR_pur = 0.000022501 * Math.cos(2 * MAS - 2 * MAP[2] - 1.01592) + 0.000019045 * Math.cos(3 * MAS - 3 * MAP[2] + 1.61577) + 0.000006887 * Math.cos(MAP[4] - MAP[2] - 2.06106) + 0.000005172 * Math.cos(MAS - MAP[2] - 0.508065) + 0.00000362 * Math.cos(5 * MAS - 4 * MAP[2] - 1.81877) + 0.000003283 * Math.cos(4 * MAS - 4 * MAP[2] + 1.10851) + 0.000003074 * Math.cos(2 * MAP[4] - 2 * MAP[2] - 0.962846);
                break;
            case 5: // Saturn perturbations
                T = dJDF / 36525; PLAN = ((0.00000000074 * T - 0.000000728) * T - 0.0003455) * T + 0.05589232; TPI = 6.2831853072;
                J1 = T / 5 + 0.1; J2 = FNUNW(4.14473 + 52.9691 * T); J3 = FNUNW(4.641118 + 21.32991 * T); J4 = FNUNW(4.250177 + 7.478172 * T);
                J5 = 5 * J3 - 2 * J2; J6 = 2 * J2 - 6 * J3 + 3 * J4; J7 = J3 - J2;
                SJ3 = Math.sin(J3); CJ3 = Math.cos(J3); S2J3 = Math.sin(2 * J3); C2J3 = Math.cos(2 * J3);
                SJ5 = Math.sin(J5); CJ5 = Math.cos(J5); S2J5 = Math.sin(2 * J5); SJ6 = Math.sin(J6);
                SJ7 = Math.sin(J7); CJ7 = Math.cos(J7); S2J7 = Math.sin(2 * J7); C2J7 = Math.cos(2 * J7);
                S3J7 = Math.sin(3 * J7); C3J7 = Math.cos(3 * J7); S4J7 = Math.sin(4 * J7); C4J7 = Math.cos(4 * J7); C5J7 = Math.cos(5 * J7);
                S3J3 = Math.sin(3 * J3); C3J3 = Math.cos(3 * J3); S4J3 = Math.sin(4 * J3); C4J3 = Math.cos(4 * J3);
                C2J5 = Math.cos(2 * J5); S5J7 = Math.sin(5 * J7); J8 = J4 - J3; S2J8 = Math.sin(2 * J8); C2J8 = Math.cos(2 * J8); S3J8 = Math.sin(3 * J8); C3J8 = Math.cos(3 * J8);
                DML_pur = 0.007581 * S2J5 - 0.007986 * SJ6 - 0.148811 * SJ7 - 0.040786 * S2J7 - (0.814181 - (0.01815 - 0.016714 * J1) * J1) * SJ5 - (0.010497 - (0.160906 - 0.0041 * J1) * J1) * CJ5 - 0.015208 * S3J7 - 0.006339 * S4J7 - 0.006244 * SJ3 - 0.0165 * S2J7 * SJ3 + (0.008931 + 0.002728 * J1) * SJ7 * SJ3 - 0.005775 * S3J7 * SJ3 + (0.081344 + 0.003206 * J1) * CJ7 * SJ3 + 0.015019 * C2J7 * SJ3 + (0.085581 + 0.002494 * J1) * SJ7 * CJ3 + 0.014394 * C2J7 * CJ3 + (0.025328 - 0.003117 * J1) * CJ7 * CJ3 + 0.006319 * C3J7 * CJ3 + 0.006369 * SJ7 * S2J3 + 0.009156 * S2J7 * S2J3 + 0.007525 * S3J8 * S2J3 - 0.005236 * CJ7 * C2J3 - 0.007736 * C2J7 * C2J3 - 0.007528 * C3J8 * C2J3;
                DML_pur = FNRAD(DML_pur);
                DS_pur = (-7927 + (2548 + 91 * J1) * J1) * SJ5 + (13381 + (1226 - 253 * J1) * J1) * CJ5 + (248 - 121 * J1) * S2J5 - (305 + 91 * J1) * C2J5 + 412 * S2J7 + 12415 * SJ3 + (390 - 617 * J1) * SJ7 * SJ3 + (165 - 204 * J1) * S2J7 * SJ3 + 26599 * CJ7 * SJ3 - 4687 * C2J7 * SJ3 - 1870 * C3J7 * SJ3 - 821 * C4J7 * SJ3 - 377 * C5J7 * SJ3 + 497 * C2J8 * SJ3 + (163 - 611 * J1) * CJ3 - 12696 * SJ7 * CJ3 - 4200 * S2J7 * CJ3 - 1503 * S3J7 * CJ3 - 619 * S4J7 * CJ3 - 268 * S5J7 * CJ3 - (282 + 1306 * J1) * CJ7 * CJ3 + (-86 + 230 * J1) * C2J7 * CJ3 + 461 * S2J8 + CJ3 - 350 * S2J3 + (2211 - 286 * J1) * SJ7 * S2J3 - 2208 * S2J7 * S2J3 - 568 * S3J7 * S2J3 - 346 * S4J7 * S2J3 - (2780 + 222 * J1) * CJ7 * S2J3 + (2022 + 263 * J1) * C2J7 * S2J3 + 248 * C3J7 * S2J3 + 242 * S3J8 * S2J3 + 467 * C3J8 * S2J3 - 490 * C2J3 - (2842 - 279 * J1) * SJ7 * C2J3 + (128 + 226 * J1) * S2J7 * C2J3 + 224 * S3J7 * C2J3 + (-1594 + 282 * J1) * CJ7 * C2J3 + (2162 - 207 * J1) * C2J7 * C2J3 + 561 * C3J7 * C2J3 + 343 * C4J7 * C2J3 + 469 * S3J8 * C2J3 - 242 * C3J8 * C2J3 - 205 * SJ7 * S3J3 + 262 * S3J7 * S3J3 + 208 * CJ7 * C3J3 - 271 * C3J7 * C3J3 - 382 * C3J7 * S4J3 - 376 * S3J7 * C4J3;
                DS_pur = DS_pur * 0.0000001;
                DA_pur = 572 * SJ5 - 1590 * S2J7 * CJ3 + 2933 * CJ5 - 647 * S3J7 * CJ3 + 33629 * CJ7 - 344 * S4J7 * CJ3 - 3081 * C2J7 + 2885 * CJ7 * CJ3 - 1423 * C3J7 + (2172 + 102 * J1) * C2J7 * CJ3 - 671 * C4J7 + 296 * C3J7 * CJ3 - 320 * C5J7 - 267 * S2J7 * S2J3 + 1098 * SJ3 - 778 * CJ7 * S2J3 - 2812 * SJ7 * SJ3 + 495 * C2J7 * S2J3 + 688 * S2J7 * SJ3 + 250 * C3J7 * S2J3 - 393 * S3J7 * SJ3 - 856 * SJ7 * C2J3 - 228 * S4J7 * SJ3 + 441 * S2J7 * C2J3 + 2138 * CJ7 * SJ3 + 296 * C2J7 * C2J3 - 999 * C2J7 * SJ3 - 325 * C4J7 * SJ3 + 398 * S3J7 * S3J3 - 890 * CJ3 + 211 * C3J7 * C2J3 - 642 * C3J7 * SJ3 - 427 * SJ7 * S3J3 + 344 * CJ7 * C3J3 + 2206 * SJ7 * CJ3 - 427 * C3J7 * C3J3;
                DA_pur = DA_pur * 0.000001;
                DP_pur = (0.07710801 + (0.007186 - 0.001533 * J1) * J1) * SJ5 - 0.007075 * SJ7 + (0.045803 - (0.014766 + 0.000536 * J1) * J1) * CJ5 - 0.072586 * CJ3 - 0.075825 * SJ7 * SJ3 - 0.024839 * S2J7 * SJ3 - 0.008631 * S3J7 * SJ3 - 0.150383 * CJ7 * CJ3 + 0.026897 * C2J7 * CJ3 + 0.010053 * C3J7 * CJ3 - (0.013597 + 0.001719 * J1) * SJ7 * S2J3 + 0.011981 * S2J7 * C2J3 - (0.007742 - 0.001517 * J1) * CJ7 * S2J3 + (0.013586 - 0.001375 * J1) * C2J7 * C2J3 - (0.013667 - 0.001239 * J1) * SJ7 * C2J3 + (0.014861 + 0.001136 * J1) * CJ7 * C2J3 - (0.013064 + 0.001628 * J1) * C2J7 * C2J3;
                DM_pur = DML_pur - FNRAD(DP_pur) / PLAN;
                DHL_pur = 0.000747 * CJ7 * SJ3 + 0.001069 * CJ7 * CJ3 + 0.002108 * S2J7 * S2J3 + 0.001261 * C2J7 * S2J3 + 0.001236 * S2J7 * C2J3 - 0.002075 * C2J7 * C2J3;
                DHL_pur = FNRAD(DHL_pur);
                break;
        }

        M_d = M_d + DM_pur * dRCN;
        P_d = P_d + (DML_pur - DM_pur) * dRCN;
        K_d = K_d + DS_pur;
        Q_d = Q_d + DA_pur;
        O_d = 0;
        for (var Y = 1; Y <= 6; Y++) { O_d = M_d + K_d * FNSN(O_d) * 57.2957795; }
        var V_d = 2 * FNAT(Math.sqrt((1 + K_d) / (1 - K_d)) * FNTN(O_d / 2)) + P_d;
        var U_d = FNAT(FNTN(V_d) * FNCS(L_d)) + dL;
        U_d = FNFR(U_d / 360 + 100) * 360;
        var YY_d = Math.abs(FNFR((U_d - V_d) / 360) - 0.5);
        if (YY_d <= 0.25) { U_d = U_d + 180; }
        U_d = U_d + N_d;
        var WW_d = Q_d * (1 - K_d * FNCS(O_d));
        var W_d = Q_d * (1 - K_d * FNCS(O_d)) * FNCS(FNASN(FNSN(L_d) * FNSN(V_d) + DHL * dRCN));
        X_d = H_d + FNAT(FNSN(U_d - H_d) * W_d / (J_d + W_d * FNCS(U_d - H_d)));
        X_d = FNR(X_d, 360);
        var TS_d = FNCS(U_d - H_d) + (WW_d * J_d + (WW_d + J_d) * Math.sqrt(WW_d * J_d) * W_d / WW_d) / (WW_d * WW_d + J_d * J_d + WW_d * J_d);
        YY_d = Math.abs(FNFR((X_d - U_d) / 360) - 0.5);
        if (YY_d <= 0.25) { X_d = X_d + 180; }
        if (W_d < J_d) {
            YY_d = Math.abs(FNFR((X_d - H_d) / 360) - 0.5);
            if (YY_d <= 0.25) { X_d = X_d + 180; }
            YY_d = FNCS(X_d - H_d);
            if (YY_d <= 0) { X_d = X_d + 180; }
        }
        if (TS_d > 0) { iVM[Panchgrahi + 3] = 0; sVakMar[Panchgrahi + 3] = "Margi"; }
        else { iVM[Panchgrahi + 3] = 1; sVakMar[Panchgrahi + 3] = "Vakri"; }
        dPlt[Panchgrahi + 3] = FNR(X_d - dAyanamsa, 360);
        dPltS[Panchgrahi + 3] = FNQ(dPlt[Panchgrahi + 3]);
        dLTP[Panchgrahi + 3] = FNAT(FNSN(U_d - N_d) * FNTN(L_d) * FNSN(X_d - H_d) / FNSN(U_d - H_d));

        // Lagna & Dasham Bhava (calculated inside loop per original)
        var dSiderealTime = 16.6981583332 + 15 * dBirthTime + dLong + dJDF * 0.985647335386 + dJDF * dJDF / 3446481700000.0;
        dSiderealTime = FNR(dSiderealTime, 360);
        dPlt[1] = FNAT(0.9174 * FNTN(dSiderealTime) + 0.398 * FNTN(dLatt) / FNCS(dSiderealTime)) + 90 + dSiderealTime - FNAT(FNTN(dSiderealTime));
        dPlt[1] = FNR(dPlt[1] - dAyanamsa, 360);
        dPlt[11] = FNAT(1.09 * FNTN(dSiderealTime)) + dSiderealTime - FNAT(FNTN(dSiderealTime));
        dPlt[11] = FNR(dPlt[11] - dAyanamsa, 360);
        dPltS[1] = FNQ(dPlt[1]);
        dPltS[11] = FNQ(dPlt[11]);
    }

    // Sun, Moon, Rahu, Ketu final values
    dPlt[2] = FNR(dPlt[2] - dAyanamsa, 360);
    dPltS[2] = FNQ(dPlt[2]);
    dPlt[3] = getmoon(dJDF);
    dPlt[3] = FNR(dPlt[3] - dAyanamsa, 360);
    dPltS[3] = FNQ(dPlt[3]);
    dPlt[9] = FNR(RHM_d - dAyanamsa, 360);
    dPltS[9] = FNQ(dPlt[9]);
    dPlt[10] = FNR(dPlt[9] + 180, 360);
    dPltS[10] = FNQ(dPlt[10]);

    return {
        lagna: roundNumber(dPltS[1], 6),
        sun: roundNumber(dPltS[2], 6),
        moon: roundNumber(dPltS[3], 6),
        mars: roundNumber(dPltS[4], 6),
        mercury: roundNumber(dPltS[5], 6),
        jupiter: roundNumber(dPltS[6], 6),
        venus: roundNumber(dPltS[7], 6),
        saturn: roundNumber(dPltS[8], 6),
        rahu: roundNumber(dPltS[9], 6),
        ketu: roundNumber(dPltS[10], 6),
        dPltS: dPltS.slice(),
        dPlt: dPlt.slice(),
        iVM: iVM.slice(),
        sVakMar: sVakMar.slice(),
        ayanamsa: dAyanamsa
    };
}

// ============================================================
// HOUSE MAPPING (preserved logic from Kundli_Graph)
// ============================================================
function buildHouseData(result) {
    var planetNames = ['', 'Lag', 'Sur', 'Cha', 'Mgl', 'Bud', 'Gur', 'Suk', 'San', 'Rah', 'Ket'];
    var planetFull = ['', 'Ascendant', 'Sun', 'Moon', 'Mars', 'Mercury', 'Jupiter', 'Venus', 'Saturn', 'Rahu', 'Ketu'];
    var iRasi = [];
    for (var i = 1; i <= 10; i++) { iRasi[i] = FNTGR(result.dPltS[i]) + 1; }

    var houses = [];
    var lagnaRasi = iRasi[1];

    for (var H = 1; H <= 12; H++) {
        var rasiNum = lagnaRasi;
        if (rasiNum === 12) rasiNum = 0;
        var house = { houseNum: H, rasi: rasiNum, rasiName: getRasiName(rasiNum), planets: [] };

        var bRR = 0;
        if (iRasi[2] === lagnaRasi) { house.planets.push({ abbr: 'Sur', name: 'Sun', vakri: result.sVakMar[2] }); }
        if (iRasi[3] === lagnaRasi) { house.planets.push({ abbr: 'Cha', name: 'Moon', vakri: result.sVakMar[3] }); }
        if (iRasi[4] === lagnaRasi) { house.planets.push({ abbr: 'Mgl', name: 'Mars', vakri: result.sVakMar[4] }); }
        if (iRasi[5] === lagnaRasi) { house.planets.push({ abbr: 'Bud', name: 'Mercury', vakri: result.sVakMar[5] }); }
        if (iRasi[6] === lagnaRasi) { house.planets.push({ abbr: 'Gur', name: 'Jupiter', vakri: result.sVakMar[6] }); }
        if (iRasi[7] === lagnaRasi) { house.planets.push({ abbr: 'Suk', name: 'Venus', vakri: result.sVakMar[7] }); }
        if (iRasi[8] === lagnaRasi) { house.planets.push({ abbr: 'San', name: 'Saturn', vakri: result.sVakMar[8] }); }
        if (iRasi[9] === lagnaRasi) { house.planets.push({ abbr: 'Rah', name: 'Rahu', vakri: result.sVakMar[9] }); }
        if (iRasi[10] === lagnaRasi) { house.planets.push({ abbr: 'Ket', name: 'Ketu', vakri: result.sVakMar[10] }); }

        if (lagnaRasi === 12) lagnaRasi = 0;
        else lagnaRasi = lagnaRasi;
        houses.push(house);
    }
    return houses;
}

function getRasiName(num) {
    var names = ['Aries', 'Taurus', 'Gemini', 'Cancer', 'Leo', 'Virgo', 'Libra', 'Scorpio', 'Sagittarius', 'Capricorn', 'Aquarius', 'Pisces'];
    return names[num] || '';
}

// ============================================================
// PUBLIC API: Calculate Kundli
// ============================================================
function calculateKundli(day, month, year, hours, minutes, longitude, latitude) {
    var dBirthTime = (parseFloat(hours) + parseFloat(minutes) / 60) - 5.5;
    var result = Kepler(parseInt(day), parseInt(month), parseInt(year), dBirthTime, parseFloat(longitude), parseFloat(latitude));
    result.houses = buildHouseData(result);
    window.kundliData = result;
    return result;
}