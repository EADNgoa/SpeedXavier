
IF NOT EXISTS (SELECT * FROM UserFunctions)
BEGIN
	SET IDENTITY_INSERT [dbo].[UserFunctions] ON
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1, N'User Rights', N'UserRights')

	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (2, N'Holiday', N'Masters')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (3, N'Leave Type', N'Masters')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (4, N'Leave Entitlement', N'Masters')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5, N'LeaveApprove', N'Masters')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (6, N'Leave Application', N'Leave Application')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (7, N'Service Requests', N'Service Requests')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (8, N'BossLogDetails', N'Boss Log Details')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (9, N'Petty Cash', N'Petty Cash')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (10, N'Bank Details', N'Bank Details')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (11, N'Asset Bill', N'Asset Bill')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (12, N'Car Trip', N'Car Trip')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (13, N'Accomodation', N'Accomodation')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (14, N'Activity', N'Activity')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (15, N'Attraction', N'Attraction')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (16, N'Attribute', N'Attribute')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (17, N'Booking', N'Booking')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (18, N'Booking Status', N'Booking Status')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (19, N'Category', N'Category')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (20, N'Coupon', N'Coupon')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (21, N'Customer Query', N'Customer Query')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (22, N'Driver', N'Driver')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (23, N'Facility', N'Facility')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (24, N'Geo Locations', N'Geo Locations')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (25, N'Guide Language', N'Guide Language')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (26, N'Mice', N'Mice')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (27, N'Visa', N'Visa')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (28, N'Option Type', N'Option Type')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (29, N'Package', N'Package')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (30, N'Commision', N'Commision')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (31, N'Review', N'Review')    
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (32, N'Car & Bike', N'Car & Bike')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (33, N'Tax', N'Tax')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (34, N'Questions', N'Questions')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (35, N'Reciept', N'Reciept')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (36, N'Group', N'Group')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (37, N'Supplier', N'Supplier')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (38, N'LogDetails', N'Log Details')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (39, N'Agents', N'Setup')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (40, N'OwnedAsset', N'OwnedAsset')
    INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (41, N'Admin', N'EditComm') ---Edit employee commission
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (42, N'Config', N'Config')
	INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (43, N'Finance', N'Finance')
	




	SET IDENTITY_INSERT [dbo].[UserFunctions] OFF
END

IF NOT EXISTS (SELECT * FROM AspNetUsers)
BEGIN
		INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [DateCreated], [Disabled], [LastLogin], [UserType], [RealName]) VALUES (N'aaa59288-d983-4999-a53b-3109b9ff02ea', N'ea@ea.com', 1, N'ALdc2SanvmCYCeHZSsO7BcHQlbqdWdtQghYvuoD2eDk511epC+xcue7+yOF2KZSQEQ==', N'2db98bf0-a78a-4cb0-91dd-42a2a401d21b', NULL, 0, 0, NULL, 0, 0, N'ea@ea.com', N'2018-03-10 03:56:39', 0, NULL, 0,'SA')
END



IF NOT EXISTS (SELECT * FROM Groups)
BEGIN --Create an Admin group
	SET IDENTITY_INSERT [dbo].Groups ON
	INSERT INTO Groups(GroupId, GroupName)
	Values(1,'Admin')
	SET IDENTITY_INSERT [dbo].Groups OFF
END


IF NOT EXISTS (SELECT * FROM UserGroups)
BEGIN 
	INSERT INTO [dbo].[UserGroups] ([UserID], [GroupID]) VALUES (N'aaa59288-d983-4999-a53b-3109b9ff02ea', 1)
END

--Give all permissions to admin group
	INSERT INTO FunctionGroups
	SELECT FunctionID,1,'True' FROM UserFunctions where FunctionID not in (Select FunctionId from FunctionGroups where GroupID=1)


IF NOT EXISTS (SELECT * FROM ServiceCommision)
BEGIN
SET IDENTITY_INSERT [dbo].[ServiceCommision] ON
		INSERT INTO [dbo].ServiceCommision ([ServiceID], [ServiceName]) VALUES (0, N'Transfer')
		INSERT INTO [dbo].ServiceCommision ([ServiceID], [ServiceName]) VALUES (1, N'Accomodation')
		INSERT INTO [dbo].ServiceCommision ([ServiceID], [ServiceName]) VALUES (2, N'SightSeeing')
		INSERT INTO [dbo].ServiceCommision ([ServiceID], [ServiceName]) VALUES (3, N'Flight')
		INSERT INTO [dbo].ServiceCommision ([ServiceID], [ServiceName]) VALUES (4, N'Insurance')
		INSERT INTO [dbo].ServiceCommision ([ServiceID], [ServiceName]) VALUES (5, N'Packages')
		INSERT INTO [dbo].ServiceCommision ([ServiceID], [ServiceName]) VALUES (6, N'Visa')
		INSERT INTO [dbo].ServiceCommision ([ServiceID], [ServiceName]) VALUES (7, N'CarBike')
		INSERT INTO [dbo].ServiceCommision ([ServiceID], [ServiceName]) VALUES (8, N'Cruise')
		SET IDENTITY_INSERT [dbo].[ServiceCommision] OFF
END

IF NOT EXISTS (SELECT * FROM Config)
BEGIN
SET IDENTITY_INSERT [dbo].[Config] ON
		INSERT INTO [dbo].Config ([ConfigId], [ProductId], [TransServiceCharge], [MerchantId], [Pwd]) VALUES (1, N'NSE',0,'197','Test@123')
		SET IDENTITY_INSERT [dbo].[ServiceCommision] OFF
END

