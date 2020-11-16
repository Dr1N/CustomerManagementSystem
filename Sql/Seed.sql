USE Customers303

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
	'+380963128945',
	'admin',
	'+TFHNvcqJxrWTHki4Pi50RtCsrfOfULOtdX6xQnbes4='
);

GO

-- Admin roles

INSERT INTO [dbo].[role_customer] VALUES('1', '1');
INSERT INTO [dbo].[role_customer] VALUES('1', '2');
INSERT INTO [dbo].[role_customer] VALUES('1', '3');
INSERT INTO [dbo].[role_customer] VALUES('1', '4');
