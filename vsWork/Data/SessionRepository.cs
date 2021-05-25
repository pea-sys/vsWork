using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using Dapper;

namespace vsWork.Data
{
    public class SessionRepository: IRepository<Session, string>
    {
        private readonly string connectionString;
        private const string tableName = "session_tbl";

        public SessionRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }

        /// <summary>
        /// 一時テーブルの方が良いかもしれないが未検証
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
                        db.Execute($"CREATE TABLE IF NOT EXISTS {tableName} (session_id text PRIMARY KEY, user_id varchar(100) NOT NULL, create_timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP);");
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }

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
                    }
                }
            }
        }
        public void Add(Session item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"INSERT INTO {tableName} (session_id, user_id) VALUES ('{item.SessionId}', '{item.UserId}');", tran);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }

        public IEnumerable<Session> FindAll()
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<Session>($"SELECTsession_id,  user_id  FROM {tableName}");
            }
        }

        public Session FindById(string id)
        {
            if (id == null)
            {
                return null;
            }
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<Session>($"SELECT session_id, user_id FROM {tableName} WHERE session_id = '{id}' LIMIT 1").FirstOrDefault();
            }
        }

        public void Remove(string id)
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
                        db.Execute($"DELETE FROM {tableName} WHERE session_id = '{id}'", tran);
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

        public bool Update(Session item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        var count = db.Execute($"UPDATE {tableName} SET user_id = {item.UserId} WHERE session_id = '{item.SessionId}'", tran);
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
