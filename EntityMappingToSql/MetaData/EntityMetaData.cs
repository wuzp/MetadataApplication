using System;

namespace EntityMappingToSql.MetaData
{
    public class EntityMetaData
    {
        public string TableName { get; set; }
        public string EntityName { get; set; }
        public Type EntityType { get; set; }
    }
}