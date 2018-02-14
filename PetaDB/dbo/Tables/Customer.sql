CREATE TABLE [dbo].[Customer]
(
	[CustomerID] INT NOT NULL PRIMARY KEY IDENTITY, 
	[Name] varCHAR(150) NULL, 
    [Address] varCHAR(350) NULL, 
    [PassportNo] varCHAR(20) NULL, 
    [DateIssue] DATE NULL, 
    [DateExpiry] DATE NULL, 
    [PhotograghID] VARCHAR(100) NULL, 
  
)
