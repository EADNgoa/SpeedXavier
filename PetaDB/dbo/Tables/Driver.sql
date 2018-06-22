CREATE TABLE [dbo].[Driver]
(
	[DriverID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DriverName] VARCHAR(100) NULL, 
    [Phone] VARCHAR(15) NULL, 
    [Address] VARCHAR(250) NULL, 
    [EmerContactName] VARCHAR(50) NULL, 
    [EmerContactNo] VARCHAR(15) NULL, 
    [CarModel] VARCHAR(50) NULL, 
    [LocationId] INT NULL, 
    CONSTRAINT [FK_Driver_ToGeoTree] FOREIGN KEY ([LocationId]) REFERENCES GeoTree([GeoTreeId])
)
