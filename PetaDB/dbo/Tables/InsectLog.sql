CREATE TABLE [dbo].[InsectLog]
(
	[CritterTime] DATETIME NOT NULL PRIMARY KEY, 
    [Controller] VARCHAR(50) NULL, 
    [Action] VARCHAR(50) NULL, 
    [Message] VARCHAR(MAX) NULL, 
    [Stack] VARCHAR(MAX) NULL
)
