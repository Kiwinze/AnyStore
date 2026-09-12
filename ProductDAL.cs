using System.Data;
using System.Data.SqlClient;

namespace AnyStore.DAL
{
    public class ProductDAL
    {
        public DataTable SelectAll()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT p.Id, p.Name, c.Title AS Category, p.Description, 
                                        p.Rate, p.Qty, p.AddedDate
                                 FROM Products p
                                 INNER JOIN Categories c ON p.CategoryId = c.Id
                                 ORDER BY p.Id DESC";
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
                string query = @"SELECT p.Id, p.Name, c.Title AS Category, p.Description, 
                                        p.Rate, p.Qty, p.AddedDate
                                 FROM Products p
                                 INNER JOIN Categories c ON p.CategoryId = c.Id
                                 WHERE p.Name LIKE @kw OR c.Title LIKE @kw 
                                    OR p.Description LIKE @kw";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public bool Insert(string name, int categoryId, string description, decimal rate, int qty, int addedBy)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Products (Name, CategoryId, Description, Rate, Qty, AddedBy)
                                 VALUES (@n, @c, @d, @r, @q, @ab)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@c", categoryId);
                    cmd.Parameters.AddWithValue("@d", (object)description ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@r", rate);
                    cmd.Parameters.AddWithValue("@q", qty);
                    cmd.Parameters.AddWithValue("@ab", addedBy);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Update(int id, string name, int categoryId, string description, decimal rate, int qty)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Products SET 
                                 Name = @n, CategoryId = @c, Description = @d, Rate = @r, Qty = @q
                                 WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@c", categoryId);
                    cmd.Parameters.AddWithValue("@d", (object)description ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@r", rate);
                    cmd.Parameters.AddWithValue("@q", qty);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string checkQuery = "SELECT COUNT(*) FROM Transactions WHERE ProductId = @id";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@id", id);
                    if ((int)checkCmd.ExecuteScalar() > 0)
                        throw new System.Exception("Нельзя удалить товар, по которому есть транзакции.");
                }

                string query = "DELETE FROM Products WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }


        public bool UpdateQuantity(int productId, int newQty, SqlTransaction transaction = null)
        {
            SqlConnection conn = transaction?.Connection ?? DbConnection.GetConnection();
            bool needToOpen = conn.State != ConnectionState.Open;
            if (needToOpen) conn.Open();

            try
            {
                string query = "UPDATE Products SET Qty = @q WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.Parameters.AddWithValue("@q", newQty);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            finally
            {
                if (needToOpen) conn.Close();
            }
        }

        public int GetQuantity(int productId)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT Qty FROM Products WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", productId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? (int)result : 0;
                }
            }
        }
    }
}