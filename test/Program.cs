using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityMappingToSql;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            EntityMappingToSql.EntityRepository<User> repository = new EntityMappingToSql.EntityRepository<User>();
            User u = new User();
            Console.WriteLine(repository.Insert(u));
            Console.Read();
        }


    }

    public class User:EntityBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }
    }
}
