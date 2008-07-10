CREATE TABLE [dbo].[SessionPackets]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[SessionID] [int] NOT NULL,
[Sent] [bit] NOT NULL,
[Contents] [varbinary] (2048) NOT NULL
) ON [PRIMARY]


