CREATE TABLE [dbo].[Package_Language]
(
	[PackageId] INT NOT NULL, 
    [GuideLanguageId] INT NOT NULL, 
    CONSTRAINT [PK_Package_Language] PRIMARY KEY ([PackageId], [GuideLanguageId])
    
)
