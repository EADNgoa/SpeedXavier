CREATE TABLE [dbo].[PackageValidity]
(
	[PVId] INT NOT NULL IDENTITY PRIMARY KEY, 
    [PackageId] INT NOT NULL, 
    [ValidFrom] DATE NOT NULL, 
    [ValidTo] DATE NOT NULL,
	CONSTRAINT [FK_PackageValidity_ToPackage] FOREIGN KEY ([PackageId]) REFERENCES [Package]([PackageId])
)
