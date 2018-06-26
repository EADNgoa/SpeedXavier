CREATE TABLE [dbo].[FeedBack]
(
	[FeedBackID] INT NOT NULL PRIMARY KEY, 
    [QuestionID] INT NULL, 
    [SRDID] INT NULL, 
    [StarRating] INT NULL, 
    CONSTRAINT [FK_FeedBack_ToFeedback] FOREIGN KEY ([QuestionID]) REFERENCES [Questions]([QuestionID]), 
    CONSTRAINT [FK_FeedBack_ToSRdetails] FOREIGN KEY ([SRDID]) REFERENCES [SRdetails]([SRDID])
)
