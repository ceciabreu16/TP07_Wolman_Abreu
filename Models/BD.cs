
using Microsoft.Data.SqlClient;
using Dapper;

namespace try_catch_poc.Models
{
    public static class BD
    {
        private static string _connectionString = @"Server=localhost;DataBase=TP06_Prog;Integrated Security=True;TrustServerCertificate=True;";

        public static Usuario BuscarUsuario(string Username, string password)
        {
            Usuario nuevoUsuario = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Usuarios WHERE Username = @Username AND Password = @Password";
                nuevoUsuario = connection.QueryFirstOrDefault<Usuario>(sql, new { Username, password });
            }
            return nuevoUsuario;
        }

        public static List<Tareas> ListarTareas(string username)
        {
            List<Tareas> listaTareas = new List<Tareas>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT T.* FROM Tareas T INNER JOIN TareasCompartidas TC ON T.Id = TC.TareaId INNER JOIN Usuarios U ON U.Id = TC.UsuarioId WHERE U.Username = @username";
                listaTareas = connection.Query<Tareas>(sql, new { username }).ToList();
            }
            return listaTareas;
        }
        public static void InsertarTarea(string titulo, string estado, string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Insertar tarea sin EstaActiva
                string sqlTarea = "INSERT INTO Tareas (Titulo, FechaCreacion, EstaFinalizada, EstaEliminada) OUTPUT INSERTED.Id VALUES (@titulo, GETDATE(), @finalizada, @eliminada)";

                bool finalizada = estado == "finalizada";
                bool eliminada = estado == "eliminada";

                int tareaId = connection.QuerySingle<int>(sqlTarea, new { titulo, finalizada, eliminada });

                // Obtener el Id del usuario
                string sqlUsuario = "SELECT Id FROM Usuarios WHERE Username = @username";
                int usuarioId = connection.QuerySingle<int>(sqlUsuario, new { username });

                // Insertar en TareasCompartidas
                string sqlCompartir = "INSERT INTO TareasCompartidas (TareaId, UsuarioId, UsuarioEsCreador) VALUES (@tareaId, @usuarioId, 1)";
                connection.Execute(sqlCompartir, new { tareaId, usuarioId });
            }
        }
        public static Tareas BuscarTareaPorId(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Tareas WHERE Id = @id";
                return connection.QueryFirstOrDefault<Tareas>(sql, new { id });
            }
        }

        public static void EditarTarea(int id, string titulo, string estado)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                bool finalizada = estado == "finalizada";
                bool eliminada = estado == "eliminada";
                string sql = "UPDATE Tareas SET Titulo = @titulo, EstaFinalizada = @finalizada, EstaEliminada = @eliminada WHERE Id = @id";
                connection.Execute(sql, new { id, titulo, finalizada, eliminada });
            }
        }
    }
}


/*namespace try_catch_poc.Models;

public static class BD
{
    private static string _connectionString = @"Server=localhost;DataBase=TP06_Prog;Integrated Security=True;TrustServerCertificate=True;";

    public static List<Usuario> LevantarIntegrante()
    {
        List<Usuario> usuarios = new List<Usuario>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM Integrante";
            usuarios = connection.Query<Usuario>(query).ToList();
        }
        return usuarios;
    }
    public static Usuario BuscarUsuario(string Username, string password)
    {
        Usuario nuevoUsuario = null;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Usuarios WHERE Username = @Username AND Password = @Password";
            nuevoUsuario = connection.QueryFirstOrDefault<Usuario>(sql, new { Username, password });
        }
        return nuevoUsuario;
    }
    public static List<Tareas> ListarTareas(string username)
    {
        List<Tareas> listaTareas = new List<Tareas>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string sql = "SELECT T.* FROM Tareas T INNER JOIN TareasCompartidas TC ON T.Id = TC.TareaId INNER JOIN Usuarios U ON U.Id = TC.UsuarioId WHERE U.Username = @username";
            listaTareas = connection.Query<Tareas>(sql, new { username }).ToList();
        }
        return listaTareas;
    }

}*/
/*
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


*/