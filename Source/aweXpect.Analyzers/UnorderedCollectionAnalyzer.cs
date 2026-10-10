using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace aweXpect.Analyzers;

/// <summary>
///     An analyzer that checks that a set or a dictionary, whose enumeration order is an implementation detail, is not
///     compared by position.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnorderedCollectionAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	///     The diagnostic property that is set when appending <c>.InAnyOrder()</c> fixes the expectation.
	/// </summary>
	internal const string InAnyOrderProperty = "InAnyOrder";

	private const string IgnoringInterspersedItems = nameof(IgnoringInterspersedItems);
	private const string InAnyOrder = nameof(InAnyOrder);

	private static readonly ImmutableHashSet<string> PositionalExpectations = ImmutableHashSet.Create(
		"IsEqualTo", "IsNotEqualTo", "Contains", "DoesNotContain", "IsContainedIn", "IsNotContainedIn");

	private static readonly ImmutableHashSet<string> OrderExpectations = ImmutableHashSet.Create(
		"StartsWith", "DoesNotStartWith", "EndsWith", "DoesNotEndWith");

	private static readonly ImmutableHashSet<string> UnorderedInterfaces = ImmutableHashSet.Create(
		"System.Collections.Generic.ISet<T>",
		"System.Collections.Generic.IReadOnlySet<T>",
		"System.Collections.Generic.IDictionary<TKey, TValue>",
		"System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>");

	private static readonly ImmutableHashSet<string> UnorderedTypes = ImmutableHashSet.Create(
		"System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection",
		"System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection");

	private static readonly ImmutableHashSet<string> SortedTypes = ImmutableHashSet.Create(
		"System.Collections.Generic.SortedSet<T>",
		"System.Collections.Generic.SortedDictionary<TKey, TValue>",
		"System.Collections.Generic.SortedList<TKey, TValue>",
		"System.Collections.Generic.OrderedDictionary<TKey, TValue>",
		"System.Collections.Immutable.ImmutableSortedSet<T>",
		"System.Collections.Immutable.ImmutableSortedDictionary<TKey, TValue>");

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(Rules.UnorderedCollectionRule, Rules.UnorderedCollectionNoMeaningRule);

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterOperationAction(AnalyzeOperation, OperationKind.Invocation);
	}

	private static void AnalyzeOperation(OperationAnalysisContext context)
	{
		if (context.Operation is not IInvocationOperation invocation)
		{
			return;
		}

		IMethodSymbol method = invocation.TargetMethod;
		if (method.Name == IgnoringInterspersedItems)
		{
			if (method.IsExtensionMethod && IsInNamespace(method.ContainingType, "aweXpect") &&
			    GetExpectation(invocation) is { } expectation &&
			    GetUnorderedSubjectType(expectation) is { } type)
			{
				context.ReportDiagnostic(Diagnostic.Create(Rules.UnorderedCollectionNoMeaningRule,
					GetLocation(invocation), method.Name, type));
			}

			return;
		}

		if (!method.IsExtensionMethod || !IsInNamespace(method.ContainingType, "aweXpect"))
		{
			return;
		}

		if (PositionalExpectations.Contains(method.Name) && ProvidesCollectionMatchOptions(method.ReturnType))
		{
			if (!HasInAnyOrder(invocation) &&
			    (GetUnorderedSubjectType(invocation) ?? GetUnorderedExpectedType(invocation)) is { } type)
			{
				context.ReportDiagnostic(Diagnostic.Create(Rules.UnorderedCollectionRule,
					GetLocation(invocation),
					ImmutableDictionary<string, string?>.Empty.Add(InAnyOrderProperty, null),
					type));
			}
		}
		else if (OrderExpectations.Contains(method.Name) && GetUnorderedSubjectType(invocation) is { } type)
		{
			context.ReportDiagnostic(Diagnostic.Create(Rules.UnorderedCollectionNoMeaningRule,
				GetLocation(invocation), method.Name, type));
		}
	}

	private static bool IsInNamespace(ITypeSymbol? type, params string[] namespaces)
	{
		INamespaceSymbol? current = type?.ContainingNamespace;
		for (int i = namespaces.Length - 1; i >= 0; i--)
		{
			if (current is null || current.Name != namespaces[i])
			{
				return false;
			}

			current = current.ContainingNamespace;
		}

		return current?.IsGlobalNamespace == true;
	}

	/// <summary>
	///     Whether the result <paramref name="type" /> offers <c>InAnyOrder()</c>, which is an extension method on every
	///     result that provides the <c>CollectionMatchOptions</c>.
	/// </summary>
	private static bool ProvidesCollectionMatchOptions(ITypeSymbol? type)
		=> type?.AllInterfaces.Any(i
			=> i.OriginalDefinition.ToDisplayString() == "aweXpect.Core.IOptionsProvider<TOptions>" &&
			   i.TypeArguments[0].ToDisplayString() == "aweXpect.Options.CollectionMatchOptions") == true;

	/// <summary>
	///     Whether <c>InAnyOrder()</c> follows among the options that are chained on the result of the
	///     <paramref name="expectation" />.
	/// </summary>
	private static bool HasInAnyOrder(IInvocationOperation expectation)
	{
		IOperation current = expectation;
		while (GetChainedOption(current) is { } option)
		{
			if (option.TargetMethod.Name == InAnyOrder)
			{
				return true;
			}

			current = option;
		}

		return false;
	}

	/// <summary>
	///     The option that is chained on the <paramref name="result" />, either as an instance method or as an
	///     extension method.
	/// </summary>
	private static IInvocationOperation? GetChainedOption(IOperation result)
	{
		if (result.Parent is IInvocationOperation option && option.Instance == result)
		{
			return option;
		}

		IOperation? parent = result.Parent;
		while (parent is IConversionOperation)
		{
			parent = parent.Parent;
		}

		return parent is IArgumentOperation
		{
			Parameter.Ordinal: 0, Parent: IInvocationOperation { TargetMethod.IsExtensionMethod: true, } extension,
		}
			? extension
			: null;
	}

	/// <summary>
	///     The expectation on whose result the <paramref name="option" /> is chained, either directly or behind other
	///     instance or extension method options: the first invocation whose receiver is an <c>IThat&lt;T&gt;</c>.
	/// </summary>
	private static IInvocationOperation? GetExpectation(IOperation option)
	{
		IOperation? current = option;
		while (current is IInvocationOperation invocation)
		{
			IOperation? receiver = invocation.Instance ??
			                       (invocation.TargetMethod.IsExtensionMethod ? GetReceiver(invocation) : null);
			if (GetThatTypeArgument(receiver?.Type) is not null)
			{
				return invocation;
			}

			current = receiver;
		}

		return null;
	}

	/// <summary>
	///     The display string of the static subject type of the <paramref name="expectation" />, if it has no defined
	///     order. <c>.And</c> and <c>.Or</c> are followed back to the previous expectation, because they continue with
	///     the same subject, but usually with its type as declared by that expectation.
	/// </summary>
	private static string? GetUnorderedSubjectType(IInvocationOperation expectation)
	{
		IOperation? receiver = GetReceiver(expectation);
		while (receiver is IPropertyReferenceOperation { Property.Name: "And" or "Or", Instance: { } result, } &&
		       GetExpectation(result) is { TargetMethod.IsExtensionMethod: true, } previous)
		{
			receiver = GetReceiver(previous);
		}

		return GetUnorderedDisplayString(GetThatTypeArgument(receiver?.Type));
	}

	/// <summary>
	///     The display string of the static type of the collection that the <paramref name="expectation" /> expects, if
	///     it has no defined order.
	/// </summary>
	private static string? GetUnorderedExpectedType(IInvocationOperation expectation)
	{
		IOperation? expected = expectation.Arguments.FirstOrDefault(argument => argument.Parameter?.Ordinal == 1)?.Value;
		while (expected is IConversionOperation conversion)
		{
			expected = conversion.Operand;
		}

		return GetUnorderedDisplayString(expected?.Type);
	}

	private static string? GetUnorderedDisplayString(ITypeSymbol? type)
		=> type is not null && IsUnordered(type)
			? type.WithNullableAnnotation(NullableAnnotation.NotAnnotated)
				.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
			: null;

	/// <summary>
	///     The receiver of an extension method invocation, without the implicit conversion to the
	///     <c>this</c> parameter type.
	/// </summary>
	private static IOperation? GetReceiver(IInvocationOperation invocation)
	{
		IOperation? receiver = invocation.Arguments.FirstOrDefault(argument => argument.Parameter?.Ordinal == 0)?.Value;
		while (receiver is IConversionOperation conversion)
		{
			receiver = conversion.Operand;
		}

		return receiver;
	}

	private static ITypeSymbol? GetThatTypeArgument(ITypeSymbol? type)
	{
		if (type is null)
		{
			return null;
		}

		INamedTypeSymbol? that = type is INamedTypeSymbol named && IsThat(named)
			? named
			: type.AllInterfaces.FirstOrDefault(IsThat);
		return that?.TypeArguments[0];
	}

	private static bool IsThat(INamedTypeSymbol type)
		=> type is { Name: "IThat", Arity: 1, } && IsInNamespace(type, "aweXpect", "Core");

	private static bool IsUnordered(ITypeSymbol type)
	{
		for (ITypeSymbol? current = type; current != null; current = current.BaseType)
		{
			if (SortedTypes.Contains(current.OriginalDefinition.ToDisplayString()))
			{
				return false;
			}
		}

		string definition = type.OriginalDefinition.ToDisplayString();
		return UnorderedTypes.Contains(definition) ||
		       UnorderedInterfaces.Contains(definition) ||
		       type.AllInterfaces.Any(i => UnorderedInterfaces.Contains(i.OriginalDefinition.ToDisplayString()));
	}

	/// <summary>
	///     Reports on the method name when it is accessed as a member, so that the code fix can append to the
	///     invocation; otherwise on the whole invocation.
	/// </summary>
	private static Location GetLocation(IInvocationOperation invocation)
		=> invocation.Syntax is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax memberAccess, }
			? memberAccess.Name.GetLocation()
			: invocation.Syntax.GetLocation();
}
