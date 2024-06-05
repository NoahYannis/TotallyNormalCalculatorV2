IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'DiaryEntryDB')
BEGIN
    CREATE DATABASE DiaryEntryDB;
END;

USE [DiaryEntryDB];

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Entries]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Entries] (
        [Id]      INT            IDENTITY (1, 1) NOT NULL,
        [Title]   NVARCHAR (50)  NULL,
        [Message] NVARCHAR (MAX) NULL,
        [Date]    NVARCHAR (50)  NULL
    );
END;
