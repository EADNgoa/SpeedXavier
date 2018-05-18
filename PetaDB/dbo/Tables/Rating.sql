CREATE TABLE [dbo].[Rating]
(
	[RatingID] INT NOT NULL PRIMARY KEY Identity, 
    [Value] INT NULL, 
    [SRDID] INT NULL, 
    [Note] VARCHAR(MAX) NULL,
    CONSTRAINT [FK_Ratings_ToSrdets] FOREIGN KEY ([SRDID]) REFERENCES [SRdetails]([SRDID])

)
