CREATE TABLE [dbo].[PackageValidity]
(
	[PVId] INT NOT NULL IDENTITY PRIMARY KEY, 
    [ServiceID] INT NOT NULL, 
    [ValidFrom] DATE NOT NULL, 
    [ValidTo] DATE NOT NULL,
	[ServiceTypeID] INT NULL, 
)