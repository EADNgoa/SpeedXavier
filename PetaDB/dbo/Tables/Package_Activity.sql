CREATE TABLE [dbo].[Package_Activity]
(
	[PackageID] INT NOT NULL , 
    [ActivityID] INT NOT NULL ,
	CONSTRAINT [PK_dbo.PackAct] PRIMARY KEY CLUSTERED ([PackageID] ASC, [ActivityID] ASC),

)
