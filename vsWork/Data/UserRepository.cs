using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;
using System.Text;
using System.Diagnostics;

namespace vsWork.Data
{
    /// <summary>
    /// ユーザデータのリポジトリサービス
    /// </summary>
    public class UserRepository : IRepository<User, string>
    {
        /// <summary>DB接続文字列</summary>
        private readonly string connectionString;
        /// <summary>DBテーブル名</summary>
        private const string tableName = "user_tbl";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionString">DB接続文字列</param>
        public UserRepository(string connectionString)
        {
            this.connectionString = connectionString;
#if DEBUG
            CreateTable();
            if (FindById("helloworld") == null)
            {
                Add(new User { UserId = "helloworld", Password = "helloworld", UserName = "UserName", Rank = User.RankType.SystemAdmin, OrganizationId=0 });
                Add(new User { UserId = "apple", Password = "apple", UserName = "apple", Rank = User.RankType.OrganizationAdmin, OrganizationId = 1 });

                int organizationId = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (i % 2 == 0)
                    {
                        organizationId = 1;
                    }
                    else
                    {
                        organizationId = 2;
                    }
                    string valStr = string.Empty;
                    for (int j = 0; j < 5; j++)
                    {
                        valStr += i.ToString();
                    }
                    Add(new User { UserId = valStr, Password = valStr, UserName = valStr, Rank = User.RankType.General, OrganizationId = organizationId });
                }
            }
#endif
        }
        /// <summary>
        /// DBコネクションプロパティ[R]
        /// </summary>
        internal IDbConnection Connection
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
                        db.Execute($"CREATE TABLE IF NOT EXISTS {tableName} (UserId varchar(100) NOT NULL PRIMARY KEY, password bytea NOT NULL, UserName varchar(100), Rank integer NOT NULL default {(int)User.RankType.General} , OrganizationId integer NOT NULL REFERENCES organization_tbl ON DELETE CASCADE);");
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
        public void Add(User item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"INSERT INTO {tableName} (UserId, password, UserName, Rank, OrganizationId) VALUES ('{item.UserId}', encrypt(convert_to('{item.Password}','UTF8'), 'pass', 'aes'),'{item.UserName}','{(int)item.Rank}','{item.OrganizationId}');", tran);
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
                        db.Execute($"DELETE FROM {tableName} WHERE UserId = '{id}'",tran);
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
        /// <param name="item">ユーザ情報</param>
        public bool Update(User item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        var count = db.Execute($"UPDATE {tableName} SET password = encrypt(convert_to('{item.Password}','UTF8'), 'pass', 'aes'), UserName = '{item.UserName}' WHERE Userid = '{item.UserId}'",tran);
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
        /// <param name="id">ユーザID</param>
        public User FindById(string id)
        {
            if (id == null)
            {
                return null;
            }
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<User>($"SELECT UserId, convert_from(decrypt(password, 'pass'::bytea, 'aes'),'UTF8') as password, UserName, Rank, OrganizationId FROM {tableName} WHERE UserId = '{id}' LIMIT 1").FirstOrDefault();
            }
        }
        /// <summary>
        /// 有効なレコードを全取得します
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> FindAll()
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<User>($"SELECT UserId, convert_from(decrypt(password, 'pass'::bytea, 'aes') ,'UTF8') as password, UserName, Rank, OrganizationId FROM {tableName}");
            }
        }
        /// <summary>
        /// 任意のレコード数を取得します
        /// </summary>
        /// <param name="id">ユーザID</param>
        public int GetUserCountByOrganizationId(int id)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<int>($"SELECT COUNT(*) FROM {tableName} WHERE OrganizationId = '{id}'").FirstOrDefault();
            }
        }
    }
}
