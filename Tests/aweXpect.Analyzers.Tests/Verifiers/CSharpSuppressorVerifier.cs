using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace aweXpect.Analyzers.Tests.Verifiers;

public static class CSharpSuppressorVerifier<TSuppressor>
	where TSuppressor : DiagnosticSuppressor, new()
{
	/// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])" />
	public static async Task VerifySuppressorAsync([StringSyntax("c#-test")] string source,
		params DiagnosticResult[] expected)
	{
		Test test = new()
		{
			TestCode = source,
			// Unlike the other verifiers, all warnings are validated below, so the reference assemblies must match
			// the ones that aweXpect.Core was compiled against to avoid CS1701 assembly binding warnings.
			ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
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

	public class Test : CSharpAnalyzerTest<TSuppressor, DefaultVerifier>
	{
		public Test()
		{
			// The nullability warnings must be validated as warnings, because a suppressor cannot suppress
			// diagnostics that are reported as errors.
			CompilerDiagnostics = CompilerDiagnostics.Warnings;

			SolutionTransforms.Add((solution, projectId) =>
			{
				Project? project = solution.GetProject(projectId);

				if (project?.CompilationOptions is not CSharpCompilationOptions compilationOptions ||
				    project.ParseOptions is not CSharpParseOptions parseOptions)
				{
					return solution;
				}

				return solution
					.WithProjectCompilationOptions(projectId,
						compilationOptions.WithNullableContextOptions(NullableContextOptions.Enable))
					.WithProjectParseOptions(projectId, parseOptions
						.WithLanguageVersion(LanguageVersion.Preview)
						// Missing XML comments (CS1591) are irrelevant for the test sources.
						.WithDocumentationMode(DocumentationMode.Parse));
			});
		}
	}
}
