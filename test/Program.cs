using System;
using EntityMappingToSql;

namespace test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var repository = new EntityRepository<User>();
            var u = new User();
            Console.WriteLine("User实体新增");
            Console.WriteLine(repository.Insert(u));
            Console.WriteLine("User实体查询");
            Console.WriteLine(repository.GetById<User>(1));
            Console.Read();
        }
    }

    [Entity("User")]
    public class User : EntityBase
    {
        [Property(IsPrimarykey =true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}