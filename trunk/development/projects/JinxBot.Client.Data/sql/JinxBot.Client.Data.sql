SET  ARITHABORT, CONCAT_NULL_YIELDS_NULL, ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, QUOTED_IDENTIFIER ON 
SET  NUMERIC_ROUNDABORT OFF
GO
:setvar DatabaseName "JinxBot.Client.Data"
:setvar PrimaryFilePhysicalName "C:\Program Files\Microsoft SQL Server\MSSQL.2\MSSQL\DATA\JinxBot.Client.Data.mdf"
:setvar PrimaryLogFilePhysicalName "C:\Program Files\Microsoft SQL Server\MSSQL.2\MSSQL\DATA\JinxBot.Client.Data_log.ldf"

USE [master]

GO

:on error exit

IF  (DB_ID(N'$(DatabaseName)') IS NOT NULL
    AND DATABASEPROPERTYEX(N'$(DatabaseName)','Status') <> N'ONLINE')
BEGIN
    RAISERROR(N'The state of the target database, %s, is not set to ONLINE. To deploy to this database, its state must be set to ONLINE.', 16, 127,N'$(DatabaseName)') WITH NOWAIT
    RETURN
END
GO

IF (DB_ID(N'$(DatabaseName)') IS NOT NULL)
BEGIN
    IF ((SELECT CAST(value AS nvarchar(128))
	    FROM 
		    [$(DatabaseName)]..fn_listextendedproperty('microsoft_database_tools_deploystamp', null, null, null, null, null, null )) 
	    = CAST(N'011f24e6-d10c-47f5-988c-6102f5813eb2' AS nvarchar(128)))
    BEGIN
	    RAISERROR(N'Deployment has been skipped because the script has already been deployed to the target server.', 16 ,100) WITH NOWAIT
	    RETURN
    END
END
GO


:on error exit

CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'PrimaryFileName', FILENAME = N'$(PrimaryFilePhysicalName)') LOG ON ( NAME = N'PrimaryLogFileName', FILENAME = N'$(PrimaryLogFilePhysicalName)') COLLATE SQL_Latin1_General_CP1_CS_AS 

GO

:on error resume
     
EXEC sp_dbcmptlevel N'$(DatabaseName)', 90

GO

IF EXISTS (SELECT 1 FROM [sys].[databases] WHERE [name] = N'$(DatabaseName)') 
    ALTER DATABASE [$(DatabaseName)] SET  
	ALLOW_SNAPSHOT_ISOLATION OFF
GO

IF EXISTS (SELECT 1 FROM [sys].[databases] WHERE [name] = N'$(DatabaseName)') 
    ALTER DATABASE [$(DatabaseName)] SET  
	READ_COMMITTED_SNAPSHOT OFF
GO

IF EXISTS (SELECT 1 FROM [sys].[databases] WHERE [name] = N'$(DatabaseName)') 
    ALTER DATABASE [$(DatabaseName)] SET  
	MULTI_USER,
	CURSOR_CLOSE_ON_COMMIT OFF,
	CURSOR_DEFAULT LOCAL,
	AUTO_CLOSE OFF,
	AUTO_CREATE_STATISTICS ON,
	AUTO_SHRINK OFF,
	AUTO_UPDATE_STATISTICS ON,
	AUTO_UPDATE_STATISTICS_ASYNC ON,
	ANSI_NULL_DEFAULT ON,
	ANSI_NULLS ON,
	ANSI_PADDING ON,
	ANSI_WARNINGS ON,
	ARITHABORT ON,
	CONCAT_NULL_YIELDS_NULL ON,
	NUMERIC_ROUNDABORT OFF,
	QUOTED_IDENTIFIER ON,
	RECURSIVE_TRIGGERS OFF,
	RECOVERY FULL,
	PAGE_VERIFY NONE,
	DISABLE_BROKER,
	PARAMETERIZATION SIMPLE
	WITH ROLLBACK IMMEDIATE
GO

IF IS_SRVROLEMEMBER ('sysadmin') = 1
BEGIN

IF EXISTS (SELECT 1 FROM [sys].[databases] WHERE [name] = N'$(DatabaseName)') 
    EXEC sp_executesql N'
    ALTER DATABASE [$(DatabaseName)] SET  
	DB_CHAINING OFF,
	TRUSTWORTHY OFF'

END
ELSE
BEGIN
    RAISERROR(N'Unable to modify the database settings for DB_CHAINING or TRUSTWORTHY. You must be a SysAdmin in order to apply these settings.',0,1)
END

GO

USE [$(DatabaseName)]

GO
/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script	
 Use SQLCMD syntax to include a file into the pre-deployment script			
 Example:      :r .\filename.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/








GO

:on error exit

:on error resume
GO
PRINT N'Creating [dbo].[ClientSessions]'
GO
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
GO
PRINT N'Creating primary key [PK_ClientSessions] on [dbo].[ClientSessions]'
GO
ALTER TABLE [dbo].[ClientSessions] ADD CONSTRAINT [PK_ClientSessions] PRIMARY KEY CLUSTERED  ([ClientSessionID]) ON [PRIMARY]
GO
PRINT N'Creating [dbo].[SessionPackets]'
GO
CREATE TABLE [dbo].[SessionPackets]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[SessionID] [int] NOT NULL,
[Sent] [bit] NOT NULL,
[Contents] [varbinary] (2048) NOT NULL
) ON [PRIMARY]
GO
PRINT N'Creating primary key [PK_SessionPackets] on [dbo].[SessionPackets]'
GO
ALTER TABLE [dbo].[SessionPackets] ADD CONSTRAINT [PK_SessionPackets] PRIMARY KEY CLUSTERED  ([ID]) ON [PRIMARY]
GO
PRINT N'Adding foreign keys to [dbo].[SessionPackets]'
GO
ALTER TABLE [dbo].[SessionPackets] ADD
CONSTRAINT [FK_SessionPackets_ClientSessions] FOREIGN KEY ([SessionID]) REFERENCES [dbo].[ClientSessions] ([ClientSessionID])
GO

GO
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script		
 Use SQLCMD syntax to include a file into the post-deployment script			
 Example:      :r .\filename.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/










USE [$(DatabaseName)]
IF ((SELECT COUNT(*) 
	FROM 
		::fn_listextendedproperty( 'microsoft_database_tools_deploystamp', null, null, null, null, null, null )) 
	> 0)
BEGIN
	EXEC [dbo].sp_dropextendedproperty 'microsoft_database_tools_deploystamp'
END
EXEC [dbo].sp_addextendedproperty 'microsoft_database_tools_deploystamp', N'011f24e6-d10c-47f5-988c-6102f5813eb2'
GO

