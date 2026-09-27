# Legacy Traceability Map — VB6 Indu (`D:\NirmlaV2`) → ASP.NET Web

> **Full function-level map:** [`Docs/VB6_CSHARP_FUNCTION_MAPPING.md`](VB6_CSHARP_FUNCTION_MAPPING.md)  
> **Calculation mismatch / fix report:** [`Docs/CALCULATION_DIFFERENCE_REPORT.md`](CALCULATION_DIFFERENCE_REPORT.md)

**Purpose:** Debug new app **function-by-function / variable-by-variable** against legacy.  
**Rule:** Prefer exact legacy names (`dPlt`, `dJDFx`, `FNDG`, `Milan_Calc`, …). Do not change business logic.  
**Calc rule:** VB6 `.bas` formulas are source of truth — see difference report for corrected terms (Rahu quadratic, FNSN, ST_Lagn_Dasam, VSOP sun, …).

**Legacy roots:**
- Astronomy: `aBasic_Astronomy.bas`, `a2Functions_Antronomy.bas`, `zVariables.bas`
- Match: `nMatchMaking.bas`
- Globals: `zVariables.bas` (`dPlt(12)`, `dPltS(12)`, `dAyanamsa`, `dGati(10)`, …)

**New roots:**
- Astronomy: `Services/KundliCalculator.cs`, `Services/AstroMath.cs`
- Orchestration: `Services/KundliWorkflowService.cs`
- Match: `Services/MatchMakingService.cs`
- Panchang: `Services/LegacyPanchangCalculator.cs`

Sample chart used for comparison: **08-Feb-1981 07:47 Patiala** (`dLatt≈30.19`, `dLong≈76.25`, `dTimeZone=5.30`).

---

## 1. Function mapping (Kundli / Astronomy)

| # | Legacy module | Legacy function | New class | New function | Status |
| - | ------------- | --------------- | --------- | ------------ | ------ |
| 1 | `aBasic_Astronomy` | `Basic_Astronomy_Calc` | `KundliCalculator` | `CalculateKundli` **alias** `Basic_Astronomy_Calc` | Same logic; public API kept for MVC |
| 2 | `a2Functions_Antronomy` | `DT2JDF` | `KundliCalculator` / `AstroMath` | `julday` / `AstroMath.Dt2Jdf` | Mapped (see JD notes) |
| 3 | `a2Functions_Antronomy` | `PlanetsLongLattDeclRet(dJDFx)` | `KundliCalculator` | `ComputeCore` (planet body + ayanamsa) | Folded into one method — see comments |
| 4 | `a2Functions_Antronomy` | `Ayanamsa_Calc(dJDFx)` | `KundliCalculator` | inline in `ComputeCore` (`dAyanamsa = …`) | Same formula family |
| 5 | `a2Functions_Antronomy` | `ST_Lagn_Dasam(dJDFx, dLatt, dLong)` | `KundliCalculator` | Lagna/MC block when `Panchgrahi == 5` | Same `dPlt(1)`, `dPlt(11)` |
| 6 | `a2Functions_Antronomy` | Moon inside `PlanetsLongLattDeclRet` | `KundliCalculator` | `GetMoon(dJDF)` | Mapped |
| 7 | `aBasic_Astronomy` | next-day `PlanetsLongLattDeclRet(dJDF+1)` + `dGati` | `KundliCalculator` | 2nd `ComputeCore` + `DailyMotion` / gati in `BuildResult` | Mapped |
| 8 | `a2Functions_Antronomy` | `FNDG` / `FNDM` / `FNR` / `FNQ` / `FNTGR` / `FNSN` / … | `KundliCalculator` + `AstroMath.FnDg` | **Same names** `FNDG`, `FNR`, … | Same |
| 9 | `a2Functions_Antronomy` | `FnPanchang` | `LegacyPanchangCalculator` | `Calculate` | Partial port — compare carefully |
| 10 | `a2Functions_Antronomy` | `Cal_Nak_N_Charan` | `KundliDetailsBuilder` | `NakshatraNumber` / `NakshatraPada` | Mapped |
| 11 | Control_Panel / DataEntry choice | Opt_Kundli / DETAILED / MEDIUM | `KundliWorkflowService` | `Generate(choice, …)` | Mapped |
| 12 | — (MVC) | — | `HomeController` | `CalculateKundli` | Web entry only |

