CREATE TABLE [dbo].[PriceInclusions]
(
	[PriceInclusionId] INT NOT NULL Identity PRIMARY KEY, 
    [PriceId] INT NOT NULL, 
    [Amount] DECIMAL(15, 2) NOT NULL DEFAULT 0, 
    [Description] VARCHAR(100) NULL, 
    [MealPlanId] INT NULL, 
    CONSTRAINT [FK_PriceInclusions_ToPrice] FOREIGN KEY ([PriceId]) REFERENCES [Prices]([PriceId])
)
