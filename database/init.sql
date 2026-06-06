CREATE TABLE IF NOT EXISTS insert_types (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  type_code varchar(20) NOT NULL,
  diameter_mm numeric(8,3) NOT NULL,
  pos_mapping varchar(20) NOT NULL
);

INSERT INTO insert_types(type_code, diameter_mm, pos_mapping)
SELECT 'M' || gs::text, gs::numeric, 'POS_' || lpad(gs::text, 2, '0')
FROM generate_series(8, 64) gs;
