using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace aweXpect.Analyzers;

/// <summary>
///     An analyzer that checks that an expectation which is written for an ordinary subject is not applied to a
///     delegate subject, where the covariance of <c>IThat&lt;out T&gt;</c> silently binds it to the delegate wrapper.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DelegateSubjectAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	///     The diagnostic property that is set when the delegate has a return value, so that the code fix can offer
	///     to continue with it.
	/// </summary>
	internal const string HasResultProperty = "HasResult";

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [Rules.DelegateSubjectRule,];

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
		    !invocation.TargetMethod.IsExtensionMethod ||
		    !TargetsAnOrdinarySubject(invocation.TargetMethod))
		{
			return;
		}

		IOperation? receiver = GetReceiver(invocation);
		if (receiver is null || !IsDelegateSubject(receiver.Type, out bool hasResult))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(Rules.DelegateSubjectRule,
			GetLocation(invocation, receiver),
			hasResult
				? ImmutableDictionary<string, string?>.Empty.Add(HasResultProperty, null)
				: ImmutableDictionary<string, string?>.Empty,
			invocation.TargetMethod.Name));
	}

	/// <summary>
	///     Whether the <c>this</c> parameter of the <b>declaration</b> is an <c>IThat&lt;…&gt;</c> of something other
	///     than a delegate subject. The declaration is decisive, because a type parameter that only got <i>inferred</i>
	///     as the delegate wrapper means the author never intended the expectation for a delegate.
	/// </summary>
	private static bool TargetsAnOrdinarySubject(IMethodSymbol method)
	{
		IMethodSymbol declaration = (method.ReducedFrom ?? method).OriginalDefinition;
		return declaration.Parameters.FirstOrDefault()?.Type is INamedTypeSymbol
		       {
			       Name: "IThat" or "IExpectThat", Arity: 1, ContainingNamespace.Name: "Core",
		       } subject &&
		       subject.ContainingNamespace.ContainingNamespace?.Name == "aweXpect" &&
		       subject.ContainingNamespace.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace == true &&
		       !IsDelegateSubject(subject.TypeArguments[0], out _);
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

	private static bool IsDelegateSubject(ITypeSymbol? type, out bool hasResult)
	{
		hasResult = false;
		if (type is not INamedTypeSymbol
		    {
			    ContainingType: { Name: "ThatDelegate", ContainingType: null, },
			    ContainingNamespace.Name: "Delegates",
		    } named ||
		    named.ContainingNamespace.ContainingNamespace?.Name != "aweXpect" ||
		    named.ContainingNamespace.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace != true)
		{
			return false;
		}

		hasResult = named is { Name: "WithValue", Arity: 1, };
		return hasResult || named is { Name: "WithoutValue", Arity: 0, };
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
