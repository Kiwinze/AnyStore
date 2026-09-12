using System;
using System.Data;
using AnyStore.DAL;

namespace AnyStore.BLL
{
    public class TransactionBLL
    {
        private readonly TransactionDAL _dal = new TransactionDAL();

        public DataTable GetAll() => _dal.SelectAll();

        public DataTable GetByPeriod(DateTime from, DateTime to, string type = null)
        {
            if (from > to) throw new Exception("Дата начала больше даты окончания.");
            return _dal.SelectByPeriod(from, to, type);
        }

        public DataTable GetProductsForCombo() => _dal.GetProductsForCombo();

        public string Add(string type, string dealerCustomer, int productId,
                          decimal rate, int qty, decimal total, int addedBy, string addedByName)
        {
            if (string.IsNullOrWhiteSpace(type)) return "Выберите тип операции.";
            if (string.IsNullOrWhiteSpace(dealerCustomer)) return "Укажите поставщика/покупателя.";
            if (productId <= 0) return "Выберите товар.";
            if (qty <= 0) return "Количество должно быть больше нуля.";
            if (rate < 0) return "Цена не может быть отрицательной.";

            try
            {
                return _dal.Insert(type, dealerCustomer.Trim(), productId, rate, qty, total, addedBy, addedByName)
                    ? "OK" : "Не удалось сохранить транзакцию.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}