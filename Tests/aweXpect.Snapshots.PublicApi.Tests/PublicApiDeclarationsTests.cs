using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using aweXpect.Snapshots.PublicApi.Fixture;
using Microsoft.CodeAnalysis;
using static aweXpect.Snapshots.PublicApi.Tests.TestHelpers.Compiler;

namespace aweXpect.Snapshots.PublicApi.Tests;

public sealed class PublicApiDeclarationsTests
{
	private const string LibrarySource = """
	                                     namespace Library;
	                                     public class Api
	                                     {
	                                         public void Enter(Dependency.Gate gate) { }
	                                         private void Hidden() { }
	                                     }
	                                     """;

	private const string DependencySource = "namespace Dependency; public class Gate { }";

	private const string TypesSource = """
	                                   namespace Library
	                                   {
	                                       public class Api { }
	                                       public class Legacy { }
	                                       public class Box<T> { }
	                                       public class Outer { public class Inner { } }
	                                   }
	                                   namespace Library.Generated { public class Generated { } }
	                                   namespace Library.Generated.Models { public class Model { } }
	                                   namespace Library.GeneratedTools { public class Tool { } }
	                                   namespace Other { public class Thing { } }
	                                   """;

	private const string AttributesSource = """
	                                        [assembly: Library.Mark]
	                                        namespace Library
	                                        {
	                                            [System.AttributeUsage(System.AttributeTargets.All)]
	                                            public sealed class MarkAttribute : System.Attribute { }
	                                            public static class Outer
	                                            {
	                                                public sealed class NestedAttribute : System.Attribute { }
	                                            }
	                                            public sealed class TagAttribute<T> : System.Attribute { }
	                                            internal sealed class SecretAttribute : System.Attribute { }
	                                            public class Api
	                                            {
	                                                [Mark, Outer.Nested, Tag<int>, Secret, System.Obsolete("old")]
	                                                [return: Mark]
	                                                public object Call<[Mark] T>([Mark] T value) => value;
	                                                public string Name { [Mark] get; set; } = "";
	                                                [System.Runtime.CompilerServices.IndexerName("Slot")]
	                                                public string this[int index] => "";
	                                                public bool TryGet([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out object value)
	                                                {
	                                                    value = "";
	                                                    return true;
	                                                }
	                                            }
	                                        }
	                                        namespace System.Diagnostics.CodeAnalysis
	                                        {
	                                            internal sealed class NotNullWhenAttribute(bool returnValue) : System.Attribute
	                                            {
	                                                public bool ReturnValue { get; } = returnValue;
	                                            }
	                                        }
	                                        """;

	private const string RecordsSource = """
	                                     namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }
	                                     namespace Library
	                                     {
	                                         public record Artist(string Name) { public override string ToString() => Name; }
	                                         public record struct Point(int X, int Y) { public override string ToString() => ""; }
	                                     }
	                                     """;

