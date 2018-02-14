CREATE TABLE [dbo].[GeoTree]
(
	[GeoTreeID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [GeoName] VARCHAR(150) NULL, 
    [GeoParentID] INT NULL, 
    [GeoLevel] INT NULL
)
