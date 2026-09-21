using System.Collections.Immutable;
using System.Text;
using aweXpect.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace aweXpect.SourceGenerators;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> for the tolerance overloads of collection expectations.
/// </summary>
/// <remarks>
///     The declaration only names the family, the helper that holds the body, the factory that supplies the
///     tolerance options and the collection type. Everything else - element types, nullability, the result type
///     and the parameter types - is read from those symbols, so a new element type is added by adding a factory
///     method and a new subject kind by adding one attribute.
/// </remarks>
[Generator]
public class CollectionToleranceGenerator : IIncrementalGenerator
{
	private const string AttributeName = "aweXpect.SourceGenerators.CreateCollectionToleranceExpectationsAttribute";

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"CreateCollectionToleranceExpectationsAttribute.g.cs",
			SourceText.From(CollectionToleranceSources.Attribute, Encoding.UTF8)));

		IncrementalValuesProvider<CollectionToleranceFamily?> families = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				AttributeName,
				static (node, _) => node is Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax,
				static (ctx, _) => GetFamilies(ctx))
			.SelectMany(static (x, _) => x);

		context.RegisterSourceOutput(families.Collect(), static (spc, source) => Execute(source, spc));
	}

	private static ImmutableArray<CollectionToleranceFamily?> GetFamilies(GeneratorAttributeSyntaxContext context)
	{
		if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
		{
			return ImmutableArray<CollectionToleranceFamily?>.Empty;
		}

		ImmutableArray<CollectionToleranceFamily?>.Builder builder =
			ImmutableArray.CreateBuilder<CollectionToleranceFamily?>();
		foreach (AttributeData attributeData in context.Attributes)
		{
			CollectionToleranceFamily? family =
				CollectionToleranceFamily.Create(classSymbol, attributeData, context.SemanticModel.Compilation);
			if (family != null)
			{
				builder.Add(family);
			}
		}

		return builder.ToImmutable();
	}

	private static void Execute(ImmutableArray<CollectionToleranceFamily?> families, SourceProductionContext context)
	{
		foreach (IGrouping<string, CollectionToleranceFamily> group in families
			         .Where(x => x != null)
			         .Select(x => x!)
			         .GroupBy(x => $"{x.ClassName}.{x.Name}"))
		{
			string result = CollectionToleranceSources.GenerateExtensionClass(group.ToList());
			context.AddSource($"{group.Key}.Tolerance.g.cs", SourceText.From(result, Encoding.UTF8));
		}
	}
}
