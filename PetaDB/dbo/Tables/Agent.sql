CREATE TABLE [dbo].[Agent]
(
	[AgentId] NVARCHAR(128) NOT NULL PRIMARY KEY, 
    [ContactName] VARCHAR(80) NULL,
    [PhoneNo] VARCHAR(50) NULL, 
    [Email] VARCHAR(50) NULL, 
    [Address] VARCHAR(150) NULL, 
    [PAN] VARCHAR(10) NULL, 
    [GST] VARCHAR(50) NULL, 
    [RCbook] VARCHAR(50) NULL, 
    [BkAccNo] VARCHAR(30) NULL, 
    [BkName] VARCHAR(50) NULL, 
    [BkIFSC] VARCHAR(15) NULL, 
    [BkAddress] VARCHAR(150) NULL, 
    [State] VARCHAR(50) NULL, 
    [CreditAmt] DECIMAL(18, 2) NULL DEFAULT 0, 
    CONSTRAINT [FK_Agent_ToAspNetUsers] FOREIGN KEY ([AgentId]) REFERENCES [AspNetUsers]([Id])
)
