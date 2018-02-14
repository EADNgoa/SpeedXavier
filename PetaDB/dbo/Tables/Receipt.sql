CREATE TABLE [dbo].[Receipt]
(
	[ReceiptID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TDate] DATE NULL, 
    [Name] VarCHAR(100) NULL, 
    [Amount] decimal(15,2) NULL, 
    [ChequeNo] varCHAR(20) NULL, 
    [ChqDate] DATE NULL, 
    [DrawnOn] VARCHAR(50) NULL, 
    [RoomNo] varchar(10) NULL, 
    [BillNo] INT NULL, 
)
