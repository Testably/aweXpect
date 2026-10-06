using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using aweXpect.Equivalency;

namespace aweXpect.Generators.Tests;

/// <summary>
///     The generator skips the types the equivalency comparison compares by value, and has to keep its own list of
///     them. Here the comparison itself says for every candidate type how it is compared, and the generator is
///     held to that answer.
/// </summary>
[Explicit]
[Category(TestCategories.Slow)]
public sealed class ComparisonTypeParityTests
{
	/// <remarks>
	///     Every assembly of the framework contributes every public type it has, so that a type added to either list
	///     is likely to have a candidate already.
	/// </remarks>
	private static readonly Assembly[] Assemblies = Directory
		.GetFiles(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "System.*.dll")
		.Select(TryLoad)
		.OfType<Assembly>()
		.ToArray();

	private static readonly Type[] Candidates = Assemblies
		.SelectMany(assembly => assembly.GetExportedTypes())
		.Concat(NestedTypes(typeof(Corpus)))
		.Where(type => !type.ContainsGenericParameters && type != typeof(void) && !IsUnreferenceable(type))
		.Append(typeof(Task<int>))
		.Append(typeof(ValueTask<int>))
		.Append(typeof(Corpus.DerivedTask<int>))
		.ToArray();

	private static readonly Lazy<GeneratorRunner.GeneratorResult> ByValue = new(()
		=> Run(Candidates.Where(IsComparedByValue)));

	private static readonly Lazy<GeneratorRunner.GeneratorResult> ByMembers = new(()
		=> Run(Candidates.Where(type => !IsComparedByValue(type))));

	[Test]
	public async Task Candidates_ShouldCoverEveryRuleOfTheComparison()
	{
		Type[] expected =
		[
			typeof(int), typeof(nint), typeof(DayOfWeek), typeof(string), typeof(decimal), typeof(DateTime),
			typeof(DateTimeOffset), typeof(TimeSpan), typeof(Guid), typeof(DateOnly), typeof(TimeOnly), typeof(Half),
			typeof(NFloat), typeof(Int128), typeof(UInt128), typeof(BigInteger),
			typeof(Complex), typeof(Type), typeof(Assembly), typeof(Module), typeof(Action), typeof(Uri),
			typeof(CultureInfo), typeof(IPAddress), typeof(Encoding), typeof(Task),
			typeof(Task<int>), typeof(ValueTask), typeof(ValueTask<int>), typeof(StringBuilder),
			typeof(Regex), typeof(JsonElement), typeof(JsonNode),
			typeof(JsonObject), typeof(JsonArray),
			typeof(UTF8Encoding), typeof(Corpus.Level), typeof(Corpus.Handler), typeof(Corpus.DerivedType),
			typeof(Corpus.DerivedAssembly), typeof(Corpus.DerivedModule), typeof(Corpus.DerivedUri),
			typeof(Corpus.DerivedCulture), typeof(Corpus.DerivedAddress), typeof(Corpus.DerivedEncoding),
			typeof(Corpus.DerivedRegex), typeof(Corpus.DerivedTask), typeof(Corpus.DerivedTask<int>),
		];

		await That(Candidates.Where(IsComparedByValue)).Contains(expected).InAnyOrder()
			.Because("a rule of the comparison that no candidate represents is not held against the generator");
	}

	[Test]
	public async Task TypesComparedByMembers_ShouldBeRegistered()
	{
		HashSet<string> registered = [..Keys(ByMembers.Value.Generated),];

		List<string> missing = Candidates
			.Where(type => !IsComparedByValue(type) && IsRegistrable(type))
			.Select(Name)
			.Where(name => !registered.Contains(name))
			.ToList();

		await That(missing).IsEmpty()
			.Because("the comparison reads the members of a type the generator takes for compared by value by reflection, which fails when trimmed");
	}

	[Test]
	public async Task TypesComparedByValue_ShouldNotBeRegistered()
	{
		IEnumerable<string> registered = Keys(ByValue.Value.Generated)
			.Where(key => !key.StartsWith("events of ", StringComparison.Ordinal));

		await That(registered).IsEmpty()
			.Because("the comparison never reads the members of a type it compares by value, so a registration only roots them in a trimmed application");
		await That(ByValue.Value.GeneratorDiagnostics.Select(x => x.Id)).All().AreEqualTo("aweXpect2001").And
			.HasCount(Candidates.Count(IsComparedByValue))
			.Because("naming a type that is compared by value has no effect, which the warning tells");
	}

