ALTER TABLE naivart.buildings 
CHANGE COLUMN `hp` `hp` INT NOT NULL AFTER `Level`,
ADD `BuildingTypeId` bigint NOT NULL AFTER `KingdomId`;