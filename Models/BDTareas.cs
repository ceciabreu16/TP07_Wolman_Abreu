using Microsoft.Data.SqlClient;
using Dapper;

namespace try_catch_poc.Models
{
    public static class BDTareas
    {
        private static string _connectionString = @"Server=localhost;DataBase=TP06_Prog;Integrated Security=True;TrustServerCertificate=True;";

        public static List<Tareas> ListarTareas(string username)
        {
            var listaTareas = new List<Tareas>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"
            SELECT T.Id, T.Titulo, T.FechaCreacion, T.EstaFinalizada, T.EstaEliminada, TC.UsuarioEsCreador
            FROM Tareas T
            INNER JOIN TareasCompartidas TC ON T.Id = TC.TareaId
            INNER JOIN Usuarios U ON U.Id = TC.UsuarioId
            WHERE U.Username = @username";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@username", username);

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaTareas.Add(new Tareas
                        {
                            Id = (int)reader["Id"],
                            Titulo = reader["Titulo"].ToString(),
                            FechaCreacion = (DateTime)reader["FechaCreacion"],
                            EstaFinalizada = (bool)reader["EstaFinalizada"],
                            EstaEliminada = (bool)reader["EstaEliminada"],
                            EsPropia = (bool)reader["UsuarioEsCreador"]
                        });
                    }
                }
            }
            return listaTareas.OrderBy(t => t.FechaCreacion).ToList();
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
                string sql = "SELECT T.*, TC.UsuarioEsCreador FROM Tareas T INNER JOIN TareasCompartidas TC ON T.Id = TC.TareaId WHERE T.Id = @id";
                var result = connection.QueryFirstOrDefault(sql, new { id });
                if (result != null)
                {
                    return new Tareas
                    {
                        Id = result.Id,
                        Titulo = result.Titulo,
                        FechaCreacion = result.FechaCreacion,
                        EstaFinalizada = result.EstaFinalizada,
                        EstaEliminada = result.EstaEliminada,
                        EsPropia = result.UsuarioEsCreador
                    };
                }
                return null;
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

        public static void EliminarTarea(int id)
        {
            // Borrado lógico: marca la tarea como eliminada
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Tareas SET EstaEliminada = 1 WHERE Id = @id";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void FinalizarTarea(int id)
        {
            // Marca la tarea como finalizada
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Tareas SET EstaFinalizada = 1 WHERE Id = @id";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
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
                string query = "INSERT INTO TareasCompartidas (TareaId, UsuarioId, UsuarioEsCreador) VALUES (@tareaId, @usuarioId, 0)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@tareaId", tareaId);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
} 