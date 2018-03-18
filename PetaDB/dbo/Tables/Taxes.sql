CREATE TABLE [dbo].[Taxes]
(
	[TaxId] INT NOT NULL Identity PRIMARY KEY, 
    [ServiceTypeId] INT NOT NULL,
	[WEF] DATE NULL, 
    [Percentage] DECIMAL(15, 2) NOT NULL DEFAULT 0, 
    [TaxName] VARCHAR(50) NOT NULL, 
)
