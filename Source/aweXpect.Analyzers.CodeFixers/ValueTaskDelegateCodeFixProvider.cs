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
///     A code fix provider that makes the delegate return a <c>Task</c> by appending <c>.AsTask()</c>, so that it
///     binds to the <c>Task</c> overload of <c>Expect.That</c>, which awaits it.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ValueTaskDelegateCodeFixProvider))]
[Shared]
public class ValueTaskDelegateCodeFixProvider : CodeFixProvider
{
	private const string CancellationTokenName = "token";

	/// <inheritdoc />
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [Rules.ValueTaskDelegateRule.Id,];

	/// <inheritdoc />
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	/// <inheritdoc />
	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		SyntaxNode? root = await context.Document
			.GetSyntaxRootAsync(context.CancellationToken)
			.ConfigureAwait(false);
		SemanticModel? semanticModel = await context.Document
			.GetSemanticModelAsync(context.CancellationToken)
			.ConfigureAwait(false);

		foreach (Diagnostic diagnostic in context.Diagnostics)
		{
			if (root?.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is not ExpressionSyntax
				    @delegate ||
			    HasExplicitTypeArgument(@delegate))
			{
				continue;
			}

			ExpressionSyntax? replacement = @delegate switch
			{
				LambdaExpressionSyntax { ExpressionBody: { } body, } lambda => lambda.WithExpressionBody(AsTask(body)),
				IdentifierNameSyntax or MemberAccessExpressionSyntax =>
					InvokeInLambda(@delegate,
						diagnostic.Properties.ContainsKey(ValueTaskDelegateAnalyzer.HasCancellationTokenProperty),
						semanticModel),
				_ => null,
			};
			if (replacement is null)
			{
				continue;
			}

			context.RegisterCodeFix(
				CodeAction.Create(
					Resources.aweXpect0007CodeFixTitle,
					_ => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(@delegate, replacement))),
					nameof(Resources.aweXpect0007CodeFixTitle)),
				diagnostic);
		}
	}

	/// <summary>
	///     Whether <c>Expect.That</c> is called with an explicit type argument, which a delegate returning a <c>Task</c>
	///     would no longer match.
	/// </summary>
	private static bool HasExplicitTypeArgument(ExpressionSyntax @delegate)
		=> @delegate.Parent?.Parent?.Parent is InvocationExpressionSyntax
		{
			Expression: GenericNameSyntax or MemberAccessExpressionSyntax { Name: GenericNameSyntax, },
		};

	private static ExpressionSyntax AsTask(ExpressionSyntax expression)
	{
		ExpressionSyntax target = expression.WithoutTrivia();
		if (target is not (InvocationExpressionSyntax or IdentifierNameSyntax or MemberAccessExpressionSyntax
		    or ParenthesizedExpressionSyntax))
		{
			target = SyntaxFactory.ParenthesizedExpression(target);
		}

		return SyntaxFactory.InvocationExpression(
				SyntaxFactory.MemberAccessExpression(
					SyntaxKind.SimpleMemberAccessExpression,
					target,
					SyntaxFactory.IdentifierName("AsTask")))
			.WithTriviaFrom(expression);
	}

	private static ExpressionSyntax? InvokeInLambda(ExpressionSyntax @delegate, bool hasCancellationToken,
		SemanticModel? semanticModel)
	{
		string target = @delegate.WithoutTrivia().ToString();
		if (!hasCancellationToken)
		{
			return SyntaxFactory.ParseExpression($"() => {target}().AsTask()").WithTriviaFrom(@delegate);
		}

		if (semanticModel is null ||
		    !semanticModel.LookupSymbols(@delegate.SpanStart, name: CancellationTokenName).IsEmpty)
		{
			return null;
		}

		return SyntaxFactory
			.ParseExpression($"{CancellationTokenName} => {target}({CancellationTokenName}).AsTask()")
			.WithTriviaFrom(@delegate);
	}
}
