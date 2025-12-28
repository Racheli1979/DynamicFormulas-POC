use DynamicFormulasDB

DECLARE @BatchSize INT = 100000;
DECLARE @Inserted INT = 0;

-- הכנסת מליון נתונים רנדומלי
WHILE @Inserted < 1000000
BEGIN
    ;WITH Numbers AS (
        SELECT TOP (@BatchSize) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
        FROM master..spt_values AS a
        CROSS JOIN master..spt_values AS b
    )
    INSERT INTO tmp_data (a,b,c,d,e,f,g,h)
    SELECT 
        RAND(CHECKSUM(NEWID()))*100,
        RAND(CHECKSUM(NEWID()))*100,
        RAND(CHECKSUM(NEWID()))*100,
        RAND(CHECKSUM(NEWID()))*100,
        RAND(CHECKSUM(NEWID()))*100,
        RAND(CHECKSUM(NEWID()))*100,
        RAND(CHECKSUM(NEWID()))*100,
        RAND(CHECKSUM(NEWID()))*100
    FROM Numbers;

    SET @Inserted = @Inserted + @BatchSize;
END