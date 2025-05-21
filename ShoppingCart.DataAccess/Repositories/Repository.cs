using Microsoft.EntityFrameworkCore;
using ShoppingCart.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private DbSet<T> dbSet;
        public Repository(DbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public IEnumerable<T> GetAll(string? includePropertirs = null, object someParam = null)
        {
            IQueryable<T> query = dbSet;

            if (includePropertirs != null)
                foreach (var item in includePropertirs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(item);

            return query.ToList();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.ToList();
        }
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet.Where(filter);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();
        }
    }
}
