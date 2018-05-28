CREATE TABLE [dbo].[LeaveApplications] (
    [LeaveApplicationID] INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationDate]    DATE           NULL,
    [UserID]             NVARCHAR (128) NULL,
    [LeaveTypeID]        INT            NULL,
    [LeaveStartDate]     DATE           NULL,
    [NoOfDays]           DECIMAL(5, 2)            NULL,
    [StatusID]           INT            NULL,
    [StatusBy]           NVARCHAR (128) NULL,
    [StatusDate]         DATE           NULL,
    PRIMARY KEY CLUSTERED ([LeaveApplicationID] ASC),
    CONSTRAINT [FK_LeaveApplication_LeaveType] FOREIGN KEY ([LeaveTypeID]) REFERENCES [dbo].[LeaveType] ([LeaveTypeID])
);

