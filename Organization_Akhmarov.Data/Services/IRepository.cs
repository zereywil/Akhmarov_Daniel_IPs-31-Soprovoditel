using System.Collections.Generic;

namespace Organization_Akhmarov.Data.Services
{
    public interface IRepository<T>
    {
        List<T> GetAll();

        //Получить запись 
        T? GetById(object id);

        // новая запись
        void Add(T entity);

        //Обновить запись
        void Update(T entity);

        //Удалить запись 
        void Delete(object id);
    }
}