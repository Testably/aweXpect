using System.Collections.Immutable;
using System.Text;
using aweXpect.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace aweXpect.SourceGenerators;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> for the overloads of collection expectations.
/// </summary>
/// <remarks>
///     The overloads are generated from the signature of the annotated helper that holds their body, so a new subject
///     kind costs one helper and a new element type of a tolerance family costs one factory method.
/// </remarks>
[Generator]
public class CollectionExpectationGenerator : IIncrementalGenerator
{
	private const string AttributeName = "aweXpect.SourceGenerators.CreateCollectionExpectationAttribute";

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"CreateCollectionExpectationAttribute.g.cs",
			SourceText.From(CollectionExpectationSources.Attribute, Encoding.UTF8)));

		IncrementalValuesProvider<CollectionExpectationFamily> families = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				AttributeName,
				static (node, _) => node is MethodDeclarationSyntax,
				static (ctx, _) => GetFamilies(ctx))
			.SelectMany(static (x, _) => x);

		context.RegisterSourceOutput(families.Collect(), static (spc, source) => Execute(source, spc));
	}

	private static ImmutableArray<CollectionExpectationFamily> GetFamilies(GeneratorAttributeSyntaxContext context)
	{
		if (context.TargetSymbol is not IMethodSymbol helper)
		{
			return ImmutableArray<CollectionExpectationFamily>.Empty;
		}

		ImmutableArray<CollectionExpectationFamily>.Builder builder =
			ImmutableArray.CreateBuilder<CollectionExpectationFamily>();
		foreach (AttributeData attributeData in context.Attributes)
		{
			CollectionExpectationFamily? family =
				CollectionExpectationFamily.Create(helper, attributeData, context.SemanticModel.Compilation);
			if (family != null)
			{
				builder.Add(family);
			}
		}

		return builder.ToImmutable();
	}

	private static void Execute(ImmutableArray<CollectionExpectationFamily> families, SourceProductionContext context)
	{
		foreach (IGrouping<string, CollectionExpectationFamily> group in families
			         .GroupBy(x => $"{x.ClassName}.{x.Name}"))
		{
			string result = CollectionExpectationSources.GenerateExtensionClass(group.ToList());
			context.AddSource($"{group.Key}.g.cs", SourceText.From(result, Encoding.UTF8));
		}
	}
}
