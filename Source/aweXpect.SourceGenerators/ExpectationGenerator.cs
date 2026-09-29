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
	/// <summary>
	///     Reported for an annotated class that cannot declare extension methods.
	/// </summary>
	private static readonly DiagnosticDescriptor NotTopLevel = new(
		"aweXpect3005",
		"The expectations need a top-level class",
		"'{0}' cannot hold the generated expectations, because extension methods need a top-level static class",
		"aweXpect.SourceGenerators",
		DiagnosticSeverity.Error,
		true);

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		// Add the marker attributes to the compilation
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"CreateExpectationOnAttribute.g.cs",
			SourceText.From(SourceGenerationHelper.CreateExpectationOnAttribute, Encoding.UTF8)));
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"CreateExpectationOnNullableAttribute.g.cs",
			SourceText.From(SourceGenerationHelper.CreateExpectationOnNullableAttribute, Encoding.UTF8)));

		RegisterExpectations(context, "aweXpect.SourceGenerators.CreateExpectationOnAttribute`1");
		RegisterExpectations(context, "aweXpect.SourceGenerators.CreateExpectationOnNullableAttribute`1");
	}

	private static void RegisterExpectations(IncrementalGeneratorInitializationContext context,
		string attributeMetadataName)
	{
		IncrementalValuesProvider<(EquatableArray<ExpectationToGenerate> Expectations, Problem? Problem)>
			expectationsToGenerate = context.SyntaxProvider
				.ForAttributeWithMetadataName(
					attributeMetadataName,
					static (node, _) => node is ClassDeclarationSyntax,
					static (ctx, _) => GetExpectationsToGenerate(ctx))
				.WithTrackingName("Expectations");

		context.RegisterSourceOutput(expectationsToGenerate,
			static (spc, source) => Execute(source.Expectations, source.Problem, spc));
	}

	/// <remarks>
	///     Only the attributes on the matched declaration are read, so that a class declared in several parts
	///     yields every expectation once.
	/// </remarks>
	private static (EquatableArray<ExpectationToGenerate>, Problem?) GetExpectationsToGenerate(
		GeneratorAttributeSyntaxContext context)
	{
		if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
		{
			return (new EquatableArray<ExpectationToGenerate>([]), null);
		}

		if (classSymbol.ContainingType is not null)
		{
			return (new EquatableArray<ExpectationToGenerate>([]),
				new Problem(NotTopLevel, ((ClassDeclarationSyntax)context.TargetNode).Identifier.GetLocation(),
					classSymbol.ToDisplayString()));
		}

		List<ExpectationToGenerate> expectations = [];
		foreach (AttributeData attributeData in context.Attributes)
		{
			ExpectationToGenerate? expectationToGenerate = GetExpectationToGenerate(classSymbol, attributeData);
			if (expectationToGenerate != null)
			{
				expectations.Add(expectationToGenerate.Value);
			}
		}

		return (new EquatableArray<ExpectationToGenerate>(expectations.ToArray()), null);
	}

	private static void Execute(EquatableArray<ExpectationToGenerate> expectationsToGenerate, Problem? problem,
		SourceProductionContext context)
	{
		if (problem is not null)
		{
			context.ReportDiagnostic(problem.ToDiagnostic());
		}

		foreach (ExpectationToGenerate expectationToGenerate in expectationsToGenerate)
		{
			string result = SourceGenerationHelper.GenerateExtensionClass(expectationToGenerate);
			context.AddSource(expectationToGenerate.FileName, SourceText.From(result, Encoding.UTF8));
		}
	}

	private static ExpectationToGenerate? GetExpectationToGenerate(INamedTypeSymbol classSymbol,
		AttributeData attributeData)
	{
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
			classSymbol,
			targetType,
			positiveName,
			negativeName,
			outcomeMethod,
			attributeData);
	}
}
