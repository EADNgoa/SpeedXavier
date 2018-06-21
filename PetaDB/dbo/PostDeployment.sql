
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
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (4003, N'Bank Details', N'Bank Details')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (4004, N'Asset Bill', N'Asset Bill')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (4005, N'Car Trip', N'Car Trip')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5002, N'Accomodation', N'Accomodation')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5003, N'Activity', N'Activity')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5004, N'Attraction', N'Attraction')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5005, N'Attribute', N'Attribute')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5006, N'Booking', N'Booking')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5007, N'Booking Status', N'Booking Status')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5008, N'Car & Bike', N'Car & Bike')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5009, N'Category', N'Category')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5010, N'Coupon', N'Coupon')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5011, N'Customer Query', N'Customer Query')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5012, N'Driver', N'Driver')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5013, N'Facility', N'Facility')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5014, N'Geo Locations', N'Geo Locations')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5015, N'Guide Language', N'Guide Language')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5016, N'Mice', N'Mice')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5017, N'Visa', N'Visa')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5018, N'Option Type', N'Option Type')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5019, N'Package', N'Package')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5020, N'Commision', N'Commision')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5021, N'Review', N'Review')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5022, N'Review', N'Review')









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
