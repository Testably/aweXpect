using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using aweXpect.Equivalency;

namespace aweXpect.Generators.Tests;

public sealed partial class MemberParityTests
{
	private static readonly (Type Type, string Name, string[] OwnMembers)[] CollectionTypes =
	[
		(typeof(Corpus.PlainList), "aweXpect.Generators.Tests.Corpus.PlainList", []),
		(typeof(Corpus.NamedList), "aweXpect.Generators.Tests.Corpus.NamedList", ["F:Tag", "P:Name",]),
		(typeof(Corpus.DescribedList), "aweXpect.Generators.Tests.Corpus.DescribedList",
			["F:Tag", "P:Name", "P:Description",]),
		(typeof(Corpus.RecountingList), "aweXpect.Generators.Tests.Corpus.RecountingList", ["P:Count",]),
		(typeof(Corpus.MiscountingQueue), "aweXpect.Generators.Tests.Corpus.MiscountingQueue", ["P:Offset",]),
		(typeof(Corpus.NamedLookup), "aweXpect.Generators.Tests.Corpus.NamedLookup", ["P:Name",]),
		(typeof(Corpus.Page<int>), "aweXpect.Generators.Tests.Corpus.Page<int>", ["P:Name",]),
		(typeof(Corpus.SelfMadeLookup), "aweXpect.Generators.Tests.Corpus.SelfMadeLookup", ["P:Name",]),
		(typeof(Corpus.ImplementingThroughItsBase), "aweXpect.Generators.Tests.Corpus.ImplementingThroughItsBase",
			["P:Total",]),
		(typeof(Corpus.VirtualPage), "aweXpect.Generators.Tests.Corpus.VirtualPage", ["P:Title",]),
		(typeof(Corpus.OverridingPage), "aweXpect.Generators.Tests.Corpus.OverridingPage", ["P:Extra", "P:Title",]),
		(typeof(Corpus.LabeledPair), "aweXpect.Generators.Tests.Corpus.LabeledPair", ["F:Label", "P:Sum",]),
		(typeof(Corpus.TitledSequence), "aweXpect.Generators.Tests.Corpus.TitledSequence", ["P:Title",]),
	];

	private static readonly Lazy<GeneratorRunner.GeneratorResult> CollectionResult = new(()
		=> GeneratorRunner.Run([Corpus(), CollectionAttributes(),]));

	private static readonly Lazy<GeneratorRunner.GeneratorResult> CollectionLibraryResult = new(()
		=> GeneratorRunner.Run([CollectionAttributes(),],
			additionalReferences: GeneratorRunner.CompileToReference("Corpus", Corpus())));

	private static string CollectionAttributes()
		=> string.Join(Environment.NewLine, CollectionTypes.Select(x
			=> $"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof({x.Name}))]"));

	public static IEnumerable<(Type, string, string[])> Collections
	{
		get
		{
			List<(Type, string, string[])> data = [];
			foreach ((Type type, string name, string[] ownMembers) in CollectionTypes)
			{
				data.Add((type, "global::" + name, ownMembers));
			}

			return data;
		}
	}

	[Test]
	public async Task GeneratedCollectionRegistrations_ShouldCompileWithoutWarnings()
	{
		await That(CollectionResult.Value.Errors).IsEmpty();
		await That(CollectionResult.Value.Warnings).IsEmpty();
		await That(CollectionResult.Value.GeneratorDiagnostics).HasSingle().Which
			.Satisfies(x => x.Id == "aweXpect2001" && x.GetMessage().Contains("Corpus.PlainList"))
			.Because("only the collection that declares no members itself yields no registration");
	}

	[Test]
	public async Task GeneratedCollectionRegistrations_WhenCorpusIsReferenced_ShouldCompileWithoutWarnings()
	{
		await That(CollectionLibraryResult.Value.Errors).IsEmpty();
		await That(CollectionLibraryResult.Value.Warnings).IsEmpty();
		await That(CollectionLibraryResult.Value.GeneratorDiagnostics).HasSingle();
	}

	[Test]
	[MethodDataSource(nameof(Collections))]
	public async Task RegisteredCollections_ShouldBeMarkedExactlyWhenTheyHaveOwnMembers(Type type, string key,
		string[] ownMembers)
	{
		string marker = $".RegisterCollection<{key}>();";

		await That(CollectionResult.Value.Generated.Contains(marker)).IsEqualTo(ownMembers.Length > 0)
			.Because($"the members registered for {type.Name} are only its own, which the marker tells the comparison");
	}

	[Test]
	[MethodDataSource(nameof(Collections))]
	public async Task RegisteredOwnMembers_ShouldMatchTheComparison(Type type, string key, string[] ownMembers)
	{
		Dictionary<string, HashSet<string>> registrations = Parse(CollectionResult.Value.Generated);
		IEnumerable<string> registered = registrations.TryGetValue(key, out HashSet<string>? members) ? members : [];

		await That(registered).IsEqualTo(ownMembers).InAnyOrder()
			.Because("only the members that the collection declares itself are registered");
		await That(await ComparedOwnMembers(type)).IsEqualTo(ownMembers).InAnyOrder()
			.Because("a registration that differs from the comparison would compare a different set of members under AOT");
	}

	[Test]
	[MethodDataSource(nameof(Collections))]
	public async Task RegisteredOwnMembers_WhenCorpusIsReferenced_ShouldMatchTheComparison(Type type, string key,
		string[] ownMembers)
	{
		Dictionary<string, HashSet<string>> registrations = Parse(CollectionLibraryResult.Value.Generated);
		IEnumerable<string> registered = registrations.TryGetValue(key, out HashSet<string>? members) ? members : [];

		await That(registered).IsEqualTo(ownMembers).InAnyOrder()
			.Because($"the members of {type.Name} are found the same in a referenced assembly");
	}

	/// <remarks>
	///     The oracle is the comparison itself, which reflects over the collection here: against a sequence without
	///     any members, it reports exactly the members it compares in addition to the items as missing.
	/// </remarks>
	private static async Task<IEnumerable<string>> ComparedOwnMembers(Type type)
	{
		object expected = Activator.CreateInstance(type)!;
		StringBuilder failureBuilder = new();

		await EquivalencyComparison.Compare<object, object>(new WithoutMembers(), expected, new EquivalencyOptions(),
			failureBuilder);

		return MissingMember().Matches(failureBuilder.ToString())
			.Select(match => (match.Groups[1].Value == "Field" ? "F:" : "P:") + match.Groups[2].Value)
			.ToList();
	}

	[GeneratedRegex("(Field|Property) (\\w+) was missing on the actual object")]
	private static partial Regex MissingMember();

	private sealed class WithoutMembers : IEnumerable
	{
		public IEnumerator GetEnumerator() => Array.Empty<object>().GetEnumerator();
	}
}
