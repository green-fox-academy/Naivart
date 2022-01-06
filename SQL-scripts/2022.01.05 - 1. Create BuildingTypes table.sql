      USE `naivart`;
      CREATE TABLE `BuildingTypes` (
      `Id` bigint NOT NULL AUTO_INCREMENT,
      `Type` longtext CHARACTER SET utf8mb4 NULL,
      `Level` int NOT NULL, 
      `Hp` int NOT NULL,
      `GoldCost` int NOT NULL,
      `RequiredTownhallLevel` int NOT NULL,
      CONSTRAINT `Pk_BuildingTypes` PRIMARY KEY(`Id`)
      ) CHARACTER SET utf8mb4;
      
      
      
