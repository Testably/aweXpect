using System.Collections.Immutable;
using System.Linq;
using aweXpect.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace aweXpect.Analyzers;

/// <summary>
///     An analyzer that checks that the value of an expectation is not used when an <c>Or</c> combines it with an
///     alternative for another subject type: when that alternative is the one that is met, there is no value of the
///     resulting type and the expectation returns its <see langword="default" />.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class OrResultValueAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [Rules.OrResultValueRule,];

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterOperationAction(AnalyzeAwait, OperationKind.Await);
		context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
	}

	private static void AnalyzeAwait(OperationAnalysisContext context)
	{
		if (context.Operation is IAwaitOperation awaitOperation)
		{
			Analyze(context, awaitOperation, awaitOperation.Operation);
		}
	}

	private static void AnalyzeInvocation(OperationAnalysisContext context)
	{
		if (context.Operation is IInvocationOperation invocation &&
		    GetSynchronouslyVerifiedExpectation(invocation) is { } expectation)
		{
			Analyze(context, invocation, expectation);
		}
	}

	private static void Analyze(OperationAnalysisContext context, IOperation value, IOperation expectation)
	{
		if (value.Type is not { SpecialType: not SpecialType.System_Void, TypeKind: not TypeKind.Error, } valueType ||
		    !IsExpectationResult(expectation.Type) ||
		    !IsUsed(value) ||
		    FindOrOnAnotherSubject(expectation, valueType) is not { } or)
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(Rules.OrResultValueRule,
			(or.Syntax as MemberAccessExpressionSyntax)?.Name.GetLocation() ?? or.Syntax.GetLocation(),
			valueType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
	}

	/// <summary>
	///     The expectation whose value the <paramref name="invocation" /> returns without an <see langword="await" />:
	///     the argument of <c>Synchronously.Verify(…)</c> or the receiver of <c>VerifySynchronously()</c> or of
	///     <c>GetAwaiter().GetResult()</c>.
	/// </summary>
	private static IOperation? GetSynchronouslyVerifiedExpectation(IInvocationOperation invocation)
	{
		IMethodSymbol method = invocation.TargetMethod;
		if (method.MatchesFullName("aweXpect", "Synchronous", "Synchronously", "Verify") ||
		    method.MatchesFullName("aweXpect", "Synchronous", "SynchronouslyExtensions", "VerifySynchronously"))
		{
			return GetFirstArgument(invocation);
		}

		return invocation is
		{
			TargetMethod.Name: "GetResult",
			Instance: IInvocationOperation { TargetMethod.Name: "GetAwaiter", Instance: { } expectation, },
		}
			? expectation
			: null;
	}

	/// <summary>
	///     Only an aweXpect result returns the value of its expectation, unlike e.g. a task that an extension method
	///     creates from the result.
	/// </summary>
	private static bool IsExpectationResult(ITypeSymbol? type)
	{
		for (INamedTypeSymbol? current = type as INamedTypeSymbol; current is not null; current = current.BaseType)
		{
			if (current.Arity == 2 && IsAweXpectType(current, "Results", "ExpectationResult"))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     An expression statement and an assignment to a discard are the only ways to evaluate the expectation
	///     without using its value.
	/// </summary>
	private static bool IsUsed(IOperation value)
	{
		IOperation? parent = value.Parent;
		while (parent is IConversionOperation)
		{
			parent = parent.Parent;
		}

		return parent is not (null or IExpressionStatementOperation
			or ISimpleAssignmentOperation { Target: IDiscardOperation, });
	}

	/// <summary>
	///     Follows the <paramref name="expectation" /> back along its fluent chain to the last <c>Or</c> that continues
	///     with a subject of another type than the <paramref name="valueType" />.
	/// </summary>
	/// <remarks>
	///     An <c>Or</c> returns the <c>IThat&lt;TSubject&gt;</c> that its alternatives were applied to. When their
	///     subject already has the type of the value, each alternative returns it, so the value is the same whichever
	///     of them is met.
	/// </remarks>
	private static IOperation? FindOrOnAnotherSubject(IOperation expectation, ITypeSymbol valueType)
	{
		for (IOperation? current = expectation; current is not null; current = GetReceiver(current))
		{
			if (current is IPropertyReferenceOperation { Property: var property, Type: { } that, } &&
			    IsOr(property) &&
			    GetSubjectType(that) is { } subjectType &&
			    !SymbolEqualityComparer.Default.Equals(WithoutNullable(subjectType), WithoutNullable(valueType)))
			{
				return current;
			}
		}

		return null;
	}

	/// <summary>
	///     An expectation on a nullable value type can return the underlying type, which is still the subject.
	/// </summary>
	private static ITypeSymbol WithoutNullable(ITypeSymbol type)
		=> type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T, } nullable
			? nullable.TypeArguments[0]
			: type;

	private static IOperation? GetReceiver(IOperation operation)
		=> operation switch
		{
			IInvocationOperation { Instance: { } instance, } => instance,
			IInvocationOperation { TargetMethod.IsExtensionMethod: true, } invocation => GetFirstArgument(invocation),
			IMemberReferenceOperation memberReference => memberReference.Instance,
			IConversionOperation conversion => conversion.Operand,
			_ => null,
		};

	private static IOperation? GetFirstArgument(IInvocationOperation invocation)
		=> invocation.Arguments.FirstOrDefault(argument => argument.Parameter?.Ordinal == 0)?.Value;

	private static bool IsOr(IPropertySymbol property)
		=> property.Name == "Or" && IsAweXpectType(property.ContainingType, "Results", "AndOrResult");

	/// <summary>
	///     The <c>TSubject</c> of the <c>IThat&lt;TSubject&gt;</c> that <paramref name="that" /> is or implements.
	/// </summary>
	private static ITypeSymbol? GetSubjectType(ITypeSymbol that)
		=> that.AllInterfaces.Concat(that is INamedTypeSymbol namedType ? [namedType,] : [])
			.FirstOrDefault(type => type.Arity == 1 && IsAweXpectType(type, "Core", "IThat"))?.TypeArguments[0];

	private static bool IsAweXpectType(INamedTypeSymbol? type, string @namespace, string name)
		=> type is
		   {
			   ContainingType: null,
			   ContainingNamespace:
			   {
				   ContainingNamespace: { Name: "aweXpect", ContainingNamespace.IsGlobalNamespace: true, },
			   },
		   } &&
		   type.Name == name &&
		   type.ContainingNamespace.Name == @namespace;
}
