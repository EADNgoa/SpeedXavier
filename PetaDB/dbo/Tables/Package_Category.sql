CREATE TABLE [dbo].[Package_Category]
(
	[PackageID] INT NOT NULL , 
    [CategoryID] INT NOT NULL,
 CONSTRAINT [PK_dbo.PackCat] PRIMARY KEY CLUSTERED ([PackageID] ASC, [CategoryID] ASC),

)
