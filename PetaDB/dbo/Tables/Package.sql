﻿CREATE TABLE [dbo].[Package]
(
	[PackageID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ServiceTypeID] INT NULL, 
    [PackageName] VARCHAR(100) NULL, 
    [Description] VARCHAR(MAX) NULL, 
    [Duration] INT NULL, 
    [Itinerary] VARCHAR(MAX) NULL, 
    [Dificulty] INT NULL, 
    [GroupSize] INT NULL, 
    [GuideLanguageID] INT NULL, 
    [StartTime] VARCHAR(300) NULL, 
    [Inclusion] VARCHAR(MAX) NULL, 
    [Exclusion] VARCHAR(MAX) NULL, 
	[Highlights] VARCHAR(MAX) NULL, 
    [CouponCode] VARCHAR(50) NULL,     
    [SupplierNotepad] VARCHAR(MAX) NULL, 
    [MeetAndInfo] VARCHAR(MAX) NULL, 
    CONSTRAINT [FK_Package_GuideLanguage] FOREIGN KEY ([GuideLanguageID]) REFERENCES [GuideLanguage]([GuideLanguageID])
)
