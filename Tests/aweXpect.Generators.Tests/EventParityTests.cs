using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace aweXpect.Generators.Tests;

public sealed partial class EventParityTests
{
	private static readonly (Type Type, string Name)[] CorpusTypes =
	[
		(typeof(Corpus.Publisher), "aweXpect.Generators.Tests.Corpus.Publisher"),
		(typeof(Corpus.PublisherDerived), "aweXpect.Generators.Tests.Corpus.PublisherDerived"),
		(typeof(Corpus.PublisherHiding), "aweXpect.Generators.Tests.Corpus.PublisherHiding"),
		(typeof(Corpus.PublisherHidingPrivately), "aweXpect.Generators.Tests.Corpus.PublisherHidingPrivately"),
		(typeof(Corpus.PublisherHidingPrivatelyDerived),
			"aweXpect.Generators.Tests.Corpus.PublisherHidingPrivatelyDerived"),
		(typeof(Corpus.PublisherHidingInternally), "aweXpect.Generators.Tests.Corpus.PublisherHidingInternally"),
		(typeof(Corpus.PublisherHidingByProperty), "aweXpect.Generators.Tests.Corpus.PublisherHidingByProperty"),
		(typeof(Corpus.PublisherWithExplicitInterface),
			"aweXpect.Generators.Tests.Corpus.PublisherWithExplicitInterface"),
		(typeof(Corpus.PublisherWithKeywords), "aweXpect.Generators.Tests.Corpus.PublisherWithKeywords"),
		(typeof(Corpus.PublisherWithObsolete), "aweXpect.Generators.Tests.Corpus.PublisherWithObsolete"),
		(typeof(Corpus.GenericPublisher<int>), "aweXpect.Generators.Tests.Corpus.GenericPublisher<int>"),
		(typeof(Corpus.GenericPublisherDerived), "aweXpect.Generators.Tests.Corpus.GenericPublisherDerived"),
	];

	private static readonly Lazy<GeneratorRunner.GeneratorResult> Result = new(()
		=> GeneratorRunner.Run([Corpus(), Attributes(),]));

	/// <remarks>
	///     A subject usually lives in the assembly under test, whose non-public members Roslyn does not import by
	///     default, so the corpus is also run as a referenced library.
	/// </remarks>
	private static readonly Lazy<GeneratorRunner.GeneratorResult> LibraryResult = new(()
		=> GeneratorRunner.Run([Attributes(),],
			additionalReferences: GeneratorRunner.CompileToReference("Corpus", Corpus())));

	private static string Corpus()
	{
		using Stream stream = typeof(EventParityTests).Assembly.GetManifestResourceStream("Corpus.cs")!;
		using StreamReader reader = new(stream);
		return reader.ReadToEnd();
	}

	private static string Attributes()
		=> string.Join(Environment.NewLine, CorpusTypes.Select(x
			=> $"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof({x.Name}))]"));

	public static TheoryData<Type, string> Types
	{
		get
		{
			TheoryData<Type, string> data = new();
			foreach ((Type type, string name) in CorpusTypes)
			{
				data.Add(type, "events of global::" + name);
			}

			return data;
		}
	}

	[Fact]
	public async Task GeneratedRegistrations_ShouldCompileWithoutWarnings()
	{
		await That(Result.Value.Errors).IsEmpty();
		await That(Result.Value.Warnings).IsEmpty();
		await That(Result.Value.GeneratorDiagnostics).IsEmpty()
			.Because("every corpus type is meant to be registered");
	}

	[Fact]
	public async Task GeneratedRegistrations_WhenCorpusIsReferenced_ShouldCompileWithoutWarnings()
	{
		await That(LibraryResult.Value.Errors).IsEmpty();
		await That(LibraryResult.Value.Warnings).IsEmpty();
		await That(LibraryResult.Value.GeneratorDiagnostics).IsEmpty();
	}

	[Theory]
	[MemberData(nameof(Types))]
	public async Task RegisteredEvents_ShouldMatchReflection(Type type, string key)
	{
		Dictionary<string, HashSet<string>> registrations = Parse(Result.Value.Generated);

		await That(registrations).ContainsKey(key)
			.Because("every corpus type has public events, so reflection would return them");
		await That(registrations[key]).IsEqualTo(ReflectedEvents(type)).InAnyOrder()
			.Because("a registration that differs from reflection would record a different set of events under AOT");
	}

	[Theory]
	[MemberData(nameof(Types))]
	public async Task RegisteredEvents_WhenCorpusIsReferenced_ShouldMatchReflection(Type type, string key)
	{
		Dictionary<string, HashSet<string>> registrations = Parse(LibraryResult.Value.Generated);

		await That(registrations).ContainsKey(key);
		await That(registrations[key]).IsEqualTo(ReflectedEvents(type)).InAnyOrder()
			.Because("a non-public hiding declaration in a referenced assembly is invisible to the default import");
	}

	/// <remarks>
	///     The oracle is what <c>EventRecording</c> reflects over: <c>Type.GetEvents()</c> with its default binding
	///     flags, which return public instance and static events, hidden by name.
	/// </remarks>
	private static IEnumerable<string> ReflectedEvents(Type type)
		=> type.GetEvents().Select(x => x.Name);

	private static Dictionary<string, HashSet<string>> Parse(string generated)
	{
		Dictionary<string, HashSet<string>> result = new(StringComparer.Ordinal);
		HashSet<string>? current = null;
		foreach (string rawLine in generated.Split('\n'))
		{
			string line = rawLine.TrimEnd('\r').TrimStart('\t');
			if (line.StartsWith("// ", StringComparison.Ordinal))
			{
				current = line.StartsWith("// events of ", StringComparison.Ordinal) ? [] : null;
				if (current is not null)
				{
					result[line.Substring(3)] = current;
				}

				continue;
			}

			Match match = Registration().Match(line);
			if (match.Success && current is not null)
			{
				current.Add(match.Groups[1].Value);
			}
		}

		return result;
	}

	[GeneratedRegex("\\.RegisterEvent<.*>\\(\"(\\w+)\",")]
	private static partial Regex Registration();
}
