CREATE TABLE [dbo].[Employees]
(
	[Id]                INT NOT NULL Identity(1,1), 
    [Salutation]        NVARCHAR(50)    NULL,
    [NickName]          NVARCHAR(35)    NULL,
    [FirstName]         NVARCHAR(35)    NOT NULL,
    [MiddleName]        NVARCHAR(35)    NULL,
    [LastName]          NVARCHAR(35)    NOT NULL,
    [Suffix]            NVARCHAR(50)    NULL,
    [MobileNumber]      NVARCHAR(35)    NULL,
    [Designation]       NVARCHAR(200)   NOT NULL,
    [Branch]            NVARCHAR(100)   NOT NULL,
    [ImagePath]         NVARCHAR(MAX)   NULL DEFAULT NULL,
    [Active]            BIT NOT NULL DEFAULT 1,
	[Deleted]           BIT NOT NULL DEFAULT 0,
    [CreatedBy]         INT NOT NULL, 
    [CreatedAt]         DATETIME NOT NULL,
    [ModifiedBy]        INT NULL,
    [ModifiedAt]        DATETIME NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
)