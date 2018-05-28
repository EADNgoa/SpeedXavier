CREATE TABLE [dbo].[Reminders]
(
	[ReminderID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SRDID] INT NULL, 
    [Reminder] DATETIME NULL, 
    [Note] VARCHAR(MAX) NULL,
	 CONSTRAINT [FK_Reminder_ToSrdets] FOREIGN KEY ([SRDID]) REFERENCES [SRdetails]([SRDID])

)
