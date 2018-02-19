CREATE TABLE [dbo].[CarBike]
(
	[CarBikeID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CarBikeName] VARCHAR(100) NULL, 
	GeoTreeId int not null,
    [Description] VARCHAR(MAX) NULL, 
    [NoPax] INT NULL, 
    [NoSmallBags] INT NULL, 
    [NoLargeBags] INT NULL, 
    [HasAc] BIT NULL, 
    [HasCarrier] BIT NULL, 
    [InclHelmet] BIT NULL, 
    CONSTRAINT [FK_CarBike_ToGeotree] FOREIGN KEY ([GeoTreeId]) REFERENCES [GeoTree]([GeoTreeId])
)
