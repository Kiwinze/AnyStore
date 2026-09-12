using AnyStore.BLL;
using System;
using System.Data;
using System.Windows.Forms;

namespace AnyStore.UI
{
    public partial class frmLogin : Form
    {
        private readonly LoginBLL _loginBll = new LoginBLL();

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();
                string userType = cmbUserType.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userType))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Предупреждение",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dt = _loginBll.Login(username, password, userType);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    MessageBox.Show($"Добро пожаловать, {row["FullName"]}!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Открываем главную форму
                    frmMain main = new frmMain(
                        Convert.ToInt32(row["Id"]),
                        row["FullName"].ToString(),
                        row["UserType"].ToString());
                    this.Hide();
                    main.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин, пароль или тип пользователя.", "Ошибка входа",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к БД: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}