CREATE TABLE [dbo].[Payments]
(
	[PaymentID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ChequeNo] VARCHAR(50) NULL, 
    [TDate] DATE NULL, 
    [Amount] DECIMAL(18, 2) NULL, 
    [TransactionNo] VARCHAR(50) NULL, 
    [Note] VARCHAR(MAX) NULL, 
    [Type] INT NULL, 
	[DriverID] INT Null,
	[BankID] INT NULL, 
    [AgentId] NVARCHAR(128) NULL, 
    [SupplierID] INT NULL, 
    CONSTRAINT [FK_Payments_ToBanks] FOREIGN KEY ([BankID]) REFERENCES [Banks]([BankID]), 
    CONSTRAINT [FK_Payments_Driver] FOREIGN KEY ([DriverID]) REFERENCES [Driver]([DriverID]),
	CONSTRAINT [FK_Payments_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [Supplier]([SupplierID]), 
    CONSTRAINT [FK_Payments_Agent] FOREIGN KEY ([AgentId]) REFERENCES [Agent]([AgentId])
)