### Public alias for debugger (new)

```text
KundliCalculator.Basic_Astronomy_Calc(...)  ==  CalculateKundli(...)
```

Set breakpoint on **either**; they share one body.

---

## 2. Variable mapping (critical)

| Legacy (`zVariables` / bas) | New | Type | Purpose | Status |
| --------------------------- | --- | ---- | ------- | ------ |
| `dPlt(1..11)` | `dPlt[1..11]` | `double[]` | Nirayana longitudes: 1=Lagna…10=Ketu, 11=MC | **Same** |
| `dPltS(1..11)` | `dPltS[1..11]` | `double[]` | `FNQ` RAKV encoding | **Same** |
| `dAyanamsa` | `dAyanamsa` | `double` | Ayanamsa | **Same** |
| `dJDF` / `dJDFx` | `dJDF` (param out) | `double` | Julian day fraction | Same meaning; prefer inspect as **dJDFx** |
| `dLatt` | `dLatt` | `double` | Latitude | **Same** |
| `dLong` | `dLong` | `double` | Longitude | **Same** |
| `dBirthTime` / local hours | `dBirthTimeIST` | `double` | Local civil hours | Same role (name IST historical) |
| `dTimeZone` packed HH.MM | `timezoneOffset` → `tzHours = FNDG(...)` | `double` | Zone | **Same FNDG** |
| `dGati(Z)` | `PlanetPosition.DailyMotion` / Details `Gati` | `double` | Daily motion | Mapped (degree/day vs arcmin display) |
| `iRasi(Z)` | `iRasi[i]` / `PlanetPosition.Rashi` | `int` | Sign 1–12 | **Same** |
| `Panchgrahi` | `Panchgrahi` | `int` | 1..5 planet loop | **Same** |
| `RHM_d` / mean Rahu | `RHM_d` | `double` | Mean node | **Same** |
| `ABBQQ_d`, `NUTQQ_d` | `ABBQQ_d`, `NUTQQ_d` | `double` | Aberration / nutation for Moon | **Same** |
| `sPltNam(Z)` | `names[]` in `BuildResult` | string | ASC/SUN/… | Near-same |
| MVC result wrappers | `KundliResult.Sun` etc. | `double` | Convenience mirrors of `dPlt[2]`… | New layer only |

### Renames (unavoidable / documented)

| Legacy | New | Reason |
| ------ | --- | ------ |
| `Basic_Astronomy_Calc` | also `CalculateKundli` | MVC/API already wired; **alias added** |
| `PlanetsLongLattDeclRet` + `ST_Lagn_Dasam` | single `ComputeCore` | One method for step-debug; **comments mark both** |
| `dJDFx` | `dJDF` out-param | Same value; use Watch alias |
| `dGati` as FNDM arc-minutes | `DailyMotion` degrees/day | Display layer may format as Gati text |
| `FindGunaString_FrVb6` | `MatchDetRepository.GetGunaString` | Repository pattern |

---

## 3. Calculation mapping (Basic_Astronomy_Calc)

### Legacy sequence (`aBasic_Astronomy.bas`)

```text
Basic_Astronomy_Calc
  ↓ dJDF = DT2JDF(dtBirth, dBirthTime, dTimeZone, …)
  ↓ Uday_Astha(dJDF)                    [optional / DB]
  ↓ PlanetsLongLattDeclRet(dJDF)        → fills dPlt, dAyanamsa
  ↓ ST_Lagn_Dasam(dJDF, dLatt, dLong)   → dPlt(1), dPlt(11)
  ↓ save dPlt1(Z), dAyanamsa1
  ↓ PlanetsLongLattDeclRet(dJDF+1)
  ↓ dGati(Z) from delta
  ↓ restore dPlt from dPlt1; dPltS(Z)=FNQ(dPlt(Z))
```

### New sequence (`KundliCalculator`)

