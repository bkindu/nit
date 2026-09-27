# Legacy Verification Report — Nirmala Astrology Web

**Legacy source of truth:** `D:\NirmlaV2` (VB6)  
**New application:** `E:\NirmalaAstrology\NirmalaAstrology_Web`  
**Rule:** Do not invent business logic; preserve VB6 / database / Kun.svg behavior.

---

## Phase status

| Phase | Focus | Status |
|-------|--------|--------|
| 1 | Structure / module map | Done (baseline) |
| 2 | Admin isolation | Fixed (nav + redirects + Numerology authorize) |
| 3 | Module-by-module | Partial — continue per module |
| 4 | Match Making | Partial — Rashish Maitri restored to VB6 call |
| 5 | Kundli diamond SVG / PDF placement | Fixed house centers to Kun.svg |
| 6 | Report field checklist | Needs reference PDFs + sample runs |
| 7 | Final QA | In progress |

---

## Admin

| Check | Status | Notes |
|-------|--------|--------|
| Admin login → Dashboard | PASS | `AuthController` redirects Admin role |
| Admin-only sidebar | PASS | Removed Contact / Payment / Public site from `_AppLayout` |
| Brand → Admin Home | PASS | Links to `Admin/Dashboard` |
| Public Home blocked for Admin | PASS | `HomeController.Index` redirects Admin |
| Contact / Payment URLs for Admin | PASS | Redirect to Admin Dashboard |
| Kundli Result chrome | PASS | Admin uses `_AppLayout`; public uses `_Layout` |
| Server-side `[AdminAuthorize]` | PASS | BirthData, MatchMaking, Report, Muhurat, etc. |
| Numerology | PASS | Now `[AdminAuthorize]` |
| Detailed reports | PASS | `ReportController` + `ReportAuthorize` |

---

## Module map (legacy → new)

| Module | Legacy | New |
|--------|--------|-----|
| Kundli / Spashta | `a3Functions_Gen.bas`, print helpers | `kundli-reference-engine.js` + Jint `WorkingJavascriptKundliCalculator` |
| Diamond chart UI | `Print_Kundli_Sng` / print layout | `Content/Kun.svg` + `kundli-chart-svg.js` |
| Diamond chart PDF | Same house order | `NorthIndianKundliChartRenderer` (centers = Kun.svg) |
| Match Making | `nMatchMaking.bas` `Milan_Calc` | `MatchMakingService` + `tbl_match_det` |
| Guna matrix | Access `MatchDet` | MySQL `tbl_match_det` via `MatchDetRepository` |
| Places | Indu.mdb places | Place repositories / MySQL |

---

## Kundli / SVG / PDF

### House geometry (verified)

VB6 / Kun.svg (not the old mirrored PDF layout):

| House | Role | Kun.svg approx (x,y) in 400×400 |
|-------|------|----------------------------------|
| 1 | Center Lagna | 188, 168 |
| 2 | Top-left | 96, 82 |
| 3 | Left | 64, 109 |
| 4 | Left-center | 161, 208 |
| 5 | Bottom-left | 68, 304 |
| 6 | Bottom-leftish | 94, 328 |
| 7 | Bottom | 193, 242 |
| 8 | Bottom-right | 291, 334 |
| 9 | Lower-right | 322, 306 |
| 10 | Right-center | 224, 209 |
| 11 | Upper-right | 312, 107 |
| 12 | Top-right | 283, 78 |

**Fix applied:** `NorthIndianKundliChartRenderer.BuildHouseCenters` now scales these anchors. Kun.svg design unchanged; only data placement mapping corrected.

**UI binding:** `kundli-chart-svg.js` binds `h1`–`h12` rashis and planet IDs `s`/`c`/`m`/… + house number — preserve this.

---

## Match Making

### Flow (legacy)

```
Birth details (M/F)
 → Kundli / Moon lon, Nak, Rashi, Pada
 → MatchDet row/col (36×36)
 → 8-koot guna string
 → Navamsa maitri + Rashi maitri
 → Parihar / cancellations
 → Totals + verdict
 → Report
```

