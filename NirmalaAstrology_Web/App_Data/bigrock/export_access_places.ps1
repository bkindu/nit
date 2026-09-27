# Export Access Indu.mdb Place atlas -> MySQL SQL (no MySQL credentials needed).
$ErrorActionPreference = "Stop"
$mdb = "D:\NirmlaV2 - Copy\Indu.mdb"
$outDir = "E:\NirmalaAstrology\NirmalaAstrology_Web\App_Data\bigrock"
$outFile = Join-Path $outDir "03_places_from_access.sql"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

function SqlStr($v) {
  if ($null -eq $v -or $v -eq "") { return "NULL" }
  $s = [string]$v
  $s = $s.Replace("\", "\\").Replace("'", "''").Replace("`0", "")
  return "'" + $s + "'"
}
function SqlDec($v) {
  if ($null -eq $v -or $v -eq "") { return "NULL" }
  $n = 0.0
  if ([double]::TryParse([string]$v, [Globalization.NumberStyles]::Any, [Globalization.CultureInfo]::InvariantCulture, [ref]$n)) {
    return $n.ToString("0.######", [Globalization.CultureInfo]::InvariantCulture)
  }
  return "NULL"
}

if (-not (Test-Path $mdb)) { throw "Access DB not found: $mdb" }

$ado = New-Object -ComObject ADODB.Connection
$ado.Open("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$mdb;Persist Security Info=False")

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("-- Places exported from Indu.mdb Place table")
[void]$sb.AppendLine("-- Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$sb.AppendLine("SET NAMES utf8mb4;")
[void]$sb.AppendLine("SET FOREIGN_KEY_CHECKS=0;")
[void]$sb.AppendLine(@"
CREATE TABLE IF NOT EXISTS tbl_place (
  place_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  place_name VARCHAR(255) NOT NULL,
  country_name VARCHAR(255) NULL,
  latitude DECIMAL(9,6) NULL,
  longitude DECIMAL(9,6) NULL,
  time_zone VARCHAR(64) NULL,
  summer_time_correction VARCHAR(20) NULL,
  KEY ix_tbl_place_place_name (place_name),
  KEY ix_tbl_place_country_name (country_name),
  KEY ix_tbl_place_place_country (place_name, country_name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
"@)
[void]$sb.AppendLine("DELETE FROM tbl_place;")

$rs = New-Object -ComObject ADODB.Recordset
$rs.Open("SELECT * FROM [Place]", $ado)
$count = 0
$batch = New-Object System.Collections.Generic.List[string]
while (-not $rs.EOF) {
  $name = $null; $country = $null; $lat = $null; $lon = $null; $tz = $null; $smt = $null
  foreach ($f in @($rs.Fields)) {
    switch -Regex ($f.Name) {
      '^(Place|PlaceName)$' { $name = $f.Value }
      '^(Country|CountryName)$' { $country = $f.Value }
      '^(Latt|Lat|Latitude)$' { $lat = $f.Value }
      '^(Long|Longitude)$' { $lon = $f.Value }
      '^(TimeZone|Timezone)$' { $tz = $f.Value }
      '^(SMTCorr|SMT|Summer)' { $smt = $f.Value }
    }
  }
  if ([string]::IsNullOrWhiteSpace([string]$name)) { $rs.MoveNext(); continue }
  $vals = "({0},{1},{2},{3},{4},{5})" -f (SqlStr $name), (SqlStr $country), (SqlDec $lat), (SqlDec $lon), (SqlStr $tz), (SqlStr $smt)
  $batch.Add($vals) | Out-Null
  $count++
  if ($batch.Count -ge 100) {
    [void]$sb.AppendLine("INSERT INTO tbl_place (place_name, country_name, latitude, longitude, time_zone, summer_time_correction) VALUES")
    [void]$sb.AppendLine(($batch -join ",`r`n") + ";")
    $batch.Clear()
  }
  $rs.MoveNext()
}
if ($batch.Count -gt 0) {
  [void]$sb.AppendLine("INSERT INTO tbl_place (place_name, country_name, latitude, longitude, time_zone, summer_time_correction) VALUES")
  [void]$sb.AppendLine(($batch -join ",`r`n") + ";")
}

[void]$sb.AppendLine(@"
CREATE TABLE IF NOT EXISTS tbl_country (
  country_id SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
  country_name VARCHAR(255) NOT NULL,
  UNIQUE KEY uq_tbl_country_name (country_name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
INSERT IGNORE INTO tbl_country (country_name)
SELECT DISTINCT TRIM(country_name) FROM tbl_place
WHERE country_name IS NOT NULL AND TRIM(country_name) <> '';
SET FOREIGN_KEY_CHECKS=1;
-- Places loaded: $count
"@)

$rs.Close()
$ado.Close()
[System.IO.File]::WriteAllText($outFile, $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $outFile ($count places)"
