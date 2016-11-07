using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityMappingToSql
{
   public interface IRepository<T>
   {
       string Insert<T>(T entity);

       string Update<T>(T entity);

       string Delete<T>(T entity);

       string GetById<T>(object id);
   }
}
