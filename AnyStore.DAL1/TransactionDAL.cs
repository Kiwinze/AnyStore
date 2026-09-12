using System;
using System.Data;
using System.Data.SqlClient;

namespace AnyStore.DAL
{
    public class TransactionDAL
    {

        public bool Insert(string type, string dealerCustomer, int productId,
                           decimal rate, int qty, decimal total, int addedBy, string addedByName)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        
                        string insertQuery = @"INSERT INTO Transactions 
                            (Type, DealerCustomerName, ProductId, Rate, Qty, Total, AddedBy, AddedByName)
                            VALUES (@t, @dc, @p, @r, @q, @tot, @ab, @abn)";

                        using (SqlCommand cmd = new SqlCommand(insertQuery, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@t", type);
                            cmd.Parameters.AddWithValue("@dc", dealerCustomer);
                            cmd.Parameters.AddWithValue("@p", productId);
                            cmd.Parameters.AddWithValue("@r", rate);
                            cmd.Parameters.AddWithValue("@q", qty);
                            cmd.Parameters.AddWithValue("@tot", total);
                            cmd.Parameters.AddWithValue("@ab", addedBy);
                            cmd.Parameters.AddWithValue("@abn", addedByName);
                            cmd.ExecuteNonQuery();
                        }

                        
                        int currentQty;
                        string getQtyQuery = "SELECT Qty FROM Products WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(getQtyQuery, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@id", productId);
                            currentQty = (int)cmd.ExecuteScalar();
                        }

                        int newQty = type == "PURCHASE" ? currentQty + qty : currentQty - qty;
                        if (newQty < 0)
                            throw new Exception("Недостаточно товара на складе для продажи.");

                        string updateQtyQuery = "UPDATE Products SET Qty = @q WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(updateQtyQuery, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@id", productId);
                            cmd.Parameters.AddWithValue("@q", newQty);
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public DataTable SelectAll()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT t.Id, t.Type, t.DealerCustomerName, p.Name AS Product,
                                        c.Title AS Category, t.Rate, t.Qty, t.Total, 
                                        t.TransDate, t.AddedByName
                                 FROM Transactions t
                                 INNER JOIN Products p ON t.ProductId = p.Id
                                 INNER JOIN Categories c ON p.CategoryId = c.Id
                                 ORDER BY t.TransDate DESC";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }

        public DataTable SelectByPeriod(DateTime from, DateTime to, string type = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT t.Id, t.Type, t.DealerCustomerName, p.Name AS Product,
                                        c.Title AS Category, t.Rate, t.Qty, t.Total, 
                                        t.TransDate, t.AddedByName
                                 FROM Transactions t
                                 INNER JOIN Products p ON t.ProductId = p.Id
                                 INNER JOIN Categories c ON p.CategoryId = c.Id
                                 WHERE t.TransDate BETWEEN @from AND @to";

                if (!string.IsNullOrEmpty(type))
                    query += " AND t.Type = @type";

                query += " ORDER BY t.TransDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@from", from);
                    cmd.Parameters.AddWithValue("@to", to);
                    if (!string.IsNullOrEmpty(type))
                        cmd.Parameters.AddWithValue("@type", type);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetProductsForCombo()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Name, Rate, Qty FROM Products ORDER BY Name";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }
    }
}