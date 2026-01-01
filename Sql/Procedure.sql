CREATE PROCEDURE dbo.DynamicFormulasProcedure
AS
BEGIN
    SET NOCOUNT ON;

    -- יצירת טבלה זמנית לתוצאות
    CREATE TABLE #temp_results (
        data_id INT,
        targil_id INT,
        method NVARCHAR(50),
        result FLOAT,
        execution_time FLOAT
    );

    DECLARE @sql NVARCHAR(MAX) = N'';

    -- דינמי לכל התרגילים SQL יצירת
    SELECT @sql = @sql + '
        INSERT INTO #temp_results (data_id, targil_id, method, result, execution_time)
        SELECT 
            d.id AS data_id,
            ' + CAST(t.id AS NVARCHAR(10)) + ' AS targil_id,  -- הערך של התרגיל מוכנס ישירות
            ''DB Procedure'' AS method,
            CAST((' + 
                CASE 
                    WHEN CHARINDEX('ELSE', UPPER(t.targil)) = 0 
                    THEN 'CASE WHEN ' + ISNULL(t.tnai,'1=1') + ' THEN ' + t.targil + ' ELSE NULL END' 
                    ELSE t.targil 
                END
            + ') AS FLOAT) AS result,
            CAST(DATEDIFF(MICROSECOND, timer.start_time, SYSDATETIME())/1000000.0 AS FLOAT) AS execution_time
        FROM tmp_data d
        CROSS APPLY (SELECT SYSDATETIME() AS start_time) AS timer;
    '
    FROM tmp_targil t;

    -- הרצת כל החישובים בבת אחת
    EXEC sp_executesql @sql;

    -- העתקת התוצאות לטבלה הקבועה
    INSERT INTO tmp_results (data_id, targil_id, method, result, execution_time)
    SELECT data_id, targil_id, method, result, execution_time
    FROM #temp_results;

    DROP TABLE #temp_results;
END;
GO

-- הרצה
EXEC dbo.DynamicFormulasProcedure;
