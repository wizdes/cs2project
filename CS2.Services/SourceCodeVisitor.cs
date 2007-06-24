using DDW;
using Lucene.Net.Documents;

namespace CS2.Services
{
    public class SourceCodeVisitor : AbstractVisitor
    {
        private readonly Document document;

        public SourceCodeVisitor(Document document)
        {
            this.document = document;
        }

        public override object VisitCommentStatement(CommentStatement commentStatement, object data)
        {
            document.Add(FieldFactory.CreateCommentField(commentStatement.Comment));
            return base.VisitCommentStatement(commentStatement, data);
        }

        public override object VisitMethodDeclaration(MethodNode methodDeclaration, object data)
        {
            document.Add(FieldFactory.CreateMethodField(methodDeclaration.Names[0].GenericIdentifier));
            return base.VisitMethodDeclaration(methodDeclaration, data);
        }

        public override object VisitNamespaceDeclaration(NamespaceNode namespaceDeclaration, object data)
        {
            string name = namespaceDeclaration.Name != null ? namespaceDeclaration.Name.GenericIdentifier : "Global";

            document.Add(FieldFactory.CreateNamespaceField(name));
            return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
        }

        public override object VisitPropertyDeclaration(PropertyNode propertyDeclaration, object data)
        {
            document.Add(FieldFactory.CreatePropertyField(propertyDeclaration.Names[0].QualifiedIdentifier));
            return base.VisitPropertyDeclaration(propertyDeclaration, data);
        }

        public override object VisitClassDeclaration(ClassNode classDeclaration, object data)
        {
            document.Add(FieldFactory.CreateClassField(classDeclaration.Name.Identifier));
            return base.VisitClassDeclaration(classDeclaration, data);
        }

        public override object VisitInterfaceDeclaration(InterfaceNode interfaceDeclaration, object data)
        {
            //            document.Add(new Field("interface", interfaceDeclaration.Name.Identifier, Field.Store.YES, Field.Index.TOKENIZED));
            return base.VisitInterfaceDeclaration(interfaceDeclaration, data);
        }

        public override object VisitEnumDeclaration(EnumNode enumDeclaration, object data)
        {
            //            document.Add(new Field("enum", enumDeclaration.Name.Identifier, Field.Store.YES, Field.Index.TOKENIZED));
            return base.VisitEnumDeclaration(enumDeclaration, data);
        }
    }
}