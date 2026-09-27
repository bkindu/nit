# Tables and stored procedures used by NirmalaAstrology_Web (from C# + App_Data SPs)
# Staging stg_* tables are NOT used by the app and are excluded.

$USED_TABLES = @(
  'Zodiac',
  'Customer',
  'Place',
  'tbl_country',
  'tbl_place',
  'tbl_native',
  'tbl_match_det',
  'tbl_app_user',
  'tbl_app_role',
  'tbl_app_user_role',
  'tbl_report_definition',
  'tbl_service',
  'tbl_appointment',
  'tbl_payment',
  'tbl_public_report_order',
  'tbl_article',
  'tbl_language',
  'tbl_house',
  'tbl_planet',
  'tbl_sign',
  'tbl_nakshatra',
  'tbl_nakshatra_reading',
  'tbl_nakshatra_charan_reading',
  'tbl_sign_reading',
  'tbl_house_reading',
  'tbl_planet_house_reading',
  'tbl_planet_sign_reading',
  'tbl_udaya_astha_rule'
)

$USED_SPS = @(
  'sp_GetAllCountries',
  'sp_SearchPlaces',
  'sp_GetPlaceById',
  'sp_InsertPlace',
  'sp_GetAllZodiacs',
  'sp_GetZodiacByName',
  'sp_GetZodiacById'
)
