using System;
using System.Windows.Forms;
using AnyStore.BLL;

namespace AnyStore.UI
{
    public partial class frmUsers : Form
    {
        private readonly UserBLL _bll = new UserBLL();
        private readonly int _currentUserId;

        public frmUsers(int currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
        }

        private void frmUsers_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            dgvUsers.DataSource = _bll.GetAll();
            if (dgvUsers.Columns.Count > 0)
            {
                dgvUsers.Columns["Id"].HeaderText = "ID";
                dgvUsers.Columns["Username"].HeaderText = "Логин";
                dgvUsers.Columns["Password"].Visible = false;
                dgvUsers.Columns["FullName"].HeaderText = "ФИО";
                dgvUsers.Columns["Email"].HeaderText = "Email";
                dgvUsers.Columns["Contact"].HeaderText = "Телефон";
                dgvUsers.Columns["Address"].HeaderText = "Адрес";
                dgvUsers.Columns["Gender"].HeaderText = "Пол";
                dgvUsers.Columns["UserType"].HeaderText = "Тип";
                dgvUsers.Columns["AddedDate"].HeaderText = "Дата добавления";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string result = _bll.Add(
                txtUsername.Text, txtPassword.Text, txtFullName.Text,
                txtEmail.Text, txtContact.Text, txtAddress.Text,
                cmbGender.SelectedItem?.ToString(),
                cmbUserType.SelectedItem?.ToString() ?? "User",
                _currentUserId);

            if (result == "OK")
            {
                MessageBox.Show("Пользователь добавлен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            else
            {
                MessageBox.Show(result, "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                MessageBox.Show("Выберите пользователя из таблицы.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string result = _bll.Update(
                int.Parse(txtUserId.Text), txtUsername.Text, txtPassword.Text,
                txtFullName.Text, txtEmail.Text, txtContact.Text,
                txtAddress.Text, cmbGender.SelectedItem?.ToString(),
                cmbUserType.SelectedItem?.ToString() ?? "User");

            if (result == "OK")
            {
                MessageBox.Show("Данные обновлены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            else
            {
                MessageBox.Show(result, "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                MessageBox.Show("Выберите пользователя для удаления.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (int.Parse(txtUserId.Text) == _currentUserId)
            {
                MessageBox.Show("Нельзя удалить самого себя!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Удалить пользователя?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string result = _bll.Delete(int.Parse(txtUserId.Text));
                if (result == "OK")
                {
                    MessageBox.Show("Пользователь удалён!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadData();
                }
                else
                {
                    MessageBox.Show(result, "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtUserId.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtContact.Clear();
            txtAddress.Clear();
            cmbGender.SelectedIndex = -1;
            cmbUserType.SelectedIndex = -1;
            txtSearch.Clear();
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvUsers.Rows[e.RowIndex];
                txtUserId.Text = row.Cells["Id"].Value?.ToString();
                txtUsername.Text = row.Cells["Username"].Value?.ToString();
                txtPassword.Text = row.Cells["Password"].Value?.ToString();
                txtFullName.Text = row.Cells["FullName"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtContact.Text = row.Cells["Contact"].Value?.ToString();
                txtAddress.Text = row.Cells["Address"].Value?.ToString();
                cmbGender.SelectedItem = row.Cells["Gender"].Value?.ToString();
                cmbUserType.SelectedItem = row.Cells["UserType"].Value?.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dgvUsers.DataSource = _bll.Search(txtSearch.Text);
        }
    }
}