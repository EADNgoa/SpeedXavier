CREATE TABLE [dbo].[Customer]
(
	[CustomerID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FName] VARCHAR(50) NULL, 
    [SName] VARCHAR(50) NULL, 
    [Email] VARCHAR(50) NULL, 
    [Phone] VARCHAR(15) NULL, 
    [IdPicture] VARCHAR(100) NULL, 
    [Type] VARCHAR(50) NULL
)
