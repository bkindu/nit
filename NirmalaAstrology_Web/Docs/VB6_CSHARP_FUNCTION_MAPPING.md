# VB6 → C# Traceability & Function Mapping

**Legacy root:** `D:\NirmlaV2`  
**New root:** `E:\NirmalaAstrology\NirmalaAstrology_Web`  
**Generated from actual source inventories** (`.bas` / `.cls` / `.frm` ↔ `Services/*.cs`).  
**Rule:** Do not treat Status = Matched unless implementations were compared. Prefer **Review Required** when only structural mapping exists.

**Related docs:** `Docs/LEGACY_TRACEABILITY.md` (variables / breakpoints), `Docs/LEGACY_VERIFICATION.md`, `Docs/MDB_MYSQL_VERIFICATION.md`.

**Sample chart for numeric checks:** 08-Feb-1981 07:47 Patiala (`lat≈30.19`, `lon≈76.25`, `tz=5.30`).

---

## Status legend

| Status | Meaning |
|--------|---------|
| **Matched** | Equivalent role verified (name and/or sample values compared). |
| **Renamed** | Same role; C# name differs (reason listed in §3). |
| **Split** | One VB6 routine → multiple C# methods. |
| **Merged** | Multiple VB6 routines → one C# method. |
| **Moved** | Logic lives in a different class than the VB6 module name suggests. |
| **Not Migrated** | No C# equivalent found. |
| **Review Required** | Port exists or is partial; open both sides and compare before trusting parity. |
| **UI Only / N/A** | VB6 form or print helper; web uses MVC/PDF instead (not a calc gap by itself). |

---

# 1. File Mapping

