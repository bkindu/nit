# Nirmala Astrology — UI/UX Design Specification

## Design direction

The current interface uses a strong purple/gold visual language, but the hierarchy is visually heavy and the same decorative treatment is applied to too many surfaces. The redesigned experience keeps the astrology identity while making the product calmer, faster to scan and easier to operate during data entry and report review.

The recommended direction is **light-first with dark focus surfaces**:

- **Light mode:** off-white canvas, white cards, ink text and indigo-violet navigation accents.
- **Dark mode:** deep navy canvas, elevated blue-violet cards and warm gold highlights.
- **Focus surfaces:** the Kundli chart, calculation status and primary actions use dark indigo panels in both modes.
- **Accent:** gold is reserved for primary actions, active state, chart markers and important results rather than being used for every heading.

## Product goals

1. Make Birth Data entry feel like a guided workflow rather than a long form.
2. Make the Kundli result page easy to scan before opening the detailed report.
3. Keep VB6 parity fields available without overwhelming the first screen.
4. Make calculation state visible: input confirmed, calculating, calculated, warning or error.
5. Improve mobile behavior without changing the calculation or report contracts.
6. Preserve PDF/print output as a separate document experience.

## Recommended information architecture

| Area | Primary screen | Main user goal |
|---|---|---|
| Dashboard | Home | Start a new Kundli or open a recent record |
| Calculation | Birth Data | Enter and validate birth particulars |
| Preview | Kundli Result | Scan the chart, key placements and Panchang |
| Detail | Report Preview | Read or export the official report |
| Tools | Numerology, Gochar, Vivah Muhurta, Match Making | Use specialist calculators |
| Knowledge | Zodiac, Articles | Browse educational content |
| Account | Login, Register, Records, Account | Manage saved natives and access |

## Main workflow

```text
Dashboard
  -> New Kundli
  -> Birth Data: Person
  -> Birth Data: Time & Place
  -> Birth Data: Report choice
  -> Validate and calculate
  -> Kundli Result: chart + summary
  -> Detailed Report / PDF / Print
```

## Screen designs

### 1. Dashboard / Home

- Compact top bar with logo, primary nav and mode switch.
- Hero should not consume most of the viewport; use a focused two-column start panel.
- Left: welcome copy, `Create Kundli` primary CTA and `Open saved record` secondary CTA.
- Right: dark indigo “Quick start” card with three steps and a small zodiac ring.
- Below: recent records, service cards and a quiet educational/article strip.

### 2. Birth Data — guided entry

Replace the single dense form with a three-step stepper:

1. **Person** — name, nickname, father name, gender.
2. **Time & place** — date, time, AM/PM, birth place search, read-only coordinates and timezone.
3. **Report** — Kundli, Detailed Horoscope, Medium Horoscope or Match Making; save record option.

Design rules:

- Use a two-column desktop grid and one-column mobile layout.
- Keep labels above fields; never rely on placeholder text as the label.
- Show a small green “Location confirmed” status after a place is selected.
- Show timezone as a visible badge such as `IST · UTC +05:30`.
- Keep `Generate Kundli` sticky at the bottom of the form on long screens.
- Use inline validation and one summary message at the top for submission errors.
- Keep advanced coordinates collapsed behind `Edit coordinates` so the normal flow remains simple.

### 3. Kundli result

Use a dashboard layout rather than a long document wall:

- Header: native name, birth place, date/time and calculation status.
- Left main panel: North Indian chart in a dark focus card.
- Right rail: Ascendant, Moon sign, Sun sign, Nakshatra and Ayanamsha cards.
- Below: tabs for `Panchang`, `Planets`, `Houses`, `Hora`, `Ghat`, `Miscellaneous`.
- Keep all VB6 parity fields in the corresponding tabs; do not delete fields to simplify the UI.
- Add `Detailed Report`, `Export PDF`, `Print` and `New calculation` actions.

### 4. Report preview

- Keep the official report visually distinct from the app dashboard.
- Use a light paper preview with a narrow action bar above it.
- Use a full-width iframe/PDF preview only on the report screen.
- Do not make the PDF viewer the only way to inspect calculation results.

### 5. Records

- Search-first layout with name, place, date and last updated filters.
- Table on desktop, stacked cards on mobile.
- Each record has `Open`, `Recalculate`, `Print` and `Delete` actions with a confirmation step only for delete.

## Design tokens

### Color palette

