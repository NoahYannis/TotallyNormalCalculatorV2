USE [DiaryEntryDB]
GO

/****** Object: Table [dbo].[Entries] Script Date: 03.06.2024 20:50:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Entries] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [Title]   NVARCHAR (50)  NULL,
    [Message] NVARCHAR (MAX) NULL,
    [Date]    NVARCHAR (50)  NULL
);


