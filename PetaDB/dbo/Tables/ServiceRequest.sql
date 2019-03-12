CREATE TABLE [dbo].[ServiceRequest]
(
	[SRID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BookingNo]        INT            NULL,
	[CustID] INT NULL, 
	[UserID] NVARCHAR(128) NULL, 
    [SRStatusID] INT NULL, 
    [EmpID] NVARCHAR(128) NULL, 
    [EnquirySource] INT NULL, 
    [PayStatusID] INT            NULL,
	[BookingTypeID] INT NULL, 
    [AgentID] NVARCHAR(128) NULL, 
    [ServiceTypeID] INT NULL, 
    [TDate] DATETIME NULL, 
    [IgnoreReason] VARCHAR(300) NULL, 
    [RemindAt] DATETIME NULL, 
    [AgentBooker] VARCHAR(50) NULL, 
    CONSTRAINT [FK_ServiceRequest_ToCust] FOREIGN KEY ([CustID]) REFERENCES [Customer]([CustomerID]), 
    CONSTRAINT [FK_ServiceRequest_ToUser] FOREIGN KEY ([EmpID]) REFERENCES [AspNetUsers]([Id]), 

    CONSTRAINT [FK_ServiceRequest_ToUserAgent] FOREIGN KEY ([AgentID]) REFERENCES [AspNetUsers]([Id]), 
		CONSTRAINT [FK_ServiceRequest_ToWebUser] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers]([Id]), 


)
