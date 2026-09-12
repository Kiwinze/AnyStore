using System.Data;
using AnyStore.DAL;

namespace AnyStore.BLL
{
    public class LoginBLL
    {
        private readonly LoginDAL _dal = new LoginDAL();


        public DataTable Login(string username, string password, string userType)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(userType))
                return new DataTable();

            return _dal.Login(username.Trim(), password.Trim(), userType.Trim());
        }
    }
}