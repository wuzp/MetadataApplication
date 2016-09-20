using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace EntityMappingToSql
{
    public sealed class DB
    {
        private static DbConnection GetConnection(string connString)
        {
            DbConnection conn = new SqlConnection(connString);
            conn.Open();
            return conn;
        }
        public static List<T> ExecuteAndGetInstanceList<T>(string connString, string sql, Action<ParameterSet> dbParames, Action<IDataReader, T> readMapper, bool useTransaction = false) where T : new()
        {
            using (var conn = GetConnection(connString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                DbTransaction dbTransaction;
                DbCommand cmd;
                List<T> list = new List<T>();
                CreateCommand(sql, dbParames, useTransaction, conn, out dbTransaction, out cmd);
                try
                {
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T instance = new T();
                            readMapper(reader, instance);
                            list.Add(instance);
                        }
                        if (dbTransaction != null)
                        {
                            dbTransaction.Commit();
                        }
                    }
                    return list;
                }
                catch
                {
                    if (dbTransaction != null)
                    {
                        dbTransaction.Rollback();
                    }
                    throw;

                }
            }
        }
        public static T ExecuteAndGetInstance<T>(string connString, string sql, Action<ParameterSet> dbParames, Action<IDataReader, T> readMapper, bool useTransaction = false) where T : new()
        {
            using (var conn = GetConnection(connString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                DbTransaction dbTransaction;
                DbCommand cmd;
                T instance = new T();
                CreateCommand(sql, dbParames, useTransaction, conn, out dbTransaction, out cmd);
                try
                {
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            readMapper(reader, instance);
                        }
                        if (dbTransaction != null)
                        {
                            dbTransaction.Commit();
                        }
                    }
                    return instance;
                }
                catch
                {
                    if (dbTransaction != null)
                    {
                        dbTransaction.Rollback();
                    }
                    throw;

                }
            }
        }
        public static T ExecuteScalar<T>(string connString, string sql, Action<ParameterSet> dbParames, bool useTransaction = false)
        {
            return ExecuteCommand<T>(connString, sql, dbParames, cmd =>
            {
                return (T)Convert.ChangeType(cmd.ExecuteScalar(), typeof(T));
            }, useTransaction);
        }
        public static int ExecuteNonQuery(string connString, string sql, Action<ParameterSet> dbParames, bool useTransaction = false)
        {
            return ExecuteCommand<int>(connString, sql, dbParames, cmd => { return cmd.ExecuteNonQuery(); }, useTransaction);
        }
        private static T ExecuteCommand<T>(string connString, string sql, Action<ParameterSet> dbParames, Func<DbCommand, T> excuteCommand, bool useTransaction = false)
        {
            using (var conn = GetConnection(connString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                DbTransaction dbTransaction;
                DbCommand cmd;
                CreateCommand(sql, dbParames, useTransaction, conn, out dbTransaction, out cmd);
                try
                {
                    var result = excuteCommand(cmd);
                    if (dbTransaction != null)
                    {
                        dbTransaction.Commit();
                    }
                    return result;
                }
                catch
                {
                    if (dbTransaction != null)
                    {
                        dbTransaction.Rollback();
                    }
                    throw;

                }
            }

        }
        private static void CreateCommand(string sql, Action<ParameterSet> dbParames, bool useTransaction, DbConnection conn, out DbTransaction dbTransaction, out DbCommand cmd)
        {
            dbTransaction = null;
            cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (dbParames != null)
            {
                ParameterSet set = new ParameterSet(cmd);
                dbParames(set);
            }
            if (useTransaction)
            {
                dbTransaction = conn.BeginTransaction();
                cmd.Transaction = dbTransaction;
            }
        }
    }
    public static class DataReaderExtend
    {
        public static T Get<T>(this IDataReader reader, string fieldName)
        {
            return reader.IsDBNull(reader.GetOrdinal(fieldName)) ? default(T) : (T)(ChangeType(reader[fieldName], typeof(T)));
        }
        private static object ChangeType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            if (conversionType.IsEnum)
                return Enum.ToObject(conversionType, value);
            else
                return Convert.ChangeType(value, conversionType);
        }
    }
    public class ParameterSet
    {
        private DbCommand _cmd;
        private DbParameterCollection parameters;
        public ParameterSet(DbCommand cmd)
        {
            _cmd = cmd;
            parameters = cmd.Parameters;
        }
        private static class ParameterFactory
        {
            public static DbParameter GetParameter()
            {
                return new SqlParameter();
            }
        }
        public void AddWithValue(string ParameterName, object value)
        {
            DbParameter paras = ParameterFactory.GetParameter();
            paras.ParameterName = ParameterName;
            paras.Value = value;
            parameters.Add(paras);
        }

    }
}
