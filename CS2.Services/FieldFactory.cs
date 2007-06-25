using System.IO;
using Lucene.Net.Documents;

namespace CS2.Services
{
    public static class FieldFactory
    {
        public const string CommentFieldName = "comment";
        public const string FileNameFieldName = "fileName";
        public const string ModifiedFieldName = "modified";
        public const string PathFieldName = "path";
        public const string SourceFieldName = "source";
        public const string MethodFieldName = "method";
        public const string NameSpaceFieldName = "namespace";
        public const string PropertyFieldName = "property";
        public const string ClassFieldName = "class";
        public const string InterfaceFieldName = "interface";

        public static readonly DateTools.Resolution ModifiedResolution = DateTools.Resolution.SECOND;

        public static Field CreateCommentField(string identifier)
        {
            return new Field(CommentFieldName, identifier, Field.Store.NO, Field.Index.TOKENIZED);
        }

        public static Field CreateFileNameField(string identifier)
        {
            return new Field(FileNameFieldName, identifier, Field.Store.YES, Field.Index.NO);
        }

        public static Field CreateModifiedField(string identifier)
        {
            return new Field(ModifiedFieldName, identifier, Field.Store.YES, Field.Index.UN_TOKENIZED);
        }

        public static Field CreatePathField(string path)
        {
            return new Field(PathFieldName, path, Field.Store.YES, Field.Index.NO);
        }

        public static Field CreateSourceField(StreamReader source)
        {
            return new Field(SourceFieldName, source);
        }

        public static Field CreateMethodField(string identifier)
        {
            return new Field(MethodFieldName, identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreateNamespaceField(string identifier)
        {
            return new Field(NameSpaceFieldName, identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreatePropertyField(string identifier)
        {
            return new Field(PropertyFieldName, identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreateClassField(string identifier)
        {
            return new Field(ClassFieldName, identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }

        public static Field CreateInterfaceField(string identifier)
        {
            return new Field(InterfaceFieldName, identifier, Field.Store.YES, Field.Index.TOKENIZED);
        }
    }
}