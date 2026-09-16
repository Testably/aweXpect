using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace aweXpect.Analyzers;

/// <summary>
///     An analyzer that checks that the standalone <c>Has…</c> exception expectations are not bound directly on a
///     thrown exception, where the nested <c>With…</c> vocabulary is required.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ThrownExceptionVocabularyAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	///     The diagnostic property holding the name of the <c>With…</c> twin.
	/// </summary>
	internal const string TwinProperty = "Twin";

	/// <summary>
	///     The <c>Has…</c> expectations of <c>ThatException</c> with the name of their <c>With…</c> twin, which
	///     accepts the same arguments.
	/// </summary>
	private static readonly ImmutableDictionary<string, string> Expectations =
		new Dictionary<string, string>
		{
			["HasMessage"] = "WithMessage",
			["HasParamName"] = "WithParamName",
			["HasHResult"] = "WithHResult",
			["HasInner"] = "WithInner",
			["HasInnerException"] = "WithInnerException",
			["HasRecursiveInnerExceptions"] = "WithRecursiveInnerExceptions",
		}.ToImmutableDictionary();

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		[Rules.ThrownExceptionVocabularyRule,];

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterOperationAction(AnalyzeOperation, OperationKind.Invocation);
	}

	private static void AnalyzeOperation(OperationAnalysisContext context)
	{
		if (context.Operation is not IInvocationOperation invocation ||
		    !IsThatExceptionMethod(invocation.TargetMethod) ||
		    !Expectations.TryGetValue(invocation.TargetMethod.Name, out string? twin))
		{
			return;
		}

		IOperation? receiver = GetReceiver(invocation);
		if (receiver is null || !IsThatDelegateThrows(receiver.Type))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(Rules.ThrownExceptionVocabularyRule,
			GetLocation(invocation, receiver),
			ImmutableDictionary<string, string?>.Empty.Add(TwinProperty, twin),
			invocation.TargetMethod.Name,
			twin));
	}

	private static bool IsThatExceptionMethod(IMethodSymbol method)
		=> method.IsExtensionMethod &&
		   method.ContainingType?.Name == "ThatException" &&
		   method.ContainingNamespace?.Name == "aweXpect" &&
		   method.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace == true;

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

	private static bool IsThatDelegateThrows(ITypeSymbol? type)
	{
		for (INamedTypeSymbol? current = type as INamedTypeSymbol; current != null; current = current.BaseType)
		{
			if (current is { Name: "ThatDelegateThrows", Arity: 1, ContainingNamespace.Name: "Delegates", } &&
			    current.ContainingNamespace.ContainingNamespace?.Name == "aweXpect" &&
			    current.ContainingNamespace.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace == true)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Reports on the method name when the expectation is chained on the receiver, so that the code fix can
	///     rewrite the member access; otherwise (e.g. a static call) on the whole invocation.
	/// </summary>
	private static Location GetLocation(IInvocationOperation invocation, IOperation receiver)
		=> invocation.Syntax is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax memberAccess, } &&
		   memberAccess.Expression == receiver.Syntax
			? memberAccess.Name.GetLocation()
			: invocation.Syntax.GetLocation();
}
