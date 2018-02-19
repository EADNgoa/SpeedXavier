CREATE FUNCTION [dbo].[GetChildGeos]
(	
	@GeoTreeID int	
)
RETURNS @Results TABLE 
(	GeoTreeID int,
	GeoName varchar(100),
	GeoParentId int,
	GeoLevel int)	
AS
begin
	With KidGeos(GeoTreeID,	GeoName,GeoParentId, GeoLevel)
AS
	(
	Select g.GeoTreeID, g.GeoName,g.GeoParentID, g.GeoLevel
	from GeoTree g where g.GeoTreeID=@GeoTreeID
	UNION ALL
	Select g.GeoTreeID, g.GeoName,g.GeoParentID, g.GeoLevel
	from GeoTree g inner join KidGeos k on g.GeoParentID=k.GeoTreeID
	)
		insert @Results
	select GeoTreeID,	GeoName,GeoParentId, GeoLevel
	from KidGeos

	return;
end