```text
CalculateKundli / Basic_Astronomy_Calc
  ↓ ComputeCore(birth)     // PlanetsLongLattDeclRet + ST_Lagn_Dasam + Moon
  │    tzHours = FNDG(timezoneOffset)
  │    dJDF = julday(...)
  │    dAyanamsa = … (Ayanamsa_Calc family)
  │    Sun / Panchgrahi loops → dPlt[4..8]
  │    ST_Lagn block → dPlt[1], dPlt[11]
  │    GetMoon → dPlt[3]; Sun/Rahu/Ketu nirayana
  ↓ ComputeCore(next day)  // Gati source
  ↓ BuildResult → KundliResult + DailyMotion
```

### Intermediate steps to breakpoint (do **not** collapse)

| Step | Legacy | New inspect |
| ---- | ------ | ----------- |
| 1 | `dJDF` | `dJDF` after `julday` |
| 2 | `dAyanamsa` | `dAyanamsa` after ayanamsa lines |
| 3 | `dPlt(2)` Sun | `dPlt[2]` |
| 4 | `dPlt(4..8)` | after Kepler loop |
| 5 | `dPlt(1)` Lagna | after `Panchgrahi == 5` block |
| 6 | `dPlt(3)` Moon | after `GetMoon` |
| 7 | `dPlt(9)`, `dPlt(10)` | Rahu/Ketu |
| 8 | `dGati` | `DailyMotion` in `BuildResult` |

---

## 4. Match-Making mapping

| Legacy | New | Status |
| ------ | --- | ------ |
| `Kundli_Milaan_Detailed` | `MatchMakingService.Compare` | Entry |
| `Milan_Calc` | `MatchMakingService.MilanCalc` | **Same logic name (Pascal)** |
| `FindGunaString_FrVb6` | `MatchDetRepository.GetGunaString` | DB |
| Access `MatchDet` | MySQL `tbl_match_det` (`field_01`… = `Field1`…) | Verified sample |
| `Rashish_Matri(iBoyRsi, iGirlRsi)` | `RashishMatri` | **Same**; call `RashishMatri(rasiF, rasiF)` mirrors VB6 `(iRasiNum_F, iRasiNum_F)` |
| `iNakNum_M/F`, `iRasiNum_M/F` | profile `Hora.NakshatraNumber`, `MoonRashi` | Mapped |
| `sGunStr` | `gunaStr` / `GunaString` | Mapped |
| `dGunNrml(Z)`, `dGunModi(Z)` | `normal[z]`, `modified[z]` | Mapped |

---

## 5. Panchang mapping

| Legacy | New | Status |
| ------ | --- | ------ |
| `FnPanchang` | `LegacyPanchangCalculator.Calculate` | Partial |
| Tithi / Yoga / Karan from Sun–Moon | same file | Compare against VB6 carefully |
| `SunRiseSetTimeZST` | `Sunrise` helper / `AstroMath.ApproxSunEventHours` | Approx — verify |
| Home card | `HomeController.GetTodayPanchangCached` | Uses `KundliCalculator` + `LegacyPanchangCalculator` |

---

## 6. Database mapping (readings / place / match)

| MDB | MySQL | Legacy use | New use |
| --- | ----- | ---------- | ------- |
| `Place` | `tbl_place` | Atlas | `PlaceRepository` |
| `MatchDet` | `tbl_match_det` | `FindGunaString_FrVb6` | `MatchDetRepository` |
| `Nak27_Reading` | `tbl_nakshatra_reading` | Text_Phalit | `KundliReportRepository.Enrich` |
| `NakCharan108_Reading` | `tbl_nakshatra_charan_reading` | Text_Phalit | Enrich |
| `Sign_Reading` | `tbl_sign_reading` | Text_Phalit | Enrich |
| `House_Reading` | `tbl_house_reading` | Text_Phalit | Enrich |
| `Plt9xHs12_Reading` | `tbl_planet_house_reading` | Gochar/Lal | Enrich / Gochar |
| `Plt9xSign12_Reading` | `tbl_planet_sign_reading` | Text_Phalit | Enrich |
| `Record_F_Natives` | `tbl_native` | Records | `NativeRepository` |
| `Uday_Astha` | `Uday_Astha` flat; `tbl_udaya_astha_rule` often empty | Udaya | Gap if rules table empty |

