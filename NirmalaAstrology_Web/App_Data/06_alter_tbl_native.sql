-- Align existing indu_astro.tbl_native with VB6 Record_F_Natives fields.
USE `indu_astro`;

ALTER TABLE `tbl_native`
  ADD COLUMN IF NOT EXISTS `serial_no` INT NULL AFTER `native_id`,
  ADD COLUMN IF NOT EXISTS `gender` CHAR(1) NULL AFTER `gender_code`,
  ADD COLUMN IF NOT EXISTS `birth_date` DATE NULL AFTER `birth_at`,
  ADD COLUMN IF NOT EXISTS `birth_date_text` VARCHAR(20) NULL AFTER `birth_date_legacy`,
  ADD COLUMN IF NOT EXISTS `birth_time` TIME NULL AFTER `birth_time_legacy`,
  ADD COLUMN IF NOT EXISTS `birth_time_text` VARCHAR(20) NULL AFTER `birth_time`,
  ADD COLUMN IF NOT EXISTS `birth_place` VARCHAR(120) NULL AFTER `place_id`,
  ADD COLUMN IF NOT EXISTS `country_name` VARCHAR(100) NULL AFTER `birth_place`,
  ADD COLUMN IF NOT EXISTS `latitude` DECIMAL(9,6) NULL AFTER `country_name`,
  ADD COLUMN IF NOT EXISTS `longitude` DECIMAL(9,6) NULL AFTER `latitude`,
  ADD COLUMN IF NOT EXISTS `time_zone` DECIMAL(6,2) NULL AFTER `longitude`,
  ADD COLUMN IF NOT EXISTS `nick_name` VARCHAR(80) NULL AFTER `time_zone`,
  ADD COLUMN IF NOT EXISTS `father_name` VARCHAR(150) NULL AFTER `nick_name`,
  ADD COLUMN IF NOT EXISTS `temp_record` TINYINT(1) NOT NULL DEFAULT 0 AFTER `is_temporary`,
  ADD COLUMN IF NOT EXISTS `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  ADD COLUMN IF NOT EXISTS `updated_at` DATETIME NULL ON UPDATE CURRENT_TIMESTAMP;

-- MySQL 8.0 may not support ADD COLUMN IF NOT EXISTS on older patch levels.
-- Fallback statements are applied by the PowerShell loader when needed.

CREATE INDEX IF NOT EXISTS `idx_native_birth_place` ON `tbl_native` (`birth_place`);
CREATE INDEX IF NOT EXISTS `idx_native_serial_no` ON `tbl_native` (`serial_no`);
CREATE INDEX IF NOT EXISTS `idx_native_birth_date` ON `tbl_native` (`birth_date`);
