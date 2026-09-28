using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace aweXpect.Analyzers;

/// <summary>
///     An analyzer that checks that a delegate returning a <c>ValueTask</c> does not bind to the
///     <c>Expect.That</c> overload for a delegate with a return value, which never awaits it. This happens when no
///     dedicated <c>ValueTask</c> overload exists (the netstandard2.0 build) or the type argument is given explicitly.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValueTaskDelegateAnalyzer : DiagnosticAnalyzer
{
	/// <summary>
	///     The diagnostic property that is set when the delegate takes a <c>CancellationToken</c>, so that the code fix
	///     can pass it on.
	/// </summary>
	internal const string HasCancellationTokenProperty = "HasCancellationToken";

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [Rules.ValueTaskDelegateRule,];

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterOperationAction(AnalyzeOperation, OperationKind.Invocation);
	}

	private static void AnalyzeOperation(OperationAnalysisContext context)
	{
		if (context.Operation is not IInvocationOperation invocation ||
		    !IsExpectThatForADelegateWithValue(invocation.TargetMethod, out bool hasCancellationToken) ||
		    !IsValueTask(invocation.TargetMethod.TypeArguments[0]) ||
		    invocation.Arguments.FirstOrDefault(a => a.Parameter?.Ordinal == 0) is not { } argument)
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(Rules.ValueTaskDelegateRule,
			argument.Value.Syntax.GetLocation(),
			hasCancellationToken
				? ImmutableDictionary<string, string?>.Empty.Add(HasCancellationTokenProperty, null)
				: ImmutableDictionary<string, string?>.Empty,
			invocation.TargetMethod.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
	}

	/// <summary>
	///     Whether the <paramref name="method" /> is <c>Expect.That&lt;TValue&gt;(Func&lt;TValue&gt;)</c> or
	///     <c>Expect.That&lt;TValue&gt;(Func&lt;CancellationToken, TValue&gt;)</c>.
	/// </summary>
	private static bool IsExpectThatForADelegateWithValue(IMethodSymbol method, out bool hasCancellationToken)
	{
		hasCancellationToken = false;
		IMethodSymbol declaration = method.OriginalDefinition;
		if (declaration is not
		    {
			    Name: "That", Arity: 1,
			    ContainingType: { Name: "Expect", ContainingType: null, ContainingNamespace.Name: "aweXpect", },
		    } ||
		    declaration.ContainingType.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace != true ||
		    declaration.Parameters.FirstOrDefault()?.Type is not INamedTypeSymbol
		    {
			    Name: "Func", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true, },
		    } func ||
		    !SymbolEqualityComparer.Default.Equals(func.TypeArguments.Last(), declaration.TypeParameters[0]))
		{
			return false;
		}

		hasCancellationToken = func.Arity == 2 && IsCancellationToken(func.TypeArguments[0]);
		return func.Arity == 1 || hasCancellationToken;
	}

	private static bool IsCancellationToken(ITypeSymbol type)
		=> type is INamedTypeSymbol { Name: "CancellationToken", Arity: 0, } &&
		   IsInSystemThreading(type.ContainingNamespace);

	private static bool IsValueTask(ITypeSymbol type)
		=> type is INamedTypeSymbol { Name: "ValueTask", Arity: 0 or 1, ContainingNamespace.Name: "Tasks", } &&
		   IsInSystemThreading(type.ContainingNamespace.ContainingNamespace);

	private static bool IsInSystemThreading(INamespaceSymbol? @namespace)
		=> @namespace is { Name: "Threading", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true, }, };
}
