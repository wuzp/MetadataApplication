using System;

namespace EntityMappingToSql.MetaData
{
    /// <summary>
    ///     属性元数据
    /// </summary>
    public class PropertyMetaData
    {
        /// <summary>
        ///     实体类型
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        ///     属性类型
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        ///     属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        ///     映衬数据库列的名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        ///     长度
        /// </summary>
        public string ColumnLength { get; set; }

        /// <summary>
        ///     是否必填
        /// </summary>
        public string IsMust { get; set; }
    }
}