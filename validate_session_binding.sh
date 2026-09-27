#!/usr/bin/env bash
set -e
cd /home/ubuntu/nirmala_astrology_project/NirmalaAstrology_Web
grep -q 'tbl_place' Repositories/PlaceRepository.cs
grep -q 'GetAllPlaces' Controllers/HomeController.cs
grep -q 'placeSelect' Views/Home/Index.cshtml
 grep -q 'SetBirthDataSession' Scripts/Custom/site.js
grep -q 'BirthDataSessionStore' Controllers/HomeController.cs
grep -q 'BirthDataSessionModel.cs' NirmalaAstrology_Web.csproj
grep -q 'BirthDataSessionStore.cs' NirmalaAstrology_Web.csproj
grep -q 'Html.Raw' Views/Kundli/Result.cshtml
node --check Scripts/Custom/site.js
node --check Scripts/Custom/kundli-chart-svg.js
if command -v msbuild >/dev/null 2>&1; then
  msbuild NirmalaAstrology_Web.csproj /t:Build /p:Configuration=Release /v:minimal
else
  echo 'MSBuild unavailable in sandbox; build with Visual Studio on Windows.'
fi
