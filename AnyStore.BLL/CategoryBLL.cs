using System.Data;
using AnyStore.DAL;

namespace AnyStore.BLL
{
    public class CategoryBLL
    {
        private readonly CategoryDAL _dal = new CategoryDAL();

        public DataTable GetAll() => _dal.SelectAll();

        public DataTable Search(string keyword)
        {
            return string.IsNullOrWhiteSpace(keyword) ? _dal.SelectAll() : _dal.Search(keyword.Trim());
        }

        public string Add(string title, string description, int addedBy)
        {
            if (string.IsNullOrWhiteSpace(title)) return "Введите название категории.";
            return _dal.Insert(title.Trim(), description, addedBy) ? "OK" : "Не удалось добавить категорию.";
        }

        public string Update(int id, string title, string description)
        {
            if (id <= 0) return "Выберите категорию.";
            if (string.IsNullOrWhiteSpace(title)) return "Введите название.";
            return _dal.Update(id, title.Trim(), description) ? "OK" : "Не удалось обновить категорию.";
        }

        public string Delete(int id)
        {
            if (id <= 0) return "Выберите категорию для удаления.";
            try
            {
                return _dal.Delete(id) ? "OK" : "Не удалось удалить категорию.";
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }
    }
}