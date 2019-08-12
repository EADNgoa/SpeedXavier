CREATE TABLE [dbo].[AtomRefundLogs]
(
	[RefundId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Tdate] DATETIME NULL, 
    [SRID] INT NULL, 
    [SRDID] INT NULL, 
    [UserId] NVARCHAR(128) NULL, 
    [RefundAmt] VARCHAR(50) NULL, 
    [TransactionDate] DATE NULL, 
    [AtomTxnId] VARCHAR(100) NULL, 
    [MerchantReferanceId] VARCHAR(50) NULL, 
    [AtomRefundId] VARCHAR(100) NULL, 
    [StatusCode] VARCHAR(50) NULL, 
    [StatusMessege] VARCHAR(100) NULL,
	 CONSTRAINT [FK_RefundLogs_ToSR] FOREIGN KEY ([SRID]) REFERENCES [dbo].[ServiceRequest] ([SRID]),
	CONSTRAINT [FK_RefundLogs_ToWebUser] FOREIGN KEY ([UserID]) REFERENCES [dbo].[AspNetUsers] ([Id]),
	CONSTRAINT [FK_RefundLogs_ToSRdetails] FOREIGN KEY ([SRDID]) REFERENCES [dbo].[SRdetails] ([SRDID])

)
