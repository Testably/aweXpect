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

		return context.Attributes
			.Select(attributeData =>
				CollectionExpectationFamily.Create(helper, attributeData, context.SemanticModel.Compilation))
			.ToImmutableArray();
	}

	private static void Execute(ImmutableArray<CollectionExpectationFamily> families, SourceProductionContext context)
	{
		foreach (Problem problem in families.SelectMany(x => x.Problems))
		{
			context.ReportDiagnostic(problem.ToDiagnostic());
		}

		// A family without methods is a subject kind that this target framework does not have.
		List<IGrouping<(string Namespace, string ClassName, string FileName), CollectionExpectationFamily>> groups =
			families
				.Where(x => x.Methods.Count > 0)
				.GroupBy(x => (x.Namespace, x.ClassName, x.FileName))
				.ToList();
		foreach (IGrouping<(string Namespace, string ClassName, string FileName), CollectionExpectationFamily> group
		         in groups)
		{
			// Two classes with a same-named helper file must not share an output file.
			bool sharedFileName = groups.Count(x => x.Key.FileName == group.Key.FileName) > 1;
			string hintName = sharedFileName
				? $"{group.Key.Namespace}.{group.Key.ClassName}.{group.Key.FileName}"
				: group.Key.FileName;
			string result = CollectionExpectationSources.GenerateExtensionClass(group.ToList());
			context.AddSource($"{hintName}.g.cs", SourceText.From(result, Encoding.UTF8));
		}
	}
}
