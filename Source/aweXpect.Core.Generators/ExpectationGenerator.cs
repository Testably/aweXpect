using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace aweXpect.Core.Generators;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> that generates an expectation from every method marked with
///     <c>aweXpect.Core.Extending.GenerateExpectationAttribute</c>.
/// </summary>
[Generator]
public class ExpectationGenerator : IIncrementalGenerator
{
	private const string Attribute = "aweXpect.Core.Extending.GenerateExpectationAttribute";
	private const string CollectionAttribute = "aweXpect.Core.Extending.GenerateCollectionExpectationAttribute";
	private const string TokenAttribute = "aweXpect.Core.Extending.EvaluationCancellationTokenAttribute";
	private const string TokenAttributeName = "EvaluationCancellationToken";
	private const string OptionsAttribute = "aweXpect.Core.Extending.ExpectationOptionsAttribute";
	private const string OptionsAttributeName = "ExpectationOptions";
	private const string NegatedParameterNameAttribute ="aweXpect.Core.Extending.NegatedParameterNameAttribute";
	private const string NegatedParameterNameAttributeName = "NegatedParameterName";
	private const string HelpLink = "https://docs.testably.org/aweXpect/analyzers#expectation-generator";

	private static readonly DiagnosticDescriptor InvalidClass = new(
		"aweXpect4001",
		"The class cannot hold the generated expectation",
		"'{0}' cannot hold the generated expectation, because extension methods need a top-level, non-generic, static partial class that is not file-local",
		"aweXpect.Core.Generators",
		DiagnosticSeverity.Error,
		true,
		helpLinkUri: HelpLink);

	private static readonly DiagnosticDescriptor InvalidMethod = new(
		"aweXpect4002",
		"The method cannot generate an expectation",
		"'{0}' cannot generate an expectation, because {1}",
		"aweXpect.Core.Generators",
		DiagnosticSeverity.Error,
		true,
		helpLinkUri: HelpLink);

	private static readonly DiagnosticDescriptor InvalidResult = new(
		"aweXpect4003",
		"The result method is not suitable",
		"'{0}' names no static method that returns a string and takes the subject, optionally followed by the other parameters of '{1}' and by the string 'it' and the ExpectationGrammars",
		"aweXpect.Core.Generators",
		DiagnosticSeverity.Error,
		true,
		helpLinkUri: HelpLink);

	private static readonly DiagnosticDescriptor InvalidFormattedItem = new(
		"aweXpect4004",
		"The formatted item method is not suitable",
		"'{0}' names no static method that returns a value and takes the item of '{1}', which can be null for a reference type",
		"aweXpect.Core.Generators",
		DiagnosticSeverity.Error,
		true,
		helpLinkUri: HelpLink);

	private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
		.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

	private static readonly SymbolDisplayFormat NamespaceFormat = SymbolDisplayFormat.FullyQualifiedFormat
		.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValuesProvider<ExpectationResult> expectations = context.SyntaxProvider
			.ForAttributeWithMetadataName(Attribute,
				static (node, _) => node is MethodDeclarationSyntax,
				static (ctx, _) => GetExpectation(ctx))
			.WithTrackingName("Expectations");

		context.RegisterSourceOutput(expectations, static (spc, result) =>
		{
			if (result.Problem is not null)
			{
				spc.ReportDiagnostic(result.Problem.ToDiagnostic());
			}

			if (result.Model is not null)
			{
				spc.AddSource(result.Model.HintName,
					SourceText.From(ExpectationWriter.Write(result.Model), Encoding.UTF8));
			}
		});

		IncrementalValueProvider<bool> supportsAsyncEnumerables = context.CompilationProvider
			.Select(static (compilation, _)
				=> compilation.GetTypeByMetadataName("aweXpect.IAsyncEnumerableItems`1") is not null);
		IncrementalValuesProvider<CollectionExpectationResult> collectionExpectations = context.SyntaxProvider
			.ForAttributeWithMetadataName(CollectionAttribute,
				static (node, _) => node is MethodDeclarationSyntax,
				static (ctx, _) => GetCollectionExpectation(ctx))
			.Combine(supportsAsyncEnumerables)
			.Select(static (pair, _) => pair.Left.Model is null
				? pair.Left
				: pair.Left with { Model = pair.Left.Model with { SupportsAsyncEnumerables = pair.Right, }, })
			.WithTrackingName("CollectionExpectations");

