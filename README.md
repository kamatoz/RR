# RR
RussianRoboticsApp

Adding table PriceItems in PostgreSQL


CREATE TABLE IF NOT EXISTS public."PriceItems"
(
    "ID" integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "Vendor" character varying(64) COLLATE pg_catalog."default",
    "Number" character varying(64) COLLATE pg_catalog."default",
    "SearchVendor" character varying(64) COLLATE pg_catalog."default",
    "SearchNumber" character varying(64) COLLATE pg_catalog."default",
    "Description" character varying(512) COLLATE pg_catalog."default",
    "Price" numeric(18,2),
    "Count" integer,
    CONSTRAINT "PriceItems_pkey" PRIMARY KEY ("ID")
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."PriceItems"
    OWNER to postgres;
