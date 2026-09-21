using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace aweXpect.SourceGenerators.Helpers;

/// <summary>
///     One <c>[CreateCollectionExpectation]</c> declaration, resolved into the rendered overloads.
/// </summary>
/// <remarks>
///     The helper's own signature is the declaration: its return type, its subject and expected parameters and its
///     type parameters are emitted verbatim. Only what the signature cannot state - the name, the element types of a
///     tolerance family, the subject kinds of a per-subject family, the overload priority and the documentation -
///     comes from the attribute. Everything is reduced to strings here so that no symbol is held across the pipeline.
/// </remarks>
internal sealed class CollectionExpectationFamily
{
	private const string ItemPlaceholder = "{item}";
	private const string SubjectPlaceholder = "{subject}";

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

	public static CollectionExpectationFamily? Create(IMethodSymbol helper, AttributeData attributeData,
		Compilation compilation)
	{
		if (attributeData.ConstructorArguments.Length != 1 ||
		    attributeData.ConstructorArguments[0].Value?.ToString() is not { } name ||
		    helper.Parameters.Length < 2)
		{
			return null;
		}

		Declaration declaration = new();
		foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
		{
			declaration.Apply(namedArgument.Key, namedArgument.Value);
		}

		string positiveName = name.Replace("{Not}", "");
		string negatedName = name.Replace("{Not}", "Not");
		List<string> methods = [];
		foreach (string? subject in Subjects(helper, declaration, compilation))
		{
			foreach (Instantiation instantiation in Instantiate(declaration, compilation))
			{
				Instantiation bound = instantiation.For(subject);
				methods.Add(Render(helper, declaration, bound, positiveName, "expected", declaration.Summary,
					declaration.Remarks, false));
				methods.Add(Render(helper, declaration, bound, negatedName, "unexpected",
					declaration.NegatedSummary, declaration.NegatedRemarks ?? declaration.Remarks, true));
			}
		}

		if (methods.Count == 0)
		{
			return null;
		}

		return new CollectionExpectationFamily(helper.ContainingType.ContainingNamespace.ToString(),
			helper.ContainingType.Name, positiveName, methods);
	}

	/// <remarks>
	///     A per-subject family is emitted once per collection type listed on the containing class, which is how a
	///     subject kind that cannot reach the expectation through the covariance of <c>IThat&lt;out T&gt;</c> is
	///     added. A kind whose type does not exist in this compilation is skipped, so the target frameworks sort
	///     themselves out. Every other family is emitted once.
	/// </remarks>
	private static IEnumerable<string?> Subjects(IMethodSymbol helper, Declaration declaration,
		Compilation compilation)
	{
		if (!declaration.PerSubject)
		{
			yield return null;
			yield break;
		}

		foreach (AttributeData attribute in helper.ContainingType.GetAttributes()
			         .Where(x => x.AttributeClass?.Name == "CollectionSubjectsAttribute"))
		{
			foreach (TypedConstant value in attribute.ConstructorArguments.SelectMany(x => x.Values))
			{
				if (value.Value?.ToString() is { } template && Resolve(template, compilation) != null)
				{
					yield return template;
				}
			}
		}
	}

	private static INamedTypeSymbol? Resolve(string template, Compilation compilation)
	{
		int index = template.IndexOf('<');
		string metadataName = index < 0 ? template : $"{template.Substring(0, index)}`1";
		return compilation.GetTypeByMetadataName(metadataName);
	}

	/// <remarks>
	///     A factory expands into one instantiation per <c>Create*</c> method, and a nullable element additionally
	///     accepts a non-nullable expected collection, which is cast up.
	/// </remarks>
	private static IEnumerable<Instantiation> Instantiate(Declaration declaration, Compilation compilation)
	{
		if (declaration.Factory == null)
		{
			yield return new Instantiation(null, null, null);
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
		// Without a factory the element type is the one the expected collection already carries.
		string item = instantiation.Item ?? ElementOf(helper.Parameters[1].Type) ?? "TItem";
		Dictionary<string, string> substitutions = [];
		if (instantiation.Item != null)
		{
			substitutions["TItem"] = instantiation.Item;
			substitutions["TTolerance"] = instantiation.Tolerance ?? "";
		}

		if (instantiation.Subject != null)
		{
			substitutions["TCollection"] = instantiation.Subject.Replace(ItemPlaceholder, item);
		}

		// The expected parameter may take the non-nullable element while the subject keeps the nullable one.
		Dictionary<string, string> expectedSubstitutions = instantiation.ExpectedItem == null
			? substitutions
			: new Dictionary<string, string>(substitutions) { ["TItem"] = instantiation.ExpectedItem, };

		string expectedType = declaration.ExpectedType == null
			? Substitute(helper.Parameters[1].Type, expectedSubstitutions)
			: Qualify(declaration.ExpectedType
				.Replace(SubjectPlaceholder, instantiation.Subject ?? "")
				.Replace(ItemPlaceholder, instantiation.ExpectedItem ?? item));
		string argument = instantiation.ExpectedItem == instantiation.Item
			? parameterName
			: $"global::System.Linq.Enumerable.Cast<{item}>({parameterName})";

		List<string> arguments = ["subject", argument,];
		arguments.AddRange(helper.Parameters.Skip(2).Select(x => ArgumentFor(x, instantiation, negated)));

		// A type parameter bound by the factory or by the subject kind is consumed by the instantiation.
		string[] ownTypeParameters = helper.TypeParameters
			.Select(x => x.Name).Where(x => !substitutions.ContainsKey(x)).ToArray();
		string typeParameters = ownTypeParameters.Length == 0 ? "" : $"<{string.Join(", ", ownTypeParameters)}>";
		string typeArguments = helper.TypeParameters.Length == 0
			? ""
			: $"<{string.Join(", ", helper.TypeParameters.Select(x
				=> substitutions.TryGetValue(x.Name, out string? v) ? v : x.Name))}>";

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
		         			{{expectedType}} {{parameterName}},
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
	///     The helper is rendered unconstructed and the bound type parameters are replaced by name, because an
	///     instantiation cannot construct a method that still carries the overload's own type parameters.
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

	private static string? ElementOf(ITypeSymbol type)
		=> type is INamedTypeSymbol { TypeArguments.Length: 1, } named
			? named.TypeArguments[0].ToDisplayString(TypeFormat)
			: null;

	private static string Qualify(string type)
		=> Regex.Replace(type, @"(?<!global::)\bSystem\.", "global::System.");

	private sealed class Declaration
	{
		public string? Factory { get; private set; }
		public string? ExpectedType { get; private set; }
		public bool PerSubject { get; private set; }
		public int Priority { get; private set; }
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
				case "ExpectedType":
					ExpectedType = value.Value?.ToString();
					break;
				case "PerSubject":
					PerSubject = value.Value as bool? ?? false;
					break;
				case "Priority":
					Priority = value.Value as int? ?? 0;
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

	private sealed class Instantiation(string? item, string? expectedItem, string? tolerance)
	{
		public string? Item { get; } = item;
		public string? ExpectedItem { get; } = expectedItem;
		public string? Tolerance { get; } = tolerance;
		public string? Subject { get; private init; }
		public string? FactoryCall { get; init; }

		public Instantiation For(string? subject)
			=> new(Item, ExpectedItem, Tolerance) { Subject = subject, FactoryCall = FactoryCall, };
	}
}
