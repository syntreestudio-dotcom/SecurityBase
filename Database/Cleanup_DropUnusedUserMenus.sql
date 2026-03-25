-- Cleanup script: removes legacy per-user menu overrides (UserMenus)
-- Safe to run multiple times.
-- IMPORTANT: This permanently drops the table and related stored procedures.
USE SecurityBase;
GO

SET NOCOUNT ON;

IF OBJECT_ID('dbo.sp_AssignMenuToUser', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_AssignMenuToUser;
END
GO

IF OBJECT_ID('dbo.sp_RevokeMenuFromUser', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_RevokeMenuFromUser;
END
GO

IF OBJECT_ID('dbo.UserMenus', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.UserMenus;
END
GO

PRINT 'Cleanup complete: UserMenus + per-user menu procs dropped.';

