-- ============================================================
-- NIRMALA ASTROLOGY - MySQL Database Creation Script
-- Compatible with GoDaddy MySQL Hosting
-- ============================================================

-- Create Database (if not already created on GoDaddy)
-- CREATE DATABASE IF NOT EXISTS nirmala_astrology
-- CHARACTER SET utf8mb4
-- COLLATE utf8mb4_unicode_ci;
-- USE nirmala_astrology;

-- ============================================================
-- TABLE: Place
-- Stores birth place information with coordinates and timezone
-- ============================================================
CREATE TABLE IF NOT EXISTS `Place` (
    `PlaceId` INT(11) NOT NULL AUTO_INCREMENT,
    `PlaceName` VARCHAR(200) NOT NULL,
    `State` VARCHAR(100) NOT NULL,
    `Country` VARCHAR(100) NOT NULL,
    `Latitude` DECIMAL(10, 7) NOT NULL,
    `Longitude` DECIMAL(10, 7) NOT NULL,
    `Timezone` VARCHAR(50) NOT NULL,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (`PlaceId`),
    INDEX `idx_place_name` (`PlaceName`),
    INDEX `idx_state` (`State`),
    INDEX `idx_country` (`Country`),
    INDEX `idx_active` (`IsActive`),
    INDEX `idx_search` (`PlaceName`, `State`, `Country`, `IsActive`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- TABLE: Customer
-- Stores customer / Kundli request data
-- ============================================================
CREATE TABLE IF NOT EXISTS `Customer` (
    `CustomerId` INT(11) NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(150) NOT NULL,
    `DOB` DATE NOT NULL,
    `BirthTime` TIME NOT NULL,
    `PlaceId` INT(11) NOT NULL,
    `Latitude` DECIMAL(10, 7) NOT NULL,
    `Longitude` DECIMAL(10, 7) NOT NULL,
    `Timezone` VARCHAR(50) DEFAULT '',
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`CustomerId`),
    INDEX `idx_customer_name` (`Name`),
    INDEX `idx_customer_dob` (`DOB`),
    INDEX `idx_place_id` (`PlaceId`),
    CONSTRAINT `fk_customer_place` FOREIGN KEY (`PlaceId`) REFERENCES `Place` (`PlaceId`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- TABLE: Zodiac
-- Stores all 12 zodiac sign details
-- ============================================================
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
    UNIQUE INDEX `idx_zodiac_name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- INSERT: Sample Places (Indian Cities)
-- ============================================================
INSERT INTO `Place` (`PlaceName`, `State`, `Country`, `Latitude`, `Longitude`, `Timezone`) VALUES
('New Delhi', 'Delhi', 'India', 28.6139000, 77.2090000, 'Asia/Kolkata'),
('Mumbai', 'Maharashtra', 'India', 19.0760000, 72.8777000, 'Asia/Kolkata'),
('Bangalore', 'Karnataka', 'India', 12.9716000, 77.5946000, 'Asia/Kolkata'),
('Chennai', 'Tamil Nadu', 'India', 13.0827000, 80.2707000, 'Asia/Kolkata'),
('Kolkata', 'West Bengal', 'India', 22.5726000, 88.3639000, 'Asia/Kolkata'),
('Hyderabad', 'Telangana', 'India', 17.3850000, 78.4867000, 'Asia/Kolkata'),
('Pune', 'Maharashtra', 'India', 18.5204000, 73.8567000, 'Asia/Kolkata'),
('Ahmedabad', 'Gujarat', 'India', 23.0225000, 72.5714000, 'Asia/Kolkata'),
('Jaipur', 'Rajasthan', 'India', 26.9124000, 75.7873000, 'Asia/Kolkata'),
('Lucknow', 'Uttar Pradesh', 'India', 26.8467000, 80.9462000, 'Asia/Kolkata'),
('Chandigarh', 'Chandigarh', 'India', 30.7333000, 76.7794000, 'Asia/Kolkata'),
('Bhopal', 'Madhya Pradesh', 'India', 23.2599000, 77.4126000, 'Asia/Kolkata'),
('Patna', 'Bihar', 'India', 25.6093000, 85.1376000, 'Asia/Kolkata'),
('Bhubaneswar', 'Odisha', 'India', 20.2961000, 85.8245000, 'Asia/Kolkata'),
('Kochi', 'Kerala', 'India', 9.9312000, 76.2673000, 'Asia/Kolkata'),
('Guwahati', 'Assam', 'India', 26.1445000, 91.7362000, 'Asia/Kolkata'),
('Varanasi', 'Uttar Pradesh', 'India', 25.3176000, 82.9739000, 'Asia/Kolkata'),
('Agra', 'Uttar Pradesh', 'India', 27.1767000, 78.0081000, 'Asia/Kolkata'),
('Nagpur', 'Maharashtra', 'India', 21.1458000, 79.0882000, 'Asia/Kolkata'),
('Indore', 'Madhya Pradesh', 'India', 22.7196000, 75.8577000, 'Asia/Kolkata'),
('Thiruvananthapuram', 'Kerala', 'India', 8.5241000, 76.9366000, 'Asia/Kolkata'),
('Dehradun', 'Uttarakhand', 'India', 30.3165000, 78.0322000, 'Asia/Kolkata'),
('Ranchi', 'Jharkhand', 'India', 23.3441000, 85.3096000, 'Asia/Kolkata'),
('Raipur', 'Chhattisgarh', 'India', 21.2514000, 81.6296000, 'Asia/Kolkata'),
('Surat', 'Gujarat', 'India', 21.1702000, 72.8311000, 'Asia/Kolkata'),
('Vadodara', 'Gujarat', 'India', 22.3072000, 73.1812000, 'Asia/Kolkata'),
('Coimbatore', 'Tamil Nadu', 'India', 11.0168000, 76.9558000, 'Asia/Kolkata'),
('Madurai', 'Tamil Nadu', 'India', 9.9252000, 78.1198000, 'Asia/Kolkata'),
('Mysore', 'Karnataka', 'India', 12.2958000, 76.6394000, 'Asia/Kolkata'),
('Visakhapatnam', 'Andhra Pradesh', 'India', 17.6868000, 83.2185000, 'Asia/Kolkata');

-- ============================================================
-- INSERT: All 12 Zodiac Signs with complete data
-- ============================================================
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

DROP PROCEDURE IF EXISTS `sp_SearchPlaces`;
DELIMITER $$
CREATE PROCEDURE `sp_SearchPlaces`(
    IN p_keyword VARCHAR(100)
)
BEGIN
    SELECT p.PlaceID, p.PlaceName, p.CountryID, c.CountryName,
           p.Latitude, p.Longitude, p.TimeZone, p.SMTCorr
    FROM place p
    INNER JOIN country c ON p.CountryID = c.CountryID
    WHERE p.PlaceName LIKE CONCAT('%', p_keyword, '%')
    ORDER BY p.PlaceName ASC
    LIMIT 20;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `sp_GetPlaceById`;
DELIMITER $$
CREATE PROCEDURE `sp_GetPlaceById`(
    IN p_PlaceID INT
)
BEGIN
    SELECT p.PlaceID, p.PlaceName, p.CountryID, c.CountryName,
           p.Latitude, p.Longitude, p.TimeZone, p.SMTCorr
    FROM place p
    INNER JOIN country c ON p.CountryID = c.CountryID
    WHERE p.PlaceID = p_PlaceID;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `sp_InsertPlace`;
DELIMITER $$
CREATE PROCEDURE `sp_InsertPlace`(
    IN p_PlaceName VARCHAR(100),
    IN p_CountryID SMALLINT,
    IN p_Latitude DECIMAL(10,4),
    IN p_Longitude DECIMAL(10,4),
    IN p_TimeZone VARCHAR(10),
    IN p_SMTCorr CHAR(1)
)
BEGIN
    INSERT INTO place (PlaceName, CountryID, Latitude, Longitude, TimeZone, SMTCorr)
    VALUES (p_PlaceName, p_CountryID, p_Latitude, p_Longitude, p_TimeZone, p_SMTCorr);
    SELECT LAST_INSERT_ID() AS NewId;
END$$
DELIMITER ;

-- ============================================================
-- STORED PROCEDURE: Country
-- ============================================================

DROP PROCEDURE IF EXISTS `sp_GetAllCountries`;
DELIMITER $$
CREATE PROCEDURE `sp_GetAllCountries`()
BEGIN
    SELECT CountryID, CountryName
    FROM country
    ORDER BY CountryName ASC;
END$$
DELIMITER ;

-- ============================================================
-- STORED PROCEDURES: Zodiac
-- ============================================================

DROP PROCEDURE IF EXISTS `sp_GetAllZodiacs`;
DELIMITER $$
CREATE PROCEDURE `sp_GetAllZodiacs`()
BEGIN
    SELECT ZodiacId, Name, HindiName, Icon, Image, Description,
           LuckyNumber, LuckyColor, LuckyDay, Planet, Element, Nature, Strength, Weakness,
           Career, Love, Health, Finance, Personality, Characteristics, CompatibleSigns, CreatedDate
    FROM Zodiac
    ORDER BY ZodiacId ASC;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `sp_GetZodiacByName`;
DELIMITER $$
CREATE PROCEDURE `sp_GetZodiacByName`(
    IN p_Name VARCHAR(50)
)
BEGIN
    SELECT ZodiacId, Name, HindiName, Icon, Image, Description,
           LuckyNumber, LuckyColor, LuckyDay, Planet, Element, Nature, Strength, Weakness,
           Career, Love, Health, Finance, Personality, Characteristics, CompatibleSigns, CreatedDate
    FROM Zodiac
    WHERE LOWER(Name) = LOWER(p_Name)
    LIMIT 1;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `sp_GetZodiacById`;
DELIMITER $$
CREATE PROCEDURE `sp_GetZodiacById`(
    IN p_ZodiacId INT
)
BEGIN
    SELECT ZodiacId, Name, HindiName, Icon, Image, Description,
           LuckyNumber, LuckyColor, LuckyDay, Planet, Element, Nature, Strength, Weakness,
           Career, Love, Health, Finance, Personality, Characteristics, CompatibleSigns, CreatedDate
    FROM Zodiac
    WHERE ZodiacId = p_ZodiacId
    LIMIT 1;
END$$
DELIMITER ;