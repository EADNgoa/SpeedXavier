﻿CREATE TABLE [dbo].[MICE]
(
	[MiceID] INT NOT NULL PRIMARY KEY IDENTITY , 
    [GuestName] VARCHAR(50) NULL, 
    [TDate] DATETIME NULL, 
    [Phone] VARCHAR(50) NULL, 
    [Email] VARCHAR(50) NULL, 
    [AgentName] VARCHAR(100) NULL, 
    [Detail] VARBINARY(350) NULL
)
