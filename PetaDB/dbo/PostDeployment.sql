
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
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (3005, N'BossLogDetails', N'Boss Log Details')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (4002, N'Petty Cash', N'Petty Cash')




	SET IDENTITY_INSERT [dbo].[UserFunctions] OFF
END


IF NOT EXISTS (SELECT * FROM ServiceCommision)
BEGIN
	SET IDENTITY_INSERT [dbo].[ServiceCommison] ON
	INSERT INTO [dbo].[ServiceCommision] ([ServiceID], [ServiceName]) VALUES (1, N'Accomodation')
		INSERT INTO [dbo].[ServiceCommision] ([ServiceID], [ServiceName]) VALUES (2, N'Packages')
			INSERT INTO [dbo].[ServiceCommision] ([ServiceID], [ServiceName]) VALUES (3, N'Cruise')
				INSERT INTO [dbo].[ServiceCommision] ([ServiceID], [ServiceName]) VALUES (4, N'SightSeeing')

	INSERT INTO [dbo].[ServiceCommision] ([ServiceID], [ServiceName]) VALUES (5, N'CarBike')
		INSERT INTO [dbo].[ServiceCommision] ([ServiceID], [ServiceName]) VALUES (6, N'Insurance')
			INSERT INTO [dbo].[ServiceCommision] ([ServiceID], [ServiceName]) VALUES (7, N'Visa')
				INSERT INTO [dbo].[ServiceCommision] ([ServiceID], [ServiceName]) VALUES (8, N'Flight')






	SET IDENTITY_INSERT [dbo].[ServiceCommison] OFF
END
