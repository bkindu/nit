# Nirmala Astrology — Discovery & Gap Analysis (Refreshed)

Date: 2026-09-13  
Phase: **DISCOVERY ONLY** (no new feature implementation in this document)  
Website: `E:\NirmalaAstrology\NirmalaAstrology_Web`  
VB6: `D:\NirmlaV2 - Copy` (NirmlaV2.vbp / Indu / Nirmla.exe)  
Legacy DB: `Indu.mdb` (Jet) → MySQL `indu_astro` (partial migration)

---

## 1. Existing Website Architecture

```
ASP.NET MVC 5.3 / .NET 4.8
├── Controllers (16) — thin/medium; HomeController is large hub
├── Services — KundliWorkflowService + ganit chain + feature MVPs
├── Repositories — Place, Country, Native, MatchDet, Zodiac, KundliReport
├── DAL — DatabaseHelper (parameterized MySQL)
├── Views — Public _Layout + Workspace _AppLayout
├── Reports — OfficialReportDocument → LetterheadPdfReportRenderer (PdfSharp)
└── Auth — NONE (authentication mode="None")
```

**Shells**
| Shell | Purpose |
|---|---|
| `_Layout.cshtml` | Public marketing + Kundli Result + Numerology + Contact + Payment |
| `_AppLayout.cshtml` | Workspace tools (Birth Data, Match, Muhurat, Lal Kitab, …) |

**Engine:** `WorkingJavascriptKundliCalculator` (Jint + `kundli-reference-engine.js`) is the parity path. Older `KundliCalculator.cs` is superseded for core longitudes.

---

## 2. VB6 Architecture

```
aData_Entry (birth + sChoice)
    → Control_Panel (a1Action_Plan.bas)
        → Basic_Astronomy_Calc + module chain
        → Text_Ganit.frm (RTF ganit)
        → Text_Phalit.frm (judgement texts)
Forms: Muhurat, Record, Atlas, Palmistry, Graphology, YogResearch, …
DB: Indu.mdb (Jet OLEDB) + text files (Past_Birth, Palmistry, GOCHAR)
Auth: none (no login)
```

---

## 3. Complete VB6 Feature Inventory

### Forms
| Form | Role |
|---|---|
| Data_Entry | Hub: birth, language, sChoice, save |
| Text_Ganit | Ganit/kundli RTF + print |
| Text_Phalit | Gems, yog, sade-sati, gochar, nak, past birth, dasha, bhav, life, remedies |
| Muhurat | Kaal shudhi, vivah, shop, grih, rahu, … |
| Record | Search/delete natives |
| Atlas | Place CRUD |
| Palmistry | Palm feature → readings |
| Graphology | Handwriting questionnaire |
| YogResearch | Yoga/life-script research over DB |
| mdi_Nirmala | Unused MDI shell |
| Spl/Display | Stub |

### Modules (calc / business)
Birth astronomy, Panchang, Janam patri, Bhava/Chalit, Vimshottari + alt dashas, 16 Vargas, Special lagnas, Avasthas, Panchadha/Kinshukadi, Upagrahas, Graha Bala, Ashtakavarga, Yogas, Match Making, Melapak-27Nak, Vivah Muhurta, Kaal Shudhi, Varshphal Tajik, Lal Kitab, Numerology, BioRhythms, Palmistry/Upay texts.

### sChoice routes
DETAILED, MEDIUM, KUNDALI, PRASHAN, MATCH_MAKING, VARSH, LALKITAB, MUHURAT, Numerology, Palmistry, Graphology, BioRhythms, Record, Atlas.

---

## 4. Existing Website Feature Inventory

| Area | Controllers / Services | Status |
|---|---|---|
| Marketing Home | Home | Complete |
| Zodiac encyclopedia | Zodiac | Complete |
| Birth Data hub | BirthData | Partial |
| Kundli chart + Result UI | Home/Kundli + Workflow | Partial (strong screen) |
| DETAILED ganit chain | Bhava…Yoga + ExtraDasha | Partial (wired; parity TBD) |
| Official PDF Report | Report + LetterheadPdf | Partial (Kundli subset) |
| Match Making | MatchMaking | Partial |
| Numerology | Numerology + LegacyNumerology | Partial (no DB phalit) |
| Records | Records | Partial |
| Muhurat | Muhurat | MVP |
| Vivah Muhurta | VivahMuhurta | MVP (JSON 2016–18) |
| Gochar | Gochar | MVP (no RasPrves windows) |
| Varsh Phal | VarshPhal | MVP |
| Lal Kitab | LalKitab | MVP |
| Palmistry | Palmistry | MVP (JSON) |
| Contact / Payment | Contact, Payment | Partial / MVP |
| Places | AddPlace modal only | Partial |
| Auth / Admin area | — | **Missing** |
| Graphology, BioRhythms, Prashan, YogResearch | — | **Missing** |
| Text_Phalit encyclopedia (full) | KundliReport subset | Partial / Missing depth |
| Eng/Hindi/Punjabi UI | — | Missing (en only) |

---

## 5. Missing Features (VB6 → Website)

