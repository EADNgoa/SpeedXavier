
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
INSERT INTO [dbo].[UserFunctions] ([FunctionID], [FunctionName], [Module]) VALUES (1, N'UserRights', N'UserRights')

SET IDENTITY_INSERT [dbo].[UserFunctions] OFF

IF NOT EXISTS (SELECT * FROM BookingStatus)
BEGIN
	SET IDENTITY_INSERT [dbo].[BookingStatus] ON
	INSERT INTO [dbo].[BookingStatus] ([BookingStatusID], [BookingStatusName]) VALUES (1, N'Processing')
	INSERT INTO [dbo].[BookingStatus] ([BookingStatusID], [BookingStatusName]) VALUES (2, N'Confirm')
	INSERT INTO [dbo].[BookingStatus] ([BookingStatusID], [BookingStatusName]) VALUES (3, N'Cancel')

	SET IDENTITY_INSERT [dbo].[BookingStatus] OFF
END

