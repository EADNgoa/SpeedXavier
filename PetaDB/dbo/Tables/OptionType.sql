CREATE TABLE [dbo].[OptionType]
(
	[OptionTypeID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [OptionTypeName] VARCHAR(100) NULL, 
    [ServiceTypeID] INT NULL
)