| Feature | Priority |
|---|---|
| Authentication + Admin/Public roles | P0 (security / product split) |
| Full Text_Phalit (dasha phal, bhav phal, gems, past birth files, life script, remedies UI) | P0 |
| Sade-Sati periods (RasPrves Saturn) | P1 |
| Gochar entry/exit windows (RasPrves9plts) | P1 |
| Full Muhurat / Kaal Shudhi suite | P1 |
| Rashi-wise Vivah (Dates ID + intersection) | P1 |
| Graphology | P2 |
| BioRhythms | P2 |
| Prashan Kundli | P2 |
| YogResearch over natives | P2 |
| Atlas edit/delete | P2 |
| Melapak 27-nak print page | P2 |
| Multi-language readings (hi/pa) | P2 |
| PDF for Match / Varsh / Lal Kitab / Numerology / Palmistry | P1 |
| Report permission registry | P0 (with auth) |

---

## 6. Incorrect / Partial Features

| Feature | Issue |
|---|---|
| Sunrise/sunset | Approximated vs VB6 astronomy |
| DETAILED PDF | Omits many screen sections (varga, ashtakavarga, graha bala, extra dashas) |
| Numerology meanings | Hardcoded; VB6 uses `Numerology_Reading` |
| Gochar | Single transit date; not Jantri Saahe windows |
| Vivah data | Static 2016–2018 JSON seed |
| Dual schemas | `Customer`/`Place` vs `tbl_native`/`tbl_place` |
| Shell split | Some “admin” tools still use public `_Layout` |
| Records Delete | Publicly callable (no auth) |
| Gap doc dated 2026-09-12 | Marked many modules Missing that are now MVP |

---

## 7. Public Feature Matrix (proposed)

| Feature | Public |
|---|---|
| Home / Zodiac / Contact / Payment | Yes |
| Basic / Limited Kundli (chart + summary) | Yes (limited sections) |
| Limited Numerology (core numbers) | Yes |
| Official full DETAILED PDF | **No** (Admin) |
| Lal Kitab / Gochar / Varsh / Palmistry / Match full | **No** |
| Records / Delete / Add Place | **No** |
| Muhurat / Vivah full | **No** (optional: Abhijit/Rahu only later) |

---

## 8. Admin Feature Matrix (proposed = full VB6 scope)

| Feature | Admin target |
|---|---|
| Birth Data + all report choices | Full |
| Kundli KUNDALI / MEDIUM / DETAILED / PRASHAN | Full |
| All ganit modules + phalit texts | Full |
| Match Making + Melapak | Full |
| Muhurat + Vivah + Kaal Shudhi | Full |
| Varsh Phal / Lal Kitab / Numerology / Palmistry | Full |
| Gochar / Sade-Sati / Past Birth | Full |
| Records + Atlas CRUD | Full |
| Graphology / BioRhythms / YogResearch | Full |
| Official multi-report PDF/Print | Full |

---

## 9. Astrology Engine Mapping

| VB6 | Website service |
|---|---|
| a2/aBasic/00_Earth_Sun | WorkingJavascriptKundliCalculator |
| Control_Panel | KundliWorkflowService |
| cBhava… | BhavaSpastaService |
| eDashasAll | VimshottariDashaService + ExtraDashaService |
| f16Vrg… | Varga16Service |
| lAstakVarg | AshtakavargaService |
| i/j/k/h/g/o… | Panchadha, UpGraha, GrahaBala, Avastha, SpecialLagna, Yoga |
| nMatchMaking | MatchMakingService |
| pNumerology | LegacyNumerologyService |
| qVarshPhal | VarshPhalService (MVP) |
| rLalKitab | LalKitabService (MVP) |
| tTodayzKaalShudhi | MuhuratService (MVP) |
| m1Marriage… | VivahMuhurtaService (MVP) |
| Palmistry.frm | PalmistryService (MVP) |
| Text_Phalit | KundliReportRepository (subset) |
| Print RTF | LetterheadPdfReportRenderer (Kundli only) |

**Rule:** One engine; Public/Admin differ by **authorization + report sections**, not by forked formulas.

---

## 10. Database Changes Required

| Need | Action |
|---|---|
| Auth users/roles | New tables or ASP.NET Identity |
| Report permissions | `tbl_report_definition` (code, public_allowed, admin_allowed, enabled, limits) |
| Numerology readings | Wire `tbl_numerology_reading` |
| Dasha/Yog/Misc phalit | Wire `tbl_dasa_reading`, `tbl_yoga`, `tbl_misc`, past-birth files |
| RasPrves gochar | Migrate/use `tbl_rashi_pravesh` |
| Vivah | Prefer MySQL over JSON; refresh Saahe years |
| Unify Customer vs tbl_native | Single native model |
| Secrets | Move connection string password out of committed Web.config |

---

## 11. Report Mapping

