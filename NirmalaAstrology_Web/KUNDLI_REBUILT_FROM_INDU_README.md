# Kundli Module Rebuilt from Indu.html

## Source of truth

`Scripts/Custom/kundli-reference-engine.js` is generated directly from the uploaded `Indu.html`. The astronomical formulas and calculation order are not replaced with a different astrology library or simplified algorithm. The only changes are input adaptation and output capture: the original form reads are supplied as explicit method parameters, and the calculated arrays are copied into a server-readable result.

## Runtime flow

`HomeController.CalculateKundli` and `KundliController.Index` both call `WorkingJavascriptKundliCalculator`. That class executes the reference engine with Jint, reads the calculated longitude and rashi arrays, and maps them into `KundliResult`, `PlanetPosition`, and `HouseData` models. The controller returns JSON or a Razor view model. `Result.cshtml` passes `houses` and `planetPositions` to `kundli-chart-svg.js`.

## Chart contract

The SVG renderer receives exactly twelve house objects. Each house contains its house number, rashi number, rashi name, and dynamically calculated planet labels. The first house is derived from the calculated ascendant rashi; planet house positions are calculated relative to that ascendant. No planet coordinates are hard-coded.

## Visual structure

The browser renderer uses an SVG outer square, intersecting diagonals, and the central diamond required for a North Indian chart. Planet labels are grouped by calculated house so multiple planets remain inside their correct region.

## Build

Open `NirmalaAstrology.sln` in Visual Studio with the .NET Framework 4.8 Developer Pack installed. Restore NuGet packages, then run Clean Solution and Rebuild Solution. Jint 3.1.4 and its Esprima 3.0.5 dependency are included in the solution package.
