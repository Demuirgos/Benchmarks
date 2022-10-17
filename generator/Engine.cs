using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Attributes;

namespace Generator;

[Generator]
public class Engine : ISourceGenerator
{
    private struct ClassData {
        public string ChildName;
        public string BaseName;
        public PropsData[] Properties;
    }
    private struct PropsData
    {
        public PropsData(params string[] args) : this(args[0], args[1], args[2]) { }
        public PropsData(string typename, string propertyName, string defaultValue)
        {
            Typename = typename;
            Default = defaultValue;
            PropertyName = propertyName;
        }
        public string Typename;
        public object? Default;
        public string PropertyName;
    }

    private string EmitProps (IEnumerable<PropsData> props) 
        => props
        .Select(metric => @$"public static {metric.Typename} {metric.PropertyName} => {metric.Default};")
        .Aggregate((a, b) => a + Environment.NewLine + b);
    private (string, string) GeneratedSourceCode(ClassData classMetadata ) => (classMetadata.ChildName, @$"
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin;
public partial class {classMetadata.ChildName} : {classMetadata.BaseName}
{{
    {EmitProps(classMetadata.Properties)}
}}
");

    private void GenerateFiles(string name, string code, GeneratorExecutionContext context)
    {
        // create file and insert code inside
        context.AddSource(name, SourceText.From(code, Encoding.UTF8));
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var attributeToLookFor = typeof(ImplementAttribute);
        var allFunctionsInCompilationUnit = GetMarkedFunctionBy(attributeToLookFor, context.Compilation);
        var allAttributesToBeEmited = GetTargetAttributesFrom(allFunctionsInCompilationUnit, context.Compilation);

        allAttributesToBeEmited.Select(GeneratedSourceCode)
            .ToList()
            .ForEach((result) => {
                GenerateFiles($"{result.Item1}.g.cs", result.Item2, context);
            });
    }

    public void Initialize(GeneratorInitializationContext context)
    {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif 
            Debug.WriteLine("Initalize code generator");
    }

    private bool IsAttribute(Type flag) => flag.Name.EndsWith("Attribute") && flag.BaseType == typeof(System.Attribute);

    private InterfaceDeclarationSyntax[] GetMarkedFunctionBy(Type flagType /* must be an attribute type*/, Compilation context)
    {
        if (!IsAttribute(flagType))
        {
            return Array.Empty<InterfaceDeclarationSyntax>();
        }

        IEnumerable<SyntaxNode> allNodes = context.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());
        return allNodes
            .Where(d => d.IsKind(SyntaxKind.InterfaceDeclaration))
            .OfType<InterfaceDeclarationSyntax>()
            .ToArray();
    }

    private ClassData[] GetTargetAttributesFrom(IEnumerable<InterfaceDeclarationSyntax> allInterface, Compilation context)
    { 
        return allInterface.Where(interDef =>
        {
            return interDef.AttributeLists
                .SelectMany(x => x.Attributes)
                .Where(attr => attr.Name.ToString() == nameof(ImplementAttribute))
                .Any();
        })
        .Select(@interface => {
            var semanticModel = context.GetSemanticModel(@interface.SyntaxTree);
            var interfaceSymbol = semanticModel.GetDeclaredSymbol(@interface);
            var interfaceName = interfaceSymbol.Name;
            var interfaceProperties = interfaceSymbol.GetMembers()
                .Where(member => member.Kind == SymbolKind.Property)
                .OfType<IPropertySymbol>();

            // check if property has attribute DefaultAttribute
            var Defaultsprops = interfaceProperties
                .Where(property => property.GetAttributes()
                    .Where(attribute => attribute.AttributeClass.Name == nameof(DefaultAttribute))
                    .Any())
                .Select(property => {
                    var attribute = property.GetAttributes()
                        .Where(attribute => attribute.AttributeClass.Name == nameof(DefaultAttribute))
                        .First();
                    var attributeArguments = attribute.ConstructorArguments;
                    var attributeArgumentValues = attributeArguments
                        .Select(arg => arg.Value.ToString())
                        .ToArray();
                    return new PropsData(attributeArgumentValues);
                }).ToArray();
            
            return new ClassData { ChildName = interfaceName.Substring(1), BaseName = interfaceName, Properties = Defaultsprops };
        })
        .ToArray();
    }
}
