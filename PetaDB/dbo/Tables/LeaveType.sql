CREATE TABLE [dbo].[LeaveType] (
    [LeaveTypeID]   INT           IDENTITY (1, 1) NOT NULL,
    [LeaveTypeName] VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([LeaveTypeID] ASC)
);