### Database

| Field | Source |
|-------|--------|
| Guna string (8 chars) | `tbl_match_det.id` = row; `field_01`…`field_36` = column |
| Legacy table | Access `MatchDet` |

### Business-rule parity notes

| Item | Legacy | New (after fix) |
|------|--------|-----------------|
| Rashi maitri | `Rashish_Matri(iRasiNum_F, iRasiNum_F)` | Same (female, female) — **do not “correct”** |
| Navamsa maitri | `Rashish_Matri(GirlNav, BoyNav)` | `RashishMatri(navamsaGirl, navamsaBoy)` |
| Matrix row/col | Female row, Male col with 1/2/3 correction | Same formula in `MilanCalc` |

**Still needs:** side-by-side sample vs VB6 output + uploaded reference match report (no reference PDF present in repo at verification time).

---

## Report field checklist (vs uploaded references)

Reference PDFs were not found under the web project or workspace copy at this pass. When available, fill:

| Section | Reference | Current | Status |
|---------|-----------|---------|--------|
| Personal Details | Present | Verify | Needs Verification |
| Birth Details | Present | Verify | Needs Verification |
| Rashi / Nakshatra | Present | Verify | Needs Verification |
| Graha Spashta | Present | Partial (RAKV/Gati work done earlier) | Needs Verification |
| Diamond Kundli | Present | House centers fixed | Needs PDF retest |
| Match Making | Present | Logic parity restored for maitri | Needs Verification |
| Recommendations | Present | Verify | Needs Verification |

---

## Important field → source mapping (sample)

| Field | Report section | Table / source | Legacy | New |
|-------|----------------|----------------|--------|-----|
| Guna string | Match Making | `tbl_match_det` | `FindGunaString_FrVb6` | `MatchDetRepository.GetGunaString` |
| Moon Rashi | Match / Horoscope | Calculated from Moon lon | `iRasiNum_*` | Profile `MoonRashi` |
| Nak / Pada | Match | Hora from calc | `iNakNum_*` | `Hora.NakshatraNumber` / `Pada` |
| House rashi on chart | Kundli | Lagna + house offset | Print_Kundli_Sng | `HouseData` + Kun.svg / PDF renderer |
| Planet in house | Kundli | Bhava from calc | Print slots | SVG IDs / `FormatPlanets` |

---

## QA matrix (current)

| Module | Status |
|--------|--------|
| Admin | PASS (isolation changes applied) |
| Authentication | PASS |
| Authorization | PASS (core Admin modules) |
| Match Making | NEEDS VERIFICATION (parity fix applied; sample compare pending) |
| Kundli | NEEDS VERIFICATION |
| Rashi Calculation | NEEDS VERIFICATION (engine work prior) |
| Graha Calculation | NEEDS VERIFICATION (engine work prior) |
| SVG Mapping | PASS (UI Kun.svg); PDF centers PASS (code) |
| PDF Generation | NEEDS RETEST after house-center fix |
| Report Completeness | FAIL / incomplete until reference PDFs compared |
| Database Mapping | PARTIAL (`MatchDet` mapped) |
| Regression Testing | PENDING rebuild + smoke |

---

## Fixes applied this pass

1. PDF diamond house centers aligned to Kun.svg / VB6 (not mirrored H1-top layout).
2. Admin workspace: remove public Contact/Payment/Public site; Admin Home breadcrumb/brand.
3. Admin redirected away from public Home, Contact, Payment.
4. Kundli Result uses `_AppLayout` when Admin.
5. Numerology locked with `[AdminAuthorize]`.
6. Match Making Rashi Maitri restored to VB6 `Rashish_Matri(female, female)`.

---

## Known open items

1. Full field-by-field compare vs uploaded reference reports (need files).
2. End-to-end Match Making sample vs VB6 for same birth pair.
3. Rebuild + regenerate Basic Horoscope PDF and visually confirm 12 houses.
4. Confirm public free-report path still uses correct chart after PDF fix.
5. Continue module-by-module (Muhurat, Gochar, Varsh Phal, Lal Kitab, Vivah) without changing logic.
