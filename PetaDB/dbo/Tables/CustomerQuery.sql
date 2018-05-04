CREATE TABLE [dbo].[CustomerQuery]
(
	[CustomerQueryID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FName] VARCHAR(50) NULL, 
    [SName] VARCHAR(50) NULL, 
    [Email] VARCHAR(50) NULL, 
    [Phone] VARCHAR(15) NULL, 
    [IdPicture] VARCHAR(100) NULL, 
    [Query] VARCHAR(MAX) NULL, 
    [ServiceID] INT NULL, 
    [ServiceTypeID] INT NULL, 
    [CheckIn] DATETIME NULL, 
    [CheckOut] DATETIME NULL, 
    [NoPax] INT NULL, 
    [Qty] INT NULL, 
    [Tdate] DATETIME NULL, 
    [Glang] INT NULL, 
    [Gtime] VARCHAR(10) NULL

)
