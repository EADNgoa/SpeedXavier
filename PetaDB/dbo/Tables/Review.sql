CREATE TABLE [dbo].[Review]
(
	[ReviewID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserID] NVARCHAR(128) NULL, 
    [ServiceID] INT NULL, 
    [ServiceTypeID] INT NULL, 
    [ReviewDate] DATETIME NULL, 
    [Review] VARCHAR(MAX) NULL, 
    [IsVisible] BIT NULL,
    CONSTRAINT [FK_Review_AspNetUsers] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers]([Id]), 

)
