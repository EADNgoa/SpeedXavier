CREATE TABLE [dbo].[BankAccount]
(
	[BankAccountID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TDate]         DATE        NULL,
    [AmountIn] DECIMAL(10, 2) NULL, 
    [AmountOut] DECIMAL(10, 2) NULL, 
    [SRID] NCHAR(10) NULL, 
    [TransNo] VARCHAR(50) NULL, 
    [UserID] NVARCHAR(128) NULL, 
    [SupplierID] INT NULL, 
    [Comment] VARCHAR(100) NULL, 
    [BankID] INT NULL, 
    CONSTRAINT [FK_BankAccount_ToAspNetUsers] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers]([Id]), 
    CONSTRAINT [FK_BankAccount_ToSupplier] FOREIGN KEY ([SupplierID]) REFERENCES [Supplier]([SupplierID]), 
    CONSTRAINT [FK_BankAccount_ToBank] FOREIGN KEY ([BankID]) REFERENCES [Banks]([BankID])
)
