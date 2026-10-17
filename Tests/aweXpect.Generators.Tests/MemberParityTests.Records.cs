using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace aweXpect.Generators.Tests;

public sealed partial class MemberParityTests
{
	private static readonly (Type Type, string Name, bool IsCompilerGeneratedRecord)[] RecordTypes =
	[
		(typeof(Corpus.PositionalRecord), "aweXpect.Generators.Tests.Corpus.PositionalRecord", true),
		(typeof(Corpus.DerivedRecord), "aweXpect.Generators.Tests.Corpus.DerivedRecord", true),
		(typeof(Corpus.GenericRecord<int>), "aweXpect.Generators.Tests.Corpus.GenericRecord<int>", true),
		(typeof(Corpus.RecordStruct), "aweXpect.Generators.Tests.Corpus.RecordStruct", true),
		(typeof(Corpus.ReadOnlyRecordStruct), "aweXpect.Generators.Tests.Corpus.ReadOnlyRecordStruct", true),
		(typeof(Corpus.WithOwnPrintMembers), "aweXpect.Generators.Tests.Corpus.WithOwnPrintMembers", false),
		(typeof(Corpus.InheritingOwnPrintMembers), "aweXpect.Generators.Tests.Corpus.InheritingOwnPrintMembers",
			true),
		(typeof(Corpus.SealedWithOwnPrintMembers), "aweXpect.Generators.Tests.Corpus.SealedWithOwnPrintMembers",
			false),
		(typeof(Corpus.WithSealedToString), "aweXpect.Generators.Tests.Corpus.WithSealedToString", false),
		(typeof(Corpus.InheritingSealedToString), "aweXpect.Generators.Tests.Corpus.InheritingSealedToString",
			false),
		(typeof(Corpus.RecordStructWithOwnToString), "aweXpect.Generators.Tests.Corpus.RecordStructWithOwnToString",
			false),
		(typeof(Corpus.RecordLookalike), "aweXpect.Generators.Tests.Corpus.RecordLookalike", true),
		(typeof(Corpus.Base), "aweXpect.Generators.Tests.Corpus.Base", false),
	];

	private static readonly Lazy<GeneratorRunner.GeneratorResult> RecordResult = new(()
		=> GeneratorRunner.Run([GeneratorRunner.CorpusSource(), RecordAttributes(),]));

	private static readonly Lazy<GeneratorRunner.GeneratorResult> RecordLibraryResult = new(()
		=> GeneratorRunner.Run([RecordAttributes(),],
			additionalReferences: GeneratorRunner.CompileToReference("Corpus", GeneratorRunner.CorpusSource())));

	public static IEnumerable<(Type, string, bool)> Records
	{
		get
		{
			List<(Type, string, bool)> data = [];
			foreach ((Type type, string name, bool isCompilerGeneratedRecord) in RecordTypes)
			{
				data.Add((type, "global::" + name, isCompilerGeneratedRecord));
			}

			return data;
		}
	}

	private static string RecordAttributes()
		=> string.Join(Environment.NewLine, RecordTypes.Select(x
			=> $"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof({x.Name}))]"));

	[Test]
	public async Task GeneratedRecordRegistrations_ShouldCompileWithoutWarnings()
	{
		await That(RecordResult.Value.Errors).IsEmpty();
		await That(RecordResult.Value.Warnings).IsEmpty();
		await That(RecordResult.Value.GeneratorDiagnostics).IsEmpty();
	}

	[Test]
	public async Task GeneratedRecordRegistrations_WhenCorpusIsReferenced_ShouldCompileWithoutWarnings()
	{
		await That(RecordLibraryResult.Value.Errors).IsEmpty();
		await That(RecordLibraryResult.Value.Warnings).IsEmpty();
		await That(RecordLibraryResult.Value.GeneratorDiagnostics).IsEmpty();
	}

	[Test]
	[MethodDataSource(nameof(Records))]
	public async Task RegisteredRecords_ShouldBeMarkedExactlyWhenReflectionRecognizesThem(Type type, string key,
		bool isCompilerGeneratedRecord)
	{
		string marker = $".RegisterRecord<{key}>();";

		await That(IsReflectedAsCompilerGeneratedRecord(type)).IsEqualTo(isCompilerGeneratedRecord);
		await That(RecordResult.Value.Generated.Contains(marker)).IsEqualTo(isCompilerGeneratedRecord)
			.Because($"the formatter has to write {type.Name} the same way with and without reflection");
	}

	[Test]
	[MethodDataSource(nameof(Records))]
	public async Task RegisteredRecords_WhenCorpusIsReferenced_ShouldBeMarkedExactlyWhenReflectionRecognizesThem(
		Type type, string key, bool isCompilerGeneratedRecord)
	{
		string marker = $".RegisterRecord<{key}>();";

		await That(RecordLibraryResult.Value.Generated.Contains(marker)).IsEqualTo(isCompilerGeneratedRecord)
			.Because($"the compiler marks what it generated for {type.Name} with an attribute in a referenced assembly, and a private PrintMembers is not imported by default");
	}

	/// <remarks>
	///     The oracle is what <c>ValueFormatters.IsCompilerGeneratedRecord</c> reflects over.
	/// </remarks>
	private static bool IsReflectedAsCompilerGeneratedRecord(Type type)
	{
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		MethodInfo? toString = type.GetMethod(nameof(ToString), flags, null, Type.EmptyTypes, null);
		MethodInfo? printMembers = type.GetMethod("PrintMembers", flags, null, [typeof(StringBuilder),], null);
		return toString?.IsDefined(typeof(CompilerGeneratedAttribute), false) == true &&
		       printMembers?.IsDefined(typeof(CompilerGeneratedAttribute), false) == true;
	}
}
