CREATE TABLE [dbo].[Refunds]
(
	[RefundId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SRDID] INT NULL, 
    [ProdCanxCost] DECIMAL(18, 2) NULL, 
    [SBCanxCost] DECIMAL(18, 2) NULL, 
    [Note] VARCHAR(250) NULL, 
    CONSTRAINT [FK_Refunds_SRdetails] FOREIGN KEY ([SRDID]) REFERENCES [SRdetails]([SRDID])
)
