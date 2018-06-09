CREATE TABLE [dbo].[OwnCarTrip]
(
	[OwnCarTripID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CarBikeID] INT NULL, 
    [DriverID] INT NULL, 
    [TripStart] DATETIME NULL, 
    [StartKms] INT NULL, 
    [EndKms] INT NULL, 
    CONSTRAINT [FK_OwnCarTrip_ToCarBike] FOREIGN KEY ([CarBikeID]) REFERENCES [CarBike]([CarBikeID]), 
    CONSTRAINT [FK_OwnCarTrip_ToDriver] FOREIGN KEY ([DriverID]) REFERENCES [Driver]([DriverID])
)
