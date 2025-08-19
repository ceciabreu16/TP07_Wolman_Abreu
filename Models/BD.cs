
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
        public static void InsertarTarea(string titulo, string estado, string username, DateTime fechaCreacion)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlTarea = "INSERT INTO Tareas (Titulo, FechaCreacion, EstaFinalizada, EstaEliminada) OUTPUT INSERTED.Id VALUES (@titulo, @fechaCreacion, @finalizada, @eliminada)";
                bool finalizada = estado == "finalizada";
                bool eliminada = estado == "eliminada";
                int tareaId = connection.QuerySingle<int>(sqlTarea, new { titulo, fechaCreacion, finalizada, eliminada });

                string sqlUsuario = "SELECT Id FROM Usuarios WHERE Username = @username";
                int usuarioId = connection.QuerySingle<int>(sqlUsuario, new { username });

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

        public static void EditarTarea(int id, string titulo, string estado, DateTime fechaCreacion)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                bool finalizada = estado == "finalizada";
                bool eliminada = estado == "eliminada";
                string sql = "UPDATE Tareas SET Titulo = @titulo, EstaFinalizada = @finalizada, EstaEliminada = @eliminada, FechaCreacion = @fechaCreacion WHERE Id = @id";
                connection.Execute(sql, new { id, titulo, finalizada, eliminada, fechaCreacion });
            }
        }
        public static bool UsuarioExiste(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Usuarios WHERE Username = @username";
                int count = connection.QuerySingle<int>(sql, new { username });
                return count > 0;
            }
        }

        public static void RegistrarUsuario(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Usuarios (Username, Password) VALUES (@username, @password)";
                connection.Execute(sql, new { username, password });
            }
        }
        public static bool UsuarioExiste(string username)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Usuarios WHERE Username = @username";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", username);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static int ObtenerIdUsuario(string username)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id FROM Usuarios WHERE Username = @username";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", username);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? (int)result : -1;
            }
        }

        public static bool TareaYaCompartidaConUsuario(int tareaId, int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM TareasCompartidas WHERE TareaId = @tareaId AND UsuarioId = @usuarioId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@tareaId", tareaId);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static void AgregarTareaCompartida(int tareaId, int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO TareasCompartidas (TareaId, UsuarioId) VALUES (@tareaId, @usuarioId)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@tareaId", tareaId);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Tareas> TareasCompartidasConUsuario(int usuarioId)
        {
            var tareas = new List<Tareas>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"SELECT t.* FROM Tareas t
                             INNER JOIN TareasCompartidas tc ON t.Id = tc.TareaId
                             WHERE tc.UsuarioId = @usuarioId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Asumiendo que tienes un constructor adecuado
                        tareas.Add(new Tareas
                        {
                            Id = (int)reader["Id"],
                            Titulo = reader["Titulo"].ToString(),
                            Descripcion = reader["Descripcion"].ToString(),
                            // ... otras propiedades
                        });
                    }
                }
            }
            return tareas;
        }
    }
}

