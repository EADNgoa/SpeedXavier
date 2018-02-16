CREATE TABLE [dbo].[Picture]
(
	[PictureID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ServiceTypeID] INT NULL, 
    [PictureName] VARCHAR(100) NULL, 
    [ServiceID] INT NULL
)
