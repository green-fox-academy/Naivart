using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T GetById(long id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        IEnumerable<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

        T FirstOrDefault(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        IEnumerable<T> Include(params Expression<Func<T, object>>[] includes);
        void UpdateState(T entity);

        bool Any(Expression<Func<T, bool>> expression);
    }
}
