# Legacy MDB → MySQL Verification & Performance

**Legacy MDB:** `D:\NirmlaV2\Indu.mdb` (29 user tables)  
**Local MySQL:** `indu_astro` (via `App_Data/connection.local.config`)  
**Deploy schema pack:** `App_Data/bigrock/BIGROCK_PROJECT_ONLY.sql`  
**Legacy code:** `D:\NirmlaV2`  
**Rule:** Same business results; optimize retrieval only.

---

## 1. Module → MDB dependency map

| Module | Legacy entry | MDB tables | MySQL | App code |
|--------|--------------|------------|-------|----------|
| Atlas / Place | `a3Functions_Gen.bas`, `Atlas.frm` | `Place` | `tbl_place` (+ legacy `Place`) | `PlaceRepository` / `sp_SearchPlaces` |
| Natives / Records | `Data_Entry.frm`, `Record.frm` | `Record_F_Natives` | `tbl_native` | `NativeRepository` |
| Match Making | `nMatchMaking.bas`, `mMatching.bas` | `MatchDet` | `tbl_match_det` | `MatchMakingService` + `MatchDetRepository` |
| Phalit / readings | `Text_Phalit.frm` | `Sign_Reading`, `House_Reading`, `Nak27_Reading`, `NakCharan108_Reading`, `Plt9xHs12_Reading`, `Plt9xSign12_Reading`, `Brg_Reading`, `Dasa_Reading`, `Misc`, `Yogas`, `Upay`, `Lrd12xHs12_Reading` | Flat copies **or** normalized `tbl_*_reading` | `KundliReportRepository`, services |
| Gochar | `Text_Phalit` + calc | `Plt9xHs12_Reading`, `RasPrves9plts1950_2100`, `Gochar_Phal8Plts` | `tbl_planet_house_reading`, `RasPrves…`, `Gochar_Phal8Plts` | `GocharPhalService` (readings OK; Jantri windows deferred) |
| Udaya/Astha | `a3Functions_Gen.bas` | `Uday_Astha`, `Combustion_4_Uday_Astha` | Same flat tables; **`tbl_udaya_astha_rule` empty** | Gap for normalized path |
| Numerology | `pNumerology.bas` | `Numerology_Reading` | `Numerology_Reading` | `LegacyNumerologyService` |
| Lal Kitab | `rLalKitab.bas` | `Lal_Kitab_Digits`, `Plt9xHs12_Reading` | Same / normalized | `LalKitab` services |
| Vivah Muhurta | `m1Marriage_Muhurata.bas` | `VivahMuhurtaDates`, `VivahMuhurtaPuja` | Same | Vivah module |
| Palmistry | `Palmistry.frm` | `Palmistry`, `Brg_Reading`, … | Tables present; service may use JSON export | `PalmistryService` |
| Kundli astronomy | Calc modules | **Not from MDB** (ephemeris/math) | N/A | `kundli-reference-engine.js` |
| Web Zodiac pages | N/A (not in Indu.mdb) | — | `Zodiac` **0 rows** | Static fallback in `HomeController` |
| Auth / appointments | N/A (new) | — | `tbl_app_*`, `tbl_appointment`, … | New commerce |

---

## 2. Database migration map (MDB → MySQL)