| Old VB6 File | Old Type | New C# File | New C# Class | Migration Status | Notes |
| ------------ | -------- | ----------- | ------------ | ---------------- | ----- |
| `aBasic_Astronomy.bas` | BAS | `Services/KundliCalculator.cs` | `KundliCalculator` | Completed (core) | Entry `Basic_Astronomy_Calc`; also `CalculateKundli` alias |
| `a2Functions_Antronomy.bas` | BAS | `KundliCalculator.cs` + `AstroMath.cs` + `LegacyPanchangCalculator.cs` + `HoraSangyaCalculator.cs` | several | Partial | Math helpers + planets + lagna + partial panchang |
| `zVariables.bas` | BAS globals | (no single class) | `dPlt[]` locals / `KundliResult` | Mapped conceptually | Globals → parameters / result DTOs |
| `a1Action_Plan.bas` | BAS | `Services/KundliWorkflowService.cs` | `KundliWorkflowService` | Partial | `Control_Panel` → `Generate(choice,…)` |
| `nMatchMaking.bas` | BAS | `Services/MatchMakingService.cs` | `MatchMakingService` | Partial | `Milan_Calc` core; many `*_Vichar` not migrated |
| `mMatching.bas` | BAS | — | — | **Not Migrated** | `Matching_for_27Nak` |
| `aPanchanga.bas` | BAS | `LegacyPanchangCalculator.cs` + workflow | `LegacyPanchangCalculator` | Partial | Print module vs web display model |
| `cBhavaSpastaAndChalitKundli.bas` | BAS | `Services/BhavaSpastaService.cs` | `BhavaSpastaService` | Review Required | Calc port; print not same |
| `eDashasAll.bas` | BAS | `VimshottariDashaService.cs` + `ExtraDashaService.cs` | both | Review Required | Vimshottari + several extras; not all dashas |
| `f16Vrg_Kundlies_CalcPrn.bas` | BAS | `Services/Varga16Service.cs` | `Varga16Service` | Review Required | |
| `h16VrgTableGrahAvsthasCalcPrn.bas` | BAS | `GrahaAvasthaService.cs` (+ varga table elsewhere) | `GrahaAvasthaService` | Review Required | |
| `iPanchadhaKinshukadi.bas` | BAS | `Services/PanchadhaKinshukService.cs` | `PanchadhaKinshukService` | Review Required | |
| `jUpGrahLgnCalcPrn.bas` | BAS | `SpecialLagnaUpGrahaService.cs` | same | Review Required | UpGrahas methods |
| `gSpl_LgnKundlies_CalcPrn.bas` | BAS | `SpecialLagnaUpGrahaService.cs` | same | Review Required | Special lagnas methods |
| `kGrahBalaCalcPrn.bas` | BAS | `Services/GrahaBalaService.cs` | `GrahaBalaService` | Review Required | |
| `lAstakVarg.bas` | BAS | `Services/AshtakavargaService.cs` | `AshtakavargaService` | Review Required | |
| `oYogasWithGrammar.bas` | BAS | `Services/YogaDetectionService.cs` | `YogaDetectionService` | Partial | Subset of yogas; grammar print not full |
| `bPlanetsLongAndKundli_Printing.bas` | BAS | `KundliDetailsBuilder` + SVG/PDF renderers | several | Partial | Screen/PDF, not RTF Text_Ganit |
| `a4Functions_Print_RTF.bas` | BAS | `AstroMath` + PDF/SVG services | several | Partial | `Format_RAKV` / `Kala` style helpers |
| `a3Functions_Gen.bas` | BAS | partial (`Uday_Astha` gap) | — | Partial | Place UI → repos; `Uday_Astha` **Not Migrated** to live path |
| `pNumerology.bas` | BAS | `Services/LegacyNumerologyService.cs` | `LegacyNumerologyService` | Review Required | |
| `qVarshPhal.bas` | BAS | `Services/VarshPhalService.cs` | `VarshPhalService` | Review Required | |
| `rLalKitab.bas` | BAS | `Services/LalKitabService.cs` | `LalKitabService` | Review Required | |
| `m1Marriage_Muhurata.bas` | BAS | `Services/VivahMuhurtaService.cs` | `VivahMuhurtaService` | Review Required | |
| `a5Fill_Yearly_Muhurat_Data.bas` | BAS | `MuhuratService` / data files | `MuhuratService` | Review Required | |
| `tTodayzKaalShudhi.bas` | BAS | partial via Muhurat/Panchang | — | Partial / Review | Large muhurta engine |
| `sBioRhythms.bas` | BAS | — | — | **Not Migrated** | |
| `00_Earth_Sun.bas` | BAS | — | — | **Not Migrated** | VSOP series; active kundli path does not call these |
| `Data_Entry.frm` | Form | Controllers + BirthData session | MVC | UI replaced | |
| `Atlas.frm` | Form | `PlaceRepository` + Home place search | MVC | UI replaced | |
| `Text_Ganit.frm` / `Text_Phalit.frm` | Form | Result views + report builders | MVC/PDF | UI replaced | |
| `Muhurat.frm` | Form | `MuhuratController` | MVC | UI replaced | |
| `Palmistry.frm` | Form | `PalmistryService` | MVC | Review Required | |
| `Record.frm` | Form | Native/Customer repos | MVC | UI replaced | |
| `mdi_Nirmala.frm` / `Spl.frm` / `YogResearch.frm` / `Graphology.frm` | Form | — / partial | — | N/A or Not Migrated | |

---

# 2. Function Mapping

## 2.1 Orchestration — `a1Action_Plan.bas`

| # | Old VB6 File | Old VB6 Function | New C# Class | New C# Method | Purpose | Status |
| - | ------------ | ---------------- | ------------ | ------------- | ------- | ------ |
| 1 | `a1Action_Plan.bas` | `Control_Panel` | `KundliWorkflowService` | `Generate` | Route by `sChoice` / report choice after astronomy | **Renamed** + **Review Required** (choice set differs: web uses `OPT_KUNDLI`/`KUNDALI`/`MEDIUM`/`DETAILED`) |

### Split detail — `Control_Panel`

