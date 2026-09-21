using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace aweXpect.SourceGenerators.Helpers;

/// <summary>
///     One <c>[CreateCollectionExpectation]</c> declaration, resolved into the rendered overloads.
/// </summary>
/// <remarks>
///     Everything is reduced to strings here so that no symbol is held across the generator pipeline.
/// </remarks>
internal sealed class CollectionExpectationFamily
{
	private const string ItemPlaceholder = "{item}";
	private const string DefaultExpectedType = "System.Collections.Generic.IEnumerable<{item}>";

	private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
		.WithMiscellaneousOptions(
			SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
			SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

	private CollectionExpectationFamily(string @namespace, string className, string name, List<string> methods)
	{
		Namespace = @namespace;
		ClassName = className;
		Name = name;
		Methods = methods;
	}

	public string Namespace { get; }
	public string ClassName { get; }
	public string Name { get; }
	public List<string> Methods { get; }

	public static CollectionExpectationFamily? Create(INamedTypeSymbol classSymbol, AttributeData attributeData,
		Compilation compilation)
	{
		if (attributeData.ConstructorArguments.Length != 3)
		{
			return null;
		}

		string? name = attributeData.ConstructorArguments[0].Value?.ToString();
		string? helperName = attributeData.ConstructorArguments[1].Value?.ToString();
		string? collectionType = attributeData.ConstructorArguments[2].Value?.ToString();
		if (name == null || helperName == null || collectionType == null)
		{
			return null;
		}

		Declaration declaration = new(collectionType);
		foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
		{
			declaration.Apply(namedArgument.Key, namedArgument.Value);
		}

		IMethodSymbol? helper = classSymbol.GetMembers(helperName).OfType<IMethodSymbol>().FirstOrDefault();
		if (helper == null)
		{
			return null;
		}

		string positiveName = name.Replace("{Not}", "");
		string negatedName = name.Replace("{Not}", "Not");
		List<string> methods = [];
		foreach (Instantiation instantiation in Instantiate(declaration, compilation))
		{
			methods.Add(Render(helper, declaration, instantiation, positiveName, "expected", declaration.Summary,
				declaration.Remarks, false));
			methods.Add(Render(helper, declaration, instantiation, negatedName, "unexpected",
				declaration.NegatedSummary, declaration.NegatedRemarks ?? declaration.Remarks, true));
		}

		if (methods.Count == 0)
		{
			return null;
		}

		if (declaration.ConditionalOn != null)
		{
			methods.Insert(0, $"#if {declaration.ConditionalOn}");
			methods.Add("#endif");
		}

		return new CollectionExpectationFamily(classSymbol.ContainingNamespace.ToString(), classSymbol.Name,
			positiveName, methods);
	}

	/// <remarks>
	///     A factory expands into one instantiation per <c>Create*</c> method, and a nullable element additionally
	///     accepts a non-nullable expected collection, which is cast up. Without a factory the element is either a
	///     fixed type or the generated method's own type parameter, which yields a single instantiation.
	/// </remarks>
	private static IEnumerable<Instantiation> Instantiate(Declaration declaration, Compilation compilation)
	{
		if (declaration.Factory == null)
		{
			string item = declaration.ElementType ?? declaration.TypeParameters.FirstOrDefault() ?? "object?";
			yield return new Instantiation(item, item, null);
			yield break;
		}

		INamedTypeSymbol? factory = compilation.GetTypeByMetadataName(declaration.Factory);
		if (factory == null)
		{
			yield break;
		}

		foreach (IMethodSymbol factoryMethod in factory.GetMembers().OfType<IMethodSymbol>()
			         .Where(m => m is { IsStatic: true, Parameters.Length: 0, }))
		{
			if (factoryMethod.ReturnType is not INamedTypeSymbol
			    {
				    Name: "ObjectEqualityWithToleranceOptions", TypeArguments.Length: 2,
			    } options)
			{
				continue;
			}

			string item = options.TypeArguments[0].ToDisplayString(TypeFormat);
			string tolerance = options.TypeArguments[1].ToDisplayString(TypeFormat);
			string call = $"{factory.ToDisplayString(TypeFormat)}.{factoryMethod.Name}()";
			yield return new Instantiation(item, item, tolerance) { FactoryCall = call, };
			if (options.TypeArguments[0] is INamedTypeSymbol
			    {
				    ConstructedFrom.SpecialType: SpecialType.System_Nullable_T,
			    } nullable)
			{
				yield return new Instantiation(item, nullable.TypeArguments[0].ToDisplayString(TypeFormat), tolerance)
				{
					FactoryCall = call,
				};
			}
		}
	}

	private static string Render(IMethodSymbol helper, Declaration declaration, Instantiation instantiation,
		string methodName, string parameterName, string summary, string? remarks, bool negated)
	{
		string collection = declaration.CollectionType.Replace(ItemPlaceholder, instantiation.Item);
		Dictionary<string, string> substitutions = new()
		{
			["TCollection"] = collection,
			["TItem"] = instantiation.Item,
			["TTolerance"] = instantiation.Tolerance ?? "",
		};

		string expectedType = (declaration.ExpectedType ?? DefaultExpectedType)
			.Replace(ItemPlaceholder, instantiation.ExpectedItem);
		string argument = instantiation.ExpectedItem == instantiation.Item
			? parameterName
			: $"global::System.Linq.Enumerable.Cast<{instantiation.Item}>({parameterName})";

		List<string> arguments = ["subject", argument,];
		foreach (IParameterSymbol parameter in helper.Parameters.Skip(2))
		{
			arguments.Add(ArgumentFor(parameter, instantiation, negated));
		}

		string typeParameters = declaration.TypeParameters.Length == 0
			? ""
			: $"<{string.Join(", ", declaration.TypeParameters)}>";
		string typeArguments = helper.TypeParameters.Length == 0
			? ""
			: $"<{string.Join(", ", helper.TypeParameters.Select(x => substitutions[x.Name]))}>";

		string header = $"""
		                 	/// <summary>
		                 	///     {summary}
		                 	/// </summary>
		                 """;
		if (!string.IsNullOrEmpty(remarks))
		{
			header += $"\n\t/// <remarks>\n\t///     {remarks!.Replace("\n", "\n\t///     ")}\n\t/// </remarks>";
		}

		if (declaration.Priority != 0)
		{
			header +=
				$"\n\t[global::System.Runtime.CompilerServices.OverloadResolutionPriority({declaration.Priority})]";
		}

		return $$"""
		         {{header}}
		         	public static {{Substitute(helper.ReturnType, substitutions)}}
		         		{{methodName}}{{typeParameters}}(
		         			this {{Substitute(helper.Parameters[0].Type, substitutions)}} subject,
		         			{{Qualify(expectedType)}} {{parameterName}},
		         			[global::System.Runtime.CompilerServices.CallerArgumentExpression("{{parameterName}}")]
		         			string doNotPopulateThisValue = "")
		         		=> {{helper.Name}}{{typeArguments}}(
		         {{string.Join(",\n", arguments.Select(x => "\t\t\t" + x))}});
		         """;
	}

	private static string ArgumentFor(IParameterSymbol parameter, Instantiation instantiation, bool negated)
	{
		if (parameter.Type.Name == "ObjectEqualityWithToleranceOptions")
		{
			return instantiation.FactoryCall ?? "default!";
		}

		return parameter.Type.SpecialType switch
		{
			SpecialType.System_Boolean => negated ? "true" : "false",
			_ => "doNotPopulateThisValue",
		};
	}

	/// <remarks>
	///     The helper is rendered unconstructed and its type parameters are replaced by name, because an instantiation
	///     may substitute the generated method's own type parameter, which has no symbol to construct with.
	/// </remarks>
	private static string Substitute(ITypeSymbol type, Dictionary<string, string> substitutions)
	{
		string result = type.ToDisplayString(TypeFormat);
		foreach (KeyValuePair<string, string> substitution in substitutions.OrderByDescending(x => x.Key.Length))
		{
			result = Regex.Replace(result, $@"\b{substitution.Key}\b", substitution.Value.Replace("$", "$$"));
		}

		return result;
	}

	private static string Qualify(string type)
		=> Regex.Replace(type, @"(?<!global::)\bSystem\.", "global::System.");

	private sealed class Declaration(string collectionType)
	{
		public string CollectionType { get; } = collectionType;
		public string? Factory { get; private set; }
		public string? ElementType { get; private set; }
		public string[] TypeParameters { get; private set; } = [];
		public string? ExpectedType { get; private set; }
		public int Priority { get; private set; }
		public string? ConditionalOn { get; private set; }
		public string Summary { get; private set; } = "";
		public string NegatedSummary { get; private set; } = "";
		public string? Remarks { get; private set; }
		public string? NegatedRemarks { get; private set; }

		public void Apply(string key, TypedConstant value)
		{
			switch (key)
			{
				case "Factory":
					Factory = (value.Value as INamedTypeSymbol)?.ToDisplayString();
					break;
				case "ElementType":
					ElementType = value.Value?.ToString();
					break;
				case "TypeParameters":
					TypeParameters = value.Values.Select(x => x.Value?.ToString() ?? "").ToArray();
					break;
				case "ExpectedType":
					ExpectedType = value.Value?.ToString();
					break;
				case "Priority":
					Priority = value.Value as int? ?? 0;
					break;
				case "ConditionalOn":
					ConditionalOn = value.Value?.ToString();
					break;
				case "Summary":
					Summary = value.Value?.ToString() ?? "";
					break;
				case "NegatedSummary":
					NegatedSummary = value.Value?.ToString() ?? "";
					break;
				case "Remarks":
					Remarks = value.Value?.ToString();
					break;
				case "NegatedRemarks":
					NegatedRemarks = value.Value?.ToString();
					break;
			}
		}
	}

	private sealed class Instantiation(string item, string expectedItem, string? tolerance)
	{
		public string Item { get; } = item;
		public string ExpectedItem { get; } = expectedItem;
		public string? Tolerance { get; } = tolerance;
		public string? FactoryCall { get; init; }
	}
}
