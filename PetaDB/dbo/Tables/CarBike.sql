CREATE TABLE [dbo].[CarBike]
(
	[CarBikeID] INT NOT NULL PRIMARY KEY, 
    [CarBikeName] VARCHAR(100) NULL, 
    [Description] VARCHAR(MAX) NULL, 
    [NoPax] INT NULL, 
    [NoSmallBags] INT NULL, 
    [NoLargeBags] INT NULL, 
    [HasAc] BIT NULL, 
    [HasCarrier] BIT NULL, 
    [InclHelmet] BIT NULL
)