| Old VB6 | New C# methods / services |
|---------|---------------------------|
| `Control_Panel` | `KundliWorkflowService.Generate` → `KundliCalculator.CalculateKundli` → (if MEDIUM/DETAILED) `BhavaSpastaService.Calculate`, `VimshottariDashaService.Calculate`, `Varga16Service.Calculate`, `AshtakavargaService.Calculate`, `PanchadhaKinshukService.*`, `GrahaBalaService.Calculate`, `SpecialLagnaUpGrahaService.*`, `GrahaAvasthaService.Calculate`, `YogaDetectionService.Calculate`, `ExtraDashaService.Calculate` |

---

## 2.2 Astronomy entry — `aBasic_Astronomy.bas`

| # | Old VB6 File | Old VB6 Function | New C# Class | New C# Method | Purpose | Status |
| - | ------------ | ---------------- | ------------ | ------------- | ------- | ------ |
| 2 | `aBasic_Astronomy.bas` | `Basic_Astronomy_Calc` | `KundliCalculator` | `Basic_Astronomy_Calc` | Full birth chart longitudes + lagna + gati | **Matched** (exact name alias; sample chart smoke-tested) |
| 3 | `aBasic_Astronomy.bas` | `Basic_Astronomy_Calc` | `KundliCalculator` | `CalculateKundli` | Same body as #2 | **Renamed** (MVC API name; identical call) |
| 4 | *(MVC facade)* | — | `WorkingJavascriptKundliCalculator` | `CalculateKundli` | Thin wrapper → `KundliCalculator` | Moved/facade (JS path removed) |

### Split detail — `Basic_Astronomy_Calc`

| Old VB6 sequence | New C# |
|------------------|--------|
| `DT2JDF` | `julday` inside `ComputeCore` (+ `AstroMath.Dt2Jdf` / `JulDay` elsewhere) |
| `PlanetsLongLattDeclRet(dJDF)` | `ComputeCore` (planet/ayanamsa/moon body) |
| `ST_Lagn_Dasam` | Lagna/MC block in `ComputeCore` when `Panchgrahi == 5` |
| `PlanetsLongLattDeclRet(dJDF+1)` + `dGati` | 2nd `ComputeCore` + gati in `BuildResult` |
| `FNQ` → `dPltS` | `FNQ` / `dPltS` in `ComputeCore` + rashi decode in `BuildResult` |

Status of fold: **Merged** into `ComputeCore` + `BuildResult` (comments mark legacy names).

---

## 2.3 Astronomy math & planets — `a2Functions_Antronomy.bas`

