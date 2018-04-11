CREATE TABLE [dbo].[Package_Supplier]
(
	[PackageID] INT NOT NULL , 
    [SupplierID] INT NOT NULL ,
	[ContractNo] VARChar(50) NULL, 
    CONSTRAINT [PK_dbo.PackSup] PRIMARY KEY CLUSTERED ([PackageID] ASC, [SupplierID] ASC), 
    CONSTRAINT [FK_Package_Supplier_ToSupplier] FOREIGN KEY ([SupplierID]) REFERENCES [Supplier]([SupplierID]),
	CONSTRAINT [FK_Package_Supplier_ToPackage] FOREIGN KEY ([PackageID]) REFERENCES [Package]([PackageID])
)
