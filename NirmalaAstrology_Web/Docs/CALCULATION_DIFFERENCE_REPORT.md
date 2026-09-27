# VB6 → C# Calculation Difference Report

**Legacy source of truth:** `D:\NirmlaV2\a2Functions_Antronomy.bas`, `aBasic_Astronomy.bas`, `00_Earth_Sun.bas`  
**C# target:** `Services/KundliCalculator.cs`, `Services/Earth_Sun.cs`  
**Cross-check:** `Scripts/Custom/kundli-reference-engine.js` (prior VB6-aligned port)

**Sample chart:** 08-Feb-1981 07:47, Patiala atlas packed `lon=76.25` `lat=30.19` `tz=5.30`

---

| Body | VB6 print (RAKV) | C# FormatRakv | Status |
|------|-----------------:|---------------|--------|
| Lagna | 10:06:13:46 | 10:06:13:46 | MATCH |
| Sun | 09:25:39:28 | 09:25:39:19 | ~9″ |
| Moon | 11:07:09:35 | 11:07:09:27 | ~8″ |
| Mars | 10:07:18:35 | 10:07:18:35 | MATCH |
| Mercury | 10:11:18:33 | 10:11:18:33 | MATCH |
| Jupiter | 05:16:28:11 | 05:16:28:11 | MATCH |
| Venus | 09:11:13:15 | 09:11:13:15 | MATCH |
| Saturn | 05:15:49:60 | 05:15:49:56 | ~4″ |
| Rahu | 03:16:54:35 | 03:16:54:35 | MATCH |
| Ketu | 09:16:54:35 | 09:16:54:35 | MATCH |
| Ayanamsa | 23:35:23 | 23:35:23 | MATCH (print uses Ayanamsa_Calc; planets use nutated) |

**Display:** UI now uses legacy patterns — planets `RR:DD:MM:SS` (0-based rashi), ayanamsa/sidereal `DD:MM:SS` via `AstroMath.FormatAyanamsa` / `FormatSiderealTimeHours` (VB6 `Format_RAK(FNDM(...))`). Do **not** show ayanamsa as `23.59`.

**Note:** VB6 print first field is **0-based** rashi (`10:` = Kumbha band starting at 300°), not 1-based `(R-1)*30`.

---

## A. Fixes applied (were MISMATCH)

### 1. Mean Rahu quadratic term — **FIXED**

**VB6**
```vb
dRahuMean = FNR((259.2097527 - 1934.138634 * dJDFx / 36525!), 360) + 7.48 / 3600 * (dJDFx / 36525) ^ 2
```

**Old C#**
```csharp
double RHM_d = FNR((259.2097527 - 1934.138634 * dJDF / 36525.0), 360.0);
// missing + 7.48/3600 * T^2
```

**Problem:** Quadratic correction omitted.  
**Impact:** `dRahuMean`, ayanamsa, Rahu/Ketu, all nirayana longitudes.  
**New C#:** Full formula restored (see `ComputeCore`).

---

### 2. Ayanamsa used `Math.Sin` on degrees — **FIXED**

**VB6:** `FNSN(dRahuMean)` (sin of degrees)  
**Old C#:** `Math.Sin(RHM_d)` (treats degrees as radians)  
**Impact:** Ayanamsa completely wrong → every planet.  
**New C#:** `FNSN(RHM_d)`.

---

### 3. MSAQQ constant 358 vs 357 — **FIXED**

**VB6:** `357.9830444`  
**Old C#:** `358.9830444`  
**Impact:** Sun mean anomaly, nutation, planet perturbations.

---

### 4. NUTQQ second term MSAQQ vs MSNQQ — **FIXED**

**VB6:** `0.13 * FNSN(MSAQQ_d)`  
**Old C#:** `0.13 * FNSN(MSNQQ)`  
**Impact:** Nutation / sun longitude.

---

### 5. QJQQ missing `FNDG(41.114619)` — **FIXED**

**VB6:** `180 - FNDG(41.114619) - ...`  
**Old C#:** `180.0 - 41.114619 - ...`  
**Impact:** Jupiter perturbation (`PSJQQ`, `DRJQQ`).

---

### 6. Extra PSJQQ terms — **FIXED**

**VB6:** second assignment with `-0.56*FNSN(...)`, `-0.16*...`, `-0.21*...`  
**Old C#:** missing  
**Impact:** Sun longitude via `DLQQ`.

---

### 7. Saturn inclination `L_d` — **FIXED**

**VB6:** `2.4938056`  
**Old C#:** `0.492519`  
**Impact:** Saturn (and geocentric conversion).

---

### 8. Saturn `K3_d` — **FIXED**

**VB6:** `0.00000000074`  
**Old C#:** `0`

---

### 9. Mars DL/DR phase angles — **FIXED**

**VB6:** `-3.2873` and `+0.771864`  
**Old C#:** `-0.85448` and `+0.00717864`

---

### 10. Venus missing DL term + wrong T — **FIXED**

Added `0.00198 * Cos(3*MAS - 3*MAP(2) + 0.044768)`;  
Venus DML uses `T = dJDF/36525` (not `B_d = (dJDF-0.5)/36525`).

