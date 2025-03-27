CREATE DATABASE [Rehearsal Cloud];
GO

USE [Rehearsal Cloud];
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,   -- Cambié el nombre a PasswordHash
    Email NVARCHAR(100) NOT NULL UNIQUE    -- Agregado campo Email
);
GO
