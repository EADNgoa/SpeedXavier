CREATE PROCEDURE [dbo].[AmtPerPax]
(
	@SRID int 	= 0
)
AS
BEGIN

--Fetch Loan details
	DECLARE @Service TABLE(
	SRID INT,	
	SRDID INT,
	CustomerID INT,
	Cost  DECIMAL(18,2),
	SellPrice DECIMAL(18,2),
	Qty INT,
	PCost  DECIMAL(18,2),
	PSell  DECIMAL(18,2)
	)

	IF @SRID != 0 
	BEGIN
		INSERT INTO @Service(SRID, SRDID ,CustomerID, Cost, SellPrice,Qty)
		select srd.SRID,sc.SRDID,sc.CustomerID, srd.Cost, srd.SellPrice, (select count(CustomerID) from SRD_Cust isc where isc.SRDID=sc.SRDID)  as qty
		from SRD_Cust sc
		inner join SRdetails srd on sc.SRDID=srd.SRDID 
		where srd.srid=@SRID 
	END


	UPDATE s SET PSell=( s.SellPrice/Qty)  , PCost=( s.Cost/Qty)
	FROM @Service s

	Select CONCAT(c.FName,c.SName) as UserName,tmpServ.CustomerID,Cost, SellPrice,PCost,PSell from
	(SELECT  CustomerID,sum(Cost) Cost,sum(SellPrice) SellPrice,sum(PCost) PCost,sum(PSell) PSell FROM @Service  
	Group by CustomerID) as tmpServ 
	inner join Customer c on c.CustomerID =tmpServ.CustomerID

END