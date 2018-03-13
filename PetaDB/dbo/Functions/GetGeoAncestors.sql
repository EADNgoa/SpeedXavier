CREATE FUNCTION [dbo].[GetGeoAncestors]
(	
	@GeoTreeID int	
)
RETURNS @Results TABLE 
(	GeoTreeID int,
	GeoName varchar(100),
	GeoParentId int)	
AS
begin
	
	DECLARE @ParentId as Int

	Select @ParentId=GeoParentID From GeoTree where GeoTreeID=@GeoTreeID

	IF (COALESCE(@ParentId,0) >0)
	BEGIN		
		INSERT @Results		
		Select g.GeoTreeID, g.GeoName, g.GeoParentId from GeoTree g where g.GeoTreeID=@ParentId
		union 
		Select * from GetGeoAncestors(@ParentId)	
	END
	
	return;
end