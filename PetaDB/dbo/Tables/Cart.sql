CREATE TABLE [dbo].[Cart]
(
	[CartID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Id] NVARCHAR(128) NULL, 
    [ServiceID] INT NULL, 
    [ServiceTypeID] INT NULL, 
    [OptionTypeID] INT NULL, 
    [Qty] INT NULL, 
    [CheckIn] DATETIME NULL, 
    [CheckOut] DATETIME NULL, 
    [NoOfGuest] INT NULL, 
    [OrigPrice] DECIMAL(18, 2) NULL, 
    [CouponCode] VARCHAR(MAX) NULL, 
    [DiscountedPrice] DECIMAL(18, 2) NULL, 
    CONSTRAINT [FK_Cart_AspNetUsers] FOREIGN KEY ([Id]) REFERENCES [AspNetUsers]([Id])
)
