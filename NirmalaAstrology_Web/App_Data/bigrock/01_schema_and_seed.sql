-- ============================================================
-- Nirmala Astrology — BigRock / cPanel MySQL
-- Part 1: Schema + essential seed data
-- ============================================================
-- HOW TO IMPORT (BigRock cPanel → phpMyAdmin):
-- 1) Use existing database: nit (select it in phpMyAdmin before import).
-- 2) Import BIGROCK_FULL.sql (or 01, then 02, then 03).
-- 3) Web.config: Database=nit; plus BigRock host/user/password.
-- 4) First app start seeds default admin if tbl_app_user is empty:
--    username: admin   password: Nirmala@1974  (change immediately)
-- ============================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;
SET SQL_SAFE_UPDATES = 0;
SET SQL_MODE = 'NO_AUTO_VALUE_ON_ZERO';
SET time_zone = '+00:00';

USE `nit`;

-- ---------- Zodiac (public site) ----------
CREATE TABLE IF NOT EXISTS `Zodiac` (
  `ZodiacId` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(50) NOT NULL,
  `HindiName` VARCHAR(50) DEFAULT '',
  `Icon` VARCHAR(200) DEFAULT '',
  `Image` VARCHAR(500) DEFAULT '',
  `Description` TEXT,
  `LuckyNumber` VARCHAR(100) DEFAULT '',
  `LuckyColor` VARCHAR(100) DEFAULT '',
  `LuckyDay` VARCHAR(50) DEFAULT '',
  `Planet` VARCHAR(50) DEFAULT '',
  `Element` VARCHAR(50) DEFAULT '',
  `Nature` VARCHAR(50) DEFAULT '',
  `Strength` TEXT,
  `Weakness` TEXT,
  `Career` TEXT,
  `Love` TEXT,
  `Health` TEXT,
  `Finance` TEXT,
  `Personality` TEXT,
  `Characteristics` TEXT,
  `CompatibleSigns` VARCHAR(200) DEFAULT '',
  `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`ZodiacId`),
  UNIQUE KEY `idx_zodiac_name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ---------- Legacy Customer (public kundli form) ----------
CREATE TABLE IF NOT EXISTS `Customer` (
  `CustomerId` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(150) NOT NULL,
  `DOB` DATE NOT NULL,
  `BirthTime` TIME NOT NULL,
  `PlaceId` INT(11) NOT NULL DEFAULT 0,
  `Latitude` DECIMAL(10,7) NOT NULL DEFAULT 0,
  `Longitude` DECIMAL(10,7) NOT NULL DEFAULT 0,
  `Timezone` VARCHAR(50) DEFAULT '',
  `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`CustomerId`),
  KEY `idx_customer_name` (`Name`),
  KEY `idx_customer_dob` (`DOB`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ---------- Place atlas (data in 03_places_from_access.sql) ----------
CREATE TABLE IF NOT EXISTS `tbl_place` (
  `place_id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `place_name` VARCHAR(255) NOT NULL,
  `country_name` VARCHAR(255) NULL,
  `latitude` DECIMAL(9,6) NULL,
  `longitude` DECIMAL(9,6) NULL,
  `time_zone` VARCHAR(64) NULL,
  `summer_time_correction` VARCHAR(20) NULL,
  KEY `ix_tbl_place_place_name` (`place_name`),
  KEY `ix_tbl_place_country_name` (`country_name`),
  KEY `ix_tbl_place_place_country` (`place_name`, `country_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_country` (
  `country_id` SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `country_name` VARCHAR(255) NOT NULL,
  UNIQUE KEY `uq_tbl_country_name` (`country_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ---------- Auth / roles / reports ----------
CREATE TABLE IF NOT EXISTS `tbl_app_user` (
  `user_id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `username` VARCHAR(64) NOT NULL,
  `password_hash` VARCHAR(256) NOT NULL,
  `password_salt` VARCHAR(128) NOT NULL,
  `display_name` VARCHAR(120) NULL,
  `email` VARCHAR(150) NULL,
  `mobile` VARCHAR(20) NULL,
  `is_active` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY `uq_app_user_username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `tbl_app_role` (
  `role_id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `role_code` VARCHAR(32) NOT NULL,
  `role_name` VARCHAR(64) NOT NULL,
  UNIQUE KEY `uq_app_role_code` (`role_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `tbl_app_user_role` (
  `user_id` INT NOT NULL,
  `role_id` INT NOT NULL,
  PRIMARY KEY (`user_id`, `role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `tbl_report_definition` (
  `report_id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `report_code` VARCHAR(64) NOT NULL,
  `report_name` VARCHAR(120) NOT NULL,
  `public_allowed` TINYINT(1) NOT NULL DEFAULT 0,
  `admin_allowed` TINYINT(1) NOT NULL DEFAULT 1,
  `is_enabled` TINYINT(1) NOT NULL DEFAULT 1,
  `usage_limit_daily` INT NULL,
  `notes` VARCHAR(255) NULL,
  `price_inr` DECIMAL(10,2) NOT NULL DEFAULT 0,
  `short_description` VARCHAR(255) NULL,
  UNIQUE KEY `uq_report_code` (`report_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------- Commerce ----------
CREATE TABLE IF NOT EXISTS `tbl_service` (
  `service_id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `service_code` VARCHAR(64) NOT NULL,
  `service_name` VARCHAR(120) NOT NULL,
  `description` VARCHAR(500) NULL,
  `duration_minutes` INT NOT NULL DEFAULT 30,
  `price_inr` DECIMAL(10,2) NOT NULL DEFAULT 0,
  `is_active` TINYINT(1) NOT NULL DEFAULT 1,
  `sort_order` INT NOT NULL DEFAULT 0,
  UNIQUE KEY `uq_service_code` (`service_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `tbl_appointment` (
  `appointment_id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `booking_ref` VARCHAR(32) NOT NULL,
  `user_id` INT NOT NULL,
  `service_id` INT NOT NULL,
  `appointment_date` DATE NOT NULL,
  `appointment_time` TIME NOT NULL,
  `amount` DECIMAL(10,2) NOT NULL,
  `currency` CHAR(3) NOT NULL DEFAULT 'INR',
  `payment_status` VARCHAR(32) NOT NULL,
  `appointment_status` VARCHAR(32) NOT NULL,
  `customer_name` VARCHAR(120) NOT NULL,
  `customer_email` VARCHAR(150) NOT NULL,
  `customer_mobile` VARCHAR(20) NULL,
  `notes` VARCHAR(500) NULL,
  `admin_notes` VARCHAR(500) NULL,
  `approved_at` DATETIME NULL,
  `approved_by` INT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY `uq_booking_ref` (`booking_ref`),
  KEY `ix_appt_user` (`user_id`),
  KEY `ix_appt_date_time` (`appointment_date`, `appointment_time`),
  KEY `ix_appt_status` (`appointment_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `tbl_payment` (
  `payment_id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `user_id` INT NOT NULL,
  `appointment_id` INT NULL,
  `report_order_id` INT NULL,
  `amount` DECIMAL(10,2) NOT NULL,
  `currency` CHAR(3) NOT NULL DEFAULT 'INR',
  `gateway` VARCHAR(32) NOT NULL DEFAULT 'ManualUpi',
  `order_id` VARCHAR(64) NOT NULL,
  `transaction_ref` VARCHAR(128) NULL,
  `payment_status` VARCHAR(32) NOT NULL,
  `payment_method` VARCHAR(64) NULL,
  `gateway_response_ref` VARCHAR(255) NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY `uq_payment_order` (`order_id`),
  KEY `ix_pay_user` (`user_id`),
  KEY `ix_pay_appt` (`appointment_id`),
  KEY `ix_pay_txn` (`transaction_ref`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `tbl_public_report_order` (
  `report_order_id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `order_ref` VARCHAR(32) NOT NULL,
  `user_id` INT NULL,
  `report_code` VARCHAR(64) NOT NULL,
  `amount` DECIMAL(10,2) NOT NULL DEFAULT 0,
  `currency` CHAR(3) NOT NULL DEFAULT 'INR',
  `payment_status` VARCHAR(32) NOT NULL,
  `birth_payload_json` MEDIUMTEXT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY `uq_report_order_ref` (`order_ref`),
  KEY `ix_pro_user` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------- Articles (admin CMS) ----------
CREATE TABLE IF NOT EXISTS `tbl_article` (
  `article_id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `title` VARCHAR(200) NOT NULL,
  `slug` VARCHAR(220) NOT NULL,
  `summary` VARCHAR(500) NULL,
  `body_html` MEDIUMTEXT NOT NULL,
  `image_url` VARCHAR(255) NULL,
  `is_published` TINYINT(1) NOT NULL DEFAULT 0,
  `published_at` DATETIME NULL,
  `created_by` INT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY `uq_article_slug` (`slug`),
  KEY `ix_article_published` (`is_published`, `published_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------- Native / match schema (data in part 02) ----------
CREATE TABLE IF NOT EXISTS `tbl_native` (
  `native_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `serial_no` INT NULL,
  `native_name` VARCHAR(150) NOT NULL,
  `gender` CHAR(1) NOT NULL DEFAULT 'M',
  `birth_date` DATE NULL,
  `birth_date_text` VARCHAR(20) NULL,
  `birth_time` TIME NULL,
  `birth_time_text` VARCHAR(20) NULL,
  `birth_place` VARCHAR(120) NULL,
  `country_name` VARCHAR(100) NULL,
  `place_id` INT NULL,
  `latitude` DECIMAL(9,6) NULL,
  `longitude` DECIMAL(9,6) NULL,
  `time_zone` DECIMAL(6,2) NULL,
  `nick_name` VARCHAR(80) NULL,
  `father_name` VARCHAR(150) NULL,
  `address_text` VARCHAR(500) NULL,
  `remarks` VARCHAR(500) NULL,
  `temp_record` TINYINT(1) NOT NULL DEFAULT 0,
  `life_spheres` VARCHAR(255) NULL,
  `house_asc` SMALLINT NULL, `house_sun` SMALLINT NULL, `house_mon` SMALLINT NULL, `house_mar` SMALLINT NULL,
  `house_mer` SMALLINT NULL, `house_jup` SMALLINT NULL, `house_ven` SMALLINT NULL, `house_sat` SMALLINT NULL,
  `house_rah` SMALLINT NULL, `house_ket` SMALLINT NULL,
  `ua4` SMALLINT NULL, `ua5` SMALLINT NULL, `ua6` SMALLINT NULL, `ua7` SMALLINT NULL, `ua8` SMALLINT NULL,
  `vm4` SMALLINT NULL, `vm5` SMALLINT NULL, `vm6` SMALLINT NULL, `vm7` SMALLINT NULL, `vm8` SMALLINT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`native_id`),
  KEY `idx_native_name` (`native_name`),
  KEY `idx_native_birth_date` (`birth_date`),
  KEY `idx_native_place` (`birth_place`),
  KEY `idx_native_serial` (`serial_no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tbl_match_det` (
  `id` SMALLINT UNSIGNED NOT NULL,
  `field_01` VARCHAR(64) NOT NULL DEFAULT '',
  `field_02` VARCHAR(64) NOT NULL DEFAULT '',
  `field_03` VARCHAR(64) NOT NULL DEFAULT '',
  `field_04` VARCHAR(64) NOT NULL DEFAULT '',
  `field_05` VARCHAR(64) NOT NULL DEFAULT '',
  `field_06` VARCHAR(64) NOT NULL DEFAULT '',
  `field_07` VARCHAR(64) NOT NULL DEFAULT '',
  `field_08` VARCHAR(64) NOT NULL DEFAULT '',
  `field_09` VARCHAR(64) NOT NULL DEFAULT '',
  `field_10` VARCHAR(64) NOT NULL DEFAULT '',
  `field_11` VARCHAR(64) NOT NULL DEFAULT '',
  `field_12` VARCHAR(64) NOT NULL DEFAULT '',
  `field_13` VARCHAR(64) NOT NULL DEFAULT '',
  `field_14` VARCHAR(64) NOT NULL DEFAULT '',
  `field_15` VARCHAR(64) NOT NULL DEFAULT '',
  `field_16` VARCHAR(64) NOT NULL DEFAULT '',
  `field_17` VARCHAR(64) NOT NULL DEFAULT '',
  `field_18` VARCHAR(64) NOT NULL DEFAULT '',
  `field_19` VARCHAR(64) NOT NULL DEFAULT '',
  `field_20` VARCHAR(64) NOT NULL DEFAULT '',
  `field_21` VARCHAR(64) NOT NULL DEFAULT '',
  `field_22` VARCHAR(64) NOT NULL DEFAULT '',
  `field_23` VARCHAR(64) NOT NULL DEFAULT '',
  `field_24` VARCHAR(64) NOT NULL DEFAULT '',
  `field_25` VARCHAR(64) NOT NULL DEFAULT '',
  `field_26` VARCHAR(64) NOT NULL DEFAULT '',
  `field_27` VARCHAR(64) NOT NULL DEFAULT '',
  `field_28` VARCHAR(64) NOT NULL DEFAULT '',
  `field_29` VARCHAR(64) NOT NULL DEFAULT '',
  `field_30` VARCHAR(64) NOT NULL DEFAULT '',
  `field_31` VARCHAR(64) NOT NULL DEFAULT '',
  `field_32` VARCHAR(64) NOT NULL DEFAULT '',
  `field_33` VARCHAR(64) NOT NULL DEFAULT '',
  `field_34` VARCHAR(64) NOT NULL DEFAULT '',
  `field_35` VARCHAR(64) NOT NULL DEFAULT '',
  `field_36` VARCHAR(64) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- SEED DATA
-- ============================================================

INSERT IGNORE INTO `tbl_app_role` (`role_code`, `role_name`) VALUES
('Admin', 'Administrator'),
('Public', 'Public User');

INSERT IGNORE INTO `tbl_report_definition`
(`report_code`, `report_name`, `public_allowed`, `admin_allowed`, `is_enabled`, `notes`, `price_inr`, `short_description`) VALUES
('KUNDLI_BASIC', 'Basic Kundli Chart', 1, 1, 1, 'Public chart/summary', 299.00, '2-page PDF: basic birth info + kundli chart'),
('KUNDLI_DETAILED', 'Detailed Horoscope Report', 0, 1, 1, 'Official letterhead PDF', 0, NULL),
('NUMEROLOGY_BASIC', 'Basic Numerology', 1, 1, 1, 'Core numbers', 199.00, 'Basic core numerology numbers PDF (limited)'),
('NUMEROLOGY_FULL', 'Full Numerology Report', 0, 1, 1, 'Admin full', 0, NULL),
('MATCH_MAKING', 'Match Making', 0, 1, 1, NULL, 0, NULL),
('LAL_KITAB', 'Lal Kitab', 0, 1, 1, NULL, 0, NULL),
('VARSH_PHAL', 'Varsh Phal', 0, 1, 1, NULL, 0, NULL),
('MUHURAT', 'Muhurat / Kaal Shuddhi', 0, 1, 1, NULL, 0, NULL),
('VIVAH_MUHURTA', 'Vivah Muhurta', 0, 1, 1, NULL, 0, NULL),
('GOCHAR', 'Gochar Phal', 0, 1, 1, NULL, 0, NULL),
('PALMISTRY', 'Palmistry', 0, 1, 1, NULL, 0, NULL),
('RECORDS', 'Native Records', 0, 1, 1, NULL, 0, NULL),
('PLACES_ADMIN', 'Place Atlas Admin', 0, 1, 1, NULL, 0, NULL);

INSERT IGNORE INTO `tbl_service`
(`service_code`, `service_name`, `description`, `duration_minutes`, `price_inr`, `is_active`, `sort_order`) VALUES
('CONSULT_GENERAL', 'Astrology Consultation', 'Personal consultation with Indu Sharma (pre-appointment).', 45, 1100.00, 1, 1),
('CONSULT_MATCH', 'Match Making Consultation', 'Ashtakoot / compatibility discussion.', 45, 1500.00, 1, 2),
('CONSULT_MUHURAT', 'Muhurat Consultation', 'Auspicious timing guidance.', 30, 900.00, 1, 3);

-- Sample Indian cities (full atlas loaded in 03_places_from_access.sql)
INSERT IGNORE INTO `tbl_place` (`place_name`, `country_name`, `latitude`, `longitude`, `time_zone`, `summer_time_correction`) VALUES
('New Delhi', 'India', 28.613900, 77.209000, '5.5', NULL),
('Mumbai', 'India', 19.076000, 72.877700, '5.5', NULL),
('Bangalore', 'India', 12.971600, 77.594600, '5.5', NULL),
('Chennai', 'India', 13.082700, 80.270700, '5.5', NULL),
('Kolkata', 'India', 22.572600, 88.363900, '5.5', NULL),
('Hyderabad', 'India', 17.385000, 78.486700, '5.5', NULL),
('Pune', 'India', 18.520400, 73.856700, '5.5', NULL),
('Ahmedabad', 'India', 23.022500, 72.571400, '5.5', NULL),
('Jaipur', 'India', 26.912400, 75.787300, '5.5', NULL),
('Lucknow', 'India', 26.846700, 80.946200, '5.5', NULL),
('Chandigarh', 'India', 30.733300, 76.779400, '5.5', NULL),
('Patiala', 'India', 30.339800, 76.386900, '5.5', NULL),
('Mohali', 'India', 30.704600, 76.717900, '5.5', NULL),
('Ludhiana', 'India', 30.901000, 75.857300, '5.5', NULL),
('Amritsar', 'India', 31.634000, 74.872300, '5.5', NULL);

INSERT IGNORE INTO `tbl_country` (`country_name`) VALUES ('India');

-- Zodiac seed: clear + reload 12 signs (safe for fresh BigRock DB / Workbench)
TRUNCATE TABLE `Zodiac`;

INSERT INTO `Zodiac` (`Name`, `HindiName`, `Icon`, `Image`, `Description`, `LuckyNumber`, `LuckyColor`, `LuckyDay`, `Planet`, `Element`, `Nature`, `Strength`, `Weakness`, `Career`, `Love`, `Health`, `Finance`, `Personality`, `Characteristics`, `CompatibleSigns`) VALUES
('Aries', 'Mesha', 'fas fa-fire', '', 'Aries is the first sign of the zodiac, known for courage, confidence, and enthusiasm. Ruled by Mars, Aries natives are natural-born leaders who thrive on challenges and adventure. They possess an incredible amount of energy and are always ready to take the initiative in any situation.', '1, 8, 17', 'Red', 'Tuesday', 'Mars', 'Fire', 'Cardinal', 'Courageous, confident, enthusiastic, optimistic, honest, passionate', 'Impatient, moody, short-tempered, impulsive, aggressive', 'Leadership roles, entrepreneurship, sports, military, police, surgery, engineering', 'Passionate and adventurous in love. They are direct and bold partners who value honesty and excitement in relationships.', 'Prone to headaches, migraines, and stress-related issues. Should manage anger and practice relaxation techniques.', 'Bold financial decisions. Good earners who are not afraid to take risks. Natural talent for business leadership.', 'Dynamic and energetic, Aries individuals are always ready for action. They possess a natural magnetism that draws people toward them. Their confidence and determination make them excellent leaders who inspire others.', 'Bold, ambitious, and energetic. Aries natives are pioneers who love to be first in everything they do. Their competitive spirit and drive help them achieve remarkable success in their chosen fields.', 'Leo, Sagittarius, Gemini, Aquarius'),

('Taurus', 'Vrishabha', 'fas fa-leaf', '', 'Taurus is an earth sign known for reliability, patience, and practicality. Ruled by Venus, Taurus natives appreciate beauty, comfort, and the finer things in life. They are grounded individuals who value stability and security above all else.', '2, 6, 9', 'Green, Pink', 'Friday', 'Venus', 'Earth', 'Fixed', 'Reliable, patient, practical, devoted, responsible, stable', 'Stubborn, possessive, uncompromising, materialistic', 'Finance, banking, art, agriculture, architecture, interior design, culinary arts', 'Loyal and devoted partners who value long-term commitment. They express love through physical affection and material generosity.', 'Throat and neck issues. Should avoid overeating and maintain a balanced diet for thyroid health.', 'Financially stable with excellent money management skills. Good savers who build wealth steadily over time.', 'Taurus individuals are grounded and practical. They value stability and are known for their unwavering determination. Their love for beauty and comfort makes them excellent hosts and aesthetes.', 'Dependable, patient, and sensual. Taureans have an innate appreciation for beauty and luxury. Their steadfast nature makes them reliable friends and partners who stand by their loved ones through thick and thin.', 'Virgo, Capricorn, Cancer, Pisces'),

('Gemini', 'Mithuna', 'fas fa-wind', '', 'Gemini is an air sign known for versatility, communication skills, and intellectual curiosity. Ruled by Mercury, Gemini natives are adaptable and love social interactions. They have a dual nature that makes them fascinating and unpredictable.', '3, 5, 7', 'Yellow, Light Green', 'Wednesday', 'Mercury', 'Air', 'Mutable', 'Gentle, curious, adaptable, quick learner, versatile, expressive', 'Nervous, inconsistent, indecisive, superficial', 'Journalism, teaching, writing, sales, communication, media, marketing, public relations', 'Playful and communicative in relationships. They need intellectual stimulation and variety to keep the spark alive.', 'Respiratory issues, anxiety, and nervous system problems. Should practice breathing exercises and maintain calm.', 'Multiple income sources. Versatile earners who can adapt to various financial opportunities. Natural talent for business communication.', 'Gemini individuals are witty and charming. They have a dual nature that makes them fascinating conversationalists. Their quick wit and versatile interests make them the life of any social gathering.', 'Versatile, curious, and expressive. Geminis are the communicators of the zodiac with an insatiable thirst for knowledge. Their adaptability allows them to thrive in any social or professional environment.', 'Libra, Aquarius, Aries, Leo'),

('Cancer', 'Karka', 'fas fa-water', '', 'Cancer is a water sign known for emotional depth, intuition, and nurturing qualities. Ruled by the Moon, Cancer natives are deeply connected to family and home. They possess a powerful intuition that guides them through life.', '2, 3, 15', 'White, Silver', 'Monday', 'Moon', 'Water', 'Cardinal', 'Tenacious, highly imaginative, loyal, emotional, sympathetic, persuasive', 'Moody, pessimistic, suspicious, manipulative, insecure', 'Healthcare, education, hospitality, real estate, social work, cooking, childcare', 'Deeply caring and protective partners. They create a warm, loving home environment and are devoted to family.', 'Digestive system and stomach issues. Should manage stress and emotional eating for better digestive health.', 'Cautious with money, good at saving. They prefer financial security and long-term investments over risky ventures.', 'Cancer individuals are deeply intuitive and sentimental. They value emotional security and family bonds above all else. Their nurturing nature makes them natural caregivers who create warmth wherever they go.', 'Nurturing, intuitive, and protective. Cancerians have a remarkable ability to understand emotions, both their own and others. Their strong connection to home and family makes them incredibly loyal and devoted individuals.', 'Scorpio, Pisces, Taurus, Virgo'),

('Leo', 'Simha', 'fas fa-sun', '', 'Leo is a fire sign known for charisma, creativity, and natural leadership. Ruled by the Sun, Leo natives shine brightly and love being the center of attention. They have a warm, generous heart that attracts people naturally.', '1, 3, 10', 'Gold, Orange', 'Sunday', 'Sun', 'Fire', 'Fixed', 'Creative, passionate, generous, warm-hearted, cheerful, humorous', 'Arrogant, stubborn, self-centered, lazy, inflexible', 'Entertainment, management, politics, fashion, performing arts, leadership roles', 'Generous and warm in love. They are romantic partners who love grand gestures and expect loyalty and admiration.', 'Heart and spine issues. Should maintain cardiovascular health through regular exercise and avoid overexertion.', 'Generous spenders who attract wealth naturally. They have a knack for finding lucrative opportunities and enjoy luxury.', 'Leo individuals are natural stars who command attention wherever they go. Their warmth and generosity make them beloved by many. They lead with confidence and inspire others through their enthusiasm.', 'Bold, charismatic, and confident. Leos are born leaders who radiate warmth and creativity. Their magnetic personality and generous spirit make them natural centerpieces of any group, inspiring loyalty and admiration.', 'Aries, Sagittarius, Gemini, Libra'),

('Virgo', 'Kanya', 'fas fa-seedling', '', 'Virgo is an earth sign known for analytical thinking, attention to detail, and practical service. Ruled by Mercury, Virgo natives are perfectionists with a strong sense of duty. They have an exceptional ability to organize and improve.', '5, 14, 23', 'Green, Brown', 'Wednesday', 'Mercury', 'Earth', 'Mutable', 'Loyal, analytical, kind, hardworking, practical, reliable', 'Shyness, overly critical, all work no play, worrisome', 'Healthcare, research, editing, accounting, data analysis, quality control, nutrition', 'Supportive and detail-oriented partners. They show love through acts of service and practical help.', 'Digestive issues and anxiety. Should practice mindfulness and maintain a healthy, organic diet.', 'Practical and careful with money. Excellent at budgeting and finding value. Natural financial planners.', 'Virgo individuals are meticulous and reliable. They have an innate ability to improve everything they touch. Their analytical mind and attention to detail make them excellent problem solvers and advisors.', 'Analytical, practical, and humble. Virgos have an exceptional eye for detail and a deep desire to serve and help others. Their methodical approach to life ensures that everything they undertake is done to the highest standard.', 'Taurus, Capricorn, Cancer, Scorpio'),

('Libra', 'Tula', 'fas fa-balance-scale', '', 'Libra is an air sign known for balance, harmony, and aesthetic sense. Ruled by Venus, Libra natives seek justice, beauty, and peaceful relationships. They are natural diplomats who can see all sides of any situation.', '4, 6, 13', 'Pink, Light Blue', 'Friday', 'Venus', 'Air', 'Cardinal', 'Cooperative, diplomatic, gracious, fair-minded, social, idealistic', 'Indecisive, avoids confrontation, self-pitying, gullible', 'Law, diplomacy, design, counseling, human resources, art curation, mediation', 'Romantic and harmonious in love. They seek partnership and balance, creating beautiful relationships.', 'Kidney and lower back issues. Should maintain hydration and practice gentle exercises like yoga.', 'Balanced approach to finances. They enjoy spending on beauty and luxury but maintain equilibrium.', 'Libra individuals are natural peacemakers who value harmony and beauty in all aspects of life. Their diplomatic nature and sense of fairness make them excellent mediators and advisors in both personal and professional settings.', 'Diplomatic, graceful, and fair. Librans have an innate sense of justice and a deep appreciation for beauty and harmony. Their ability to see multiple perspectives makes them excellent judges, mediators, and partners.', 'Gemini, Aquarius, Leo, Sagittarius'),

('Scorpio', 'Vrischika', 'fas fa-spider', '', 'Scorpio is a water sign known for intensity, passion, and transformative power. Ruled by Pluto, Scorpio natives are deeply emotional and fiercely loyal. They possess an incredible depth of character that is both mysterious and magnetic.', '8, 11, 18', 'Dark Red, Black', 'Tuesday', 'Pluto', 'Water', 'Fixed', 'Resourceful, brave, passionate, loyal, strategic, focused', 'Distrusting, jealous, secretive, violent, resentful', 'Research, investigation, surgery, psychology, detective work, crisis management, science', 'Intensely passionate and loyal partners. They love deeply and expect complete loyalty and honesty in return.', 'Reproductive system and intestines. Should manage emotional intensity and practice regular detoxification.', 'Strategic with investments. They have a talent for uncovering hidden financial opportunities and managing resources.', 'Scorpio individuals are powerful and magnetic. They possess a depth of emotion that few can match. Their intensity and passion make them unforgettable, and their strategic mind ensures they achieve their goals.', 'Intense, mysterious, and determined. Scorpios have an almost supernatural ability to see through deception and understand the deepest truths. Their transformative nature allows them to reinvent themselves and rise from challenges stronger than ever.', 'Cancer, Pisces, Virgo, Capricorn'),

('Sagittarius', 'Dhanu', 'fas fa-horse', '', 'Sagittarius is a fire sign known for optimism, adventure, and philosophical thinking. Ruled by Jupiter, Sagittarius natives love freedom and exploration. They have an infectious enthusiasm that inspires everyone around them.', '3, 7, 9', 'Purple, Blue', 'Thursday', 'Jupiter', 'Fire', 'Mutable', 'Generous, idealistic, great sense of humor, adventurous, open-minded', 'Promises more than can deliver, impatient, tactless, inconsistent', 'Travel, education, publishing, philosophy, outdoor sports, foreign trade, tourism', 'Adventurous and honest in love. They value freedom and intellectual connection in their relationships.', 'Hip and thigh issues. Should maintain physical activity and avoid overindulgence in food and drink.', 'Optimistic about money, lucky with investments. Natural talent for finding opportunities through travel and education.', 'Sagittarius individuals are free spirits who love adventure. Their optimism is contagious and their humor is legendary. They have a deep philosophical nature that drives them to seek truth and meaning in everything.', 'Adventurous, optimistic, and philosophical. Sagittarians are the explorers of the zodiac, always seeking new experiences and knowledge. Their boundless enthusiasm and honest nature make them inspiring companions on any journey.', 'Aries, Leo, Libra, Aquarius'),

('Capricorn', 'Makara', 'fas fa-mountain', '', 'Capricorn is an earth sign known for discipline, ambition, and practical wisdom. Ruled by Saturn, Capricorn natives are determined achievers who value structure and hard work. They are the masters of long-term planning and persistence.', '4, 8, 13', 'Brown, Black, Dark Green', 'Saturday', 'Saturn', 'Earth', 'Cardinal', 'Responsible, disciplined, self-control, good managers, patient, resourceful', 'Know-it-all, unforgiving, condescending, expecting the worst, rigid', 'Business, management, engineering, government, banking, real estate, administration', 'Loyal and committed partners. They take relationships seriously and are willing to work through challenges.', 'Bones, joints, and teeth issues. Should maintain joint health through exercise and proper nutrition.', 'Excellent financial planners and long-term thinkers. They build wealth steadily through discipline and smart investments.', 'Capricorn individuals are the achievers of the zodiac. Their discipline and determination help them reach the highest peaks of success. They combine ambition with practicality, making them formidable in any professional field.', 'Ambitious, disciplined, and wise. Capricorns are the architects of their own success, building their dreams brick by brick with patience and unwavering determination. Their wisdom and reliability make them pillars of strength.', 'Taurus, Virgo, Scorpio, Pisces'),

('Aquarius', 'Kumbha', 'fas fa-tint', '', 'Aquarius is an air sign known for innovation, humanitarianism, and independent thinking. Ruled by Uranus, Aquarius natives are visionaries ahead of their time. They have a unique perspective on life that challenges conventional thinking.', '4, 7, 11', 'Turquoise, Blue', 'Saturday', 'Uranus', 'Air', 'Fixed', 'Progressive, original, independent, humanitarian, inventive, intellectual', 'Runs from emotional expression, temperamental, uncompromising, aloof', 'Technology, science, social work, activism, innovation, aerospace, environmental work', 'Intellectual and unconventional in love. They value mental connection and freedom within relationships.', 'Circulatory system and ankles. Should maintain cardiovascular health and avoid sedentary lifestyles.', 'Unconventional but successful with money. They often find unique ways to generate income through innovation.', 'Aquarius individuals are the innovators of the zodiac. They think differently and are driven by a desire to make the world better. Their humanitarian spirit and original ideas set them apart from the crowd.', 'Innovative, independent, and humanitarian. Aquarians are the visionaries who see the world not as it is, but as it could be. Their progressive thinking and genuine concern for humanity make them agents of positive change.', 'Gemini, Libra, Aries, Sagittarius'),

('Pisces', 'Meena', 'fas fa-fish', '', 'Pisces is a water sign known for compassion, artistic talent, and spiritual depth. Ruled by Neptune, Pisces natives are dreamers with profound emotional intelligence. They have a deep connection to the unseen world and possess remarkable empathy.', '3, 9, 12', 'Sea Green, Lavender', 'Thursday', 'Neptune', 'Water', 'Mutable', 'Compassionate, artistic, intuitive, gentle, wise, musical', 'Fearful, overly trusting, sad, desire to escape reality, indecisive', 'Arts, music, healing, spirituality, film, photography, poetry, counseling, charity', 'Romantic and selfless in love. They give themselves completely to their partners and seek deep spiritual connection.', 'Feet and immune system issues. Should practice grounding exercises and maintain emotional boundaries for better health.', 'Generous but needs to be careful with spending. They should set financial boundaries and avoid lending money impulsively.', 'Pisces individuals are the most empathetic of the zodiac. Their artistic sensibility and spiritual depth make them truly unique. They have the rare ability to feel and understand emotions on a profound level.', 'Compassionate, artistic, and intuitive. Pisceans possess a deep well of emotional wisdom and creative talent. Their ability to connect with the spiritual realm and express themselves through art makes them truly magical beings.', 'Cancer, Scorpio, Taurus, Capricorn');

-- ============================================================
-- STORED PROCEDURES: Place (tables: place + country)
-- ============================================================



SET FOREIGN_KEY_CHECKS = 1;

-- End of Part 1. Next: 02_match_and_natives.sql then 03_places_from_access.sql
