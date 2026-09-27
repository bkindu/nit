# Nirmala Astrology — Logo-Led All Screens Figma Specification

## Brand direction from the supplied logo

The supplied logo is a circular zodiac emblem with a warm red/vermilion body, a strong golden yellow ring, white lettering and a red ribbon marked `Estd. 1974`. The interface should feel like a modern digital extension of that identity rather than a generic purple astrology template.

### Brand palette

| Token | Hex | Role |
|---|---|---|
| Brand Vermilion | `#C62828` | Primary brand surface and active emphasis |
| Brand Red | `#E31B23` | CTA hover, ribbon energy and alerts only |
| Brand Gold | `#F6C400` | Primary CTA, logo ring, active indicator |
| Deep Maroon | `#5B151B` | Dark navigation, hero and focus surfaces |
| Saffron Cream | `#FFF8E7` | Warm public canvas |
| Paper | `#FFFFFF` | Cards and report surfaces |
| Ink | `#24191A` | Primary text |
| Muted Ink | `#765F60` | Secondary text |
| Dark Canvas | `#191113` | Dark mode canvas |
| Dark Surface | `#2A171B` | Dark cards |
| Border | `#E8D9CF` | Light borders |
| Success | `#2E8B67` | Verified calculation state |

### Visual rules

- Use the actual logo in the public header, login, report cover and admin workspace.
- Use maroon as the structural color; use vermilion for emphasis; reserve gold for actions and active states.
- Use cream/paper surfaces for public screens and dark maroon surfaces for calculation/admin focus areas.
- Avoid applying gradients, glow and glass effects to every card.
- Keep the zodiac emblem as a subtle motif, not as a repeated decorative watermark behind readable text.

## Screen inventory and Figma frame names

### Public views

| Figma frame | Existing view/route | Core design |
|---|---|---|
| Public / Home | `Home/Index` | Hero, quick Kundli, services, trust/heritage, articles |
| Public / About | `Home/About` | Brand story, scholars, heritage and CTA |
| Public / Contact | `Home/Contact`, `Contact/Index` | Contact form, address, phone, map/contact cards |
| Public / Zodiac index | `Zodiac/Index` | Searchable zodiac grid and category filters |
| Public / Zodiac detail | `Zodiac/Detail`, `Zodiac/Details` | Sign hero, traits, horoscope panels |
| Public / Articles index | `Articles/Index` | Editorial cards, category tabs, search |
| Public / Article detail | `Articles/Detail` | Reading layout, metadata, related articles |
| Public / Reports & pricing | `PublicReport/Index`, `BasicHoroscope` | Plan cards, comparison, purchase/contact CTA |
| Public / Appointment | `Appointment/Index` | Consultant/service/date selection |
| Public / Appointment detail | `Appointment/Details` | Booking summary and status |
| Public / Payment | `Payment/Index`, `Appointment/Pay` | Payment summary, UPI/QR and confirmation |
| Public / Login | `Auth/Login` | Branded split layout, secure sign-in |
| Public / Register | `Account/Register` | Simple account setup with trust copy |
| Public / Forgot password | `Account/ForgotPassword` | Email recovery flow |
| Public / Access denied | `Auth/AccessDenied` | Calm explanation and route back |
| Public / Error | `Error/Index`, `Error/NotFound`, `Shared/Error` | Recovery-first error state |

### User/operator views

| Figma frame | Existing view/route | Core design |
|---|---|---|
| User / Workspace dashboard | `Home/Index` or dashboard route | Recent natives, quick actions, daily Panchang |
| User / Birth data — step 1 | `BirthData/Index` | Person details step |
| User / Birth data — step 2 | `BirthData/Index` | Time/place step with confirmed location |
| User / Birth data — step 3 | `BirthData/Index` | Report choice and save option |
| User / Kundli result | `Home/KundliResult`, `Kundli` | Chart, summary cards, tabs |
| User / Official report preview | `Report/KundliPreview` | Report actions plus paper/PDF preview |
| User / Official print | `Report/KundliPrint` | Print-safe letterhead document |
| User / Records | `Records/Index` | Searchable natives table/cards |
| User / Account | `Account/Index` | Profile, preferences and saved data |
| User / Change password | `Account/ChangePassword` | Secure credential update |
| User / Match making | `MatchMaking/Index` | Two-person input and compatibility result |
| User / Numerology | `Numerology/Index` | Name/date input and number cards |
| User / Gochar | `Gochar/Index` | Current transit dashboard |
| User / Muhurat | `Muhurat/Index` | Date/event filters and auspicious windows |
| User / Vivah Muhurta | `VivahMuhurta/Index` | Wedding muhurta search and result cards |
| User / Varsh Phal | `VarshPhal/Index` | Year selector and annual result sections |
| User / Lal Kitab | `LalKitab/Index` | Remedy/input workflow and result cards |
| User / Palmistry | `Palmistry/Index` | Upload/instruction state and reading result |

### Admin views

| Figma frame | Existing route/area | Core design |
|---|---|---|
| Admin / Dashboard | `Admin/Dashboard/Index` | KPI cards, recent records, appointments, activity |
| Admin / Birth data | `BirthData/Index` | Operator-oriented guided form |
| Admin / Kundli queue | `Kundli`/report routes | Recent calculation queue and status |
| Admin / Records | `Records/Index` | Search, filters, actions, export |
| Admin / Appointments | `Admin/Appointments/Index` | Calendar/list toggle, statuses, payments |
| Admin / Appointment detail | `Appointment/Details` | Client timeline, notes, status actions |
| Admin / Articles | `Admin/Articles/Index` | Editorial table, draft/published status |
| Admin / Article editor | `Admin/Articles/Edit` if available | Rich content editor and preview |
| Admin / Places | shared `AddPlace` modal | Search/add coordinates/timezone form |
| Admin / Users/account | `Account/Index` | Profile and permission-aware controls |
| Admin / Access denied | `Auth/AccessDenied` | Role-aware recovery state |

## Component set to create in Figma

Create reusable components and variants for:

- Brand header with logo and `Estd. 1974` lockup.
- Public navigation, operator navigation and admin sidebar.
- Light/dark mode toggle.
- Primary gold button, maroon button, outline button, text action.
- Form field: default, focus, valid, warning, error, read-only.
- Place-search combobox with location-confirmed state.
- Three-step form stepper.
- Native summary card.
- Panchang metric card.
- Kundli chart frame.
- Zodiac icon tile.
- Report/pricing plan card.
- Appointment calendar/list row.
- Data table row and mobile record card.
- Toast, alert, empty state, loading state and error state.
- Modal for adding a place.
- Tabs for `Panchang`, `Planets`, `Houses`, `Hora`, `Ghat`, `Miscellaneous`.

## Responsive frames

For every major frame, create:

- Desktop: 1440 × 1024
- Tablet: 1024 × 1024
- Mobile: 390 × 844

Desktop uses a 240 px sidebar for operator/admin views. Public pages use a top navigation. Mobile uses a compact header and bottom navigation for logged-in views.

## Figma workflow

1. Import `NirmalaAstrology_Figma_All_Screens_Brand.svg`.
2. Place `Images/logo.png` into the logo component and keep the supplied aspect ratio.
3. Convert the palette into Figma color variables.
4. Create the component variants listed above.
5. Duplicate the frame templates for each route in the inventory.
6. Add prototype links: Home → Birth Data → Kundli Result → Official Report; Home → Appointment → Payment; Login → Workspace; Admin Dashboard → Records/Appointments/Articles.

The supplied SVG is an editable visual storyboard and screen map. Use it as the base for the final componentized Figma file.
