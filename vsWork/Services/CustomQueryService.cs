using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;

namespace vsWork.Services
{
    public class CustomQueryService
    {
        /// <summary>DB接続文字列</summary>
        private readonly string connectionString;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionString">DB接続文字列</param>
        public CustomQueryService(string connectionString)
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
        /// バキューム処理
        /// </summary>
        public void Vacuum()
        {
            using (IDbConnection db = Connection)
            {
                // transactionの内側ではVACUUM実行不可
                db.Open();
                db.Execute("VACUUM;");
            }
        }
    }
}
