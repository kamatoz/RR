# RR
RussianRoboticsApp

Adding table PriceItems in PostgreSQL

CREATE TABLE IF NOT EXISTS public."PriceItems"
(</br>
    "ID" integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),</br>
    "Vendor" character varying(64) COLLATE pg_catalog."default",</br>
    "Number" character varying(64) COLLATE pg_catalog."default",</br>
    "SearchVendor" character varying(64) COLLATE pg_catalog."default",</br>
    "SearchNumber" character varying(64) COLLATE pg_catalog."default",</br>
    "Description" character varying(512) COLLATE pg_catalog."default",</br>
    "Price" numeric(18,2),</br>
    "Count" integer,</br>
    CONSTRAINT "PriceItems_pkey" PRIMARY KEY ("ID")</br>
)</br>
</br>
TABLESPACE pg_default;</br>
</br>
ALTER TABLE IF EXISTS public."PriceItems"</br>
    OWNER to postgres;</br>
