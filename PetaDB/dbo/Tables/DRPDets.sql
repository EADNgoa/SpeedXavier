CREATE TABLE [dbo].[DRPdets]
(
	[DRPDID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BankName] INT NULL, 
    [ChequeNo] VARCHAR(50) NULL, 
    [Date] DATE NULL, 
    [Amount] DECIMAL(18, 2) NULL, 
    [TransactionNo] VARCHAR(50) NULL, 
    [Note] VARCHAR(MAX) NULL, 
    [SRID] INT NULL, 
    [Type] INT NULL, 
    [AmtUsed] BIT NULL, 
    [IsPayment] BIT NULL, 
	    [Cdate] DATE NULL, 

    CONSTRAINT [FK_DRPdets_ToSR] FOREIGN KEY ([SRID]) REFERENCES [ServiceRequest]([SRID])
)
