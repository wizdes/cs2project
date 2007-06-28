using System.Collections.Generic;
using System.Data;
using System.IO;
using FirebirdSql.Data.FirebirdClient;

namespace CS2.Repositories
{
    public class FirebirdFilesRepository : IFilesRepository
    {
        private readonly string connectionString;

        public FirebirdFilesRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Add(FileInfo file)
        {
            using(FbConnection conn = new FbConnection(connectionString))
            {
                conn.Open();
                FbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO Files '" + file.FullName + "'";
                cmd.ExecuteNonQuery();
            }
        }

        public void Remove(FileInfo file)
        {
            using (FbConnection conn = new FbConnection(connectionString))
            {
                conn.Open();
                FbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM Files WHERE FileName='" + file.FullName + "'";
                cmd.ExecuteNonQuery();
            }
        }

        public bool Contains(FileInfo file)
        {
            using (FbConnection conn = new FbConnection(connectionString))
            {
                conn.Open();
                FbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT COUNT(*) FROM Files WHERE FileName='" + file.FullName + "'";

                // TODO: check this, I don't remeber sql!
                return (int)cmd.ExecuteScalar() == 1;
            }
        }

        public IEnumerable<string> GetAll()
        {
            DataTable dt = new DataTable();
            using(FbDataAdapter da = new FbDataAdapter("select * Files", connectionString))
                da.Fill(dt);

            foreach(DataRow row in dt.Rows)
                yield return row["FileName"] as string;
        }
    }
}