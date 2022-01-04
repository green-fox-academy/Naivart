		USE `naivart`;
        DROP TABLE IF EXISTS naivart.Troops;
      CREATE TABLE `Troops` (
          `Id` bigint NOT NULL AUTO_INCREMENT,
          `Started_at` bigint NOT NULL,
          `Finished_at` bigint NOT NULL,
          `KingdomId` bigint NOT NULL,
          `TroopTypeId` bigint NOT NULL,
          CONSTRAINT `PK_Troops` PRIMARY KEY (`Id`),
          CONSTRAINT `FK_Troops_Kingdoms_KingdomId` FOREIGN KEY (`KingdomId`) REFERENCES `Kingdoms` (`Id`) ON DELETE CASCADE,
          CONSTRAINT `FK_Troops_TroopTypes_TroopTypeId` FOREIGN KEY (`TroopTypeId`) REFERENCES `TroopTypes` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET utf8mb4;
