ALTER TABLE naivart.buildings
ADD `Status` longtext CHARACTER SET utf8mb4 NULL AFTER `FinishedAt`;

UPDATE naivart.buildings 
SET `Status` = 'done'; 