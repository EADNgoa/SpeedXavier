CREATE TABLE [dbo].[InsectLog]
(
	[CritterTime] DATETIME NOT NULL PRIMARY KEY, 
    [Controller] VARCHAR(50) NULL, 
    [Action] VARCHAR(50) NULL, 
    [Message] VARCHAR(380) NULL, 
    [Stack] VARCHAR(MAX) NULL
)
