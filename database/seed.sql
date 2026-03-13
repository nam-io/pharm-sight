-- PharmSight 샘플 데이터 (Seed Data)
-- Supabase SQL Editor에서 schema.sql 실행 후 이 파일을 실행하세요.

-- 병원 데이터
INSERT INTO "Hospitals" ("Name") VALUES
    ('한빛소아과의원'),
    ('서울내과클리닉'),
    ('미래정형외과'),
    ('하늘가정의학과'),
    ('연세이비인후과'),
    ('그린피부과의원')
ON CONFLICT DO NOTHING;

-- 의약품 데이터
INSERT INTO "Drugs" ("Name", "Type", "IsCovered") VALUES
    ('아목시실린 250mg', 'Rx', TRUE),
    ('타이레놀 500mg', 'OTC', FALSE),
    ('메트포르민 500mg', 'Rx', TRUE),
    ('이부프로펜 200mg', 'OTC', FALSE),
    ('암로디핀 5mg', 'Rx', TRUE),
    ('비타민C 1000mg', 'OTC', FALSE),
    ('세티리진 10mg', 'Rx', TRUE),
    ('오메프라졸 20mg', 'Rx', TRUE),
    ('아스피린 100mg', 'OTC', TRUE),
    ('리보플라빈 5mg', 'OTC', FALSE)
ON CONFLICT DO NOTHING;

-- 환자 데이터 (100명)
INSERT INTO "Patients" ("DateOfBirth")
SELECT
    CURRENT_DATE - (INTERVAL '1 day' * (random() * 365 * 75 + 365 * 5)::int)
FROM generate_series(1, 100);

-- 처방전 데이터 (최근 13개월, 병원별 배분)
INSERT INTO "Prescriptions" ("PatientId", "HospitalId", "DispenseDate")
SELECT
    (random() * 99 + 1)::int,
    CASE
        WHEN n % 18 < 5  THEN 1  -- 한빛소아과 (가장 많음)
        WHEN n % 18 < 9  THEN 2  -- 서울내과
        WHEN n % 18 < 12 THEN 3  -- 미래정형외과
        WHEN n % 18 < 14 THEN 4  -- 하늘가정의학과
        WHEN n % 18 < 16 THEN 5  -- 연세이비인후과
        ELSE                  6  -- 그린피부과
    END,
    CURRENT_DATE - (INTERVAL '1 day' * (random() * 395)::int)
FROM generate_series(1, 1800) AS t(n);

-- 매출 데이터 (처방조제 매출 - PrescriptionId 연결)
INSERT INTO "Sales" ("Amount", "SaleDate", "PrescriptionId")
SELECT
    (random() * 40000 + 5000)::numeric(12,2),
    p."DispenseDate",
    p."Id"
FROM "Prescriptions" p;

-- 매출 데이터 (OTC 일반판매 - PrescriptionId NULL)
INSERT INTO "Sales" ("Amount", "SaleDate", "PrescriptionId")
SELECT
    (random() * 15000 + 2000)::numeric(12,2),
    CURRENT_DATE - (INTERVAL '1 day' * (random() * 395)::int),
    NULL
FROM generate_series(1, 600);

-- 도매 발주 데이터 (최근 13개월)
INSERT INTO "Orders" ("WholesaleName", "DrugId", "Amount", "OrderDate")
SELECT
    CASE (n % 5)
        WHEN 0 THEN '한국의약품유통'
        WHEN 1 THEN '대원제약물류'
        WHEN 2 THEN '지오영'
        WHEN 3 THEN '백제약품'
        ELSE        '신풍제약유통'
    END,
    (random() * 9 + 1)::int,
    (random() * 800000 + 100000)::numeric(12,2),
    CURRENT_DATE - (INTERVAL '1 day' * (random() * 395)::int)
FROM generate_series(1, 300) AS t(n);
