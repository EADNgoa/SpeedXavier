CREATE TABLE [dbo].[UserFunctions]
(
	[FunctionID] INT NOT NULL IDENTITY PRIMARY KEY, 
    [FunctionName] VARCHAR(150) NOT NULL, 
    [Module] VARCHAR(50) NOT NULL
)
