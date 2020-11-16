CREATE DATABASE Customers303 COLLATE SQL_Latin1_General_CP1_CI_AS;

GO

USE Customers303

-- Create Customer table

CREATE TABLE [dbo].[customers]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [Email] NVARCHAR(50) NULL, 
    [PhoneNumber] NVARCHAR(50) NULL, 
    [Login] NVARCHAR(15) NOT NULL UNIQUE, 
    [Password] NVARCHAR(50) NOT NULL, 
    [IsDisabled] BIT NOT NULL DEFAULT 0, 
    [Created] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CreatedBy] INT NOT NULL DEFAULT 1,
    [Updated] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NOT NULL DEFAULT 1,
    [Deleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [FK_createdby_customer] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[customers] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_updateby_customer] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[customers] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
)

GO

-- Create Role table

CREATE TABLE [dbo].[roles]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(50) NOT NULL UNIQUE
)

GO

-- Create Customer - Role relation table

CREATE TABLE [dbo].[role_customer] (
    [CustomerId] INT NOT NULL,
    [RoleId]     INT NOT NULL,
    CONSTRAINT [FK_role_customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[customers] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_role_roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[roles] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);
