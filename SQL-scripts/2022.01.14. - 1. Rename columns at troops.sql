ALTER TABLE naivart.troops
CHANGE COLUMN Started_at StartedAt bigint NOT NULL;
ALTER TABLE naivart.troops
CHANGE COLUMN Finished_at FinishedAt bigint NOT NULL;