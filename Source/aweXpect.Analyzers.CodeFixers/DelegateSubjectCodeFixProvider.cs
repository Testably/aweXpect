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
///     A code fix provider that continues with the value returned by the delegate, by inserting
///     <c>.DoesNotThrow().WhoseResult</c> before the expectation.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DelegateSubjectCodeFixProvider))]
[Shared]
public class DelegateSubjectCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc />
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [Rules.DelegateSubjectRule.Id,];

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
			if (!diagnostic.Properties.ContainsKey(DelegateSubjectAnalyzer.HasResultProperty) ||
			    root?.FindNode(diagnostic.Location.SourceSpan) is not SimpleNameSyntax name ||
			    name.Parent is not MemberAccessExpressionSyntax memberAccess)
			{
				continue;
			}

			context.RegisterCodeFix(
				CodeAction.Create(
					Resources.aweXpect0004CodeFixTitle,
					_ => Task.FromResult(InsertWhoseResult(context.Document, root, memberAccess)),
					nameof(Resources.aweXpect0004CodeFixTitle)),
				diagnostic);
		}
	}

	private static Document InsertWhoseResult(Document document, SyntaxNode root,
		MemberAccessExpressionSyntax memberAccess)
	{
		ExpressionSyntax receiver = memberAccess.Expression;
		// Keep a line break between the receiver and the expectation behind ".WhoseResult"
		MemberAccessExpressionSyntax whoseResult = SyntaxFactory.MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,
				SyntaxFactory.InvocationExpression(
					SyntaxFactory.MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						receiver.WithoutTrailingTrivia(),
						SyntaxFactory.IdentifierName("DoesNotThrow"))),
				SyntaxFactory.IdentifierName("WhoseResult"))
			.WithTrailingTrivia(receiver.GetTrailingTrivia());
		return document.WithSyntaxRoot(root.ReplaceNode(memberAccess, memberAccess.WithExpression(whoseResult)));
	}
}
