CREATE TABLE [dbo].[Visa]
(
	[VisaID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [VisaCountry] VARCHAR(50) NULL, 
    [FlagPicture] VARCHAR(100) NULL, 
    [EmbassyAddress] VARBINARY(MAX) NULL, 
    [Details] VARCHAR(50) NULL
)
