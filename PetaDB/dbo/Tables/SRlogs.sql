﻿CREATE TABLE [dbo].[SRlogs]
(
	[SRLID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SRDID] INT NULL, 
    [LogDateTime] DATETIME NULL, 
    [UserID] NVARCHAR(128) NULL, 
    [Type] BIT NULL, 
    [Event] VARCHAR(MAX) NULL, 
    CONSTRAINT [FK_SRlogs_ToSrdets] FOREIGN KEY ([SRDID]) REFERENCES [SRdetails]([SRDID]),
	CONSTRAINT [FK_SRlogs_ToUser] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers]([Id])

)
