CREATE TABLE [dbo].[OwnAssetBill]
(
	[OwnAssetBillID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ServiceTypeID] INT NULL, 
    [ServiceID] INT NULL, 
    [BillDate] DATE NULL, 
    [BillNo] VARCHAR(15) NULL, 
    [Amount] DECIMAL(10, 2) NULL, 
    [BillImage] VARCHAR(50) NULL
)
