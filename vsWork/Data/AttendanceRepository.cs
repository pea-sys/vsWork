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
        [Obsolete]
        public void Remove(string id)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// レコードを更新します
        /// </summary>
        [Obsolete]
        public bool Update(Attendance item)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// PunchOutTimestampを更新します
        /// </summary>
        public bool UpdateAtPunchOutTimestamp(Attendance item)
        {
            if (item.UserId == null)
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
                        var count = db.Execute($"UPDATE {tableName} SET PunchOutTimestamp = CURRENT_TIMESTAMP WHERE UserId = '{item.UserId}' and attendancecount = (SELECT COUNT(*) FROM attendance_tbl where userid = '{item.UserId}')", tran);
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
        [Obsolete]
        public Attendance FindById(string id)
        {
            throw new NotSupportedException();

        }
        /// <summary>
        /// 有効なレコードを全取得します
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public IEnumerable<Attendance> FindAll()
        {
            throw new NotSupportedException();
        }
    }
}
