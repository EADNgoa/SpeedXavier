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
    UserName VARCHAR(100),

	Cost  DECIMAL(18,2),
	SellPrice DECIMAL(18,2),
	Qty INT,
	PCost  DECIMAL(18,2),
	PSell  DECIMAL(18,2)
	)

	IF @SRID != 0 
	BEGIN
		INSERT INTO @Service(SRID, SRDID ,CustomerID,UserName, Cost, SellPrice,Qty)
		select srd.SRID,sc.SRDID,sc.CustomerID,CONCAT(c.FName,c.SName) as UserName, srd.Cost, srd.SellPrice, (select count(CustomerID) from SRD_Cust isc where isc.SRDID=sc.SRDID)  as qty
		from SRD_Cust sc
		inner join SRdetails srd on sc.SRDID=srd.SRDID inner join Customer c on c.CustomerID =sc.CustomerID
		where srd.srid=@SRID 
	END


	UPDATE s SET PSell=( s.SellPrice/Qty)  , PCost=( s.Cost/Qty)
	FROM @Service s





	SELECT  UserName,CustomerID,sum(Cost) Cost,sum(SellPrice) SellPrice,sum(PCost) PCost,sum(PSell) PSell FROM @Service Group by UserName,CustomerID

END