﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using Dapper;

namespace vsWork.Data
{
    public class OrganizationRepository:IRepository<Organization, int>
    {
        /// <summary>DB接続文字列</summary>
        private readonly string connectionString;
        /// <summary>DBテーブル名</summary>
        private readonly string tableName = "organization_tbl";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionString">DB接続文字列</param>
        public OrganizationRepository(string connectionString)
        {
            this.connectionString = connectionString;
#if DEBUG
            CreateTable();
            if (FindById(0) == null)
            {
                Add(new Organization { OrganizationId = 0, OrganizationName = "ベンダー" });
                for (int i = 1; i < 30; i++)
                {
                    Add(new Organization { OrganizationId = i, OrganizationName = $"ユーザー会社{i}", HolidayEnable = true });
                }
            }
#endif
        }
        /// <summary>
        /// DBコネクションプロパティ[R]
        /// </summary>
        private IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }
        /// <summary>
        /// テーブルを作成します
        /// </summary>
        public void CreateTable()
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"CREATE TABLE IF NOT EXISTS {tableName} (OrganizationId integer PRIMARY KEY, OrganizationName varchar(100) NOT NULL,  HolidayEnable boolean, CreateTimeStamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP);");
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// テーブルを削除します
        /// </summary>
        public void DropTable()
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"DROP TABLE  IF EXISTS {tableName};");
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// レコードを追加します
        /// </summary>
        public void Add(Organization item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"INSERT INTO {tableName} (OrganizationId, OrganizationName, HolidayEnable) VALUES ('{item.OrganizationId}', '{item.OrganizationName}', '{item.HolidayEnable}');", tran);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// レコードを削除します
        /// </summary>
        public void Remove(int id)
        {
            if (id == null)
            {
                return;
            }
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"DELETE FROM {tableName} WHERE OrganizationId = '{id}'", tran);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// レコードを更新します
        /// </summary>
        public bool Update(Organization item)
        {
            if (item.OrganizationId == null)
            {
                return false;
            }
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        var count = db.Execute($"UPDATE {tableName} SET OrganizationName = '{item.OrganizationName}', HolidayEnable = '{item.HolidayEnable}' WHERE OrganizationId = '{item.OrganizationId}'", tran);
                        tran.Commit();
                        return count > 0;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// 任意のレコードを取得します
        /// </summary>
        /// <param name="id">セッションID</param>
        public Organization FindById(int id)
        {
            if (id == null)
            {
                return null;
            }
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<Organization>($"SELECT OrganizationId, OrganizationName, HolidayEnable FROM {tableName} WHERE OrganizationId = '{id}' LIMIT 1").FirstOrDefault();
            }
        }
        /// <summary>
        /// 有効なレコードを全取得します
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Organization> FindAll()
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<Organization>($"SELECT OrganizationId,  OrganizationName, HolidayEnable FROM {tableName} ORDER BY OrganizationId;");
            }
        }
    }
}
