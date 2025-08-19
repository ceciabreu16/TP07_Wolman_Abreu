
-- Crear la base de datos
CREATE DATABASE TP06_Prog;
GO

USE TP06_Prog;
GO

-- Tabla de usuarios
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL
);
GO

-- Tabla de tareas
CREATE TABLE Tareas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(255) NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    EstaFinalizada BIT NOT NULL DEFAULT 0,
    EstaEliminada BIT NOT NULL DEFAULT 0,
);
GO

-- Tabla para compartir tareas con otros usuarios
CREATE TABLE TareasCompartidas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TareaId INT NOT NULL,
    UsuarioId INT NOT NULL,
	UsuarioEsCreador BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (TareaId) REFERENCES Tareas(Id),
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    CONSTRAINT UC_Tarea_Usuario UNIQUE (TareaId, UsuarioId)
);
GO

-- Insertar usuarios
INSERT INTO Usuarios (Username, Password) VALUES 
('ceciabreu', 'ceci16'),
('clarawolman', 'clara13');
GO


CREATE PROCEDURE InsertarUsuario
@Username varchar(50),
@Password varchar(50)
AS
BEGIN
SELECT * from Usuarios WHERE  Username= @Username 
END 
GO 


ALTER PROCEDURE InsertarUsuario
@Username varchar(50),
@Password varchar(50)
AS
BEGIN 
 IF NOT EXISTS (SELECT 1 from Usuarios WHERE Username = @Username)
	BEGIN 
	INSERT INTO Usuarios (Username, Password)
	VALUES (@Username, @Password)
	PRINT 'INSERTO OK'
	END 
	ELSE 
	BEGIN 
	PRINT 'NO INSERTO'
	END
	END