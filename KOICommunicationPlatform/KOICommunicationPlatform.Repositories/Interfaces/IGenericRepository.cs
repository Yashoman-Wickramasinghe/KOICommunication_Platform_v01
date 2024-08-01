using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Repositories.Interfaces
{
    public interface IGenericRepository<T> : IDisposable
    {
        void Add(T entity);
        Task<T> AddAsync(T entity);
        void Delete(T entity);
        Task<T> DeleteAsync(T entity);
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        T GetById(object id);
        Task<T> GetByIdAsync(object id);
        void Update(T entity);

        Task<T> UpdateAsync(T entity);

       


        //IQueryable<T> GetQueryable();
        //List<T> GetAll();
        //List<T> GetList(Func<T, bool> where);
        //T GetSingle(Func<T, bool> where);
        //T Insert(T entity);
        //int Delete(Func<T, bool> where, T entity);
        //List<T> UpdateMany(List<T> entities);
        //T Update(Func<T, bool> where, T entity);
        //T Update(T entity);
        //T InsertOrUpdate(Func<T, bool> where, T entity);
        //int UpdateProperty(Func<T, bool> where, KeyValuePair<string, object> set);
        //int UpdatePropertyDecimal(Func<T, bool> where, KeyValuePair<string, object> set);
        //int UpdatePropertyForList(Func<T, bool> where, KeyValuePair<string, object> set);
        //bool IsExist(Func<T, bool> where);
        //int UpdateProperties(Func<T, bool> where, KeyValuePair<string, object> set1, KeyValuePair<string, object> set2);
    }

}
