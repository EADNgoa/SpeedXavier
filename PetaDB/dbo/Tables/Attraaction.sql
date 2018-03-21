CREATE TABLE [dbo].[Attraaction]
(
	[AttractionID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AttractionName] VARCHAR(100) NULL, 
    [ImagePath] VARCHAR(50) NULL, 
    [Description] VARCHAR(MAX) NULL
)
