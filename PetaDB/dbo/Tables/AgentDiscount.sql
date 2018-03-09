CREATE TABLE [dbo].[AgentDiscount]
(
	[AgentDiscountID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserID] NVARCHAR(128) NULL, 
    [ServiceTypeID] INT NULL, 
    [Amount] DECIMAL(18, 2) NULL, 
    [Percentage] DECIMAL(18, 2) NULL, 
    [IsApproved] BIT NULL, 
    CONSTRAINT [FK_AgentDiscount_AspNetUsers] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers]([Id])
)
