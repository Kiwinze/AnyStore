using System;
using System.Windows.Forms;
using AnyStore.BLL;

namespace AnyStore.UI
{
    public partial class frmTransactions : Form
    {
        private readonly TransactionBLL _bll = new TransactionBLL();

        public frmTransactions()
        {
            InitializeComponent();
        }

        private void frmTransactions_Load(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Today.AddMonths(-1);
            dtpTo.Value = DateTime.Today;

            cmbFilterType.Items.AddRange(new object[] { "Все", "PURCHASE", "SALE" });
            cmbFilterType.SelectedIndex = 0;

            dgvTransactions.DataSource = _bll.GetAll();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string type = cmbFilterType.SelectedItem.ToString();
            if (type == "Все") type = null;

            try
            {
                dgvTransactions.DataSource = _bll.GetByPeriod(dtpFrom.Value, dtpTo.Value, type);
                lblCount.Text = $"Записей: {dgvTransactions.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dgvTransactions.DataSource = _bll.GetAll();
            lblCount.Text = $"Записей: {dgvTransactions.Rows.Count}";
        }
    }
}