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
public class AwaitExpectationAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		[Rules.AwaitExpectationRule, Rules.AsyncVoidExpectationRule,];

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterOperationAction(AnalyzeOperation, OperationKind.Invocation);
	}


	private static void AnalyzeOperation(OperationAnalysisContext context)
	{
		if (context.Operation is not IInvocationOperation invocationOperation ||
		    !IsExpectationStart(invocationOperation.TargetMethod))
		{
			return;
		}

		if (!IsEvaluated(invocationOperation))
		{
			context.ReportDiagnostic(
				Diagnostic.Create(Rules.AwaitExpectationRule, invocationOperation.Syntax.GetLocation()));
		}
		else if (IsInAsyncVoidLambda(invocationOperation))
		{
			context.ReportDiagnostic(
				Diagnostic.Create(Rules.AsyncVoidExpectationRule, invocationOperation.Syntax.GetLocation()));
		}
	}

	private static bool IsExpectationStart(IMethodSymbol methodSymbol)
		=> methodSymbol.MatchesFullName("aweXpect", "Expect", "That") ||
		   methodSymbol.MatchesFullName("aweXpect", "Expect", "ThatAll") ||
		   methodSymbol.MatchesFullName("aweXpect", "Expect", "ThatAny");

	/// <summary>
	///     Follows the expectation along its fluent continuations and only reports when it is provably discarded, so
	///     that an unrecognized usage never breaks a build.
	/// </summary>
	private static bool IsEvaluated(IOperation expectation)
	{
		IOperation current = expectation;
		for (IOperation? parent = current.Parent; parent is not null; parent = current.Parent)
		{
			switch (parent)
			{
				case IAwaitOperation:
				case IReturnOperation:
					return true;
				case IExpressionStatementOperation:
				case ISimpleAssignmentOperation { Target: IDiscardOperation, }:
					return false;
				case IVariableInitializerOperation initializer:
					return IsLocalUsed(initializer);
				case IInvocationOperation invocation when IsVerification(invocation.TargetMethod):
					return true;
				case IArgumentOperation argument when !IsExtensionReceiver(argument):
					return true;
				case IConversionOperation:
				case IParenthesizedOperation:
				case IInvocationOperation:
				case IArgumentOperation:
					break;
				case IMemberReferenceOperation memberReference
					when ReferenceEquals(memberReference.Instance, current):
					break;
				default:
					return true;
			}

			current = parent;
		}

		return true;
	}

	/// <summary>
	///     An invocation that consumes the expectation instead of continuing it: nothing can be chained on a
	///     <c>void</c> continuation, and <c>GetAwaiter</c> starts the synchronous evaluation.
	/// </summary>
	private static bool IsVerification(IMethodSymbol methodSymbol)
		=> methodSymbol.ReturnsVoid ||
		   methodSymbol.Name == "GetAwaiter" ||
		   methodSymbol.MatchesFullName("aweXpect", "Synchronous", "SynchronouslyExtensions", "VerifySynchronously");

	private static bool IsExtensionReceiver(IArgumentOperation argument)
		=> argument is
		{
			Parameter.Ordinal: 0, Parent: IInvocationOperation { TargetMethod.IsExtensionMethod: true, },
		};

	/// <summary>
	///     Whether the local the expectation is assigned to is referenced anywhere in the block that declares it,
	///     which is the whole scope in which it could be awaited, verified or returned.
	/// </summary>
	private static bool IsLocalUsed(IVariableInitializerOperation initializer)
	{
		if (initializer.Parent is not IVariableDeclaratorOperation declarator)
		{
			return true;
		}

		IOperation? scope = declarator.Parent;
		while (scope is not null and not IBlockOperation)
		{
			scope = scope.Parent;
		}

		return scope is null || IsReferenced(declarator.Symbol, scope);
	}

	private static bool IsReferenced(ILocalSymbol local, IOperation scope)
	{
		foreach (IOperation child in scope.ChildOperations)
		{
			if (child is ILocalReferenceOperation reference &&
			    SymbolEqualityComparer.Default.Equals(reference.Local, local))
			{
				return true;
			}

			if (IsReferenced(local, child))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     An <see langword="async" /> lambda that is converted to a void-returning delegate returns before the
	///     expectation is evaluated, so its failure surfaces after the test has completed.
	/// </summary>
	private static bool IsInAsyncVoidLambda(IOperation operation)
	{
		for (IOperation? parent = operation.Parent; parent is not null; parent = parent.Parent)
		{
			if (parent is IAnonymousFunctionOperation anonymousFunction)
			{
				return anonymousFunction.Symbol is { IsAsync: true, ReturnsVoid: true, };
			}

			if (parent is ILocalFunctionOperation or IMethodBodyOperation)
			{
				return false;
			}
		}

		return false;
	}
}
