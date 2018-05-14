CREATE TABLE [dbo].[LeaveEntitlement] (
    [LeaveEntitlementID] INT             IDENTITY (1, 1) NOT NULL,
    [LeaveYear]          INT             NOT NULL,
    [LeaveTypeID]        INT             NOT NULL,
    [LeaveDays]          DECIMAL (10, 2) NULL,
    CONSTRAINT [PK_LeaveEntitlement] PRIMARY KEY CLUSTERED ([LeaveEntitlementID] ASC),
    CONSTRAINT [FK_LeaveEntitlement_LeaveType] FOREIGN KEY ([LeaveTypeID]) REFERENCES [dbo].[LeaveType] ([LeaveTypeID])
);

