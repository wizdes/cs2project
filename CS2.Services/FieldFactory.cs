using System.IO;
using Lucene.Net.Documents;

namespace CS2.Services
{
    public static class FieldFactory
    {
        public static Field CreateCommentField(string comment)
        {
            return new Field("comment", comment, Field.Store.NO, Field.Index.TOKENIZED);
        }

        public static Field CreateFileNameField(string fileName)
        {
            return new Field("fileName", fileName, Field.Store.YES, Field.Index.NO);
        }

        public static Field CreateModifiedField(string modified)
        {
            return new Field("modified", modified, Field.Store.YES, Field.Index.UN_TOKENIZED);
        }

        public static Field CreatePathField(string path)
        {
            return new Field("path", path, Field.Store.YES, Field.Index.NO);
        }

        public static Field CreateSourceField(StreamReader source)
        {
            return new Field("source", source);
        }

        public static Field CreateMethodField(string method)
        {
            return new Field("method", method, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreateNamespaceField(string namespaceName)
        {
            return new Field("namespace", namespaceName, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreatePropertyField(string property)
        {
            return new Field("property", property, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreateClassField(string className)
        {
            return new Field("class", className, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreateInterfaceField(string identifier)
        {
            return new Field("interface", identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }
    }
}