| Token | Light mode | Dark mode | Use |
|---|---|---|---|
| Canvas | `#F7F8FC` | `#0B1020` | Page background |
| Surface | `#FFFFFF` | `#121A2D` | Cards and panels |
| Surface raised | `#F1F3F9` | `#18233B` | Nested cards |
| Ink | `#15213A` | `#F4F6FB` | Main text |
| Ink muted | `#60708D` | `#AAB6CC` | Supporting text |
| Primary | `#4C3FBF` | `#8C7CFF` | Links and active states |
| Primary deep | `#30277D` | `#30266F` | Focus panels |
| Gold accent | `#C9972B` | `#F3C969` | Primary CTA and astrology markers |
| Success | `#1F9D72` | `#4FD1A1` | Validated/complete |
| Warning | `#C47B18` | `#F3B95F` | Warnings |
| Danger | `#C94C62` | `#F27D93` | Errors |
| Border | `#E2E6F0` | `#2A3854` | Dividers and field borders |

### Typography

- Display: `Poppins`, 600–700.
- UI/body: `Inter`, 400–600. If Inter is not available, use `Segoe UI`.
- Hindi/Sanskrit fallback: `Noto Sans Devanagari`, `Nirmala UI`.
- Body: 14–16 px with 1.5 line height.
- Page title: 28–34 px.
- Section title: 18–22 px.
- Caption/meta: 12–13 px.

### Spacing and shape

- Base spacing unit: 4 px.
- Page gutter: 24 px desktop, 16 px mobile.
- Card padding: 20–24 px.
- Small radius: 10 px.
- Card radius: 16 px.
- Primary button height: 44 px.
- Inputs: 44–48 px.
- Use restrained shadows; prefer borders and tonal surfaces.

## Components

### Navigation

- Desktop: 240 px left rail after login, compact top nav for public pages.
- Mobile: bottom navigation with `Home`, `New Kundli`, `Records`, `More`.
- Active navigation has an indigo pill and a small gold indicator.
- The theme toggle must have a visible label/tooltip and preserve preference.

### Buttons

- Primary: indigo fill, gold focus ring only when relevant.
- Secondary: transparent with indigo border.
- Tertiary: text button.
- Destructive: red only for destructive actions.
- Avoid using gold for all buttons; it reduces hierarchy.

### Cards

- Use one primary card hierarchy per screen.
- Do not combine glass blur, gradient, gold border and large shadow on every card.
- Focus card: dark indigo background, subtle star texture, white text.

### Forms

- Label, field, hint/error in a consistent vertical stack.
- Input icons only when they improve recognition.
- Search results should appear in a keyboard-accessible popover.
- Disabled/read-only coordinates must look intentional, not broken.

### Data tables and tabs

- Use tabs for high-volume VB6 parity data.
- On mobile, convert tables to label/value rows.
- Keep column headers sticky when a table scrolls.
- Provide a `Copy values` action for raw calculation data where useful.

## UX states

Every calculation screen must support these states:

| State | UI treatment |
|---|---|
| Empty | Helpful first step and primary CTA |
| Editing | Active stepper and inline hints |
| Validating | Field-level feedback and summary message |
| Calculating | Progress indicator with “Calculating chart…” |
| Success | Success badge, timestamp and result actions |
| Warning | Amber callout with non-blocking explanation |
| Error | Clear recovery message; preserve entered values |
| No results | Explain what to change or create next |

## Accessibility checklist

- Contrast target: WCAG AA for normal text.
- Visible keyboard focus for every interactive element.
- Do not use color alone to communicate calculation status.
- Use `aria-live` for calculation progress and validation summary.
- Buttons must have action-oriented labels.
- Form controls need programmatic labels.
- Chart SVG must include a text alternative or data table.
- Respect `prefers-reduced-motion`.
- Keep touch targets at least 44 × 44 px.

## MVC implementation guidance

The design can be implemented without changing calculation contracts:

- Keep existing routes and controller actions.
- Refactor only the view structure, CSS classes and progressive enhancement scripts.
- Add a `CalculationStatus`/`CalculationSummary` view model if the current result does not expose status.
- Preserve existing `BirthDataViewModel` fields and add step metadata rather than renaming fields.
- Keep the official PDF/print view separate from the interactive report view.
- Centralize theme tokens in `Content/css/design-tokens.css` or the existing site stylesheet.
- Add dark mode using a root attribute such as `data-theme="dark"`; do not duplicate every component stylesheet.
- Avoid placing calculation logic in Razor views.

## Suggested rollout

1. Add design tokens and global typography.
2. Redesign `_AppLayout` and navigation.
3. Redesign Birth Data as the highest-value workflow.
4. Redesign Kundli result cards/tabs without removing parity fields.
5. Update report action bar and print separation.
6. Add responsive/mobile behavior.
7. Run visual regression and calculation parity tests.

## Figma delivery

The companion SVG files in this folder are **Figma-importable visual boards**. In Figma:

1. Create a new design file.
2. Drag the SVG into the canvas, or use `File → Import`.
3. Ungroup the imported SVG to edit text, fills and shapes.
4. Create components from the navigation, buttons, stat cards, tabs and form fields.
5. Add the tokens above as Figma variables and color styles.
6. Use the light and dark boards as the first desktop frames.

These boards are design references, not production HTML. The MVC implementation should use the existing project routes and models.
