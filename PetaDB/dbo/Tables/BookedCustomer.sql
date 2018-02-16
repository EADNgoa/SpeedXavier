CREATE TABLE [dbo].[BookedCustomer]
(
	[BCID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CartID] INT NULL, 
    [BookingID] INT NULL, 
    [CustomerID] INT NULL, 
    CONSTRAINT [FK_BookedCustomer_Cart] FOREIGN KEY ([CartID]) REFERENCES [Cart]([CartID]), 
    CONSTRAINT [FK_BookedCustomer_Booking] FOREIGN KEY ([BookingID]) REFERENCES [Booking]([BookingID]), 
    CONSTRAINT [FK_BookedCustomer_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [Customer]([CustomerID])
)
