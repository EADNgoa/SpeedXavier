CREATE TABLE [dbo].[PettyCash] (
    [CashInHandRegID]    INT             IDENTITY (1, 1) NOT NULL,
    [Tdate]              DATETIME        NULL,
    [NameAndDesg]        VARCHAR (150)   NULL,
    [CashToDeclareStart] DECIMAL (18, 2) NULL,
    [DetailsOfCashExp]   VARCHAR (250)   NULL,
    [CashToDeclareEnd]   DECIMAL (18, 2) NULL,
    [Remarks]            VARCHAR (300)   NULL,
    PRIMARY KEY CLUSTERED ([CashInHandRegID] ASC)
);

