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
    /// 勤怠データのリポジトリサービス
    /// </summary>
    public class AttendanceRepository : IRepository<Attendance, string>
    {
        /// <summary>DB接続文字列</summary>
        private readonly string connectionString;
        /// <summary>DBテーブル名</summary>
        private readonly string tableName = "attendance_tbl";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionString">DB接続文字列</param>
        public AttendanceRepository(string connectionString)
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
                        db.Execute($"CREATE TABLE IF NOT EXISTS {tableName} ( UserId varchar(100) NOT NULL, AttendanceCount serial NOT NULL, PunchInTimeStamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP, PunchOutTimeStamp TIMESTAMP, PRIMARY KEY (UserId, AttendanceCount));");
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
        public void Add(Attendance item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"INSERT INTO {tableName} (UserId, AttendanceCount) VALUES ('{item.UserId}', (SELECT COUNT(*) from {tableName} where UserId = '{item.UserId}') + 1);", tran);
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
        /// レコードを削除します
        /// </summary>
        public void Remove(string id)
        {
            // Not Supported
            if (id == null)
            {
                return;
            }
        }
        /// <summary>
        /// レコードを更新します
        /// ※多分、システム不具合による打刻時刻の訂正とかも必要(サーバのマザボ電池がなくなったら時刻初期化される等)
        /// </summary>
        public bool Update(Attendance item)
        {
            // Not Supported
            if (item.UserId == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 任意のレコードを取得します
        /// </summary>
        /// <param name="id">セッションID</param>
        public Attendance FindById(string id)
        {
            // Not Supported
            if (id == null)
            {
                return null;
            }
            return new Attendance();
            
        }
        /// <summary>
        /// 有効なレコードを全取得します
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Attendance> FindAll()
        {
            // Not Supported
            return new List<Attendance>();
        }
    }
}
