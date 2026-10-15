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
	private const string HelpLink = "https://docs.testably.org/aweXpect/analyzers#expectation-generator";

	private static readonly DiagnosticDescriptor InvalidClass = new(
		"aweXpect4001",
		"The class cannot hold the generated expectation",
		"'{0}' cannot hold the generated expectation, because extension methods need a top-level, non-generic, static partial class",
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
		"'{0}' names no static method that returns a string and takes the subject, optionally followed by the other parameters of '{1}'",
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
	}

	private static ExpectationResult GetExpectation(GeneratorAttributeSyntaxContext context)
	{
		IMethodSymbol method = (IMethodSymbol)context.TargetSymbol;
		Location location = ((MethodDeclarationSyntax)context.TargetNode).Identifier.GetLocation();
		INamedTypeSymbol classSymbol = method.ContainingType;
		if (!IsExtensionClass(classSymbol, context.TargetNode))
		{
			return Fail(InvalidClass, location, classSymbol.ToDisplayString());
		}

		string methodName = method.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);
		AttributeData attribute = context.Attributes[0];
		if (attribute.ConstructorArguments.Length != 2 ||
		    attribute.ConstructorArguments[0].Value is not string name ||
		    attribute.ConstructorArguments[1].Value is not string expectationText)
		{
			return Fail(InvalidMethod, location, methodName, "the name and the expectation text are required");
		}

		if (GetMethodProblem(method) is { } methodProblem)
		{
			return Fail(InvalidMethod, location, methodName, methodProblem);
		}

		bool isAsync = method.ReturnType.SpecialType != SpecialType.System_Boolean;
		ImmutableArray<IParameterSymbol> parameters = method.Parameters;
		bool passesCancellationToken = isAsync && parameters.Length > 1 &&
		                               IsCancellationToken(parameters[parameters.Length - 1].Type);
		IParameterSymbol subject = parameters[0];
		List<IParameterSymbol> expectationParameters = parameters
			.Skip(1)
			.Take(parameters.Length - (passesCancellationToken ? 2 : 1))
			.ToList();

		if (expectationParameters.FirstOrDefault(p => p.Name == "subject") is { } clash)
		{
			return Fail(InvalidMethod, location, methodName,
				$"the parameter '{clash.Name}' would clash with the subject of the expectation");
		}

		if (GetNameProblem(name, GetNamedArgument<string>(attribute, "NegatedName"),
			    out string positiveName, out string? negatedName) is { } nameProblem)
		{
			return Fail(InvalidMethod, location, methodName, nameProblem);
		}

		string? negatedExpectationText = GetNamedArgument<string>(attribute, "NegatedExpectationText");
		if (negatedExpectationText is null && !expectationText.Contains("{not}"))
		{
			return Fail(InvalidMethod, location, methodName,
				"the expectation text has no '{not}' and no negated expectation text is given");
		}

		if (!TryParseTexts(expectationText, negatedExpectationText, expectationParameters.Select(p => p.Name).ToList(),
			    out EquatableArray<TextPart> positiveText, out EquatableArray<TextPart> negatedText,
			    out string? unknownPlaceholder))
		{
			return Fail(InvalidMethod, location, methodName,
				$"the placeholder '{{{unknownPlaceholder}}}' names no parameter of the expectation");
		}

		string? resultMethod = GetNamedArgument<string>(attribute, "Result");
		bool resultTakesParameters = false;
		if (resultMethod is not null &&
		    !TryFindResultMethod(classSymbol, resultMethod, subject, expectationParameters,
			    out resultTakesParameters))
		{
			return Fail(InvalidResult, location, resultMethod, methodName);
		}

		int overloadIndex = classSymbol.GetMembers(method.Name).OfType<IMethodSymbol>().ToList()
			.FindIndex(m => SymbolEqualityComparer.Default.Equals(m, method));
		string constraintPrefix = overloadIndex > 0 ? $"{method.Name}_{overloadIndex}" : method.Name;
		string? ns = classSymbol.ContainingNamespace.IsGlobalNamespace
			? null
			: classSymbol.ContainingNamespace.ToDisplayString(NamespaceFormat);

		return new ExpectationResult(new ExpectationModel(
			new EquatableArray<string>(((CompilationUnitSyntax)context.TargetNode.SyntaxTree.GetRoot()).Usings
				.Where(u => u.GlobalKeyword.IsKind(SyntaxKind.None))
				.Select(u => u.ToString())
				.ToImmutableArray()),
			new EquatableArray<string>(context.TargetNode.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
				.Reverse()
				.SelectMany(n => n.Usings)
				.Select(u => u.ToString())
				.ToImmutableArray()),
			ns,
			Escape(classSymbol.Name),
			SyntaxFacts.GetText(classSymbol.DeclaredAccessibility),
			$"{classSymbol.ToDisplayString()}.{constraintPrefix}.g.cs",
			Escape(method.Name),
			positiveName,
			negatedName,
			positiveText,
			negatedText,
			GetNamedArgument<string>(attribute, "Summary"),
			GetNamedArgument<string>(attribute, "NegatedSummary"),
			GetNamedArgument<string>(attribute, "Remarks"),
			GetNamedArgument<string>(attribute, "NegatedRemarks"),
			isAsync,
			passesCancellationToken,
			new EquatableArray<ParameterModel>(expectationParameters
				.Select(p => new ParameterModel(Escape(p.Name), p.Type.ToDisplayString(TypeFormat), p.IsParams,
					p.Type.IsReferenceType && p.NullableAnnotation == NullableAnnotation.NotAnnotated))
				.ToImmutableArray()),
			resultMethod is null ? null : Escape(resultMethod),
			resultTakesParameters,
			GetOverloads(subject, constraintPrefix,
				GetNamedArgument<bool?>(attribute, "NullableOverload") ?? true)), null);
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

	private static bool TryParseTexts(string expectationText, string? negatedExpectationText,
		List<string> parameterNames, out EquatableArray<TextPart> positiveText,
		out EquatableArray<TextPart> negatedText, out string? unknownPlaceholder)
	{
		negatedText = default;
		return TryParseText(expectationText.Replace("{not}", "").Replace("  ", " ").Trim(), parameterNames,
			       out positiveText, out unknownPlaceholder) &&
		       TryParseText(negatedExpectationText ?? expectationText.Replace("{not}", "not").Replace("  ", " ").Trim(),
			       parameterNames, out negatedText, out unknownPlaceholder);
	}

	private static ExpectationResult Fail(DiagnosticDescriptor descriptor, Location location, string subject,
		string reason = "")
		=> new(null, new Problem(descriptor, location, subject, reason));

	/// <remarks>
	///     The compiler requires every part of a partial class to be marked, so the part that declares the method is
	///     enough.
	/// </remarks>
	private static bool IsExtensionClass(INamedTypeSymbol classSymbol, SyntaxNode method)
		=> classSymbol is { TypeKind: TypeKind.Class, IsStatic: true, ContainingType: null, Arity: 0, } &&
		   method.Parent is ClassDeclarationSyntax declaration &&
		   declaration.Modifiers.Any(SyntaxKind.PartialKeyword);

	private static string? GetMethodProblem(IMethodSymbol method)
	{
		if (!method.IsStatic)
		{
			return "it is not static";
		}

		if (method.IsGenericMethod)
		{
			return "it is generic";
		}

		if (method.ReturnType.SpecialType != SpecialType.System_Boolean && !IsBooleanTask(method.ReturnType))
		{
			return "it returns neither bool nor Task<bool> nor ValueTask<bool>";
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

	private static bool IsBooleanTask(ITypeSymbol type)
		=> type is INamedTypeSymbol { Arity: 1, ContainingNamespace: var ns, } named &&
		   named.Name is "Task" or "ValueTask" &&
		   ns.ToDisplayString() == "System.Threading.Tasks" &&
		   named.TypeArguments[0].SpecialType == SpecialType.System_Boolean;

	private static bool IsCancellationToken(ITypeSymbol type)
		=> type is { Name: "CancellationToken", ContainingNamespace: var ns, } &&
		   ns.ToDisplayString() == "System.Threading";

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
		List<IParameterSymbol> expectationParameters, out bool takesParameters)
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

			if (candidate.Parameters.Length == 1)
			{
				takesParameters = false;
				return true;
			}

			if (candidate.Parameters.Length == expectationParameters.Count + 1 &&
			    candidate.Parameters.Skip(1).Zip(expectationParameters,
				    (a, b) => SymbolEqualityComparer.Default.Equals(a.Type, b.Type)).All(x => x))
			{
				takesParameters = true;
				return true;
			}
		}

		takesParameters = false;
		return false;
	}

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
}
