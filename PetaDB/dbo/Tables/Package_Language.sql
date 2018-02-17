CREATE TABLE [dbo].[Package_Language]
(
	[PackageId] INT NOT NULL, 
    [GuideLanguageId] INT NOT NULL, 
    CONSTRAINT [FK_Package_Language_ToGuideLanguage] FOREIGN KEY ([GuideLanguageId]) REFERENCES [GuideLanguage]([GuideLanguageId]),
	CONSTRAINT [FK_Package_Language_ToPackage] FOREIGN KEY ([PackageId]) REFERENCES [Package]([PackageId])
)
