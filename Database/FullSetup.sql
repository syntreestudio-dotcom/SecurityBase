-- SecurityBase Tables Script
USE SecurityBase;
GO

-- Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
    CREATE INDEX IX_Users_Username ON Users(Username);
END
GO

-- Roles Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE Roles (
        RoleId INT PRIMARY KEY IDENTITY(1,1),
        RoleName NVARCHAR(100) NOT NULL UNIQUE,
        Description NVARCHAR(255) NULL
    );
END
GO

-- Menus Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Menus')
BEGIN
    CREATE TABLE Menus (
        MenuId INT PRIMARY KEY IDENTITY(1,1),
        MenuName NVARCHAR(100) NOT NULL,
        ParentMenuId INT NULL FOREIGN KEY REFERENCES Menus(MenuId),
        Route NVARCHAR(200) NULL,
        Icon NVARCHAR(100) NULL,
        DisplayOrder INT NOT NULL DEFAULT 0
    );
END
GO

-- UserRoles Table (Mapping)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserRoles')
BEGIN
    CREATE TABLE UserRoles (
        UserRoleId INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
        RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(RoleId),
        CONSTRAINT UQ_UserRoles_User_Role UNIQUE (UserId, RoleId)
    );
    CREATE INDEX IX_UserRoles_UserId ON UserRoles(UserId);
    CREATE INDEX IX_UserRoles_RoleId ON UserRoles(RoleId);
END
GO

-- RoleMenus Table (Mapping) - Role-based menu access
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RoleMenus')
BEGIN
    CREATE TABLE RoleMenus (
        RoleMenuId INT PRIMARY KEY IDENTITY(1,1),
        RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(RoleId),
        MenuId INT NOT NULL FOREIGN KEY REFERENCES Menus(MenuId),
        CONSTRAINT UQ_RoleMenus_Role_Menu UNIQUE (RoleId, MenuId)
    );
    CREATE INDEX IX_RoleMenus_RoleId ON RoleMenus(RoleId);
    CREATE INDEX IX_RoleMenus_MenuId ON RoleMenus(MenuId);
END
GO
-- SecurityBase Stored Procedures Script
USE SecurityBase;
GO

-------------------------------------------------------------------------------
-- USER PROCEDURES
-------------------------------------------------------------------------------

-- sp_CreateUser
CREATE OR ALTER PROCEDURE sp_CreateUser
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @Email NVARCHAR(150),
    @IsActive BIT
AS
BEGIN
    INSERT INTO Users (Username, PasswordHash, Email, IsActive, CreatedDate)
    VALUES (@Username, @PasswordHash, @Email, @IsActive, GETDATE());
    SELECT SCOPE_IDENTITY() AS UserId;
END
GO

-- sp_GetUsers
CREATE OR ALTER PROCEDURE sp_GetUsers
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SELECT *, COUNT(*) OVER() AS TotalCount
    FROM Users
    WHERE (@SearchTerm IS NULL OR Username LIKE '%' + @SearchTerm + '%' OR Email LIKE '%' + @SearchTerm + '%')
    ORDER BY UserId
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- sp_UpdateUser
CREATE OR ALTER PROCEDURE sp_UpdateUser
    @UserId INT,
    @Username NVARCHAR(100),
    @Email NVARCHAR(150),
    @IsActive BIT
AS
BEGIN
    UPDATE Users
    SET Username = @Username,
        Email = @Email,
        IsActive = @IsActive
    WHERE UserId = @UserId;
END
GO

-- sp_UpdateUserPassword
CREATE OR ALTER PROCEDURE sp_UpdateUserPassword
    @UserId INT,
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    UPDATE Users
    SET PasswordHash = @PasswordHash
    WHERE UserId = @UserId;
END
GO

-- sp_DeleteUser
CREATE OR ALTER PROCEDURE sp_DeleteUser
    @UserId INT
AS
BEGIN
    DELETE FROM UserRoles WHERE UserId = @UserId;
    DELETE FROM Users WHERE UserId = @UserId;
END
GO

-------------------------------------------------------------------------------
-- ROLE PROCEDURES
-------------------------------------------------------------------------------

-- sp_CreateRole
CREATE OR ALTER PROCEDURE sp_CreateRole
    @RoleName NVARCHAR(100),
    @Description NVARCHAR(255)
AS
BEGIN
    INSERT INTO Roles (RoleName, Description)
    VALUES (@RoleName, @Description);
    SELECT SCOPE_IDENTITY() AS RoleId;
END
GO

