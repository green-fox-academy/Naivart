Use naivart;
CREATE TABLE `Battles` (
		  `Id` bigint NOT NULL AUTO_INCREMENT,
          `AttackerId` bigint NOT NULL,
          `DefenderId` bigint NOT NULL,
          `BattleType` longtext CHARACTER SET utf8mb4 NULL,
		  `Result` longtext CHARACTER SET utf8mb4 NULL,
          `StartedAt` bigint NOT NULL,
          `FinishedAt` bigint NOT NULL,
          `GoldStolen` int NOT NULL,
          `FoodStolen` int NOT NULL,
          CONSTRAINT `PK_Battles` PRIMARY KEY (`Id`)
      ) CHARACTER SET utf8mb4;
      
CREATE TABLE `AttackerTroops` (
          `Id` bigint NOT NULL AUTO_INCREMENT,
          `Type` longtext CHARACTER SET utf8mb4 NULL,
          `Quantity` int NOT NULL,
          `Level` int NOT NULL,
          `BattleId` bigint NOT NULL,
          CONSTRAINT `PK_AttackerTroops` PRIMARY KEY (`Id`),
          CONSTRAINT `FK_AttackerTroops_Battles_BattleId` FOREIGN KEY (`BattleId`) REFERENCES `Battles` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET utf8mb4;

      CREATE TABLE `TroopsLost` (
          `Id` bigint NOT NULL AUTO_INCREMENT,
          `IsAttacker` tinyint(1) NOT NULL,
          `Type` longtext CHARACTER SET utf8mb4 NULL,
          `Quantity` int NOT NULL,
          `BattleId` bigint NOT NULL,
          CONSTRAINT `PK_TroopsLost` PRIMARY KEY (`Id`),
          CONSTRAINT `FK_TroopsLost_Battles_BattleId` FOREIGN KEY (`BattleId`) REFERENCES `Battles` (`Id`) ON DELETE CASCADE
      ) CHARACTER SET utf8mb4;