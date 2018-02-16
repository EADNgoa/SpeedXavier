CREATE TABLE [dbo].[DiscountCoupon]
(
	[DiscountCouponID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CouponCode] VARCHAR(50) NULL, 
    [ValidFrom] DATE NULL, 
    [ValidTo] DATE NULL, 
    [Amount] DECIMAL(18, 2) NULL, 
    [Perc] DECIMAL(18, 2) NULL
)
