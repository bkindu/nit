# Kundli MVC Refactor

The project now targets **.NET Framework 4.8** and separates kundli responsibilities into independent classes.

| Class | Responsibility |
|---|---|
| `Services/KundliCalculator.cs` | Performs astronomical and house calculations only. It has no Razor, DOM, or HTML-generation responsibility. |
| `Models/PlanetPosition.cs` | Stores one planet’s position and derived display properties. |
| `Models/HouseData.cs` | Stores a house, its rashi, and occupying planets. |
| `Models/KundliResult.cs` | Stores the complete calculation result. |
| `Services/KundliDrawer.cs` | Builds chart HTML and house-label data from an existing result. It does not calculate astronomy. |
| `Services/KundliFormatter.cs` | Formats degrees, dates, times, planet text, and safely HTML-encodes output. |

The existing `KundliController` can continue calling `KundliCalculator.CalculateKundli(...)` without changing its current API. To use server-rendered chart HTML, inject or instantiate `KundliDrawer` after calculation and pass `drawer.RenderHtml(result)` to the view. The existing client-side SVG chart can remain in place because it is independent of the calculation class.

The archive did not contain a usable MSBuild executable in the Linux validation environment, so a full Windows/Visual Studio build could not be run here. Static checks confirmed that all new C# files have balanced braces, one intended class each, and are included in the legacy project file. Open the solution in Visual Studio 2022 with the .NET Framework 4.8 Developer Pack installed and run a normal Clean/Rebuild.
