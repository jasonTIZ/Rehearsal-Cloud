CREATE DATABASE [Rehearsal Cloud];
GO

USE [Rehearsal Cloud];
GO

CREATE TABLE Emails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ToAddress NVARCHAR(100) NOT NULL,      -- Dirección de correo electrónico del destinatario
    Subject NVARCHAR(255) NOT NULL,        -- Asunto del correo
    Body NVARCHAR(MAX) NOT NULL,           -- Cuerpo del correo
    SentAt DATETIME NOT NULL DEFAULT GETDATE() -- Fecha y hora de envío
);
GO