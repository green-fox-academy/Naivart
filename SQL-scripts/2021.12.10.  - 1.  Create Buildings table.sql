	  USE `naivart`;
      CREATE TABLE `Buildings` (
          `Id` bigint NOT NULL AUTO_INCREMENT,
          `Type` longtext CHARACTER SET utf8mb4 NULL,
          `Level` int NOT NULL,
          `StartedAt` bigint NOT NULL,
          `FinishedAt` bigint NOT NULL,
          `KingdomId` bigint NOT NULL,
          CONSTRAINT `PK_Buildings` PRIMARY KEY (`Id`),
          CONSTRAINT `FK_Buildings_Kingdoms_KingdomId` FOREIGN KEY (`KingdomId`) REFERENCES `Kingdoms` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET utf8mb4;