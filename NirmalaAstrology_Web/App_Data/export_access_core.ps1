# Export-only: Access Indu.mdb -> SQL dump (no MySQL write).
$ErrorActionPreference = "Stop"
$outDir = "E:\NirmalaAstrology\NirmalaAstrology_Web\App_Data\migration"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

function SqlStr($v) {
  if ($null -eq $v -or $v -eq "") { return "NULL" }
  $s = [string]$v
  $s = $s.Replace("\", "\\").Replace("'", "''").Replace("`0", "")
  return "'" + $s + "'"
}
function SqlInt($v) {
  if ($null -eq $v -or $v -eq "") { return "NULL" }
  $n = 0
  if ([int]::TryParse([string]$v, [ref]$n)) { return [string]$n }
  return "NULL"
}
function ParseDateSql($s) {
  if ([string]::IsNullOrWhiteSpace($s)) { return "NULL" }
  $dt = [datetime]::MinValue
  $formats = @('dd.MM.yyyy','d.M.yyyy','dd/MM/yyyy','d/M/yyyy','dd-MM-yyyy','d-M-yyyy','yyyy-MM-dd','MM/dd/yyyy')
  if ([datetime]::TryParseExact($s.Trim(), $formats, [Globalization.CultureInfo]::InvariantCulture, 'None', [ref]$dt)) {
    return "'" + $dt.ToString("yyyy-MM-dd") + "'"
  }
  return "NULL"
}
function ParseTimeSql($s) {
  if ([string]::IsNullOrWhiteSpace($s)) { return "NULL" }
  $t = [timespan]::Zero
  if ([timespan]::TryParse($s.Trim(), [ref]$t)) { return "'" + $t.ToString("hh\:mm\:ss") + "'" }
  $n = 0.0
  if ([double]::TryParse($s.Trim(), [Globalization.NumberStyles]::Any, [Globalization.CultureInfo]::InvariantCulture, [ref]$n)) {
    $h = [int][math]::Floor($n)
    $m = [int][math]::Round(($n - $h) * 100)
    if ($m -ge 60) { $m = [int](($n - $h) * 60) }
    return ("'{0:00}:{1:00}:00'" -f $h, $m)
  }
  return "NULL"
}

$ado = New-Object -ComObject ADODB.Connection
$ado.Open("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\NirmlaV2 - Copy\Indu.mdb;Persist Security Info=False")

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("SET NAMES utf8mb4;")
[void]$sb.AppendLine("SET FOREIGN_KEY_CHECKS=0;")

# Inline schema (avoid nested USE from 04/05 files)
[void]$sb.AppendLine(@"
CREATE TABLE IF NOT EXISTS tbl_native (
  native_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  serial_no INT NULL,
  native_name VARCHAR(150) NOT NULL,
  gender CHAR(1) NOT NULL DEFAULT 'M',
  birth_date DATE NULL,
  birth_date_text VARCHAR(20) NULL,
  birth_time TIME NULL,
  birth_time_text VARCHAR(20) NULL,
  birth_place VARCHAR(120) NULL,
  country_name VARCHAR(100) NULL,
  place_id INT NULL,
  latitude DECIMAL(9,6) NULL,
  longitude DECIMAL(9,6) NULL,
  time_zone DECIMAL(6,2) NULL,
  nick_name VARCHAR(80) NULL,
  father_name VARCHAR(150) NULL,
  address_text VARCHAR(500) NULL,
  remarks VARCHAR(500) NULL,
  temp_record TINYINT(1) NOT NULL DEFAULT 0,
  life_spheres VARCHAR(255) NULL,
  house_asc SMALLINT NULL, house_sun SMALLINT NULL, house_mon SMALLINT NULL, house_mar SMALLINT NULL,
  house_mer SMALLINT NULL, house_jup SMALLINT NULL, house_ven SMALLINT NULL, house_sat SMALLINT NULL,
  house_rah SMALLINT NULL, house_ket SMALLINT NULL,
  ua4 SMALLINT NULL, ua5 SMALLINT NULL, ua6 SMALLINT NULL, ua7 SMALLINT NULL, ua8 SMALLINT NULL,
  vm4 SMALLINT NULL, vm5 SMALLINT NULL, vm6 SMALLINT NULL, vm7 SMALLINT NULL, vm8 SMALLINT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (native_id),
  KEY idx_native_name (native_name),
  KEY idx_native_birth_date (birth_date),
  KEY idx_native_place (birth_place),
  KEY idx_native_serial (serial_no)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
"@)

$fields = (1..36 | ForEach-Object { "field_{0:00} VARCHAR(64) NOT NULL DEFAULT ''" -f $_ }) -join ",`n  "
[void]$sb.AppendLine("CREATE TABLE IF NOT EXISTS tbl_match_det (`n  id SMALLINT UNSIGNED NOT NULL,`n  $fields,`n  PRIMARY KEY (id)`n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;")
[void]$sb.AppendLine("DELETE FROM tbl_match_det;")
[void]$sb.AppendLine("DELETE FROM tbl_native;")

Write-Host "Exporting MatchDet..."
$rs = $ado.Execute("SELECT * FROM MatchDet")
$mc = 0
while (-not $rs.EOF) {
  $id = [int]$rs.Fields.Item("ID").Value
  $vals = New-Object System.Collections.Generic.List[string]
  $vals.Add([string]$id)
  for ($i = 1; $i -le 36; $i++) { $vals.Add((SqlStr $rs.Fields.Item("Field$i").Value)) }
  [void]$sb.AppendLine("INSERT INTO tbl_match_det VALUES (" + ($vals -join ",") + ");")
  $mc++; $rs.MoveNext()
}
$rs.Close()
Write-Host "MatchDet $mc"

Write-Host "Exporting natives..."
$rs = $ado.Execute("SELECT * FROM Record_F_Natives")
$n = 0
while (-not $rs.EOF) {
  $name = [string]$rs.Fields.Item("Native_Name").Value
  if ([string]::IsNullOrWhiteSpace($name)) { $name = "(unnamed)" }
  $g = [string]$rs.Fields.Item("Gender").Value
  if ([string]::IsNullOrWhiteSpace($g)) { $g = "M" } else { $g = $g.Substring(0,1).ToUpperInvariant() }
  $dtTxt = [string]$rs.Fields.Item("sBirth_Dt").Value
  $tmTxt = [string]$rs.Fields.Item("sBirth_Time").Value
  $temp = $rs.Fields.Item("Temp_Rec").Value
  $tempBit = 0
  if ($temp -eq $true -or "$temp" -eq "1" -or "$temp" -eq "True") { $tempBit = 1 }
  $cols = @(
    (SqlInt $rs.Fields.Item("SN").Value),
    (SqlStr $name.Trim()),
    (SqlStr $g),
    (ParseDateSql $dtTxt),
    (SqlStr $dtTxt),
    (ParseTimeSql $tmTxt),
    (SqlStr $tmTxt),
    (SqlStr $rs.Fields.Item("Birth_Place").Value),
    (SqlStr $rs.Fields.Item("Country").Value),
    (SqlStr $rs.Fields.Item("Addresses").Value),
    (SqlStr $rs.Fields.Item("Remarks").Value),
    [string]$tempBit,
    (SqlStr $rs.Fields.Item("LifeSpheres").Value)
  )
  [void]$sb.AppendLine("INSERT INTO tbl_native (serial_no, native_name, gender, birth_date, birth_date_text, birth_time, birth_time_text, birth_place, country_name, address_text, remarks, temp_record, life_spheres) VALUES (" + ($cols -join ",") + ");")
  $n++; $rs.MoveNext()
  if (($n % 500) -eq 0) { Write-Host "natives $n" }
}
$rs.Close(); $ado.Close()
[void]$sb.AppendLine("SET FOREIGN_KEY_CHECKS=1;")

$sqlPath = Join-Path $outDir "load_core.sql"
[System.IO.File]::WriteAllText($sqlPath, $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $sqlPath size=$((Get-Item $sqlPath).Length) natives=$n match=$mc"
