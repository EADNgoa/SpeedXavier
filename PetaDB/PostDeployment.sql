
--Post-Deployment Script Template							
----------------------------------------------------------------------------------------
-- This file contains SQL statements that will be appended to the build script.		
-- Use SQLCMD syntax to include a file in the post-deployment script.			
-- Example:      :r .\myfile.sql								
-- Use SQLCMD syntax to reference a variable in the post-deployment script.		
-- Example:      :setvar TableName MyTable							
--               SELECT * FROM [$(TableName)]					
----------------------------------------------------------------------------------------


SET IDENTITY_INSERT [dbo].[UserFunctions] ON
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1, N'Units', N'Masters')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (2, N'CashCard', N'Bar')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (3, N'Course', N'Dining')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (4, N'Location', N'Masters')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (6, N'Table', N'Dining')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (7, N'Group', N'Users')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1002, N'Inventory Receipts', N'Kitchen')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1003, N'Inventory Checking', N'Kitchen')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1004, N'Unit Conversion', N'Masters')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (2002, N'Inventory Portion', N'Kitchen')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (2003, N'Wastage Report', N'Kitchen')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (2004, N'Inventory Move Use', N'Kitchen')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (3002, N'Item', N'Master')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (4002, N'Table Res', N'Dining')
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (5002, N'Menu', N'Dining')
SET IDENTITY_INSERT [dbo].[UserFunctions] OFF


IF NOT EXISTS (SELECT * FROM ItemTypes)
BEGIN
	SET IDENTITY_INSERT [dbo].[ItemTypes] ON
	INSERT INTO [dbo].[ItemTypes] ([ItemTypeId], [ItemTypeName]) VALUES (1, N'Raw Material')
	INSERT INTO [dbo].[ItemTypes] ([ItemTypeId], [ItemTypeName]) VALUES (2, N'Ready To Serve')
	INSERT INTO [dbo].[ItemTypes] ([ItemTypeId], [ItemTypeName]) VALUES (3, N'Drinks')
	INSERT INTO [dbo].[ItemTypes] ([ItemTypeId], [ItemTypeName]) VALUES (4, N'Stationary')
	INSERT INTO [dbo].[ItemTypes] ([ItemTypeId], [ItemTypeName]) VALUES (5, N'Keys')
	INSERT INTO [dbo].[ItemTypes] ([ItemTypeId], [ItemTypeName]) VALUES (6, N'Maintenance')
	INSERT INTO [dbo].[ItemTypes] ([ItemTypeId], [ItemTypeName]) VALUES (7, N'LaundryGuest')
	INSERT INTO [dbo].[ItemTypes] ([ItemTypeId], [ItemTypeName]) VALUES (8, N'LaundryStaff')
	INSERT INTO [dbo].[ItemTypes] ([ItemTypeId], [ItemTypeName]) VALUES (9, N'Linen')
	INSERT INTO [dbo].[ItemTypes] ([ItemTypeId], [ItemTypeName]) VALUES (10, N'Toiletries')
	SET IDENTITY_INSERT [dbo].[ItemTypes] OFF
END


--Do for Location Types
--Add location: Kitchen Load point

