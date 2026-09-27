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

	private static readonly ImmutableHashSet<string> SortedTypes = ImmutableHashSet.Create(
		"System.Collections.Generic.SortedSet<T>",
		"System.Collections.Generic.SortedDictionary<TKey, TValue>",
		"System.Collections.Immutable.ImmutableSortedSet<T>",
		"System.Collections.Immutable.ImmutableSortedDictionary<TKey, TValue>");

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		[Rules.UnorderedCollectionRule, Rules.UnorderedCollectionNoMeaningRule,];

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
			if (IsInNamespace(method.ContainingType, "aweXpect", "Results") &&
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

		if (PositionalExpectations.Contains(method.Name) && HasParameterlessMethod(method.ReturnType, InAnyOrder))
		{
			if (!HasInAnyOrder(invocation) && GetUnorderedSubjectType(invocation) is { } type)
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
			if (current?.Name != namespaces[i])
			{
				return false;
			}

			current = current.ContainingNamespace;
		}

		return current?.IsGlobalNamespace == true;
	}

	private static bool HasParameterlessMethod(ITypeSymbol? type, string name)
	{
		for (ITypeSymbol? current = type; current != null; current = current.BaseType)
		{
			if (current.GetMembers(name).OfType<IMethodSymbol>().Any(m => m.Parameters.Length == 0))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Whether <c>InAnyOrder()</c> follows among the options that are chained on the result of the
	///     <paramref name="expectation" />.
	/// </summary>
	private static bool HasInAnyOrder(IInvocationOperation expectation)
	{
		IOperation current = expectation;
		while (current.Parent is IInvocationOperation option && option.Instance == current)
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
	///     The expectation on whose result the <paramref name="option" /> is chained.
	/// </summary>
	private static IInvocationOperation? GetExpectation(IOperation option)
	{
		IOperation? current = option;
		while (current is IInvocationOperation { Instance: { } instance, })
		{
			current = instance;
		}

		return current as IInvocationOperation;
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

		ITypeSymbol? subjectType = GetThatTypeArgument(receiver?.Type);
		return subjectType is not null && IsUnordered(subjectType)
			? subjectType.WithNullableAnnotation(NullableAnnotation.NotAnnotated)
				.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
			: null;
	}

	/// <summary>
	///     The receiver of an extension method invocation, without the implicit conversion to the
	///     <c>this</c> parameter type.
	/// </summary>
	private static IOperation? GetReceiver(IInvocationOperation invocation)
	{
		IOperation? receiver = invocation.Arguments.FirstOrDefault()?.Value;
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

		return UnorderedInterfaces.Contains(type.OriginalDefinition.ToDisplayString()) ||
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
