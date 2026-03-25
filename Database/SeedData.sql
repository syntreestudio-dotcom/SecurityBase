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
IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'Dashboard' AND (ParentMenuId IS NULL OR ParentMenuId = 0))
    INSERT INTO Menus (MenuName, Route, Icon, DisplayOrder) VALUES ('Dashboard', '/Home/Index', 'fas fa-chart-line', 1);

IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'User Management' AND (ParentMenuId IS NULL OR ParentMenuId = 0))
    INSERT INTO Menus (MenuName, Route, Icon, DisplayOrder) VALUES ('User Management', '/Users/Index', 'fas fa-users', 2);

IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'Role Management' AND (ParentMenuId IS NULL OR ParentMenuId = 0))
    INSERT INTO Menus (MenuName, Route, Icon, DisplayOrder) VALUES ('Role Management', '/Roles/Index', 'fas fa-user-tag', 3);

IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'Menu Management' AND (ParentMenuId IS NULL OR ParentMenuId = 0))
    INSERT INTO Menus (MenuName, Route, Icon, DisplayOrder) VALUES ('Menu Management', '/Menus/Index', 'fas fa-list', 4);

IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'Assignments' AND (ParentMenuId IS NULL OR ParentMenuId = 0))
    INSERT INTO Menus (MenuName, Route, Icon, DisplayOrder) VALUES ('Assignments', '#', 'fas fa-tasks', 5);

-- Sub-menus for Assignments
DECLARE @AssignmentsId INT = (SELECT TOP 1 MenuId FROM Menus WHERE MenuName = 'Assignments' AND (ParentMenuId IS NULL OR ParentMenuId = 0));
IF @AssignmentsId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'User Roles' AND ParentMenuId = @AssignmentsId)
        INSERT INTO Menus (MenuName, ParentMenuId, Route, Icon, DisplayOrder) VALUES ('User Roles', @AssignmentsId, '/Assignments/UserRoles', 'fas fa-user-shield', 1);

    IF NOT EXISTS (SELECT 1 FROM Menus WHERE MenuName = 'Role Menus' AND ParentMenuId = @AssignmentsId)
        INSERT INTO Menus (MenuName, ParentMenuId, Route, Icon, DisplayOrder) VALUES ('Role Menus', @AssignmentsId, '/Assignments/RoleMenus', 'fas fa-th-list', 2);
END
GO

-- 3. Insert Test Users (Passwords are plain-text for now; AuthService uses simple comparison)
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
