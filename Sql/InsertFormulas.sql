use DynamicFormulasDB

INSERT INTO tmp_targil (targil, tnai)
VALUES
-- נוסחאות פשוטות
('a + b', NULL),
('c * 2', 'c>5'),
('d - e', NULL),
('f / 4', NULL),

-- נוסחאות מורכבות
('(a + b) * 8', 'a>5'),
('SQRT(POWER(c,2) + POWER(d,2))', NULL),
('LOG(e) + f', NULL),
('ABS(g - h)', NULL),

-- נוסחאות עם תנאים
('CASE WHEN a > 5 THEN b*2 ELSE c/2 END', 'a > 5'),
('CASE WHEN d < 10 THEN e + 1 ELSE f - 1 END', 'd < 10'),
('CASE WHEN g = h THEN 1 ELSE 0 END', 'g = h'),

-- נוסחאות נוספות
('POWER(a+b,2)', NULL),                   
('SQRT(POWER(a-c,2) + POWER(b-d,2))', NULL),
('CASE WHEN e <> f THEN e*f ELSE e+f END', 'e <> f'), 
('CASE WHEN g > h THEN LOG(g) ELSE SQRT(h) END', 'g > h'); 