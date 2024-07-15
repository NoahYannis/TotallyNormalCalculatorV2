IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'DiaryEntryDB')
BEGIN
    CREATE DATABASE DiaryEntryDB;
END;
