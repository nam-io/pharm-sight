-- PharmSight 약국 경영 통합 대시보드 데이터베이스 스키마
-- SQLite 호환 DDL

PRAGMA foreign_keys = ON;

-- 환자 테이블
CREATE TABLE IF NOT EXISTS Patients (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    DateOfBirth TEXT    NOT NULL  -- ISO 8601 형식: YYYY-MM-DD
);

-- 처방 병원(의료기관) 테이블
CREATE TABLE IF NOT EXISTS Hospitals (
    Id   INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT    NOT NULL
);

-- 의약품 테이블
CREATE TABLE IF NOT EXISTS Drugs (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Name      TEXT    NOT NULL,
    Type      TEXT    NOT NULL CHECK (Type IN ('Rx', 'OTC')),  -- Rx: 전문의약품, OTC: 일반의약품
    IsCovered INTEGER NOT NULL DEFAULT 0  -- 1: 급여, 0: 비급여
);

-- 처방전(조제) 테이블
CREATE TABLE IF NOT EXISTS Prescriptions (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    PatientId    INTEGER NOT NULL,
    HospitalId   INTEGER NOT NULL,
    DispenseDate TEXT    NOT NULL,  -- ISO 8601 형식: YYYY-MM-DD
    FOREIGN KEY (PatientId)  REFERENCES Patients(Id),
    FOREIGN KEY (HospitalId) REFERENCES Hospitals(Id)
);

-- 도매 발주 테이블
CREATE TABLE IF NOT EXISTS Orders (
    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
    WholesaleName TEXT    NOT NULL,
    DrugId        INTEGER NOT NULL,
    Amount        REAL    NOT NULL,  -- 발주 금액 (원)
    OrderDate     TEXT    NOT NULL,  -- ISO 8601 형식: YYYY-MM-DD
    FOREIGN KEY (DrugId) REFERENCES Drugs(Id)
);

-- 매출 테이블
CREATE TABLE IF NOT EXISTS Sales (
    Id             INTEGER PRIMARY KEY AUTOINCREMENT,
    Amount         REAL    NOT NULL,  -- 매출 금액 (원)
    SaleDate       TEXT    NOT NULL,  -- ISO 8601 형식: YYYY-MM-DD
    PrescriptionId INTEGER,           -- NULL: 일반의약품(OTC) 매출, NOT NULL: 조제약 매출
    FOREIGN KEY (PrescriptionId) REFERENCES Prescriptions(Id)
);
