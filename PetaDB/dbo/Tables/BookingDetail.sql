CREATE TABLE [dbo].[BookingDetail]
(
	[BookingDetailID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BookingID] INT NULL, 
    [ServiceID] INT NULL, 
    [ServiceTypeID] INT NULL, 
    [OptionTypeID] INT NULL, 
    [Qty] INT NULL, 
    [CheckIn] DATETIME NULL, 
    [CheckOut] DATETIME NULL, 
    [NoOfGuests] INT NULL, 
    [Price] DECIMAL(18, 2) NULL, 
    [BlockedReason] VARCHAR(200) NULL, 
    CONSTRAINT [FK_BookingDetail_Booking] FOREIGN KEY ([BookingID]) REFERENCES [Booking]([BookingID])
)
