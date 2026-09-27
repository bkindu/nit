-- NIRMALA ASTROLOGY - Stored Procedures for normalized indu_astro schema
-- Run this script after 01_normalized_schema.sql and place data migration.
-- MySQL Workbench: open this file, select schema indu_astro, execute all.

USE `indu_astro`;

-- Compatibility country table required by the legacy CountryRepository.
CREATE TABLE IF NOT EXISTS `tbl_country` (
    `country_id` SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `country_name` VARCHAR(255) NOT NULL,
    PRIMARY KEY (`country_id`),
    UNIQUE KEY `uq_tbl_country_name` (`country_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO `tbl_country` (`country_name`)
SELECT DISTINCT TRIM(`country_name`)
FROM `tbl_place`
WHERE `country_name` IS NOT NULL AND TRIM(`country_name`) <> '';

DROP PROCEDURE IF EXISTS `sp_SearchPlaces`;
DROP PROCEDURE IF EXISTS `sp_GetPlaceById`;
DROP PROCEDURE IF EXISTS `sp_InsertPlace`;
DROP PROCEDURE IF EXISTS `sp_GetAllCountries`;
DROP PROCEDURE IF EXISTS `sp_GetAllZodiacs`;
DROP PROCEDURE IF EXISTS `sp_GetZodiacByName`;
DROP PROCEDURE IF EXISTS `sp_GetZodiacById`;

DELIMITER $$

CREATE PROCEDURE `sp_SearchPlaces`(IN p_keyword VARCHAR(100))
BEGIN
    DECLARE v_keyword VARCHAR(100);
    SET v_keyword = TRIM(COALESCE(p_keyword, ''));

    SELECT p.place_id AS PlaceID,
           p.place_name AS PlaceName,
           COALESCE(c.country_id, 0) AS CountryID,
           p.country_name AS CountryName,
           p.latitude AS Latitude,
           p.longitude AS Longitude,
           p.time_zone AS TimeZone,
           p.summer_time_correction AS SMTCorr
    FROM tbl_place p
    LEFT JOIN tbl_country c ON c.country_name = p.country_name
    WHERE p.place_name LIKE CONCAT(v_keyword, '%')
       OR p.country_name LIKE CONCAT(v_keyword, '%')
       OR p.place_name LIKE CONCAT('%', v_keyword, '%')
       OR p.country_name LIKE CONCAT('%', v_keyword, '%')
    ORDER BY CASE
                 WHEN p.place_name LIKE CONCAT(v_keyword, '%') THEN 0
                 WHEN p.country_name LIKE CONCAT(v_keyword, '%') THEN 1
                 ELSE 2
             END,
             p.place_name,
             p.country_name
    LIMIT 20;
END$$

CREATE PROCEDURE `sp_GetPlaceById`(IN p_PlaceID INT)
BEGIN
    SELECT p.place_id AS PlaceID,
           p.place_name AS PlaceName,
           COALESCE(c.country_id, 0) AS CountryID,
           p.country_name AS CountryName,
           p.latitude AS Latitude,
           p.longitude AS Longitude,
           p.time_zone AS TimeZone,
           p.summer_time_correction AS SMTCorr
    FROM tbl_place p
    LEFT JOIN tbl_country c ON c.country_name = p.country_name
    WHERE p.place_id = p_PlaceID
    LIMIT 1;
END$$

CREATE PROCEDURE `sp_InsertPlace`(
    IN p_PlaceName VARCHAR(255),
    IN p_CountryID SMALLINT,
    IN p_Latitude DECIMAL(9,6),
    IN p_Longitude DECIMAL(9,6),
    IN p_TimeZone VARCHAR(64),
    IN p_SMTCorr VARCHAR(20)
)
BEGIN
    DECLARE v_country_name VARCHAR(255);
    SET v_country_name = COALESCE(
        (SELECT country_name FROM tbl_country WHERE country_id = p_CountryID LIMIT 1),
        'India'
    );

    INSERT INTO tbl_place
        (place_name, country_name, latitude, longitude, time_zone, summer_time_correction)
    VALUES
        (TRIM(p_PlaceName), v_country_name, p_Latitude, p_Longitude, p_TimeZone, p_SMTCorr);

    SELECT LAST_INSERT_ID() AS NewId;
END$$

CREATE PROCEDURE `sp_GetAllCountries`()
BEGIN
    SELECT country_id AS CountryID,
           country_name AS CountryName
    FROM tbl_country
    ORDER BY country_name ASC;
END$$

-- These procedures support older repository versions that use the legacy Zodiac table.
-- They are harmless if the legacy Zodiac table is retained in indu_astro.
CREATE PROCEDURE `sp_GetAllZodiacs`()
BEGIN
    SELECT ZodiacId, Name, HindiName, Icon, Image, Description,
           LuckyNumber, LuckyColor, LuckyDay, Planet, Element, Nature, Strength, Weakness,
           Career, Love, Health, Finance, Personality, Characteristics, CompatibleSigns, CreatedDate
    FROM Zodiac
    ORDER BY ZodiacId ASC;
END$$

CREATE PROCEDURE `sp_GetZodiacByName`(IN p_Name VARCHAR(50))
BEGIN
    SELECT ZodiacId, Name, HindiName, Icon, Image, Description,
           LuckyNumber, LuckyColor, LuckyDay, Planet, Element, Nature, Strength, Weakness,
           Career, Love, Health, Finance, Personality, Characteristics, CompatibleSigns, CreatedDate
    FROM Zodiac
    WHERE LOWER(Name) = LOWER(p_Name)
    LIMIT 1;
END$$

CREATE PROCEDURE `sp_GetZodiacById`(IN p_ZodiacId INT)
BEGIN
    SELECT ZodiacId, Name, HindiName, Icon, Image, Description,
           LuckyNumber, LuckyColor, LuckyDay, Planet, Element, Nature, Strength, Weakness,
           Career, Love, Health, Finance, Personality, Characteristics, CompatibleSigns, CreatedDate
    FROM Zodiac
    WHERE ZodiacId = p_ZodiacId
    LIMIT 1;
END$$

DELIMITER ;

-- Verification queries. Run after installation.
SHOW PROCEDURE STATUS
WHERE Db = DATABASE()
  AND Name IN (
      'sp_SearchPlaces', 'sp_GetPlaceById', 'sp_InsertPlace',
      'sp_GetAllCountries', 'sp_GetAllZodiacs',
      'sp_GetZodiacByName', 'sp_GetZodiacById'
  );

SELECT COUNT(*) AS CountryCount FROM tbl_country;
SELECT COUNT(*) AS PlaceCount FROM tbl_place;
-- Example smoke test:
-- CALL sp_SearchPlaces('Pat');
-- CALL sp_GetAllCountries();
-- CALL sp_GetPlaceById(1);
-- CALL sp_GetAllZodiacs();

-- IMPORTANT: If your database does not contain the legacy `Zodiac` table,
-- do not call the three Zodiac procedures above. ZodiacRepository now selects
-- explicit columns from the legacy table (no SELECT *).