---

### 11. Geocentric `U_d` used `dLong` — **FIXED**

**VB6:** `U_d = FNAT(...) + DL_per`  
**Old C#:** `+ dLong` (~76°) — catastrophic for Mars…Saturn.  
**Impact:** Was the largest planet-longitude error.

---

### 12. Moon node constant — **FIXED**

**VB6:** `259.183275`  
**Old C#:** `258.193275`

---

### 13. `Correction_Delta_T` — **IMPLEMENTED**

Applied to planet/moon/ayan JD; raw JD kept for VSOP sun + `ST_Lagn_Dasam` (VB6 ByVal semantics).

---

### 14. Ayanamsa nutation slice `XLQQ` — **FIXED**

**VB6:** `dAyanamsa = dAyanamsa - dNutationAyan / 3600` before sayana→nirayana.

---

### 15. VSOP87 Sun (`LBR_For_Earth_or_Sun`) — **IMPLEMENTED**

New `Services/Earth_Sun.cs` (port of `00_Earth_Sun.bas`).  
Called as `LBR_For_Earth_or_Sun(dJDF_raw + 2415019.5, "S")`.

---

### 16. `ST_Lagn_Dasam` formula — **REPLACED**

**Old C#:** approximate `16.698… + 15*IST + …` and `0.9174/0.398` lagna.  
**New C#:** Exact VB6 `99.195 + 360*FNFR(JD) + …`, obliquity polynomial, `Ayanamsa_Calc` on raw JD.

---

### 17. Atlas lat/long via `FNDG` — **ALIGNED**

**VB6/JS:** `dLong = FNDG(atlasPacked)`, `dLatt = FNDG(atlasPacked)`.  
Callers must pass **HH.MM packed** atlas values (e.g. `76.25` = 76°25′), not pure decimal degrees.

---

### 18. `W_d < J_d` quadrant block — **ADDED**

Preserved VB6 `DHL = 0` (variable never assigned in `.bas`).

---

### 19. Compound `-=` / `+=` flipped Jupiter & Saturn terms — **FIXED**

**VB6:** `x = x - a - b` and `x = x - a + b - c` (left-associative).  
**Old C#:** `x -= a - b` → `x = x - (a - b)` and `x -= a + b` → `x = x - (a + b)` — **second (and later) terms flip sign**.

Worst hits were Saturn `DA_pur` (`+33629*CJ7` became subtract) and Jupiter `DS_pur` / `DML_pur` / `DP_pur`.  
**New C#:** Explicit `x = x ± a ± b` matching `a2Functions_Antronomy.bas` Case 3/5.  
**Impact:** Saturn Δ vs JS dropped from ~0.024° to ~1e-5°.

---

### 20. Single-pass `MAP`/`M1X` — **FIXED**

`MAP` is rebuilt each Panchgrahi iteration from whatever `M1X` is filled so far (VB6 order), not a second full pass.

---

## B. Remaining OPEN / Review

| Item | Status | Notes |
|------|--------|-------|
| Full VB6 side-by-side on Indu debugger | REVIEW REQUIRED | Compare `dPlt(1..10)`, `dAyanamsa` on same birth |
| Place DB: packed vs decimal | REVIEW REQUIRED | Ensure UI/DB store atlas HH.MM like Indu |
| Mars residual ~0.002° vs JS | OPEN | Likely remaining MAP/order or micro-constant drift; not Saturn-class |
| JS smoke without VSOP | NOTE | Load/port `LBR` into JS before treating Sun Δ as a C# bug |
| Other `.bas` ganit (Bhava, Dasha, Match beyond Milan) | NOT in this pass | Separate reconciliation |
| `FnPanchang` full parity | PARTIAL | Display port only |

---

## C. File / service mapping (astronomy)

```text
aBasic_Astronomy.bas::Basic_Astronomy_Calc
    ↓
KundliCalculator.Basic_Astronomy_Calc / CalculateKundli

a2Functions_Antronomy.bas::PlanetsLongLattDeclRet
    ↓
KundliCalculator.ComputeCore

a2Functions_Antronomy.bas::ST_Lagn_Dasam
    ↓
KundliCalculator.ST_Lagn_Dasam

a2Functions_Antronomy.bas::Ayanamsa_Calc / Correction_Delta_T / FNDG…
    ↓
KundliCalculator (same names)

00_Earth_Sun.bas::LBR_For_Earth_or_Sun
    ↓
Earth_Sun.LBR_For_Earth_or_Sun
```

---

## D. How to re-verify in Visual Studio

1. VB6: break in `PlanetsLongLattDeclRet` after `dRahuMean` / `dAyanamsa`.  
2. C#: break in `ComputeCore` on `RHM_d` / `dAyanamsa`.  
3. Same birth → compare.  
4. Then `ST_Lagn_Dasam` vs C# `ST_Lagn_Dasam` for `dPlt(1)`.  
5. First differing variable = root cause (do not “fix” later values first).

---

*Report updated after calculation reconciliation pass. Prefer VB6 values over any prior C# smoke baselines.*
