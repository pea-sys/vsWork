using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using Dapper;
using vsWork.Utils;

namespace vsWork.Data
{
    public class UserStateRepository : IRepository<UserState, string>
    {
        /// <summary>DB接続文字列</summary>
        private readonly string connectionString;
        /// <summary>DBテーブル名</summary>
        private const string tableName = "userstate_tbl";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionString">DB接続文字列</param>
        public UserStateRepository(string connectionString)
        {
            this.connectionString = connectionString;
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
                        db.Execute($"CREATE TABLE IF NOT EXISTS {tableName} ( UserId varchar(100) NOT NULL PRIMARY KEY, State smallint, TimeStamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP);");
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
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
                    }
                }
            }
        }
        /// <summary>
        /// レコードを追加します
        /// </summary>
        [Obsolete("トリガー更新のみ許可")]
        public void Add(UserState item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// レコードを削除します
        /// </summary>
        [Obsolete("トリガー更新のみ許可")]
        public void Remove(string id)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// レコードを更新します
        /// ※多分、システム不具合による打刻時刻の訂正とかも必要(サーバのマザボ電池がなくなったら時刻初期化される等)
        /// </summary>
        [Obsolete("トリガー更新のみ許可")]
        public bool Update(UserState item)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// 任意のレコードを取得します
        /// </summary>
        /// <param name="id">セッションID</param>
        public UserState FindById(string id)
        {
            if (id == null)
            {
                return null;
            }
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<UserState>($"SELECT UserId, State, Timestamp FROM {tableName} WHERE UserId = '{id}' LIMIT 1").FirstOrDefault();
            }
        }
        /// <summary>
        /// 有効なレコードを全取得します
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public IEnumerable<UserState> FindAll()
        {
            throw new NotSupportedException();
        }
    }
}
