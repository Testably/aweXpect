using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace aweXpect.Generators.Tests;

/// <summary>
///     Under the JIT both paths are available, so the members the generator registers for the <see cref="Corpus" />
///     types are compared against the members reflection returns for the very same types.
/// </summary>
public sealed class MemberParityTests
{
	private static readonly (Type Type, string Name)[] CorpusTypes =
	[
		(typeof(Corpus.Base), "aweXpect.Generators.Tests.Corpus.Base"),
		(typeof(Corpus.Derived), "aweXpect.Generators.Tests.Corpus.Derived"),
		(typeof(Corpus.Shadowing), "aweXpect.Generators.Tests.Corpus.Shadowing"),
		(typeof(Corpus.WithIndexer), "aweXpect.Generators.Tests.Corpus.WithIndexer"),
		(typeof(Corpus.WithWriteOnly), "aweXpect.Generators.Tests.Corpus.WithWriteOnly"),
		(typeof(Corpus.WithStatics), "aweXpect.Generators.Tests.Corpus.WithStatics"),
		(typeof(Corpus.Generic<int>), "aweXpect.Generators.Tests.Corpus.Generic<int>"),
		(typeof(Corpus.WithExplicitInterface), "aweXpect.Generators.Tests.Corpus.WithExplicitInterface"),
		(typeof(Corpus.PositionalRecord), "aweXpect.Generators.Tests.Corpus.PositionalRecord"),
		(typeof(Corpus.Point), "aweXpect.Generators.Tests.Corpus.Point"),
		(typeof(Corpus.WithInitOnly), "aweXpect.Generators.Tests.Corpus.WithInitOnly"),
		(typeof(Corpus.WithVisibilities), "aweXpect.Generators.Tests.Corpus.WithVisibilities"),
	];

	private static readonly Lazy<GeneratorRunner.GeneratorResult> Result = new(() =>
	{
		using Stream stream = typeof(MemberParityTests).Assembly.GetManifestResourceStream("Corpus.cs")!;
		using StreamReader reader = new(stream);
		string attributes = string.Join(Environment.NewLine, CorpusTypes.Select(x
			=> $"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof({x.Name}))]"));
		return GeneratorRunner.Run(sources: [reader.ReadToEnd(), attributes,]);
	});

	public static IEnumerable<object[]> Types
		=> CorpusTypes.Select(x => new object[] { x.Type, "global::" + x.Name, });

	[Fact]
	public async Task GeneratedRegistrations_ShouldCompile()
	{
		await That(Result.Value.Errors).IsEmpty();
	}

	[Theory]
	[MemberData(nameof(Types))]
	public async Task RegisteredMembers_ShouldMatchReflection(Type type, string key)
	{
		Dictionary<string, HashSet<string>> registrations = Parse(Result.Value.Generated);

		await That(registrations).ContainsKey(key)
			.Because("every corpus type has public instance members, so reflection would compare it");
		await That(registrations[key]).IsEqualTo(ReflectedMembers(type)).InAnyOrder()
			.Because("a registration that differs from reflection would compare a different set of members under AOT");
	}

	/// <remarks>
	///     The oracle is what <c>IncludeMembersExtensions</c> reflects over, without indexers, whose accessors take
	///     arguments. A shadowed member appears twice in reflection and once in the registry, so both are compared as
	///     sets.
	/// </remarks>
	private static IEnumerable<string> ReflectedMembers(Type type)
	{
		const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
		return type.GetFields(flags).Select(field => "F:" + field.Name)
			.Concat(type.GetProperties(flags)
				.Where(property => property.CanRead && property.GetIndexParameters().Length == 0 &&
				                   property.GetMethod!.IsPublic)
				.Select(property => "P:" + property.Name))
			.Distinct();
	}

	private static Dictionary<string, HashSet<string>> Parse(string generated)
	{
		Dictionary<string, HashSet<string>> result = new(StringComparer.Ordinal);
		HashSet<string>? current = null;
		foreach (string rawLine in generated.Split('\n'))
		{
			string line = rawLine.TrimEnd('\r');
			if (line.StartsWith("\t// ", StringComparison.Ordinal))
			{
				current = [];
				result[line.Substring(4)] = current;
				continue;
			}

			Match match = Regex.Match(line, "\\.Register(Field|Property)(?:<.*>)?\\((?:probe, )?\"(\\w+)\"");
			if (match.Success && current is not null)
			{
				current.Add((match.Groups[1].Value == "Field" ? "F:" : "P:") + match.Groups[2].Value);
			}
		}

		return result;
	}
}
