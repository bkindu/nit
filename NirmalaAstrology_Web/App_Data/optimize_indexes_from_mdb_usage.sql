-- Safe indexes for legacy MDB query patterns (Indu.mdb → MySQL indu_astro / nit).
-- Does NOT change business data. Run manually after backup.
-- Duplicate-index errors are expected if an index already exists — skip those statements.

USE `indu_astro`;
SET NAMES utf8mb4;

-- Place search (legacy: WHERE Place = '…' / LIKE)
ALTER TABLE `tbl_place` ADD INDEX `ix_tbl_place_place_name` (`place_name`);
ALTER TABLE `tbl_place` ADD INDEX `ix_tbl_place_country_name` (`country_name`);

-- Gochar / Jantri (legacy RasPrves9plts1950_2100 WHERE Planet=… AND Entry_Dt…)
ALTER TABLE `RasPrves9plts1950_2100` ADD INDEX `ix_ras_planet` (`Planet`);
ALTER TABLE `RasPrves9plts1950_2100` ADD INDEX `ix_ras_planet_entry` (`Planet`, `Entry_Dt`);

-- Phalit / report lookups (legacy Text_Phalit.frm)
ALTER TABLE `Brg_Reading` ADD INDEX `ix_brg_name` (`Brg_Name`(64));
ALTER TABLE `Dasa_Reading` ADD INDEX `ix_dasa_lord` (`DasaLord_E`(64));
ALTER TABLE `Misc` ADD INDEX `ix_misc_desc` (`Description`(64));
ALTER TABLE `Palmistry` ADD INDEX `ix_palm_id` (`Palm_ID`(64));
ALTER TABLE `Numerology_Reading` ADD INDEX `ix_num_num` (`Num`(16));
ALTER TABLE `Upay` ADD INDEX `ix_upay` (`Upay`(64));
ALTER TABLE `Yogas` ADD INDEX `ix_yogas_id` (`ID`);
ALTER TABLE `Lal_Kitab_Digits` ADD INDEX `ix_lal_id` (`ID`);
ALTER TABLE `Lrd12xHs12_Reading` ADD INDEX `ix_lrd_hs` (`Lrd_in_Hs`(32));
ALTER TABLE `VivahMuhurtaDates` ADD INDEX `ix_vivah_dates_id` (`ID`);
ALTER TABLE `VivahMuhurtaPuja` ADD INDEX `ix_vivah_puja_dt` (`Muhurta_Dt`);

-- Normalized reading tables used by KundliReportRepository
ALTER TABLE `tbl_nakshatra_reading` ADD INDEX `ix_nr_nak_lang` (`nakshatra_id`, `language_id`);
ALTER TABLE `tbl_sign_reading` ADD INDEX `ix_sr_sign_lang` (`sign_id`, `language_id`);
ALTER TABLE `tbl_house_reading` ADD INDEX `ix_hr_house_lang` (`house_id`, `language_id`);
