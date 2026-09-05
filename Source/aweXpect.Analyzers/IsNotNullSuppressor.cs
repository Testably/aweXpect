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
///     the expectation must be a preceding statement in an enclosing block of the warning, must not be separated
///     from it by any branching and the subject must not be written to in between.
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
		if (semanticModel.GetSymbolInfo(identifier, context.CancellationToken).Symbol is not { } subject)
		{
			return false;
		}

		return ExpectsNotNullBefore(identifier, subject, semanticModel, context.CancellationToken);
	}

	/// <summary>
	///     Returns the identifier that the nullability warning refers to.
	/// </summary>
	private static IdentifierNameSyntax? GetWarnedIdentifier(SyntaxNode node)
		=> node switch
		{
			IdentifierNameSyntax identifier => identifier,
			MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax identifier, } => identifier,
			ArgumentSyntax { Expression: IdentifierNameSyntax identifier, } => identifier,
			_ => null,
		};

	/// <summary>
	///     Checks if an <c>IsNotNull</c> expectation for the <paramref name="subject" /> is guaranteed to have been
	///     evaluated before the <paramref name="usage" />.
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
			if (statement is not (ExpressionStatementSyntax or LocalDeclarationStatementSyntax))
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
	///     Checks if the <paramref name="statement" /> contains an <c>IsNotNull</c> expectation for the
	///     <paramref name="subject" />.
	/// </summary>
	private static bool ExpectsNotNull(StatementSyntax statement, ISymbol subject, SemanticModel semanticModel,
		CancellationToken cancellationToken)
	{
		foreach (InvocationExpressionSyntax invocation in statement.DescendantNodes()
			         .OfType<InvocationExpressionSyntax>())
		{
			if (semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol methodSymbol ||
			    !IsNotNullExpectation(methodSymbol, semanticModel.Compilation) ||
			    IsCombinedWithOr(invocation) ||
			    IsInsideLambda(invocation, statement))
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
	///     Checks if the <paramref name="node" /> is nested inside a lambda within the <paramref name="statement" />,
	///     which is not necessarily evaluated together with the statement.
	/// </summary>
	private static bool IsInsideLambda(SyntaxNode node, StatementSyntax statement)
	{
		for (SyntaxNode? current = node; current is not null && current != statement; current = current.Parent)
		{
			if (current is AnonymousFunctionExpressionSyntax)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="statement" /> writes to the <paramref name="subject" />, which invalidates a
	///     preceding <c>IsNotNull</c> expectation.
	/// </summary>
	private static bool WritesTo(StatementSyntax statement, ISymbol subject, SemanticModel semanticModel,
		CancellationToken cancellationToken)
	{
		foreach (SyntaxNode node in statement.DescendantNodes())
		{
			ExpressionSyntax? target = node switch
			{
				AssignmentExpressionSyntax assignment => assignment.Left,
				ArgumentSyntax argument when !argument.RefKindKeyword.IsKind(SyntaxKind.None) => argument.Expression,
				_ => null,
			};

			if (target is IdentifierNameSyntax identifier &&
			    SymbolEqualityComparer.Default.Equals(
				    semanticModel.GetSymbolInfo(identifier, cancellationToken).Symbol, subject))
			{
				return true;
			}
		}

		return false;
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
	///     Checks if the method is an <c>IsNotNull</c> expectation from aweXpect.
	/// </summary>
	private static bool IsNotNullExpectation(IMethodSymbol methodSymbol, Compilation compilation)
	{
		IMethodSymbol method = methodSymbol.ReducedFrom ?? methodSymbol;
		return method.Name == "IsNotNull" &&
		       method.ContainingNamespace?.Name == "aweXpect" &&
		       method.ContainingNamespace?.ContainingNamespace?.IsGlobalNamespace == true &&
		       IsAweXpectAssembly(method.ContainingAssembly, compilation);
	}

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