-- sp_GetRoles
CREATE OR ALTER PROCEDURE sp_GetRoles
AS
BEGIN
    SELECT * FROM Roles ORDER BY RoleName;
END
GO

-- sp_UpdateRole
CREATE OR ALTER PROCEDURE sp_UpdateRole
    @RoleId INT,
    @RoleName NVARCHAR(100),
    @Description NVARCHAR(255)
AS
BEGIN
    UPDATE Roles
    SET RoleName = @RoleName,
        Description = @Description
    WHERE RoleId = @RoleId;
END
GO

-- sp_DeleteRole
CREATE OR ALTER PROCEDURE sp_DeleteRole
    @RoleId INT
AS
BEGIN
    DELETE FROM RoleMenus WHERE RoleId = @RoleId;
    DELETE FROM UserRoles WHERE RoleId = @RoleId;
    DELETE FROM Roles WHERE RoleId = @RoleId;
END
GO

-------------------------------------------------------------------------------
-- MENU PROCEDURES
-------------------------------------------------------------------------------

-- sp_CreateMenu
CREATE OR ALTER PROCEDURE sp_CreateMenu
    @MenuName NVARCHAR(100),
    @ParentMenuId INT,
    @Route NVARCHAR(200),
    @Icon NVARCHAR(100),
    @DisplayOrder INT
AS
BEGIN
    INSERT INTO Menus (MenuName, ParentMenuId, Route, Icon, DisplayOrder)
    VALUES (@MenuName, @ParentMenuId, @Route, @Icon, @DisplayOrder);
    SELECT SCOPE_IDENTITY() AS MenuId;
END
GO

-- sp_GetMenus
CREATE OR ALTER PROCEDURE sp_GetMenus
AS
BEGIN
    SELECT * FROM Menus ORDER BY DisplayOrder;
END
GO

-- sp_UpdateMenu
CREATE OR ALTER PROCEDURE sp_UpdateMenu
    @MenuId INT,
    @MenuName NVARCHAR(100),
    @ParentMenuId INT,
    @Route NVARCHAR(200),
    @Icon NVARCHAR(100),
    @DisplayOrder INT
AS
BEGIN
    UPDATE Menus
    SET MenuName = @MenuName,
        ParentMenuId = @ParentMenuId,
        Route = @Route,
        Icon = @Icon,
        DisplayOrder = @DisplayOrder
    WHERE MenuId = @MenuId;
END
GO

-- sp_DeleteMenu
CREATE OR ALTER PROCEDURE sp_DeleteMenu
    @MenuId INT
AS
BEGIN
    DELETE FROM RoleMenus WHERE MenuId = @MenuId;
    DELETE FROM Menus WHERE MenuId = @MenuId;
END
GO

-------------------------------------------------------------------------------
-- ASSIGNMENT PROCEDURES
-------------------------------------------------------------------------------

-- sp_AssignRoleToUser
CREATE OR ALTER PROCEDURE sp_AssignRoleToUser
    @UserId INT,
    @RoleId INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
    BEGIN
        INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);
    END
END
GO

-- sp_RevokeRoleFromUser
CREATE OR ALTER PROCEDURE sp_RevokeRoleFromUser
    @UserId INT,
    @RoleId INT
AS
BEGIN
    DELETE FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId;
END
GO

-- sp_SetUserSingleRole
-- Ensures a user has exactly one role (replaces any existing roles)
CREATE OR ALTER PROCEDURE sp_SetUserSingleRole
    @UserId INT,
    @RoleId INT
AS
BEGIN
    DELETE FROM UserRoles WHERE UserId = @UserId;

    IF @RoleId IS NOT NULL AND @RoleId > 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
        BEGIN
            INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);
        END
    END
END
GO

-- sp_AssignMenuToRole
CREATE OR ALTER PROCEDURE sp_AssignMenuToRole
    @RoleId INT,
    @MenuId INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM RoleMenus WHERE RoleId = @RoleId AND MenuId = @MenuId)
    BEGIN
        INSERT INTO RoleMenus (RoleId, MenuId) VALUES (@RoleId, @MenuId);
    END
END
GO

-- sp_RevokeMenuFromRole
CREATE OR ALTER PROCEDURE sp_RevokeMenuFromRole
    @RoleId INT,
    @MenuId INT
AS
BEGIN
    DELETE FROM RoleMenus WHERE RoleId = @RoleId AND MenuId = @MenuId;
END
GO

-- sp_GetRoleMenus
CREATE OR ALTER PROCEDURE sp_GetRoleMenus
    @RoleId INT
AS
BEGIN
    SELECT m.*
    FROM Menus m
    JOIN RoleMenus rm ON m.MenuId = rm.MenuId
    WHERE rm.RoleId = @RoleId;
