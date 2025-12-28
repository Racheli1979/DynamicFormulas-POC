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

    DECLARE 
        @targil_id INT,
        @targil_expr NVARCHAR(MAX),
        @tnai NVARCHAR(MAX),
        @data_id INT,
        @res FLOAT,
        @start DATETIME2,
        @end DATETIME2,
        @elapsed FLOAT,
        @sql NVARCHAR(MAX);

    -- לולאה על כל התרגילים
    DECLARE targil_cursor CURSOR FOR
        SELECT id, targil, tnai FROM tmp_targil;

    OPEN targil_cursor;
    FETCH NEXT FROM targil_cursor INTO @targil_id, @targil_expr, @tnai;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- בניית CASE אם אין ELSE
        IF CHARINDEX('ELSE', UPPER(@targil_expr)) = 0
            SET @targil_expr = 'CASE WHEN ' + ISNULL(@tnai,'1=1') + ' THEN ' + @targil_expr + ' ELSE NULL END';

        -- לולאה על כל רשומה ב-tmp_data
        DECLARE data_cursor CURSOR FOR SELECT id FROM tmp_data;
        OPEN data_cursor;
        FETCH NEXT FROM data_cursor INTO @data_id;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- מדידת זמן לפני החישוב
            SET @start = SYSDATETIME();

            -- חישוב הביטוי עם הפניה לרשומה ספציפית
            SET @sql = '
                SELECT @res_out = CAST((' + @targil_expr + ') AS FLOAT)
                FROM tmp_data
                WHERE id = ' + CAST(@data_id AS NVARCHAR);

            EXEC sp_executesql @sql, N'@res_out FLOAT OUTPUT', @res_out=@res OUTPUT;

            -- מדידת זמן אחרי החישוב
            SET @end = SYSDATETIME();
            SET @elapsed = DATEDIFF(MICROSECOND, @start, @end)/1000000.0;

            -- הכנסת התוצאה לטבלה הזמנית
            INSERT INTO #temp_results (data_id, targil_id, method, result, execution_time)
            VALUES (@data_id, @targil_id, 'DB Procedure', @res, @elapsed);

            FETCH NEXT FROM data_cursor INTO @data_id;
        END

        CLOSE data_cursor;
        DEALLOCATE data_cursor;

        FETCH NEXT FROM targil_cursor INTO @targil_id, @targil_expr, @tnai;
    END

    CLOSE targil_cursor;
    DEALLOCATE targil_cursor;

    -- העתקת התוצאות לטבלה הקבועה
    INSERT INTO tmp_results (data_id, targil_id, method, result, execution_time)
    SELECT data_id, targil_id, method, result, execution_time
    FROM #temp_results;

    DROP TABLE #temp_results;
END;
GO

-- הרצת הפרוצדורה
EXEC dbo.DynamicFormulasProcedure;
