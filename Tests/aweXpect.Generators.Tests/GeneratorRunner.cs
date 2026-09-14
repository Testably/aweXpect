using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using aweXpect.Core.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace aweXpect.Generators.Tests;

internal static class GeneratorRunner
{
	/// <summary>
	///     Runs the <see cref="TypeMetadataGenerator" /> over the <paramref name="sources" /> compiled against the
	///     runtime, <c>aweXpect</c> and <c>aweXpect.Core</c>.
	/// </summary>
	public static GeneratorResult Run(string[] sources, bool referenceCore = true,
		params MetadataReference[] additionalReferences)
	{
		List<SyntaxTree> trees = Parse(sources);
		CSharpCompilation compilation = Compile("GeneratorTests", trees,
			GetReferences(referenceCore).Concat(additionalReferences));

		GeneratorDriver driver = CSharpGeneratorDriver.Create(new TypeMetadataGenerator());
		driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation output,
			out ImmutableArray<Diagnostic> generatorDiagnostics);

		List<SyntaxTree> generatedTrees = output.SyntaxTrees.Skip(trees.Count).ToList();
		string generated = string.Concat(generatedTrees.Select(tree => tree.ToString()));
		ImmutableArray<Diagnostic> diagnostics = output.GetDiagnostics();
		return new GeneratorResult(generated,
			diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error).ToImmutableArray(),
			diagnostics.Where(x => x.Severity == DiagnosticSeverity.Warning &&
			                       x.Location.SourceTree is { } tree && generatedTrees.Contains(tree))
				.ToImmutableArray(),
			generatorDiagnostics);
	}

	/// <summary>
	///     Compiles the <paramref name="source" /> into an in-memory assembly that other compilations can reference.
	/// </summary>
	public static MetadataReference CompileToReference(string assemblyName, string source,
		params MetadataReference[] additionalReferences)
	{
		CSharpCompilation compilation = Compile(assemblyName, Parse([source,]),
			GetReferences(false).Concat(additionalReferences));
		using MemoryStream stream = new();
		EmitResult result = compilation.Emit(stream);
		if (!result.Success)
		{
			throw new InvalidOperationException(string.Join(Environment.NewLine, result.Diagnostics));
		}

		return MetadataReference.CreateFromImage(stream.ToArray());
	}

	private static List<SyntaxTree> Parse(string[] sources)
	{
		CSharpParseOptions parseOptions = new(LanguageVersion.Latest);
		return sources.Select(source => CSharpSyntaxTree.ParseText(source, parseOptions)).ToList();
	}

	private static CSharpCompilation Compile(string assemblyName, IEnumerable<SyntaxTree> trees,
		IEnumerable<MetadataReference> references)
		=> CSharpCompilation.Create(assemblyName, trees, references,
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
				nullableContextOptions: NullableContextOptions.Enable));

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

		paths.Remove(typeof(GeneratorRunner).Assembly.GetName().Name!);
		paths["aweXpect"] = typeof(EquivalencyExtensions).Assembly.Location;
		paths["aweXpect.Core"] = typeof(TypeMetadataRegistry).Assembly.Location;
		if (!referenceCore)
		{
			paths.Remove("aweXpect");
			paths.Remove("aweXpect.Core");
		}

		return paths.Values.Select(path => MetadataReference.CreateFromFile(path));
	}

	public readonly record struct GeneratorResult(
		string Generated,
		ImmutableArray<Diagnostic> Errors,
		ImmutableArray<Diagnostic> Warnings,
		ImmutableArray<Diagnostic> GeneratorDiagnostics);
}
