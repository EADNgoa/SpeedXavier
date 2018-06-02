CREATE TABLE [dbo].[SRReciepts]
(
	[RecieptID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SRID] INT NULL, 
    [RecieptDate] DATE NULL, 
    [Amount] DECIMAL(10, 2) NULL, 
    [PayMode] INT NULL, 
    [BankID] INT NULL, 
    CONSTRAINT [FK_SRReciepts_SR] FOREIGN KEY ([SRID]) REFERENCES [ServiceRequest]([SRID]), 
    CONSTRAINT [FK_SRReciepts_bank] FOREIGN KEY ([BankID]) REFERENCES [Banks]([BankID])
)
