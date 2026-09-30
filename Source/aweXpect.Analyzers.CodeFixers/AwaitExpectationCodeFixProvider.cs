using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace aweXpect.Analyzers.CodeFixers;

/// <summary>
///     A code fix provider that makes all <c>Expect.That</c> methods that are neither async nor verified async.
/// </summary>
/// <remarks>
///     The fix is only offered where the result compiles without changing a signature that other code depends on.
/// </remarks>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AwaitExpectationCodeFixProvider))]
[Shared]
public class AwaitExpectationCodeFixProvider : CodeFixProvider
{
	private static readonly SyntaxAnnotation RemovedReturnAnnotation = new();

	/// <inheritdoc />
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [Rules.AwaitExpectationRule.Id,];

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
		if (root is null || semanticModel is null)
		{
			return;
		}

		foreach (Diagnostic diagnostic in context.Diagnostics)
		{
			if (root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is not ExpressionSyntax
				    expectationStart ||
			    await CreateFixAsync(GetExpectation(expectationStart), semanticModel, context.Document.Project.Solution,
					    context.CancellationToken).ConfigureAwait(false) is not var (node, replacement))
			{
				continue;
			}

			context.RegisterCodeFix(
				CodeAction.Create(
					Resources.aweXpect0001CodeFixTitle,
					_ => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(node, replacement))),
					nameof(Resources.aweXpect0001CodeFixTitle)),
				diagnostic);
		}
	}

	private static ExpressionSyntax GetExpectation(ExpressionSyntax expression)
	{
		while (expression.Parent is ParenthesizedExpressionSyntax ||
		       expression.Parent is MemberAccessExpressionSyntax memberAccess &&
		       memberAccess.Expression == expression ||
		       expression.Parent is InvocationExpressionSyntax invocation && invocation.Expression == expression)
		{
			expression = (ExpressionSyntax)expression.Parent;
		}

		return expression;
	}

	private static async Task<(SyntaxNode Node, SyntaxNode Replacement)?> CreateFixAsync(ExpressionSyntax expectation,
		SemanticModel semanticModel, Solution solution, CancellationToken cancellationToken)
	{
		if (AwaitExpectation(expectation) is not var (target, awaited))
		{
			return null;
		}

		SyntaxNode? function = expectation.Ancestors().FirstOrDefault(node =>
			node is AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax or MemberDeclarationSyntax
				or LockStatementSyntax);
		return function switch
		{
			GlobalStatementSyntax or AnonymousFunctionExpressionSyntax { AsyncKeyword.RawKind: not 0, } =>
				(target, awaited),
			MethodDeclarationSyntax or LocalFunctionStatementSyntax => await MakeFunctionAsync(function, target,
				awaited, semanticModel, solution, cancellationToken).ConfigureAwait(false),
			_ => null,
		};
	}

	/// <summary>
	///     Awaits the expectation where it is discarded; an unused local is dropped, as the awaited result may be
	///     <see langword="void" />.
	/// </summary>
	private static (SyntaxNode Node, SyntaxNode Replacement)? AwaitExpectation(ExpressionSyntax expectation)
		=> expectation.Parent switch
		{
			ExpressionStatementSyntax or ArrowExpressionClauseSyntax or AnonymousFunctionExpressionSyntax =>
				(expectation, Await(expectation)),
			AssignmentExpressionSyntax
			{
				Left: IdentifierNameSyntax { Identifier.ValueText: "_", }, Parent: ExpressionStatementSyntax,
			} discard when discard.Right == expectation =>
				(discard, Await(expectation).WithLeadingTrivia(discard.GetLeadingTrivia())),
			EqualsValueClauseSyntax
			{
				Parent: VariableDeclaratorSyntax
				{
					Parent: VariableDeclarationSyntax
					{
						Variables.Count: 1, Parent: LocalDeclarationStatementSyntax declaration,
					},
				},
			} => (declaration, SyntaxFactory.ExpressionStatement(Await(expectation.WithoutTrailingTrivia()),
					declaration.SemicolonToken)
				.WithLeadingTrivia(declaration.GetLeadingTrivia())),
			_ => null,
		};

	private static AwaitExpressionSyntax Await(ExpressionSyntax expression)
		=> SyntaxFactory.AwaitExpression(expression.WithLeadingTrivia(SyntaxFactory.Space))
			.WithLeadingTrivia(expression.GetLeadingTrivia());

	private static async Task<(SyntaxNode Node, SyntaxNode Replacement)?> MakeFunctionAsync(SyntaxNode function,
		SyntaxNode target, SyntaxNode awaited, SemanticModel semanticModel, Solution solution,
		CancellationToken cancellationToken)
	{
		(SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxNode? body) = function switch
		{
			MethodDeclarationSyntax method => (method.Modifiers, method.ReturnType,
				(SyntaxNode?)method.Body ?? method.ExpressionBody),
			LocalFunctionStatementSyntax localFunction => (localFunction.Modifiers, localFunction.ReturnType,
				(SyntaxNode?)localFunction.Body ?? localFunction.ExpressionBody),
			_ => default,
		};
		if (modifiers.Any(SyntaxKind.AsyncKeyword))
		{
			return (target, awaited);
		}

		if (body is null ||
		    semanticModel.GetDeclaredSymbol(function, cancellationToken) is not IMethodSymbol symbol ||
		    symbol.ReturnsByRef || symbol.ReturnsByRefReadonly ||
		    GetOwnNodes(body).OfType<YieldStatementSyntax>().Any() ||
		    symbol.Parameters.Any(parameter => parameter.RefKind != RefKind.None || parameter.Type.IsRefLikeType))
		{
			return null;
		}

		Dictionary<SyntaxNode, SyntaxNode> replacements = new() { [target] = awaited, };
		switch (GetTaskKind(symbol.ReturnType))
		{
			case TaskKind.NonGeneric
				when !TryReplaceCompletedTaskReturns(body, semanticModel, replacements, cancellationToken):
			case TaskKind.Generic when !TryAwaitReturnedTasks(body, semanticModel, replacements, cancellationToken):
				return null;
			case TaskKind.None:
				if (await HasFixedSignatureAsync(symbol, modifiers, solution, cancellationToken).ConfigureAwait(false))
				{
					return null;
				}

				returnType = SyntaxFactory.ParseTypeName(symbol.ReturnsVoid
						? "System.Threading.Tasks.Task"
						: $"System.Threading.Tasks.Task<{returnType.WithoutTrivia()}>")
					.WithTriviaFrom(returnType)
					.WithAdditionalAnnotations(Simplifier.Annotation);
				break;
		}

		SyntaxNode newFunction = RemoveTrailingReturn(
			function.ReplaceNodes(replacements.Keys, (original, _) => replacements[original]));
		modifiers = AddAsync(modifiers, ref returnType);
		newFunction = newFunction switch
		{
			MethodDeclarationSyntax method => method.WithModifiers(modifiers).WithReturnType(returnType),
			LocalFunctionStatementSyntax localFunction => localFunction.WithModifiers(modifiers)
				.WithReturnType(returnType),
			_ => newFunction,
		};
		return (function, newFunction.WithAdditionalAnnotations(Formatter.Annotation));
	}

	/// <summary>
	///     Drops the <c>return</c> statement marked for removal at the end of the body, keeping its comments.
	/// </summary>
	private static SyntaxNode RemoveTrailingReturn(SyntaxNode function)
	{
		if (function.GetAnnotatedNodes(RemovedReturnAnnotation).FirstOrDefault() is not StatementSyntax
		    {
			    Parent: BlockSyntax block,
		    } removedReturn)
		{
			return function;
		}

		SyntaxTriviaList leading = removedReturn.GetLeadingTrivia();
		IEnumerable<SyntaxTrivia> comments = leading.All(trivia =>
			trivia.IsKind(SyntaxKind.WhitespaceTrivia) || trivia.IsKind(SyntaxKind.EndOfLineTrivia))
			? []
			: leading.Reverse().SkipWhile(trivia => trivia.IsKind(SyntaxKind.WhitespaceTrivia)).Reverse();
		return function.ReplaceNode(block, block
			.WithStatements(block.Statements.Remove(removedReturn))
			.WithCloseBraceToken(block.CloseBraceToken.WithLeadingTrivia(
				comments.Concat(block.CloseBraceToken.LeadingTrivia))));
	}

	/// <summary>
	///     Whether the return type can't change, because a base type, an interface, an overriding member, another
	///     partial declaration or a reference to the method depends on it.
	/// </summary>
	/// <remarks>
	///     A reference is a method group conversion, which would no longer compile, or a call, which would discard the
	///     returned task without a warning in a caller that is not <c>async</c>.
	/// </remarks>
	private static async Task<bool> HasFixedSignatureAsync(IMethodSymbol symbol, SyntaxTokenList modifiers,
		Solution solution, CancellationToken cancellationToken)
	{
		if (modifiers.Any(SyntaxKind.PartialKeyword) || symbol.IsOverride || symbol.IsVirtual ||
		    !symbol.ExplicitInterfaceImplementations.IsEmpty ||
		    symbol.ContainingType.AllInterfaces.SelectMany(@interface => @interface.GetMembers()).Any(member =>
			    SymbolEqualityComparer.Default.Equals(
				    symbol.ContainingType.FindImplementationForInterfaceMember(member), symbol)))
		{
			return true;
		}

		IEnumerable<ReferencedSymbol> references = await SymbolFinder
			.FindReferencesAsync(symbol, solution, cancellationToken)
			.ConfigureAwait(false);
		return references.Any(reference => reference.Locations.Any());
	}

	/// <summary>
	///     <c>async</c> must precede <c>partial</c>, and takes over the leading trivia of the declaration when there are
	///     no other modifiers.
	/// </summary>
	private static SyntaxTokenList AddAsync(SyntaxTokenList modifiers, ref TypeSyntax returnType)
	{
		SyntaxToken asyncModifier = SyntaxFactory.Token(SyntaxKind.AsyncKeyword)
			.WithTrailingTrivia(SyntaxFactory.Space);
		int partialIndex = modifiers.IndexOf(SyntaxKind.PartialKeyword);
		if (partialIndex >= 0)
		{
			SyntaxToken partialModifier = modifiers[partialIndex];
			return modifiers
				.Replace(partialModifier, partialModifier.WithLeadingTrivia())
				.Insert(partialIndex, asyncModifier.WithLeadingTrivia(partialModifier.LeadingTrivia));
		}

		if (modifiers.Count == 0)
		{
			asyncModifier = asyncModifier.WithLeadingTrivia(returnType.GetLeadingTrivia());
			returnType = returnType.WithoutLeadingTrivia();
		}

		return modifiers.Add(asyncModifier);
	}

	/// <summary>
	///     The nodes of the body, excluding those of nested functions, whose statements belong to another function.
	/// </summary>
	private static IEnumerable<SyntaxNode> GetOwnNodes(SyntaxNode body)
		=> body.DescendantNodes(node => node is not (AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax));

	/// <summary>
	///     In an <c>async</c> method returning a non-generic task, <c>return Task.CompletedTask;</c> becomes
	///     <c>return;</c> and is dropped at the end of the body. Other returned tasks would need to be awaited in a
	///     separate statement, so no fix is offered for them.
	/// </summary>
	private static bool TryReplaceCompletedTaskReturns(SyntaxNode body, SemanticModel semanticModel,
		Dictionary<SyntaxNode, SyntaxNode> replacements, CancellationToken cancellationToken)
	{
		foreach (ReturnStatementSyntax returnStatement in GetOwnNodes(body).OfType<ReturnStatementSyntax>())
		{
			if (returnStatement.Expression is null ||
			    semanticModel.GetSymbolInfo(returnStatement.Expression, cancellationToken).Symbol is not IPropertySymbol
			    {
				    Name: "CompletedTask",
			    } property ||
			    GetTaskKind(property.ContainingType) != TaskKind.NonGeneric)
			{
				return false;
			}

			replacements[returnStatement] = body is BlockSyntax block && block.Statements.LastOrDefault() == returnStatement
				? returnStatement.WithAdditionalAnnotations(RemovedReturnAnnotation)
				: SyntaxFactory.ReturnStatement().WithTriviaFrom(returnStatement);
		}

		return true;
	}

	/// <summary>
	///     In an <c>async</c> method returning a generic task, the returned tasks are awaited, and the value of
	///     <c>Task.FromResult(value)</c> is returned directly.
	/// </summary>
	private static bool TryAwaitReturnedTasks(SyntaxNode body, SemanticModel semanticModel,
		Dictionary<SyntaxNode, SyntaxNode> replacements, CancellationToken cancellationToken)
	{
		foreach (ReturnStatementSyntax returnStatement in GetOwnNodes(body).OfType<ReturnStatementSyntax>())
		{
			ExpressionSyntax? expression = returnStatement.Expression;
			if (expression is null || semanticModel.GetTypeInfo(expression, cancellationToken).Type is null)
			{
				return false;
			}

			if (expression is InvocationExpressionSyntax { ArgumentList.Arguments: { Count: 1, } arguments, } &&
			    semanticModel.GetSymbolInfo(expression, cancellationToken).Symbol is IMethodSymbol
			    {
				    Name: "FromResult",
			    } method &&
			    GetTaskKind(method.ContainingType) != TaskKind.None)
			{
				replacements[returnStatement] =
					returnStatement.WithExpression(arguments[0].Expression.WithTriviaFrom(expression));
				continue;
			}

			ExpressionSyntax task = expression is InvocationExpressionSyntax or MemberAccessExpressionSyntax
				or IdentifierNameSyntax or ParenthesizedExpressionSyntax
				? expression
				: SyntaxFactory.ParenthesizedExpression(expression.WithoutTrivia()).WithTriviaFrom(expression);
			replacements[returnStatement] = returnStatement.WithExpression(Await(task));
		}

		return true;
	}

	private static TaskKind GetTaskKind(ITypeSymbol type)
		=> type.OriginalDefinition.ToDisplayString() switch
		{
			"System.Threading.Tasks.Task" or "System.Threading.Tasks.ValueTask" => TaskKind.NonGeneric,
			"System.Threading.Tasks.Task<TResult>" or "System.Threading.Tasks.ValueTask<TResult>" => TaskKind.Generic,
			_ => TaskKind.None,
		};

	private enum TaskKind
	{
		None,
		NonGeneric,
		Generic,
	}
}
