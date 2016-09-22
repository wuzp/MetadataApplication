using System;

namespace EntityMappingToSql
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        public bool IsMust { get; set; }

        public bool IsPrimarykey { get; set; }
    }
}