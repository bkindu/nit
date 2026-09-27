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

$ado = New-Object -ComObject ADODB.Connection
$ado.Open("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\NirmlaV2 - Copy\Indu.mdb;Persist Security Info=False")

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("USE indu_astro;")
[void]$sb.AppendLine("SET NAMES utf8mb4;")
[void]$sb.AppendLine((Get-Content -Raw "E:\NirmalaAstrology\NirmalaAstrology_Web\App_Data\04_tbl_native.sql" | Out-String))
[void]$sb.AppendLine((Get-Content -Raw "E:\NirmalaAstrology\NirmalaAstrology_Web\App_Data\05_tbl_match_det.sql" | Out-String))
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
$sqlPath = Join-Path $outDir "load_core.sql"
[System.IO.File]::WriteAllText($sqlPath, $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $sqlPath natives=$n match=$mc"

$mysql = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe"
$env:MYSQL_PWD = "anksri@123"
& $mysql -h 127.0.0.1 -P 3306 -u root --default-character-set=utf8mb4 indu_astro --execute="source $($sqlPath.Replace('\','/'))"
& $mysql -h 127.0.0.1 -P 3306 -u root --default-character-set=utf8mb4 indu_astro --execute="SELECT (SELECT COUNT(*) FROM tbl_match_det) AS match_det, (SELECT COUNT(*) FROM tbl_native) AS natives"
Remove-Item Env:MYSQL_PWD
