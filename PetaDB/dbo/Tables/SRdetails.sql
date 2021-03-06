﻿CREATE TABLE [dbo].[SRdetails]
(
	[SRDID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SRID] INT NULL, 
    [ServiceTypeID] INT NULL, 
	[CarType] INT NULL,
    [ItemID] INT NULL,
	[CouponCode] VARCHAR(100) NULL,
	[Model] VARCHAR(50) NULL, 
    [FromLoc] VARCHAR(50) NULL, 
    [ToLoc] VARCHAR(50) NULL, 
    [Fdate] DATETIME NULL, 
    [Tdate] DATETIME NULL, 
    [SupplierID] INT NULL, 
    [Cost] DECIMAL(10, 2) NULL, 
    [SellPrice] DECIMAL(10, 2) NULL, 
    [ChildNo] INT NULL, 
    [AdultNo] INT NULL, 
    [InfantNo] INT NULL, 
    [Heritage] VARCHAR(50) NULL, 
    [HasAc] BIT NOT NULL DEFAULT 0, 
    [HasCarrier] BIT NOT NULL DEFAULT 0, 
    [GuideLanguageID] INT NULL, 
    [DateOfIssue] DATE NULL, 
    [ContractNo] VARCHAR(50) NULL, 
    [PayTo] VARCHAR(50) NULL, 
    [PickUpPoint] VARCHAR(100) NULL, 
    [DropPoint] VARCHAR(100) NULL,   
    [DriverID] INT NULL, 
	[SuppInvNo] VARCHAR(50) NULL, 
	[Qty] DECIMAL(18, 2) NULL, 
    [ParentID] INT NULL, 
    [IsReturn] BIT NOT NULL DEFAULT 0, 
    [IsInternational] BIT NOT NULL DEFAULT 0, 
    [OptionTypeID] INT NULL, 
    [ECommision] DECIMAL(18, 2) NULL, 
    [IsCanceled] BIT NOT NULL DEFAULT 0, 
    [SuppInvDt] DATE NULL, 
    [SuppConfNo] VARCHAR(50) NULL, 
    [NoExtraBeds] INT NULL,     
    [BFCost] INT NULL, 
    [LunchCost] INT NULL, 
    [DinnerCost] INT NULL, 
    [NoExtraService] INT NULL, 
    [ExtraServiceCost] INT NULL, 
    [SuppInvAmt] INT NULL, 
    [GDSConfNo] VARCHAR(50) NULL, 
    [Tax] DECIMAL(10, 2) NULL, 
    [ExpiryDate] DATE NULL, 
    [EBCostPNight] DECIMAL(10, 2) NULL, 
    [Name] VARCHAR(50) NULL, 
    [AdditionalDetails] VARCHAR(50) NULL, 
    [PaymentID] INT NULL, 
    [IsCancelled] BIT NULL, 
    [AgentPaymentId] INT NULL, 
    [CancelledSupplierPaymentId] INT NULL, 
    [CancelledAgentPaymentId] INT NULL, 
    [SupplierPaymentId] INT NULL, 
    [Commision] DECIMAL(18, 2) NULL, 
    CONSTRAINT [FK_SRdetails_ToSR] FOREIGN KEY ([SRID]) REFERENCES [ServiceRequest]([SRID]), 
    CONSTRAINT [FK_SRdetails_ToSupplier] FOREIGN KEY ([SupplierID]) REFERENCES [Supplier]([SupplierID]), 
    CONSTRAINT [FK_SRdetails_ToGuideLanguage] FOREIGN KEY ([GuideLanguageID]) REFERENCES [GuideLanguage]([GuideLanguageID]), 
	CONSTRAINT [FK_SRdetails_OptionType] FOREIGN KEY ([OptionTypeID]) REFERENCES [OptionType]([OptionTypeID]), 
    CONSTRAINT [FK_SRdetails_ToDriver] FOREIGN KEY ([DriverID]) REFERENCES [Driver]([DriverID]), 
    CONSTRAINT [FK_SRdetails_PaymentsAP] FOREIGN KEY ([AgentPaymentId]) REFERENCES [Payments]([PaymentID]),
	CONSTRAINT [FK_SRdetails_PaymentsSP] FOREIGN KEY ([SupplierPaymentId]) REFERENCES [Payments]([PaymentID]),
	CONSTRAINT [FK_SRdetails_PaymentsACP] FOREIGN KEY ([CancelledAgentPaymentId]) REFERENCES [Payments]([PaymentID]),
	CONSTRAINT [FK_SRdetails_PaymentsSCP] FOREIGN KEY ([CancelledSupplierPaymentId]) REFERENCES [Payments]([PaymentID])
)
