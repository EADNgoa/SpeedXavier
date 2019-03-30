CREATE TABLE [dbo].[DriversCars]
(
	[CarId] INT NOT NULL Identity PRIMARY KEY, 
    [DriverId] INT NOT NULL,
    [CarBrand] VARCHAR(50) NULL, 
    [Model] VARCHAR(50) NULL, 
    [DateOfPurchase] DATE NULL, 
    [RCBookNo] VARCHAR(50) NULL, 
    [PlateNo] VARCHAR(15) NULL, 
    [InsuranceEndDate] DATE NULL, 
    [InsuranceCompany] VARCHAR(50) NULL, 
    CONSTRAINT [FK_DriversCars_ToDriver] FOREIGN KEY ([DriverId]) REFERENCES Driver(DriverID)
)
