using System.Data;
using AnyStore.DAL;

namespace AnyStore.BLL
{
    public class ProductBLL
    {
        private readonly ProductDAL _dal = new ProductDAL();

        public DataTable GetAll() => _dal.SelectAll();

        public DataTable Search(string keyword)
        {
            return string.IsNullOrWhiteSpace(keyword) ? _dal.SelectAll() : _dal.Search(keyword.Trim());
        }

        public string Add(string name, int categoryId, string description, decimal rate, int qty, int addedBy)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Введите название товара.";
            if (categoryId <= 0) return "Выберите категорию.";
            if (rate < 0) return "Цена не может быть отрицательной.";
            if (qty < 0) return "Количество не может быть отрицательным.";

            return _dal.Insert(name.Trim(), categoryId, description, rate, qty, addedBy)
                ? "OK" : "Не удалось добавить товар.";
        }

        public string Update(int id, string name, int categoryId, string description, decimal rate, int qty)
        {
            if (id <= 0) return "Выберите товар.";
            if (string.IsNullOrWhiteSpace(name)) return "Введите название товара.";
            if (categoryId <= 0) return "Выберите категорию.";

            return _dal.Update(id, name.Trim(), categoryId, description, rate, qty)
                ? "OK" : "Не удалось обновить товар.";
        }

        public string Delete(int id)
        {
            if (id <= 0) return "Выберите товар для удаления.";
            try
            {
                return _dal.Delete(id) ? "OK" : "Не удалось удалить товар.";
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }

        public int GetQuantity(int productId) => _dal.GetQuantity(productId);
    }
}