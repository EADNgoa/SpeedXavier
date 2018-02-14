CREATE TABLE [dbo].[Voucher]
(
	
	[VoucherID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TDate] DATE NULL, 
    [PayTo] varCHAR(100) NULL, 
    [Amount] decimal(15,2) NULL, 
    [OnAccountOf] varCHAR(350) NULL, 
    [ChequeNo] varCHAR(20) NULL, 
    [DrawnOn] VARCHAR(50) NULL    
)
