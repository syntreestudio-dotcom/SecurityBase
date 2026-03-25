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
