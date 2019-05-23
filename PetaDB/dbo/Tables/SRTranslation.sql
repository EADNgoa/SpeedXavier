CREATE TABLE [dbo].SRTranslation
(
	SRTranslationId INT NOT NULL PRIMARY KEY IDENTITY, 
    [ServiceTypeId] INT NOT NULL,
    [ColumnName] VARCHAR(50) NULL,
	[FriendlyName] VARCHAR(100) NULL
)
