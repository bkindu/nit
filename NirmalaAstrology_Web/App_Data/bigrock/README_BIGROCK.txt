Nirmala Astrology — BigRock import (PROJECT USED ONLY)
======================================================

Target database: nit

Preferred file
--------------
BIGROCK_PROJECT_ONLY.sql   (~1.1 MB)

Contains ONLY tables + stored procedures used by this web project.
Does NOT include unused stg_* staging tables.

Import
------
1. phpMyAdmin / Workbench → select database: nit
2. Import BIGROCK_PROJECT_ONLY.sql
3. Web.config: Database=nit; Server=localhost; Uid=...; Pwd=...;

Tables (28) — see USED_OBJECTS.txt
SPs (7): sp_GetAllCountries, sp_SearchPlaces, sp_GetPlaceById, sp_InsertPlace,
         sp_GetAllZodiacs, sp_GetZodiacByName, sp_GetZodiacById

Admin
-----
If no users: first site visit creates admin / Nirmala@1974 — change immediately.

Older larger packs (optional / superseded for clean installs)
------------------------------------------------------------
BIGROCK_FULL.sql            — earlier core pack
BIGROCK_MISSING_TABLES.sql  — all 74 local tables including unused stg_*
Do not need these if you use BIGROCK_PROJECT_ONLY.sql