| Legacy MDB | Rows (MDB) | MySQL | Rows (local) | Status |
|------------|-----------:|-------|-------------:|--------|
| Place | 886 (873 unique Place\|Country) | `tbl_place` | 871 unique | ⚠️ 2 junk keys missing (`APATIALA`, `rr`); PATIALA verified |
| MatchDet | 36 | `tbl_match_det` | 36 | ✅ Field1↔field_01 sample match |
| Record_F_Natives | 3720 | `tbl_native` | 3719 | ⚠️ −1 row |
| Brg_Reading | 44 | `Brg_Reading` | 44 | ✅ |
| Dasa_Reading | 94 | `Dasa_Reading` | 94 | ✅ |
| Dasa_Lordship_Reading | 108 | `Dasa_Lordship_Reading` | 108 | ✅ |
| Uday_Astha | 1262 | `Uday_Astha` | 1262 | ✅ flat; normalized rule table empty |
| Combustion_4_Uday_Astha | 1262 | `Combustion_4_Uday_Astha` | 1262 | ✅ |
| Yogas | 214 | `Yogas` | 214 | ✅ |
| Misc | 182 | `Misc` | 182 | ✅ |
| Palmistry | 101 | `Palmistry` | 101 | ✅ |
| Upay | 9 | `Upay` | 9 | ✅ |
| Numerology_Reading | 71 | `Numerology_Reading` | 71 | ✅ |
| Lal_Kitab_Digits | 110 | `Lal_Kitab_Digits` | 110 | ✅ |
| Lrd12xHs12_Reading | 144 | `Lrd12xHs12_Reading` | 144 | ✅ |
| RasPrves9plts1950_2100 | 32101 | `RasPrves9plts1950_2100` | 32101 | ✅ data; ⚠️ no Planet index (full scan) |
| VivahMuhurtaDates | 25 | `VivahMuhurtaDates` | 25 | ✅ |
| VivahMuhurtaPuja | 190 | `VivahMuhurtaPuja` | 190 | ✅ |
| Gochar_Phal8Plts | 12 | `Gochar_Phal8Plts` | 12 | ✅ |
| Aspect_of_Planets | 27 | `Aspect_of_Planets` | 27 | ✅ |
| Graha_Yuti | 35 | `Graha_Yuti` | 35 | ✅ |
| Mutual_Aspects_of_Plts | 245 | `Mutual_Aspects_of_Plts` | 245 | ✅ |
| Sign_Reading_Rnd | 101 | `Sign_Reading_Rnd` | 101 | ✅ |
| House_Reading | 36 | **no flat** → `tbl_house_reading` | 36 | ✅ via normalize |
| Nak27_Reading | 28 | **no flat** → `tbl_nakshatra_reading` | 29 | ⚠️ +1 vs MDB |
| NakCharan108_Reading | 109 | **no flat** → `tbl_nakshatra_charan_reading` | 322 | ⚠️ multi-language expand |
| Plt9xHs12_Reading | 109 | **no flat** → `tbl_planet_house_reading` | 324 | ⚠️ language expand |
| Plt9xSign12_Reading | 108 | **no flat** → `tbl_planet_sign_reading` | 108 | ✅ |
| Sign_Reading | 13 | **no flat** → `tbl_sign_reading` | 36 | ⚠️ language expand |
| *(web)* Zodiac | n/a | `Zodiac` | **0** | ⚠️ empty; app uses static list |
| *(new)* | — | `tbl_udaya_astha_rule` | **0** | ❌ not populated |

### Sample data verification

| Check | Result |
|-------|--------|
| MatchDet ID=1 Field1…Field8 | MDB `12345670/3`, `12325578/7`, … = MySQL `field_01`…`field_08` |
| PATIALA | MDB Latt=30.19 Long=76.25 TZ=5.3 = MySQL same |
| Place search EXPLAIN | Uses `uq_place_country` (range) — OK |

---

## 3. Missing / incorrect objects

### Missing as flat legacy tables (intentionally normalized)

`House_Reading`, `Nak27_Reading`, `NakCharan108_Reading`, `Plt9xHs12_Reading`, `Plt9xSign12_Reading`, `Sign_Reading`, `MatchDet`, `Record_F_Natives`

→ Use `tbl_*` equivalents. Confirm language-code filtering matches VB6 column language (E/H/P).

### Data gaps

1. **`tbl_udaya_astha_rule` = 0** while `Uday_Astha` has 1262 — `KundliReportRepository` reads the empty table.  
2. **`Zodiac` = 0** — web content only; not in Indu.mdb.  
3. **2 Place keys** not in MySQL: `APATIALA|INDIA`, `rr|INDIA` (likely typos/junk).  
4. **`tbl_native` −1** vs `Record_F_Natives`.  
5. **RasPrves** only `PRIMARY(_row_id)` — Planet filters scan ~32k rows.

### Recommended (not auto-applied until approved on prod)

Script: `App_Data/optimize_indexes_from_mdb_usage.sql`

---

## 4. Function mapping

| Legacy | MDB SQL | New | MySQL |
|--------|---------|-----|-------|
| `FindGunaString_FrVb6` | `Select FieldN from MatchDet where ID=row` | `MatchDetRepository.GetGunaString` | `tbl_match_det` (**in-memory cache after first load**) |
| Place lookup | `Select * from Place where Place='…'` | `PlaceRepository.SearchPlaces` / GetById | `tbl_place` (+ SPs) |
| Native CRUD | `Record_F_Natives` | `NativeRepository` | `tbl_native` |
| Sign/House/Nak readings | per-table SELECT | `KundliReportRepository.Enrich` | batched multi-SELECT on `tbl_*` |
| Gochar text | `Plt9xHs12_Reading` | `GocharPhalService` | `tbl_planet_house_reading.gochar_reading` |
| Numerology text | `Numerology_Reading` | numerology services | `Numerology_Reading` |
| Vivah | Vivah* tables | Vivah services | same names |

