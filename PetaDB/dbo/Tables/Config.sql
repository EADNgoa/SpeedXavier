CREATE TABLE [dbo].[Config]
(
	[ConfigId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProductId] VARCHAR(50) NULL, 
    [TransServiceCharge] DECIMAL NULL, 
    [MerchantId] VARCHAR(50) NULL, 
    [Pwd] VARCHAR(50) NULL
)
