CREATE TABLE [dbo].[SR_Cust]
(
	[ServiceRequestID] INT NOT NULL , 
    [CustomerID] INT NOT NULL, 
    PRIMARY KEY ([CustomerID],[ServiceRequestID])
)
