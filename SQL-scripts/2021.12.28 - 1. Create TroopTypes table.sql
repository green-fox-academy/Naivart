      USE `naivart`;
      CREATE TABLE `TroopTypes` (
      `Id` bigint NOT NULL AUTO_INCREMENT,
      `Level` int NOT NULL,
      `Type` longtext CHARACTER SET utf8mb4 NULL,
      `Hp` int NOT NULL,
      `Attack` int NOT NULL,
      `Defense` int NOT NULL,
      `GoldCost` int NOT NULL,
      `DailyFoodCost` int NOT NULL,
      CONSTRAINT `Pk_TroopTypes` PRIMARY KEY(`Id`)
      ) CHARACTER SET utf8mb4;
      
      
      
