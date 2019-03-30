CREATE TABLE [dbo].[SRReciepts] (
    [RecieptID]   INT             IDENTITY (1, 1) NOT NULL,
    [SRID]        INT             NULL,
    [RecieptDate] DATE            NULL,
    [Amount]      DECIMAL (10, 2) NULL,
    [PayMode]     INT             NULL,
    [BankID]      INT             NULL,
    PRIMARY KEY CLUSTERED ([RecieptID] ASC),
    CONSTRAINT [FK_SRReciepts_SR] FOREIGN KEY ([SRID]) REFERENCES [dbo].[ServiceRequest] ([SRID]),
    CONSTRAINT [FK_SRReciepts_bank] FOREIGN KEY ([BankID]) REFERENCES [dbo].[Banks] ([BankID])
);

