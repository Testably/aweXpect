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
public sealed partial class MemberParityTests
{
	private static readonly (Type Type, string Name)[] CorpusTypes =
	[
		(typeof(Corpus.Base), "aweXpect.Generators.Tests.Corpus.Base"),
		(typeof(Corpus.Derived), "aweXpect.Generators.Tests.Corpus.Derived"),
		(typeof(Corpus.Shadowing), "aweXpect.Generators.Tests.Corpus.Shadowing"),
		(typeof(Corpus.FieldHidingProperty), "aweXpect.Generators.Tests.Corpus.FieldHidingProperty"),
		(typeof(Corpus.WithIndexer), "aweXpect.Generators.Tests.Corpus.WithIndexer"),
		(typeof(Corpus.WithWriteOnly), "aweXpect.Generators.Tests.Corpus.WithWriteOnly"),
		(typeof(Corpus.WithStatics), "aweXpect.Generators.Tests.Corpus.WithStatics"),
		(typeof(Corpus.Generic<int>), "aweXpect.Generators.Tests.Corpus.Generic<int>"),
		(typeof(Corpus.WithExplicitInterface), "aweXpect.Generators.Tests.Corpus.WithExplicitInterface"),
		(typeof(Corpus.PositionalRecord), "aweXpect.Generators.Tests.Corpus.PositionalRecord"),
		(typeof(Corpus.Point), "aweXpect.Generators.Tests.Corpus.Point"),
		(typeof(Corpus.WithBigTuple), "aweXpect.Generators.Tests.Corpus.WithBigTuple"),
		(typeof(ValueTuple<int, int, int, int, int, int, int, ValueTuple<int>>),
			"(int, int, int, int, int, int, int, int)"),
		(typeof(Corpus.WithInitOnly), "aweXpect.Generators.Tests.Corpus.WithInitOnly"),
		(typeof(Corpus.WithRefProperty), "aweXpect.Generators.Tests.Corpus.WithRefProperty"),
		(typeof(Corpus.WithKeywords), "aweXpect.Generators.Tests.Corpus.WithKeywords"),
		(typeof(Corpus.WithObsolete), "aweXpect.Generators.Tests.Corpus.WithObsolete"),
		(typeof(Corpus.WithVisibilities), "aweXpect.Generators.Tests.Corpus.WithVisibilities"),
	];

	private static readonly Lazy<GeneratorRunner.GeneratorResult> Result = new(() =>
	{
		using Stream stream = typeof(MemberParityTests).Assembly.GetManifestResourceStream("Corpus.cs")!;
		using StreamReader reader = new(stream);
		string attributes = string.Join(Environment.NewLine, CorpusTypes.Select(x
			=> $"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof({x.Name}))]"));
		return GeneratorRunner.Run([reader.ReadToEnd(), attributes,]);
	});

	public static TheoryData<Type, string> Types
	{
		get
		{
			TheoryData<Type, string> data = new();
			foreach ((Type type, string name) in CorpusTypes)
			{
				data.Add(type, name.StartsWith('(') ? name : "global::" + name);
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
	///     The oracle is what <c>IncludeMembersExtensions</c> reflects over: public instance fields and readable
	///     public instance properties, without indexers, and one declaration per name.
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

			Match match = Registration().Match(line);
			if (match.Success && current is not null)
			{
				current.Add((match.Groups[1].Value == "Field" ? "F:" : "P:") + match.Groups[2].Value);
			}
		}

		return result;
	}

	[GeneratedRegex("\\.Register(Field|Property)(?:<.*>)?\\((?:probe, )?\"(\\w+)\"")]
	private static partial Regex Registration();
}
