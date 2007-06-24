using CS2.Services.Parsing;
using DDW;
using Lucene.Net.Documents;

namespace CS2.Services.Parsing
{
    public class CSharpParsingVisitor : AbstractVisitor, IParsingVisitor
    {
        private static Document GetDocument(object data)
        {
            return data as Document;
        }

        //public override object VisitCommentStatement(CommentStatement commentStatement, object data)
        //{
        //    GetDocument(data).Add(FieldFactory.CreateCommentField(commentStatement.Comment));
        //    return base.VisitCommentStatement(commentStatement, data);
        //}

        public override object VisitMethodDeclaration(MethodNode methodDeclaration, object data)
        {
            GetDocument(data).Add(FieldFactory.CreateMethodField(methodDeclaration.Names[0].GenericIdentifier));
            return base.VisitMethodDeclaration(methodDeclaration, data);
        }

        public override object VisitNamespaceDeclaration(NamespaceNode namespaceDeclaration, object data)
        {
            string name = namespaceDeclaration.Name != null ? namespaceDeclaration.Name.GenericIdentifier : "Global";

            GetDocument(data).Add(FieldFactory.CreateNamespaceField(name));
            return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
        }

        public override object VisitPropertyDeclaration(PropertyNode propertyDeclaration, object data)
        {
            GetDocument(data).Add(FieldFactory.CreatePropertyField(propertyDeclaration.Names[0].QualifiedIdentifier));
            return base.VisitPropertyDeclaration(propertyDeclaration, data);
        }

        public override object VisitClassDeclaration(ClassNode classDeclaration, object data)
        {
            GetDocument(data).Add(FieldFactory.CreateClassField(classDeclaration.Name.Identifier));
            return base.VisitClassDeclaration(classDeclaration, data);
        }

        public override object VisitInterfaceDeclaration(InterfaceNode interfaceDeclaration, object data)
        {
            GetDocument(data).Add(FieldFactory.CreateInterfaceField(interfaceDeclaration.Name.Identifier));
            return base.VisitInterfaceDeclaration(interfaceDeclaration, data);
        }
    }
}