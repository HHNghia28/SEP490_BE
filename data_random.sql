UPDATE [orbapicl_db_v2].[orbapicl_admin].[Schedules]
SET [Rank] = 
    CASE 
        WHEN RAND(CHECKSUM(NEWID())) <= 0.90 THEN N'A'
        WHEN RAND(CHECKSUM(NEWID())) <= 0.95 THEN N'B'
        WHEN RAND(CHECKSUM(NEWID())) <= 0.98 THEN N'C'
        ELSE N'D'
    END
WHERE [Date] <= CAST(GETDATE() AS DATE);

-------------------------------------------------------------------------
UPDATE [orbapicl_db_v2].[orbapicl_admin].[Attendances]
SET [Present] = 1
WHERE [Date] <= CAST(GETDATE() AS DATE);

WITH CTE AS (
    SELECT TOP 1 PERCENT *
    FROM [orbapicl_db_v2].[orbapicl_admin].[Attendances]
    WHERE [Date] <= CAST(GETDATE() AS DATE)
    ORDER BY NEWID()
)
UPDATE CTE
SET [Present] = 0;

WITH CTE AS (
    SELECT TOP 2 PERCENT *
    FROM [orbapicl_db_v2].[orbapicl_admin].[Attendances]
    WHERE [Date] <= CAST(GETDATE() AS DATE) AND [Present] = 1
    ORDER BY NEWID()
)
UPDATE CTE
SET [Present] = 0, [Confirmed] = 1;

-------------------------------------------------------------------------
UPDATE [orbapicl_db_v2].[orbapicl_admin].[StudentScores]
SET [Score] = 
    CASE 
        WHEN RAND(CHECKSUM(NEWID())) <= 0.1 THEN ROUND(3 + RAND(), 2)
        WHEN RAND(CHECKSUM(NEWID())) <= 0.2 THEN ROUND(4 + RAND(), 2)
        WHEN RAND(CHECKSUM(NEWID())) <= 0.3 THEN ROUND(5 + RAND(), 2)
        ELSE ROUND(6 + (10 - 6) * RAND(CHECKSUM(NEWID())), 2)
    END
WHERE [Subject] != N'Thể dục';

UPDATE [orbapicl_db_v2].[orbapicl_admin].[StudentScores]
SET [Score] = N'Đ'
WHERE [Subject] = N'Thể dục';

-------------------------------------------------------------------------
SELECT TOP 30 [StudentID]
INTO #RandomStudents
FROM [orbapicl_db_v2].[orbapicl_admin].[StudentScores]
ORDER BY NEWID();

UPDATE [orbapicl_db_v2].[orbapicl_admin].[StudentScores]
SET [Score] = ROUND(8 + (10 - 8) * RAND(CHECKSUM(NEWID())), 2)
WHERE [StudentID] IN (SELECT [StudentID] FROM #RandomStudents);

DROP TABLE #RandomStudents;