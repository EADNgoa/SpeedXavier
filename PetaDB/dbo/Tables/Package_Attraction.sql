CREATE TABLE [dbo].[Package_Attraction]
(
	[PackageID] INT NOT NULL , 
    [AttractionID] INT NOT NULL,
	    CONSTRAINT [PK_dbo.PackAttrac] PRIMARY KEY CLUSTERED ([PackageID] ASC, [AttractionID] ASC),

)
