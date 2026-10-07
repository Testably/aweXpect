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
	public static GeneratorResult Run(string[] sources, bool referenceCore = true,
		LanguageVersion languageVersion = LanguageVersion.Latest, params MetadataReference[] additionalReferences)
		=> Run(new TypeMetadataGenerator(), sources, referenceCore, languageVersion, additionalReferences);

	public static GeneratorResult Run(IIncrementalGenerator generator, string[] sources, bool referenceCore = true,
		LanguageVersion languageVersion = LanguageVersion.Latest, params MetadataReference[] additionalReferences)
	{
		CSharpParseOptions parseOptions = new(languageVersion);
		List<SyntaxTree> trees = Parse(sources, parseOptions);
		CSharpCompilation compilation = Compile("GeneratorTests", trees,
			GetReferences(referenceCore).Concat(additionalReferences),
			parseOptions.LanguageVersion >= LanguageVersion.CSharp8);

		GeneratorDriver driver = CSharpGeneratorDriver.Create([generator.AsSourceGenerator(),],
			parseOptions: parseOptions);
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
	///     Runs the <paramref name="generator" /> on the <paramref name="sources" />, then again after adding the
	///     <paramref name="addedSource" />, and returns the tracked result of the second run.
	/// </summary>
	public static GeneratorDriverRunResult RunTwice(IIncrementalGenerator generator, string[] sources,
		string addedSource, bool referenceCore = true)
	{
		CSharpParseOptions parseOptions = new(LanguageVersion.Latest);
		CSharpCompilation compilation = Compile("GeneratorTests", Parse(sources, parseOptions),
			GetReferences(referenceCore));
		GeneratorDriver driver = CSharpGeneratorDriver.Create([generator.AsSourceGenerator(),],
			parseOptions: parseOptions,
			driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, true));
		driver = driver.RunGenerators(compilation);
		return driver.RunGenerators(compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(addedSource, parseOptions)))
			.GetRunResult();
	}

	public static MetadataReference CompileToReference(string assemblyName, string source,
		params MetadataReference[] additionalReferences)
	{
		CSharpCompilation compilation = Compile(assemblyName,
			Parse([source,], new CSharpParseOptions(LanguageVersion.Latest)),
			GetReferences(false).Concat(additionalReferences));
		using MemoryStream stream = new();
		EmitResult result = compilation.Emit(stream);
		if (!result.Success)
		{
			throw new InvalidOperationException(string.Join(Environment.NewLine, result.Diagnostics));
		}

		return MetadataReference.CreateFromImage(stream.ToArray());
	}

	/// <summary>
	///     The source of <see cref="Corpus" />, preceded by the global usings of <c>Usings.cs</c> that it relies on.
	/// </summary>
	/// <remarks>
	///     The corpus is also compiled on its own, where the global usings of this project are missing, and a code
	///     cleanup removes the usings and namespaces in it that these global usings make redundant.
	/// </remarks>
	public static string CorpusSource()
	{
		using Stream stream = typeof(GeneratorRunner).Assembly.GetManifestResourceStream("Corpus.cs")!;
		using StreamReader reader = new(stream);
		return "global using System;" + Environment.NewLine +
		       "global using System.Threading.Tasks;" + Environment.NewLine +
		       reader.ReadToEnd();
	}

	public static CSharpCompilation CreateCompilation(string[] sources, bool referenceCore = true)
		=> Compile("GeneratorTests", Parse(sources, new CSharpParseOptions(LanguageVersion.Latest)),
			GetReferences(referenceCore));

	private static List<SyntaxTree> Parse(string[] sources, CSharpParseOptions parseOptions)
		=> sources.Select(source => CSharpSyntaxTree.ParseText(source, parseOptions)).ToList();

	/// <remarks>
	///     C# 7.3 rejects a nullable context, so it is only enabled where the language supports it.
	/// </remarks>
	private static CSharpCompilation Compile(string assemblyName, IEnumerable<SyntaxTree> trees,
		IEnumerable<MetadataReference> references, bool supportsNullable = true)
		=> CSharpCompilation.Create(assemblyName, trees, references,
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
				nullableContextOptions: supportsNullable
					? NullableContextOptions.Enable
					: NullableContextOptions.Disable));

	/// <remarks>
	///     The trusted platform assemblies include this test assembly, whose copy of the corpus would clash with the
	///     corpus source fed to the generator, so it is left out. The test framework that runs these tests is left
	///     out as well, so that a generator only sees the frameworks a test adds as a reference.
	/// </remarks>
	private static IEnumerable<MetadataReference> GetReferences(bool referenceCore)
	{
		Dictionary<string, string> paths = new(StringComparer.OrdinalIgnoreCase);
		foreach (string path in ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator))
		{
			paths[Path.GetFileNameWithoutExtension(path)] = path;
		}

		paths.Remove(typeof(GeneratorRunner).Assembly.GetName().Name!);
		foreach (string testFramework in paths.Keys.Where(name => name.StartsWith("TUnit", StringComparison.Ordinal))
			         .ToList())
		{
			paths.Remove(testFramework);
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

	public readonly record struct GeneratorResult(
		string Generated,
		ImmutableArray<Diagnostic> Errors,
		ImmutableArray<Diagnostic> Warnings,
		ImmutableArray<Diagnostic> GeneratorDiagnostics);
}
