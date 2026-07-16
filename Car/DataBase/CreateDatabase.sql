CREATE TABLE IF NOT EXISTS Cars (
    Id          BIGINT GENERATED ALWAYS AS IDENTITY,
    "Year"      INTEGER          NOT NULL,
    Make        VARCHAR(50)      NOT NULL,
    Model       VARCHAR(50)      NOT NULL,
    Odometer    BIGINT           NOT NULL,
    Price       NUMERIC(12,2)    NOT NULL,
    Status      VARCHAR(50)      NOT NULL,
    CONSTRAINT  PK_Cars          PRIMARY KEY (Id),
    CONSTRAINT  CK_Cars_Year     CHECK ("Year" BETWEEN 1901 AND 2026),
    CONSTRAINT  CK_Cars_Price    CHECK (Price >= 0),
    CONSTRAINT  CK_Cars_Status   CHECK (Status IN ('Available', 'Sold'))
);