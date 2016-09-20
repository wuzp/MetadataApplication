using System;

namespace EntityMappingToSql
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EntityAttribute : Attribute
    {
        public EntityAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; set; }
    }
}