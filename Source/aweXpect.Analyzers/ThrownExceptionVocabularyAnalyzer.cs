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
	///     The diagnostic property holding the name of the <c>With…</c> twin, when one exists.
	/// </summary>
	internal const string TwinProperty = "Twin";

	/// <summary>
	///     The <c>Has…</c> expectations of <c>ThatException</c> with the name of their <c>With…</c> twin and the
	///     argument counts for which the twin exists.
	/// </summary>
	private static readonly ImmutableDictionary<string, (string Twin, int[] ArgumentCounts)> Expectations =
		new Dictionary<string, (string Twin, int[] ArgumentCounts)>
		{
			["HasMessage"] = ("WithMessage", new[] { 0, 1, }),
			["HasParamName"] = ("WithParamName", new[] { 0, 1, }),
			["HasHResult"] = ("WithHResult", new[] { 1, }),
			["HasInner"] = ("WithInner", new[] { 0, 1, 2, }),
			["HasInnerException"] = ("WithInnerException", new[] { 0, 1, }),
			["HasRecursiveInnerExceptions"] = ("WithRecursiveInnerExceptions", new[] { 1, }),
		}.ToImmutableDictionary();

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		[Rules.ThrownExceptionVocabularyRule, Rules.ThrownExceptionVocabularyWithoutTwinRule,];

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
		    !Expectations.TryGetValue(invocation.TargetMethod.Name,
			    out (string Twin, int[] ArgumentCounts) expectation))
		{
			return;
		}

		IOperation? receiver = GetReceiver(invocation);
		if (receiver is null || !IsThatDelegateThrows(receiver.Type))
		{
			return;
		}

		int argumentCount = invocation.Arguments.Length - (invocation.Instance is null ? 1 : 0);
		Location location = GetLocation(invocation, receiver);
		if (expectation.ArgumentCounts.Contains(argumentCount))
		{
			context.ReportDiagnostic(Diagnostic.Create(Rules.ThrownExceptionVocabularyRule,
				location,
				ImmutableDictionary<string, string?>.Empty.Add(TwinProperty, expectation.Twin),
				invocation.TargetMethod.Name,
				expectation.Twin));
		}
		else
		{
			context.ReportDiagnostic(Diagnostic.Create(Rules.ThrownExceptionVocabularyWithoutTwinRule,
				location,
				invocation.TargetMethod.Name));
		}
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
		IOperation? receiver = invocation.Instance ?? invocation.Arguments.FirstOrDefault()?.Value;
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