---

## 5. Stored procedures (existing)

| SP | Purpose | Legacy equivalent |
|----|---------|-------------------|
| `sp_SearchPlaces` | Place autocomplete | Place LIKE queries |
| `sp_GetPlaceById` | Single place | Place by key |
| `sp_InsertPlace` | Add place | Atlas insert |
| `sp_GetAllCountries` | Country list | distinct Country |
| `sp_GetAllZodiacs` / ById / ByName | Web Zodiac | *(new; table empty)* |

**New SPs this pass:** none required after MatchDet matrix cache (single SELECT * once).  
Optional later: `sp_GetMatchDetMatrix` if multi-server cache sync needed.

---

## 6. Performance

### Measured (local `indu_astro`)

| Function | Before | After | Improvement |
|----------|-------:|------:|------------:|
| RasPrves `WHERE Planet='SAT'` | EXPLAIN ALL ~32275 rows | EXPLAIN ref `ix_ras_planet` ~131 rows | ~99% fewer rows examined |
| Match Making guna lookup | 1 SQL per milan | 0 SQL after warm cache | 1×36 matrix load once |
| Place search `Pat%` | range on unique | + name/country indexes | OK |

Indexes applied on local DB (2026-09-15). Re-run `App_Data/optimize_indexes_from_mdb_usage.sql` on BigRock `nit` if not yet applied.  
Note: `Misc.Description` / `Numerology_Reading.Num` prefix indexes skipped (column type incompatible with prefix length).

---

## 7. Match Making DB flow (verified path)

```
Birth M/F → calc Moon/Nak/Rashi/Pada (engine, not MDB)
 → row/col into MatchDet (legacy formula)
 → tbl_match_det field_XX  [cached]
 → Milan_Calc parihaar (VB6 parity incl. Rashish_Matri(F,F))
 → result / report
```

MatchDet **data** verified for ID=1 against MDB.

---

## 8. Kundli / Rashi / Graha

| Item | Source |
|------|--------|
| Positions, houses, degrees | Calculation engine (legacy math), **not** MDB |
| Place lat/lon/tz | `tbl_place` ← `Place` |
| Reading text | Normalized `tbl_*_reading` ← Access reading tables |
| Udaya/Astha status | Should use `Uday_Astha` JD logic or populated rules — **gap** |

---

## 9. Final verification board

```
========================================
LEGACY → MYSQL VERIFICATION
========================================
MDB Schema Verification       PASS (29 tables listed)
MySQL Schema Verification     PASS (with noted normalizations)
Data Verification             PARTIAL (counts OK; 2 places, −1 native, empty Zodiac/UA rules)
Function Verification         PARTIAL (core Place/Match/Native/Readings mapped)
Calculation Verification      N/A for MDB (calc outside DB) — see engine work
Match Making                  PASS (matrix data + cache; logic parity separate)
Kundli                        PARTIAL (calc OK; UA rule table empty)
Report Data                   PARTIAL (normalized readings; language QA pending)
PDF Generation                N/A this pass
Stored Procedures             PASS (existing place/zodiac SPs)
Index Optimization            PASS (local; RasPrves 32275→131 rows)
API Performance               PARTIAL (MatchDet + RasPrves improved)
Site Performance              PARTIAL
Regression Testing            PENDING rebuild smoke
========================================
```

---

## 10. Fixes / deliverables this pass

| Item | Type |
|------|------|
| `MatchDetRepository` in-memory matrix cache | Code |
| Indexes on RasPrves, Place, Brg/Dasa/Palmistry/… | Local MySQL applied |
| `App_Data/optimize_indexes_from_mdb_usage.sql` | SQL for BigRock/other envs |
| This document | Docs |

### Remaining issues (do not guess)

1. Populate or rewire **Udaya/Astha** to `Uday_Astha` JD lookup (legacy).  
2. Seed **Zodiac** for web or keep static (not in MDB).  
3. Run index script on `indu_astro` and BigRock `nit`.  
4. Spot-check Hindi/Punjabi reading columns vs `language_code`.  
5. Confirm missing native row if historical parity required.  
6. Ignore or delete junk places `APATIALA` / `rr` unless business needs them.
