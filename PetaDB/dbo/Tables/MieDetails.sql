CREATE TABLE [dbo].[MiceDetails] (
    [MiceID]    INT             IDENTITY (1, 1) NOT NULL,
    [GuestName] VARCHAR (50)    NULL,
    [TDate]     DATETIME        NULL,
    [Phone]     VARCHAR (50)    NULL,
    [Email]     VARCHAR (50)    NULL,
    [AgentName] VARCHAR (100)   NULL,
    [Detail]    Varchar (350) NULL,
    [IsRead] BIT NULL, 
    PRIMARY KEY CLUSTERED ([MiceID] ASC)
);

