﻿using System;
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

            DateTime now = DateTime.Now;
            for (int i = 0; i < 200; i++)
            {
                now = now.AddDays(1);
                if (now.DayOfWeek == DayOfWeek.Saturday)
                {
                    if (FindById((0, now)) == null)
                    {
                        Add(new Holiday { OrganizationId = 0, Date=now, Name = $"名前{i}", Target = ApplyTargetType.Organization });
                    }
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

        public void CreateTable()
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"CREATE TABLE IF NOT EXISTS {tableName} (OrganizationId integer  NOT NULL REFERENCES  organization_tbl ON DELETE CASCADE, Date date NOT NULL, HolidayType integer, Name text NOT NULL, Target integer, PRIMARY KEY ( OrganizationId, Date, Target));");
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
                        db.Execute($"INSERT INTO {tableName} (OrganizationId, Date, HolidayType, Name, Target) VALUES ('{item.OrganizationId}','{item.Date.ToShortDateString()}', '{(int)item.HolidayType}', '{item.Name}', '{(int)item.Target}');", tran);
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
            throw new NotSupportedException();
        }
        public IEnumerable<Holiday> FindAllByOrganization(Organization organization)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                try
                {
                    if (!organization.HolidayEnable)
                    {
                        return db.Query<Holiday>($"SELECT OrganizationId, Date, HolidayType, Name, Target FROM {tableName} WHERE OrganizationId = '{organization.OrganizationId}' and Target = 2 ORDER BY Date;");
                    }
                    else
                    {
                        return db.Query<Holiday>($"SELECT OrganizationId, Date, HolidayType, Name, Target FROM {tableName} WHERE OrganizationId = '{organization.OrganizationId}' or Target = 1 ORDER BY Date;");
                    }

                }
                catch
                {
                    throw;
                }
            }
        }
        public IEnumerable<Holiday> FindAllByTarget(ApplyTargetType targetType)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                try
                {
                    return db.Query<Holiday>($"SELECT OrganizationId, Date, HolidayType, Name, Target FROM {tableName} WHERE Target = {(int)targetType} ORDER BY Date;");
                }
                catch
                {
                    throw;
                }
            }
        }
        public Holiday FindById((int organizationId, DateTime Datetime) keys)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                try
                {
                    return db.Query<Holiday>($"SELECT OrganizationId, Date, HolidayType, Name FROM {tableName} WHERE OrganizationId = {keys.organizationId} and Date = '{keys.Datetime.Date.ToShortDateString()}' LIMIT 1;").FirstOrDefault();
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
        public void RemoveByTargetAndYear(ApplyTargetType targetType, int year)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"DELETE FROM {tableName} WHERE Target = {(int)targetType} and date::date >= '{year}-01-01' and date::date <= '{year}-12-31'" , tran);
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
                        var count = db.Execute($"UPDATE {tableName} SET HolidayType = {(int)item.HolidayType} , Name = '{item.Name}' WHERE OrganizationId = '{item.OrganizationId}' and Date = '{item.Date.ToShortDateString()}';", tran);
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
