      use naivart;
      CREATE TABLE `Resources` (
          `Id` bigint NOT NULL AUTO_INCREMENT,
          `Type` longtext CHARACTER SET utf8mb4 NULL,
          `Amount` int NOT NULL,
          `Generation` int NOT NULL,
          `UpdatedAt` bigint NOT NULL,
          `KingdomId` bigint NOT NULL,
          CONSTRAINT `PK_Resources` PRIMARY KEY (`Id`),
          CONSTRAINT `FK_Resources_Kingdoms_KingdomId` FOREIGN KEY (`KingdomId`) REFERENCES `Kingdoms` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET utf8mb4;