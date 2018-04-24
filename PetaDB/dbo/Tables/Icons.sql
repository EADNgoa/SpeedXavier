CREATE TABLE [dbo].[Icons]
(
	[IconId] INT NOT NULL IDENTITY PRIMARY KEY, 
    [ServiceId] INT NOT NULL, 
    [ServiceTypeId] INT NOT NULL, 
    [IconPath] VARCHAR(50) NOT NULL
)
