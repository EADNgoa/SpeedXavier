CREATE TABLE [dbo].[FunctionGroups]
(
	[FunctionID] INT NOT NULL , 
    [GroupID] INT NOT NULL,
	[Writable] BIT NOT NULL  DEFAULT 0, 
    PRIMARY KEY ([GroupID], [FunctionID]),
    CONSTRAINT [FK_FuctionGroups_ToUserFuncts] FOREIGN KEY (FunctionID) REFERENCES UserFunctions(FunctionID), 
    CONSTRAINT [FK_FuctionGroups_ToGroups] FOREIGN KEY (GroupID) REFERENCES Groups(GroupID)
)
