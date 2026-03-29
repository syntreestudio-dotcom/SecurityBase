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

-- CategoryTypes Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CategoryTypes')
BEGIN
    CREATE TABLE CategoryTypes (
        CategoryTypeId INT PRIMARY KEY IDENTITY(1,1),
        CategoryTypeName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(255) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        DisplayOrder INT NOT NULL DEFAULT 0,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
    CREATE INDEX IX_CategoryTypes_Name ON CategoryTypes(CategoryTypeName);
END
GO

-- Categories Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        CategoryId INT PRIMARY KEY IDENTITY(1,1),
        CategoryTypeId INT NOT NULL FOREIGN KEY REFERENCES CategoryTypes(CategoryTypeId),
        CategoryName NVARCHAR(150) NOT NULL,
        Code NVARCHAR(30) NULL,
        Description NVARCHAR(255) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        DisplayOrder INT NOT NULL DEFAULT 0,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
    CREATE INDEX IX_Categories_Name ON Categories(CategoryName);
    CREATE INDEX IX_Categories_CategoryTypeId ON Categories(CategoryTypeId);
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
