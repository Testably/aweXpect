using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using aweXpect.Core.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace aweXpect.Generators.Tests;

public sealed class ExtensionRegistrationGeneratorTests
{
	private const string Initializer = """
	                                   namespace MusicStore
	                                   {
	                                   	public static class Catalog
	                                   	{
	                                   		public static void Register() { }
	                                   	}
	                                   }
	                                   """;

	private const string Metadata = """
	                                [assembly: System.Reflection.AssemblyMetadata("MusicStore.Album", "Abbey Road")]
	                                [assembly: System.Reflection.AssemblyMetadata("MusicStore.Album", "Let \"It\" Be")]
	                                [assembly: System.Reflection.AssemblyMetadata("MusicStore.Artist", "The Beatles")]
	                                [assembly: System.Reflection.AssemblyMetadata("MusicStore.Label", null)]
	                                """;

	[Test]
	public async Task ShouldCallAnInitializerOfANestedType()
	{
		Result result = Run([
			"""
			namespace MusicStore
			{
				public static class Catalog
				{
					public static class Albums
					{
						public static void Register() { }
					}
				}
			}
			""",
		], new Dictionary<string, string>
		{
			["AweXpectModuleInitializers"] = "MusicStore.Catalog.Albums.Register",
		});

		await That(result.Errors).IsEmpty();
		await That(result.Diagnostics).IsEmpty();
		await That(result.Registrations).IsEqualTo(["global::MusicStore.Catalog.Albums.Register();",])
			.Because("the generated call names the nested type the same way as the property");
	}

	[Test]
	public async Task ShouldCallTheInitializersAfterRegisteringTheValues()
	{
		Result result = Run([Metadata, Initializer,], new Dictionary<string, string>
		{
			["AweXpectRegisteredMetadataKeys"] = "MusicStore.Artist",
			["AweXpectModuleInitializers"] = "MusicStore.Catalog.Register",
		});

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("""
		                                      			global::aweXpect.Core.Extending.ExtensionRegistry.Register("MusicStore.Artist", "The Beatles");
		                                      			global::MusicStore.Catalog.Register();
		                                      """.Replace("\r\n", "\n").Replace("\n", Environment.NewLine))
			.Because("an initializer can then read the registered values");
	}

	[Test]
	public async Task ShouldEmitAModuleInitializer()
	{
		Result result = Run([Metadata,], new Dictionary<string, string>
		{
			["AweXpectRegisteredMetadataKeys"] = "MusicStore.Artist",
		});

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("[global::System.Runtime.CompilerServices.ModuleInitializer]")
			.Because("the values have to be registered before any code of the test assembly runs");
	}

	[Test]
	public async Task ShouldRegisterTheValuesOfTheListedKeysInTheirOrder()
	{
		Result result = Run([Metadata,], new Dictionary<string, string>
		{
			["AweXpectRegisteredMetadataKeys"] = " MusicStore.Album , ,MusicStore.Label,MusicStore.Album",
		});

		await That(result.Errors).IsEmpty();
		await That(result.Registrations).IsEqualTo([
			"global::aweXpect.Core.Extending.ExtensionRegistry.Register(\"MusicStore.Album\", \"Abbey Road\");",
			"global::aweXpect.Core.Extending.ExtensionRegistry.Register(\"MusicStore.Album\", \"Let \\\"It\\\" Be\");",
		]).Because("the keys are trimmed and listed once, other keys and missing values are left out, and the values are escaped");
	}

	[Test]
	public async Task WhenAnInitializerCannotBeCalled_ShouldReportItAndLeaveItOut()
	{
		Result result = Run([
			"""
			namespace MusicStore
			{
				public static class Catalog
				{
					public static void Register() { }
					public static void Add(string album) { }
					private static void Clear() { }
					public static void Generic<T>() { }
				}

				public static class Shelf<T>
				{
					public static void Register() { }
				}
			}
			""",
		], new Dictionary<string, string>
		{
			["AweXpectModuleInitializers"] =
				"MusicStore.Catalog.Missing,MusicStore.Catalog.Add,MusicStore.Catalog.Clear,MusicStore.Catalog.Generic,MusicStore.Shelf.Register,MusicStore..Register,MusicStore.Catalog.Register",
		});

		await That(result.Errors).IsEmpty()
			.Because("an initializer that cannot be called is not emitted, so the build only fails with the reported error");
		await That(result.Diagnostics).IsEqualTo([
			"aweXpect4005: 'MusicStore.Catalog.Missing' in the MSBuild property AweXpectModuleInitializers names no accessible static method without parameters",
			"aweXpect4005: 'MusicStore.Catalog.Add' in the MSBuild property AweXpectModuleInitializers names no accessible static method without parameters",
			"aweXpect4005: 'MusicStore.Catalog.Clear' in the MSBuild property AweXpectModuleInitializers names no accessible static method without parameters",
			"aweXpect4005: 'MusicStore.Catalog.Generic' in the MSBuild property AweXpectModuleInitializers names no accessible static method without parameters",
			"aweXpect4005: 'MusicStore.Shelf.Register' in the MSBuild property AweXpectModuleInitializers names no accessible static method without parameters",
			"aweXpect4005: 'MusicStore..Register' in the MSBuild property AweXpectModuleInitializers names no accessible static method without parameters",
		]);
		await That(result.Registrations).IsEqualTo(["global::MusicStore.Catalog.Register();",]);
	}

