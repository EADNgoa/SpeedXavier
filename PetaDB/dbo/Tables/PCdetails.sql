CREATE TABLE [dbo].[PCdetails]
(
	[PCDID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PettyCashID] INT NULL, 
    [Category] VARCHAR(50) NULL, 
    [Details] VARCHAR(300) NULL, 
    [Cost] DECIMAL(18, 2) NULL, 
    [SupplierID] INT NULL, 
    [InvoiceNo] VARCHAR(50) NULL, 
    [BillImage] VARCHAR(50) NULL, 
    [SRID] INT NULL, 
    CONSTRAINT [FK_PCdetails_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [Supplier]([SupplierID]), 
    CONSTRAINT [FK_PCdetails_SR] FOREIGN KEY ([SRID]) REFERENCES [ServiceRequest]([SRID]), 
    CONSTRAINT [FK_PCdetails_PC] FOREIGN KEY ([PettyCashID]) REFERENCES [PettyCash]([CashInHandRegID])
)
