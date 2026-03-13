-- PharmSight 약국 경영 통합 대시보드 데이터베이스 스키마
-- PostgreSQL 호환 DDL (Supabase 적용용)

-- 환자 테이블
CREATE TABLE IF NOT EXISTS "Patients" (
    "Id"          SERIAL PRIMARY KEY,
    "DateOfBirth" DATE NOT NULL
);

-- 처방 병원(의료기관) 테이블
CREATE TABLE IF NOT EXISTS "Hospitals" (
    "Id"   SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL
);

-- 의약품 테이블
CREATE TABLE IF NOT EXISTS "Drugs" (
    "Id"        SERIAL PRIMARY KEY,
    "Name"      VARCHAR(200) NOT NULL,
    "Type"      VARCHAR(3)   NOT NULL CHECK ("Type" IN ('Rx', 'OTC')),
    "IsCovered" BOOLEAN      NOT NULL DEFAULT FALSE
);

-- 처방전(조제) 테이블
CREATE TABLE IF NOT EXISTS "Prescriptions" (
    "Id"           SERIAL PRIMARY KEY,
    "PatientId"    INTEGER NOT NULL REFERENCES "Patients"("Id"),
    "HospitalId"   INTEGER NOT NULL REFERENCES "Hospitals"("Id"),
    "DispenseDate" DATE    NOT NULL
);

-- 도매 발주 테이블
CREATE TABLE IF NOT EXISTS "Orders" (
    "Id"            SERIAL PRIMARY KEY,
    "WholesaleName" VARCHAR(100) NOT NULL,
    "DrugId"        INTEGER      NOT NULL REFERENCES "Drugs"("Id"),
    "Amount"        NUMERIC(12,2) NOT NULL,
    "OrderDate"     DATE          NOT NULL
);

-- 매출 테이블
CREATE TABLE IF NOT EXISTS "Sales" (
    "Id"             SERIAL PRIMARY KEY,
    "Amount"         NUMERIC(12,2) NOT NULL,
    "SaleDate"       DATE          NOT NULL,
    "PrescriptionId" INTEGER REFERENCES "Prescriptions"("Id")
);

-- 인덱스 (통계 쿼리 성능 최적화)
CREATE INDEX IF NOT EXISTS idx_sales_saledate         ON "Sales"("SaleDate");
CREATE INDEX IF NOT EXISTS idx_orders_orderdate        ON "Orders"("OrderDate");
CREATE INDEX IF NOT EXISTS idx_prescriptions_dispense  ON "Prescriptions"("DispenseDate");
CREATE INDEX IF NOT EXISTS idx_prescriptions_hospital  ON "Prescriptions"("HospitalId");
