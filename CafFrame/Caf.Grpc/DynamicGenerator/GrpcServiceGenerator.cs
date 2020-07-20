using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.Emit;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MagicOnion;
using System.Threading.Tasks;
using MagicOnion.Server;
using System.Runtime.Loader;
using Caf.Grpc.Client.Utility;

namespace Cafgemini.Frame.Grpc.Server.DynamicGenerator
{
    public class GrpcServiceGenerator : IGrpcServiceGenerator
    {
        MyAssemblyLoadContext assemblyLoadContext= new  MyAssemblyLoadContext();
        private MemoryStream stream;
        private Assembly _dynamicClientAssembly;
        private Assembly _dynamicAssembly;
        private Assembly _dynamicInterfaceAssembly;
        private Assembly _dynamicClientInterfaceAssembly;
        public Assembly DynamicAssembly => _dynamicAssembly;
        public Assembly DynamicInterfaceAssembly => _dynamicInterfaceAssembly;
        public Assembly DynamicClientInterfaceAssembly => _dynamicClientInterfaceAssembly;
        private readonly IGrpcServiceProvider _grpcServiceProvider;
        private string dynamicProxysPath;
        public GrpcServiceGenerator(IGrpcServiceProvider grpcServiceProvider)
        {
            _grpcServiceProvider=grpcServiceProvider;

            dynamicProxysPath = Path.Combine(System.AppContext.BaseDirectory,"DynamicProxys");
            if (!Directory.Exists(dynamicProxysPath))
            {
                Directory.CreateDirectory(dynamicProxysPath);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Tuple<Type, Type>> GeneraterClientProxyInterface()
        {
            var assemblys = AppDomain.CurrentDomain.GetAssemblies().Where(o => o.IsDynamic == false);
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            var types = _grpcServiceProvider.NeedProxyGrpcClientType;
            foreach (var item in types)
            {
                syntaxTrees.AddIfNotContains(GetInterfaceSyntaxTree(item,true));
            }

            var references = new List<PortableExecutableReference>();
            foreach (var item in assemblys)
            {
                if (item.Location != "")
                {
                    var a = MetadataReference.CreateFromFile(item.Location);
                    references.Add(a);
                }
            }
            references.Add(MetadataReference.CreateFromFile(typeof(IService<>).GetTypeInfo().Assembly.Location));
            CSharpCompilation compilation = CSharpCompilation.Create("ClientInterfaceProxys", syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var ms = new MemoryStream())
            {
                stream = new MemoryStream();
                EmitResult result = compilation.Emit(Path.Combine(dynamicProxysPath, "ClientInterfaceProxys.dll"));
                if (result.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    _dynamicClientInterfaceAssembly = assemblyLoadContext.LoadFromAssemblyPath(Path.Combine(dynamicProxysPath, "ClientInterfaceProxys.dll"));
                    return _dynamicClientInterfaceAssembly.ExportedTypes.ToList().Select(o => new Tuple<Type, Type>(o, types.First(a => o.Name.Contains(a.Name)))).ToList();
                }
            }
            return null;
        }

        public void GeneraterClientProxyService(out List<Type> types, out List<Type> proxyTypes)
        {
            types = new List<Type>();
            proxyTypes = new List<Type>();
            var assemblys = AppDomain.CurrentDomain.GetAssemblies().Where(o => o.IsDynamic == false);
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            var interfaces = GeneraterClientProxyInterface().ToList();
            if(!interfaces.Any())
            {
                return;
            }
            foreach (var interfacetype in interfaces)
            {
                syntaxTrees.AddIfNotContains(GetClientProxyClassSyntaxTree(interfacetype.Item1, interfacetype.Item2));
            }
            types = interfaces.Select(o => o.Item2).ToList();
            var references = new List<PortableExecutableReference>();
            foreach (var item in assemblys)
            {
                if (item.Location != "")
                {
                    var reference = MetadataReference.CreateFromFile(item.Location);
                    references.Add(reference);
                }
            }
            //stream.Seek(0, SeekOrigin.Begin);
            references.Add(MetadataReference.CreateFromFile(typeof(IGrpcConnectionUtility).GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(interfaces.First().Item2.GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(dynamicProxysPath, "ClientInterfaceProxys.dll")));
            CSharpCompilation compilation = CSharpCompilation.Create("Cafgemini.Caf.ClientClassProxys", syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(Path.Combine(dynamicProxysPath, "ClientClassProxys.dll"));
                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    _dynamicClientAssembly = assemblyLoadContext.LoadFromAssemblyPath(Path.Combine(dynamicProxysPath, "ClientClassProxys.dll"));
                    proxyTypes = _dynamicClientAssembly.GetTypes().ToList();//.ExportedTypes.ToArray();
                }
            }
        }

        public Type[] GeneraterProxyService()
        {
            var assemblys = AppDomain.CurrentDomain.GetAssemblies().Where(o => o.IsDynamic==false);
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
           var interfaces = GeneraterProxyInterface().ToList();
           foreach (var interfacetype in interfaces)
           {
               syntaxTrees.AddIfNotContains(GetProxyClassSyntaxTree(interfacetype.Item1,interfacetype.Item2));
           }
            var references = new List<PortableExecutableReference>();
            foreach (var item in assemblys)
            {
                if(item.Location!="")
                {
                    var reference = MetadataReference.CreateFromFile(item.Location);
                    references.Add(reference);
                }
            }
            //stream.Seek(0, SeekOrigin.Begin);
            references.Add(MetadataReference.CreateFromFile(typeof(IService<>).GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(ServiceBase<>).GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(interfaces.First().Item2.GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(dynamicProxysPath, "InterfaceProxys.dll")));
            CSharpCompilation compilation = CSharpCompilation.Create("Cafgemini.Caf.ClassProxys", syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(Path.Combine(dynamicProxysPath, "ClassProxys.dll"));
                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);                    
                    _dynamicAssembly = assemblyLoadContext.LoadFromAssemblyPath(Path.Combine(dynamicProxysPath, "ClassProxys.dll"));
                    return _dynamicAssembly.GetTypes();//.ExportedTypes.ToArray();
                }
            }
            return new Type[]{};
        }


        public List<Tuple<Type,Type>> GeneraterProxyInterface()
        {
            var assemblys = AppDomain.CurrentDomain.GetAssemblies().Where(o => o.IsDynamic==false);
            List<SyntaxTree> syntaxTrees=new List<SyntaxTree>();
            var types = _grpcServiceProvider.NeedProxyGrpcServiceType;
            foreach (var item in types)
            {               
                syntaxTrees.AddIfNotContains(GetInterfaceSyntaxTree(item));
            }
            
            var references = new List<PortableExecutableReference>();
            foreach (var item in assemblys)
            {
                if(item.Location!="")
                {
                    var a = MetadataReference.CreateFromFile(item.Location);
                    references.Add(a);
                }
            }
            references.Add(MetadataReference.CreateFromFile(typeof(IService<>).GetTypeInfo().Assembly.Location));
            CSharpCompilation compilation = CSharpCompilation.Create("InterfaceProxys", syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var ms = new MemoryStream())
            {
                stream=new MemoryStream();
                EmitResult result = compilation.Emit(Path.Combine(dynamicProxysPath, "InterfaceProxys.dll"));
                if (result.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    _dynamicInterfaceAssembly = assemblyLoadContext.LoadFromAssemblyPath(Path.Combine(dynamicProxysPath, "InterfaceProxys.dll"));
                    return _dynamicInterfaceAssembly.ExportedTypes.ToList().Select(o =>new Tuple<Type,Type>(o,types.First(a =>o.Name.Contains(a.Name)))).ToList();
                }
            }
            return null;
        }

        private SyntaxTree GetClientProxyClassSyntaxTree(Type interfaceType, Type interfaceTypeOld)
        {
            var className = interfaceTypeOld.Name.StartsWith("I") ? interfaceTypeOld.Name.Substring(1) : interfaceTypeOld.Name;
            className += "ClientProxy";
            var members = new List<MemberDeclarationSyntax>
            {
                GetClientFieldDeclaration(),
                GetClientConstructorDeclaration(className)

            };
            members.AddRange(interfaceTypeOld.GetMethods().Select(m => GetClientMethodDeclaration(m, interfaceType)));
            var syntaxTree = CompilationUnit()
                .WithUsings(GetUsings(interfaceTypeOld))
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        NamespaceDeclaration(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("Cafgemini"),
                                    IdentifierName("Caf")),
                                IdentifierName("GrpcClientProxys")))
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        ClassDeclaration(className)
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                   .WithBaseList(
                                    BaseList(
                                        SingletonSeparatedList<BaseTypeSyntax>(
                                            SimpleBaseType(
                                            IdentifierName(interfaceTypeOld.Name)))))
                            .WithMembers(List(members))))))
                .NormalizeWhitespace().SyntaxTree;
            return syntaxTree;
        }


        private SyntaxTree GetProxyClassSyntaxTree (Type interfaceType,Type interfaceTypeOld)
        {
            var className = interfaceType.Name.StartsWith("I") ? interfaceType.Name.Substring(1) : interfaceType.Name;
            //className += "GrpcProxy";
            var members = new List<MemberDeclarationSyntax>
            {
                GetFieldDeclaration(interfaceTypeOld),
                GetConstructorDeclaration(className,interfaceTypeOld)
                          
            };
            members.AddRange(interfaceType.GetMethods().Select(m=>GetMethodDeclaration(m, interfaceTypeOld)));
            var syntaxTree = CompilationUnit()
                .WithUsings(GetUsings(interfaceTypeOld))
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        NamespaceDeclaration(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("Cafgemini"),
                                    IdentifierName("Caf")),
                                IdentifierName("GrpcProxys")))
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        ClassDeclaration(className)
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                            .WithBaseList(
                                BaseList(
                                    SeparatedList<BaseTypeSyntax>(
                                        new SyntaxNodeOrToken[]{
                                            SimpleBaseType(
                                                GenericName(
                                                    Identifier("ServiceBase"))
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SingletonSeparatedList<TypeSyntax>(
                                                            IdentifierName(interfaceType.Name))))),
                                            Token(SyntaxKind.CommaToken),
                                            SimpleBaseType(
                                                IdentifierName(interfaceType.Name))})))
                            .WithMembers(List(members))))))
                .NormalizeWhitespace().SyntaxTree;
                return syntaxTree;
        }
        private MemberDeclarationSyntax GetClientFieldDeclaration()
        {
            return FieldDeclaration(
                        VariableDeclaration(
                            IdentifierName("IGrpcConnectionUtility"))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("_grpcConnectionUtility")))))
                    .WithModifiers(
                        TokenList(
                            new[]{
                                Token(SyntaxKind.PrivateKeyword),
                                Token(SyntaxKind.ReadOnlyKeyword)}));
        }
        private MemberDeclarationSyntax GetFieldDeclaration(Type interfaceType)
        {
            return FieldDeclaration(
                        VariableDeclaration(
                            IdentifierName(interfaceType.Name))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("_service")))))
                    .WithModifiers(
                        TokenList(
                            new []{
                                Token(SyntaxKind.PrivateKeyword),
                                Token(SyntaxKind.ReadOnlyKeyword)}));
        }

        private MemberDeclarationSyntax GetClientMethodDeclaration(MethodInfo methodInfo, Type InterfaceType)
        {
            var InterfaceName = InterfaceType.Name;
            bool isTask = methodInfo.ReturnType.BaseType == typeof(Task);

            var returnDeclaration = GetTypeSyntax(methodInfo.ReturnType);
            var parameterList = new List<SyntaxNodeOrToken>();
            var parameterDeclarationList = new List<SyntaxNodeOrToken>();

            foreach (var parameter in methodInfo.GetParameters())
            {
                if (parameter.ParameterType.IsGenericType)
                {
                    parameterDeclarationList.Add(Parameter(
                                     Identifier(parameter.Name))
                                     .WithType(GetTypeSyntax(parameter.ParameterType)));
                }
                else
                {
                    parameterDeclarationList.Add(Parameter(
                                        Identifier(parameter.Name))
                                        .WithType(GetQualifiedNameSyntax(parameter.ParameterType)));
                }
                parameterDeclarationList.Add(Token(SyntaxKind.CommaToken));

                parameterList.Add(Argument(IdentifierName(parameter.Name)));
                parameterList.Add(Token(SyntaxKind.CommaToken));
            }
            if (parameterList.Any())//去掉‘,’
            {
                parameterList.RemoveAt(parameterList.Count - 1);
                parameterDeclarationList.RemoveAt(parameterDeclarationList.Count - 1);
            }
            var declaration = MethodDeclaration(
                returnDeclaration,
                Identifier(methodInfo.Name));
            if (isTask)
            {
                declaration = declaration.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword)))//, Token(SyntaxKind.AsyncKeyword)
                .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(parameterDeclarationList)));
            }
            else
            {
                declaration = declaration.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))//, Token(SyntaxKind.AsyncKeyword)
               .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(parameterDeclarationList)));
            }
            ExpressionSyntax expressionSyntax;
            StatementSyntax statementSyntax;
            LocalDeclarationStatementSyntax localDeclarationStatement = null;
            //var reslut=_service(xxx);
            LocalDeclarationStatementSyntax grpcserverStatement = LocalDeclarationStatement(
                                VariableDeclaration(
                                    IdentifierName("var"))
                                .WithVariables(
                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        VariableDeclarator(
                                            Identifier("grpcserver"))
                                        .WithInitializer(
                                            EqualsValueClause(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("_grpcConnectionUtility"),
                                                        GenericName(
                                                            Identifier("GetRemoteServiceForDirectConnection"))
                                                        .WithTypeArgumentList(
                                                            TypeArgumentList(
                                                                SingletonSeparatedList<TypeSyntax>(
                                                                    IdentifierName(InterfaceName))))))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                LiteralExpression(
                                                                    SyntaxKind.StringLiteralExpression,
                                                                    Literal("TestServiceName")))))))))));
            if (isTask)
            {
                localDeclarationStatement = LocalDeclarationStatement(
                    VariableDeclaration(
                        IdentifierName("var"))
                    .WithVariables(
                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                            VariableDeclarator(
                                Identifier("data"))
                            .WithInitializer(
                                EqualsValueClause(
                                    AwaitExpression(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("grpcserver"),
                                            IdentifierName(methodInfo.Name)))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                parameterList)))))))));
                statementSyntax = ReturnStatement(IdentifierName("data"));
            }
            else
            {
                localDeclarationStatement = LocalDeclarationStatement(
                                    VariableDeclaration(
                                        IdentifierName("var"))
                                    .WithVariables(
                                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                                            VariableDeclarator(
                                                Identifier("reslut"))
                                            .WithInitializer(
                                                EqualsValueClause(
                                                    InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("_service"),
                                                            IdentifierName(methodInfo.Name)))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SeparatedList<ArgumentSyntax>(
                                                                parameterList))))))));

                expressionSyntax = ObjectCreationExpression(
                        returnDeclaration)
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList<ArgumentSyntax>(
                                Argument(
                                    IdentifierName("reslut")))));
                statementSyntax = ReturnStatement(expressionSyntax);
            }

            declaration = declaration.WithBody(
                        Block(
                            grpcserverStatement,localDeclarationStatement, statementSyntax));

            return declaration;
        }
        private MemberDeclarationSyntax GetMethodDeclaration(MethodInfo methodInfo,Type InterfaceTypeOld)
        {
            bool isTask = InterfaceTypeOld.GetMethod(methodInfo.Name).ReturnType.BaseType ==typeof(Task);

            var returnDeclaration = GetTypeSyntax(methodInfo.ReturnType);
            var parameterList = new List<SyntaxNodeOrToken>();
            var parameterDeclarationList = new List<SyntaxNodeOrToken>();

            foreach (var parameter in methodInfo.GetParameters())
            {
                if (parameter.ParameterType.IsGenericType)
                {
                    parameterDeclarationList.Add(Parameter(
                                     Identifier(parameter.Name))
                                     .WithType(GetTypeSyntax(parameter.ParameterType)));
                }
                else
                {
                    parameterDeclarationList.Add(Parameter(
                                        Identifier(parameter.Name))
                                        .WithType(GetQualifiedNameSyntax(parameter.ParameterType)));
                }
                parameterDeclarationList.Add(Token(SyntaxKind.CommaToken));
               
                parameterList.Add(Argument(IdentifierName(parameter.Name)));
                parameterList.Add(Token(SyntaxKind.CommaToken));
            }
            if (parameterList.Any())//去掉‘,’
            {
                parameterList.RemoveAt(parameterList.Count - 1);
                parameterDeclarationList.RemoveAt(parameterDeclarationList.Count - 1);
            }
            var declaration = MethodDeclaration(
                returnDeclaration,
                Identifier(methodInfo.Name));
            if(isTask)
            {
                declaration = declaration.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword)))//, Token(SyntaxKind.AsyncKeyword)
                .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(parameterDeclarationList)));
            }
            else
            {
                declaration = declaration.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))//, Token(SyntaxKind.AsyncKeyword)
               .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(parameterDeclarationList)));
            }
            ExpressionSyntax expressionSyntax;
            StatementSyntax statementSyntax;

            //var reslut=_service(xxx);
            LocalDeclarationStatementSyntax localDeclarationStatement = null;
            if (isTask)
            {
                localDeclarationStatement = LocalDeclarationStatement(
                    VariableDeclaration(
                        IdentifierName("var"))
                    .WithVariables(
                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                            VariableDeclarator(
                                Identifier("reslut"))
                            .WithInitializer(
                                EqualsValueClause(
                                    AwaitExpression(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("_service"),
                                            IdentifierName(methodInfo.Name)))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                parameterList)))))))));
                statementSyntax = ReturnStatement(IdentifierName("reslut"));
            }
            else
            {
                localDeclarationStatement = LocalDeclarationStatement(
                                    VariableDeclaration(
                                        IdentifierName("var"))
                                    .WithVariables(
                                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                                            VariableDeclarator(
                                                Identifier("reslut"))
                                            .WithInitializer(
                                                EqualsValueClause(
                                                    InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("_service"),
                                                            IdentifierName(methodInfo.Name)))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SeparatedList<ArgumentSyntax>(
                                                                parameterList))))))));

                expressionSyntax = ObjectCreationExpression(
                        returnDeclaration)
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList<ArgumentSyntax>(
                                Argument(
                                    IdentifierName("reslut")))));
                statementSyntax = ReturnStatement(expressionSyntax);
            }

            declaration = declaration.WithBody(
                        Block(
                            localDeclarationStatement,statementSyntax));

            return declaration;
        }
        private MemberDeclarationSyntax GetClientConstructorDeclaration(string className)
        {
            return ConstructorDeclaration(
                        Identifier(className))
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(
                        ParameterList(
                            SingletonSeparatedList<ParameterSyntax>(
                                Parameter(
                                    Identifier("grpcConnectionUtility"))
                                .WithType(
                                    IdentifierName("IGrpcConnectionUtility")))))
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName("_grpcConnectionUtility"),
                                        IdentifierName("grpcConnectionUtility"))))));
        }
        private MemberDeclarationSyntax GetConstructorDeclaration(string className,Type interfaceType)
        {
            return ConstructorDeclaration(
                        Identifier(className))
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(
                        ParameterList(
                            SingletonSeparatedList<ParameterSyntax>(
                                Parameter(
                                    Identifier("service"))
                                .WithType(
                                    IdentifierName(interfaceType.Name)))))
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName("_service"),
                                        IdentifierName("service"))))));
        }
        private SyntaxTree GetInterfaceSyntaxTree (Type interfaceType,bool isClient=false)
        {
            var nameSpace = "GrpcProxys";
            if(isClient)
            {
                nameSpace = "GrpcClientProxys";
            }
            var InterfaceName = interfaceType.Name.StartsWith("I") ? interfaceType.Name : "I"+interfaceType.Name;          
            InterfaceName += "GrpcProxy";
            var syntaxTree= CompilationUnit()
                .WithUsings(GetUsings())
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        NamespaceDeclaration(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("Cafgemini"),
                                    IdentifierName("Caf")),
                                IdentifierName(nameSpace)))
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
//

            InterfaceDeclaration(InterfaceName)
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithBaseList(
                    BaseList(
                        SingletonSeparatedList<BaseTypeSyntax>(
                            SimpleBaseType(
                                GenericName(
                                    Identifier("IService"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            IdentifierName(InterfaceName))))))))
                .WithMembers(
                    List<MemberDeclarationSyntax>(
                        GetDeclarationSyntaxes(interfaceType)))
                .NormalizeWhitespace()
                        //
                        ))))
                .NormalizeWhitespace().SyntaxTree;
                return syntaxTree;


        }

        private MemberDeclarationSyntax[] GetDeclarationSyntaxes(Type interfaceType)
        {
            var syntaxs=new List<MemberDeclarationSyntax>();
            foreach (var method in interfaceType.GetMethods())
            {
                var name = method.Name;
                bool isAsyncMethod = false;
                TypeSyntax returnDeclaration = null;
                if (typeof(Task).IsAssignableFrom(method.ReturnType) && method.ReturnType.GenericTypeArguments.Count()>0)
                {
                    isAsyncMethod = true;
                    returnDeclaration = GetTypeSyntax(method.ReturnType.GenericTypeArguments.First());
                }
                else if(typeof(Task).IsAssignableFrom(method.ReturnType))
                {
                    isAsyncMethod = true;
                    returnDeclaration = null;
                }
                else
                {
                    returnDeclaration = GetTypeSyntax(method.ReturnType);
                }
                
                var parameterDeclarationList = new List<SyntaxNodeOrToken>();               

            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType.IsGenericType)
                {
                    parameterDeclarationList.Add(Parameter(
                                     Identifier(parameter.Name))
                                     .WithType(GetTypeSyntax(parameter.ParameterType)));
                }
                else
                {
                    parameterDeclarationList.Add(Parameter(
                                        Identifier(parameter.Name))
                                        .WithType(GetQualifiedNameSyntax(parameter.ParameterType)));

                }
                parameterDeclarationList.Add(Token(SyntaxKind.CommaToken));
            }
            if (parameterDeclarationList.Any())//去掉‘,’
            {
                parameterDeclarationList.RemoveAt(parameterDeclarationList.Count - 1);
            }
                var a = MethodDeclaration(
                GenericName(
                    Identifier("UnaryResult"))
                .WithTypeArgumentList(
                    TypeArgumentList(
                        SingletonSeparatedList<TypeSyntax>(
                            returnDeclaration))),
                Identifier(name))
                .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(parameterDeclarationList)))
            .WithSemicolonToken(
                Token(SyntaxKind.SemicolonToken));
                syntaxs.Add(a);
            }
            return syntaxs.ToArray();
        }

        private static TypeSyntax GetTypeSyntax(Type type)
        {
            //没有返回值。
            if (type == null)
                return null;

            //非泛型。
            if (!type.GetTypeInfo().IsGenericType)
                return GetQualifiedNameSyntax(type.FullName);

            var list = new List<SyntaxNodeOrToken>();

            foreach (var genericTypeArgument in type.GenericTypeArguments)
            {
                list.Add(genericTypeArgument.GetTypeInfo().IsGenericType
                    ? GetTypeSyntax(genericTypeArgument)
                    : GetQualifiedNameSyntax(genericTypeArgument.FullName));
                list.Add(Token(SyntaxKind.CommaToken));
            }

            var array = list.Take(list.Count - 1).ToArray();
            var typeArgumentListSyntax = TypeArgumentList(SeparatedList<TypeSyntax>(array));
            return GenericName(type.Name.Substring(0, type.Name.IndexOf('`')))
                .WithTypeArgumentList(typeArgumentListSyntax);
        }
        private static NameSyntax GetQualifiedNameSyntax(Type type)
        {
            var fullName = type.Namespace + "." + type.Name;
            return GetQualifiedNameSyntax(fullName);
        }
        private static QualifiedNameSyntax GetQualifiedNameSyntax(IReadOnlyCollection<string> names)
        {
            var ids = names.Select(IdentifierName).ToArray();

            var index = 0;
            QualifiedNameSyntax left = null;
            while (index + 1 < names.Count)
            {
                left = left == null ? QualifiedName(ids[index], ids[index + 1]) : QualifiedName(left, ids[index + 1]);
                index++;
            }
            return left;
        }
        private static  NameSyntax GetQualifiedNameSyntax(string fullName)
        {
            if(fullName.Contains("."))
            {
                return GetQualifiedNameSyntax(fullName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries));
            }
            return IdentifierName(fullName);
        }

        private static SyntaxList<UsingDirectiveSyntax> GetUsings(Type type)
        {
            var directives = new List<UsingDirectiveSyntax>();
            return List(
                new[]
                {
                    UsingDirective(GetQualifiedNameSyntax("Caf.Grpc.Client.Utility")),
                    UsingDirective(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.IdentifierName("System")),
                    UsingDirective(GetQualifiedNameSyntax("System.Threading.Tasks")),
                    UsingDirective(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.IdentifierName("MagicOnion")),
                    UsingDirective(GetQualifiedNameSyntax("MagicOnion.Server")),
                    UsingDirective(GetQualifiedNameSyntax(type.Namespace)),
                    UsingDirective(GetQualifiedNameSyntax("System.Collections.Generic"))
                }.Concat(directives));
        }
        private static SyntaxList<UsingDirectiveSyntax> GetUsings()
        {
            var directives = new List<UsingDirectiveSyntax>();
            return List(
                new[]
                {
                    UsingDirective(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.IdentifierName("System")),
                    UsingDirective(GetQualifiedNameSyntax("System.Threading.Tasks")),
                    UsingDirective(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.IdentifierName("MagicOnion")),
                    UsingDirective(GetQualifiedNameSyntax("MagicOnion.Server")),
                    UsingDirective(GetQualifiedNameSyntax("System.Collections.Generic"))
                }.Concat(directives));
        }
    }
}