# Kundli Calculation Parity Report

The supplied `pasted_content_2.txt` is now treated as the authoritative working implementation. The earlier MVC service was not equivalent: it used different calculation flow and approximations in places such as lunar handling, timezone treatment, and result construction.

The MVC project now includes `Scripts/Custom/kundli-reference-engine.js`, generated from the working JavaScript. The old browser form reads were replaced with explicit parameters for day, month, year, hour, minute, longitude, latitude, and timezone offset. The original formulas, helper functions, planet order, ayanamsa calculation, and rashi values remain unchanged.

`Services/WorkingJavascriptKundliCalculator.cs` executes that exact reference engine on the server using Jint, reads the computed longitude and rashi arrays, maps planets to `PlanetPosition`, and builds the twelve houses using the same ascendant-relative house rule. The controller now uses this parity calculator rather than the previous approximate calculator.

The existing `kundli-chart-svg.js` remains the client-side drawing layer. It receives the MVC `houses` and `planetPositions` JSON in the same shape used by the working chart. The server-side `KundliDrawer` remains available for HTML/table rendering but does not perform calculations.

A Node probe successfully executed the generated reference engine for a sample birth input and produced the expected full longitude/rashi arrays. A full Visual Studio build could not be run in this environment because Windows MSBuild/.NET Framework tooling is unavailable. After opening the solution in Visual Studio, restore NuGet packages, then run Clean and Rebuild.
