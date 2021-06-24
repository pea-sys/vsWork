using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using Dapper;

namespace vsWork.Data
{
    public class HolidayRepository : IRepository<Holiday, (int organizationId, DateTime Datetime)>
    {
        // <summary>DB接続文字列</summary>
        private readonly string connectionString;
        /// <summary>DBテーブル名</summary>
        private readonly string tableName = "holiday_tbl";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionString">DB接続文字列</param>
        public HolidayRepository(string connectionString)
        {
            this.connectionString = connectionString;
#if DEBUG
            CreateTable();
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

        public void CreateTable()
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"CREATE TABLE IF NOT EXISTS {tableName} (OrganizationId integer  NOT NULL REFERENCES  organization_tbl ON DELETE CASCADE, Date TIMESTAMP NOT NULL, HolidayType integer, Name text NOT NULL, PRIMARY KEY ( OrganizationId, Date));");
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
        public void Add(Holiday item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"INSERT INTO {tableName} (OrganizationId, Date, HolidayType, Name) VALUES ('{item.OrganizationId}','{item.Date.ToShortDateString()}', '{(int)item.HolidayType}', '{item.Name}');", tran);
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
        public IEnumerable<Holiday> FindAll()
        {
            throw new NotImplementedException();
        }

        public Holiday FindById((int organizationId, DateTime Datetime) keys)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                try
                {
                    return db.Query<Holiday>($"SELECT OrganizationId, Date, HolidayType, Name FROM {tableName} WHERE OrganizationId = '{keys.organizationId} & Date = '{keys.Datetime.ToShortDateString()}' ' LIMIT 1").FirstOrDefault();
                }
                catch
                {
                    throw;
                }
            }
        }

        public void Remove((int organizationId, DateTime Datetime) keys)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"DELETE FROM {tableName} WHERE OrganizationId = '{keys.organizationId}' and Date = '{keys.Datetime.ToShortDateString()}'", tran);
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

        public bool Update(Holiday item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        var count = db.Execute($"UPDATE {tableName} SET HolidayType = '{(int)item.HolidayType}' ,Name = '{item.Name}' WHERE OrganizationId = '{item.OrganizationId}' &  Date = '{item.Date}'", tran);
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
    }
}
