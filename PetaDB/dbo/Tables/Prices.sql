﻿CREATE TABLE [dbo].[Prices]
(
	[PriceID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ServiceID] INT NULL, 
    [OptionTypeID] INT NULL, 
    [WEF] DATE NULL, 
    [Price] DECIMAL(18, 2) NULL, 
    [WeekendPrice] DECIMAL(18, 2) NULL
)
