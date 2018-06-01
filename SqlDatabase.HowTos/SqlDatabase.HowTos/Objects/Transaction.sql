CREATE TABLE [dbo].[Transaction]
(
	[Id] INT NOT NULL PRIMARY KEY,
	CreationDate datetime2 not null default GETUTCDATE(),
	Details varchar(100)
)
