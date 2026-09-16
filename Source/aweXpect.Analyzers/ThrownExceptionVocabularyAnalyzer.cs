using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace aweXpect.Analyzers;

/// <summary>
///     An analyzer that checks that the standalone <c>Has…</c>/<c>DoesNotHave…</c> exception expectations are not
///     bound directly on a thrown exception, where the nested <c>With…</c>/<c>Without…</c> vocabulary is required.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ThrownExceptionVocabularyAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	///     The diagnostic property holding the name of the <c>With…</c>/<c>Without…</c> twin, when one exists.
	/// </summary>
	internal const string TwinProperty = "Twin";

	private const string HasPrefix = "Has";
	private const string DoesNotHavePrefix = "DoesNotHave";

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
		    !IsStandaloneExceptionExpectation(invocation.TargetMethod, out string? twinName))
		{
			return;
		}

		IOperation? receiver = GetReceiver(invocation);
		INamedTypeSymbol? receiverType = GetThatDelegateThrowsType(receiver?.Type);
		if (receiver is null || receiverType is null)
		{
			return;
		}

		ImmutableDictionary<string, string?> properties = ImmutableDictionary<string, string?>.Empty;
		if (HasTwin(context.Compilation, receiverType, invocation.TargetMethod, twinName))
		{
			properties = properties.Add(TwinProperty, twinName);
		}

		context.ReportDiagnostic(Diagnostic.Create(Rules.ThrownExceptionVocabularyRule,
			GetLocation(invocation, receiver),
			properties,
			invocation.TargetMethod.Name,
			twinName));
	}

	private static bool IsStandaloneExceptionExpectation(IMethodSymbol method, out string twinName)
	{
		twinName = "";
		if (!method.IsExtensionMethod ||
		    method.ContainingType?.Name != "ThatException" ||
		    method.ContainingNamespace?.Name != "aweXpect" ||
		    method.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace != true)
		{
			return false;
		}

		if (method.Name.StartsWith(DoesNotHavePrefix, StringComparison.Ordinal))
		{
			twinName = "Without" + method.Name.Substring(DoesNotHavePrefix.Length);
			return true;
		}

		if (method.Name.StartsWith(HasPrefix, StringComparison.Ordinal))
		{
			twinName = "With" + method.Name.Substring(HasPrefix.Length);
			return true;
		}

		return false;
	}

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

	private static INamedTypeSymbol? GetThatDelegateThrowsType(ITypeSymbol? type)
	{
		for (INamedTypeSymbol? current = type as INamedTypeSymbol; current != null; current = current.BaseType)
		{
			if (current is { Name: "ThatDelegateThrows", Arity: 1, ContainingNamespace.Name: "Delegates", } &&
			    current.ContainingNamespace.ContainingNamespace?.Name == "aweXpect" &&
			    current.ContainingNamespace.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace == true)
			{
				return current;
			}
		}

		return null;
	}

	/// <summary>
	///     Checks whether the receiver offers a <paramref name="twinName" /> method with the same number of
	///     arguments, either as instance method or as extension method in the static <c>ThatDelegateThrows</c> class.
	/// </summary>
	private static bool HasTwin(Compilation compilation, INamedTypeSymbol receiverType, IMethodSymbol method,
		string twinName)
	{
		int argumentCount = method.ReducedFrom is null ? method.Parameters.Length - 1 : method.Parameters.Length;
		if (receiverType.GetMembers(twinName).OfType<IMethodSymbol>()
			.Any(m => m.Parameters.Length == argumentCount))
		{
			return true;
		}

		INamedTypeSymbol? extensions = compilation.GetTypeByMetadataName("aweXpect.ThatDelegateThrows");
		return extensions?.GetMembers(twinName).OfType<IMethodSymbol>()
			.Any(m => m.IsExtensionMethod && m.Parameters.Length == argumentCount + 1) == true;
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