	[Test]
	public async Task WhenConsumerDeclaresTheModuleInitializerAsPolyfill_ShouldEmit()
	{
		Result result = Run(
		[
			"namespace System.Runtime.CompilerServices { internal sealed class ModuleInitializerAttribute : Attribute { } }",
			Metadata,
		], new Dictionary<string, string>
		{
			["AweXpectRegisteredMetadataKeys"] = "MusicStore.Artist",
		});

		await That(result.Errors).IsEmpty();
		await That(result.Registrations).HasSingle()
			.Because("an internal polyfill in the consumer's own assembly can be applied by the generated code");
	}

	[Test]
	public async Task WhenConsumerUsesCSharp8_ShouldNotEmit()
	{
		Result result = Run([Metadata, Initializer,], new Dictionary<string, string>
		{
			["AweXpectRegisteredMetadataKeys"] = "MusicStore.Artist",
			["AweXpectModuleInitializers"] = "MusicStore.Catalog.Register",
		}, LanguageVersion.CSharp8);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).IsEmpty()
			.Because("a module initializer needs C# 9, so emitting one would break the consumer's build");
	}

	[Test]
	public async Task WhenConsumerUsesCSharp9_ShouldCompile()
	{
		Result result = Run([Metadata, Initializer,], new Dictionary<string, string>
		{
			["AweXpectRegisteredMetadataKeys"] = "MusicStore.Artist",
			["AweXpectModuleInitializers"] = "MusicStore.Catalog.Register",
		}, LanguageVersion.CSharp9);

		await That(result.Errors).IsEmpty()
			.Because("the generated file must not use anything newer than the module initializer itself");
		await That(result.Registrations).HasCount(2);
	}

	[Test]
	public async Task WhenNothingIsListed_ShouldNotEmit()
	{
		Result result = Run([Metadata, Initializer,], new Dictionary<string, string>
		{
			["AweXpectRegisteredMetadataKeys"] = " , ",
		});

		await That(result.Errors).IsEmpty();
		await That(result.Generated).IsEmpty()
			.Because("a test project without an extension that uses the hook must not get a module initializer");
	}

	private static Result Run(string[] sources, Dictionary<string, string> properties,
		LanguageVersion languageVersion = LanguageVersion.Latest)
	{
		CSharpParseOptions parseOptions = new(languageVersion);
		List<SyntaxTree> trees = sources.Select(source => CSharpSyntaxTree.ParseText(source, parseOptions)).ToList();
		CSharpCompilation compilation = GeneratorRunner.CreateCompilation([]).AddSyntaxTrees(trees);

		GeneratorDriver driver = CSharpGeneratorDriver.Create(
			[new ExtensionRegistrationGenerator().AsSourceGenerator(),],
			parseOptions: parseOptions,
			optionsProvider: new BuildProperties(properties));
		driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation output,
			out ImmutableArray<Diagnostic> diagnostics);

		string generated = string.Concat(output.SyntaxTrees.Skip(compilation.SyntaxTrees.Length)
			.Select(tree => tree.ToString()));
		return new Result(generated,
			output.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).ToImmutableArray(),
			generated.Split('\n').Select(line => line.Trim())
				.Where(line => line.StartsWith("global::", StringComparison.Ordinal)).ToArray(),
			diagnostics.Select(x => $"{x.Id}: {x.GetMessage(CultureInfo.InvariantCulture)}").ToArray());
	}

	private readonly record struct Result(
		string Generated,
		ImmutableArray<Diagnostic> Errors,
		string[] Registrations,
		string[] Diagnostics);

	private sealed class BuildProperties(Dictionary<string, string> properties) : AnalyzerConfigOptionsProvider
	{
		public override AnalyzerConfigOptions GlobalOptions { get; } = new Options(properties);

		public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => new Options([]);

		public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => new Options([]);

		private sealed class Options(Dictionary<string, string> properties) : AnalyzerConfigOptions
		{
			public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
				=> properties.TryGetValue(key.Replace("build_property.", ""), out value);
		}
	}
}
