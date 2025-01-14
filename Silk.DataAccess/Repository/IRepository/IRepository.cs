using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Silk.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T Generic can be Category,Product
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false); 
        // track false becoz - when something retrieved from db, entity framework tracks it's changes and no matter what if you wrote add/update/delete statement or not,
        //if it finds save statement , it will do the changes to DB therefore to turn off this feature.
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);

    }
}
