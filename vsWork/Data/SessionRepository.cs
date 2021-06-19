using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using Dapper;

namespace vsWork.Data
{
    /// <summary>
    /// セッションデータのリポジトリサービス
    /// </summary>
    public class SessionRepository: IRepository<Session, string>
    {
        /// <summary>DB接続文字列</summary>
        private readonly string connectionString;
        /// <summary>DBテーブル名</summary>
        private readonly string tableName = "session_tbl";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionString">DB接続文字列</param>
        public SessionRepository(string connectionString)
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
                        db.Execute($"CREATE TABLE IF NOT EXISTS {tableName} (SessionId text PRIMARY KEY, UserId varchar(100) NOT NULL, CreateTimeStamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP);");
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
        public void Add(Session item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"INSERT INTO {tableName} (SessionId, UserId) VALUES ('{item.SessionId}', '{item.UserId}');", tran);
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
                        db.Execute($"DELETE FROM {tableName} WHERE SessionId = '{id}'", tran);
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
        public bool Update(Session item)
        {
            if (item.SessionId == null)
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
                        var count = db.Execute($"UPDATE {tableName} SET UserId = '{item.UserId}' WHERE SessionId = '{item.SessionId}'", tran);
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
        public Session FindById(string id)
        {
            if (id == null)
            {
                return null;
            }
            using (IDbConnection db = Connection)
            {
                db.Open();
                try
                {
                    return db.Query<Session>($"SELECT SessionId, UserId FROM {tableName} WHERE SessionId = '{id}' LIMIT 1").FirstOrDefault();
                }
                catch
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// 有効なレコードを全取得します
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Session> FindAll()
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<Session>($"SELECT SessionId,  UserId  FROM {tableName}");
            }
        }
    }
}
