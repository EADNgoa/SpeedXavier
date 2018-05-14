CREATE TABLE [dbo].[Holidays] (
    [HolidayID]   INT           IDENTITY (1, 1) NOT NULL,
    [HDate]       DATE          NULL,
    [HolidayName] VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([HolidayID] ASC)
);

