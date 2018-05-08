CREATE TABLE [dbo].[LeaveBalance] (
    [LeaveBalanceID] INT             IDENTITY (1, 1) NOT NULL,
    [UserID]         NVARCHAR (128)  NULL,
    [LeaveTypeID]    INT             NOT NULL,
    [LeaveYear]      INT             NOT NULL,
    [LeaveDays]      DECIMAL (10, 2) NULL,
    [Attendance]     INT             NULL,
    PRIMARY KEY CLUSTERED ([LeaveBalanceID] ASC, [LeaveTypeID] ASC, [LeaveYear] ASC),
    CONSTRAINT [FK_LeaveBalance_LeaveType] FOREIGN KEY ([LeaveTypeID]) REFERENCES [dbo].[LeaveType] ([LeaveTypeID])
);

