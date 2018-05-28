CREATE TABLE [dbo].[SRdetails]
(
	[SRDID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SRID] INT NULL, 
    [ServiceTypeID] INT NULL, 
    [FromLoc] VARCHAR(50) NULL, 
    [ToLoc] VARCHAR(50) NULL, 
    [Fdate] DATE NULL, 
    [Tdate] DATE NULL, 
    [SupplierID] INT NULL, 
    [Cost] DECIMAL(10, 2) NULL, 
    [SellPrice] DECIMAL(10, 2) NULL, 
    [PNRno] VARCHAR(50) NULL, 
    [TicketNo] VARCHAR(50) NULL, 
    CONSTRAINT [FK_SRdetails_ToSR] FOREIGN KEY ([SRID]) REFERENCES [ServiceRequest]([SRID]), 
    CONSTRAINT [FK_SRdetails_ToSupplier] FOREIGN KEY ([SupplierID]) REFERENCES [Supplier]([SupplierID])
)
