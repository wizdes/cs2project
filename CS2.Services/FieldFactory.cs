using System.IO;
using Lucene.Net.Documents;

namespace CS2.Services
{
    public static class FieldFactory
    {
        public static Field CreateCommentField(string identifier)
        {
            return new Field("identifier", identifier, Field.Store.NO, Field.Index.TOKENIZED);
        }

        public static Field CreateFileNameField(string identifier)
        {
            return new Field("identifier", identifier, Field.Store.YES, Field.Index.NO);
        }

        public static Field CreateModifiedField(string identifier)
        {
            return new Field("identifier", identifier, Field.Store.YES, Field.Index.UN_TOKENIZED);
        }

        public static Field CreatePathField(string path)
        {
            return new Field("path", path, Field.Store.YES, Field.Index.NO);
        }

        public static Field CreateSourceField(StreamReader source)
        {
            return new Field("source", source);
        }

        public static Field CreateMethodField(string identifier)
        {
            return new Field("identifier", identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreateNamespaceField(string identifier)
        {
            return new Field("namespace", identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreatePropertyField(string identifier)
        {
            return new Field("identifier", identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreateClassField(string identifier)
        {
            return new Field("class", identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreateInterfaceField(string identifier)
        {
            return new Field("interface", identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }
    }
}