		context.RegisterSourceOutput(collectionExpectations, static (spc, result) =>
		{
			if (result.Problem is not null)
			{
				spc.ReportDiagnostic(result.Problem.ToDiagnostic());
			}

			if (result.Model is not null)
			{
				spc.AddSource(result.Model.Class.HintName,
					SourceText.From(CollectionExpectationWriter.Write(result.Model), Encoding.UTF8));
			}
		});
	}

	private static ExpectationResult GetExpectation(GeneratorAttributeSyntaxContext context)
	{
		if (GetSignatureProblem(context, false, out Signature signature) is { } signatureProblem)
		{
			return new ExpectationResult(null, signatureProblem);
		}

		if (signature.OptionsParameter is { } optionsParameter)
		{
			return signature.Fail(
				$"the parameter '{optionsParameter.Name}' is marked with [{OptionsAttributeName}], which only [GenerateCollectionExpectation] supports");
		}

		AttributeData attribute = context.Attributes[0];
		if (attribute.ConstructorArguments.Length != 2 ||
		    attribute.ConstructorArguments[0].Value is not string name ||
		    attribute.ConstructorArguments[1].Value is not string expectationText)
		{
			return signature.Fail("the name and the expectation text are required");
		}

		if (GetNameProblem(name, GetNamedArgument<string>(attribute, "NegatedName"),
			    out string positiveName, out string? negatedName) is { } nameProblem)
		{
			return signature.Fail(nameProblem);
		}

		if (GetTextProblem(expectationText, GetNamedArgument<string>(attribute, "NegatedExpectationText"),
			    signature.GetParameterNames(),
			    out EquatableArray<TextPart> positiveText, out EquatableArray<TextPart> negatedText) is { } textProblem)
		{
			return signature.Fail(textProblem);
		}

		string? resultMethod = GetNamedArgument<string>(attribute, "Result");
		ResultModel? result = null;
		if (resultMethod is not null &&
		    !TryFindResultMethod(signature.Class, resultMethod, signature.Subject, signature.ExpectationParameters,
			    out result))
		{
			return new ExpectationResult(null,
				new Problem(InvalidResult, signature.Location, resultMethod, signature.MethodName));
		}

		ClassModel classModel = signature.ToClassModel(context, "");
		return new ExpectationResult(new ExpectationModel(
			classModel.Usings,
			classModel.NamespaceUsings,
			classModel.Namespace,
			classModel.ClassName,
			classModel.ClassAccessibility,
			classModel.MethodAccessibility,
			classModel.HintName,
			classModel.PredicateName,
			positiveName,
			negatedName,
			positiveText,
			negatedText,
			GetNamedArgument<string>(attribute, "Summary"),
			GetNamedArgument<string>(attribute, "NegatedSummary"),
			GetNamedArgument<string>(attribute, "Remarks"),
			GetNamedArgument<string>(attribute, "NegatedRemarks"),
			signature.IsAsync,
			signature.CancellationTokenIndex,
			signature.ReturnsPredicateResult,
			classModel.Parameters,
			result,
			GetOverloads(signature.Subject, signature.ConstraintPrefix,
				GetNamedArgument<bool?>(attribute, "NullableOverload") ?? true)), null);
	}

	private static CollectionExpectationResult GetCollectionExpectation(GeneratorAttributeSyntaxContext context)
	{
		if (GetSignatureProblem(context, true, out Signature signature) is { } signatureProblem)
		{
			return new CollectionExpectationResult(null, signatureProblem);
		}

		AttributeData attribute = context.Attributes[0];
		ImmutableArray<TypedConstant> arguments = attribute.ConstructorArguments;
		if (arguments.Length is not (2 or 3) ||
		    arguments[0].Value is not string name ||
		    arguments[arguments.Length - 1].Value is not string pluralExpectationText)
		{
			return signature.FailCollection("the name and the plural expectation text are required");
		}

		if ((arguments.Length == 3 ? arguments[1].Value as string : GetSingularText(signature.Method)) is not
		    { } expectationText)
		{
			return signature.FailCollection(
				"the expectation text is required, unless the method is also marked with [GenerateExpectation]");
		}

		string? negatedName = GetNamedArgument<string>(attribute, "NegatedName");
		if (GetCollectionNameProblem(name, negatedName) is { } nameProblem)
		{
			return signature.FailCollection(nameProblem);
		}

		if (GetCollectionTextProblem(expectationText, pluralExpectationText, signature.GetParameterNames(),
			    out EquatableArray<TextPart> text, out EquatableArray<TextPart> pluralText) is { } textProblem)
		{
			return signature.FailCollection(textProblem);
		}

		string? formattedItem = GetNamedArgument<string>(attribute, "FormattedItem");
		string? formattedItemMethod = null;
		if (formattedItem is not null &&
		    !TryFindFormattedItemMethod(context.SemanticModel.Compilation, signature.Class, formattedItem,
			    signature.Subject, out formattedItemMethod))
		{
			return new CollectionExpectationResult(null,
				new Problem(InvalidFormattedItem, signature.Location, formattedItem, signature.MethodName));
		}

		EquatableArray<ItemModel> items = GetItems(signature.Subject, signature.ConstraintPrefix,
			GetNamedArgument<bool?>(attribute, "NullableOverload") ?? true);
		bool nullItemFailsBothWays = GetNamedArgument<bool?>(attribute, "NullItemFailsBothWays") ?? false;
		if (GetNullItemFailsBothWaysProblem(nullItemFailsBothWays, signature.Subject, items) is { } nullItemProblem)
		{
			return signature.FailCollection(nullItemProblem);
		}

		if (GetOptionsProblem(signature, out OptionsModel? options) is { } optionsProblem)
		{
			return signature.FailCollection(optionsProblem);
		}

		ITypeParameterSymbol? itemTypeParameter = GetItemTypeParameter(signature.Method);
		return new CollectionExpectationResult(new CollectionExpectationModel(
			signature.ToClassModel(context, ".Items"),
			name,
			text,
			pluralText,
			GetResultVerb(expectationText),
			GetNamedArgument<string>(attribute, "Summary"),
			GetNamedArgument<string>(attribute, "Remarks"),
			GetPredicateKind(signature.Method.ReturnType),
			signature.CancellationTokenIndex,
			signature.ReturnsPredicateResult,
			items,
			nullItemFailsBothWays,
			GetNamedArgument<bool?>(attribute, "CollectionOverload") ?? false,
			negatedName,
			formattedItemMethod,
			itemTypeParameter is null ? null : Escape(itemTypeParameter.Name),
			itemTypeParameter is null ? null : GetConstraints(itemTypeParameter),
			false,
			options), null);
	}

	/// <summary>
	///     Finds the method that returns the value which stands for an item in the failure message: it takes the item,
	///     which can be <see langword="null" /> for a reference type, and returns a value.
	/// </summary>
	private static bool TryFindFormattedItemMethod(Compilation compilation, INamedTypeSymbol classSymbol, string name,
		IParameterSymbol subject, out string? method)
	{
		ITypeSymbol subjectType = subject.Type;
		foreach (IMethodSymbol candidate in classSymbol.GetMembers(name).OfType<IMethodSymbol>())
		{
			if (candidate is { IsStatic: true, IsGenericMethod: false, ReturnsVoid: false, Parameters.Length: 1, } &&
			    candidate.Parameters[0].RefKind == RefKind.None &&
			    compilation.ClassifyConversion(subjectType, candidate.Parameters[0].Type).IsImplicit &&
			    (subjectType.IsValueType ||
			     candidate.Parameters[0].NullableAnnotation != NullableAnnotation.NotAnnotated))
			{
				method = Escape(name);
				return true;
			}
		}

		method = null;
		return false;
	}

	/// <summary>
	///     The expectation text of the <c>GenerateExpectationAttribute</c> on the <paramref name="method" /> without
	///     <c>{not}</c>, which is the text on a single item, or <see langword="null" /> without that attribute.
	/// </summary>
	private static string? GetSingularText(IMethodSymbol method)
		=> method.GetAttributes()
				.FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == Attribute)?
				.ConstructorArguments.ElementAtOrDefault(1).Value is string text
			? text.Replace("{not}", "").Replace("  ", " ").Trim()
			: null;

	/// <summary>
	///     The result that exposes the options of the <paramref name="type" />, when it is an options type of
	///     aweXpect.Core that a parameter marked with the <c>ExpectationOptionsAttribute</c> supports.
	/// </summary>
	private static string? GetOptionsResult(ITypeSymbol type)
		=> type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) switch
		{
			"global::aweXpect.Options.StringEqualityOptions" => "global::aweXpect.Results.StringEqualityTypeResult",
			_ => null,
		};

	/// <summary>
	///     Finds the options parameter and the <see langword="string" /> parameter that the options compare with.
	/// </summary>
	private static string? GetOptionsProblem(Signature signature, out OptionsModel? options)
	{
		options = null;
		if (signature.OptionsParameter is not { } parameter)
		{
			return null;
		}

		List<int> patterns = signature.ExpectationParameters
			.Select((p, i) => (p, i))
			.Where(x => x.p.Type.SpecialType == SpecialType.System_String)
			.Select(x => x.i)
			.ToList();
		if (patterns.Count != 1)
		{
			return $"the options parameter '{parameter.Name}' needs exactly one string parameter to compare with";
		}

		options = new OptionsModel(parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
			GetOptionsResult(parameter.Type)!, parameter.Ordinal - 1, patterns[0]);
		return null;
	}

	private static bool IsCancellationToken(ITypeSymbol type)
		=> type is { Name: "CancellationToken", ContainingNamespace: var ns, } &&
		   ns.ToDisplayString() == "System.Threading";

	private static string? GetTokenProblem(List<IParameterSymbol> markedTokens)
	{
		if (markedTokens.Count > 1)
		{
			return $"more than one parameter is marked with [{TokenAttributeName}]";
		}

		if (markedTokens.FirstOrDefault() is not { } token)
		{
			return null;
		}

		if (token.Ordinal == 0)
		{
			return $"the subject cannot be marked with [{TokenAttributeName}]";
		}

		return IsCancellationToken(token.Type)
			? null
			: $"the parameter '{token.Name}' is marked with [{TokenAttributeName}], but is no CancellationToken";
	}

	private static string? GetOptionsMarkerProblem(List<IParameterSymbol> markedOptions)
	{
		if (markedOptions.Count > 1)
		{
			return $"more than one parameter is marked with [{OptionsAttributeName}]";
		}

		if (markedOptions.FirstOrDefault() is not { } parameter)
		{
			return null;
		}

		if (parameter.Ordinal == 0)
		{
			return $"the subject cannot be marked with [{OptionsAttributeName}]";
		}

		return GetOptionsResult(parameter.Type) is not null
			? null
			: $"the parameter '{parameter.Name}' is marked with [{OptionsAttributeName}], but '{parameter.Type.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)}' is no supported options type";
	}

	/// <summary>
	///     Checks the class, the method and its parameters, which both attributes require alike.
	/// </summary>
	/// <param name="context">The context of the marked method.</param>
	/// <param name="allowsItemTypeParameter">Whether the type of the subject can be a type parameter of the method.</param>
	/// <param name="signature">The signature of the marked method.</param>
	private static Problem? GetSignatureProblem(GeneratorAttributeSyntaxContext context, bool allowsItemTypeParameter,
		out Signature signature)
	{
		IMethodSymbol method = (IMethodSymbol)context.TargetSymbol;
		Location location = ((MethodDeclarationSyntax)context.TargetNode).Identifier.GetLocation();
		INamedTypeSymbol classSymbol = method.ContainingType;
		string methodName = method.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);
		signature = new Signature(method, location, classSymbol, methodName);
		if (!IsExtensionClass(classSymbol, context.TargetNode))
		{
			return new Problem(InvalidClass, location, classSymbol.ToDisplayString());
		}

		if (GetMethodProblem(method, allowsItemTypeParameter) is { } methodProblem)
		{
			return new Problem(InvalidMethod, location, methodName, methodProblem);
		}

		if (GetTokenProblem(signature.MarkedTokens) is { } tokenProblem)
		{
			return new Problem(InvalidMethod, location, methodName, tokenProblem);
		}

		if (GetOptionsMarkerProblem(signature.MarkedOptions) is { } optionsMarkerProblem)
		{
			return new Problem(InvalidMethod, location, methodName, optionsMarkerProblem);
		}

		if (signature.ExpectationParameters.FirstOrDefault(p => p.Name == "subject") is { } clash)
		{
			return new Problem(InvalidMethod, location, methodName,
				$"the parameter '{clash.Name}' would clash with the subject of the expectation");
		}

		if (GetNegatedParameterNameProblem(signature) is { } negatedParameterNameProblem)
		{
			return new Problem(InvalidMethod, location, methodName, negatedParameterNameProblem);
		}

		if (signature.SignatureParameters.FirstOrDefault(p => GetAccessibility(p.Type) < Accessibility.Internal) is
		    { } inaccessible)
		{
			return new Problem(InvalidMethod, location, methodName,
				$"the type of the parameter '{inaccessible.Name}' is not accessible outside of '{classSymbol.Name}'");
		}

		return null;
	}

	/// <summary>
	///     The name that the <c>NegatedParameterNameAttribute</c> gives the <paramref name="parameter" /> in the negated
	///     expectation, an empty string when the attribute names none, or <see langword="null" /> without the attribute.
	/// </summary>
	private static string? GetNegatedParameterName(IParameterSymbol parameter)
		=> parameter.GetAttributes()
			.FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == NegatedParameterNameAttribute) is { } attribute
			? attribute.ConstructorArguments.FirstOrDefault().Value as string ?? ""
			: null;

	private static string? GetNegatedParameterNameProblem(Signature signature)
	{
		foreach (IParameterSymbol parameter in signature.Method.Parameters)
		{
			if (GetNegatedParameterName(parameter) is not { } negatedName)
			{
				continue;
			}

			if (parameter.Ordinal == 0)
			{
				return $"the subject cannot be marked with [{NegatedParameterNameAttributeName}]";
			}

			if (!signature.ExpectationParameters.Contains(parameter, SymbolEqualityComparer.Default))
			{
				string marker = signature.MarkedOptions.Contains(parameter, SymbolEqualityComparer.Default)
					? OptionsAttributeName
					: TokenAttributeName;
				return
					$"the parameter '{parameter.Name}' is marked with [{marker}] and cannot be marked with [{NegatedParameterNameAttributeName}]";
			}

			if (!SyntaxFacts.IsValidIdentifier(negatedName))
			{
				return $"the negated parameter name '{negatedName}' is not a valid parameter name";
			}
		}

		List<string> negatedNames = signature.ExpectationParameters.Select(p => GetNegatedParameterName(p) ?? p.Name).ToList();
		return negatedNames.FirstOrDefault(n => n == "subject" || negatedNames.Count(x => x == n) > 1) is { } clash
			? $"the negated parameter name '{clash}' would clash with another parameter of the negated expectation"
			: null;
	}

	private static string? GetNameProblem(string name, string? explicitNegatedName, out string positiveName,
		out string? negatedName)
	{
		positiveName = name.Replace("{Not}", "");
		negatedName = explicitNegatedName ?? (name.Contains("{Not}") ? name.Replace("{Not}", "Not") : null);
		if (!SyntaxFacts.IsValidIdentifier(positiveName) ||
		    (explicitNegatedName is null && negatedName is not null && !SyntaxFacts.IsValidIdentifier(negatedName)))
		{
			return $"the name '{name}' is not a valid method name";
		}

		if (explicitNegatedName is not null &&
		    (!SyntaxFacts.IsValidIdentifier(explicitNegatedName) || explicitNegatedName == positiveName))
		{
			return $"the negated name '{explicitNegatedName}' is not a valid method name that differs from the name";
		}

		return null;
	}

	private static string? GetTextProblem(string expectationText, string? negatedExpectationText,
		List<string> parameterNames, out EquatableArray<TextPart> positiveText,
		out EquatableArray<TextPart> negatedText)
	{
		positiveText = default;
		negatedText = default;
		if (negatedExpectationText is null && !expectationText.Contains("{not}"))
		{
			return "the expectation text has no '{not}' and no negated expectation text is given";
		}

		if (!TryParseText(expectationText.Replace("{not}", "").Replace("  ", " ").Trim(), parameterNames,
			    out positiveText, out string? unknownPlaceholder) ||
		    !TryParseText(negatedExpectationText ?? expectationText.Replace("{not}", "not").Replace("  ", " ").Trim(),
			    parameterNames, out negatedText, out unknownPlaceholder))
		{
			return $"the placeholder '{{{unknownPlaceholder}}}' names no parameter of the expectation";
		}

		return null;
	}

	private static string? GetCollectionTextProblem(string expectationText, string pluralExpectationText,
		List<string> parameterNames, out EquatableArray<TextPart> text, out EquatableArray<TextPart> pluralText)
	{
		text = default;
		pluralText = default;
		if (expectationText.Contains("{not}") || pluralExpectationText.Contains("{not}"))
		{
			return "the expectation text contains '{not}', but the quantifier carries the negation";
		}

		if (!TryParseText(expectationText.Trim(), parameterNames, out text, out string? unknownPlaceholder) ||
		    !TryParseText(pluralExpectationText.Trim(), parameterNames, out pluralText, out unknownPlaceholder))
		{
			return $"the placeholder '{{{unknownPlaceholder}}}' names no parameter of the expectation";
		}

		return null;
	}

	private static string? GetNullItemFailsBothWaysProblem(bool nullItemFailsBothWays, IParameterSymbol subject,
		EquatableArray<ItemModel> items)
		=> nullItemFailsBothWays && LetsThePredicateDecideOnNull(subject, items)
			? "NullItemFailsBothWays requires an item parameter that is not nullable or is marked with [DisallowNull]"
			: null;

	private static string? GetCollectionNameProblem(string name, string? negatedName)
	{
		if (!SyntaxFacts.IsValidIdentifier(name))
		{
			return $"the name '{name}' is not a valid method name";
		}

		if (negatedName is not null && (!SyntaxFacts.IsValidIdentifier(negatedName) || negatedName == name))
		{
			return $"the negated name '{negatedName}' is not a valid method name that differs from the name";
		}

		return null;
	}

	/// <summary>
	///     The verb in the past tense that stands in for the expectation in the result, e.g. "were" in "but only 1 of 3
	///     were", which is "were" for a form of "be" and "did" for any other verb, as English requires do-support for it.
	/// </summary>
	private static string GetResultVerb(string expectationText)
	{
		string trimmed = expectationText.Trim();
		int space = trimmed.IndexOf(' ');
		return (space < 0 ? trimmed : trimmed.Substring(0, space)) switch
		{
			"" or "is" or "are" or "was" or "were" => "were",
			_ => "did",
		};
	}

	private static PredicateKind GetPredicateKind(ITypeSymbol returnType)
	{
		if (!IsTask(returnType))
		{
			return PredicateKind.Synchronous;
		}

		return returnType.Name == "Task" ? PredicateKind.Task : PredicateKind.ValueTask;
	}

	/// <remarks>
	///     The compiler requires every part of a partial class to be marked, so the part that declares the method is
	///     enough.
	/// </remarks>
	private static bool IsExtensionClass(INamedTypeSymbol classSymbol, SyntaxNode method)
		=> classSymbol is
		   {
			   TypeKind: TypeKind.Class, IsStatic: true, ContainingType: null, Arity: 0, IsFileLocal: false,
		   } &&
		   method.Parent is ClassDeclarationSyntax declaration &&
		   declaration.Modifiers.Any(SyntaxKind.PartialKeyword);

	private static string? GetMethodProblem(IMethodSymbol method, bool allowsItemTypeParameter)
	{
		if (!method.IsStatic)
		{
			return "it is not static";
		}

		if (method.IsGenericMethod && !(allowsItemTypeParameter && GetItemTypeParameter(method) is not null))
		{
			return allowsItemTypeParameter
				? "it is generic, but not only in the type of the item, which a reference type constraint restricts"
				: "it is generic";
		}

		if (GetResultType(method.ReturnType) is null)
		{
			return "it returns neither bool nor PredicateResult, nor a Task or ValueTask of them";
		}

		if (method.Parameters.Length == 0)
		{
			return "it has no parameter for the subject";
		}

		foreach (IParameterSymbol parameter in method.Parameters)
		{
			if (parameter.RefKind != RefKind.None)
			{
				return $"the parameter '{parameter.Name}' is passed by reference";
			}

			if (parameter.IsOptional)
			{
				return $"the parameter '{parameter.Name}' is optional";
			}
		}

		if (method.Parameters[0].IsParams)
		{
			return "the subject cannot be a params array";
		}

		return null;
	}

	/// <summary>
	///     The only type parameter of the <paramref name="method" />, when it is the type of the item and restricted to
	///     reference types, so that the generated code can name its nullable form, or <see langword="null" /> otherwise.
	/// </summary>
	private static ITypeParameterSymbol? GetItemTypeParameter(IMethodSymbol method)
	{
		if (method.TypeParameters.Length != 1 || method.Parameters.Length == 0 ||
		    method.Parameters[0].Type is not ITypeParameterSymbol itemType ||
		    !SymbolEqualityComparer.Default.Equals(itemType, method.TypeParameters[0]) ||
		    !itemType.IsReferenceType ||
		    itemType.Name is "TCollection" or "TEnumerable")
		{
			return null;
		}

		return method.Parameters.Skip(1).Any(p => ContainsTypeParameter(p.Type, itemType)) ||
		       itemType.ConstraintTypes.Any(t => ContainsTypeParameter(t, itemType)) ||
		       ContainsTypeParameter(method.ReturnType, itemType)
			? null
			: itemType;
	}

	private static bool ContainsTypeParameter(ITypeSymbol type, ITypeParameterSymbol typeParameter)
		=> type switch
		{
			ITypeParameterSymbol parameter => SymbolEqualityComparer.Default.Equals(parameter, typeParameter),
			IArrayTypeSymbol array => ContainsTypeParameter(array.ElementType, typeParameter),
			INamedTypeSymbol named => named.TypeArguments.Any(t => ContainsTypeParameter(t, typeParameter)),
			_ => false,
		};

	/// <summary>
	///     The constraints of the <paramref name="typeParameter" /> in a <c>where</c> clause, e.g.
	///     <c>global::System.Reflection.MemberInfo</c>.
	/// </summary>
	private static string GetConstraints(ITypeParameterSymbol typeParameter)
	{
		List<string> constraints = [];
		if (typeParameter.HasReferenceTypeConstraint)
		{
			constraints.Add("class");
		}

		if (typeParameter.HasNotNullConstraint)
		{
			constraints.Add("notnull");
		}

		constraints.AddRange(typeParameter.ConstraintTypes.Select(t
			=> t.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(TypeFormat)));
		if (typeParameter.HasConstructorConstraint)
		{
			constraints.Add("new()");
		}

		return string.Join(", ", constraints);
	}

	/// <summary>
	///     The accessibility of the <paramref name="type" />, limited by its containing types and its type arguments.
	/// </summary>
	private static Accessibility GetAccessibility(ITypeSymbol type)
		=> type switch
		{
			IArrayTypeSymbol array => GetAccessibility(array.ElementType),
			ITypeParameterSymbol typeParameter => typeParameter.ConstraintTypes.Select(GetAccessibility)
				.Append(Accessibility.Public)
				.Min(),
			INamedTypeSymbol { TypeKind: not TypeKind.Error, } named => named.TypeArguments.Select(GetAccessibility)
				.Append(named.DeclaredAccessibility)
				.Append(named.ContainingType is { } containingType
					? GetAccessibility(containingType)
					: Accessibility.Public)
				.Min(),
			_ => Accessibility.Public,
		};

	/// <summary>
	///     The <see langword="bool" /> or <c>PredicateResult</c> that the method returns, directly or as the result of a
	///     <c>Task</c> or <c>ValueTask</c>, or <see langword="null" /> for any other return type.
	/// </summary>
	private static ITypeSymbol? GetResultType(ITypeSymbol returnType)
	{
		ITypeSymbol type = IsTask(returnType) ? ((INamedTypeSymbol)returnType).TypeArguments[0] : returnType;
		return type.SpecialType == SpecialType.System_Boolean || IsPredicateResult(type) ? type : null;
	}

	private static bool IsTask(ITypeSymbol type)
		=> type is INamedTypeSymbol { Arity: 1, ContainingNamespace: var ns, } named &&
		   named.Name is "Task" or "ValueTask" &&
		   ns.ToDisplayString() == "System.Threading.Tasks";

	private static bool IsPredicateResult(ITypeSymbol? type)
		=> type is { Name: "PredicateResult", ContainingNamespace: var ns, } &&
		   ns.ToDisplayString() == "aweXpect.Core.Extending";

	private static bool TryParseText(string text, List<string> parameterNames, out EquatableArray<TextPart> parts,
		out string? unknownPlaceholder)
	{
		List<TextPart> result = [];
		StringBuilder literal = new();
		unknownPlaceholder = null;
		int index = 0;
		while (index < text.Length)
		{
			int end = text[index] == '{' ? text.IndexOf('}', index) : -1;
			if (end > index + 1)
			{
				string placeholder = text.Substring(index + 1, end - index - 1);
				int parameterIndex = parameterNames.IndexOf(placeholder);
				if (parameterIndex < 0)
				{
					unknownPlaceholder = placeholder;
					parts = default;
					return false;
				}

				if (literal.Length > 0)
				{
					result.Add(new TextPart(literal.ToString(), -1));
					literal.Clear();
				}

				result.Add(new TextPart(null, parameterIndex));
				index = end + 1;
			}
			else
			{
				literal.Append(text[index++]);
			}
		}

		if (literal.Length > 0)
		{
			result.Add(new TextPart(literal.ToString(), -1));
		}

		parts = new EquatableArray<TextPart>(result.ToImmutableArray());
		return true;
	}

	private static bool TryFindResultMethod(INamedTypeSymbol classSymbol, string name, IParameterSymbol subject,
		List<IParameterSymbol> expectationParameters, out ResultModel? result)
	{
		foreach (IMethodSymbol candidate in classSymbol.GetMembers(name).OfType<IMethodSymbol>())
		{
			if (!candidate.IsStatic || candidate.IsGenericMethod ||
			    candidate.ReturnType.SpecialType != SpecialType.System_String ||
			    candidate.Parameters.Any(p => p.RefKind != RefKind.None) ||
			    candidate.Parameters.Length == 0 ||
			    !SymbolEqualityComparer.Default.Equals(candidate.Parameters[0].Type, subject.Type))
			{
				continue;
			}

			List<IParameterSymbol> rest = candidate.Parameters.Skip(1).ToList();
			bool takesItAndGrammars = rest.Count >= 2 &&
			                          rest[rest.Count - 2].Type.SpecialType == SpecialType.System_String &&
			                          IsExpectationGrammars(rest[rest.Count - 1].Type);
			if (takesItAndGrammars)
			{
				rest.RemoveRange(rest.Count - 2, 2);
			}

			if (rest.Count == 0 || (rest.Count == expectationParameters.Count &&
			                        rest.Zip(expectationParameters,
				                        (a, b) => SymbolEqualityComparer.Default.Equals(a.Type, b.Type)).All(x => x)))
			{
				result = new ResultModel(Escape(name), rest.Count > 0, takesItAndGrammars);
				return true;
			}
		}

		result = null;
		return false;
	}

	private static bool IsExpectationGrammars(ITypeSymbol type)
		=> type is { Name: "ExpectationGrammars", ContainingNamespace: var ns, } &&
		   ns.ToDisplayString() == "aweXpect.Core";

	private static EquatableArray<OverloadModel> GetOverloads(IParameterSymbol subject, string constraintPrefix,
		bool nullableOverload)
	{
		ITypeSymbol type = subject.Type;
		bool disallowNull = subject.GetAttributes()
			.Any(a => IsCodeAnalysisAttribute(a, "DisallowNullAttribute"));
		bool? notNullWhen = subject.GetAttributes()
			.Where(a => IsCodeAnalysisAttribute(a, "NotNullWhenAttribute"))
			.Select(a => a.ConstructorArguments.FirstOrDefault().Value as bool?)
			.FirstOrDefault();
		string constraintName = $"{constraintPrefix}_GeneratedConstraint";
		string display = type.ToDisplayString(TypeFormat);

		if (type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
		{
			string underlying = ((INamedTypeSymbol)type).TypeArguments[0].ToDisplayString(TypeFormat);
			return disallowNull
				? Single(new OverloadModel(display, constraintName, true, underlying, underlying, true, true,
					"actual", "Actual"))
				: Single(Decided(display, underlying));
		}

		if (type.IsValueType)
		{
			OverloadModel value = new(display, constraintName, false, display, display, false, false,
				"actual", "Actual");
			if (!nullableOverload)
			{
				return Single(value);
			}

			return new EquatableArray<OverloadModel>(ImmutableArray.Create(
				value,
				new OverloadModel(display + "?", $"{constraintPrefix}_GeneratedNullableConstraint", true, display,
					display, true, true, "actual.Value", "Actual.GetValueOrDefault()")));
		}

		string notNull = type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(TypeFormat);
		if (type.NullableAnnotation == NullableAnnotation.Annotated && !disallowNull)
		{
			return Single(Decided(display, notNull));
		}

		return Single(new OverloadModel(notNull + "?", constraintName, true, notNull, notNull, true, true,
			"actual", "Actual!"));

		OverloadModel Decided(string thatType, string notNullType)
			=> new(thatType, constraintName, false,
				notNullWhen == true ? notNullType : thatType,
				notNullWhen == false ? notNullType : thatType,
				notNullWhen == true,
				notNullWhen == false,
				"actual",
				"Actual");

		static EquatableArray<OverloadModel> Single(OverloadModel overload)
			=> new(ImmutableArray.Create(overload));
	}

	/// <summary>
	///     The item types whose collections the expectation extends, following the rule for <see langword="null" />
	///     subjects of <see cref="GetOverloads" /> for the items.
	/// </summary>
	private static EquatableArray<ItemModel> GetItems(IParameterSymbol subject, string constraintPrefix,
		bool nullableOverload)
	{
		ITypeSymbol type = subject.Type;
		bool disallowNull = subject.GetAttributes()
			.Any(a => IsCodeAnalysisAttribute(a, "DisallowNullAttribute"));
		string generatedPrefix = $"{constraintPrefix}_Generated";
		string display = type.ToDisplayString(TypeFormat);

		if (type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
		{
			return Items(new ItemModel(display, generatedPrefix, disallowNull,
				disallowNull ? "item.Value" : "item"));
		}

		if (type.IsValueType)
		{
			ItemModel value = new(display, generatedPrefix, false, "item");
			return nullableOverload
				? Items(value, new ItemModel(display + "?", generatedPrefix + "Nullable", true, "item.Value"))
				: Items(value);
		}

		string nullable = type.WithNullableAnnotation(NullableAnnotation.Annotated).ToDisplayString(TypeFormat);
		bool failsOnNull = type.NullableAnnotation != NullableAnnotation.Annotated || disallowNull;
		return Items(new ItemModel(nullable, generatedPrefix, failsOnNull, "item"));

		static EquatableArray<ItemModel> Items(params ItemModel[] items)
			=> new(items.ToImmutableArray());
	}

	/// <summary>
	///     Whether the predicate receives a <see langword="null" /> item, as its item parameter is nullable.
	/// </summary>
	private static bool LetsThePredicateDecideOnNull(IParameterSymbol subject, EquatableArray<ItemModel> items)
		=> !items.Values[0].FailsOnNull &&
		   (!subject.Type.IsValueType || subject.Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T);

	private static bool IsCodeAnalysisAttribute(AttributeData attribute, string name)
		=> attribute.AttributeClass is { Name: var attributeName, ContainingNamespace: var ns, } &&
		   attributeName == name &&
		   ns.ToDisplayString() == "System.Diagnostics.CodeAnalysis";

	private static T? GetNamedArgument<T>(AttributeData attribute, string name)
	{
		foreach (KeyValuePair<string, TypedConstant> argument in attribute.NamedArguments)
		{
			if (argument.Key == name && argument.Value.Value is T value)
			{
				return value;
			}
		}

		return default;
	}

	private static string Escape(string name)
		=> SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ? "@" + name : name;

	/// <summary>
	///     The marked method, split into the subject, the parameters of the expectation, the cancellation token and the
	///     options.
	/// </summary>
	private sealed class Signature
	{
		public Signature(IMethodSymbol method, Location location, INamedTypeSymbol classSymbol, string methodName)
		{
			Method = method;
			Location = location;
			Class = classSymbol;
			MethodName = methodName;
			ImmutableArray<IParameterSymbol> parameters = method.Parameters;
			MarkedTokens = parameters
				.Where(p => p.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == TokenAttribute))
				.ToList();
			IParameterSymbol? token = MarkedTokens.Count == 1 && MarkedTokens[0].Ordinal > 0 ? MarkedTokens[0] : null;
			CancellationTokenIndex = token is null ? -1 : token.Ordinal - 1;
			MarkedOptions = parameters
				.Where(p => p.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == OptionsAttribute))
				.ToList();
			OptionsParameter = MarkedOptions.Count == 1 && MarkedOptions[0].Ordinal > 0 ? MarkedOptions[0] : null;
			ExpectationParameters = parameters
				.Skip(1)
				.Where(p => !SymbolEqualityComparer.Default.Equals(p, token) &&
				            !SymbolEqualityComparer.Default.Equals(p, OptionsParameter))
				.ToList();
			SignatureParameters = parameters.Take(1).Concat(ExpectationParameters).ToList();
			int overloadIndex = classSymbol.GetMembers(method.Name).OfType<IMethodSymbol>().ToList()
				.FindIndex(m => SymbolEqualityComparer.Default.Equals(m, method));
			ConstraintPrefix = overloadIndex > 0 ? $"{method.Name}_{overloadIndex}" : method.Name;
		}

		public IMethodSymbol Method { get; }
		public Location Location { get; }
		public INamedTypeSymbol Class { get; }
		public string MethodName { get; }
		/// <summary>
		///     The parameters marked with the <c>EvaluationCancellationTokenAttribute</c>.
		/// </summary>
		public List<IParameterSymbol> MarkedTokens { get; }

		/// <summary>
		///     The position of the evaluation cancellation token among the arguments after the subject, or <c>-1</c>
		///     without one.
		/// </summary>
		public int CancellationTokenIndex { get; }

		public bool IsAsync => IsTask(Method.ReturnType);
		public bool ReturnsPredicateResult => IsPredicateResult(GetResultType(Method.ReturnType));
		public IParameterSymbol Subject => Method.Parameters[0];

		/// <summary>
		///     The parameters marked with the <c>ExpectationOptionsAttribute</c>.
		/// </summary>
		public List<IParameterSymbol> MarkedOptions { get; }

		/// <summary>
		///     The parameter that receives the options, which the generated expectation creates, if any.
		/// </summary>
		public IParameterSymbol? OptionsParameter { get; }

		/// <summary>
		///     The parameters after the subject, without the cancellation token and the options.
		/// </summary>
		public List<IParameterSymbol> ExpectationParameters { get; }

		/// <summary>
		///     The subject and the <see cref="ExpectationParameters" />, whose types appear in the generated signatures.
		/// </summary>
		public List<IParameterSymbol> SignatureParameters { get; }

		public List<string> GetParameterNames() => ExpectationParameters.Select(p => p.Name).ToList();

		/// <summary>
		///     The prefix of the generated constraints, which tells overloads of the method apart.
		/// </summary>
		public string ConstraintPrefix { get; }

		public ExpectationResult Fail(string reason)
			=> new(null, new Problem(InvalidMethod, Location, MethodName, reason));

		public CollectionExpectationResult FailCollection(string reason)
			=> new(null, new Problem(InvalidMethod, Location, MethodName, reason));

		public ClassModel ToClassModel(GeneratorAttributeSyntaxContext context, string hintSuffix)
			=> new(
				new EquatableArray<string>(((CompilationUnitSyntax)context.TargetNode.SyntaxTree.GetRoot()).Usings
					.Where(u => u.GlobalKeyword.IsKind(SyntaxKind.None))
					.Select(u => u.ToString())
					.ToImmutableArray()),
				new EquatableArray<string>(context.TargetNode.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
					.Reverse()
					.SelectMany(n => n.Usings)
					.Select(u => u.ToString())
					.ToImmutableArray()),
				Class.ContainingNamespace.IsGlobalNamespace
					? null
					: Class.ContainingNamespace.ToDisplayString(NamespaceFormat),
				Escape(Class.Name),
				SyntaxFacts.GetText(Class.DeclaredAccessibility),
				GetMethodAccessibility(Class, SignatureParameters),
				$"{Class.ToDisplayString()}.{ConstraintPrefix}{hintSuffix}.g.cs",
				Escape(Method.Name),
				new EquatableArray<ParameterModel>(ExpectationParameters
					.Select(p => new ParameterModel(Escape(p.Name), Escape(GetNegatedParameterName(p) ?? p.Name),
						p.Type.ToDisplayString(TypeFormat), p.IsParams,
						p.Type.IsReferenceType && p.NullableAnnotation == NullableAnnotation.NotAnnotated))
					.ToImmutableArray()));

		/// <remarks>
		///     The extension methods are public, unless the class or a type in their signature is only internal, which a
		///     public method must not expose.
		/// </remarks>
		private static string GetMethodAccessibility(INamedTypeSymbol classSymbol,
			IEnumerable<IParameterSymbol> parameters)
			=> classSymbol.DeclaredAccessibility == Accessibility.Public &&
			   parameters.All(p => GetAccessibility(p.Type) == Accessibility.Public)
				? "public"
				: "internal";
	}
}
