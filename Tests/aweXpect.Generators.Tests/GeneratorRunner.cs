using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using aweXpect.Core.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.Generators.Tests;

internal static class GeneratorRunner
{
	/// <summary>
	///     Runs the <see cref="TypeMetadataGenerator" /> over the <paramref name="sources" /> compiled against the
	///     runtime, <c>aweXpect</c> and <c>aweXpect.Core</c>.
	/// </summary>
	public static GeneratorResult Run(bool referenceCore = true, params string[] sources)
	{
		CSharpParseOptions parseOptions = new(LanguageVersion.Latest);
		List<SyntaxTree> trees = sources.Select(source => CSharpSyntaxTree.ParseText(source, parseOptions)).ToList();
		CSharpCompilation compilation = CSharpCompilation.Create("GeneratorTests", trees, GetReferences(referenceCore),
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
				nullableContextOptions: NullableContextOptions.Enable));

		GeneratorDriver driver = CSharpGeneratorDriver.Create(new TypeMetadataGenerator());
		driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation output, out _);

		string generated = string.Concat(output.SyntaxTrees.Skip(trees.Count).Select(tree => tree.ToString()));
		ImmutableArray<Diagnostic> errors = output.GetDiagnostics()
			.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
			.ToImmutableArray();
		return new GeneratorResult(generated, errors);
	}

	/// <remarks>
	///     The trusted platform assemblies already contain the test's own copies of <c>aweXpect</c>, so the explicit
	///     references only make sure they are present when the compilation runs elsewhere.
	/// </remarks>
	private static IEnumerable<MetadataReference> GetReferences(bool referenceCore)
	{
		Dictionary<string, string> paths = new(StringComparer.OrdinalIgnoreCase);
		foreach (string path in ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator))
		{
			paths[Path.GetFileNameWithoutExtension(path)] = path;
		}

		paths["aweXpect"] = typeof(EquivalencyExtensions).Assembly.Location;
		paths["aweXpect.Core"] = typeof(TypeMetadataRegistry).Assembly.Location;
		if (!referenceCore)
		{
			paths.Remove("aweXpect");
			paths.Remove("aweXpect.Core");
		}

		return paths.Values.Select(path => MetadataReference.CreateFromFile(path));
	}

	public readonly record struct GeneratorResult(string Generated, ImmutableArray<Diagnostic> Errors);
}
