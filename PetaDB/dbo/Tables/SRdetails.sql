CREATE TABLE [dbo].[SRdetails]
(
	[SRDID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SRID] INT NULL, 
    [ServiceTypeID] INT NULL, 
	[CarType] INT NULL,
	[CouponCode] VARCHAR(100) NULL,
	[Airline] VARCHAR(100) NULL, 

		    [Model] VARCHAR(50) NULL, 


    [FromLoc] VARCHAR(50) NULL, 
    [ToLoc] VARCHAR(50) NULL, 
    [Fdate] DATETIME NULL, 
    [Tdate] DATETIME NULL, 
    [SupplierID] INT NULL, 
    [Cost] DECIMAL(10, 2) NULL, 
    [SellPrice] DECIMAL(10, 2) NULL, 
    [PNRno] VARCHAR(50) NULL, 
    [TicketNo] VARCHAR(50) NULL, 
    [ChildNo] INT NULL, 
    [AdultNo] INT NULL, 
    [infantNo] INT NULL, 
    [RoomType] VARCHAR(50) NULL, 
    [City] VARCHAR(50) NULL, 

    [Heritage] VARCHAR(50) NULL, 
    [HasAc] BIT NULL, 
    [HasCarrier] BIT NULL, 
    [GuideLanguageID] INT NULL, 
    [DateOfIssue] DATE NULL, 
    [ContractNo] VARCHAR(50) NULL, 
    [PayTo] VARCHAR(50) NULL, 
    [RateBasis] VARCHAR(50) NULL, 
    [PickUpPoint] VARCHAR(100) NULL, 
    [DropPoint] VARCHAR(100) NULL, 
  
    [DriverID] INT NULL, 
    [FlightNo] VARCHAR(50) NULL, 
	    [SuppInvNo] VARCHAR(50) NULL, 

    [ParentID] INT NULL, 
    [IsReturn] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_SRdetails_ToSR] FOREIGN KEY ([SRID]) REFERENCES [ServiceRequest]([SRID]), 
    CONSTRAINT [FK_SRdetails_ToSupplier] FOREIGN KEY ([SupplierID]) REFERENCES [Supplier]([SupplierID]), 
    CONSTRAINT [FK_SRdetails_ToGuideLanguage] FOREIGN KEY ([GuideLanguageID]) REFERENCES [GuideLanguage]([GuideLanguageID]), 
    CONSTRAINT [FK_SRdetails_ToDriver] FOREIGN KEY ([DriverID]) REFERENCES [Driver]([DriverID])
)
