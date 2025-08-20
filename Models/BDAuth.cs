using Microsoft.Data.SqlClient;
using Dapper;

namespace try_catch_poc.Models
{
    public static class BDAuth
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
    }
} 