using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace aweXpect.Analyzers;

/// <summary>
///     An analyzer that checks that the value returned by an option method of an immutable option, e.g.
///     <c>Quantifier.AtLeast(2)</c>, is used: the option itself never changes, so the call alone has no effect.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscardedOptionAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	///     The classes of <c>aweXpect.Options</c> that are immutable like its <see langword="readonly" /> structs.
	/// </summary>
	private static readonly ImmutableHashSet<string> ImmutableOptionClasses =
		ImmutableHashSet.Create("StringEqualityOptions", "SignalerPredicateOptions", "ObjectToleranceMatchType",
			"TriggerEventFilter");

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(Rules.DiscardedOptionRule);

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
	}

	private static void AnalyzeInvocation(OperationAnalysisContext context)
	{
		if (context.Operation is not IInvocationOperation { TargetMethod: { IsStatic: false, } method, } invocation ||
		    !IsDiscarded(invocation) ||
		    !IsImmutableOption(method.ContainingType) ||
		    !SymbolEqualityComparer.Default.Equals(method.ReturnType, method.ContainingType))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(Rules.DiscardedOptionRule,
			(invocation.Syntax as InvocationExpressionSyntax)?.Expression switch
			{
				MemberAccessExpressionSyntax memberAccess => memberAccess.Name.GetLocation(),
				MemberBindingExpressionSyntax memberBinding => memberBinding.Name.GetLocation(),
				_ => invocation.Syntax.GetLocation(),
			},
			method.Name, method.ContainingType.Name));
	}

	/// <summary>
	///     Whether the <paramref name="invocation" /> is the whole statement, also when it is only made if the instance
	///     is not <see langword="null" />, e.g. <c>options?.AtLeast(2);</c>.
	/// </summary>
	private static bool IsDiscarded(IOperation invocation)
	{
		IOperation operation = invocation;
		while (operation.Parent is IConditionalAccessOperation conditionalAccess &&
		       conditionalAccess.WhenNotNull == operation)
		{
			operation = conditionalAccess;
		}

		return operation.Parent is IExpressionStatementOperation { IsImplicit: false, };
	}

	/// <summary>
	///     A <see langword="readonly" /> struct cannot change itself, and the immutable option classes return a new
	///     instance from each option method.
	/// </summary>
	private static bool IsImmutableOption(INamedTypeSymbol? type)
		=> type is
		   {
			   ContainingType: null,
			   ContainingNamespace:
			   {
				   Name: "Options",
				   ContainingNamespace: { Name: "aweXpect", ContainingNamespace.IsGlobalNamespace: true, },
			   },
		   } &&
		   (type is { IsValueType: true, IsReadOnly: true, } ||
		    (type.TypeKind == TypeKind.Class && ImmutableOptionClasses.Contains(type.Name)));
}
