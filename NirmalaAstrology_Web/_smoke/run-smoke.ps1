$ErrorActionPreference = "Stop"

function Get-JulDay([int]$day, [int]$month, [int]$year, [double]$hour) {
  if ($month -lt 3) { $year -= 1; $month += 12 }
  $leep = [int][math]::Floor($year / 100.0)
  $leep = 2 - $leep + [int][math]::Floor($leep / 4.0)
  return [math]::Floor(365.2501 * $year) + [math]::Floor(($month + 1) * 30.6001) - 694025.0 + $leep + $day + $hour / 24.0
}

$epoch = [datetime]"1899-12-31"
$dt = [datetime]"1981-02-08"
$days = ($dt.Date - $epoch).TotalDays
$tz = 5.5
$tob = 7 + 47 / 60.0
$ut = $tob - $tz
$dt2 = $days + $ut / 24.0
$jd = Get-JulDay 8 2 1981 $ut
Write-Host "DT2JDF=$dt2"
Write-Host "julday=$jd"
Write-Host ("diff={0}" -f ($jd - $dt2))
Write-Host ("julday+2415019.5={0}" -f ($jd + 2415019.5))

$web = "E:\NirmalaAstrology\NirmalaAstrology_Web"
$out = Join-Path $web "_smoke\bin"
New-Item -ItemType Directory -Force -Path $out | Out-Null

# Build web project first
$msbuild = @(
  "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
  "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
  "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $msbuild) {
  Write-Host "MSBuild not found; trying csc against bin DLL"
} else {
  & $msbuild (Join-Path $web "NirmalaAstrology_Web.csproj") /t:Build /p:Configuration=Debug /v:m
  if ($LASTEXITCODE -ne 0) { throw "Web build failed" }
}

$dll = Join-Path $web "bin\NirmalaAstrology_Web.dll"
if (-not (Test-Path $dll)) { $dll = Join-Path $web "bin\Debug\NirmalaAstrology_Web.dll" }
$refs = @(
  $dll,
  (Join-Path $web "bin\MySql.Data.dll"),
  (Join-Path $web "bin\Newtonsoft.Json.dll")
) | Where-Object { Test-Path $_ }

$csc = "${env:WINDIR}\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$refArgs = ($refs | ForEach-Object { "/r:`"$_`"" }) -join " "
$exe = Join-Path $out "KundliSmoke.exe"
$src = Join-Path $web "_smoke\Program.cs"
cmd /c "`"$csc`" /nologo /t:exe /out:`"$exe`" $refArgs /r:System.dll /r:System.Core.dll /r:System.Web.dll `"$src`""
if ($LASTEXITCODE -ne 0) { throw "csc failed" }

Copy-Item (Join-Path $web "bin\*") $out -Force -ErrorAction SilentlyContinue
# App_Data for Uday CSV
$ad = Join-Path $out "App_Data"
New-Item -ItemType Directory -Force -Path $ad | Out-Null
Copy-Item (Join-Path $web "App_Data\Uday_Astha.csv") $ad -Force

Push-Location $out
& $exe
Pop-Location