| # | Old VB6 Function | New C# Class | New C# Method | Status |
| - | ---------------- | ------------ | ------------- | ------ |
| 5 | `FNTGR` | `KundliCalculator` / `AstroMath` | `FNTGR` / `FnTgr` | **Matched** (same formula family; dual location) |
| 6 | `FNFR` | both | `FNFR` / `FnFr` | **Matched** |
| 7 | `FNR` | both | `FNR` / `FnR` | **Matched** |
| 8 | `iFNR` | `AstroMath` | `IFnR` | **Renamed** (C# casing) |
| 9 | `FNQ` | both | `FNQ` / `FnQ` | **Matched** |
| 10 | `FNC` | — | — | **Not Migrated** (verify call sites) |
| 11 | `FNSN` | both | `FNSN` / `FnSn` | **Matched** |
| 12 | `FNCS` | both | `FNCS` / `FnCs` | **Matched** |
| 13 | `FNTN` | both | `FNTN` / `FnTn` | **Matched** |
| 14 | `FNAT` | both | `FNAT` / `FnAt` | **Matched** |
| 15 | `FNASN` | `KundliCalculator` | `FNASN` | **Matched** |
| 16 | `FNACS` | — | — | **Not Migrated** |
| 17 | `FNRAD` | `KundliCalculator` | `FNRAD` | **Matched** |
| 18 | `FNC2` | `AstroMath` | `FnC2` | **Renamed** |
| 19 | `FNC3` | `AstroMath` | `FnC3` | **Renamed** |
| 20 | `PksBalEtc` | `AstroMath` | `PksBalEtc` | **Matched** (name) / **Review Required** (callers) |
| 21 | `FNUNW` | `KundliCalculator` | `FNUNW` | **Matched** |
| 22 | `FNDM` | `KundliCalculator` | `FNDM` | **Matched** |
| 23 | `FNDG` | `KundliCalculator` / `AstroMath` | `FNDG` / `FnDg` | **Matched** |
| 24 | `Correction_Delta_T` | — (inline path may omit) | — | **Review Required** / likely **Not Migrated** on web JD path |
| 25 | `Ayanamsa_Calc` | `KundliCalculator` | inline in `ComputeCore` | **Moved** + **Merged** |
| 26 | `VaarAnkHindu` | `LegacyPanchangCalculator` | via `DateTime.DayOfWeek` / `Vaar[]` | **Review Required** |
| 27 | `DT2JD` | — | — | **Not Migrated** (use `DT2JDF` / `julday`) |
| 28 | `DT2JDF` | `AstroMath` / `KundliCalculator` | `Dt2Jdf` / `julday` | **Split** / **Review Required** (two JD styles exist) |
| 29 | `LastDayOfMonth` | — | BCL `DateTime` | **Not Migrated** as named API |
| 30 | `FirstDayOfMonth` | — | BCL | **Not Migrated** as named API |
| 31 | `JD2DT` | `AstroMath` | `Jd2Dt` | **Renamed** |
| 32 | `Chk_Validity_Dt` | — | MVC validation | **Not Migrated** (same name) |
| 33 | `ST_Lagn_Dasam` | `KundliCalculator` | Lagna/MC block in `ComputeCore` | **Merged** |
| 34 | `SunBriefSyn` | — | — | **Not Migrated** (full sun path used instead) |
| 35 | `MoonBriefSyn` | — | — | **Not Migrated** |
| 36 | `IshatKaalGPdec` | `KundliWorkflowService` | ishtGp calc in `Generate` | **Review Required** |
| 37 | `SunRiseSetTimeZST` | `LegacyPanchangCalculator` / `AstroMath` | `Sunrise` / `ApproxSunEventHours` | **Review Required** (approx formula) |
| 38 | `Cal_Nak_N_Charan` | `HoraSangyaCalculator` / details builder | `FromMoon` / nak fields | **Review Required** |
| 39 | `Uchadi_Avasthas` | `GrahaAvasthaService` | `Calculate` | **Review Required** |
| 40 | `Lagnodya_Kaal_ZST` | — | — | **Not Migrated** |
| 41 | `Godhuli_LagnaTimeZST` | — | — | **Not Migrated** |
| 42 | `RsiNum2RsiName` | `KundliCalculator.BuildResult` | `rashiNames[]` | **Moved** |
| 43 | `PlanetsLongLattDeclRet` | `KundliCalculator` | `ComputeCore` (+ `GetMoon`) | **Merged** / **Renamed** |
| 44 | *(Moon inside PlanetsLong…)* | `KundliCalculator` | `GetMoon` | **Split** |
| 45 | `FnPanchang` | `LegacyPanchangCalculator` | `Calculate` | **Renamed** + **Review Required** (partial port) |
| 46 | `Hora_Sangya` | `HoraSangyaCalculator` | `FromMoon` | **Renamed** + **Review Required** |
| 47 | `Mean_Planets` | `AstroMath` | `MeanPlanets` | **Renamed** + **Review Required** |
| 48 | `Sheeghroch` | — / GrahaBala internal | — | **Review Required** |
| 49 | `Grah9_Rasi_Pravesh_Dt_DataFile_Creation` | — | — | **Not Migrated** |
| 50 | `Create_YearlyDataFile_4_Longitudes_of_9Planets` | — | — | **Not Migrated** |
| 51 | `Sun_Perturbations` | inline in `ComputeCore` | sun perturbation terms | **Merged** |

---

## 2.4 Match-making — `nMatchMaking.bas` / `mMatching.bas`

| # | Old VB6 Function | New C# Class | New C# Method | Status |
| - | ---------------- | ------------ | ------------- | ------ |
| 52 | `Kundli_Milaan_Detailed` | `MatchMakingService` | `Compare` | **Renamed** + **Review Required** (entry; less print/grammar) |
| 53 | `Milan_Calc` | `MatchMakingService` | `MilanCalc` (private) | **Matched** (name preserved privately; matrix + parihar logic ported) |
| 54 | `FindGunaString_FrVb6` | `MatchDetRepository` | `GetGunaString` | **Renamed** + **Moved** |
| 55 | `FindGunaString_FrVbDotNet` | — | — | **Not Migrated** (VB.NET path) |
| 56 | `Rashish_Matri` | `MatchMakingService` | `RashishMatri` | **Matched** (incl. legacy female/female call site) |
| 57 | `Manglik_Grammar_N_Cancellation` | `MatchMakingService` | manglik string in `MilanCalc` | **Partial** / **Review Required** |
| 58 | `Manglik_Bhang_Vichar` | — | — | **Not Migrated** |
| 59 | `Varaga_Vichar` | — | — | **Not Migrated** |
| 60 | `Yunja_Vichar` | — | — | **Not Migrated** |
| 61 | `Hansak_Vichar` | — | — | **Not Migrated** |
| 62 | `Rashi_Match` | — | — | **Not Migrated** |
| 63 | `Din_Vichar` | — | — | **Not Migrated** |
| 64 | `Mahender_Vichar` | — | — | **Not Migrated** |
| 65 | `Istri_Deergha_Vichar` | — | — | **Not Migrated** |
| 66 | `Vedha_Vichar` | — | — | **Not Migrated** |
| 67 | `Vihag_Vichar` | — | — | **Not Migrated** |
| 68 | `Istri_Purush_Vichar` | — | — | **Not Migrated** |
| 69 | `Gotra_Vivah_Vichar` | — | — | **Not Migrated** |
| 70 | `Rajju_Vichar` | — | — | **Not Migrated** |
| 71 | `Aaya_Vayaya_Vichar` | — | — | **Not Migrated** |
| 72 | `How2Deal_Spouse` | — | — | **Not Migrated** |
| 73 | `Display_1stPage` | MatchMaking views | Razor | **UI Only** |
| 74 | `mMatching.Matching_for_27Nak` | — | — | **Not Migrated** |

---

## 2.5 Control_Panel ganit modules

| # | Old VB6 File / Function | New C# Class | New C# Method | Status |
| - | ----------------------- | ------------ | ------------- | ------ |
| 75 | `aPanchanga.Panchang_Calc_and_Print` | `LegacyPanchangCalculator` | `Calculate` | **Review Required** |
| 76 | `aPanchanga.Prn_HoroPanchang` | — / views | — | **UI Only** / **Not Migrated** as print |
| 77 | `bPlanetsLong….PlanetsLongAndKundli_Printing_Janam_Patri` | `KundliDetailsBuilder` + SVG/PDF | `Build` / renderers | **Review Required** |
| 78 | `cBhava….BhavaSpastaAndChalitKundli` | `BhavaSpastaService` | `Calculate` | **Review Required** |
| 79 | `cBhava….FnBhava_Calc` | `BhavaSpastaService` | (inside `Calculate`) | **Merged** / **Review Required** |
| 80 | `eDashasAll.DashasCalcAndPrinting` | `VimshottariDashaService` + `ExtraDashaService` | `Calculate` | **Split** / **Review Required** |
| 81 | `eDashasAll.VimshottariDasha` | `VimshottariDashaService` | `Calculate` | **Review Required** |
| 82 | `eDashasAll.PartyantarDasha` | — | — | **Not Migrated** (verify ExtraDasha scope) |
| 83 | `eDashasAll.Ashtottari78…` | `ExtraDashaService` | inside `Calculate` | **Review Required** |
| 84 | `eDashasAll.YoginiDasha` | `ExtraDashaService` | Yogini branch | **Review Required** |
| 85 | `eDashasAll.Shodshottari106Dasha` | `ExtraDashaService` | Shodshottari branch | **Review Required** |
| 86 | `eDashasAll.Panchottari105Dasha` | `ExtraDashaService` | Panchottari branch | **Review Required** |
| 87 | `eDashasAll.Shatabadika100Dasha` | — | — | **Not Migrated** (confirm) |
| 88 | `eDashasAll.Chaturshityabadika84Dasha` | — | — | **Not Migrated** (confirm) |
| 89 | `eDashasAll.Davisapatika72Dasha` | — | — | **Not Migrated** (confirm) |
| 90 | `eDashasAll.Sashathihayani36Dasha` | — | — | **Not Migrated** (confirm) |
| 91 | `eDashasAll.SandhyaDasha` | — | — | **Not Migrated** (confirm) |
| 92 | `eDashasAll.PachakDasha` | — | — | **Not Migrated** (confirm) |
| 93 | `f16Vrg….Varg16KundaliesCalcAndPrinting` | `Varga16Service` | `Calculate` | **Review Required** |
| 94 | `f16Vrg….FnShodashVargCalc` | `Varga16Service` | inside `Calculate` | **Merged** / **Review Required** |
| 95 | `gSpl_Lgn….Special_Lagnas_Calc_Printing` | `SpecialLagnaUpGrahaService` | `CalculateSpecialLagnas` | **Review Required** |
| 96 | `h16….Varg16_Table_Printing` | — / varga result UI | — | **Review Required** |
| 97 | `h16….Graha_Avasthas_Calc_Printing` | `GrahaAvasthaService` | `Calculate` | **Review Required** |
| 98 | `iPanchadha….Panchadha_Matri_Calc` | `PanchadhaKinshukService` | `CalculatePanchadha` | **Review Required** |
| 99 | `iPanchadha….KinshukAdi_Sangya` | `PanchadhaKinshukService` | `CalculateKinshukadi` | **Review Required** |
| 100 | `jUpGrah….UpGrahas_Lagnas_Objects` | `SpecialLagnaUpGrahaService` | `CalculateUpGrahas` | **Review Required** |
| 101 | `kGrahBala….Graha_Bala_Calc_Printing` | `GrahaBalaService` | `Calculate` | **Review Required** |
| 102 | `lAstakVarg.Astak_Varg` | `AshtakavargaService` | `Calculate` | **Review Required** |
| 103 | `oYogas….Astrology_Related_Grammar` | `YogaDetectionService` | `Calculate` | **Partial** / **Review Required** |
| 104 | `oYogas….Astrology_Math` | — / GrahaBala | — | **Review Required** |
| 105 | `oYogas….Manglik_Yoga` | Match manglik note / yogas | — | **Review Required** |
| 106 | `qVarshPhal.VarshPhalTajik` | `VarshPhalService` | `Calculate` | **Review Required** |
| 107 | `rLalKitab.Lal_Kitab` | `LalKitabService` | `Calculate` | **Review Required** |
| 108 | `pNumerology.Numerology` | `LegacyNumerologyService` | `Calculate` | **Review Required** |
| 109 | `pNumerology.FnCalcRootNum` | `LegacyNumerologyService` | `Root` | **Renamed** |
| 110 | `pNumerology.FnCalcNameNumber` | `LegacyNumerologyService` | `CalculateNameNumber` | **Renamed** |
| 111 | `pNumerology.FnCalcFriendship` | `LegacyNumerologyService` | `Friendship` | **Renamed** |
| 112 | `sBioRhythms.BioRhythms` | — | — | **Not Migrated** |
| 113 | `a3Functions_Gen.Uday_Astha` | — | — | **Not Migrated** (table empty / not wired) |
| 114 | `a3Functions_Gen.Print_Kundli_Sng` | `NorthIndianKundliChartRenderer` | `RenderPng` (+ browser `Kun.svg`) | **Review Required** |
| 115 | `a4….Format_RAKV` / `RAKV` / `Kala` | `AstroMath` | `FormatRakv` / `FormatGatiKala` / … | **Renamed** / **Review Required** |
| 116 | `m1Marriage_Muhurata.Marriage_Muhuratas` | `VivahMuhurtaService` | `Search` | **Review Required** |
| 117 | `tTodayzKaalShudhi.TodayzKaalShudhiMain` | Muhurat stack | — | **Review Required** / largely **Not Migrated** |
| 118 | `00_Earth_Sun.Earth_*` | — | — | **Not Migrated** |

---

## 2.6 Web entry points (no VB6 twin)

| C# Class | C# Method | Role |
|----------|-----------|------|
| `HomeController` | `CalculateKundli` | AJAX kundli JSON |
| `KundliController` | `Index` | Simple kundli page |
| `MatchMakingController` | `Index` | Match UI |
| `ReportController` / `PublicReportController` | PDF builders | Letterhead PDF |

---

# 3. Exceptions / Differences

## Renamed (document both)

| Old VB6 Name | New C# Name | Reason |
| ------------ | ----------- | ------ |
| `Basic_Astronomy_Calc` | also `CalculateKundli` | Existing MVC/API; **alias keeps legacy name** |
| `Control_Panel` | `Generate` | Service-oriented naming |
| `Milan_Calc` | `MilanCalc` (private) / entry `Compare` | C# method casing; public API is `Compare` |
| `Rashish_Matri` | `RashishMatri` | C# casing |
| `FindGunaString_FrVb6` | `MatchDetRepository.GetGunaString` | Repository layer |
| `FnPanchang` | `LegacyPanchangCalculator.Calculate` | Class-oriented API |
| `Hora_Sangya` | `HoraSangyaCalculator.FromMoon` | Static helper |
| `FNDG` / `FNR` / … | also `AstroMath.FnDg` / `FnR` | Duplicate helpers for ganit modules (Pascal-ish) |
| `DT2JDF` | `Dt2Jdf` / `julday` | Dual JD implementations — **compare carefully** |
| `Format_RAKV` | `FormatRakv` / `FormatRakvEncoded` | C# naming |
| `FnCalcRootNum` | `Root` | Shorter public API |

## Split

| Old VB6 | New C# |
|---------|--------|
| `Basic_Astronomy_Calc` | `CalculateKundli` → `ComputeCore` ×2 → `BuildResult` / `GetMoon` |
| `Control_Panel` | `Generate` + many `*Service.Calculate` |
| `DashasCalcAndPrinting` | `VimshottariDashaService` + `ExtraDashaService` |
| `PlanetsLongLattDeclRet` (moon) | `GetMoon` extracted |

## Merged

| Old VB6 | New C# |
|---------|--------|
| `PlanetsLongLattDeclRet` + `ST_Lagn_Dasam` + `Ayanamsa_Calc` | `ComputeCore` |
| `FnBhava_Calc` + print | `BhavaSpastaService.Calculate` (calc focus) |
| `FnShodashVargCalc` + print | `Varga16Service.Calculate` |

## Moved

| Logic | From VB6 module | To C# |
|-------|-----------------|-------|
| Match matrix lookup | `nMatchMaking` | `Repositories/MatchDetRepository` |
| Place atlas | `Atlas.frm` / gen helpers | `PlaceRepository` |
| Chart drawing | `Print_Kundli_Sng` | SVG JS + `NorthIndianKundliChartRenderer` |

## Not Migrated (must appear — do not ignore)

- Entire `00_Earth_Sun.bas` VSOP suite  
- `sBioRhythms.BioRhythms`  
- `mMatching.Matching_for_27Nak`  
- Most `nMatchMaking.*_Vichar` secondary match essays  
- Several dasha systems in `eDashasAll` (Shatabadika, Chaturashiti, etc. — confirm against `ExtraDashaService`)  
- `Uday_Astha` live path  
- `SunBriefSyn` / `MoonBriefSyn` / `Lagnodya_Kaal_ZST` / `Godhuli_LagnaTimeZST`  
- `Correction_Delta_T` on primary web JD path (verify)  
- Graphology / YogResearch special forms  

## Logic differences requiring manual verification

| Item | Note |
|------|------|
| `Rashish_Matri(iRasiNum_F, iRasiNum_F)` | VB6 calls female/female; C# preserves — unusual but intentional |
| `FnPanchang` vs `LegacyPanchangCalculator` | Partial display port; sunrise approx |
| `DT2JDF` vs `julday` | Different epochs/helpers exist side by side |
| Tithi/Yoga **Bhukat** | Web port uses full phase/span (documented REVIEW) |
| Print/RTF | VB6 `Text_Ganit` ≠ web HTML/PDF layout |

## Matched with evidence (safe for side-by-side debug first)

| VB6 | C# | Evidence |
|-----|----|----------|
| `Basic_Astronomy_Calc` | `KundliCalculator.Basic_Astronomy_Calc` / `CalculateKundli` | Sample chart longitudes smoke-tested identical after refactor |
| `FNDG`/`FNR`/`FNQ` family | same names in `KundliCalculator` | Formula port + shared use |
| `Milan_Calc` core guna matrix | `MilanCalc` | MatchDet + parihar rules preserved in code review |
| `Rashish_Matri` | `RashishMatri` | Same YES/NO tables |

Everything else in §2 marked **Review Required** until you breakpoint-compare.

---

# 4. Important function cards (manual comparison)

### Card A — Basic astronomy

| Field | Value |
|-------|-------|
| VB6 source | `aBasic_Astronomy.bas` |
| VB6 function | `Basic_Astronomy_Calc` |
| C# source | `Services/KundliCalculator.cs` |
| C# class / method | `KundliCalculator.Basic_Astronomy_Calc` **or** `CalculateKundli` |
| Inputs | day, month, year, hour, minute, longitude, latitude, timezonePacked (HH.MM) |
| Output | `KundliResult` (`dPlt`-equivalent properties + `PlanetPositions`) |
| Important calcs | JD → ayanamsa → sun/planets → moon → lagna/MC → next-day gati |
| Dependencies | Internal helpers `FNDG`, `julday`, `GetMoon`, `ComputeCore` |
| Status | **Matched** (alias) |
| Naming | Prefer `Basic_Astronomy_Calc` when comparing to Indu |
| Logic diff | None intended; JD helper style may differ from VB6 `DT2JDF` — watch `dJDF` first |

### Card B — Planets + Lagna (merged)

| Field | Value |
|-------|-------|
| VB6 | `PlanetsLongLattDeclRet`, `ST_Lagn_Dasam`, `Ayanamsa_Calc` |
| C# | `KundliCalculator.ComputeCore` |
| Status | **Merged** + **Review Required** for line-level float parity with Indu |
| Breakpoints | After `dAyanamsa=`; after `dPlt[2]`; after `GetMoon`; after lagna block `dPlt[1]`/`dPlt[11]` |

### Card C — Match Milan

| Field | Value |
|-------|-------|
| VB6 | `nMatchMaking.bas` → `Milan_Calc` |
| C# | `MatchMakingService.MilanCalc` (via `Compare`) |
| Inputs | Male/female birth → charts → nak/rasi/pada |
| Output | `MatchMakingResult` (8 koots, totals, verdict) |
| Dependencies | `MatchDetRepository.GetGunaString`, `RashishMatri`, `HoraSangyaCalculator.FromMoon` |
| Status | Core **Matched**; extended vichar **Not Migrated** |

### Card D — Panchang

| Field | Value |
|-------|-------|
| VB6 | `FnPanchang` / `Panchang_Calc_and_Print` |
| C# | `LegacyPanchangCalculator.Calculate` |
| Status | **Review Required** (partial) |

---

# 5. How to use this table

```text
1. Pick VB6 function from Indu (e.g. ST_Lagn_Dasam)
2. Find its row in §2
3. Open C# file/method
4. Set breakpoints on both
5. Use same birth data
6. Compare first differing variable (see LEGACY_TRACEABILITY.md § First Difference)
7. Only then change code
```

**Search tip:** In this doc, Ctrl+F the exact VB6 name (`PlanetsLongLattDeclRet`, `Milan_Calc`, …).

---

# 6. Maintenance rule

When you migrate or rename anything:

1. Update the row Status.  
2. Add Rename/Split/Merge to §3.  
3. Never mark **Matched** without opening both implementations (or a numeric smoke test).

---

*End of mapping. Accuracy over completeness of “Matched” claims.*
