using System.Collections.Immutable;
using aweXpect.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace aweXpect.Analyzers;

/// <summary>
///     An analyzer that checks that all <c>Expect.That</c> expectations are awaited.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EqualsAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [Rules.EqualsRule,];

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
	///     The call binds to <see cref="object.Equals(object)" />, because the override in <c>Expectation</c> is not
	///     a separate member, so the instance type decides.
	/// </summary>
	private static bool IsEqualsOnExpectation(IInvocationOperation invocation)
	{
		if (invocation.TargetMethod is not { IsStatic: false, Parameters.Length: 1, })
		{
			return false;
		}

		for (ITypeSymbol? current = invocation.Instance?.Type; current != null; current = current.BaseType)
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
}
