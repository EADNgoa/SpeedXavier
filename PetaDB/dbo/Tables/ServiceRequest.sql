﻿CREATE TABLE [dbo].[ServiceRequest]
(
	[SRID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CustID] INT NULL, 
    [SRStatusID] INT NULL, 
    [EmpID] NVARCHAR(128) NULL, 
    [EnquirySource] VARCHAR(50) NULL, 
    [AgentID] INT NULL, 
    CONSTRAINT [FK_ServiceRequest_ToCust] FOREIGN KEY ([CustID]) REFERENCES [Customer]([CustomerID]), 
    CONSTRAINT [FK_ServiceRequest_ToUser] FOREIGN KEY ([EmpID]) REFERENCES [AspNetUsers]([Id]), 
)