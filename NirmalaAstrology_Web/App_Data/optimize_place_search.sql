USE `indu_astro`;

ALTER TABLE `tbl_place`
  ADD INDEX `ix_tbl_place_place_name` (`place_name`),
  ADD INDEX `ix_tbl_place_country_name` (`country_name`),
  ADD INDEX `ix_tbl_place_place_country` (`place_name`, `country_name`);
