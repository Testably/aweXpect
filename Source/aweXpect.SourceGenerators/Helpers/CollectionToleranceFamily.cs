using Microsoft.CodeAnalysis;

namespace aweXpect.SourceGenerators.Helpers;

/// <summary>
///     One <c>[CreateCollectionToleranceExpectations]</c> declaration, resolved into the rendered overloads.
/// </summary>
/// <remarks>
///     Everything is reduced to strings here so that no symbol is held across the generator pipeline.
/// </remarks>
internal sealed class CollectionToleranceFamily
{
	private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
		.WithMiscellaneousOptions(
			SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
			SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

	private CollectionToleranceFamily(string @namespace, string className, string name, string? conditionalOn,
		List<string> methods)
	{
		Namespace = @namespace;
		ClassName = className;
		Name = name;
		ConditionalOn = conditionalOn;
		Methods = methods;
	}

	public string Namespace { get; }
	public string ClassName { get; }
	public string Name { get; }
	public string? ConditionalOn { get; }
	public List<string> Methods { get; }

	public static CollectionToleranceFamily? Create(INamedTypeSymbol classSymbol, AttributeData attributeData,
		Compilation compilation)
	{
		if (attributeData.ConstructorArguments.Length != 3)
		{
			return null;
		}

		string? name = attributeData.ConstructorArguments[0].Value?.ToString();
		string? helperName = attributeData.ConstructorArguments[1].Value?.ToString();
		string? collectionTypeName = attributeData.ConstructorArguments[2].Value?.ToString();
		if (name == null || helperName == null || collectionTypeName == null)
		{
			return null;
		}

		INamedTypeSymbol? factory = null;
		string? conditionalOn = null;
		string summary = "";
		string negatedSummary = "";
		foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
		{
			switch (namedArgument.Key)
			{
				case "Factory":
					factory = namedArgument.Value.Value as INamedTypeSymbol;
					break;
				case "ConditionalOn":
					conditionalOn = namedArgument.Value.Value?.ToString();
					break;
				case "Summary":
					summary = namedArgument.Value.Value?.ToString() ?? "";
					break;
				case "NegatedSummary":
					negatedSummary = namedArgument.Value.Value?.ToString() ?? "";
					break;
			}
		}

		IMethodSymbol? helper = classSymbol.GetMembers(helperName)
			.OfType<IMethodSymbol>()
			.FirstOrDefault(m => m.TypeParameters.Length == 3);
		INamedTypeSymbol? collectionType = compilation.GetTypeByMetadataName(collectionTypeName + "`1");
		if (factory == null || helper == null || collectionType == null)
		{
			return null;
		}

		string positiveName = name.Replace("{Not}", "");
		string negatedName = name.Replace("{Not}", "Not");
		List<string> methods = [];
		foreach (IMethodSymbol factoryMethod in factory.GetMembers()
			         .OfType<IMethodSymbol>()
			         .Where(m => m is { IsStatic: true, Parameters.Length: 0, }))
		{
			if (factoryMethod.ReturnType is not INamedTypeSymbol
			    {
				    Name: "ObjectEqualityWithToleranceOptions", TypeArguments.Length: 2,
			    } options)
			{
				continue;
			}

			ITypeSymbol itemType = options.TypeArguments[0];
			ITypeSymbol toleranceType = options.TypeArguments[1];
			IMethodSymbol constructed =
				helper.Construct(collectionType.Construct(itemType), itemType, toleranceType);
			string factoryCall =
				$"{factory.ToDisplayString(TypeFormat)}.{factoryMethod.Name}()";

			// A nullable element additionally accepts a non-nullable expected collection, which is cast up.
			ITypeSymbol? underlyingType = itemType is INamedTypeSymbol
			{
				ConstructedFrom.SpecialType: SpecialType.System_Nullable_T,
			} nullable
				? nullable.TypeArguments[0]
				: null;

			methods.Add(Render(constructed, itemType, itemType, factoryCall, positiveName, "expected", summary,
				false));
			methods.Add(Render(constructed, itemType, itemType, factoryCall, negatedName, "unexpected",
				negatedSummary, true));
			if (underlyingType != null)
			{
				methods.Add(Render(constructed, itemType, underlyingType, factoryCall, positiveName, "expected",
					summary, false));
				methods.Add(Render(constructed, itemType, underlyingType, factoryCall, negatedName, "unexpected",
					negatedSummary, true));
			}
		}

		return new CollectionToleranceFamily(classSymbol.ContainingNamespace.ToString(), classSymbol.Name,
			positiveName, conditionalOn, methods);
	}

	private static string Render(IMethodSymbol helper, ITypeSymbol itemType, ITypeSymbol expectedItemType,
		string factoryCall, string methodName, string parameterName, string summary, bool negated)
	{
		string item = itemType.ToDisplayString(TypeFormat);
		string expectedItem = expectedItemType.ToDisplayString(TypeFormat);
		string argument = SymbolEqualityComparer.Default.Equals(itemType, expectedItemType)
			? parameterName
			: $"global::System.Linq.Enumerable.Cast<{item}>({parameterName})";
		string typeArguments = string.Join(", ",
			helper.TypeArguments.Select(x => x.ToDisplayString(TypeFormat)));

		return $$"""
		         	/// <summary>
		         	///     {{summary}}
		         	/// </summary>
		         	public static {{helper.ReturnType.ToDisplayString(TypeFormat)}}
		         		{{methodName}}(
		         			this {{helper.Parameters[0].Type.ToDisplayString(TypeFormat)}} subject,
		         			global::System.Collections.Generic.IEnumerable<{{expectedItem}}> {{parameterName}},
		         			[global::System.Runtime.CompilerServices.CallerArgumentExpression("{{parameterName}}")]
		         			string doNotPopulateThisValue = "")
		         		=> {{helper.Name}}<{{typeArguments}}>(
		         			subject,
		         			{{argument}},
		         			{{factoryCall}},
		         			doNotPopulateThisValue,
		         			{{(negated ? "true" : "false")}});
		         """;
	}
}
