using System.Collections.Generic;
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
///     the subject must be a local variable or a parameter that is neither a <see langword="ref" /> nor a parameter
///     of a primary constructor, the expectation must be awaited or verified in a preceding statement in an
///     enclosing block of the warning, must not be separated from it by any branching or label and the subject must
///     not be written to in between.
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
		// in between and a property could even return a different value on each access. The same applies to a
		// `ref` local or parameter, which can refer to a field, and to a primary constructor parameter used in a
		// member, which is captured in a field.
		if (subject is not (ILocalSymbol { RefKind: RefKind.None, } or IParameterSymbol { RefKind: RefKind.None, }) ||
		    IsPrimaryConstructorParameter(subject, context.CancellationToken))
		{
			return false;
		}

		// The search for the expectation usually fails fast, the scan of the whole member does not.
		return ExpectsNotNullBefore(identifier, subject, semanticModel, context.CancellationToken) &&
		       !IsWrittenIndirectly(identifier, subject, semanticModel, context.CancellationToken);
	}

	private static bool IsPrimaryConstructorParameter(ISymbol subject, CancellationToken cancellationToken)
		=> subject is IParameterSymbol
		   {
			   ContainingSymbol: IMethodSymbol { MethodKind: MethodKind.Constructor, } constructor,
		   } &&
		   constructor.DeclaringSyntaxReferences.Any(reference =>
			   reference.GetSyntax(cancellationToken) is TypeDeclarationSyntax);

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
			if (node is GlobalStatementSyntax { Parent: CompilationUnitSyntax compilationUnit, } globalStatement)
			{
				List<StatementSyntax> statements = compilationUnit.Members.OfType<GlobalStatementSyntax>()
					.Select(member => member.Statement).ToList();
				return ExpectsNotNullBefore(statements, statements.IndexOf(globalStatement.Statement),
					subject, semanticModel, cancellationToken) == Verification.Verified;
			}

			if (node is AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax or MemberDeclarationSyntax)
			{
				// The expectation must be evaluated in the same scope in which the subject is used.
				return false;
			}

			if (node is not StatementSyntax statement)
			{
				continue;
			}

			Verification verification =
				VerifyEnclosingStatement(statement, usage, subject, semanticModel, cancellationToken);
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
	private static Verification ExpectsNotNullBefore(IReadOnlyList<StatementSyntax> statements, int usageIndex,
		ISymbol subject, SemanticModel semanticModel, CancellationToken cancellationToken)
	{
		for (int index = usageIndex - 1; index >= 0; index--)
		{
			StatementSyntax statement = statements[index];

			// Branching statements are not necessarily evaluated and might write to the subject, so neither an
			// expectation inside them nor an expectation before them can be relied upon. A `goto` can jump to a
			// label and skip the expectation.
			if (statement is IfStatementSyntax or SwitchStatementSyntax or TryStatementSyntax or ForStatementSyntax
			    or CommonForEachStatementSyntax or WhileStatementSyntax or DoStatementSyntax or LabeledStatementSyntax)
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
	///     Checks if the enclosing <paramref name="statement" /> of the <paramref name="usage" /> invalidates the
	///     <paramref name="subject" />, or if a statement before it in the same block expects it to be not
	///     <see langword="null" />.
	/// </summary>
	private static Verification VerifyEnclosingStatement(StatementSyntax statement, SyntaxNode usage,
		ISymbol subject, SemanticModel semanticModel, CancellationToken cancellationToken)
	{
		// Any enclosing statement can write to the subject before the usage is reached, e.g. in the condition of
		// an `if`, in an earlier statement of a `switch` section, in the `try` block before a `finally` or in an
		// earlier argument of the same statement. A later iteration of a loop reaches the usage again, so a
		// write anywhere in the loop counts. An expectation inside the loop is found before the loop is left.
		// A `goto` can jump to the label of an enclosing statement and skip an expectation before it.
		if (statement is LabeledStatementSyntax ||
		    WritesTo(statement, subject, semanticModel, cancellationToken,
			    IsLoop(statement) ? int.MaxValue : usage.SpanStart))
		{
			return Verification.Invalidated;
		}

		if (statement.Parent is not BlockSyntax block)
		{
			return Verification.NotFound;
		}

		return ExpectsNotNullBefore(block.Statements, block.Statements.IndexOf(statement),
			subject, semanticModel, cancellationToken);
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
			    IsConditionallyEvaluated(invocation, statement, semanticModel, cancellationToken) ||
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
	///     null-coalescing operator, or because it is neither awaited nor verified.
	/// </summary>
	private static bool IsConditionallyEvaluated(SyntaxNode node, StatementSyntax statement,
		SemanticModel semanticModel, CancellationToken cancellationToken)
	{
		bool isEvaluated = false;
		for (SyntaxNode? current = node.Parent; current is not null && current != statement; current = current.Parent)
		{
			// Only the nodes that make up an awaited or verified expectation chain, an `Expect.ThatAll` combination
			// or an assignment of the evaluated result are known to always evaluate their children.
			if (current is not (InvocationExpressionSyntax or MemberAccessExpressionSyntax
				    or ParenthesizedExpressionSyntax or AwaitExpressionSyntax or ArgumentListSyntax
				    or AssignmentExpressionSyntax or EqualsValueClauseSyntax
				    or VariableDeclaratorSyntax or VariableDeclarationSyntax) &&
			    !IsEvaluatingArgument(current, semanticModel, cancellationToken))
			{
				return true;
			}

			if (current is AwaitExpressionSyntax ||
			    (!isEvaluated && current is InvocationExpressionSyntax invocation &&
			     (IsSynchronousVerification(invocation, semanticModel, cancellationToken) ||
			      IsAwaiterResult(invocation))))
			{
				isEvaluated = true;
			}
			else if (!isEvaluated && current is AssignmentExpressionSyntax or EqualsValueClauseSyntax)
			{
				// An expectation that is stored before it is evaluated might only be awaited after the usage.
				return true;
			}
		}

		return !isEvaluated;
	}

	/// <summary>
	///     Checks if the <paramref name="node" /> is an argument of <c>Expect.ThatAll</c> or of
	///     <c>Synchronously.Verify</c>, which evaluate their arguments together with themselves, unlike other methods
	///     that could store the expectation or discard it.
	/// </summary>
	private static bool IsEvaluatingArgument(SyntaxNode node, SemanticModel semanticModel,
		CancellationToken cancellationToken)
		=> node is ArgumentSyntax { Parent.Parent: InvocationExpressionSyntax invocation, } &&
		   (IsSynchronousVerification(invocation, semanticModel, cancellationToken) ||
		    (semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is IMethodSymbol methodSymbol &&
		     methodSymbol.MatchesFullName("aweXpect", "Expect", "ThatAll") &&
		     IsAweXpectAssembly(methodSymbol.ContainingAssembly, semanticModel.Compilation)));

	/// <summary>
	///     Checks if the <paramref name="invocation" /> is <c>GetAwaiter().GetResult()</c>, which evaluates the
	///     expectation synchronously.
	/// </summary>
	private static bool IsAwaiterResult(InvocationExpressionSyntax invocation)
		=> invocation.Expression is MemberAccessExpressionSyntax
		{
			Name.Identifier.Text: "GetResult",
			Expression: InvocationExpressionSyntax
			{
				Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "GetAwaiter", },
			},
		};

	private static bool IsSynchronousVerification(InvocationExpressionSyntax invocation, SemanticModel semanticModel,
		CancellationToken cancellationToken)
		=> invocation.Expression is MemberAccessExpressionSyntax
			   {
				   Name.Identifier.Text: "VerifySynchronously" or "Verify",
			   }
			   or IdentifierNameSyntax { Identifier.Text: "Verify", } &&
		   semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is IMethodSymbol methodSymbol &&
		   (methodSymbol.MatchesFullName("aweXpect", "Synchronous", "SynchronouslyExtensions", "VerifySynchronously") ||
		    methodSymbol.MatchesFullName("aweXpect", "Synchronous", "Synchronously", "Verify")) &&
		   IsAweXpectAssembly(methodSymbol.ContainingAssembly, semanticModel.Compilation);

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
	///     or if a <see langword="ref" /> alias of the subject is taken, because calling the one or writing through
	///     the other changes the subject without a visible write between the expectation and the usage.
	/// </summary>
	private static bool IsWrittenIndirectly(SyntaxNode usage, ISymbol subject, SemanticModel semanticModel,
		CancellationToken cancellationToken)
	{
		SyntaxNode? member = usage.FirstAncestorOrSelf<MemberDeclarationSyntax>();
		if (member is GlobalStatementSyntax)
		{
			// Top-level statements share their locals across all global statements of the file.
			member = member.Parent;
		}

		if (member is null)
		{
			return false;
		}

		return member.DescendantNodes().Any(node => node switch
		{
			AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax
				=> WritesTo(node, subject, semanticModel, cancellationToken),
			RefExpressionSyntax refExpression
				=> IsSubject(refExpression.Expression, subject, semanticModel, cancellationToken),
			_ => false,
		});
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
				ArgumentSyntax argument when argument.RefKindKeyword.IsKind(SyntaxKind.RefKeyword) || argument.RefKindKeyword.IsKind(SyntaxKind.OutKeyword) => argument.Expression,
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
					    (!IsAweXpectAssembly(method.ContainingAssembly, semanticModel.Compilation) &&
					     !KeepsSubject(method, semanticModel.Compilation)))
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
	///     Checks if the method is an expectation that a <see langword="null" /> subject cannot fulfil.
	/// </summary>
	/// <remarks>
	///     The expectations declare this themselves with the <c>aweXpect.Core.GuaranteesNotNullAttribute</c>, because
	///     neither their name nor their result type can be used to detect it: overloads of the same name can differ,
	///     and nothing makes the result type of an extension package name the subject type as not nullable exactly
	///     when a <see langword="null" /> subject cannot fulfil the expectation.
	///     <para />
	///     The receiver of an aweXpect method is deliberately not restricted to <c>IThat&lt;TSubject&gt;</c>:
	///     <c>aweXpect.Core</c> declares expectations such as <c>Throws</c> or <c>IsExactly</c> as instance members of
	///     other types. An expectation of an extension package must be an extension method on <c>IThat&lt;TSubject&gt;</c>.
	/// </remarks>
	private static bool GuaranteesNotNull(IMethodSymbol methodSymbol, Compilation compilation)
		// The receiver check comes first: it is a few symbol comparisons, while decoding the attributes
		// of an arbitrary metadata symbol is not, and this runs for every invocation in every preceding
		// statement that is scanned.
		=> (IsAweXpectAssembly(methodSymbol.ContainingAssembly, compilation) ||
		    IsThat(methodSymbol.ReceiverType, compilation)) &&
		   (methodSymbol.ReducedFrom ?? methodSymbol).GetAttributes()
		   .Any(attribute => attribute.AttributeClass is { Name: "GuaranteesNotNullAttribute", } attributeClass &&
		                     IsAweXpectAssembly(attributeClass.ContainingAssembly, compilation));

	/// <summary>
	///     Checks if the extension method of another package continues on the same subject, because it returns an
	///     aweXpect result for its receiver <c>IThat&lt;TSubject&gt;</c>, like
	///     <c>AndOrResult&lt;TType, IThat&lt;TSubject&gt;&gt;</c>.
	/// </summary>
	/// <remarks>
	///     An extension method that returns anything else, e.g. an <c>IThat&lt;TMember&gt;</c> of a member, might
	///     switch to a different subject, so a later expectation in the chain cannot be attributed to the subject of
	///     <c>Expect.That</c>. This also applies when it returns an <c>IThat&lt;TSubject&gt;</c>, because the member
	///     can have the same type as the subject.
	/// </remarks>
	private static bool KeepsSubject(IMethodSymbol methodSymbol, Compilation compilation)
	{
		if (methodSymbol.ReducedFrom is null || !IsThat(methodSymbol.ReceiverType, compilation))
		{
			return false;
		}

		ITypeSymbol receiver = methodSymbol.ReceiverType!;
		for (INamedTypeSymbol? type = methodSymbol.ReturnType as INamedTypeSymbol;
		     type is not null;
		     type = type.BaseType)
		{
			if (IsAweXpectAssembly(type.ContainingAssembly, compilation) &&
			    type.TypeArguments.Any(argument => SymbolEqualityComparer.Default.Equals(argument, receiver)))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="type" /> is the <c>IThat&lt;TSubject&gt;</c> of aweXpect.
	/// </summary>
	private static bool IsThat(ITypeSymbol? type, Compilation compilation)
		=> type is INamedTypeSymbol { Name: "IThat", Arity: 1, } namedType &&
		   namedType.ContainingNamespace is
		   {
			   Name: "Core", ContainingNamespace: { Name: "aweXpect", ContainingNamespace.IsGlobalNamespace: true, },
		   } &&
		   IsAweXpectAssembly(namedType.ContainingAssembly, compilation);

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
