ALTER TABLE naivart.troops
ADD `Status` longtext CHARACTER SET utf8mb4 NULL AFTER `Finished_at`;

UPDATE naivart.troops 
SET `Status` = 'town'; 