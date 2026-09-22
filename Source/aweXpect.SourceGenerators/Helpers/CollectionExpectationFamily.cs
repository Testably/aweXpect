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
///     comes from the attribute. Everything is reduced to strings here so that no symbol is held across the pipeline,
///     and the record equality lets the pipeline skip the output when nothing changed.
/// </remarks>
internal sealed record CollectionExpectationFamily(
	string Namespace,
	string ClassName,
	string FileName,
	EquatableArray<string> Methods)
{
	private const string ItemPlaceholder = "{item}";
	private const string SubjectPlaceholder = "{subject}";

	private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
		.WithMiscellaneousOptions(
			SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
			SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

	public static CollectionExpectationFamily? Create(IMethodSymbol helper, AttributeData attributeData,
		Compilation compilation)
	{
		if (attributeData.ConstructorArguments.Length != 1 ||
		    attributeData.ConstructorArguments[0].Value?.ToString() is not { } name ||
		    helper.Parameters.Length < 1)
		{
			return null;
		}

		Declaration declaration = new();
		foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
		{
			declaration.Apply(namedArgument.Key, namedArgument.Value);
		}

		string positiveName = name.Replace("{Not}", "");
		string negatedName = declaration.NegatedName ?? name.Replace("{Not}", "Not");
		// The expected parameter follows the subject, but an expectation such as IsInAscendingOrder takes none, so the
		// negation flag comes right after the subject instead.
		IParameterSymbol? expected = helper.Parameters.Length > 1 &&
		                             helper.Parameters[1].Type.SpecialType != SpecialType.System_Boolean
			? helper.Parameters[1]
			: null;
		// Only an expected value turns into an unexpected one; a name such as "predicate" reads the same either way.
		string parameterName = expected?.Name ?? "";
		string negatedParameterName = parameterName == "expected" ? "unexpected" : parameterName;
		// Only a helper that takes the negation flag has a negated form.
		bool hasPolarity = helper.Parameters.Skip(expected == null ? 1 : 2)
			.Any(x => x.Type.SpecialType == SpecialType.System_Boolean);
		// A single expected value of a nullable element already accepts the non-nullable one.
		bool castsUp = expected is { Type: not ITypeParameterSymbol, };
		List<string> methods = [];
		foreach (SubjectKind? subject in Subjects(helper, declaration, compilation))
		{
			foreach (Instantiation instantiation in Instantiate(declaration, compilation, castsUp))
			{
				Instantiation bound = instantiation.For(subject);
				methods.Add(Render(helper, expected, declaration, bound, positiveName, parameterName,
					declaration.Summary, declaration.Remarks, false));
				if (hasPolarity)
				{
					methods.Add(Render(helper, expected, declaration, bound, negatedName, negatedParameterName,
						declaration.NegatedSummary, declaration.NegatedRemarks ?? declaration.Remarks, true));
				}
			}
		}

		if (methods.Count == 0)
		{
			return null;
		}

		// The generated file is named after the helper's own file, so that a before/after diff works per file.
		string fileName = Path.GetFileNameWithoutExtension(helper.Locations.FirstOrDefault()?.SourceTree?.FilePath);
		if (string.IsNullOrEmpty(fileName))
		{
			fileName = $"{helper.ContainingType.Name}.{positiveName}";
		}
		return new CollectionExpectationFamily(helper.ContainingType.ContainingNamespace.ToString(),
			helper.ContainingType.Name, fileName, new EquatableArray<string>(methods.ToArray()));
	}

	/// <remarks>
	///     A per-subject family is emitted once per collection type listed on the containing class, which is how a
	///     subject kind that cannot reach the expectation through the covariance of <c>IThat&lt;out T&gt;</c> is
	///     added. A kind whose type does not exist in this compilation is skipped, so the target frameworks sort
	///     themselves out. Every other family is emitted once.
	/// </remarks>
	private static IEnumerable<SubjectKind?> Subjects(IMethodSymbol helper, Declaration declaration,
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
			int priority = attribute.NamedArguments.FirstOrDefault(x => x.Key == "Priority").Value.Value as int? ?? 0;
			string? remarks = attribute.NamedArguments.FirstOrDefault(x => x.Key == "Remarks").Value.Value?.ToString();
			foreach (TypedConstant value in attribute.ConstructorArguments.SelectMany(x => x.Values))
			{
				if (value.Value?.ToString() is { } template &&
				    Resolve(template, compilation) is { } definition)
				{
					yield return new SubjectKind(Qualify(template), definition.IsValueType, priority, remarks,
						KindConstraints(template, definition));
				}
			}
		}
	}

	private static INamedTypeSymbol? Resolve(string template, Compilation compilation)
	{
		int index = template.IndexOf('<');
		string metadataName = index < 0
			? template
			: $"{template.Substring(0, index)}`{TypeArguments(template).Length}";
		return compilation.GetTypeByMetadataName(metadataName);
	}

	private static string[] TypeArguments(string template)
	{
		int start = template.IndexOf('<');
		if (start < 0)
		{
			return [];
		}

		List<string> arguments = [];
		int depth = 0;
		int from = start + 1;
		for (int i = from; i < template.Length; i++)
		{
			switch (template[i])
			{
				case '<':
					depth++;
					break;
				case '>' when depth > 0:
					depth--;
					break;
				case '>':
				case ',' when depth == 0:
					arguments.Add(template.Substring(from, i - from).Trim());
					from = i + 1;
					break;
			}
		}

		return arguments.ToArray();
	}

	/// <remarks>
	///     A kind's own type parameter constraints, such as <c>TKey : notnull</c> of <c>Dictionary&lt;TKey, TValue&gt;</c>,
	///     have to hold for the overload's type parameter that fills them.
	/// </remarks>
	private static Dictionary<string, List<string>> KindConstraints(string template, INamedTypeSymbol definition)
	{
		string[] arguments = TypeArguments(template);
		Dictionary<string, Bound> byName = definition.TypeParameters
			.Select((x, i) => (x.Name, Argument: i < arguments.Length ? arguments[i] : x.Name))
			.ToDictionary(x => x.Name, x => new Bound(x.Argument, $"{x.Argument}?"));
		Dictionary<string, List<string>> result = [];
		for (int i = 0; i < arguments.Length && i < definition.TypeParameters.Length; i++)
		{
			List<string> constraints = ConstraintsOf(definition.TypeParameters[i], byName);
			if (constraints.Count > 0)
			{
				result[arguments[i]] = constraints;
			}
		}

		return result;
	}

	/// <remarks>
	///     A factory expands into one instantiation per <c>Create*</c> method, and a nullable element additionally
	///     accepts a non-nullable expected collection, which is cast up.
	/// </remarks>
	private static IEnumerable<Instantiation> Instantiate(Declaration declaration, Compilation compilation,
		bool castsUp)
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

			ITypeSymbol item = options.TypeArguments[0];
			ITypeSymbol tolerance = options.TypeArguments[1];
			string itemName = item.ToDisplayString(TypeFormat);
			string call = $"{factory.ToDisplayString(TypeFormat)}.{factoryMethod.Name}()";
			yield return new Instantiation(itemName, itemName, tolerance.ToDisplayString(TypeFormat))
			{
				ItemIsValueType = item.IsValueType, ToleranceIsValueType = tolerance.IsValueType, FactoryCall = call,
			};
			if (castsUp && item is INamedTypeSymbol
			    {
				    ConstructedFrom.SpecialType: SpecialType.System_Nullable_T,
			    } nullable)
			{
				yield return new Instantiation(itemName, nullable.TypeArguments[0].ToDisplayString(TypeFormat),
					tolerance.ToDisplayString(TypeFormat))
				{
					ItemIsValueType = true, ToleranceIsValueType = tolerance.IsValueType, FactoryCall = call,
				};
			}
		}
	}

	private static string Render(IMethodSymbol helper, IParameterSymbol? expected, Declaration declaration,
		Instantiation instantiation, string methodName, string parameterName, string summary, string? remarks,
		bool negated)
	{
		// Without a factory the element type is the one the expected parameter already carries.
		string item = instantiation.Item ?? (expected == null ? "TItem" : ElementOf(expected.Type));
		Dictionary<string, Bound> substitutions = [];
		if (instantiation.Item != null)
		{
			substitutions["TItem"] = Bind(helper, "TItem", instantiation.Item, instantiation.ItemIsValueType);
			substitutions["TTolerance"] = Bind(helper, "TTolerance", instantiation.Tolerance ?? "",
				instantiation.ToleranceIsValueType);
		}

		if (instantiation.Subject != null)
		{
			substitutions["TCollection"] = Bind(helper, "TCollection",
				instantiation.Subject.Template.Replace(ItemPlaceholder, item), instantiation.Subject.IsValueType);
		}

		// The expected parameter may take the non-nullable element while the subject keeps the nullable one.
		Dictionary<string, Bound> expectedSubstitutions = instantiation.ExpectedItem == null
			? substitutions
			: new Dictionary<string, Bound>(substitutions)
			{
				["TItem"] = Bind(helper, "TItem", instantiation.ExpectedItem, instantiation.ItemIsValueType),
			};

		string subjectName = helper.Parameters[0].Name;
		List<string> parameters = [$"this {Substitute(helper.Parameters[0].Type, substitutions)} {subjectName}",];
		List<string> arguments = [subjectName,];
		// Only a helper that takes the expression can echo one, and a params array has none to echo.
		bool echoesExpression = expected != null && !declaration.Params &&
		                        helper.Parameters.Skip(2).Any(x => x.Type.SpecialType == SpecialType.System_String);
		if (expected != null)
		{
			string expectedType = declaration.ExpectedType == null
				? Substitute(expected.Type, expectedSubstitutions)
				: Qualify(declaration.ExpectedType
					.Replace(SubjectPlaceholder, instantiation.Subject?.Template ?? "")
					.Replace(ItemPlaceholder, instantiation.ExpectedItem ?? item));
			if (declaration.Params)
			{
				// A params array has no single caller expression, so the helper formats the value instead.
				expectedType =
					$"params {(declaration.ExpectedType == null ? item : Qualify(declaration.ExpectedType))}[]";
			}

			parameters.Add($"{expectedType} {parameterName}");
			if (echoesExpression)
			{
				parameters.Add(
					$"[global::System.Runtime.CompilerServices.CallerArgumentExpression(\"{parameterName}\")]\n" +
					"\t\t\tstring doNotPopulateThisValue = \"\"");
			}

			arguments.Add(instantiation.ExpectedItem == instantiation.Item
				? parameterName
				: $"global::System.Linq.Enumerable.Cast<{item}>({parameterName})");
		}

		arguments.AddRange(helper.Parameters.Skip(expected == null ? 1 : 2)
			.Select(x => ArgumentFor(x, instantiation, negated, echoesExpression)));

		// A type parameter bound by the factory or by the subject kind is consumed by the instantiation.
		ITypeParameterSymbol[] ownTypeParameters = helper.TypeParameters
			.Where(x => !substitutions.ContainsKey(x.Name)).ToArray();
		string typeParameters = ownTypeParameters.Length == 0
			? ""
			: $"<{string.Join(", ", ownTypeParameters.Select(x => x.Name))}>";
		string constraints = string.Concat(ownTypeParameters
			.Select(x => Constraints(x, substitutions, instantiation.Subject))
			.Where(x => x != null)
			.Select(x => $"\n\t\t{x}"));
		string typeArguments = helper.TypeParameters.Length == 0
			? ""
			: $"<{string.Join(", ", helper.TypeParameters.Select(x
				=> substitutions.TryGetValue(x.Name, out Bound? v) ? v.Type : x.Name))}>";

		string header = $"""
		                 	/// <summary>
		                 	///     {summary}
		                 	/// </summary>
		                 """;
		// A subject kind can have its own reason for its priority, which applies to every family it takes part in.
		remarks = string.Join("\n", new[] { remarks, instantiation.Subject?.Remarks, }
			.Where(x => !string.IsNullOrEmpty(x)));
		if (!string.IsNullOrEmpty(remarks))
		{
			header += $"\n\t/// <remarks>\n\t///     {remarks!.Replace("\n", "\n\t///     ")}\n\t/// </remarks>";
		}

		if (declaration.GuaranteesNotNull)
		{
			header += "\n\t[global::aweXpect.Core.GuaranteesNotNull]";
		}

		int priority = instantiation.Subject?.Priority is { } kindPriority and not 0
			? kindPriority
			: declaration.Priority;
		if (priority != 0)
		{
			header +=
				$"\n\t[global::System.Runtime.CompilerServices.OverloadResolutionPriority({priority})]";
		}

		return $$"""
		         {{header}}
		         	public static {{Substitute(helper.ReturnType, substitutions)}}
		         		{{methodName}}{{typeParameters}}(
		         			{{string.Join(",\n\t\t\t", parameters)}}){{constraints}}
		         		=> {{helper.Name}}{{typeArguments}}(
		         {{string.Join(",\n", arguments.Select(x => "\t\t\t" + x))}});
		         """;
	}

	/// <remarks>
	///     An annotated <c>T?</c> is <c>T</c> itself once a value type fills an unconstrained <c>T</c>, and only
	///     <c>Nullable&lt;T&gt;</c> when <c>T</c> is constrained to a struct; a reference type keeps the annotation.
	/// </remarks>
	private static Bound Bind(IMethodSymbol helper, string name, string type, bool isValueType)
	{
		bool isNullableOfType = !isValueType ||
		                        helper.TypeParameters.Any(x => x.Name == name && x.HasValueTypeConstraint);
		return new Bound(type, isNullableOfType ? $"{type.TrimEnd('?')}?" : type);
	}

	private static string ArgumentFor(IParameterSymbol parameter, Instantiation instantiation, bool negated,
		bool echoesExpression)
	{
		if (parameter.Type.Name == "ObjectEqualityWithToleranceOptions")
		{
			return instantiation.FactoryCall ?? "default!";
		}

		return parameter.Type.SpecialType switch
		{
			SpecialType.System_Boolean => negated ? "true" : "false",
			_ => echoesExpression ? "doNotPopulateThisValue" : "null",
		};
	}

	/// <remarks>
	///     A type parameter the overload keeps also keeps the helper's constraints, with the bound ones substituted.
	/// </remarks>
	private static string? Constraints(ITypeParameterSymbol typeParameter, Dictionary<string, Bound> substitutions,
		SubjectKind? subject)
	{
		List<string> constraints = [];
		if (subject != null && subject.Constraints.TryGetValue(typeParameter.Name, out List<string>? fromKind))
		{
			constraints.AddRange(fromKind);
		}

		constraints.AddRange(ConstraintsOf(typeParameter, substitutions));
		constraints = constraints.Distinct().ToList();
		return constraints.Count == 0 ? null : $"where {typeParameter.Name} : {string.Join(", ", constraints)}";
	}

	private static List<string> ConstraintsOf(ITypeParameterSymbol typeParameter,
		Dictionary<string, Bound> substitutions)
	{
		List<string> constraints = [];
		if (typeParameter.HasReferenceTypeConstraint)
		{
			constraints.Add("class");
		}
		else if (typeParameter.HasUnmanagedTypeConstraint)
		{
			constraints.Add("unmanaged");
		}
		else if (typeParameter.HasValueTypeConstraint)
		{
			constraints.Add("struct");
		}
		else if (typeParameter.HasNotNullConstraint)
		{
			constraints.Add("notnull");
		}

		constraints.AddRange(typeParameter.ConstraintTypes.Select(x => Substitute(x, substitutions)));
		if (typeParameter.HasConstructorConstraint)
		{
			constraints.Add("new()");
		}

		return constraints;
	}

	/// <remarks>
	///     The helper is rendered unconstructed and the bound type parameters are replaced by name, because an
	///     instantiation cannot construct a method that still carries the overload's own type parameters.
	/// </remarks>
	private static string Substitute(ITypeSymbol type, Dictionary<string, Bound> substitutions)
	{
		string result = type.ToDisplayString(TypeFormat);
		foreach (KeyValuePair<string, Bound> substitution in substitutions)
		{
			result = Regex.Replace(result, $@"\b{substitution.Key}\?", Escape(substitution.Value.NullableType));
			result = Regex.Replace(result, $@"\b{substitution.Key}\b", Escape(substitution.Value.Type));
		}

		return result;
	}

	private static string Escape(string replacement) => replacement.Replace("$", "$$");

	/// <remarks>
	///     The element of an array is its element type, the element of an expected collection or of a predicate is its
	///     first type argument, and a single expected value is the element itself.
	/// </remarks>
	private static string ElementOf(ITypeSymbol type)
		=> type switch
		{
			IArrayTypeSymbol array => array.ElementType.ToDisplayString(TypeFormat),
			INamedTypeSymbol { TypeArguments.Length: > 0, } named => named.TypeArguments[0].ToDisplayString(TypeFormat),
			_ => type.ToDisplayString(TypeFormat),
		};

	private static string Qualify(string type)
		=> Regex.Replace(type, @"(?<!global::)\bSystem\.", "global::System.");

	/// <summary>
	///     A type that fills one of the helper's type parameters, and what its annotated <c>T?</c> form is.
	/// </summary>
	private sealed record Bound(string Type, string NullableType);

	private sealed class Declaration
	{
		public string? Factory { get; private set; }
		public string? ExpectedType { get; private set; }
		public bool PerSubject { get; private set; }
		public bool GuaranteesNotNull { get; private set; }
		public bool Params { get; private set; }
		public string? NegatedName { get; private set; }
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
				case "GuaranteesNotNull":
					GuaranteesNotNull = value.Value as bool? ?? false;
					break;
				case "Params":
					Params = value.Value as bool? ?? false;
					break;
				case "NegatedName":
					NegatedName = value.Value?.ToString();
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
		public bool ItemIsValueType { get; init; }
		public string? ExpectedItem { get; } = expectedItem;
		public string? Tolerance { get; } = tolerance;
		public bool ToleranceIsValueType { get; init; }
		public SubjectKind? Subject { get; private init; }
		public string? FactoryCall { get; init; }

		public Instantiation For(SubjectKind? subject)
			=> new(Item, ExpectedItem, Tolerance)
			{
				ItemIsValueType = ItemIsValueType,
				ToleranceIsValueType = ToleranceIsValueType,
				Subject = subject,
				FactoryCall = FactoryCall,
			};
	}

	/// <summary>
	///     One entry of <c>[CollectionSubjects]</c>: the collection type, where <c>{item}</c> marks the element, with the
	///     priority, remarks and type parameter constraints its overloads need.
	/// </summary>
	private sealed class SubjectKind(
		string template,
		bool isValueType,
		int priority,
		string? remarks,
		Dictionary<string, List<string>> constraints)
	{
		public string Template { get; } = template;
		public bool IsValueType { get; } = isValueType;
		public int Priority { get; } = priority;
		public string? Remarks { get; } = remarks;
		public Dictionary<string, List<string>> Constraints { get; } = constraints;
	}
}
