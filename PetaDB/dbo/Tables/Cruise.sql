CREATE TABLE [dbo].[Cruise]
(
	[CruiseID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CruiseName] VARCHAR(100) NULL, 
    [Description] VARCHAR(MAX) NULL, 
    [Duration] INT NULL, 
    [Itinerary] VARCHAR(MAX) NULL, 
    [StarRating] INT NULL
)
