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

GO

-- Default roles

INSERT INTO [dbo].[roles] VALUES ('Administrator');
INSERT INTO [dbo].[roles] VALUES ('Customer');
INSERT INTO [dbo].[roles] VALUES ('Operator');
INSERT INTO [dbo].[roles] VALUES ('Manager');

GO

-- Admin

INSERT INTO [dbo].[customers] 
(
	FirstName,
	LastName,
	Email,
	PhoneNumber,
	Login,
	Password
) 
VALUES
(
	'Andrey',
	'Perfectionist',
	'andrey@gmail.com',
	'1234567890',
	'admin',
	'+TFHNvcqJxrWTHki4Pi50RtCsrfOfULOtdX6xQnbes4='
);

GO

-- Admin roles

INSERT INTO [dbo].[role_customer] VALUES('1', '1');
INSERT INTO [dbo].[role_customer] VALUES('1', '2');
INSERT INTO [dbo].[role_customer] VALUES('1', '3');
INSERT INTO [dbo].[role_customer] VALUES('1', '4');

GO 

INSERT INTO [dbo].[customers] ([FirstName], [LastName], [Email], [PhoneNumber], [Login], [Password], [IsDisabled], [Created], [CreatedBy], [Updated], [UpdatedBy], [Deleted]) VALUES (N'Operator1', N'Operator1', N'operator1@gmail.com', N'1234567890', N'operator1', N'QPIxDQk7s7UvkBfGTQT9ExWOI1F26hp+XiQu2rJXoio=', 0, N'2020-11-02 21:49:02', 1, N'2020-11-05 18:28:47', 1, 0)
INSERT INTO [dbo].[customers] ([FirstName], [LastName], [Email], [PhoneNumber], [Login], [Password], [IsDisabled], [Created], [CreatedBy], [Updated], [UpdatedBy], [Deleted]) VALUES (N'Manager1', N'Manager1', N'manager1@gmail.com', N'1234567890', N'manager1', N'7ydhIilyJFmPA2Asd2JuwNsWEE0iMi0XAVwsIDKA++g=', 0, N'2020-11-03 09:33:41', 1, N'2020-11-03 09:33:41', 1, 0)
INSERT INTO [dbo].[customers] ([FirstName], [LastName], [Email], [PhoneNumber], [Login], [Password], [IsDisabled], [Created], [CreatedBy], [Updated], [UpdatedBy], [Deleted]) VALUES (N'Admin1', N'Admin1', N'admin1@gmail.com', N'1234567890', N'admin1', N'd+1KoBUTCklOdoLrAlBqNmWAi9Rxmaf0tNGl7Q8kOa0=', 0, N'2020-11-03 09:41:28', 1, N'2020-11-03 09:41:28', 1, 0)
INSERT INTO [dbo].[customers] ([FirstName], [LastName], [Email], [PhoneNumber], [Login], [Password], [IsDisabled], [Created], [CreatedBy], [Updated], [UpdatedBy], [Deleted]) VALUES (N'Customer1', N'Customer1', N'customer1@gmail.com', N'1234567890', N'customer1', N'S3IhMt+U0DxnlYYjC4JbZ8r5p0Zc7N4AQo0LIVezRRs=', 0, N'2020-11-03 09:42:40', 4, N'2020-11-03 09:42:40', 4, 0)

GO

INSERT INTO [dbo].[role_customer] ([CustomerId], [RoleId]) VALUES (2, 3)
INSERT INTO [dbo].[role_customer] ([CustomerId], [RoleId]) VALUES (3, 4)
INSERT INTO [dbo].[role_customer] ([CustomerId], [RoleId]) VALUES (4, 1)
INSERT INTO [dbo].[role_customer] ([CustomerId], [RoleId]) VALUES (5, 2)