END
GO

-- sp_GetUserRoles
CREATE OR ALTER PROCEDURE sp_GetUserRoles
    @UserId INT
AS
BEGIN
    SELECT r.* 
    FROM Roles r
    JOIN UserRoles ur ON r.RoleId = ur.RoleId
    WHERE ur.UserId = @UserId;
END
GO

-- sp_GetUserMenus
CREATE OR ALTER PROCEDURE sp_GetUserMenus
    @UserId INT
AS
BEGIN
    -- Role-based menus only (menus come from roles)
    SELECT DISTINCT m.*
    FROM Menus m
    JOIN RoleMenus rm ON m.MenuId = rm.MenuId
    JOIN UserRoles ur ON ur.RoleId = rm.RoleId
    WHERE ur.UserId = @UserId
END
GO

-- sp_GetUserPermissions
-- Combined data for Auth
CREATE OR ALTER PROCEDURE sp_GetUserPermissions
    @Username NVARCHAR(100)
AS
BEGIN
    -- User Info
    SELECT * FROM Users WHERE Username = @Username;

    -- Roles
    SELECT r.RoleName 
    FROM Roles r
    JOIN UserRoles ur ON r.RoleId = ur.RoleId
    JOIN Users u ON ur.UserId = u.UserId
    WHERE u.Username = @Username;

    -- Menus
    ;WITH UserCte AS (
        SELECT u.UserId
        FROM Users u
        WHERE u.Username = @Username
    )
    SELECT DISTINCT m.*
    FROM Menus m
    JOIN RoleMenus rm ON m.MenuId = rm.MenuId
    JOIN UserRoles ur ON ur.RoleId = rm.RoleId
    JOIN UserCte uc ON uc.UserId = ur.UserId
END
GO
-- SecurityBase Seed Data Script
USE SecurityBase;
GO

-- 1. Insert Default Roles
IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Administrator')
    INSERT INTO Roles (RoleName, Description) VALUES ('Administrator', 'Full system access');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Manager')
    INSERT INTO Roles (RoleName, Description) VALUES ('Manager', 'Limited management access');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'User')
    INSERT INTO Roles (RoleName, Description) VALUES ('User', 'Standard user access');

-- 2. Insert Default Menus
IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'Dashboard')
    INSERT INTO Menus (MenuName, Route, Icon, DisplayOrder) VALUES ('Dashboard', '/Home/Index', 'fas fa-chart-line', 1);

IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'User Management')
    INSERT INTO Menus (MenuName, Route, Icon, DisplayOrder) VALUES ('User Management', '/Users/Index', 'fas fa-users', 2);

IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'Role Management')
    INSERT INTO Menus (MenuName, Route, Icon, DisplayOrder) VALUES ('Role Management', '/Roles/Index', 'fas fa-user-tag', 3);

IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'Menu Management')
    INSERT INTO Menus (MenuName, Route, Icon, DisplayOrder) VALUES ('Menu Management', '/Menus/Index', 'fas fa-list', 4);

IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'Assignments')
    INSERT INTO Menus (MenuName, Route, Icon, DisplayOrder) VALUES ('Assignments', '#', 'fas fa-tasks', 5);

-- Sub-menus for Assignments
DECLARE @AssignmentsId INT = (SELECT MenuId FROM Menus WHERE MenuName = 'Assignments');
IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'User Roles' AND ParentMenuId = @AssignmentsId)
    INSERT INTO Menus (MenuName, ParentMenuId, Route, Icon, DisplayOrder) VALUES ('User Roles', @AssignmentsId, '/Assignments/UserRoles', 'fas fa-user-shield', 1);

IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'Role Menus' AND ParentMenuId = @AssignmentsId)
    INSERT INTO Menus (MenuName, ParentMenuId, Route, Icon, DisplayOrder) VALUES ('Role Menus', @AssignmentsId, '/Assignments/RoleMenus', 'fas fa-th-list', 2);

