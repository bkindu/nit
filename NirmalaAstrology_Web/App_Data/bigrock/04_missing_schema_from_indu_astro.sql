-- Target database: nit
SET NAMES utf8mb4;
SET SQL_SAFE_UPDATES=0;
SET FOREIGN_KEY_CHECKS=0;
USE `nit`;
-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: indu_astro
-- ------------------------------------------------------
-- Server version	8.0.43

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `stg_aspect_of_planets`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_aspect_of_planets` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Aspects` longtext COLLATE utf8mb4_unicode_ci,
  `Sun` longtext COLLATE utf8mb4_unicode_ci,
  `Mon` longtext COLLATE utf8mb4_unicode_ci,
  `Mar` longtext COLLATE utf8mb4_unicode_ci,
  `Mer` longtext COLLATE utf8mb4_unicode_ci,
  `Jup` longtext COLLATE utf8mb4_unicode_ci,
  `Ven` longtext COLLATE utf8mb4_unicode_ci,
  `Sat` longtext COLLATE utf8mb4_unicode_ci,
  `Rah` longtext COLLATE utf8mb4_unicode_ci,
  `Ket` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_brg_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_brg_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID_Brg` longtext COLLATE utf8mb4_unicode_ci,
  `Brg_Name` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_E` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_H` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_P` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_E_Granth_Soft_Web` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_combustion_4_uday_astha`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_combustion_4_uday_astha` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `MG_Ast_Dt` longtext COLLATE utf8mb4_unicode_ci,
  `MG_Ast_Tm` longtext COLLATE utf8mb4_unicode_ci,
  `MG_UA` longtext COLLATE utf8mb4_unicode_ci,
  `BD_Ast_Dt` longtext COLLATE utf8mb4_unicode_ci,
  `BD_Ast_Tm` longtext COLLATE utf8mb4_unicode_ci,
  `BD_UA` longtext COLLATE utf8mb4_unicode_ci,
  `GR_Ast_Dt` longtext COLLATE utf8mb4_unicode_ci,
  `GR_Ast_Tm` longtext COLLATE utf8mb4_unicode_ci,
  `GR_UA` longtext COLLATE utf8mb4_unicode_ci,
  `SK_Ast_Dt` longtext COLLATE utf8mb4_unicode_ci,
  `SK_Ast_Tm` longtext COLLATE utf8mb4_unicode_ci,
  `SK_UA` longtext COLLATE utf8mb4_unicode_ci,
  `SN_Ast_Dt` longtext COLLATE utf8mb4_unicode_ci,
  `SN_Ast_Tm` longtext COLLATE utf8mb4_unicode_ci,
  `SN_UA` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1263 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_dasa_lordship_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_dasa_lordship_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `For_Asc` longtext COLLATE utf8mb4_unicode_ci,
  `Sun` longtext COLLATE utf8mb4_unicode_ci,
  `Mon` longtext COLLATE utf8mb4_unicode_ci,
  `Mar` longtext COLLATE utf8mb4_unicode_ci,
  `Mer` longtext COLLATE utf8mb4_unicode_ci,
  `Jup` longtext COLLATE utf8mb4_unicode_ci,
  `Ven` longtext COLLATE utf8mb4_unicode_ci,
  `Sat` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_dasa_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_dasa_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `DasaLord_E` longtext COLLATE utf8mb4_unicode_ci,
  `DasaPhal_E` longtext COLLATE utf8mb4_unicode_ci,
  `DasaLord_H` longtext COLLATE utf8mb4_unicode_ci,
  `DasaPhal_H` longtext COLLATE utf8mb4_unicode_ci,
  `DasaLord_P` longtext COLLATE utf8mb4_unicode_ci,
  `DasaPhal_P` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=95 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_gochar_phal8plts`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_gochar_phal8plts` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `House` longtext COLLATE utf8mb4_unicode_ci,
  `Sun_E` longtext COLLATE utf8mb4_unicode_ci,
  `Mon_E` longtext COLLATE utf8mb4_unicode_ci,
  `Mar_E` longtext COLLATE utf8mb4_unicode_ci,
  `Mer_E` longtext COLLATE utf8mb4_unicode_ci,
  `Jup_E` longtext COLLATE utf8mb4_unicode_ci,
  `Ven_E` longtext COLLATE utf8mb4_unicode_ci,
  `Sat_E` longtext COLLATE utf8mb4_unicode_ci,
  `Rah_E` longtext COLLATE utf8mb4_unicode_ci,
  `Ket_E` longtext COLLATE utf8mb4_unicode_ci,
  `Sun_H` longtext COLLATE utf8mb4_unicode_ci,
  `Mon_H` longtext COLLATE utf8mb4_unicode_ci,
  `Mar_H` longtext COLLATE utf8mb4_unicode_ci,
  `Mer_H` longtext COLLATE utf8mb4_unicode_ci,
  `Jup_H` longtext COLLATE utf8mb4_unicode_ci,
  `Ven_H` longtext COLLATE utf8mb4_unicode_ci,
  `Sat_H` longtext COLLATE utf8mb4_unicode_ci,
  `Rah_H` longtext COLLATE utf8mb4_unicode_ci,
  `Ket_H` longtext COLLATE utf8mb4_unicode_ci,
  `Sun_P` longtext COLLATE utf8mb4_unicode_ci,
  `Mon_P` longtext COLLATE utf8mb4_unicode_ci,
  `Mar_P` longtext COLLATE utf8mb4_unicode_ci,
  `Mer_P` longtext COLLATE utf8mb4_unicode_ci,
  `Jup_P` longtext COLLATE utf8mb4_unicode_ci,
  `Ven_P` longtext COLLATE utf8mb4_unicode_ci,
  `Sat_P` longtext COLLATE utf8mb4_unicode_ci,
  `Rah_P` longtext COLLATE utf8mb4_unicode_ci,
  `Ket_P` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_graha_yuti`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_graha_yuti` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `Yuti_Name` longtext COLLATE utf8mb4_unicode_ci,
  `Yuti_Phal` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_house_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_house_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `HReading_E` longtext COLLATE utf8mb4_unicode_ci,
  `HReading_H` longtext COLLATE utf8mb4_unicode_ci,
  `HReading_P` longtext COLLATE utf8mb4_unicode_ci,
  `House_Type` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_lal_kitab_digits`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_lal_kitab_digits` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `Field1` longtext COLLATE utf8mb4_unicode_ci,
  `Field2` longtext COLLATE utf8mb4_unicode_ci,
  `Field3` longtext COLLATE utf8mb4_unicode_ci,
  `Field4` longtext COLLATE utf8mb4_unicode_ci,
  `Field5` longtext COLLATE utf8mb4_unicode_ci,
  `Field6` longtext COLLATE utf8mb4_unicode_ci,
  `Field7` longtext COLLATE utf8mb4_unicode_ci,
  `Field8` longtext COLLATE utf8mb4_unicode_ci,
  `Field9` longtext COLLATE utf8mb4_unicode_ci,
  `Field10` longtext COLLATE utf8mb4_unicode_ci,
  `Field11` longtext COLLATE utf8mb4_unicode_ci,
  `Field12` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=111 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_lrd12xhs12_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_lrd12xhs12_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `Lrd_in_Hs` longtext COLLATE utf8mb4_unicode_ci,
  `LrdxHs_E` longtext COLLATE utf8mb4_unicode_ci,
  `LrdxHs_H` longtext COLLATE utf8mb4_unicode_ci,
  `LrdxHs_P` longtext COLLATE utf8mb4_unicode_ci,
  `Granth_Soft_Web_LrdxHs_H` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=145 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_matchdet`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_matchdet` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Field1` longtext COLLATE utf8mb4_unicode_ci,
  `Field2` longtext COLLATE utf8mb4_unicode_ci,
  `Field3` longtext COLLATE utf8mb4_unicode_ci,
  `Field4` longtext COLLATE utf8mb4_unicode_ci,
  `Field5` longtext COLLATE utf8mb4_unicode_ci,
  `Field6` longtext COLLATE utf8mb4_unicode_ci,
  `Field7` longtext COLLATE utf8mb4_unicode_ci,
  `Field8` longtext COLLATE utf8mb4_unicode_ci,
  `Field9` longtext COLLATE utf8mb4_unicode_ci,
  `Field10` longtext COLLATE utf8mb4_unicode_ci,
  `Field11` longtext COLLATE utf8mb4_unicode_ci,
  `Field12` longtext COLLATE utf8mb4_unicode_ci,
  `Field13` longtext COLLATE utf8mb4_unicode_ci,
  `Field14` longtext COLLATE utf8mb4_unicode_ci,
  `Field15` longtext COLLATE utf8mb4_unicode_ci,
  `Field16` longtext COLLATE utf8mb4_unicode_ci,
  `Field17` longtext COLLATE utf8mb4_unicode_ci,
  `Field18` longtext COLLATE utf8mb4_unicode_ci,
  `Field19` longtext COLLATE utf8mb4_unicode_ci,
  `Field20` longtext COLLATE utf8mb4_unicode_ci,
  `Field21` longtext COLLATE utf8mb4_unicode_ci,
  `Field22` longtext COLLATE utf8mb4_unicode_ci,
  `Field23` longtext COLLATE utf8mb4_unicode_ci,
  `Field24` longtext COLLATE utf8mb4_unicode_ci,
  `Field25` longtext COLLATE utf8mb4_unicode_ci,
  `Field26` longtext COLLATE utf8mb4_unicode_ci,
  `Field27` longtext COLLATE utf8mb4_unicode_ci,
  `Field28` longtext COLLATE utf8mb4_unicode_ci,
  `Field29` longtext COLLATE utf8mb4_unicode_ci,
  `Field30` longtext COLLATE utf8mb4_unicode_ci,
  `Field31` longtext COLLATE utf8mb4_unicode_ci,
  `Field32` longtext COLLATE utf8mb4_unicode_ci,
  `Field33` longtext COLLATE utf8mb4_unicode_ci,
  `Field34` longtext COLLATE utf8mb4_unicode_ci,
  `Field35` longtext COLLATE utf8mb4_unicode_ci,
  `Field36` longtext COLLATE utf8mb4_unicode_ci,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_misc`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_misc` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Description` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_H` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_P` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_E` longtext COLLATE utf8mb4_unicode_ci,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=183 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_mutual_aspects_of_plts`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_mutual_aspects_of_plts` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Aspect_Name` longtext COLLATE utf8mb4_unicode_ci,
  `Effect` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=246 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_nak27_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_nak27_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `Nak_Name` longtext COLLATE utf8mb4_unicode_ci,
  `NakPhal_H` longtext COLLATE utf8mb4_unicode_ci,
  `NakPhal_P` longtext COLLATE utf8mb4_unicode_ci,
  `NakPhal_E` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_E_Q1` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_E_Q2` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_E_Q3` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_E_Q4` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_H_Q1` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_H_Q2` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_H_Q3` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_H_Q4` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_P_Q1` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_P_Q2` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_P_Q3` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_P_Q4` longtext COLLATE utf8mb4_unicode_ci,
  `Nak_Name_H` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_nakcharan108_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_nakcharan108_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID1` longtext COLLATE utf8mb4_unicode_ci,
  `Nak_108Charan` longtext COLLATE utf8mb4_unicode_ci,
  `Nak_Nam_Charan` longtext COLLATE utf8mb4_unicode_ci,
  `Nak_108Reading_GrnSofWeb` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_Reading_H` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_Reading_E` longtext COLLATE utf8mb4_unicode_ci,
  `PastBirth_Reading_P` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=110 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_numerology_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_numerology_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Num` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_E` longtext COLLATE utf8mb4_unicode_ci,
  `Spl_Advice` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_P` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_H` longtext COLLATE utf8mb4_unicode_ci,
  `Meaning` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=72 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_palmistry`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_palmistry` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Palm_ID` longtext COLLATE utf8mb4_unicode_ci,
  `Palm_LineSign_Name_E` longtext COLLATE utf8mb4_unicode_ci,
  `Palm_Sign_Name_H` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_E` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_H` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_P` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=102 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_place`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_place` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Place` longtext COLLATE utf8mb4_unicode_ci,
  `Country` longtext COLLATE utf8mb4_unicode_ci,
  `Latt` longtext COLLATE utf8mb4_unicode_ci,
  `Long` longtext COLLATE utf8mb4_unicode_ci,
  `TimeZone` longtext COLLATE utf8mb4_unicode_ci,
  `SMTCorr` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=887 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_plt9xhs12_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_plt9xhs12_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `Plt_Hs_Num` longtext COLLATE utf8mb4_unicode_ci,
  `Birth_Read_E` longtext COLLATE utf8mb4_unicode_ci,
  `Birth_Read_H` longtext COLLATE utf8mb4_unicode_ci,
  `Birth_Read_P` longtext COLLATE utf8mb4_unicode_ci,
  `Gochar_Read_E` longtext COLLATE utf8mb4_unicode_ci,
  `Gochar_Read_H` longtext COLLATE utf8mb4_unicode_ci,
  `Gochar_Read_P` longtext COLLATE utf8mb4_unicode_ci,
  `Lal_Kitab_P` longtext COLLATE utf8mb4_unicode_ci,
  `Lal_Kitab_E` longtext COLLATE utf8mb4_unicode_ci,
  `Granth_Soft_Web_Plt9xHs12` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=110 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_plt9xsign12_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_plt9xsign12_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `Plt_in_Sign` longtext COLLATE utf8mb4_unicode_ci,
  `Granth_Soft_Web_Plt9xSign12` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=109 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_rasprves9plts1950_2100`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_rasprves9plts1950_2100` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `Planet` longtext COLLATE utf8mb4_unicode_ci,
  `Sign` longtext COLLATE utf8mb4_unicode_ci,
  `Entry_Dt` longtext COLLATE utf8mb4_unicode_ci,
  `Entry_Tm` longtext COLLATE utf8mb4_unicode_ci,
  `Exit_Dt` longtext COLLATE utf8mb4_unicode_ci,
  `Exit_Tm` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=32102 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_record_f_natives`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_record_f_natives` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Native_Name` longtext COLLATE utf8mb4_unicode_ci,
  `Gender` longtext COLLATE utf8mb4_unicode_ci,
  `sBirth_Dt` longtext COLLATE utf8mb4_unicode_ci,
  `sBirth_Time` longtext COLLATE utf8mb4_unicode_ci,
  `Birth_Place` longtext COLLATE utf8mb4_unicode_ci,
  `Country` longtext COLLATE utf8mb4_unicode_ci,
  `Addresses` longtext COLLATE utf8mb4_unicode_ci,
  `Remarks` longtext COLLATE utf8mb4_unicode_ci,
  `Asc` longtext COLLATE utf8mb4_unicode_ci,
  `Sun` longtext COLLATE utf8mb4_unicode_ci,
  `Mon` longtext COLLATE utf8mb4_unicode_ci,
  `Mar` longtext COLLATE utf8mb4_unicode_ci,
  `Mer` longtext COLLATE utf8mb4_unicode_ci,
  `Jup` longtext COLLATE utf8mb4_unicode_ci,
  `Ven` longtext COLLATE utf8mb4_unicode_ci,
  `Sat` longtext COLLATE utf8mb4_unicode_ci,
  `Rah` longtext COLLATE utf8mb4_unicode_ci,
  `Ket` longtext COLLATE utf8mb4_unicode_ci,
  `UA4` longtext COLLATE utf8mb4_unicode_ci,
  `UA5` longtext COLLATE utf8mb4_unicode_ci,
  `UA6` longtext COLLATE utf8mb4_unicode_ci,
  `UA7` longtext COLLATE utf8mb4_unicode_ci,
  `UA8` longtext COLLATE utf8mb4_unicode_ci,
  `VM4` longtext COLLATE utf8mb4_unicode_ci,
  `VM5` longtext COLLATE utf8mb4_unicode_ci,
  `VM6` longtext COLLATE utf8mb4_unicode_ci,
  `VM7` longtext COLLATE utf8mb4_unicode_ci,
  `VM8` longtext COLLATE utf8mb4_unicode_ci,
  `LifeSpheres` longtext COLLATE utf8mb4_unicode_ci,
  `Temp_Rec` longtext COLLATE utf8mb4_unicode_ci,
  `SN` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3721 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_sign_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_sign_reading` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `Rsi_Nam` longtext COLLATE utf8mb4_unicode_ci,
  `Rsi_Reading_E` longtext COLLATE utf8mb4_unicode_ci,
  `Rsi_Reading_H` longtext COLLATE utf8mb4_unicode_ci,
  `Rsi_Reading_P` longtext COLLATE utf8mb4_unicode_ci,
  `Rsi_Reading_Granth_Soft_Web_H` longtext COLLATE utf8mb4_unicode_ci,
  `Rsi_Reading_Granth_Soft_Web_E` longtext COLLATE utf8mb4_unicode_ci,
  `Rasiwise_RajyogKarkaMarak_H` longtext COLLATE utf8mb4_unicode_ci,
  `Rasiwise_RajyogKarkaMarak_P` longtext COLLATE utf8mb4_unicode_ci,
  `How2dealSpouse` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse1` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse2` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse3` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse4` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse5` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse6` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse7` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse8` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse9` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse10` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse11` longtext COLLATE utf8mb4_unicode_ci,
  `Spouse12` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_sign_reading_rnd`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_sign_reading_rnd` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `Sign_Name` longtext COLLATE utf8mb4_unicode_ci,
  `Sign_Reading_picked` longtext COLLATE utf8mb4_unicode_ci,
  `Sign_Reading_Bank_Dly` longtext COLLATE utf8mb4_unicode_ci,
  `ChkDate` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=102 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_uday_astha`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_uday_astha` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `JDF_Mar` longtext COLLATE utf8mb4_unicode_ci,
  `Mar_UA` longtext COLLATE utf8mb4_unicode_ci,
  `JDF_Mer` longtext COLLATE utf8mb4_unicode_ci,
  `Mer_UA` longtext COLLATE utf8mb4_unicode_ci,
  `JDF_Jup` longtext COLLATE utf8mb4_unicode_ci,
  `Jup_UA` longtext COLLATE utf8mb4_unicode_ci,
  `JDF_Ven` longtext COLLATE utf8mb4_unicode_ci,
  `Ven_UA` longtext COLLATE utf8mb4_unicode_ci,
  `JDF_Sat` longtext COLLATE utf8mb4_unicode_ci,
  `Sat_UA` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1263 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_upay`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_upay` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  `Upay` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_E` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_H` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_P` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_H1` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_P1` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_vivahmuhurtadates`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_vivahmuhurtadates` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Muh1` longtext COLLATE utf8mb4_unicode_ci,
  `Muh2` longtext COLLATE utf8mb4_unicode_ci,
  `Muh3` longtext COLLATE utf8mb4_unicode_ci,
  `Muh4` longtext COLLATE utf8mb4_unicode_ci,
  `Muh5` longtext COLLATE utf8mb4_unicode_ci,
  `Muh6` longtext COLLATE utf8mb4_unicode_ci,
  `Muh7` longtext COLLATE utf8mb4_unicode_ci,
  `Muh8` longtext COLLATE utf8mb4_unicode_ci,
  `Muh9` longtext COLLATE utf8mb4_unicode_ci,
  `Muh10` longtext COLLATE utf8mb4_unicode_ci,
  `Muh11` longtext COLLATE utf8mb4_unicode_ci,
  `Muh12` longtext COLLATE utf8mb4_unicode_ci,
  `Muh13` longtext COLLATE utf8mb4_unicode_ci,
  `Muh14` longtext COLLATE utf8mb4_unicode_ci,
  `Muh15` longtext COLLATE utf8mb4_unicode_ci,
  `Muh16` longtext COLLATE utf8mb4_unicode_ci,
  `Muh17` longtext COLLATE utf8mb4_unicode_ci,
  `Muh18` longtext COLLATE utf8mb4_unicode_ci,
  `Muh19` longtext COLLATE utf8mb4_unicode_ci,
  `Muh20` longtext COLLATE utf8mb4_unicode_ci,
  `Muh21` longtext COLLATE utf8mb4_unicode_ci,
  `Muh22` longtext COLLATE utf8mb4_unicode_ci,
  `Muh23` longtext COLLATE utf8mb4_unicode_ci,
  `Muh24` longtext COLLATE utf8mb4_unicode_ci,
  `Muh25` longtext COLLATE utf8mb4_unicode_ci,
  `Muh26` longtext COLLATE utf8mb4_unicode_ci,
  `Muh27` longtext COLLATE utf8mb4_unicode_ci,
  `Muh28` longtext COLLATE utf8mb4_unicode_ci,
  `Muh29` longtext COLLATE utf8mb4_unicode_ci,
  `Muh30` longtext COLLATE utf8mb4_unicode_ci,
  `Muh31` longtext COLLATE utf8mb4_unicode_ci,
  `Muh32` longtext COLLATE utf8mb4_unicode_ci,
  `Muh33` longtext COLLATE utf8mb4_unicode_ci,
  `Muh34` longtext COLLATE utf8mb4_unicode_ci,
  `Muh35` longtext COLLATE utf8mb4_unicode_ci,
  `Muh36` longtext COLLATE utf8mb4_unicode_ci,
  `Muh37` longtext COLLATE utf8mb4_unicode_ci,
  `Muh38` longtext COLLATE utf8mb4_unicode_ci,
  `Muh39` longtext COLLATE utf8mb4_unicode_ci,
  `Muh40` longtext COLLATE utf8mb4_unicode_ci,
  `Muh41` longtext COLLATE utf8mb4_unicode_ci,
  `Muh42` longtext COLLATE utf8mb4_unicode_ci,
  `Muh43` longtext COLLATE utf8mb4_unicode_ci,
  `Muh44` longtext COLLATE utf8mb4_unicode_ci,
  `Muh45` longtext COLLATE utf8mb4_unicode_ci,
  `Muh46` longtext COLLATE utf8mb4_unicode_ci,
  `Muh47` longtext COLLATE utf8mb4_unicode_ci,
  `Muh48` longtext COLLATE utf8mb4_unicode_ci,
  `Muh49` longtext COLLATE utf8mb4_unicode_ci,
  `Muh50` longtext COLLATE utf8mb4_unicode_ci,
  `Muh51` longtext COLLATE utf8mb4_unicode_ci,
  `Muh52` longtext COLLATE utf8mb4_unicode_ci,
  `Muh53` longtext COLLATE utf8mb4_unicode_ci,
  `Muh54` longtext COLLATE utf8mb4_unicode_ci,
  `Muh55` longtext COLLATE utf8mb4_unicode_ci,
  `Muh56` longtext COLLATE utf8mb4_unicode_ci,
  `Muh57` longtext COLLATE utf8mb4_unicode_ci,
  `Muh58` longtext COLLATE utf8mb4_unicode_ci,
  `Muh59` longtext COLLATE utf8mb4_unicode_ci,
  `Muh60` longtext COLLATE utf8mb4_unicode_ci,
  `Muh61` longtext COLLATE utf8mb4_unicode_ci,
  `Muh62` longtext COLLATE utf8mb4_unicode_ci,
  `Muh63` longtext COLLATE utf8mb4_unicode_ci,
  `Muh64` longtext COLLATE utf8mb4_unicode_ci,
  `Muh65` longtext COLLATE utf8mb4_unicode_ci,
  `Muh66` longtext COLLATE utf8mb4_unicode_ci,
  `Muh67` longtext COLLATE utf8mb4_unicode_ci,
  `Muh68` longtext COLLATE utf8mb4_unicode_ci,
  `Muh69` longtext COLLATE utf8mb4_unicode_ci,
  `Muh70` longtext COLLATE utf8mb4_unicode_ci,
  `Muh71` longtext COLLATE utf8mb4_unicode_ci,
  `Muh72` longtext COLLATE utf8mb4_unicode_ci,
  `Muh73` longtext COLLATE utf8mb4_unicode_ci,
  `Muh74` longtext COLLATE utf8mb4_unicode_ci,
  `Muh75` longtext COLLATE utf8mb4_unicode_ci,
  `Muh76` longtext COLLATE utf8mb4_unicode_ci,
  `Muh77` longtext COLLATE utf8mb4_unicode_ci,
  `Muh78` longtext COLLATE utf8mb4_unicode_ci,
  `Muh79` longtext COLLATE utf8mb4_unicode_ci,
  `Muh80` longtext COLLATE utf8mb4_unicode_ci,
  `Muh81` longtext COLLATE utf8mb4_unicode_ci,
  `Muh82` longtext COLLATE utf8mb4_unicode_ci,
  `Muh83` longtext COLLATE utf8mb4_unicode_ci,
  `Muh84` longtext COLLATE utf8mb4_unicode_ci,
  `Muh85` longtext COLLATE utf8mb4_unicode_ci,
  `Muh86` longtext COLLATE utf8mb4_unicode_ci,
  `Muh87` longtext COLLATE utf8mb4_unicode_ci,
  `Muh88` longtext COLLATE utf8mb4_unicode_ci,
  `Muh89` longtext COLLATE utf8mb4_unicode_ci,
  `Muh90` longtext COLLATE utf8mb4_unicode_ci,
  `Muh91` longtext COLLATE utf8mb4_unicode_ci,
  `Muh92` longtext COLLATE utf8mb4_unicode_ci,
  `Muh93` longtext COLLATE utf8mb4_unicode_ci,
  `Muh94` longtext COLLATE utf8mb4_unicode_ci,
  `Muh95` longtext COLLATE utf8mb4_unicode_ci,
  `Muh96` longtext COLLATE utf8mb4_unicode_ci,
  `Muh97` longtext COLLATE utf8mb4_unicode_ci,
  `Muh98` longtext COLLATE utf8mb4_unicode_ci,
  `Muh99` longtext COLLATE utf8mb4_unicode_ci,
  `Muh100` longtext COLLATE utf8mb4_unicode_ci,
  `Muh101` longtext COLLATE utf8mb4_unicode_ci,
  `Muh102` longtext COLLATE utf8mb4_unicode_ci,
  `Muh103` longtext COLLATE utf8mb4_unicode_ci,
  `Muh104` longtext COLLATE utf8mb4_unicode_ci,
  `Muh105` longtext COLLATE utf8mb4_unicode_ci,
  `Muh106` longtext COLLATE utf8mb4_unicode_ci,
  `Muh107` longtext COLLATE utf8mb4_unicode_ci,
  `Muh108` longtext COLLATE utf8mb4_unicode_ci,
  `Muh109` longtext COLLATE utf8mb4_unicode_ci,
  `Muh110` longtext COLLATE utf8mb4_unicode_ci,
  `Muh111` longtext COLLATE utf8mb4_unicode_ci,
  `Muh112` longtext COLLATE utf8mb4_unicode_ci,
  `Muh113` longtext COLLATE utf8mb4_unicode_ci,
  `Muh114` longtext COLLATE utf8mb4_unicode_ci,
  `Muh115` longtext COLLATE utf8mb4_unicode_ci,
  `Muh116` longtext COLLATE utf8mb4_unicode_ci,
  `Muh117` longtext COLLATE utf8mb4_unicode_ci,
  `Muh118` longtext COLLATE utf8mb4_unicode_ci,
  `Muh119` longtext COLLATE utf8mb4_unicode_ci,
  `Muh120` longtext COLLATE utf8mb4_unicode_ci,
  `Muh121` longtext COLLATE utf8mb4_unicode_ci,
  `Muh122` longtext COLLATE utf8mb4_unicode_ci,
  `Muh123` longtext COLLATE utf8mb4_unicode_ci,
  `Muh124` longtext COLLATE utf8mb4_unicode_ci,
  `Muh125` longtext COLLATE utf8mb4_unicode_ci,
  `Muh126` longtext COLLATE utf8mb4_unicode_ci,
  `Muh127` longtext COLLATE utf8mb4_unicode_ci,
  `Muh128` longtext COLLATE utf8mb4_unicode_ci,
  `Muh129` longtext COLLATE utf8mb4_unicode_ci,
  `Muh130` longtext COLLATE utf8mb4_unicode_ci,
  `Muh131` longtext COLLATE utf8mb4_unicode_ci,
  `Muh132` longtext COLLATE utf8mb4_unicode_ci,
  `Muh133` longtext COLLATE utf8mb4_unicode_ci,
  `Muh134` longtext COLLATE utf8mb4_unicode_ci,
  `Muh135` longtext COLLATE utf8mb4_unicode_ci,
  `Muh136` longtext COLLATE utf8mb4_unicode_ci,
  `Muh137` longtext COLLATE utf8mb4_unicode_ci,
  `Muh138` longtext COLLATE utf8mb4_unicode_ci,
  `Muh139` longtext COLLATE utf8mb4_unicode_ci,
  `Muh140` longtext COLLATE utf8mb4_unicode_ci,
  `Muh141` longtext COLLATE utf8mb4_unicode_ci,
  `Muh142` longtext COLLATE utf8mb4_unicode_ci,
  `Muh143` longtext COLLATE utf8mb4_unicode_ci,
  `Muh144` longtext COLLATE utf8mb4_unicode_ci,
  `Muh145` longtext COLLATE utf8mb4_unicode_ci,
  `Muh146` longtext COLLATE utf8mb4_unicode_ci,
  `Muh147` longtext COLLATE utf8mb4_unicode_ci,
  `Muh148` longtext COLLATE utf8mb4_unicode_ci,
  `Muh149` longtext COLLATE utf8mb4_unicode_ci,
  `Muh150` longtext COLLATE utf8mb4_unicode_ci,
  `Muh151` longtext COLLATE utf8mb4_unicode_ci,
  `Muh152` longtext COLLATE utf8mb4_unicode_ci,
  `Muh153` longtext COLLATE utf8mb4_unicode_ci,
  `Muh154` longtext COLLATE utf8mb4_unicode_ci,
  `Muh155` longtext COLLATE utf8mb4_unicode_ci,
  `Muh156` longtext COLLATE utf8mb4_unicode_ci,
  `Muh157` longtext COLLATE utf8mb4_unicode_ci,
  `Muh158` longtext COLLATE utf8mb4_unicode_ci,
  `Muh159` longtext COLLATE utf8mb4_unicode_ci,
  `Muh160` longtext COLLATE utf8mb4_unicode_ci,
  `Muh161` longtext COLLATE utf8mb4_unicode_ci,
  `Muh162` longtext COLLATE utf8mb4_unicode_ci,
  `Muh163` longtext COLLATE utf8mb4_unicode_ci,
  `Muh164` longtext COLLATE utf8mb4_unicode_ci,
  `Muh165` longtext COLLATE utf8mb4_unicode_ci,
  `Muh166` longtext COLLATE utf8mb4_unicode_ci,
  `Muh167` longtext COLLATE utf8mb4_unicode_ci,
  `Muh168` longtext COLLATE utf8mb4_unicode_ci,
  `Muh169` longtext COLLATE utf8mb4_unicode_ci,
  `Muh170` longtext COLLATE utf8mb4_unicode_ci,
  `Muh171` longtext COLLATE utf8mb4_unicode_ci,
  `Muh172` longtext COLLATE utf8mb4_unicode_ci,
  `Muh173` longtext COLLATE utf8mb4_unicode_ci,
  `Muh174` longtext COLLATE utf8mb4_unicode_ci,
  `Muh175` longtext COLLATE utf8mb4_unicode_ci,
  `Muh176` longtext COLLATE utf8mb4_unicode_ci,
  `Muh177` longtext COLLATE utf8mb4_unicode_ci,
  `Muh178` longtext COLLATE utf8mb4_unicode_ci,
  `Muh179` longtext COLLATE utf8mb4_unicode_ci,
  `Muh180` longtext COLLATE utf8mb4_unicode_ci,
  `Muh181` longtext COLLATE utf8mb4_unicode_ci,
  `Muh182` longtext COLLATE utf8mb4_unicode_ci,
  `Muh183` longtext COLLATE utf8mb4_unicode_ci,
  `Muh184` longtext COLLATE utf8mb4_unicode_ci,
  `Muh185` longtext COLLATE utf8mb4_unicode_ci,
  `Muh186` longtext COLLATE utf8mb4_unicode_ci,
  `Muh187` longtext COLLATE utf8mb4_unicode_ci,
  `Muh188` longtext COLLATE utf8mb4_unicode_ci,
  `Muh189` longtext COLLATE utf8mb4_unicode_ci,
  `Muh190` longtext COLLATE utf8mb4_unicode_ci,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stg_yogas`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `stg_yogas` (
  `source_row_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Yoga_Name` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_E` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_H` longtext COLLATE utf8mb4_unicode_ci,
  `Reading_P` longtext COLLATE utf8mb4_unicode_ci,
  `ID` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`source_row_id`),
  KEY `ix_source_row_id` (`source_row_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_app_role`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_app_role` (
  `role_id` int NOT NULL AUTO_INCREMENT,
  `role_code` varchar(32) NOT NULL,
  `role_name` varchar(64) NOT NULL,
  PRIMARY KEY (`role_id`),
  UNIQUE KEY `uq_app_role_code` (`role_code`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_app_user`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_app_user` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(64) NOT NULL,
  `password_hash` varchar(256) NOT NULL,
  `password_salt` varchar(128) NOT NULL,
  `display_name` varchar(120) DEFAULT NULL,
  `email` varchar(150) DEFAULT NULL,
  `mobile` varchar(20) DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `uq_app_user_username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_app_user_role`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_app_user_role` (
  `user_id` int NOT NULL,
  `role_id` int NOT NULL,
  PRIMARY KEY (`user_id`,`role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_appointment`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_appointment` (
  `appointment_id` int NOT NULL AUTO_INCREMENT,
  `booking_ref` varchar(32) NOT NULL,
  `user_id` int NOT NULL,
  `service_id` int NOT NULL,
  `appointment_date` date NOT NULL,
  `appointment_time` time NOT NULL,
  `amount` decimal(10,2) NOT NULL,
  `currency` char(3) NOT NULL DEFAULT 'INR',
  `payment_status` varchar(32) NOT NULL,
  `appointment_status` varchar(32) NOT NULL,
  `customer_name` varchar(120) NOT NULL,
  `customer_email` varchar(150) NOT NULL,
  `customer_mobile` varchar(20) DEFAULT NULL,
  `notes` varchar(500) DEFAULT NULL,
  `admin_notes` varchar(500) DEFAULT NULL,
  `approved_at` datetime DEFAULT NULL,
  `approved_by` int DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`appointment_id`),
  UNIQUE KEY `uq_booking_ref` (`booking_ref`),
  KEY `ix_appt_user` (`user_id`),
  KEY `ix_appt_date_time` (`appointment_date`,`appointment_time`),
  KEY `ix_appt_status` (`appointment_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_article`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_article` (
  `article_id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(200) NOT NULL,
  `slug` varchar(220) NOT NULL,
  `summary` varchar(500) DEFAULT NULL,
  `body_html` mediumtext NOT NULL,
  `image_url` varchar(255) DEFAULT NULL,
  `is_published` tinyint(1) NOT NULL DEFAULT '0',
  `published_at` datetime DEFAULT NULL,
  `created_by` int DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`article_id`),
  UNIQUE KEY `uq_article_slug` (`slug`),
  KEY `ix_article_published` (`is_published`,`published_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_aspect_effect`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_aspect_effect` (
  `aspect_id` int unsigned NOT NULL AUTO_INCREMENT,
  `aspect_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `effect_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`aspect_id`),
  UNIQUE KEY `uq_aspect_name` (`aspect_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_brg_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_brg_reading` (
  `brg_id` int unsigned NOT NULL,
  `brg_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  `source_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`brg_id`,`language_id`),
  KEY `fk_brg_language` (`language_id`),
  CONSTRAINT `fk_brg_language` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_combustion_period`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_combustion_period` (
  `combustion_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `planet_id` tinyint unsigned NOT NULL,
  `astha_at` datetime DEFAULT NULL,
  `astha_time_legacy` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `udaya_status` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`combustion_id`),
  KEY `ix_combustion_planet_date` (`planet_id`,`astha_at`),
  CONSTRAINT `tbl_combustion_period_ibfk_1` FOREIGN KEY (`planet_id`) REFERENCES `tbl_planet` (`planet_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_content_type`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_content_type` (
  `content_type_id` smallint unsigned NOT NULL AUTO_INCREMENT,
  `content_code` varchar(80) COLLATE utf8mb4_unicode_ci NOT NULL,
  `content_name` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`content_type_id`),
  UNIQUE KEY `uq_content_code` (`content_code`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_country`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_country` (
  `country_id` smallint unsigned NOT NULL AUTO_INCREMENT,
  `country_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`country_id`),
  UNIQUE KEY `uq_tbl_country_name` (`country_name`)
) ENGINE=InnoDB AUTO_INCREMENT=83 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_dasa_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_dasa_reading` (
  `dasa_id` int unsigned NOT NULL AUTO_INCREMENT,
  `dasa_lord_planet_id` tinyint unsigned DEFAULT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `lord_name_legacy` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `result_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`dasa_id`,`language_id`),
  UNIQUE KEY `uq_dasa_lord_language` (`dasa_lord_planet_id`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_dasa_reading_ibfk_1` FOREIGN KEY (`dasa_lord_planet_id`) REFERENCES `tbl_planet` (`planet_id`),
  CONSTRAINT `tbl_dasa_reading_ibfk_2` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_gochar_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_gochar_reading` (
  `house_id` tinyint unsigned NOT NULL,
  `planet_id` tinyint unsigned NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`house_id`,`planet_id`,`language_id`),
  KEY `planet_id` (`planet_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_gochar_reading_ibfk_1` FOREIGN KEY (`house_id`) REFERENCES `tbl_house` (`house_id`),
  CONSTRAINT `tbl_gochar_reading_ibfk_2` FOREIGN KEY (`planet_id`) REFERENCES `tbl_planet` (`planet_id`),
  CONSTRAINT `tbl_gochar_reading_ibfk_3` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_graha_yuti_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_graha_yuti_reading` (
  `yuti_id` int unsigned NOT NULL AUTO_INCREMENT,
  `yuti_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`yuti_id`,`language_id`),
  UNIQUE KEY `uq_yuti_lang` (`yuti_name`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_graha_yuti_reading_ibfk_1` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_house`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_house` (
  `house_id` tinyint unsigned NOT NULL AUTO_INCREMENT,
  `house_no` tinyint unsigned NOT NULL,
  `house_type` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`house_id`),
  UNIQUE KEY `uq_house_no` (`house_no`),
  CONSTRAINT `ck_house_no` CHECK ((`house_no` between 1 and 12))
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_house_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_house_reading` (
  `house_id` tinyint unsigned NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`house_id`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_house_reading_ibfk_1` FOREIGN KEY (`house_id`) REFERENCES `tbl_house` (`house_id`),
  CONSTRAINT `tbl_house_reading_ibfk_2` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_language`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_language` (
  `language_id` tinyint unsigned NOT NULL AUTO_INCREMENT,
  `language_code` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `language_name` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`language_id`),
  UNIQUE KEY `uq_language_code` (`language_code`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_lord_house_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_lord_house_reading` (
  `lord_house_id` int unsigned NOT NULL AUTO_INCREMENT,
  `asc_sign_id` tinyint unsigned NOT NULL,
  `lord_planet_id` tinyint unsigned NOT NULL,
  `house_id` tinyint unsigned NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`lord_house_id`,`language_id`),
  UNIQUE KEY `uq_lord_house_rule` (`asc_sign_id`,`lord_planet_id`,`house_id`,`language_id`),
  KEY `lord_planet_id` (`lord_planet_id`),
  KEY `house_id` (`house_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_lord_house_reading_ibfk_1` FOREIGN KEY (`asc_sign_id`) REFERENCES `tbl_sign` (`sign_id`),
  CONSTRAINT `tbl_lord_house_reading_ibfk_2` FOREIGN KEY (`lord_planet_id`) REFERENCES `tbl_planet` (`planet_id`),
  CONSTRAINT `tbl_lord_house_reading_ibfk_3` FOREIGN KEY (`house_id`) REFERENCES `tbl_house` (`house_id`),
  CONSTRAINT `tbl_lord_house_reading_ibfk_4` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_match_det`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_match_det` (
  `id` smallint unsigned NOT NULL,
  `field_01` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_02` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_03` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_04` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_05` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_06` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_07` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_08` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_09` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_10` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_11` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_12` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_13` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_14` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_15` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_16` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_17` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_18` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_19` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_20` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_21` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_22` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_23` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_24` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_25` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_26` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_27` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_28` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_29` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_30` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_31` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_32` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_33` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_34` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_35` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `field_36` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_match_detail`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_match_detail` (
  `match_detail_id` smallint unsigned NOT NULL,
  `field_no` tinyint unsigned NOT NULL,
  `field_value` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`match_detail_id`,`field_no`),
  CONSTRAINT `ck_match_field_no` CHECK ((`field_no` between 1 and 36))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_misc_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_misc_reading` (
  `misc_id` int unsigned NOT NULL AUTO_INCREMENT,
  `description` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`misc_id`,`language_id`),
  UNIQUE KEY `uq_misc_desc_lang` (`description`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_misc_reading_ibfk_1` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_muhurta_date`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_muhurta_date` (
  `muhurta_date_id` int unsigned NOT NULL AUTO_INCREMENT,
  `muhurta_date` date NOT NULL,
  `legacy_row_id` int DEFAULT NULL,
  PRIMARY KEY (`muhurta_date_id`),
  UNIQUE KEY `uq_muhurta_date` (`muhurta_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_muhurta_lagna`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_muhurta_lagna` (
  `muhurta_date_id` int unsigned NOT NULL,
  `slot_no` tinyint unsigned NOT NULL,
  `lagna_sign_id` tinyint unsigned DEFAULT NULL,
  `remarks` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`muhurta_date_id`,`slot_no`),
  KEY `lagna_sign_id` (`lagna_sign_id`),
  CONSTRAINT `tbl_muhurta_lagna_ibfk_1` FOREIGN KEY (`muhurta_date_id`) REFERENCES `tbl_muhurta_date` (`muhurta_date_id`) ON DELETE CASCADE,
  CONSTRAINT `tbl_muhurta_lagna_ibfk_2` FOREIGN KEY (`lagna_sign_id`) REFERENCES `tbl_sign` (`sign_id`),
  CONSTRAINT `ck_muhurta_slot` CHECK ((`slot_no` between 1 and 190))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_mutual_aspect_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_mutual_aspect_reading` (
  `aspect_id` int unsigned NOT NULL,
  `planet_a_id` tinyint unsigned NOT NULL,
  `planet_b_id` tinyint unsigned NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `effect_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`aspect_id`,`planet_a_id`,`planet_b_id`,`language_id`),
  KEY `planet_a_id` (`planet_a_id`),
  KEY `planet_b_id` (`planet_b_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_mutual_aspect_reading_ibfk_1` FOREIGN KEY (`aspect_id`) REFERENCES `tbl_aspect_effect` (`aspect_id`),
  CONSTRAINT `tbl_mutual_aspect_reading_ibfk_2` FOREIGN KEY (`planet_a_id`) REFERENCES `tbl_planet` (`planet_id`),
  CONSTRAINT `tbl_mutual_aspect_reading_ibfk_3` FOREIGN KEY (`planet_b_id`) REFERENCES `tbl_planet` (`planet_id`),
  CONSTRAINT `tbl_mutual_aspect_reading_ibfk_4` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`),
  CONSTRAINT `ck_mutual_planets` CHECK ((`planet_a_id` < `planet_b_id`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_nakshatra`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_nakshatra` (
  `nakshatra_id` tinyint unsigned NOT NULL AUTO_INCREMENT,
  `nakshatra_no` tinyint unsigned NOT NULL,
  `nakshatra_name_en` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nakshatra_name_local` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`nakshatra_id`),
  UNIQUE KEY `uq_nakshatra_no` (`nakshatra_no`),
  CONSTRAINT `ck_nakshatra_no` CHECK ((`nakshatra_no` between 1 and 27))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_nakshatra_charan_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_nakshatra_charan_reading` (
  `charan_id` int unsigned NOT NULL AUTO_INCREMENT,
  `nakshatra_id` tinyint unsigned NOT NULL,
  `charan_no` tinyint unsigned NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `charan_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  `past_birth_reading` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`charan_id`,`language_id`),
  UNIQUE KEY `uq_nak_charan` (`nakshatra_id`,`charan_no`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_nakshatra_charan_reading_ibfk_1` FOREIGN KEY (`nakshatra_id`) REFERENCES `tbl_nakshatra` (`nakshatra_id`),
  CONSTRAINT `tbl_nakshatra_charan_reading_ibfk_2` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`),
  CONSTRAINT `ck_charan_no` CHECK ((`charan_no` between 1 and 4))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_nakshatra_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_nakshatra_reading` (
  `nakshatra_id` tinyint unsigned NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  `past_birth_q1` longtext COLLATE utf8mb4_unicode_ci,
  `past_birth_q2` longtext COLLATE utf8mb4_unicode_ci,
  `past_birth_q3` longtext COLLATE utf8mb4_unicode_ci,
  `past_birth_q4` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`nakshatra_id`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_nakshatra_reading_ibfk_1` FOREIGN KEY (`nakshatra_id`) REFERENCES `tbl_nakshatra` (`nakshatra_id`),
  CONSTRAINT `tbl_nakshatra_reading_ibfk_2` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_native`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_native` (
  `native_id` int unsigned NOT NULL AUTO_INCREMENT,
  `serial_no` int DEFAULT NULL,
  `native_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `gender_code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `gender` char(1) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `birth_at` datetime DEFAULT NULL,
  `birth_date` date DEFAULT NULL,
  `birth_date_legacy` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `birth_date_text` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `birth_time_legacy` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `birth_time` time DEFAULT NULL,
  `birth_time_text` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `place_id` int unsigned DEFAULT NULL,
  `birth_place` varchar(120) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `country_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `latitude` decimal(9,6) DEFAULT NULL,
  `longitude` decimal(9,6) DEFAULT NULL,
  `time_zone` decimal(6,2) DEFAULT NULL,
  `nick_name` varchar(80) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `father_name` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `address_text` longtext COLLATE utf8mb4_unicode_ci,
  `remarks` longtext COLLATE utf8mb4_unicode_ci,
  `life_spheres` longtext COLLATE utf8mb4_unicode_ci,
  `is_temporary` tinyint(1) NOT NULL DEFAULT '0',
  `temp_record` tinyint(1) NOT NULL DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`native_id`),
  KEY `ix_native_name` (`native_name`),
  KEY `place_id` (`place_id`),
  KEY `idx_native_birth_place` (`birth_place`),
  KEY `idx_native_serial_no` (`serial_no`),
  KEY `idx_native_birth_date_col` (`birth_date`),
  CONSTRAINT `tbl_native_ibfk_1` FOREIGN KEY (`place_id`) REFERENCES `tbl_place` (`place_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3734 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_native_planet_position`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_native_planet_position` (
  `native_id` int unsigned NOT NULL,
  `planet_id` tinyint unsigned NOT NULL,
  `position_type` enum('SIGN','HOUSE','OTHER') COLLATE utf8mb4_unicode_ci NOT NULL,
  `position_value` smallint NOT NULL,
  PRIMARY KEY (`native_id`,`planet_id`,`position_type`),
  KEY `planet_id` (`planet_id`),
  CONSTRAINT `tbl_native_planet_position_ibfk_1` FOREIGN KEY (`native_id`) REFERENCES `tbl_native` (`native_id`) ON DELETE CASCADE,
  CONSTRAINT `tbl_native_planet_position_ibfk_2` FOREIGN KEY (`planet_id`) REFERENCES `tbl_planet` (`planet_id`),
  CONSTRAINT `ck_position_value` CHECK ((`position_value` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_native_upay`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_native_upay` (
  `native_id` int unsigned NOT NULL,
  `upay_id` int unsigned NOT NULL,
  `assigned_at` datetime DEFAULT NULL,
  PRIMARY KEY (`native_id`,`upay_id`),
  KEY `upay_id` (`upay_id`),
  CONSTRAINT `tbl_native_upay_ibfk_1` FOREIGN KEY (`native_id`) REFERENCES `tbl_native` (`native_id`) ON DELETE CASCADE,
  CONSTRAINT `tbl_native_upay_ibfk_2` FOREIGN KEY (`upay_id`) REFERENCES `tbl_upay_reading` (`upay_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_numerology_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_numerology_reading` (
  `number_value` smallint NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `meaning` longtext COLLATE utf8mb4_unicode_ci,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  `special_advice` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`number_value`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_numerology_reading_ibfk_1` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_palmistry_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_palmistry_reading` (
  `palm_id` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `line_sign_name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`palm_id`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_palmistry_reading_ibfk_1` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_payment`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_payment` (
  `payment_id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `appointment_id` int DEFAULT NULL,
  `report_order_id` int DEFAULT NULL,
  `amount` decimal(10,2) NOT NULL,
  `currency` char(3) NOT NULL DEFAULT 'INR',
  `gateway` varchar(32) NOT NULL DEFAULT 'ManualUpi',
  `order_id` varchar(64) NOT NULL,
  `transaction_ref` varchar(128) DEFAULT NULL,
  `payment_status` varchar(32) NOT NULL,
  `payment_method` varchar(64) DEFAULT NULL,
  `gateway_response_ref` varchar(255) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`payment_id`),
  UNIQUE KEY `uq_payment_order` (`order_id`),
  KEY `ix_pay_user` (`user_id`),
  KEY `ix_pay_appt` (`appointment_id`),
  KEY `ix_pay_txn` (`transaction_ref`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_place`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_place` (
  `place_id` int unsigned NOT NULL AUTO_INCREMENT,
  `place_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `country_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `latitude` decimal(9,6) DEFAULT NULL,
  `longitude` decimal(9,6) DEFAULT NULL,
  `time_zone` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `summer_time_correction` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`place_id`),
  UNIQUE KEY `uq_place_country` (`place_name`,`country_name`),
  CONSTRAINT `ck_latitude` CHECK (((`latitude` is null) or (`latitude` between -(90) and 90))),
  CONSTRAINT `ck_longitude` CHECK (((`longitude` is null) or (`longitude` between -(180) and 180)))
) ENGINE=InnoDB AUTO_INCREMENT=872 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_planet`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_planet` (
  `planet_id` tinyint unsigned NOT NULL AUTO_INCREMENT,
  `planet_code` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `planet_name` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `is_node` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`planet_id`),
  UNIQUE KEY `uq_planet_code` (`planet_code`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_planet_house_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_planet_house_reading` (
  `planet_house_id` int unsigned NOT NULL AUTO_INCREMENT,
  `planet_id` tinyint unsigned NOT NULL,
  `house_id` tinyint unsigned NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `birth_reading` longtext COLLATE utf8mb4_unicode_ci,
  `gochar_reading` longtext COLLATE utf8mb4_unicode_ci,
  `lal_kitab_reading` longtext COLLATE utf8mb4_unicode_ci,
  `source_reading` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`planet_house_id`,`language_id`),
  UNIQUE KEY `uq_planet_house_language` (`planet_id`,`house_id`,`language_id`),
  KEY `house_id` (`house_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_planet_house_reading_ibfk_1` FOREIGN KEY (`planet_id`) REFERENCES `tbl_planet` (`planet_id`),
  CONSTRAINT `tbl_planet_house_reading_ibfk_2` FOREIGN KEY (`house_id`) REFERENCES `tbl_house` (`house_id`),
  CONSTRAINT `tbl_planet_house_reading_ibfk_3` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_planet_sign_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_planet_sign_reading` (
  `planet_sign_id` int unsigned NOT NULL AUTO_INCREMENT,
  `planet_id` tinyint unsigned NOT NULL,
  `sign_id` tinyint unsigned NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `source_reading` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`planet_sign_id`,`language_id`),
  UNIQUE KEY `uq_planet_sign_language` (`planet_id`,`sign_id`,`language_id`),
  KEY `sign_id` (`sign_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_planet_sign_reading_ibfk_1` FOREIGN KEY (`planet_id`) REFERENCES `tbl_planet` (`planet_id`),
  CONSTRAINT `tbl_planet_sign_reading_ibfk_2` FOREIGN KEY (`sign_id`) REFERENCES `tbl_sign` (`sign_id`),
  CONSTRAINT `tbl_planet_sign_reading_ibfk_3` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_planet_transit`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_planet_transit` (
  `transit_id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `planet_id` tinyint unsigned NOT NULL,
  `sign_id` tinyint unsigned NOT NULL,
  `entry_at` datetime DEFAULT NULL,
  `exit_at` datetime DEFAULT NULL,
  `entry_time_legacy` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `exit_time_legacy` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`transit_id`),
  KEY `ix_transit_lookup` (`planet_id`,`sign_id`,`entry_at`),
  KEY `sign_id` (`sign_id`),
  CONSTRAINT `tbl_planet_transit_ibfk_1` FOREIGN KEY (`planet_id`) REFERENCES `tbl_planet` (`planet_id`),
  CONSTRAINT `tbl_planet_transit_ibfk_2` FOREIGN KEY (`sign_id`) REFERENCES `tbl_sign` (`sign_id`),
  CONSTRAINT `ck_transit_dates` CHECK (((`exit_at` is null) or (`entry_at` is null) or (`exit_at` >= `entry_at`)))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_public_report_order`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_public_report_order` (
  `report_order_id` int NOT NULL AUTO_INCREMENT,
  `order_ref` varchar(32) NOT NULL,
  `user_id` int DEFAULT NULL,
  `report_code` varchar(64) NOT NULL,
  `amount` decimal(10,2) NOT NULL DEFAULT '0.00',
  `currency` char(3) NOT NULL DEFAULT 'INR',
  `payment_status` varchar(32) NOT NULL,
  `birth_payload_json` mediumtext,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`report_order_id`),
  UNIQUE KEY `uq_report_order_ref` (`order_ref`),
  KEY `ix_pro_user` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_report_definition`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_report_definition` (
  `report_id` int NOT NULL AUTO_INCREMENT,
  `report_code` varchar(64) NOT NULL,
  `report_name` varchar(120) NOT NULL,
  `public_allowed` tinyint(1) NOT NULL DEFAULT '0',
  `admin_allowed` tinyint(1) NOT NULL DEFAULT '1',
  `is_enabled` tinyint(1) NOT NULL DEFAULT '1',
  `usage_limit_daily` int DEFAULT NULL,
  `notes` varchar(255) DEFAULT NULL,
  `price_inr` decimal(10,2) NOT NULL DEFAULT '0.00',
  `short_description` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`report_id`),
  UNIQUE KEY `uq_report_code` (`report_code`)
) ENGINE=InnoDB AUTO_INCREMENT=326 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_service`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_service` (
  `service_id` int NOT NULL AUTO_INCREMENT,
  `service_code` varchar(64) NOT NULL,
  `service_name` varchar(120) NOT NULL,
  `description` varchar(500) DEFAULT NULL,
  `duration_minutes` int NOT NULL DEFAULT '30',
  `price_inr` decimal(10,2) NOT NULL DEFAULT '0.00',
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `sort_order` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`service_id`),
  UNIQUE KEY `uq_service_code` (`service_code`)
) ENGINE=InnoDB AUTO_INCREMENT=103 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_sign`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_sign` (
  `sign_id` tinyint unsigned NOT NULL AUTO_INCREMENT,
  `sign_no` tinyint unsigned NOT NULL,
  `sign_code` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `sign_name` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`sign_id`),
  UNIQUE KEY `uq_sign_no` (`sign_no`),
  UNIQUE KEY `uq_sign_code` (`sign_code`),
  CONSTRAINT `ck_sign_no` CHECK ((`sign_no` between 1 and 12))
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_sign_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_sign_reading` (
  `sign_id` tinyint unsigned NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  `source_reading` longtext COLLATE utf8mb4_unicode_ci,
  `rajyog_reading` longtext COLLATE utf8mb4_unicode_ci,
  `spouse_strategy` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`sign_id`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_sign_reading_ibfk_1` FOREIGN KEY (`sign_id`) REFERENCES `tbl_sign` (`sign_id`),
  CONSTRAINT `tbl_sign_reading_ibfk_2` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_sign_spouse_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_sign_spouse_reading` (
  `sign_id` tinyint unsigned NOT NULL,
  `spouse_no` tinyint unsigned NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`sign_id`,`spouse_no`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_sign_spouse_reading_ibfk_1` FOREIGN KEY (`sign_id`) REFERENCES `tbl_sign` (`sign_id`),
  CONSTRAINT `tbl_sign_spouse_reading_ibfk_2` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`),
  CONSTRAINT `ck_spouse_no` CHECK ((`spouse_no` between 1 and 12))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_udaya_astha_rule`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_udaya_astha_rule` (
  `planet_id` tinyint unsigned NOT NULL,
  `rule_value` decimal(10,6) DEFAULT NULL,
  `status_code` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`planet_id`),
  CONSTRAINT `tbl_udaya_astha_rule_ibfk_1` FOREIGN KEY (`planet_id`) REFERENCES `tbl_planet` (`planet_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_upay_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_upay_reading` (
  `upay_id` int unsigned NOT NULL AUTO_INCREMENT,
  `upay_code` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  `alternate_reading` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`upay_id`,`language_id`),
  UNIQUE KEY `uq_upay_lang` (`upay_code`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_upay_reading_ibfk_1` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tbl_yoga_reading`
--

/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE IF NOT EXISTS `tbl_yoga_reading` (
  `yoga_id` int unsigned NOT NULL AUTO_INCREMENT,
  `yoga_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `language_id` tinyint unsigned NOT NULL,
  `reading_text` longtext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`yoga_id`,`language_id`),
  UNIQUE KEY `uq_yoga_lang` (`yoga_name`,`language_id`),
  KEY `language_id` (`language_id`),
  CONSTRAINT `tbl_yoga_reading_ibfk_1` FOREIGN KEY (`language_id`) REFERENCES `tbl_language` (`language_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'indu_astro'
--

--
-- Dumping routines for database 'indu_astro'
--
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetAllCountries` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE PROCEDURE `sp_GetAllCountries`()
BEGIN
    SELECT country_id AS CountryID,
           country_name AS CountryName
    FROM tbl_country
    ORDER BY country_name ASC;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetAllZodiacs` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE PROCEDURE `sp_GetAllZodiacs`()
BEGIN
    SELECT * FROM Zodiac ORDER BY ZodiacId ASC;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetPlaceById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
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
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetZodiacById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE PROCEDURE `sp_GetZodiacById`(IN p_ZodiacId INT)
BEGIN
    SELECT * FROM Zodiac
    WHERE ZodiacId = p_ZodiacId
    LIMIT 1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetZodiacByName` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE PROCEDURE `sp_GetZodiacByName`(IN p_Name VARCHAR(50))
BEGIN
    SELECT * FROM Zodiac
    WHERE LOWER(Name) = LOWER(p_Name)
    LIMIT 1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_InsertPlace` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
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
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_SearchPlaces` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
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
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-13 16:50:38
