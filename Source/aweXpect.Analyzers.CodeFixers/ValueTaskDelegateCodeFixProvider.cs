using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;

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
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create(Rules.ValueTaskDelegateRule.Id);

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
				AnonymousFunctionExpressionSyntax function => ReturnTask(function, semanticModel),
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

	/// <summary>
	///     Appends <c>.AsTask()</c> to the returned values of a function that is not <c>async</c>, and replaces an
	///     explicit <c>ValueTask</c> return type with the matching <c>Task</c>.
	/// </summary>
	private static ExpressionSyntax? ReturnTask(AnonymousFunctionExpressionSyntax function,
		SemanticModel? semanticModel)
	{
		TypeSyntax? returnType = (function as ParenthesizedLambdaExpressionSyntax)?.ReturnType;
		if (function.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
		{
			return returnType is null ? null : WithTaskReturnType(function, returnType);
		}

		if (semanticModel is null || AppendAsTask(function, semanticModel) is not { } fixedFunction)
		{
			return null;
		}

		return returnType is null ? fixedFunction : WithTaskReturnType(fixedFunction, returnType);
	}

	/// <summary>
	///     A returned value that only gets its <c>ValueTask</c> type from the return type, such as <c>default</c>, has
	///     no <c>AsTask()</c> method, so no fix is offered for it.
	/// </summary>
	private static AnonymousFunctionExpressionSyntax? AppendAsTask(AnonymousFunctionExpressionSyntax function,
		SemanticModel semanticModel)
	{
		List<ExpressionSyntax?> returnedValues = function.Body is ExpressionSyntax body
			? [body,]
			: function.Body
				.DescendantNodes(node => node is not (AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax))
				.OfType<ReturnStatementSyntax>()
				.Select(returnStatement => returnStatement.Expression)
				.ToList();
		if (!returnedValues.All(value => value is not null && HasAsTask(value, semanticModel)))
		{
			return null;
		}

		return function.ReplaceNodes(returnedValues.OfType<ExpressionSyntax>(), (value, _) => AsTask(value));
	}

	private static bool HasAsTask(ExpressionSyntax value, SemanticModel semanticModel)
		=> semanticModel.GetSpeculativeSymbolInfo(value.SpanStart, AsTask(value),
			   SpeculativeBindingOption.BindAsExpression).Symbol is IMethodSymbol { Name: "AsTask", } method &&
		   IsValueTask(method.ContainingType);

	/// <summary>
	///     An alias for the <c>ValueTask</c> type is not replaced, so no fix is offered for it.
	/// </summary>
	private static ExpressionSyntax? WithTaskReturnType(AnonymousFunctionExpressionSyntax function,
		TypeSyntax returnType)
	{
		SimpleNameSyntax? name = returnType switch
		{
			QualifiedNameSyntax qualifiedName => qualifiedName.Right,
			AliasQualifiedNameSyntax aliasQualifiedName => aliasQualifiedName.Name,
			SimpleNameSyntax simpleName => simpleName,
			_ => null,
		};
		if (name?.Identifier.ValueText != "ValueTask" || function is not ParenthesizedLambdaExpressionSyntax lambda)
		{
			return null;
		}

		TypeSyntax taskType = SyntaxFactory.ParseTypeName(name is GenericNameSyntax genericName
				? $"System.Threading.Tasks.Task{genericName.TypeArgumentList}"
				: "System.Threading.Tasks.Task")
			.WithTriviaFrom(returnType)
			.WithAdditionalAnnotations(Simplifier.Annotation);
		return lambda.WithReturnType(taskType);
	}

	private static bool IsValueTask(ITypeSymbol type)
		=> type.OriginalDefinition.ToDisplayString() is "System.Threading.Tasks.ValueTask"
			or "System.Threading.Tasks.ValueTask<TResult>";

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