| Report | Screen | Official PDF | Print |
|---|---|---|---|
| Kundli DETAILED | ✓ Partial/Full UI | ✓ Partial sections | ✓ via PDF |
| Match Making | ✓ | ✗ | ✗ |
| Numerology | ✓ | ✗ | ✗ |
| Lal Kitab | ✓ MVP | ✗ | ✗ |
| Varsh Phal | ✓ MVP | ✗ | ✗ |
| Muhurat / Vivah | ✓ MVP | ✗ | ✗ |
| Gochar | ✓ MVP | ✗ | ✗ |
| Palmistry | ✓ MVP | ✗ | ✗ |
| Phalit packs | ✗/partial | ✗ | ✗ |

Template architecture already started: `OfficialReportDocument` → renderer → Preview/PDF/Print.

---

## 12. PDF / Print Requirements

| Requirement | Status |
|---|---|
| Not form screenshot | ✓ Kundli official path |
| Letterhead PNG | ✓ `Indu_Visiting_LetterHead.png` |
| Selectable text A4 | ✓ PdfSharp |
| Page numbers | ✓ |
| Expand sections to match DETAILED screen | Pending |
| Same pipeline for all modules | Pending |
| Deprecate `window.print()` form export | Mostly done for Kundli; CSS letterhead print still present |

---

## 13. Security / Authorization Plan

**Current risk:** Entire workspace (Records delete, AddPlace, full DETAILED) is open.

**Plan**
1. ASP.NET Identity (or Forms Auth) with roles: `Public`, `Admin` (+ optional `SuperAdmin`)
2. `[Authorize(Roles="Admin")]` on `/Admin/*` and current workspace controllers (or Area)
3. Public controllers: Home (limited), Zodiac, Contact, Payment, limited Kundli/Numerology
4. `IReportAccessService` checks report code + role + usage limits
5. Keep AntiForgery + parameterized SQL; fix open delete; secret store for DB password
6. Never trust client-only hiding of Admin nav

---

## 14. Implementation Plan (post-discovery)

### Phase A — Access model (P0) — **DONE 2026-09-13**
- [x] Forms Auth + Admin/Public roles (MySQL `tbl_app_user` / roles; PBKDF2)
- [x] `/Admin` Area dashboard + `[AdminAuthorize]` on workspace controllers
- [x] Report permission table + `ReportAccessService` (`KUNDLI_DETAILED` admin-only)
- [x] Public limited: Home / Zodiac / Contact / Payment / Quick Kundli / Numerology; Official PDF UI admin-gated
- Default seed: `admin` / `Nirmala@1974` (change in production)

### Phase B — Complete Admin horoscope (P0/P1)
- [ ] Expand Official PDF to full DETAILED sections
- [ ] Wire remaining phalit tables + past birth
- [ ] Sade-Sati + Gochar RasPrves
- [ ] Numeric parity suite (08.02.1981 Patiala reference)

### Phase C — Complete Admin modules (P1)
- [ ] Muhurat full / Vivah rashi-wise
- [ ] Varsh / Lal Kitab depth to VB6
- [ ] Numerology DB readings + PDF
- [ ] Match PDF; Melapak page

### Phase D — Remaining VB6 (P2)
- [ ] Graphology, BioRhythms, Prashan, YogResearch
- [ ] Atlas CRUD
- [ ] Multi-language readings

### Phase E — Hardening / regression
- [ ] Security pass, regression vs VB6 samples, PDF/print consistency

---

## Final Feature Matrix (live)

| Feature | VB6 | Website | Public (proposed) | Admin (proposed) | Status |
|---|---|---|---|---|---|
| Birth Data | ✓ | ✓ | Limited | Full | Partial |
| Kundli chart | ✓ | ✓ | Limited | Full | Partial |
| DETAILED ganit | ✓ | ✓ | No | Full | Partial |
| Phalit texts | ✓ | ⚠ | No | Full | Partial |
| Match Making | ✓ | ✓ | No | Full | Partial |
| Numerology | ✓ | ✓ | Limited | Full | Partial |
| Lal Kitab | ✓ | ✓ MVP | No | Full | MVP |
| Varsh Phal | ✓ | ✓ MVP | No | Full | MVP |
| Muhurat | ✓ | ✓ MVP | Optional limited | Full | MVP |
| Vivah Muhurta | ✓ | ✓ MVP | No | Full | MVP |
| Gochar / Sade-Sati | ✓ | ⚠/MVP | No | Full | MVP |
| Palmistry | ✓ | ✓ MVP | No | Full | MVP |
| Records / Atlas | ✓ | ⚠ | No | Full | Partial |
| Graphology | ✓ | ✗ | No | Full | Missing |
| BioRhythms | ✓ | ✗ | No | Full | Missing |
| YogResearch | ✓ | ✗ | No | Full | Missing |
| Prashan | ✓ | ✗ | No | Full | Missing |
| Official PDF | RTF | ✓ Kundli | Limited | Full | Partial |
| Auth / Admin split | ✗ | ✓ Forms + Area | Public site | Full workspace | **Done (Phase A)** |

---

## Recommendation — Next Implementation Slice

**Phase A complete.** Next coding slice:

1. **Expand Kundli Official PDF** to match DETAILED screen  
2. **Phalit / Gochar-SadeSati depth**  
3. Optional: Admin UI to edit report permissions / change password
