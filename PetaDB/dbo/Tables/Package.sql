CREATE TABLE [dbo].[Package]
(
	[PackageID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ServiceTypeID] INT NULL, 
    [PackageName] VARCHAR(100) NULL, 
    [Description] VARCHAR(MAX) NULL, 
    [Duration] INT NULL, 
    [Itinerary] VARCHAR(MAX) NULL, 
    [Dificulty] INT NULL, 
    [GroupSize] INT NULL, 
    [GuideLanguageID] INT NULL, 
    [StartTime] VARCHAR(300) NULL, 
    CONSTRAINT [FK_Package_GuideLanguage] FOREIGN KEY ([GuideLanguageID]) REFERENCES [GuideLanguage]([GuideLanguageID])
)
