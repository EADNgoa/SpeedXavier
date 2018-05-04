CREATE TABLE [dbo].[CustQueryReply]
(
    [CustQueryReplyID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CustomerQueryID] INT NULL, 
    [ReplyDate] DATETIME NULL, 
    [Reply] VARCHAR(MAX) NULL, 
    CONSTRAINT [FK_CustQueryReply_QueryReply] FOREIGN KEY ([CustomerQueryID]) REFERENCES [CustomerQuery]([CustomerQueryID]),  
)
