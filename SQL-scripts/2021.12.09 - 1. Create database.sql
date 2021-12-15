      CREATE DATABASE `naivart`;
	  USE `naivart`;
      CREATE TABLE `Locations` (
          `Id` bigint NOT NULL AUTO_INCREMENT,
          `CoordinateX` int NOT NULL,
          `CoordinateY` int NOT NULL,
          CONSTRAINT `PK_Locations` PRIMARY KEY (`Id`)
      ) CHARACTER SET utf8mb4;

      CREATE TABLE `Kingdoms` (
          `Id` bigint NOT NULL AUTO_INCREMENT,
          `Name` longtext CHARACTER SET utf8mb4 NULL,
          `Population` int NOT NULL,
          `LocationId` bigint NULL,
          CONSTRAINT `PK_Kingdoms` PRIMARY KEY (`Id`),
          CONSTRAINT `FK_Kingdoms_Locations_LocationId` FOREIGN KEY (`LocationId`) REFERENCES `Locations` (`Id`) ON DELETE RESTRICT
      ) CHARACTER SET utf8mb4;

      CREATE TABLE `Players` (
          `Id` bigint NOT NULL AUTO_INCREMENT,
          `Username` longtext CHARACTER SET utf8mb4 NULL,
          `Password` longtext CHARACTER SET utf8mb4 NULL,
          `KingdomId` bigint NOT NULL,
          CONSTRAINT `PK_Players` PRIMARY KEY (`Id`),
          CONSTRAINT `FK_Players_Kingdoms_KingdomId` FOREIGN KEY (`KingdomId`) REFERENCES `Kingdoms` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET utf8mb4;
      
