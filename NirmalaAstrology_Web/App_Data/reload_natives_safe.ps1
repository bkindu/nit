# Re-export natives with safe escaping and load into MySQL.
param([string]$Password = "Ankari@1981")
$ErrorActionPreference = "Stop"
$mysql = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe"
$out = "E:\NirmalaAstrology\NirmalaAstrology_Web\App_Data\migration\load_natives_safe.sql"

function SqlStr($v) {
  if ($null -eq $v -or $v -eq "") { return "NULL" }
  $s = [string]$v
  $s = $s -replace "`r`n", " " -replace "`n", " " -replace "`r", " " -replace "`t", " "
  $s = $s.Replace("\", "\\").Replace("'", "''")
  # strip chars MySQL string literals reject in this context
  $s = [regex]::Replace($s, "[\u0000-\u0008\u000B\u000C\u000E-\u001F]", "")
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
    if ($m -lt 0) { $m = 0 }
    if ($h -gt 23) { $h = $h % 24 }
    return ("'{0:00}:{1:00}:00'" -f $h, $m)
  }
  return "NULL"
}

$ado = New-Object -ComObject ADODB.Connection
$ado.Open("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\NirmlaV2 - Copy\Indu.mdb;Persist Security Info=False")
$rs = $ado.Execute("SELECT * FROM Record_F_Natives")
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("SET NAMES utf8mb4;")
[void]$sb.AppendLine("SET FOREIGN_KEY_CHECKS=0;")
[void]$sb.AppendLine("DELETE FROM tbl_native;")
$n = 0
while (-not $rs.EOF) {
  $name = [string]$rs.Fields.Item("Native_Name").Value
  if ([string]::IsNullOrWhiteSpace($name)) { $name = "(unnamed)" }
  $g = [string]$rs.Fields.Item("Gender").Value
  if ([string]::IsNullOrWhiteSpace($g)) { $g = "M" } else { $g = $g.Substring(0,1).ToUpperInvariant() }
  $dtTxt = [string]$rs.Fields.Item("sBirth_Dt").Value
  $tmTxt = [string]$rs.Fields.Item("sBirth_Time").Value
  $bd = ParseDateSql $dtTxt
  $bt = ParseTimeSql $tmTxt
  $temp = $rs.Fields.Item("Temp_Rec").Value
  $tempBit = 0
  if ($temp -eq $true -or "$temp" -eq "1" -or "$temp" -eq "True") { $tempBit = 1 }
  $birthAt = "NULL"
  if ($bd -ne "NULL" -and $bt -ne "NULL") {
    $birthAt = "'" + $bd.Trim("'") + " " + $bt.Trim("'") + "'"
  } elseif ($bd -ne "NULL") {
    $birthAt = "'" + $bd.Trim("'") + " 00:00:00'"
  }
  $cols = @(
    (SqlInt $rs.Fields.Item("SN").Value),
    (SqlStr $name.Trim()),
    (SqlStr $g),
    (SqlStr $g),
    $birthAt,
    $bd,
    (SqlStr $dtTxt),
    (SqlStr $dtTxt),
    (SqlStr $tmTxt),
    $bt,
    (SqlStr $tmTxt),
    "NULL",
    (SqlStr $rs.Fields.Item("Birth_Place").Value),
    (SqlStr $rs.Fields.Item("Country").Value),
    (SqlStr $rs.Fields.Item("Addresses").Value),
    (SqlStr $rs.Fields.Item("Remarks").Value),
    (SqlStr $rs.Fields.Item("LifeSpheres").Value),
    [string]$tempBit,
    [string]$tempBit
  )
  [void]$sb.AppendLine("INSERT INTO tbl_native (serial_no, native_name, gender, gender_code, birth_at, birth_date, birth_date_text, birth_date_legacy, birth_time_legacy, birth_time, birth_time_text, place_id, birth_place, country_name, address_text, remarks, life_spheres, temp_record, is_temporary) VALUES (" + ($cols -join ",") + ");")
  $n++; $rs.MoveNext()
  if (($n % 500) -eq 0) { Write-Host "exported $n" }
}
$rs.Close(); $ado.Close()
[void]$sb.AppendLine("SET FOREIGN_KEY_CHECKS=1;")
[System.IO.File]::WriteAllText($out, $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $out rows=$n size=$((Get-Item $out).Length)"

$env:MYSQL_PWD = $Password
cmd /c "`"$mysql`" -h 127.0.0.1 -P 3306 -u root --default-character-set=utf8mb4 indu_astro < `"$out`""
if ($LASTEXITCODE -ne 0) { throw "Import failed: $LASTEXITCODE" }
& $mysql -h 127.0.0.1 -P 3306 -u root --default-character-set=utf8mb4 indu_astro -e "SELECT (SELECT COUNT(*) FROM tbl_match_det) AS match_det, (SELECT COUNT(*) FROM tbl_native) AS natives;"
Remove-Item Env:MYSQL_PWD -ErrorAction SilentlyContinue
