﻿CREATE TABLE [dbo].[UserLogRec]
(
	[UserLogID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserID] NVARCHAR(128) NULL, 
    [LogIn] DATETIME NULL, 
    [LogOut] DATETIME NULL, 
    CONSTRAINT [FK_UserLogRec_ToUserLogRec] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers]([Id])
)