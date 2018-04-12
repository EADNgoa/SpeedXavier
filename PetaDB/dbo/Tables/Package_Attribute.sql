CREATE TABLE [dbo].[Package_Attribute]
(
	[PackageID] INT NOT NULL , 
    [AttributeID] INT NOT NULL ,	
    CONSTRAINT [PK_dbo.PackAtt] PRIMARY KEY CLUSTERED ([PackageID] ASC, [AttributeID] ASC), 
    CONSTRAINT [FK_Package_Attribute_ToAttribute] FOREIGN KEY ([AttributeID]) REFERENCES [Attribute]([AttributeID]),
	CONSTRAINT [FK_Package_Attribute_ToPackage] FOREIGN KEY ([PackageID]) REFERENCES [Package]([PackageID])
)
