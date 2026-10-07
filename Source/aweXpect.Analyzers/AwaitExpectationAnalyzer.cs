using System.Collections.Immutable;
using System.Linq;
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
		else if (IsInAsyncVoidFunction(invocationOperation, context.ContainingSymbol))
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
				case IReturnOperation returnOperation when GetAwaitOfDeferringCall(returnOperation) is { } awaitOperation:
					current = awaitOperation;
					continue;
				case IAwaitOperation:
				case IReturnOperation:
					return true;
				case IExpressionStatementOperation:
				case ISimpleAssignmentOperation { Target: IDiscardOperation, }:
					return IsConsumedByExtensionMethod(current);
				case IVariableInitializerOperation initializer:
					return IsLocalUsed(initializer) || IsConsumedByExtensionMethod(current);
				case IInvocationOperation invocation when IsVerification(invocation):
					return true;
				case IArgumentOperation argument when !IsExtensionReceiver(argument):
					return true;
				case IConversionOperation:
				case IParenthesizedOperation:
				case IInvocationOperation:
				case IArgumentOperation:
				case IConditionalOperation conditional when !ReferenceEquals(conditional.Condition, current):
				case ISwitchExpressionArmOperation arm when ReferenceEquals(arm.Value, current):
				case ISwitchExpressionOperation when current is ISwitchExpressionArmOperation:
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
	///     The <see langword="await" /> of a call like <c>Task.Run(() => Expect.That(…)…)</c> that only hands back the
	///     expectation returned from the lambda: its delegate returns a type parameter of the method (or, for an
	///     <see langword="async" /> lambda, a task of it), and the method returns a task of that type parameter, so
	///     awaiting it (also through <c>ConfigureAwait(…)</c>) yields the expectation without evaluating it.
	/// </summary>
	private static IAwaitOperation? GetAwaitOfDeferringCall(IReturnOperation returnOperation)
	{
		IOperation? function = returnOperation.Parent;
		while (function is not null and not IAnonymousFunctionOperation and not ILocalFunctionOperation)
		{
			function = function.Parent;
		}

		if (function is not IAnonymousFunctionOperation
		    {
			    Symbol: var lambda,
			    Parent: IDelegateCreationOperation
			    {
				    Parent: IArgumentOperation
				    {
					    Parameter: { } parameter, Parent: IInvocationOperation invocation,
				    },
			    },
		    } ||
		    GetAwait(invocation) is not { } awaitOperation ||
		    parameter.OriginalDefinition.Type is not INamedTypeSymbol
		    {
			    DelegateInvokeMethod.ReturnType: var returnType,
		    } ||
		    (lambda.IsAsync ? GetTaskResult(returnType) : returnType) is not ITypeParameterSymbol
		    {
			    TypeParameterKind: TypeParameterKind.Method,
		    } result ||
		    GetTaskResult(invocation.TargetMethod.OriginalDefinition.ReturnType) is not { } taskResult ||
		    !SymbolEqualityComparer.Default.Equals(taskResult, result))
		{
			return null;
		}

		return awaitOperation;
	}

	private static ITypeSymbol? GetTaskResult(ITypeSymbol type)
		=> type is INamedTypeSymbol { TypeArguments.Length: 1, } task ? task.TypeArguments[0] : null;

	private static IAwaitOperation? GetAwait(IInvocationOperation invocation)
		=> invocation.Parent switch
		{
			IAwaitOperation awaitOperation => awaitOperation,
			IInvocationOperation { TargetMethod.Name: "ConfigureAwait", Parent: IAwaitOperation awaitOperation, } configureAwait
				when ReferenceEquals(configureAwait.Instance, invocation) => awaitOperation,
			_ => null,
		};

	/// <summary>
	///     An invocation that consumes the expectation instead of continuing it: nothing can be chained on a
	///     <c>void</c> continuation, and <c>GetResult</c> on the awaiter waits for the evaluation and throws its failure.
	/// </summary>
	/// <remarks>
	///     <c>GetAwaiter</c> alone only starts the evaluation, so a failure would remain in a task nobody observes.
	/// </remarks>
	private static bool IsVerification(IInvocationOperation invocation)
		=> invocation.TargetMethod.ReturnsVoid ||
		   invocation is
		   {
			   TargetMethod.Name: "GetResult", Instance: IInvocationOperation { TargetMethod.Name: "GetAwaiter", },
		   } ||
		   invocation.TargetMethod.MatchesFullName("aweXpect", "Synchronous", "SynchronouslyExtensions",
			   "VerifySynchronously");

	/// <summary>
	///     Whether the discarded <paramref name="value" /> was returned by an extension method that does not continue
	///     the expectation, like a helper that verifies it and returns its value. What such a method does with the
	///     expectation is unknown, so it is not provably discarded.
	/// </summary>
	/// <remarks>
	///     Only what follows the last part of the fluent API matters, because an extension method can also return a
	///     type of its own in the middle of the expectation, like the quantifier of <c>All()</c>.
	/// </remarks>
	private static bool IsConsumedByExtensionMethod(IOperation value)
	{
		IOperation? current = value;
		while (current is not null && !IsPartOfTheFluentApi(current.Type))
		{
			if (current is IInvocationOperation { TargetMethod.IsExtensionMethod: true, })
			{
				return true;
			}

			current = current switch
			{
				IInvocationOperation invocation => invocation.Instance,
				IMemberReferenceOperation memberReference => memberReference.Instance,
				IConversionOperation conversion => conversion.Operand,
				IParenthesizedOperation parenthesized => parenthesized.Operand,
				_ => null,
			};
		}

		return false;
	}

	/// <summary>
	///     Whether the <paramref name="type" /> is declared by aweXpect or one of its extension packages, or derives
	///     from or implements such a type, like a result, an <c>IThat&lt;T&gt;</c> or a quantifier. A type parameter
	///     counts when it is constrained to such a type.
	/// </summary>
	private static bool IsPartOfTheFluentApi(ITypeSymbol? type)
	{
		if (type is ITypeParameterSymbol typeParameter)
		{
			return typeParameter.ConstraintTypes.Any(IsPartOfTheFluentApi);
		}

		for (ITypeSymbol? current = type; current is not null; current = current.BaseType)
		{
			if (IsInAweXpectNamespace(current))
			{
				return true;
			}
		}

		return type?.AllInterfaces.Any(IsInAweXpectNamespace) == true;
	}

	private static bool IsInAweXpectNamespace(ITypeSymbol type)
	{
		INamespaceSymbol? @namespace = type.ContainingNamespace;
		while (@namespace?.ContainingNamespace is { IsGlobalNamespace: false, } parent)
		{
			@namespace = parent;
		}

		return @namespace?.Name == "aweXpect";
	}

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
	///     An <see langword="async" /> <see langword="void" /> method, local function or lambda returns before the
	///     expectation is evaluated, so its failure surfaces after the test has completed.
	/// </summary>
	/// <remarks>
	///     Only the nearest enclosing function matters, because a task-returning function is observed by its own caller.
	/// </remarks>
	private static bool IsInAsyncVoidFunction(IOperation operation, ISymbol containingSymbol)
	{
		for (IOperation? parent = operation.Parent; parent is not null; parent = parent.Parent)
		{
			switch (parent)
			{
				case IAnonymousFunctionOperation anonymousFunction:
					return IsAsyncVoid(anonymousFunction.Symbol);
				case ILocalFunctionOperation localFunction:
					return IsAsyncVoid(localFunction.Symbol);
			}
		}

		return IsAsyncVoid(containingSymbol);
	}

	private static bool IsAsyncVoid(ISymbol symbol)
		=> symbol is IMethodSymbol { IsAsync: true, ReturnsVoid: true, };
}
