using System.Data;
using System.Data.SQLite;
using System.IO;

namespace CS2.Repositories
{
    public class SQLiteFilesRepository : DatabaseFilesRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteFilesRepository"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SQLiteFilesRepository(string connectionString) : base(connectionString, "System.Data.SQLite")
        {
            SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder(connectionString);

            if (!File.Exists(csb.DataSource))
            {
                SQLiteConnection.CreateFile(csb.DataSource);

                using(SQLiteConnection conn = new SQLiteConnection(csb.ConnectionString))
                {
                    conn.Open();
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "CREATE TABLE Files ( FileName VARCHAR(255))";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}