CREATE TABLE [dbo].[Booking]
(
	[BookingID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserID] NVARCHAR(128) NULL, 
    [BookDate] DATE NULL, 
    [StatusID] INT NULL, 
    CONSTRAINT [FK_Booking_AspNetUsers] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers]([Id]), 
    CONSTRAINT [FK_Booking_BookingStatus] FOREIGN KEY ([StatusID]) REFERENCES [BookingStatus]([BookingStatusID])
)
