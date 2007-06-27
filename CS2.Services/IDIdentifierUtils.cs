using System.IO;

namespace CS2.Services
{
    public static class IDIdentifierUtils
    {
        public static string GetPathFromIdentifier(string identifier)
        {
            string url = identifier.Replace('\u0000', '/'); // replace nulls with slashes
            return url.Substring(0, (url.LastIndexOf('/')) - (0)); // remove date from end
        }

        public static string GetIdentifierFromFile(FileInfo file)
        {
            if(!file.Exists)
                throw new FileNotFoundException("File not found", file.FullName);

            return
                file.FullName.Replace(Path.DirectorySeparatorChar, '\u0000') + "\u0000" + file.LastWriteTime.Ticks;
        }
    }
}