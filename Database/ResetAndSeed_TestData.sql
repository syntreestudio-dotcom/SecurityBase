-- SecurityBase Reset + Test Seed Script
-- WARNING: This deletes ALL data from core tables and reseeds with fresh test data.
USE SecurityBase;
GO

SET NOCOUNT ON;

IF OBJECT_ID('dbo.Users', 'U') IS NULL OR OBJECT_ID('dbo.Roles', 'U') IS NULL OR OBJECT_ID('dbo.Menus', 'U') IS NULL
BEGIN
    RAISERROR('Missing core tables. Run Database/Tables.sql first.', 16, 1);
    RETURN;
END

IF OBJECT_ID('dbo.RoleMenus', 'U') IS NULL
BEGIN
    RAISERROR('Missing RoleMenus table. Run Database/Tables.sql (updated) first.', 16, 1);
    RETURN;
END

-------------------------------------------------------------------------------
-- 1) Delete existing data (mappings first)
-------------------------------------------------------------------------------
IF OBJECT_ID('dbo.RoleMenus', 'U') IS NOT NULL DELETE FROM RoleMenus;
IF OBJECT_ID('dbo.UserRoles', 'U') IS NOT NULL DELETE FROM UserRoles;

-- Delete Menus bottom-up because of the self FK (ParentMenuId -> Menus.MenuId)
IF OBJECT_ID('dbo.Menus', 'U') IS NOT NULL
BEGIN
    WHILE EXISTS (SELECT 1 FROM Menus)
    BEGIN
        DELETE TOP (1000) m
        FROM Menus m
        WHERE NOT EXISTS (SELECT 1 FROM Menus c WHERE c.ParentMenuId = m.MenuId);
    END
END

IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DELETE FROM Roles;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DELETE FROM Users;
GO

-------------------------------------------------------------------------------
-- 2) Reseed identities (optional but keeps ids predictable for tests)
-------------------------------------------------------------------------------
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DBCC CHECKIDENT ('Users', RESEED, 0);
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DBCC CHECKIDENT ('Roles', RESEED, 0);
IF OBJECT_ID('dbo.Menus', 'U') IS NOT NULL DBCC CHECKIDENT ('Menus', RESEED, 0);
IF OBJECT_ID('dbo.UserRoles', 'U') IS NOT NULL DBCC CHECKIDENT ('UserRoles', RESEED, 0);
IF OBJECT_ID('dbo.RoleMenus', 'U') IS NOT NULL DBCC CHECKIDENT ('RoleMenus', RESEED, 0);
GO

-------------------------------------------------------------------------------
-- 3) Insert fresh test data
-------------------------------------------------------------------------------

-- Roles
INSERT INTO Roles (RoleName, Description) VALUES
('Administrator', 'Full system access'),
('Manager', 'Limited management access'),
('User', 'Standard user access');

-- Menus (top-level)
INSERT INTO Menus (MenuName, ParentMenuId, Route, Icon, DisplayOrder) VALUES
('Dashboard', NULL, '/Home/Index', 'fas fa-chart-line', 1),
('User Management', NULL, '/Users/Index', 'fas fa-users', 2),
('Role Management', NULL, '/Roles/Index', 'fas fa-user-tag', 3),
('Menu Management', NULL, '/Menus/Index', 'fas fa-list', 4),
('Assignments', NULL, '#', 'fas fa-tasks', 5);

DECLARE @AssignmentsId INT = (SELECT TOP 1 MenuId FROM Menus WHERE MenuName = 'Assignments' AND (ParentMenuId IS NULL OR ParentMenuId = 0));

-- Menus (children)
IF @AssignmentsId IS NOT NULL
BEGIN
    INSERT INTO Menus (MenuName, ParentMenuId, Route, Icon, DisplayOrder) VALUES
    ('User Roles', @AssignmentsId, '/Assignments/UserRoles', 'fas fa-user-shield', 1),
    ('Role Menus', @AssignmentsId, '/Assignments/RoleMenus', 'fas fa-th-list', 2);
END

-- Users (PBKDF2 hashed; UI password is still Password@123 for these test users)
INSERT INTO Users (Username, PasswordHash, Email, IsActive) VALUES
('admin', 'PBKDF2$SHA256$100000$spz3Xw4KYnpMe2/SHFiQTg==$7Ucc9GqPYxsXB9a9gGViMu5SYAEKsw+L8v+vgmLr2q8=', 'admin@securitybase.com', 1),
('manager', 'PBKDF2$SHA256$100000$GQRMsgeqe/Ahm5h7PQ8Bgg==$zWmx9B14OOq6kEH7Am/rqibUKAq1QhoF+13K87VmWIQ=', 'manager@securitybase.com', 1),
('user1', 'PBKDF2$SHA256$100000$0awdQRHJVKxBKHphuhfHCg==$C6kz0KfpFDS7aPR3OCNFWZZQkVvB+dGt4TT7FQrYeVw=', 'user1@securitybase.com', 1);

-- Assign roles to users
DECLARE @AdminUserId INT = (SELECT UserId FROM Users WHERE Username='admin');
DECLARE @ManagerUserId INT = (SELECT UserId FROM Users WHERE Username='manager');
DECLARE @User1Id INT = (SELECT UserId FROM Users WHERE Username='user1');

DECLARE @AdminRoleId INT = (SELECT RoleId FROM Roles WHERE RoleName='Administrator');
DECLARE @ManagerRoleId INT = (SELECT RoleId FROM Roles WHERE RoleName='Manager');
DECLARE @UserRoleId INT = (SELECT RoleId FROM Roles WHERE RoleName='User');

INSERT INTO UserRoles (UserId, RoleId) VALUES
(@AdminUserId, @AdminRoleId),
(@ManagerUserId, @ManagerRoleId),
(@User1Id, @UserRoleId);

-- Role-based menus
-- Admin gets all
INSERT INTO RoleMenus (RoleId, MenuId)
SELECT @AdminRoleId, MenuId FROM Menus;

-- Manager gets Dashboard + Assignments + children
DECLARE @DashboardId INT = (SELECT TOP 1 MenuId FROM Menus WHERE MenuName='Dashboard' AND (ParentMenuId IS NULL OR ParentMenuId = 0));
IF @DashboardId IS NOT NULL INSERT INTO RoleMenus (RoleId, MenuId) VALUES (@ManagerRoleId, @DashboardId);
IF @AssignmentsId IS NOT NULL INSERT INTO RoleMenus (RoleId, MenuId) VALUES (@ManagerRoleId, @AssignmentsId);
IF @AssignmentsId IS NOT NULL
BEGIN
    INSERT INTO RoleMenus (RoleId, MenuId)
    SELECT @ManagerRoleId, MenuId FROM Menus WHERE ParentMenuId=@AssignmentsId;
END

-- User gets Dashboard only
IF @DashboardId IS NOT NULL INSERT INTO RoleMenus (RoleId, MenuId) VALUES (@UserRoleId, @DashboardId);
GO

PRINT 'Reset + test seed completed.';