	/// <remarks>
	///     The public <see cref="EquivalencyDefaults.DefaultComparisonType" /> is the whole decision of the comparison,
	///     including the types that are compared by their content.
	/// </remarks>
	private static bool IsComparedByValue(Type type)
		=> EquivalencyDefaults.DefaultComparisonType(type) == EquivalencyComparisonType.ByValue;

	/// <summary>
	///     Whether the generator has no other reason than the comparison type to leave the <paramref name="type" /> out.
	/// </summary>
	/// <remarks>
	///     Only a type the generator registers for certain is held against it, so a type with a member that could
	///     keep it from being registered is left out.
	/// </remarks>
	private static bool IsRegistrable(Type type)
	{
		if (type.IsAbstract || type.IsByRefLike || typeof(IEnumerable).IsAssignableFrom(type))
		{
			return false;
		}

		const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
		PropertyInfo[] properties = type.GetProperties(flags);
		List<(MemberInfo Member, Type Type)> members = type.GetFields(flags)
			.Select(field => ((MemberInfo)field, field.FieldType))
			.Concat(properties.Select(property => ((MemberInfo)property, property.PropertyType)))
			.ToList();
		return members.Count > 0 &&
		       properties.All(property => property.GetIndexParameters().Length == 0 &&
		                                  property.GetMethod is { IsPublic: true, } getter &&
		                                  !IsUnreferenceable(getter)) &&
		       members.All(member => !IsUnreferenceable(member.Member) && !IsUnreferenceable(member.Type) &&
		                             member.Type is
		                             {
			                             IsPointer: false, IsFunctionPointer: false, IsByRef: false, IsByRefLike: false,
		                             });
	}

	/// <remarks>
	///     The generator cannot name something that is obsolete, experimental or unsafe for trimming without a
	///     diagnostic in the generated code, and <see langword="typeof" /> in the attribute cannot either.
	/// </remarks>
	private static bool IsUnreferenceable(MemberInfo member)
		=> member.IsDefined(typeof(ObsoleteAttribute), false) ||
		   member.IsDefined(typeof(ExperimentalAttribute), false) ||
		   member.IsDefined(typeof(RequiresUnreferencedCodeAttribute), false) ||
		   member.IsDefined(typeof(RequiresDynamicCodeAttribute), false) ||
		   member.IsDefined(typeof(RequiresAssemblyFilesAttribute), false) ||
		   (member is Type { IsArray: true, } array && IsUnreferenceable(array.GetElementType()!)) ||
		   (member is Type { IsGenericType: true, } generic && generic.GetGenericArguments().Any(IsUnreferenceable)) ||
		   (member.DeclaringType is { } declaringType && member is Type && IsUnreferenceable(declaringType));

	private static Assembly? TryLoad(string path)
	{
		try
		{
			return Assembly.Load(AssemblyName.GetAssemblyName(path));
		}
		catch (BadImageFormatException)
		{
			return null;
		}
	}

	private static IEnumerable<Type> NestedTypes(Type type)
		=> type.GetNestedTypes().SelectMany(nested => NestedTypes(nested).Prepend(nested));

	private static GeneratorRunner.GeneratorResult Run(IEnumerable<Type> types)
		=> GeneratorRunner.Run(
		[
			Corpus(),
			string.Join(Environment.NewLine, types.Select(type
				=> $"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof({Name(type)}))]")),
		]);

	private static string Corpus()
	{
		using Stream stream = typeof(ComparisonTypeParityTests).Assembly.GetManifestResourceStream("Corpus.cs")!;
		using StreamReader reader = new(stream);
		return reader.ReadToEnd();
	}

	/// <summary>
	///     The <paramref name="type" /> as source code, which for a type that is not generic is also the key of its
	///     registration.
	/// </summary>
	private static string Name(Type type)
	{
		if (!type.IsGenericType)
		{
			return "global::" + type.FullName!.Replace('+', '.');
		}

		string definition = type.GetGenericTypeDefinition().FullName!;
		return "global::" + definition.Substring(0, definition.IndexOf('`')).Replace('+', '.') + "<" +
		       string.Join(", ", type.GetGenericArguments().Select(Name)) + ">";
	}

	private static IEnumerable<string> Keys(string generated)
		=> generated.Split('\n')
			.Select(line => line.Trim())
			.Where(line => line.StartsWith("// ", StringComparison.Ordinal) &&
			               !line.StartsWith("// <auto-generated", StringComparison.Ordinal) &&
			               !line.StartsWith("// ILC ", StringComparison.Ordinal))
			.Select(line => line.Substring(3));
}
