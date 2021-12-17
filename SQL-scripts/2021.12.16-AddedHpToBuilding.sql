USE naivart;
alter table buildings
add hp int NOT NULL;
UPDATE naivart.buildings
SET hp = '50';
