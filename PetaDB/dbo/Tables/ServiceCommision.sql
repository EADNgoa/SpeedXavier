CREATE TABLE [dbo].[ServiceCommision]
(
	[ServiceID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ServiceName] VARCHAR(50) NULL, 
    [Amount] DECIMAL(18, 2) NULL, 
    [Perc] DECIMAL(18, 2) NULL
)
