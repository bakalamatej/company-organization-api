-- ================================
-- Delete firmyDB if it exists
-- ================================
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'firmyDB')
BEGIN
    ALTER DATABASE firmyDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE firmyDB;
    PRINT 'Database firmyDB has been deleted.';
END
ELSE
BEGIN
    PRINT 'Database firmyDB does not exist.';
END
GO