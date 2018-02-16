CREATE TABLE [dbo].[ServiceDiscount]
(
	[SDID] INT NOT NULL PRIMARY KEY iDENTITY, 
    [ServiceID] INT NULL, 
    [ServiceTypeID] INT NULL, 
    [CouponCode] VARCHAR(50) NULL
)
