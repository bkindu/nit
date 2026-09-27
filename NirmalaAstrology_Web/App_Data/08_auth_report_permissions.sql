-- Phase A: Auth + report permissions (indu_astro)
-- Run manually if auto-bootstrap is disabled.

CREATE TABLE IF NOT EXISTS tbl_app_user (
  user_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(64) NOT NULL,
  password_hash VARCHAR(256) NOT NULL,
  password_salt VARCHAR(128) NOT NULL,
  display_name VARCHAR(120) NULL,
  email VARCHAR(150) NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY uq_app_user_username (username)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS tbl_app_role (
  role_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  role_code VARCHAR(32) NOT NULL,
  role_name VARCHAR(64) NOT NULL,
  UNIQUE KEY uq_app_role_code (role_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS tbl_app_user_role (
  user_id INT NOT NULL,
  role_id INT NOT NULL,
  PRIMARY KEY (user_id, role_id),
  CONSTRAINT fk_ur_user FOREIGN KEY (user_id) REFERENCES tbl_app_user(user_id) ON DELETE CASCADE,
  CONSTRAINT fk_ur_role FOREIGN KEY (role_id) REFERENCES tbl_app_role(role_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS tbl_report_definition (
  report_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  report_code VARCHAR(64) NOT NULL,
  report_name VARCHAR(120) NOT NULL,
  public_allowed TINYINT(1) NOT NULL DEFAULT 0,
  admin_allowed TINYINT(1) NOT NULL DEFAULT 1,
  is_enabled TINYINT(1) NOT NULL DEFAULT 1,
  usage_limit_daily INT NULL,
  notes VARCHAR(255) NULL,
  UNIQUE KEY uq_report_code (report_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO tbl_app_role (role_code, role_name) VALUES
('Admin', 'Administrator'),
('Public', 'Public User');

INSERT IGNORE INTO tbl_report_definition
(report_code, report_name, public_allowed, admin_allowed, is_enabled, notes) VALUES
('KUNDLI_BASIC', 'Basic Kundli Chart', 1, 1, 1, 'Public chart/summary'),
('KUNDLI_DETAILED', 'Detailed Horoscope Report', 0, 1, 1, 'Official letterhead PDF'),
('NUMEROLOGY_BASIC', 'Basic Numerology', 1, 1, 1, 'Core numbers'),
('NUMEROLOGY_FULL', 'Full Numerology Report', 0, 1, 1, 'Admin full'),
('MATCH_MAKING', 'Match Making', 0, 1, 1, NULL),
('LAL_KITAB', 'Lal Kitab', 0, 1, 1, NULL),
('VARSH_PHAL', 'Varsh Phal', 0, 1, 1, NULL),
('MUHURAT', 'Muhurat / Kaal Shuddhi', 0, 1, 1, NULL),
('VIVAH_MUHURTA', 'Vivah Muhurta', 0, 1, 1, NULL),
('GOCHAR', 'Gochar Phal', 0, 1, 1, NULL),
('PALMISTRY', 'Palmistry', 0, 1, 1, NULL),
('RECORDS', 'Native Records', 0, 1, 1, NULL),
('PLACES_ADMIN', 'Place Atlas Admin', 0, 1, 1, NULL);
