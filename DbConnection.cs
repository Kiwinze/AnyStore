using System.Data;
using System.Data.SqlClient;

namespace AnyStore.DAL
{

    public class DbConnection
    {

        public static readonly string ConnectionString =
            @"Data Source=DESKTOP-LH3UBGG;Initial Catalog=AnyStoreDB;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}