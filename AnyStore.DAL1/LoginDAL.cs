using System.Data;
using System.Data.SqlClient;

namespace AnyStore.DAL
{
    public class LoginDAL
    {

        public DataTable Login(string username, string password, string userType)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT Id, Username, FullName, Email, Contact, Address, Gender, UserType
                                 FROM Users
                                 WHERE Username = @Username
                                   AND Password = @Password
                                   AND UserType = @UserType";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@UserType", userType);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
}