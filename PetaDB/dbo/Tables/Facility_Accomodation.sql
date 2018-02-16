CREATE TABLE [dbo].[Facility_Accomodation]
(
	[FacilityID] INT NOT NULL, 
    [AccomodationID] INT NOT NULL,
    CONSTRAINT [PK_dbo.FacilityAccom] PRIMARY KEY CLUSTERED ([FacilityID] ASC, [AccomodationID] ASC),

)
