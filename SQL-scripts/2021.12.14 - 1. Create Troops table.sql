		USE `naivart`;      
		CREATE TABLE `Troops` (
          `Id` bigint NOT NULL AUTO_INCREMENT,
          `Level` int NOT NULL,
          `Hp` int NOT NULL,
          `Attack` int NOT NULL,
          `Defense` int NOT NULL,
          `Started_at` bigint NOT NULL,
          `Finished_at` bigint NOT NULL,
          `KingdomId` bigint NOT NULL,
          CONSTRAINT `PK_Troops` PRIMARY KEY (`Id`),
          CONSTRAINT `FK_Troops_Kingdoms_KingdomId` FOREIGN KEY (`KingdomId`) REFERENCES `Kingdoms` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET utf8mb4;
