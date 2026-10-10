using System.Collections.Immutable;
using System.Linq;
using aweXpect.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace aweXpect.Analyzers;

/// <summary>
///     An analyzer that reports calls to <c>Equals</c> on an <c>Expect.That</c> subject or on an expectation, which
///     should use <c>IsEqualTo</c> instead.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EqualsAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(Rules.EqualsRule);

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterOperationAction(AnalyzeOperation, OperationKind.Invocation);
	}


	private static void AnalyzeOperation(OperationAnalysisContext context)
	{
		if (context.Operation is IInvocationOperation invocationOperation &&
		    invocationOperation.TargetMethod.Name == nameof(object.Equals))
		{
			IMethodSymbol methodSymbol = invocationOperation.TargetMethod;
			if (methodSymbol.MatchesFullName("aweXpect", "Core", "IThat", "Equals") ||
			    IsEqualsOnExpectation(invocationOperation))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(Rules.EqualsRule, context.Operation.Syntax.GetLocation())
				);
			}
		}
	}

	/// <summary>
	///     The call binds to <see cref="object.Equals(object)" />, because neither the override in <c>Expectation</c>
	///     nor the member of the implemented <c>IThat&lt;T&gt;</c> is found on a class, so the instance type decides.
	/// </summary>
	private static bool IsEqualsOnExpectation(IInvocationOperation invocation)
	{
		if (invocation.TargetMethod is not { IsStatic: false, Parameters.Length: 1, } ||
		    invocation.Instance?.Type is not { } receiverType)
		{
			return false;
		}

		// A delegate subject is no `Expectation`, but like every subject it implements `IThat<T>`.
		if (receiverType.AllInterfaces.Any(IsIThat))
		{
			return true;
		}

		for (ITypeSymbol? current = receiverType; current != null; current = current.BaseType)
		{
			if (current is { Name: "Expectation", ContainingType: null, ContainingNamespace.Name: "Core", } &&
			    current.ContainingNamespace.ContainingNamespace?.Name == "aweXpect" &&
			    current.ContainingNamespace.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace == true)
			{
				return true;
			}
		}

		return false;
	}

	private static bool IsIThat(INamedTypeSymbol type)
		=> type is { Name: "IThat", Arity: 1, ContainingType: null, ContainingNamespace.Name: "Core", } &&
		   type.ContainingNamespace.ContainingNamespace?.Name == "aweXpect" &&
		   type.ContainingNamespace.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace == true;
}
