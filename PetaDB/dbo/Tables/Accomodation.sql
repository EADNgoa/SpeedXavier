CREATE TABLE [dbo].[Accomodation]
(
	[AccomodationID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AccomName] VARCHAR(150) NULL, 
    [Description] VARCHAR(MAX) NULL, 
    [GeoTreeID] INT NULL,     
    [lat] VARCHAR(50) NULL, 
    [longt] VARCHAR(50) NULL, 
    [CouponCode] VARCHAR(50) NULL, 
    [SupplierNotepad] VARCHAR(MAX) NULL, 
    [SelfOwned] BIT NULL, 
    CONSTRAINT [FK_Accomodation_GeoTree] FOREIGN KEY ([GeoTreeID]) REFERENCES [GeoTree]([GeoTreeID])
)
