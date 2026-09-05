using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using aweXpect.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace aweXpect.Analyzers;

/// <summary>
///     A suppressor that silences nullability warnings for a subject that a preceding
///     <c>Expect.That(subject).IsNotNull()</c> expectation verified to be not <see langword="null" />.
/// </summary>
/// <remarks>
///     The suppression is limited to expectations that are guaranteed to have been evaluated for the same subject:
///     the subject must be a local variable or a parameter, the expectation must be a preceding statement in an
///     enclosing block of the warning, must not be separated from it by any branching and the subject must not be
///     written to in between.
///     <para />
///     Only the warnings are suppressed, the null state of the compiler remains unchanged.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class IsNotNullSuppressor : DiagnosticSuppressor
{
	/// <inheritdoc />
	public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; }
		= Rules.IsNotNullSuppressions;

	/// <inheritdoc />
	public override void ReportSuppressions(SuppressionAnalysisContext context)
	{
		foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
		{
			SuppressionDescriptor? suppression = Rules.IsNotNullSuppressions
				.FirstOrDefault(descriptor => descriptor.SuppressedDiagnosticId == diagnostic.Id);
			if (suppression is not null && IsVerifiedNotNull(context, diagnostic))
			{
				context.ReportSuppression(Suppression.Create(suppression, diagnostic));
			}
		}
	}

	private static bool IsVerifiedNotNull(SuppressionAnalysisContext context, Diagnostic diagnostic)
	{
		if (diagnostic.Location.SourceTree is not { } sourceTree)
		{
			return false;
		}

		SyntaxNode root = sourceTree.GetRoot(context.CancellationToken);
		if (GetWarnedIdentifier(root.FindNode(diagnostic.Location.SourceSpan)) is not { } identifier)
		{
			return false;
		}

		SemanticModel semanticModel = context.GetSemanticModel(sourceTree);
		ISymbol? subject = semanticModel.GetSymbolInfo(identifier, context.CancellationToken).Symbol;

		// Only local variables and parameters can be tracked reliably: a field could be changed by any method call
		// in between and a property could even return a different value on each access.
		if (subject is not (ILocalSymbol or IParameterSymbol))
		{
			return false;
		}

		// The search for the expectation usually fails fast, the scan of the whole member for lambdas does not.
		return ExpectsNotNullBefore(identifier, subject, semanticModel, context.CancellationToken) &&
		       !IsWrittenInsideLambda(identifier, subject, semanticModel, context.CancellationToken);
	}

	/// <summary>
	///     Returns the identifier that the nullability warning refers to.
	/// </summary>
	private static IdentifierNameSyntax? GetWarnedIdentifier(SyntaxNode node)
		=> node switch
		{
			IdentifierNameSyntax identifier => identifier,
			ArgumentSyntax { Expression: IdentifierNameSyntax identifier, } => identifier,
			_ => null,
		};

	/// <summary>
	///     Checks if an expectation that guarantees a not-<see langword="null" /> <paramref name="subject" /> is
	///     guaranteed to have been evaluated before the <paramref name="usage" />.
	/// </summary>
	private static bool ExpectsNotNullBefore(SyntaxNode usage, ISymbol subject, SemanticModel semanticModel,
		CancellationToken cancellationToken)
	{
		for (SyntaxNode? node = usage; node is not null; node = node.Parent)
		{
			if (node is AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax or MemberDeclarationSyntax)
			{
				// The expectation must be evaluated in the same scope in which the subject is used.
				return false;
			}

			// Any enclosing statement can write to the subject before the usage is reached, e.g. in the condition of
			// an `if`, in an earlier statement of a `switch` section, in the `try` block before a `finally` or in an
			// earlier argument of the same statement. A later iteration of a loop reaches the usage again, so a
			// write anywhere in the loop counts. An expectation inside the loop is found before the loop is left.
			if (node is StatementSyntax enclosing &&
			    WritesTo(enclosing, subject, semanticModel, cancellationToken,
				    IsLoop(enclosing) ? int.MaxValue : usage.SpanStart))
			{
				return false;
			}

			if (node is not StatementSyntax statement || statement.Parent is not BlockSyntax block)
			{
				continue;
			}

			Verification verification = ExpectsNotNullBefore(block.Statements, block.Statements.IndexOf(statement),
				subject, semanticModel, cancellationToken);
			if (verification != Verification.NotFound)
			{
				return verification == Verification.Verified;
			}
		}

		return false;
	}

	/// <summary>
	///     Checks if one of the statements before <paramref name="usageIndex" /> expects the <paramref name="subject" />
	///     to be not <see langword="null" />.
	/// </summary>
	private static Verification ExpectsNotNullBefore(SyntaxList<StatementSyntax> statements, int usageIndex,
		ISymbol subject, SemanticModel semanticModel, CancellationToken cancellationToken)
	{
		for (int index = usageIndex - 1; index >= 0; index--)
		{
			StatementSyntax statement = statements[index];

			// Branching statements are not necessarily evaluated and might write to the subject, so neither an
			// expectation inside them nor an expectation before them can be relied upon.
			if (statement is IfStatementSyntax or SwitchStatementSyntax or TryStatementSyntax or ForStatementSyntax
			    or CommonForEachStatementSyntax or WhileStatementSyntax or DoStatementSyntax)
			{
				return Verification.Invalidated;
			}

			if (ExpectsNotNull(statement, subject, semanticModel, cancellationToken))
			{
				return Verification.Verified;
			}

			if (WritesTo(statement, subject, semanticModel, cancellationToken))
			{
				return Verification.Invalidated;
			}
		}

		return Verification.NotFound;
	}

	/// <summary>
	///     Checks if the <paramref name="statement" /> contains an expectation for the <paramref name="subject" /> that
	///     guarantees it to be not <see langword="null" />.
	/// </summary>
	private static bool ExpectsNotNull(StatementSyntax statement, ISymbol subject, SemanticModel semanticModel,
		CancellationToken cancellationToken)
	{
		foreach (InvocationExpressionSyntax invocation in statement.DescendantNodes()
			         .OfType<InvocationExpressionSyntax>())
		{
			if (semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol methodSymbol ||
			    !GuaranteesNotNull(methodSymbol, semanticModel.Compilation) ||
			    IsCombinedWithOr(invocation) ||
			    IsConditionallyEvaluated(invocation, statement) ||
			    IsInsideThatAny(invocation, statement, semanticModel, cancellationToken))
			{
				continue;
			}

			if (FindSubject(invocation, semanticModel, cancellationToken) is IdentifierNameSyntax expectedSubject &&
			    SymbolEqualityComparer.Default.Equals(
				    semanticModel.GetSymbolInfo(expectedSubject, cancellationToken).Symbol, subject))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="node" /> is not necessarily evaluated together with the
	///     <paramref name="statement" />, e.g. because it is nested inside a lambda, a conditional expression or a
	///     null-coalescing operator.
	/// </summary>
	private static bool IsConditionallyEvaluated(SyntaxNode node, StatementSyntax statement)
	{
		for (SyntaxNode? current = node.Parent; current is not null && current != statement; current = current.Parent)
		{
			// Only the nodes that make up an awaited expectation chain, an `Expect.ThatAll` combination or an
			// assignment of the awaited result are known to always evaluate their children.
			if (current is not (InvocationExpressionSyntax or MemberAccessExpressionSyntax
			    or ParenthesizedExpressionSyntax or AwaitExpressionSyntax
			    or ArgumentSyntax or ArgumentListSyntax
			    or AssignmentExpressionSyntax or EqualsValueClauseSyntax
			    or VariableDeclaratorSyntax or VariableDeclarationSyntax))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="node" /> is nested inside an <c>Expect.ThatAny</c> combination within the
	///     <paramref name="statement" />, which only requires any of its expectations to be met.
	/// </summary>
	private static bool IsInsideThatAny(SyntaxNode node, StatementSyntax statement, SemanticModel semanticModel,
		CancellationToken cancellationToken)
	{
		for (SyntaxNode? current = node; current is not null && current != statement; current = current.Parent)
		{
			if (current is InvocationExpressionSyntax invocation &&
			    semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is IMethodSymbol methodSymbol &&
			    methodSymbol.MatchesFullName("aweXpect", "Expect", "ThatAny") &&
			    IsAweXpectAssembly(methodSymbol.ContainingAssembly, semanticModel.Compilation))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Checks if any lambda or local function in the enclosing member writes to the <paramref name="subject" />,
	///     because calling it changes the subject without a visible write between the expectation and the usage.
	/// </summary>
	private static bool IsWrittenInsideLambda(SyntaxNode usage, ISymbol subject, SemanticModel semanticModel,
		CancellationToken cancellationToken)
	{
		if (usage.FirstAncestorOrSelf<MemberDeclarationSyntax>() is not { } member)
		{
			return false;
		}

		return member.DescendantNodes()
			.Where(node => node is AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax)
			.Any(node => WritesTo(node, subject, semanticModel, cancellationToken));
	}

	private static bool IsLoop(StatementSyntax statement)
		=> statement is ForStatementSyntax or CommonForEachStatementSyntax or WhileStatementSyntax
			or DoStatementSyntax;

	/// <summary>
	///     Checks if the <paramref name="node" /> writes to the <paramref name="subject" />, which invalidates a
	///     preceding expectation. Only writes that start before the <paramref name="before" /> position are considered.
	/// </summary>
	private static bool WritesTo(SyntaxNode node, ISymbol subject, SemanticModel semanticModel,
		CancellationToken cancellationToken, int before = int.MaxValue)
	{
		foreach (SyntaxNode descendant in node.DescendantNodes())
		{
			if (descendant.SpanStart >= before)
			{
				continue;
			}

			ExpressionSyntax? target = descendant switch
			{
				AssignmentExpressionSyntax assignment => assignment.Left,
				ArgumentSyntax argument when !argument.RefKindKeyword.IsKind(SyntaxKind.None) => argument.Expression,
				_ => null,
			};

			if (target is not null && IsSubject(target, subject, semanticModel, cancellationToken))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="expression" /> refers to the <paramref name="subject" />, also when it is one
	///     of the targets of a deconstruction.
	/// </summary>
	private static bool IsSubject(ExpressionSyntax expression, ISymbol subject, SemanticModel semanticModel,
		CancellationToken cancellationToken)
	{
		if (expression is TupleExpressionSyntax tuple)
		{
			return tuple.Arguments.Any(argument
				=> IsSubject(argument.Expression, subject, semanticModel, cancellationToken));
		}

		return expression is IdentifierNameSyntax &&
		       SymbolEqualityComparer.Default.Equals(
			       semanticModel.GetSymbolInfo(expression, cancellationToken).Symbol, subject);
	}

	/// <summary>
	///     Checks if the expectation is combined with an <c>Or</c>, which makes it optional.
	/// </summary>
	private static bool IsCombinedWithOr(InvocationExpressionSyntax invocation)
	{
		for (SyntaxNode? node = invocation.Parent; node is not null; node = node.Parent)
		{
			if (node is MemberAccessExpressionSyntax memberAccess)
			{
				if (memberAccess.Name.Identifier.Text == "Or")
				{
					return true;
				}
			}
			else if (node is not (InvocationExpressionSyntax or ParenthesizedExpressionSyntax))
			{
				return false;
			}
		}

		return false;
	}

	/// <summary>
	///     Walks the expectation chain back to the <c>Expect.That(subject)</c> it belongs to and returns the subject.
	/// </summary>
	private static ExpressionSyntax? FindSubject(InvocationExpressionSyntax invocation, SemanticModel semanticModel,
		CancellationToken cancellationToken)
	{
		ExpressionSyntax? current = GetSource(invocation);
		while (current is not null)
		{
			switch (current)
			{
				case InvocationExpressionSyntax chained:
					if (GetSubject(chained, semanticModel, cancellationToken) is { } subject)
					{
						return subject;
					}

					if (semanticModel.GetSymbolInfo(chained, cancellationToken).Symbol is not IMethodSymbol method ||
					    !IsAweXpectAssembly(method.ContainingAssembly, semanticModel.Compilation))
					{
						return null;
					}

					current = GetSource(chained);
					break;
				// Only `And` keeps expecting on the same subject: `Or` makes the expectation optional and
				// `Which`, `Whose` or `WhoseValue` switch to a different subject.
				case MemberAccessExpressionSyntax { Name.Identifier.Text: "And", } memberAccess:
					current = memberAccess.Expression;
					break;
				default:
					return null;
			}
		}

		return null;
	}

	private static ExpressionSyntax? GetSource(InvocationExpressionSyntax invocation)
		=> invocation.Expression is MemberAccessExpressionSyntax memberAccess ? memberAccess.Expression : null;

	/// <summary>
	///     Returns the subject of an <c>Expect.That(subject)</c> invocation.
	/// </summary>
	private static ExpressionSyntax? GetSubject(InvocationExpressionSyntax invocation, SemanticModel semanticModel,
		CancellationToken cancellationToken)
	{
		if (semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol methodSymbol ||
		    !methodSymbol.MatchesFullName("aweXpect", "Expect", "That") ||
		    !IsAweXpectAssembly(methodSymbol.ContainingAssembly, semanticModel.Compilation) ||
		    invocation.ArgumentList.Arguments.Count == 0)
		{
			return null;
		}

		return invocation.ArgumentList.Arguments[0].Expression;
	}

	/// <summary>
	///     Checks if the method is an aweXpect expectation that a <see langword="null" /> subject cannot fulfil.
	/// </summary>
	/// <remarks>
	///     The result type of an expectation cannot be used to detect this, although it often replaces the nullable
	///     subject type of the <c>IThat&lt;TSubject?&gt;</c> it extends with its not-nullable counterpart: it also
	///     does so for expectations that a <see langword="null" /> subject does fulfil, e.g. <c>IsNotEmpty</c> on a
	///     string or <c>IsNotEqualTo</c> on a collection.
	/// </remarks>
	private static bool GuaranteesNotNull(IMethodSymbol methodSymbol, Compilation compilation)
		=> (methodSymbol.ReducedFrom ?? methodSymbol).Name
		   is "IsNotNull" or "IsNotNullOrEmpty" or "IsNotNullOrWhiteSpace" &&
		   IsAweXpectAssembly(methodSymbol.ContainingAssembly, compilation) &&
		   methodSymbol.ReceiverType is INamedTypeSymbol receiver &&
		   IsThatSubject(receiver);

	/// <summary>
	///     Checks if the type is the <c>aweXpect.Core.IThat&lt;TSubject&gt;</c> that all expectations extend.
	/// </summary>
	private static bool IsThatSubject(INamedTypeSymbol type)
		=> type is { Name: "IThat", TypeArguments.Length: 1, } &&
		   type.ContainingNamespace?.Name == "Core" &&
		   type.ContainingNamespace?.ContainingNamespace?.Name == "aweXpect" &&
		   type.ContainingNamespace?.ContainingNamespace?.ContainingNamespace?.IsGlobalNamespace == true;

	/// <summary>
	///     Checks that the symbol originates from a referenced aweXpect assembly and not from a look-alike that is
	///     defined in the compilation itself.
	/// </summary>
	private static bool IsAweXpectAssembly(IAssemblySymbol? assembly, Compilation compilation)
		=> assembly is not null &&
		   assembly.Name is "aweXpect" or "aweXpect.Core" &&
		   !SymbolEqualityComparer.Default.Equals(assembly, compilation.Assembly);

	private enum Verification
	{
		/// <summary>
		///     No expectation was found, so the search continues in the enclosing block.
		/// </summary>
		NotFound,

		/// <summary>
		///     The subject was verified to be not <see langword="null" />.
		/// </summary>
		Verified,

		/// <summary>
		///     Something in between invalidates any earlier expectation, so the search must stop.
		/// </summary>
		Invalidated,
	}
}
