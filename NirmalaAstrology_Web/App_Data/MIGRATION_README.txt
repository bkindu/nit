# Data migration — Access Indu.mdb → MySQL indu_astro

## Already generated

`App_Data/migration/load_core.sql` (≈1.4 MB)

Contents from `D:\NirmlaV2 - Copy\Indu.mdb`:

| Table | Rows |
|---|---:|
| MatchDet → tbl_match_det | 36 |
| Record_F_Natives → tbl_native | 3718 |

## Import (run once)

MySQL root password in Web.config did not authenticate from this environment. Import locally:

```powershell
cd E:\NirmalaAstrology\NirmalaAstrology_Web\App_Data
$env:MYSQL_PWD = 'YOUR_MYSQL_ROOT_PASSWORD'
powershell -ExecutionPolicy Bypass -File .\import_load_core.ps1
```

Then confirm counts: `match_det=36`, `natives=3718`.

Update `Web.config` `NirmalaAstrologyDB` if the password differs from what is stored there.

## Re-export (if Access data changes)

```powershell
powershell -ExecutionPolicy Bypass -File .\export_access_core.ps1
```

## Match Making

After import, open `/MatchMaking`, enter male + female birth data (AJAX place search), Calculate Gunas.
If `tbl_match_det` is empty, the page shows an error asking you to migrate.
