# Import App_Data\migration\load_core.sql into MySQL schema indu_astro.
# Usage (PowerShell):
#   $env:MYSQL_PWD = 'your_mysql_password'
#   powershell -ExecutionPolicy Bypass -File .\import_load_core.ps1
#
# Or pass -Password. Password is never stored by this script.

param(
    [string]$Password = $env:MYSQL_PWD,
    [string]$Server = "127.0.0.1",
    [int]$Port = 3306,
    [string]$User = "root",
    [string]$Database = "indu_astro",
    [string]$SqlFile = (Join-Path $PSScriptRoot "migration\load_core.sql")
)

$ErrorActionPreference = "Stop"
$mysql = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe"
if (-not (Test-Path $mysql)) { throw "mysql.exe not found at $mysql" }
if (-not (Test-Path $SqlFile)) { throw "SQL dump not found: $SqlFile. Run export_access_core.ps1 first." }
if ([string]::IsNullOrWhiteSpace($Password)) {
    throw "Set MYSQL_PWD or pass -Password. Example: `$env:MYSQL_PWD='...'; .\import_load_core.ps1"
}

$env:MYSQL_PWD = $Password
try {
    Write-Host "Creating/loading into $Database ..."
    & $mysql -h $Server -P $Port -u $User --default-character-set=utf8mb4 -e "CREATE DATABASE IF NOT EXISTS ``$Database`` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
    cmd /c "`"$mysql`" -h $Server -P $Port -u $User --default-character-set=utf8mb4 $Database < `"$SqlFile`""
    if ($LASTEXITCODE -ne 0) { throw "mysql import failed with exit $LASTEXITCODE" }
    & $mysql -h $Server -P $Port -u $User --default-character-set=utf8mb4 $Database -e "SELECT (SELECT COUNT(*) FROM tbl_match_det) AS match_det, (SELECT COUNT(*) FROM tbl_native) AS natives;"
    Write-Host "Import complete. Expected: match_det=36, natives=3718."
}
finally {
    Remove-Item Env:MYSQL_PWD -ErrorAction SilentlyContinue
}
