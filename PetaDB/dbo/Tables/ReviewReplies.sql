CREATE TABLE [dbo].[ReviewReplies]
(
	[ReviewReplyID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ReviewID] INT NULL, 
    [UserID] NVARCHAR(128) NULL, 
    [ReviewDate] DATETIME NULL, 
    [Reply] VARCHAR(MAX) NULL, 
    [IsVisible] BIT NULL, 
    CONSTRAINT [FK_ReviewReplies_Review] FOREIGN KEY ([ReviewID]) REFERENCES [Review]([ReviewID]), 
    CONSTRAINT [FK_ReviewReply_AspNetUsers] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers]([Id]), 
)
