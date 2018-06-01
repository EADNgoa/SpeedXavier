CREATE TABLE [dbo].[ServiceRequest]
(
	[SRID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CustID] INT NULL, 
    [SRStatusID] INT NULL, 
    [EmpID] NVARCHAR(128) NULL, 
    [EnquirySource] INT NULL, 
	[BookingTypeID] INT NULL, 
    [AgentID] NVARCHAR(128) NULL, 
    [ServiceTypeID] INT NULL, 
    [TDate] DATETIME NULL, 
    CONSTRAINT [FK_ServiceRequest_ToCust] FOREIGN KEY ([CustID]) REFERENCES [Customer]([CustomerID]), 
    CONSTRAINT [FK_ServiceRequest_ToUser] FOREIGN KEY ([EmpID]) REFERENCES [AspNetUsers]([Id]), 
    CONSTRAINT [FK_ServiceRequest_ToUserAgent] FOREIGN KEY ([AgentID]) REFERENCES [AspNetUsers]([Id]), 

)
