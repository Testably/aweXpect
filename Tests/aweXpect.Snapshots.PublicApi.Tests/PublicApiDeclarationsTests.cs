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
	public async Task GenerateFrom_WhenExcludingAnAttribute_ShouldExcludeItEverywhere()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.ExcludingAttribute("Library.MarkAttribute"));

		await That(result).DoesNotContain("Library.Mark]").And.Contains("public sealed class MarkAttribute")
			.Because("the attribute is excluded on the assembly, members, return values, accessors and parameters");
	}

	[Fact]
	public async Task GenerateFrom_WhenExcludingAnAttributeByType_ShouldExcludeIt()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.ExcludingAttribute<ObsoleteAttribute>());

		await That(result).DoesNotContain("System.Obsolete");
	}

	[Fact]
	public async Task GenerateFrom_WhenExcludingANestedAttribute_ShouldExcludeIt()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library,
			o => o.ExcludingAttribute("Library.Outer+NestedAttribute"));

		await That(result).DoesNotContain("[Library.Outer.Nested]");
	}

	[Fact]
	public async Task GenerateFrom_WhenExcludingAGenericAttribute_ShouldExcludeItWithAnyTypeArguments()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.ExcludingAttribute("Library.TagAttribute`1"));

		await That(result).DoesNotContain("[Library.Tag<int>]");
	}

#if NET
	[Fact]
	public async Task GenerateFrom_WhenExcludingAGenericAttributeByType_ShouldExcludeItWithAnyTypeArguments()
	{
		string result = PublicApiDeclarations.GenerateFrom(typeof(Album).Assembly,
			o => o.ExcludingAttribute<TagAttribute<string>>());

		await That(result).DoesNotContain("Tag<int>]")
			.Because("the fixture applies the attribute with an int, but its definition is excluded");
	}
#endif

	[Fact]
	public async Task GenerateFrom_WhenExcludingIndexerName_ShouldNotWriteIt()
	{
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", AttributesSource));

		string result = PublicApiDeclarations.GenerateFrom(library,
			o => o.ExcludingAttribute<System.Runtime.CompilerServices.IndexerNameAttribute>());

		await That(result).DoesNotContain("IndexerName");
	}

	[Fact]
	public async Task GenerateFrom_WhenExcludingTypeForwardedTo_ShouldNotWriteForwardedTypes()
	{
		FileInfo library = WriteForwardingLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library,
			o => o.ExcludingAttribute<System.Runtime.CompilerServices.TypeForwardedToAttribute>());

		await That(result).DoesNotContain("TypeForwardedTo");
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
	public async Task GenerateFrom_WhenExcludingForwardedTypes_ShouldNotIncludeThem()
	{
		FileInfo library = WriteForwardingLibrary();

		string result = PublicApiDeclarations.GenerateFrom(library, o => o.ExcludingForwardedTypes());

		await That(result).DoesNotContain("TypeForwardedTo");
	}

	[Fact]
	public async Task GenerateFrom_WhenExcludingForwardedTypesOfAnAssembly_ShouldNotIncludeThem()
	{
		Assembly assembly = Assembly.LoadFile(WriteForwardingLibrary().FullName);

		string result = PublicApiDeclarations.GenerateFrom(assembly, o => o.ExcludingForwardedTypes());

		await That(result).DoesNotContain("TypeForwardedTo");
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
