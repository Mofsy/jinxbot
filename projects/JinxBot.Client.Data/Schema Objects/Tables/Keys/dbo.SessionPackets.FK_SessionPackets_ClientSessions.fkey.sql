ALTER TABLE [dbo].[SessionPackets] ADD
CONSTRAINT [FK_SessionPackets_ClientSessions] FOREIGN KEY ([SessionID]) REFERENCES [dbo].[ClientSessions] ([ClientSessionID])