Full counts: see `Docs/MDB_MYSQL_VERIFICATION.md`.

---

## 7. Execution flow (debug path)

```text
Home Generate → SetBirthDataSession
  ↓
Result.cshtml AJAX
  ↓
HomeController.CalculateKundli
  ↓
KundliWorkflowService.Generate
  ↓
KundliCalculator.Basic_Astronomy_Calc / CalculateKundli
  ↓
ComputeCore  ≈ PlanetsLongLattDeclRet + ST_Lagn_Dasam
  ↓
ComputeCore(+1 day)  ≈ Gati
  ↓
BuildResult  ≈ dPltS=FNQ + houses
  ↓
KundliDetailsBuilder + LegacyPanchangCalculator
  ↓
KundliReportRepository.Enrich (MySQL texts)
  ↓
[DETAILED/MEDIUM] Bhava / Dasha / Varga / …
  ↓
JSON / SVG / PDF
```

---

## 8. Breakpoint list (legacy-aligned)

| BP | Place | Watch (legacy names) |
| -- | ----- | -------------------- |
| 1 | `HomeController.CalculateKundli` entry | day/month/year, lat, lon, timezone |
| 2 | `KundliCalculator.CalculateKundli` / `Basic_Astronomy_Calc` | same |
| 3 | `ComputeCore` after `dJDF = julday` | **dJDF / dJDFx**, `tzHours`, `dLong`, `dLatt` |
| 4 | After `dAyanamsa =` | **dAyanamsa**, `RHM_d` |
| 5 | After Sun assignment `dPlt[2]` | **dPlt(2)** |
| 6 | End of `Panchgrahi` Kepler assign | **dPlt(4..8)**, `isRetrograde` |
| 7 | After Lagna `dPlt[1]=` | **dPlt(1)**, `_siderealTime` |
| 8 | After `GetMoon` | **dPlt(3)** |
| 9 | After Rahu/Ketu | **dPlt(9)**, **dPlt(10)** |
| 10 | `BuildResult` loop | **iRasi**, House, `DailyMotion` (~dGati) |
| 11 | `MilanCalc` after `gunaStr` | row/col, `sGunStr` |
| 12 | `RashishMatri` | boy/girl rasi args |

---

## 9. First-difference debugging guide

```text
1. Run SAME birth in VB6 Indu and Web (08.02.1981 07:47 Patiala 5.30).
2. Break at dJDF / dJDFx — if different, stop (time/zone/FNDG/DT2JDF).
3. Break at dAyanamsa — if different, Ayanamsa_Calc / Delta-T.
4. Break at dPlt(1) Lagna — ST_Lagn_Dasam vs Lagna block.
5. Break at dPlt(2..10) — PlanetsLongLattDeclRet vs Kepler loop.
6. Break at dGati / DailyMotion — next-day call.
7. Only then compare UI/SVG/PDF (mapping issues, not astronomy).
```

Report format when values diverge:

```text
First Difference:
Legacy Function: …
New Function: …
Legacy Variable: dPlt(n) / dAyanamsa / …
New Variable: dPlt[n] / …
Legacy Value: …
New Value: …
```

**Do not “fix” until the first differing variable is identified.**

---

## 10. Quality checklist (per module)

```text
[x] Legacy astronomy entry identified (Basic_Astronomy_Calc)
[x] New equivalent identified (CalculateKundli + alias)
[x] dPlt / dPltS / dAyanamsa / FNDG preserved in calc core
[x] Intermediate steps kept (not one-liner collapsed)
[x] Match Milan_Calc / Rashish_Matri mapped
[x] MatchDet → tbl_match_det mapped
[x] Breakpoints listed
[ ] Side-by-side numeric sheet filled for sample chart (do in VS)
[ ] FnPanchang vs LegacyPanchangCalculator line-by-line (pending)
[ ] Uday_Astha full parity (pending if required)
```

---

## 11. Related docs

| Doc | Content |
| --- | ------- |
| `Docs/MDB_MYSQL_VERIFICATION.md` | MDB↔MySQL counts & indexes |
| `Docs/LEGACY_VERIFICATION.md` | Admin / SVG / Match parity notes |
| This file | Function/variable/calc traceability |
