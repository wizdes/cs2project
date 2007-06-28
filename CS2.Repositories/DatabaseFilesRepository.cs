using System;
using System.Data;
using System.Data.Common;
using System.IO;

namespace CS2.Repositories
{
    public abstract class DatabaseFilesRepository : IFilesRepository
    {
        private readonly string connectionString;
        private readonly DbProviderFactory fact;

        public DatabaseFilesRepository(string connectionString, string databaseProvider)
        {
            this.connectionString = connectionString;
            fact = DbProviderFactories.GetFactory(databaseProvider);
        }

        #region IFilesRepository Members

        public void Add(FileInfo file)
        {
            using(IDbConnection conn = fact.CreateConnection())
            {
                conn.ConnectionString = connectionString;
                conn.Open();
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO Files '" + file.FullName + "'";
                cmd.ExecuteNonQuery();
            }
        }

        public void Remove(FileInfo file)
        {
            using(IDbConnection conn = fact.CreateConnection())
            {
                conn.ConnectionString = connectionString;
                conn.Open();
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM Files WHERE FileName='" + file.FullName + "'";
                cmd.ExecuteNonQuery();
            }
        }

        public bool Contains(FileInfo file)
        {
            using(IDbConnection conn = fact.CreateConnection())
            {
                conn.ConnectionString = connectionString;
                conn.Open();
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT COUNT(*) FROM Files WHERE FileName='" + file.FullName + "'";

                return (int) cmd.ExecuteScalar() == 1;
            }
        }

        public string[] GetAll()
        {
            DataTable dt = new DataTable();
            using(DbDataAdapter da = fact.CreateDataAdapter())
            {
                DbConnection conn = fact.CreateConnection();
                conn.ConnectionString = connectionString;
                DbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Files";
                da.SelectCommand = cmd;
                da.Fill(dt);
            }

            return Array.ConvertAll<DataRow, string>(dt.Select(), delegate(DataRow row)
                                                                      {
                                                                          return
                                                                              row["FileName"] as
                                                                              string;
                                                                      });
        }

        #endregion
    }
}