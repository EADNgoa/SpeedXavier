﻿CREATE TABLE [dbo].[CarBike]
(
	[CarBikeID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CarBikeName] VARCHAR(100) NULL, 
	GeoTreeId int not null,
    [Description] VARCHAR(MAX) NULL, 
    [NoPax] INT NULL, 
    [NoSmallBags] INT NULL, 
    [NoLargeBags] INT NULL, 
    [HasAc] BIT NOT NULL DEFAULT 0, 
    [HasCarrier] BIT NOT NULL DEFAULT 0, 
    [InclHelmet] BIT NOT NULL DEFAULT 0, 
    [CouponCode] VARCHAR(50) NULL, 
    [IsBike] BIT NOT NULL, 
    [SupplierNotepad] VARCHAR(MAX) NULL, 
    [SelfOwned] BIT NULL, 
    CONSTRAINT [FK_CarBike_ToGeotree] FOREIGN KEY ([GeoTreeId]) REFERENCES [GeoTree]([GeoTreeId])
)
