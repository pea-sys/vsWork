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
    public class UserRepository : IRepository<User, string>
    {
        private readonly string connectionString;
        private const string tableName = "user_tbl";

        public UserRepository(string connectionString)
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

        public void CreateTable()
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"CREATE TABLE IF NOT EXISTS {tableName} (id varchar(100) CONSTRAINT firstkey PRIMARY KEY, password bytea NOT NULL, name varchar(100));");
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

        public void Add(User item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute($"INSERT INTO {tableName} (id, password) VALUES ('{item.Id}', encrypt(convert_to('{item.Password}','UTF8'), 'pass', 'aes'));", tran);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }

        public IEnumerable<User> FindAll()
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<User>($"SELECT id, convert_from(decrypt(password, 'pass'::bytea, 'aes') ,'UTF8') as password, name , activedate FROM {tableName}");
            }
        }

        public User FindById(string id)
        {
            if (id == null)
            {
                return null;
            }
            using (IDbConnection db = Connection)
            {
                db.Open();
                return db.Query<User>($"SELECT id, convert_from(decrypt(password, 'pass'::bytea, 'aes'),'UTF8') as password FROM {tableName} WHERE id = '{id}' LIMIT 1").FirstOrDefault();
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
                        db.Execute($"DELETE FROM {tableName} WHERE id = '{id}'",tran);
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

        public bool Update(User item)
        {
            using (IDbConnection db = Connection)
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        var count = db.Execute($"UPDATE {tableName} SET password = encrypt(convert_to('{item.Password}','UTF8'), 'pass', 'aes') WHERE id = '{item.Id}'",tran);
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
