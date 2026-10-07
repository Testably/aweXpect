using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace aweXpect.Analyzers.Tests.Verifiers;

public static partial class CSharpAnalyzerVerifier<TAnalyzer>
	where TAnalyzer : DiagnosticAnalyzer, new()
{
	/// <inheritdoc cref="Microsoft.CodeAnalysis.Diagnostic" />
	public static DiagnosticResult Diagnostic()
		=> CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic();

	/// <inheritdoc cref="Microsoft.CodeAnalysis.Diagnostic" />
	public static DiagnosticResult Diagnostic(string diagnosticId)
		=> CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(diagnosticId);

	/// <inheritdoc cref="Microsoft.CodeAnalysis.Diagnostic" />
	public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
		=> CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(descriptor);

	/// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])" />
	public static async Task VerifyAnalyzerAsync([StringSyntax("c#-test")] string source,
		params DiagnosticResult[] expected)
	{
		Test test = new()
		{
			TestCode = source,
			ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
			TestState =
			{
				AdditionalReferences =
				{
					typeof(Expect).Assembly.Location,
					typeof(ThatBool).Assembly.Location,
				},
			},
		};

		test.ExpectedDiagnostics.AddRange(expected);
		await test.RunAsync(CancellationToken.None);
	}
}
