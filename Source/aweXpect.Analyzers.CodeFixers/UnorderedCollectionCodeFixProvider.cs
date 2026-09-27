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
///     A code fix provider that appends <c>.InAnyOrder()</c> to an expectation that compares a collection without a
///     defined order by position.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnorderedCollectionCodeFixProvider))]
[Shared]
public class UnorderedCollectionCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc />
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		[Rules.UnorderedCollectionRule.Id,];

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
			if (!diagnostic.Properties.ContainsKey(UnorderedCollectionAnalyzer.InAnyOrderProperty) ||
			    root?.FindNode(diagnostic.Location.SourceSpan) is not SimpleNameSyntax name ||
			    name.Parent is not MemberAccessExpressionSyntax { Parent: InvocationExpressionSyntax invocation, })
			{
				continue;
			}

			context.RegisterCodeFix(
				CodeAction.Create(
					Resources.aweXpect0006CodeFixTitle,
					_ => Task.FromResult(AppendInAnyOrder(context.Document, root, invocation)),
					nameof(Resources.aweXpect0006CodeFixTitle)),
				diagnostic);
		}
	}

	private static Document AppendInAnyOrder(Document document, SyntaxNode root,
		InvocationExpressionSyntax invocation)
	{
		// Keep a line break between the expectation and the options chained behind ".InAnyOrder()"
		InvocationExpressionSyntax inAnyOrder = SyntaxFactory.InvocationExpression(
				SyntaxFactory.MemberAccessExpression(
					SyntaxKind.SimpleMemberAccessExpression,
					invocation.WithoutTrailingTrivia(),
					SyntaxFactory.IdentifierName("InAnyOrder")))
			.WithTrailingTrivia(invocation.GetTrailingTrivia());
		return document.WithSyntaxRoot(root.ReplaceNode(invocation, inAnyOrder));
	}
}
