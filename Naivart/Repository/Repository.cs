using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Naivart.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext DbContext;

        protected DbSet<T> Entities;
        public Repository(ApplicationDbContext context)
        {
            DbContext = context;
            Entities = DbContext.Set<T>();
        }

        public void Add(T entity)
        {
            DbContext.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            DbContext.Set<T>().AddRange(entities);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return DbContext.Set<T>().Where(expression);
        }

        public IEnumerable<T> GetAll()
        {
            return DbContext.Set<T>().ToList();
        }

        public T GetById(long id)
        {
            return DbContext.Set<T>().Find(id);
        }

        public void Remove(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            DbContext.Set<T>().RemoveRange(entities);
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            return DbContext.Set<T>().Any(expression);
        }
        public T FirstOrDefault(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return Entities.Where(predicate).FirstOrDefault();
        }
        public IEnumerable<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return Entities.Where(predicate);
        }
        public IEnumerable<T> Include(params Expression<Func<T, object>>[] includes)
        {
            DbSet<T> dbSet = DbContext.Set<T>();

            IEnumerable<T> query = null;
            foreach (var include in includes)
            {
                query = dbSet.Include(include);
            }

            return query ?? dbSet;
        }
        public void UpdateState(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
