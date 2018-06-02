/*
IF NOT EXISTS (SELECT * FROM UserFunctions)
BEGIN
	SET IDENTITY_INSERT [dbo].[UserFunctions] ON
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1, N'User Rights', N'UserRights')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1002, N'Holiday', N'Masters')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1003, N'Leave Type', N'Masters')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1004, N'Leave Entitlement', N'Masters')
	    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1005, N'LeaveApprove', N'Masters')
		    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1006, N'Leave Application', N'Leave Application')
					    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (2002, N'Service Requests', N'Service Requests')




	SET IDENTITY_INSERT [dbo].[UserFunctions] OFF
END
*/
	