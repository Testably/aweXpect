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
		ImmutableHashSet.Create("StringEqualityOptions", "SignalerPredicateOptions");

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
		if (context.Operation is not IInvocationOperation
		    {
			    Parent: IExpressionStatementOperation { IsImplicit: false, },
			    TargetMethod: { IsStatic: false, } method,
		    } invocation ||
		    !IsImmutableOption(method.ContainingType) ||
		    !SymbolEqualityComparer.Default.Equals(method.ReturnType, method.ContainingType))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(Rules.DiscardedOptionRule,
			(invocation.Syntax as InvocationExpressionSyntax)?.Expression is MemberAccessExpressionSyntax memberAccess
				? memberAccess.Name.GetLocation()
				: invocation.Syntax.GetLocation(),
			method.Name, method.ContainingType.Name));
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
