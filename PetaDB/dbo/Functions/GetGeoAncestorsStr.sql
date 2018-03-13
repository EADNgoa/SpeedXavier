CREATE FUNCTION [dbo].[GetGeoAncestorsStr]
(
	@GeoTreeId int
)
RETURNS varchar(100)
AS
BEGIN
	DECLARE @histor varchar(100)
	SET @histor=' '
	DECLARE @ParentId as Int

	Select @ParentId=GeoParentID From GeoTree where GeoTreeID=@GeoTreeID

	IF (COALESCE(@ParentId,0) >0)
	BEGIN
		Select @histor= @histor + g.GeoName + ', ' + dbo.GetGeoAncestorsStr(@ParentId)  from GeoTree g where g.GeoTreeID=@ParentId	
		
	END
	
	return @histor

END