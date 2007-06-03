using System;

namespace CS2.Core.Models
{
    public class SubversionSourceCodeRepository : ISourceCodeRepository
    {
        private Uri path;
        private string username;
        private string password;

        public Uri Path
        {
            get { return path; }
            set { path = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public SubversionSourceCodeRepository(Uri path)
        {
            this.path = path;
        }

        public SubversionSourceCodeRepository(Uri path, string username, string password) : this(path)
        {
            this.username = username;
            this.password = password;
        }
    }
}