-- 3. Insert Test Users (PBKDF2 hashed; UI password is still Password@123 for these test users)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
    INSERT INTO Users (Username, PasswordHash, Email, IsActive)
    VALUES ('admin', 'PBKDF2$SHA256$100000$spz3Xw4KYnpMe2/SHFiQTg==$7Ucc9GqPYxsXB9a9gGViMu5SYAEKsw+L8v+vgmLr2q8=', 'admin@securitybase.com', 1);

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'manager')
    INSERT INTO Users (Username, PasswordHash, Email, IsActive)
    VALUES ('manager', 'PBKDF2$SHA256$100000$GQRMsgeqe/Ahm5h7PQ8Bgg==$zWmx9B14OOq6kEH7Am/rqibUKAq1QhoF+13K87VmWIQ=', 'manager@securitybase.com', 1);

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'user1')
    INSERT INTO Users (Username, PasswordHash, Email, IsActive)
    VALUES ('user1', 'PBKDF2$SHA256$100000$0awdQRHJVKxBKHphuhfHCg==$C6kz0KfpFDS7aPR3OCNFWZZQkVvB+dGt4TT7FQrYeVw=', 'user1@securitybase.com', 1);
GO

-- 4. Assign Roles to Users
DECLARE @AdminUserId INT = (SELECT UserId FROM Users WHERE Username = 'admin');
DECLARE @ManagerUserId INT = (SELECT UserId FROM Users WHERE Username = 'manager');
DECLARE @User1Id INT = (SELECT UserId FROM Users WHERE Username = 'user1');

DECLARE @AdminRoleId INT = (SELECT RoleId FROM Roles WHERE RoleName = 'Administrator');
DECLARE @ManagerRoleId INT = (SELECT RoleId FROM Roles WHERE RoleName = 'Manager');
DECLARE @UserRoleId INT = (SELECT RoleId FROM Roles WHERE RoleName = 'User');

IF @AdminUserId IS NOT NULL AND @AdminRoleId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserId = @AdminUserId AND RoleId = @AdminRoleId)
    INSERT INTO UserRoles (UserId, RoleId) VALUES (@AdminUserId, @AdminRoleId);

IF @ManagerUserId IS NOT NULL AND @ManagerRoleId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserId = @ManagerUserId AND RoleId = @ManagerRoleId)
    INSERT INTO UserRoles (UserId, RoleId) VALUES (@ManagerUserId, @ManagerRoleId);

IF @User1Id IS NOT NULL AND @UserRoleId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserId = @User1Id AND RoleId = @UserRoleId)
    INSERT INTO UserRoles (UserId, RoleId) VALUES (@User1Id, @UserRoleId);
GO

-- 5. Assign Menus to Roles (RoleMenus)
-- Administrator gets all menus
IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO RoleMenus (RoleId, MenuId)
    SELECT @AdminRoleId, m.MenuId
    FROM Menus m
    WHERE NOT EXISTS (SELECT 1 FROM RoleMenus rm WHERE rm.RoleId = @AdminRoleId AND rm.MenuId = m.MenuId);
END

-- Manager gets Dashboard + Assignments (and its children)
IF @ManagerRoleId IS NOT NULL
BEGIN
    DECLARE @DashboardId INT = (SELECT TOP 1 MenuId FROM Menus WHERE MenuName = 'Dashboard' AND (ParentMenuId IS NULL OR ParentMenuId = 0));
    DECLARE @AssignmentsTopId INT = (SELECT TOP 1 MenuId FROM Menus WHERE MenuName = 'Assignments' AND (ParentMenuId IS NULL OR ParentMenuId = 0));

    IF @DashboardId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoleMenus WHERE RoleId = @ManagerRoleId AND MenuId = @DashboardId)
        INSERT INTO RoleMenus (RoleId, MenuId) VALUES (@ManagerRoleId, @DashboardId);

    IF @AssignmentsTopId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoleMenus WHERE RoleId = @ManagerRoleId AND MenuId = @AssignmentsTopId)
        INSERT INTO RoleMenus (RoleId, MenuId) VALUES (@ManagerRoleId, @AssignmentsTopId);

    IF @AssignmentsTopId IS NOT NULL
    BEGIN
        INSERT INTO RoleMenus (RoleId, MenuId)
        SELECT @ManagerRoleId, m.MenuId
        FROM Menus m
        WHERE m.ParentMenuId = @AssignmentsTopId
          AND NOT EXISTS (SELECT 1 FROM RoleMenus rm WHERE rm.RoleId = @ManagerRoleId AND rm.MenuId = m.MenuId);
    END
END

-- User gets Dashboard only
IF @UserRoleId IS NOT NULL
BEGIN
    DECLARE @DashboardId2 INT = (SELECT TOP 1 MenuId FROM Menus WHERE MenuName = 'Dashboard' AND (ParentMenuId IS NULL OR ParentMenuId = 0));
    IF @DashboardId2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoleMenus WHERE RoleId = @UserRoleId AND MenuId = @DashboardId2)
        INSERT INTO RoleMenus (RoleId, MenuId) VALUES (@UserRoleId, @DashboardId2);
END
GO
