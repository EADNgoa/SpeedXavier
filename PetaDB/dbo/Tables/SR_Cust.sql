CREATE TABLE [dbo].[SR_Cust]
(
	[ServiceRequestID] INT NOT NULL , 
    [CustomerID] INT NOT NULL, 
    [IsLead] BIT NOT NULL DEFAULT 0, 
    PRIMARY KEY ([CustomerID],[ServiceRequestID])
)