	[Fact]
	public async Task GenerateFrom_ShouldWriteTheAttributesOfTheLibrary()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library);

		await That(result).Contains("[assembly: Library.Mark]").And.Contains("[Library.Mark]")
			.And.Contains("[return: Library.Mark]").And.Contains("Call<[Library.Mark] T>([Library.Mark] T value)")
			.And.Contains("{ [Library.Mark] get; set; }").And.Contains("[Library.Outer.Nested]")
			.And.Contains("[Library.Tag<int>]").And.Contains("[System.Obsolete(\"old\")]")
			.And.Contains("[System.Runtime.CompilerServices.IndexerName(\"Slot\")]")
			.Because("the tests that exclude attributes would otherwise pass without excluding anything");
	}

	[Fact]
	public async Task GenerateFrom_ShouldNotWriteInternalAttributes()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library);

		await That(result).DoesNotContain("Secret")
			.Because("consumers cannot see an internal attribute");
	}

	[Fact]
	public async Task GenerateFrom_ShouldWriteInternalAttributesThatCompilersRead()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library);

		await That(result).Contains("[System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out object value")
			.Because("compilers read a polyfilled attribute by its name, so it changes how consumers compile");
	}

	[Fact]
	public async Task GenerateFrom_ShouldWriteInternalFeatureSwitchAttributes()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", """
			namespace Library
			{
			    public static class Features
			    {
			        [System.Diagnostics.CodeAnalysis.FeatureGuard(typeof(Features))]
			        [System.Diagnostics.CodeAnalysis.FeatureSwitchDefinition("Library.IsSupported")]
			        public static bool IsSupported => true;
			    }
			}
			namespace System.Diagnostics.CodeAnalysis
			{
			    internal sealed class FeatureGuardAttribute(System.Type featureType) : System.Attribute
			    {
			        public System.Type FeatureType { get; } = featureType;
			    }
			    internal sealed class FeatureSwitchDefinitionAttribute(string switchName) : System.Attribute
			    {
			        public string SwitchName { get; } = switchName;
			    }
			}
			"""));

		string result = PublicApiDeclarations.GenerateFrom(library);

		await That(result).Contains("[System.Diagnostics.CodeAnalysis.FeatureGuard(typeof(Library.Features))]")
			.And.Contains("[System.Diagnostics.CodeAnalysis.FeatureSwitchDefinition(\"Library.IsSupported\")]")
			.Because("trim and AOT analyzers read polyfilled feature switch attributes by their name");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringAnAttribute_ShouldExcludeItEverywhere()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.Attribute("Library.MarkAttribute"));

		await That(result).DoesNotContain("Library.Mark]").And.Contains("public sealed class MarkAttribute")
			.Because("the attribute is excluded on the assembly, members, return values, accessors and parameters");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringAnAttributeByType_ShouldExcludeIt()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.Attribute<ObsoleteAttribute>());

		await That(result).DoesNotContain("System.Obsolete");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringANestedAttribute_ShouldExcludeIt()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library,
			o => o.Ignoring.Attribute("Library.Outer+NestedAttribute"));

		await That(result).DoesNotContain("[Library.Outer.Nested]");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringAGenericAttribute_ShouldExcludeItWithAnyTypeArguments()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.Attribute("Library.TagAttribute`1"));

		await That(result).DoesNotContain("[Library.Tag<int>]");
	}

#if NET
	[Fact]
	public async Task GenerateFrom_WhenIgnoringAGenericAttributeByType_ShouldExcludeItWithAnyTypeArguments()
	{
		string result = PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly,
			o => o.Ignoring.Attribute<TagAttribute<string>>());

		await That(result).DoesNotContain("Tag<int>]")
			.Because("the fixture applies the attribute with an int, but its definition is excluded");
	}
#endif

	[Fact]
	public async Task GenerateFrom_WhenIgnoringIndexerName_ShouldNotWriteIt()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library,
			o => o.Ignoring.Attribute<System.Runtime.CompilerServices.IndexerNameAttribute>());

		await That(result).DoesNotContain("IndexerName");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringTypeForwardedTo_ShouldNotWriteForwardedTypes()
	{
		FileInfo library = WriteForwardingLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library,
			o => o.Ignoring.Attribute<System.Runtime.CompilerServices.TypeForwardedToAttribute>());

		await That(result).DoesNotContain("TypeForwardedTo");
	}

	[Fact]
	public async Task GenerateFrom_ShouldWriteTheAttributesThatAreStoredAsMetadataFlags()
	{
		FileInfo library = WritePseudoAttributesLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.Type("Library.Note"));

		await That(result).IsEqualTo("""
		                             namespace Library
		                             {
		                                 public delegate void Handler();
		                                 public enum Kind
		                                 {
		                                     A = 0,
		                                 }
		                                 public static class Native
		                                 {
		                                     [System.Runtime.InteropServices.DllImport("x")]
		                                     public static extern void Beep();
		                                     public static void Configure([System.Runtime.InteropServices.Optional] int retries, int depth = 3, int timeout = 5, [System.Runtime.CompilerServices.CallerMemberName] string caller = "", params int[] values) { }
		                                     [System.Runtime.InteropServices.DllImport("x", EntryPoint = "send", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true, ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, BestFitMapping = false, ThrowOnUnmappableChar = true)]
		                                     public static extern int Send(string message);
		                                 }
		                                 [System.Serializable]
		                                 public class Payload
		                                 {
		                                     public Payload() { }
		                                 }
		                                 public class Plain
		                                 {
		                                     public Plain() { }
		                                 }
		                             }

		                             """).IgnoringNewlineStyle()
			.Because("[Serializable] is written only where it is declared, e.g. not on enums or delegates, [Optional] " +
			         "only without a default value, which C# also stores as optional, and [DllImport] only with the " +
			         "arguments that differ from a plain [DllImport(\"x\")]");
	}

	[Fact]
	public async Task GenerateFrom_ShouldNotWriteSerializableForRecords()
	{
		FileInfo library = WritePseudoAttributesLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Within.Type("Library.Note"));

		await That(result).StartsWith("""
		                              namespace Library
		                              {
		                                  public record Note
		                              """).IgnoringNewlineStyle()
			.Because("the compiler does not mark a record as serializable unless it declares [Serializable]");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringSerializable_ShouldNotWriteIt()
	{
		FileInfo library = WritePseudoAttributesLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.Attribute<SerializableAttribute>());

		await That(result).DoesNotContain("Serializable").And.Contains("public class Payload");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringDllImport_ShouldNotWriteIt()
	{
		FileInfo library = WritePseudoAttributesLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library,
			o => o.Ignoring.Attribute<System.Runtime.InteropServices.DllImportAttribute>());

		await That(result).DoesNotContain("DllImport").And.Contains("public static extern void Beep();");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringOptional_ShouldNotWriteIt()
	{
		FileInfo library = WritePseudoAttributesLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library,
			o => o.Ignoring.Attribute<System.Runtime.InteropServices.OptionalAttribute>());

		await That(result).Contains("Configure(int retries, int depth = 3, int timeout = 5,");
	}

	[Fact]
	public async Task GenerateFrom_ShouldWriteAllTypesOfTheLibrary()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", TypesSource));

		string result = PublicApiDeclarations.GenerateFrom(library);

		await That(result).Contains("namespace Library\n").And.Contains("namespace Library.Generated\n")
			.And.Contains("namespace Library.GeneratedTools\n").And.Contains("namespace Other\n")
			.And.Contains("class Legacy").And.Contains("class Box<T>").And.Contains("class Inner")
			.Because("the tests that filter types would otherwise pass without filtering anything");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringANamespace_ShouldIgnoreItAndItsSubNamespacesOnly()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", TypesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.Namespace("Library.Generated"));

		await That(result).DoesNotContain("namespace Library.Generated\n")
			.And.DoesNotContain("namespace Library.Generated.Models\n")
			.And.Contains("namespace Library.GeneratedTools\n")
			.Because("the namespace matches whole segments");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringATypeByName_ShouldIgnoreIt()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", TypesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.Type("Library.Legacy"));

		await That(result).DoesNotContain("class Legacy").And.Contains("class Api");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringANestedType_ShouldKeepItsContainingType()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", TypesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.Type("Library.Outer+Inner"));

		await That(result).DoesNotContain("class Inner").And.Contains("class Outer");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringAGenericType_ShouldIgnoreItWithAnyTypeArguments()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", TypesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.Type("Library.Box`1"));

		await That(result).DoesNotContain("class Box<T>");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringATypeByType_ShouldIgnoreItWithItsNestedTypes()
	{
		string result = PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly, o => o.Ignoring.Type<Album>());

		await That(result).DoesNotContain("class Album").And.DoesNotContain("class Track")
			.And.Contains("record Artist");
	}

	[Fact]
	public async Task GenerateFrom_WithinANamespace_ShouldOnlyWriteItAndItsSubNamespaces()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", TypesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Within.Namespace("Library.Generated"));

		await That(result).Contains("namespace Library.Generated\n")
			.And.Contains("namespace Library.Generated.Models\n")
			.And.DoesNotContain("namespace Library.GeneratedTools\n")
			.And.DoesNotContain("namespace Library\n").And.DoesNotContain("namespace Other\n");
	}

	[Fact]
	public async Task GenerateFrom_WithinSeveralRestrictions_ShouldWriteTheTypesWithinAnyOfThem()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", TypesSource));

		string result = PublicApiDeclarations.GenerateFrom(library,
			o => o.Within.Namespace("Other").Within.Type("Library.Outer"));

		await That(result).Contains("namespace Other\n").And.Contains("class Outer").And.Contains("class Inner")
			.And.DoesNotContain("class Api").And.DoesNotContain("namespace Library.Generated\n");
	}

	[Fact]
	public async Task GenerateFrom_WithinAType_ShouldOnlyWriteItWithItsNestedTypes()
	{
		string result = PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly, o => o.Within.Type<Album>());

		await That(result).Contains("class Album").And.Contains("class Track").And.Contains("class Edition")
			.And.DoesNotContain("record Artist").And.DoesNotContain("public class GlobalType");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringWithinARestriction_ShouldIgnoreIt()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", TypesSource));

		string result = PublicApiDeclarations.GenerateFrom(library,
			o => o.Within.Namespace("Library").Ignoring.Namespace("Library.Generated").Ignoring.Type("Library.Legacy"));

		await That(result).Contains("class Api").And.DoesNotContain("class Legacy")
			.And.DoesNotContain("namespace Library.Generated\n").And.DoesNotContain("namespace Other\n")
			.Because("ignoring wins over a restriction");
	}

	[Fact]
	public async Task GenerateFrom_WhenFilteringNamespaces_ShouldAlsoFilterForwardedTypes()
	{
		FileInfo library = WriteForwardingLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.Namespace("Dependency"));

		await That(result).DoesNotContain("TypeForwardedTo");
	}

	[Fact]
	public async Task Within_WhenTypeIsNested_ShouldThrowArgumentException()
	{
		void Act()
			=> new PublicApiOptions().Within.Type<Album.Track>();

		await That(Act).Throws<ArgumentException>()
			.WithMessage(
				"The type 'aweXpect.Snapshots.PublicApi.Fixture.Album+Track' is nested, so it is written with its containing type: restrict the public API to the containing type instead.*")
			.AsWildcard();
	}

	[Fact]
	public async Task GenerateFrom_ShouldWriteNumbersIndependentlyOfTheCurrentCulture()
	{
		Assembly assembly = typeof(Album).Assembly;
		CultureInfo culture = CultureInfo.CurrentCulture;
		string expected;
		string result;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
			expected = PublicApiDeclarations.GenerateFrom(assembly);
			CultureInfo.CurrentCulture = new CultureInfo("sv-SE");
			result = PublicApiDeclarations.GenerateFrom(assembly);
		}
		finally
		{
			CultureInfo.CurrentCulture = culture;
		}

		await That(result).IsEqualTo(expected)
			.Because("sv-SE writes negative numbers with the minus sign U+2212, e.g. the value of Mood.Sad");
	}

	[Fact]
	public async Task GenerateFrom_ShouldIncludeForwardedTypes()
	{
		FileInfo library = WriteForwardingLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library);

		await That(result).StartsWith("""
		                              [assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Dependency.Box<>))]
		                              [assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Dependency.Gate))]
		                              [assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Dependency.Outer))]
		                              namespace Library
		                              """).IgnoringNewlineStyle()
			.Because("removing a forwarded type breaks the consumers that were compiled against the assembly");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringForwardedTypes_ShouldNotIncludeThem()
	{
		FileInfo library = WriteForwardingLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.ForwardedTypes());

		await That(result).DoesNotContain("TypeForwardedTo");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringForwardedTypesOfAnAssembly_ShouldNotIncludeThem()
	{
		Assembly assembly = Assembly.LoadFile(WriteForwardingLibrary().FullName);

		string result = PublicApiDeclarations.GenerateFrom(assembly, o => o.Ignoring.ForwardedTypes());

		await That(result).DoesNotContain("TypeForwardedTo");
	}

	[Fact]
	public async Task GenerateFrom_ShouldWriteTheCompilerGeneratedMembersOfRecords()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", RecordsSource));

		string result = PublicApiDeclarations.GenerateFrom(library);

		await That(result).Contains("protected Artist(Library.Artist original) { }")
			.And.Contains("public void Deconstruct(out string Name) { }")
			.And.Contains("public readonly void Deconstruct(out int X, out int Y) { }")
			.And.Contains("public static bool operator ==(Library.Point left, Library.Point right) { }")
			.Because("the test that ignores compiler-generated members would otherwise pass without ignoring anything");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringCompilerGeneratedMembers_ShouldOnlyWriteTheDeclaredMembersOfRecords()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", RecordsSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.CompilerGeneratedMembers());

		await That(result).IsEqualTo("""
		                             namespace Library
		                             {
		                                 public record Artist : System.IEquatable<Library.Artist>
		                                 {
		                                     public Artist(string Name) { }
		                                     public string Name { get; init; }
		                                     public override string ToString() { }
		                                 }
		                                 public struct Point : System.IEquatable<Library.Point>
		                                 {
		                                     public Point(int X, int Y) { }
		                                     public int X { readonly get; set; }
		                                     public int Y { readonly get; set; }
		                                     public override string ToString() { }
		                                 }
		                             }

		                             """).IgnoringNewlineStyle()
			.Because("the compiler marks only the accessors of positional properties as compiler-generated, and the primary constructor and a hand-written override are declared");
	}

	[Fact]
	public async Task GenerateFrom_WhenIgnoringCompilerGeneratedMembers_ShouldKeepAutoPropertiesAndEvents()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", """
			namespace Library
			{
			    public class Album
			    {
			        public string Title { get; set; } = "";
			        public event System.EventHandler Changed { add { } remove { } }
			        public event System.EventHandler Played;
			    }
			}
			"""));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.Ignoring.CompilerGeneratedMembers());

		await That(result).IsEqualTo(PublicApiDeclarations.GenerateFrom(library))
			.And.Contains("public string Title { get; set; }").And.Contains("public event System.EventHandler Played;")
			.Because("the compiler marks only the accessors and backing fields of auto-properties and field-like events");
	}

	[Fact]
	public async Task GenerateFrom_WhenAReferenceIsMissing_ShouldThrowInvalidOperationException()
	{
		byte[] dependency = Compile("Dependency", "2.0.0.0", DependencySource);
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", LibrarySource,
			[MetadataReference.CreateFromImage(dependency),]));

		void Act()
			=> PublicApiDeclarations.GenerateFrom(library);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"The public API of 'Library.dll' cannot be generated, because its references 'Dependency, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null' could not be resolved.*")
			.AsWildcard();
	}

	[Fact]
	public async Task GenerateFrom_WhenAReferenceIsAPackageOfTheLibrary_ShouldResolveItFromTheNuGetPackages()
	{
		FileInfo library = new(typeof(PublicApiDeclarationsTests).Assembly
			.GetCustomAttributes<AssemblyMetadataAttribute>()
			.Single(attribute => attribute.Key == "PackageFixture").Value!);

		string result = PublicApiDeclarations.GenerateFrom(library);

		await That(result).Contains("public System.Text.Json.Nodes.JsonNode? Root { get; set; }")
			.Because("the build output of the library does not contain System.Text.Json 10.0, which it references");
	}

	[Fact]
	public async Task GenerateFrom_WhenAReferenceIsOnlyFoundInALowerVersion_ShouldThrowInvalidOperationException()
	{
		byte[] dependency = Compile("Dependency", "2.0.0.0", DependencySource);
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", LibrarySource,
			[MetadataReference.CreateFromImage(dependency),]));
		WriteFile("Dependency.dll", Compile("Dependency", "1.0.0.0", DependencySource),
			library.DirectoryName);

		void Act()
			=> PublicApiDeclarations.GenerateFrom(library);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"*'Dependency, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null (only version 1.0.0.0 was found)'*")
			.AsWildcard()
			.Because("a lower version may lack types of the public API, e.g. an older .NET than the assembly targets");
	}

	[Fact]
	public async Task GenerateFrom_WhenAssemblyIsNotLoadedFromAFile_ShouldThrowArgumentException()
	{
		AssemblyBuilder assembly =
			AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("InMemory"), AssemblyBuilderAccess.Run);

		void Act()
			=> PublicApiDeclarations.GenerateFrom(assembly);

		await That(Act).Throws<ArgumentException>()
			.WithMessage("The assembly 'InMemory' was not loaded from a file, so its public API cannot be read.*")
			.AsWildcard();
	}

	[Fact]
	public async Task GenerateFrom_WhenFileIsAReferenceAssembly_ShouldGenerateThePublicApiOfTheImplementation()
	{
		byte[] dependency = Compile("Dependency", "1.0.0.0", DependencySource);
		MetadataReference[] references = [MetadataReference.CreateFromImage(dependency),];
		FileInfo implementation = WriteFile("Library.dll", Compile("Library", "1.0.0.0", LibrarySource, references));
		WriteFile("Dependency.dll", dependency, implementation.DirectoryName);
		FileInfo reference = WriteFile("Library.dll", Compile("Library", "1.0.0.0", LibrarySource, references, true));
		WriteFile("Dependency.dll", dependency, reference.DirectoryName);

		string result = PublicApiDeclarations.GenerateFrom(reference);

		await That(result).IsEqualTo(PublicApiDeclarations.GenerateFrom(implementation))
			.Because("a reference assembly, which cannot be loaded, has the same public API as its implementation");
	}

	[Fact]
	public async Task GenerateFrom_WhenGivenAFile_ShouldNotLoadTheAssembly()
	{
		string name = $"Unloaded{Guid.NewGuid():N}";
		FileInfo file = WriteFile($"{name}.dll", Compile(name, "1.0.0.0", "namespace Unloaded; public class Api { }"));

		PublicApiDeclarations.GenerateFrom(file);

		await That(AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetName().Name))
			.DoesNotContain(name);
	}

	[Fact]
	public async Task GenerateFrom_WhenGivenTheFileOfAnAssembly_ShouldGenerateTheSamePublicApiAsForTheAssembly()
	{
		Assembly assembly = typeof(Album).Assembly;

		string result = PublicApiDeclarations.GenerateFrom(new FileInfo(assembly.Location));

		await That(result).IsEqualTo(PublicApiDeclarations.GenerateFrom(assembly));
	}
}
