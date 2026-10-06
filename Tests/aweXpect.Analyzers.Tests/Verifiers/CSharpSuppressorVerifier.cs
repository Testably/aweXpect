using System.Diagnostics.CodeAnalysis;
using System.Threading;
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
	public static Task VerifySuppressorAsync([StringSyntax("c#-test")] string source,
		params DiagnosticResult[] expected)
		=> VerifySuppressorAsync(OutputKind.DynamicallyLinkedLibrary, source, expected);

	/// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])" />
	public static async Task VerifySuppressorAsync(OutputKind outputKind, [StringSyntax("c#-test")] string source,
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
				OutputKind = outputKind,
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

	/// <summary>
	///     Verifies the suppressions in the <paramref name="source" />, which references a separate extension assembly
	///     compiled from the <paramref name="extensionSource" />.
	/// </summary>
	public static async Task VerifySuppressorWithExtensionAsync([StringSyntax("c#-test")] string source,
		[StringSyntax("c#-test")] string extensionSource, params DiagnosticResult[] expected)
	{
		Test test = new()
		{
			TestCode = source,
			ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
			TestState =
			{
				AdditionalReferences =
				{
					typeof(Expect).Assembly.Location,
					typeof(ThatBool).Assembly.Location,
				},
				AdditionalProjectReferences =
				{
					"Extension",
				},
			},
		};
		test.TestState.AdditionalProjects["Extension"].Sources.Add(extensionSource);
		test.TestState.AdditionalProjects["Extension"].AdditionalReferences.Add(typeof(Expect).Assembly.Location);
		test.TestState.AdditionalProjects["Extension"].AdditionalReferences.Add(typeof(ThatBool).Assembly.Location);

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

			// Also applied to the additional projects, e.g. the extension assembly of a test.
			SolutionTransforms.Add((solution, _) =>
			{
				foreach (Project project in solution.Projects)
				{
					if (project.CompilationOptions is not CSharpCompilationOptions compilationOptions ||
					    project.ParseOptions is not CSharpParseOptions parseOptions)
					{
						continue;
					}

					solution = solution
						.WithProjectCompilationOptions(project.Id,
							compilationOptions.WithNullableContextOptions(NullableContextOptions.Enable))
						.WithProjectParseOptions(project.Id, parseOptions
							.WithLanguageVersion(LanguageVersion.Preview)
							// Missing XML comments (CS1591) are irrelevant for the test sources.
							.WithDocumentationMode(DocumentationMode.Parse));
				}

				return solution;
			});
		}
	}
}
