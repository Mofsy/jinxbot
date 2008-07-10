CREATE TABLE [dbo].[ClientSessions]
(
[ClientSessionID] [int] NOT NULL IDENTITY(1, 1),
[Client] [varchar] (4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Server] [varchar] (127) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[VersionFile1] [varbinary] (max) NOT NULL,
[VersionFile2] [varbinary] (max) NOT NULL,
[VersionFile3] [varbinary] (max) NOT NULL,
[VersionFile4] [varbinary] (max) NULL,
[Started] [datetime] NOT NULL,
[Ended] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


