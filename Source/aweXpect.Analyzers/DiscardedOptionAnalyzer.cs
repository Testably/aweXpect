using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace aweXpect.Analyzers;

/// <summary>
///     An analyzer that checks that the value returned by an option method of an immutable option, e.g.
///     <c>Quantifier.AtLeast(2)</c>, is used: the option itself never changes, so the call alone has no effect.
/// </summary>
/// <remarks>
///     The option types are marked with the <c>aweXpect.Core.ImmutableOptionsAttribute</c>, and the option methods are
///     their instance methods and the extension methods on them that return the same type.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscardedOptionAnalyzer : DiagnosticAnalyzer
{
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
		if (context.Operation is not IInvocationOperation { TargetMethod: var method, } invocation ||
		    !IsDiscarded(invocation))
		{
			return;
		}

		// IOperation presents an extension method in its static form, also when it is called like an instance method.
		// One that takes any type, e.g. `T Log<T>(this T value)`, is no option method and can have an effect.
		ITypeSymbol? optionType = method switch
		{
			{ IsStatic: false, } => method.ContainingType,
			{ IsExtensionMethod: true, } when method.OriginalDefinition.Parameters[0].Type is INamedTypeSymbol
				=> method.Parameters[0].Type,
			_ => null,
		};
		if (optionType is not INamedTypeSymbol namedOptionType ||
		    !SymbolEqualityComparer.Default.Equals(method.ReturnType, optionType) ||
		    !IsImmutableOption(namedOptionType))
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
			method.Name, optionType.Name));
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
	///     The options declare themselves immutable with the <c>aweXpect.Core.ImmutableOptionsAttribute</c>, so that the
	///     options of the extension packages and of other extensions are covered as well.
	/// </summary>
	private static bool IsImmutableOption(INamedTypeSymbol type)
		=> type.GetAttributes().Any(attribute => attribute.AttributeClass is
		{
			Name: "ImmutableOptionsAttribute",
			ContainingNamespace:
			{
				Name: "Core",
				ContainingNamespace: { Name: "aweXpect", ContainingNamespace.IsGlobalNamespace: true, },
			},
		});
}
