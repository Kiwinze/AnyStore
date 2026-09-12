using System.Data;
using System.Data.SqlClient;

namespace AnyStore.DAL
{
    public class CategoryDAL
    {
        public DataTable SelectAll()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT Id, Title, Description, AddedDate 
                                 FROM Categories ORDER BY Title";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT Id, Title, Description, AddedDate 
                                 FROM Categories
                                 WHERE Title LIKE @kw OR Description LIKE @kw";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public bool Insert(string title, string description, int addedBy)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Categories (Title, Description, AddedBy)
                                 VALUES (@t, @d, @ab)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@t", title);
                    cmd.Parameters.AddWithValue("@d", (object)description ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@ab", addedBy);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Update(int id, string title, string description)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Categories SET Title = @t, Description = @d WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@t", title);
                    cmd.Parameters.AddWithValue("@d", (object)description ?? System.DBNull.Value);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                // Проверка: есть ли товары в категории?
                string checkQuery = "SELECT COUNT(*) FROM Products WHERE CategoryId = @id";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@id", id);
                    if ((int)checkCmd.ExecuteScalar() > 0)
                        throw new System.Exception("Нельзя удалить категорию, содержащую товары.");
                }

                string query = "DELETE FROM Categories WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}