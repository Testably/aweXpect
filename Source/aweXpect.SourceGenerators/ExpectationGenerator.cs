using System.Collections.Immutable;
using System.Text;
using aweXpect.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace aweXpect.SourceGenerators;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> for simple expectations.
/// </summary>
[Generator]
public class ExpectationGenerator : IIncrementalGenerator
{
	private static readonly string[] _supportedAttributes =
	[
		nameof(SourceGenerationHelper.CreateExpectationOnAttribute),
		nameof(SourceGenerationHelper.CreateExpectationOnNullableAttribute),
	];

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		// Add the marker attributes to the compilation
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"CreateExpectationOnAttribute.g.cs",
			SourceText.From(SourceGenerationHelper.CreateExpectationOnAttribute, Encoding.UTF8)));
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"CreateExpectationOnNullableAttribute.g.cs",
			SourceText.From(SourceGenerationHelper.CreateExpectationOnNullableAttribute, Encoding.UTF8)));

		HashSet<string> files = new();
		IncrementalValuesProvider<ExpectationToGenerate> expectationsToGenerate = context.SyntaxProvider
			.CreateSyntaxProvider(
				static (s, _) => IsSyntaxTargetForGeneration(s),
				(ctx, _) => GetSemanticTargetForGeneration(ctx, files))
			.Where(static m => m is not null)
			.SelectMany((x, _) => x!.ToImmutableArray());

		context.RegisterSourceOutput(expectationsToGenerate,
			static (spc, source) => Execute(source, spc));
	}

	private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
		=> node is ClassDeclarationSyntax { AttributeLists.Count: > 0, };

	private static IEnumerable<ExpectationToGenerate> GetSemanticTargetForGeneration(GeneratorSyntaxContext context,
		HashSet<string> files)
	{
		// we know the node is a ClassDeclarationSyntax thanks to IsSyntaxTargetForGeneration
		ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

		SemanticModel semanticModel = context.SemanticModel;
		if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
		{
			yield break;
		}

		foreach (AttributeData? attributeData in classSymbol.GetAttributes())
		{
			INamedTypeSymbol? attributeClass = attributeData.AttributeClass;
			if (attributeClass == null || !attributeClass.IsGenericType ||
			    !_supportedAttributes.Contains(attributeClass.Name))
			{
				continue;
			}

			// Extract the target type from the generic type argument
			INamedTypeSymbol? targetType = attributeClass.TypeArguments[0] as INamedTypeSymbol;
			if (targetType == null)
			{
				continue;
			}

			ExpectationToGenerate? expectationToGenerate = GetExpectationToGenerate(classSymbol, attributeData);
			if (expectationToGenerate != null &&
			    files.Add(expectationToGenerate.Value.FileName))
			{
				yield return expectationToGenerate.Value;
			}
		}
	}

	private static void Execute(ExpectationToGenerate expectationToGenerate, SourceProductionContext context)
	{
		string result = SourceGenerationHelper.GenerateExtensionClass(expectationToGenerate);
		// Create a separate partial class file for each enum
		context.AddSource(expectationToGenerate.FileName, SourceText.From(result, Encoding.UTF8));
	}

	private static ExpectationToGenerate? GetExpectationToGenerate(INamedTypeSymbol classSymbol,
		AttributeData attributeData)
	{
		string containingNamespace = classSymbol.ContainingNamespace.ToString();
		if (containingNamespace is null)
		{
			return null;
		}

		INamedTypeSymbol? targetType = attributeData.AttributeClass?.TypeArguments[0] as INamedTypeSymbol;
		if (targetType == null)
		{
			return null;
		}

		if (attributeData.ConstructorArguments.Length != 2)
		{
			return null;
		}

		string? name = attributeData.ConstructorArguments[0].Value?.ToString();
		string? positiveName = name?.Replace("{Not}", "");
		string? negativeName = name?.Contains("{Not}") == true ? name.Replace("{Not}", "Not") : null;
		string? outcomeMethod = attributeData.ConstructorArguments[1].Value?.ToString();
		if (outcomeMethod == null || positiveName == null)
		{
			return null;
		}

		if (targetType.TypeKind == TypeKind.Error)
		{
			return null;
		}

		return new ExpectationToGenerate(
			containingNamespace,
			classSymbol.Name,
			targetType,
			positiveName,
			negativeName,
			outcomeMethod,
			attributeData);
	}
}
