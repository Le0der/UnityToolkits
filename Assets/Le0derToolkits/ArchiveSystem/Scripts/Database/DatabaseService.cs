using System;
using System.Collections.Generic;
using System.Linq;
using SQLite4Unity3d;
using UnityEngine;

namespace Le0der.ArchiveSystem
{
    public class DatabaseService
    {
#if UNITY_EDITOR
        private const string m_dbPathFormat = "{0}/Le0derToolkits/ArchiveSystem/Demo/Data/{1}.db";
#else
        private const string m_dbPathFormat = "{0}/{1}.db";
#endif

        private string _databasePath;
        private SQLiteConnection _connection;

        public DatabaseService(string dbFileName)
        {
            // 在构造函数中初始化数据库连接
            _databasePath = string.Format(m_dbPathFormat, Application.dataPath, dbFileName);
            _connection = new SQLiteConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }

        #region 获取数据

        // 获取所有数据
        public List<T> LoadAllDatas<T>() where T : IDBDataBase, new()
        {
            try
            {
                var result = _connection.Table<T>().ToList();
                return result;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to load data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return null;
            }
        }

        // 根据Id加载数据
        public T LoadDataById<T>(int id) where T : IDBDataBase, new()
        {
            try
            {
                var table = _connection.Table<T>();
                var result = table != null ? table.FirstOrDefault(data => data.Id == id) : default(T);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to load data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return default(T);
            }
        }

        // 自定义条件获取数据
        public T LoadDataByWhere<T>(Func<T, bool> predicate) where T : IDBDataBase, new()
        {
            try
            {
                var table = _connection.Table<T>();
                var result = table != null ? table.FirstOrDefault(predicate) : default;
                return result;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to load data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return default;
            }
        }

        // 自定义条件获取符合条件的数组
        public List<T> LoadDatasByWhere<T>(Func<T, bool> predExpr) where T : IDBDataBase, new()
        {
            try
            {
                var result = _connection.Table<T>().Where(predExpr).ToList();
                return result;

            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to load data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return default;
            }
        }

        // 用于执行 SQL 查询并返回单个值，为了安全行，尽可能不要使，使用问号做占位符然后使用参数替换
        // 通常情况下，ExecuteScalar 用于执行聚合函数查询（如 COUNT、SUM、AVG 等）或者返回单个值的查询。
        public T LoadDataByQuery<T>(string query, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Debug.LogError("query is null or empty");
                return default;
            }

            try
            {
                var result = _connection.ExecuteScalar<T>(query, args);
                return result;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Unable to load data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return default;
            }
        }

        // 自定义sql语句获取数据，为了安全行，尽可能不要使，使用问号做占位符然后使用参数替换
        public List<T> LoadDatasByQuery<T>(string query, params object[] args) where T : IDBDataBase, new()
        {
            try
            {
                var result = _connection.Query<T>(query, args);
                return result;
            }
            catch (System.Exception e)
            {

                Debug.LogError($"Unable to load data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return null;
            }
        }

        // 获取指定表所对应的数据条数
        public int LoadDatasCount<T>() where T : IDBDataBase, new()
        {
            try
            {
                var result = _connection.Table<T>().Count();
                return result;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to load datas count due to: {e.Message}; StackTrace: {e.StackTrace}");
                return 0;
            }
        }

        // 获取指定表符合条件的数据条数
        public int LoadDatasCount<T>(Func<T, bool> predExpr) where T : IDBDataBase, new()
        {
            try
            {
                var result = _connection.Table<T>()
                                        .Where(predExpr)
                                        .Count();
                return result;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to load condition datas count due to: {e.Message}; StackTrace: {e.StackTrace}");
                return 0;
            }
        }

        // 获取所有数据中的具体分页数据
        public List<T> LoadDatasByPage<T>(int page, int pageSize) where T : IDBDataBase, new()
        {
            try
            {
                int offset = (page - 1) * pageSize;
                var result = _connection.Table<T>()             // 获取对应的表
                                        .Skip(offset)           // 跳过前面的记录
                                        .Take(pageSize)         // 取 pageSize 条记录
                                        .ToList();
                return result;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to load page datas due to: {e.Message}; StackTrace: {e.StackTrace}");
                return default;
            }
        }

        // 获取符合条件的数据中的分页数据
        public List<T> LoadDatasByPage<T>(int page, int pageSize, Func<T, bool> predExpr) where T : IDBDataBase, new()
        {
            try
            {
                int offset = (page - 1) * pageSize;
                var result = _connection.Table<T>()             // 获取对应的表
                                        .Where(predExpr)        // 筛选条件
                                        .Skip(offset)           // 跳过前面的记录
                                        .Take(pageSize)         // 取 pageSize 条记录
                                        .ToList();
                return result;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to load condition page datas due to: {e.Message}; StackTrace: {e.StackTrace}");
                return default;
            }
        }
        #endregion

        #region 保存数据

        // 保存数据
        public bool SaveData<T>(T data, CreateFlags createFlags = CreateFlags.None) where T : IDBDataBase, new()
        {
            // 尝试创建表格，如果已经存在了就不需要再创建了
            try
            {
                _connection.CreateTable<T>(createFlags);
            }
            catch (Exception)
            {
                Debug.Log("Table already exists, no need to create it.");
            }

            try
            {
                // 如果数据存在则更新，否则插入
                if (_connection.Find<T>(data.Id) != null)
                {
                    _connection.Update(data);
                }
                else
                {
                    _connection.Insert(data);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return false;
            }

        }

        // 批量保存数据
        public bool SaveData<T>(List<T> datas, CreateFlags createFlags = CreateFlags.None) where T : IDBDataBase, new()
        {
            // 尝试创建表格，如果已经存在了就不需要再创建了
            try
            {
                _connection.CreateTable<T>(createFlags);
            }
            catch (Exception)
            {
                Debug.Log("Table already exists, no need to create it.");
            }

            try
            {
                // 如果数据存在则更新，否则插入
                foreach (var data in datas)
                {
                    if (_connection.Find<T>(data.Id) != null)
                    {
                        _connection.Update(data);
                    }
                    else
                    {
                        _connection.Insert(data);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return false;
            }
        }

        #endregion

        #region 删除数据

        // 根据Id删除单个数据
        public bool DeleteDataById<T>(int id) where T : IDBDataBase, new()
        {
            try
            {
                _connection.Delete<T>(id);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to delete data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return false;
            }
        }

        // 清空表内数据
        public bool DeleteTableDatas<T>() where T : IDBDataBase, new()
        {
            try
            {
                _connection.DeleteAll<T>();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to delete data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return false;
            }
        }
        #endregion

        #region 数据库链接
        public void TryConnectToDatabase()
        {
            if (!CheckConnectionStatus())
            {
                _connection = new SQLiteConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            }
        }

        public void DisconnectFromDatabase()
        {
            // 断开数据库连接
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
                Debug.Log("Database disconnected.");
            }
            else
            {
                Debug.LogWarning("Database connection is already null.");
            }
        }
        private bool CheckConnectionStatus()
        {
            try
            {
                // 尝试执行一个简单的查询来检查连接状态
                _connection.ExecuteScalar<int>("SELECT 1");
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }
        #endregion

    }
}