﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityMappingToSql.MetaData;

namespace EntityMappingToSql
{
    public class EntityRepository<T> : IRepository<T> where T : EntityBase
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, EntityMetaData> EntityMetaDataDic = new ConcurrentDictionary<RuntimeTypeHandle, EntityMetaData>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> InsertSqls = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private string connstring = "";
        public string Insert<T>(T entity)
        {
            Type type = typeof(T);
            var allProperties = TypePropertiesCache(type);
            string sql;
            if (!InsertSqls.TryGetValue(type.TypeHandle, out sql))
            {
                var name = GetTableName(type);
                var sbColumnList = new StringBuilder(null);
                for (int i = 0, c = allProperties.Count; i < c; i++)
                {
                    var property = allProperties.ElementAt(i);
                    sbColumnList.Append(property.Name);
                    if (i < c - 1)
                        sbColumnList.Append(", ");
                }
                var sbParameterList = new StringBuilder(null);
                for (int i = 0, c = allProperties.Count; i < c; i++)
                {
                    var property = allProperties.ElementAt(i);
                    sbParameterList.AppendFormat("@{0}", property.Name);
                    if (i < c - 1)
                        sbParameterList.Append(", ");
                }
                sql = $"insert into {name} ({sbColumnList}) values ({sbParameterList})";
                InsertSqls[type.TypeHandle] = sql;
            }
            //var result = DB.ExecuteNonQuery(connstring, sql, p =>
            //{
            //    for (int i = 0, c = allProperties.Count; i < c; i++)
            //    {
            //        var property = allProperties.ElementAt(i);
            //        p.AddWithValue(property.Name, Convert.ChangeType(property.GetValue(entity), property.PropertyType));
            //    }
            //});
            return sql;
        }

        private string GetTableName(Type type)
        {
            return GetEntityMetaData(type).TableName;
        }

        public string Update<T>(T entity)
        {
            throw new NotImplementedException();
        }

        public string Delete<T>(T entity)
        {
            throw new NotImplementedException();
        }

        public string GetById<T>(string id)
        {
            throw new NotImplementedException();
        }
        private static EntityMetaData GetEntityMetaData(Type type)
        {
            EntityMetaData entityMetaData;
            if (EntityMetaDataDic.TryGetValue(type.TypeHandle, out entityMetaData)) return entityMetaData;
            entityMetaData = new EntityMetaData();
            entityMetaData.EntityName = type.Name;
            entityMetaData.EntityType = type;
            EntityAttribute attibute = type.GetCustomAttribute(typeof(EntityAttribute)) as EntityAttribute;
            if (attibute != null)
            {
                entityMetaData.TableName = attibute.TableName;
            }
            else
            {
                entityMetaData.TableName = type.Name;
            }
            EntityMetaDataDic[type.TypeHandle] = entityMetaData;
            return entityMetaData;
        }

        private static List<PropertyInfo> TypePropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pis;
            if (TypeProperties.TryGetValue(type.TypeHandle, out pis))
            {
                return pis.ToList();
            }
            var properties = type.GetProperties().Where(p => p.CanWrite && p.CanRead).ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }
    }
}