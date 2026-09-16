using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace aweXpect.Analyzers.CodeFixers;

/// <summary>
///     A code fix provider that replaces a <c>Has…</c> expectation bound directly on a thrown exception with its
///     <c>With…</c> twin, or inserts <c>.Which</c> before it.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ThrownExceptionVocabularyCodeFixProvider))]
[Shared]
public class ThrownExceptionVocabularyCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc />
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		[Rules.ThrownExceptionVocabularyRule.Id,];

	/// <inheritdoc />
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	/// <inheritdoc />
	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		SyntaxNode? root = await context.Document
			.GetSyntaxRootAsync(context.CancellationToken)
			.ConfigureAwait(false);

		foreach (Diagnostic diagnostic in context.Diagnostics)
		{
			if (root?.FindNode(diagnostic.Location.SourceSpan) is not SimpleNameSyntax name ||
			    name.Parent is not MemberAccessExpressionSyntax memberAccess)
			{
				continue;
			}

			if (diagnostic.Properties.TryGetValue(ThrownExceptionVocabularyAnalyzer.TwinProperty,
				    out string? twinName) && twinName is not null)
			{
				context.RegisterCodeFix(
					CodeAction.Create(
						string.Format(Resources.aweXpect0003ReplaceCodeFixTitle, twinName),
						_ => Task.FromResult(ReplaceWithTwin(context.Document, root, name, twinName)),
						nameof(Resources.aweXpect0003ReplaceCodeFixTitle)),
					diagnostic);
			}

			context.RegisterCodeFix(
				CodeAction.Create(
					Resources.aweXpect0003InsertWhichCodeFixTitle,
					_ => Task.FromResult(InsertWhich(context.Document, root, memberAccess)),
					nameof(Resources.aweXpect0003InsertWhichCodeFixTitle)),
				diagnostic);
		}
	}

	private static Document ReplaceWithTwin(Document document, SyntaxNode root, SimpleNameSyntax name,
		string twinName)
	{
		SyntaxToken twinIdentifier = SyntaxFactory.Identifier(twinName).WithTriviaFrom(name.Identifier);
		return document.WithSyntaxRoot(root.ReplaceToken(name.Identifier, twinIdentifier));
	}

	private static Document InsertWhich(Document document, SyntaxNode root,
		MemberAccessExpressionSyntax memberAccess)
	{
		ExpressionSyntax receiver = memberAccess.Expression;
		// Keep a line break between the receiver and the expectation behind ".Which"
		MemberAccessExpressionSyntax which = SyntaxFactory.MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,
				receiver.WithoutTrailingTrivia(),
				SyntaxFactory.IdentifierName("Which"))
			.WithTrailingTrivia(receiver.GetTrailingTrivia());
		return document.WithSyntaxRoot(root.ReplaceNode(memberAccess, memberAccess.WithExpression(which)));
	}
}
