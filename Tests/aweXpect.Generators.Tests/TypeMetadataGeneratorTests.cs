using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace aweXpect.Generators.Tests;

public sealed partial class TypeMetadataGeneratorTests
{
	private const string Models = """
	                              namespace Models;

	                              public class Subject
	                              {
	                              	public int Id { get; set; }
	                              	public Address? Address { get; set; }
	                              }

	                              public class Address
	                              {
	                              	public string Street = "";
	                              }

	                              public class Other
	                              {
	                              	public int Count { get; set; }
	                              }
	                              """;

	[Fact]
	public async Task ShouldEmitAModuleInitializer()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Other());"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("[System.Runtime.CompilerServices.ModuleInitializer]")
			.Because("the metadata has to be registered before any code of the assembly compares anything");
	}

	[Fact]
	public async Task WhenAnonymousTypeCarriesATypeParameter_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			using aweXpect;
			public class Tests
			{
				public void Test<T>(T value) => Expect.That(new { Value = value }).IsEquivalentTo(new { Value = value });
			}
			""",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("default(T)")
			.Because("the type parameter of the enclosing method cannot be named in a module initializer");
	}

	[Fact]
	public async Task WhenArgumentIsAnonymous_ShouldRegisterThroughAProbe()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			Call("Expect.That(new Models.Other()).IsEquivalentTo(new { Count = 1, Inner = new { Name = \"foo\" } });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("var probe = new { Count = default(int), Inner = new { Name = default(string), }, };")
			.Because("an anonymous type cannot be named, so an instance has to provide the type argument");
		await That(result.Generated).Contains("RegisterProperty(probe, \"Inner\", o => o.Inner);");
		await That(result.Generated).Contains("var probe = new { Name = default(string), };")
			.Because("the nested anonymous type is a member type and gets its own registration");
	}

	[Fact]
	public async Task WhenArgumentIsMarked_ShouldRegisterTheExpectedTypeTransitively()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Subject());"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Subject, int>(\"Id\", o => o.Id);");
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Subject, global::Models.Address>(\"Address\", o => o.Address);");
		await That(result.Generated)
			.Contains("RegisterField<global::Models.Address, string>(\"Street\", o => o.Street);")
			.Because("the comparison recurses into the members, so their types need a registration too");
	}

	[Fact]
	public async Task WhenArgumentIsMarked_ShouldRegisterTheSubjectType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Subject());"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the comparison fetches the members of the subject by name, so it needs the subject registered");
	}

	[Fact]
	public async Task WhenArgumentIsNamed_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(expected: new Models.Subject());"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Subject, int>(\"Id\", o => o.Id);")
			.Because("a named argument is matched by its name instead of its position");
	}

	[Fact]
	public async Task WhenBasePropertyIsHiddenByALessVisibleOne_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public class WithValue
			{
				public int Value { get; set; }
			}

			public class HidingPrivately : WithValue
			{
				public int Own { get; set; }
				private new int Value { get; set; }
			}
			""",
			Call("Expect.That(new Models.HidingPrivately()).IsEquivalentTo(new Models.HidingPrivately());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("(\"Own\", o => o.Own);");
		await That(result.Generated).DoesNotContain("\"Value\"")
			.Because("the runtime drops a base property hidden by name and type, whatever the visibility of the hiding one");
	}

	[Fact]
	public async Task WhenCalledAsAStaticMethod_ShouldRegisterTheArgumentAndTheSubject()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			Call("ThatObject.IsEquivalentTo(Expect.That(new Models.Other()), new Models.Subject());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Subject, int>(\"Id\", o => o.Id);");
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the receiver is the first argument when an extension method is called as a static method");
	}

	[Fact]
	public async Task WhenCoreIsNotReferenced_ShouldNotEmit()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			"""
			namespace aweXpect.Core.Metadata
			{
				public sealed class GenerateMetadataAttribute(System.Type type) : System.Attribute
				{
					public System.Type Type { get; } = type;
				}
			}
			""",
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Other))]",
		], false);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).IsEmpty()
			.Because("a registration against a Core without the registry would not compile");
	}

	[Fact]
	public async Task WhenDiagnosticIdIsMalformed_ShouldNotRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithOddObsolete { [System.Obsolete(\"gone\", DiagnosticId = \"MY LIB\")] public int Old { get; set; } } }",
			Call("Expect.That(new Models.WithOddObsolete()).IsEquivalentTo(new Models.WithOddObsolete());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated).DoesNotContain("WithOddObsolete")
			.Because("an id that is not an identifier cannot be suppressed by a pragma");
	}

	[Fact]
	public async Task WhenFieldHidesAProperty_ShouldRegisterBoth()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public class WithProperty
			{
				public int Value => 1;
			}

			public class HidingField : WithProperty
			{
				public new int Value = 2;
			}
			""",
			Call("Expect.That(new Models.HidingField()).IsEquivalentTo(new Models.HidingField());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterField<global::Models.HidingField, int>(\"Value\", o => o.Value);");
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.HidingField, int>(\"Value\", o => ((global::Models.WithProperty)o).Value);")
			.Because("reflection compares the field and the hidden property, so the registration has to hold both");
	}

	[Fact]
	public async Task WhenGenerateMetadataAttributeIsApplied_ShouldRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, "[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Other))]",]);

		await That(result.Errors).IsEmpty();
		await That(result.GeneratorDiagnostics).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the attribute is the escape hatch for a type no marked call site reveals");
	}

	[Fact]
	public async Task WhenGenerateMetadataAttributeNamesAnArray_ShouldRegisterTheElementWithoutADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, "[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Other[]))]",]);

		await That(result.Errors).IsEmpty();
		await That(result.GeneratorDiagnostics).IsEmpty()
			.Because("the element type is what the comparison visits, so registering it is the expected outcome");
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);");
	}

	[Fact]
	public async Task WhenGenerateMetadataAttributeNamesAnUnregistrableType_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class Generic<T> { public T Value { get; set; } = default!; } }",
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Generic<>))]",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).IsEmpty();
		await That(result.GeneratorDiagnostics).HasSingle().Which
			.Satisfies(x => x.Id == "aweXpect2001" && x.GetMessage().Contains("'Models.Generic<>'"))
			.Because("the escape hatch is used when something already went wrong, so silently doing nothing is not acceptable");
	}

	[Fact]
	public async Task WhenGetterRequiresUnreferencedCode_ShouldNotRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models
			{
				public class WithTrimmedGetter
				{
					public int Id { get; set; }
					public string Name
					{
						[System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("reflects")]
						get => "";
					}
				}
			}
			""",
			Call("Expect.That(new Models.WithTrimmedGetter()).IsEquivalentTo(new Models.WithTrimmedGetter());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("WithTrimmedGetter")
			.Because("calling such a getter would make the registration itself a trimming warning on publish");
	}

	[Fact]
	public async Task WhenMemberIsABigTuple_ShouldRegisterRestInsteadOfTheVirtualItems()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithBigTuple { public (int, int, int, int, int, int, int, int) Eight { get; set; } } }",
			Call("Expect.That(new Models.WithBigTuple()).IsEquivalentTo(new Models.WithBigTuple());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("(\"Rest\", o => o.Rest);");
		await That(result.Generated).DoesNotContain("\"Item8\"")
			.Because("the eighth element is a virtual field of the tuple syntax that reflection never sees");
	}

	[Fact]
	public async Task WhenMemberIsARefStruct_ShouldNotRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithSpan { public int Id { get; set; } public System.Span<byte> Bytes => default; } }",
			Call("Expect.That(new Models.WithSpan()).IsEquivalentTo(new Models.WithSpan());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("WithSpan")
			.Because("a ref struct cannot be a type argument, and registering the other members alone would compare fewer of them than reflection");
	}

	[Fact]
	public async Task WhenMemberIsATuple_ShouldRegisterItsItems()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithTuple { public (int Left, string Right) Pair { get; set; } } }",
			Call("Expect.That(new Models.WithTuple()).IsEquivalentTo(new Models.WithTuple());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterField<(int, string), int>(\"Item1\", o => o.Item1);")
			.Because("reflection sees the tuple's fields, not the element names of the declaration");
		await That(result.Generated).DoesNotContain("\"Left\"");
	}

	[Fact]
	public async Task WhenMemberIsHidden_ShouldRegisterTheMostDerivedDeclaration()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public class WithValue
			{
				public int Value { get; set; }
			}

			public class Hiding : WithValue
			{
				public new string Value { get; set; } = "";
			}
			""",
			Call("Expect.That(new Models.Hiding()).IsEquivalentTo(new Models.Hiding());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Hiding, string>(\"Value\", o => o.Value);");
		await That(result.Generated).DoesNotContain("RegisterProperty<global::Models.Hiding, int>")
			.Because("reflection keeps the declaration on the most derived type, so the registry has to as well");
	}

	[Fact]
	public async Task WhenMemberIsNamedLikeAKeyword_ShouldEscapeIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithKeywords { public int @class { get; set; } public string @event = \"\"; } }",
			Call("Expect.That(new Models.WithKeywords()).IsEquivalentTo(new Models.WithKeywords());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("(\"class\", o => o.@class);")
			.Because("the registered name is the metadata name, while the member access needs the escape");
		await That(result.Generated).Contains("(\"event\", o => o.@event);");
	}

	[Fact]
	public async Task WhenMemberIsNullableStruct_ShouldRegisterTheUnderlyingType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public struct Point { public int X; } public class WithNullable { public Point? Maybe { get; set; } } }",
			Call("Expect.That(new Models.WithNullable()).IsEquivalentTo(new Models.WithNullable());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.WithNullable, global::Models.Point?>(\"Maybe\", o => o.Maybe);");
		await That(result.Generated).Contains("RegisterField<global::Models.Point, int>(\"X\", o => o.X);")
			.Because("a boxed nullable has the runtime type of its underlying struct");
	}

	[Fact]
	public async Task WhenMemberIsObsolete_ShouldRegisterItWithoutWarnings()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithObsolete { [System.Obsolete(\"gone\")] public int Old { get; set; } } }",
			Call("Expect.That(new Models.WithObsolete()).IsEquivalentTo(new Models.WithObsolete());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty()
			.Because("a consumer building with warnings as errors must not fail on generated code");
		await That(result.Generated).Contains("(\"Old\", o => o.Old);")
			.Because("reflection compares an obsolete member like any other");
	}

	[Fact]
	public async Task WhenMemberIsObsoleteWithDiagnosticId_ShouldRegisterItWithoutWarnings()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithCustomObsolete { [System.Obsolete(\"gone\", DiagnosticId = \"MYLIB001\")] public int Old { get; set; } } }",
			Call("Expect.That(new Models.WithCustomObsolete()).IsEquivalentTo(new Models.WithCustomObsolete());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty()
			.Because("an obsolete member reports under its own id, which the plain CS0618 suppression does not cover");
		await That(result.Generated).Contains("#pragma warning disable CS0612, CS0618, MYLIB001");
	}

	[Fact]
	public async Task WhenMemberIsObsoleteWithError_ShouldNotRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithDead { [System.Obsolete(\"gone\", true)] public int Dead { get; set; } public int Alive { get; set; } } }",
			Call("Expect.That(new Models.WithDead()).IsEquivalentTo(new Models.WithDead());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("WithDead")
			.Because("the member cannot be referenced, and registering the other members alone would compare fewer of them than reflection");
	}

	[Fact]
	public async Task WhenMemberTypeIsNotReferenced_ShouldNotRegisterTheType()
	{
		MetadataReference dependency = GeneratorRunner.CompileToReference("Dependency",
			"namespace Dependency { public class Dep { public int Value { get; set; } } }");
		MetadataReference owner = GeneratorRunner.CompileToReference("Owner",
			"namespace Owner { public class Root { public Dependency.Dep D { get; set; } = new(); public int Id { get; set; } } }",
			dependency);

		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Call("Expect.That(new Owner.Root()).IsEquivalentTo(new Owner.Root());"),], true, owner);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("Owner.Root")
			.Because("the generated code cannot name a type from an assembly the consumer does not reference");
	}

	[Fact]
	public async Task WhenNothingIsMarked_ShouldNotEmit()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, Call("Expect.That(new Models.Other()).IsNotNull();"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).IsEmpty();
	}

	[Fact]
	public async Task WhenPropertyReturnsByReference_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithRef { private int _value; public ref int Value => ref _value; } }",
			Call("Expect.That(new Models.WithRef()).IsEquivalentTo(new Models.WithRef());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.WithRef, int>(\"Value\", o => o.Value);")
			.Because("reflection reads a ref-returning property like any other, so the registry has to hold it");
	}

	[Fact]
	public async Task WhenReceiverIsAnInstance_ShouldRegisterItsTypeArgument()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			Call("Expect.That(new[] { new Models.Other(), }).All().AreEquivalentTo(new Models.Subject());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the element type of the receiver is the subject of each element comparison");
	}

	[Fact]
	public async Task WhenTypeIsEnumerable_ShouldRegisterTheElementTypeInstead()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			Call(
				"Expect.That(new System.Collections.Generic.List<Models.Other>()).IsEquivalentTo(new Models.Other[0]);"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);");
		await That(result.Generated).DoesNotContain("System.Collections.Generic.List<")
			.Because("the comparison enumerates a collection instead of comparing its members");
	}

	[Fact]
	public async Task WhenTypeIsFileLocal_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			using aweXpect;
			file class Hidden
			{
				public int Value { get; set; }
			}
			public class Tests
			{
				public void Test() => Expect.That(new Hidden()).IsEquivalentTo(new Hidden());
			}
			""",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("Hidden")
			.Because("a file-local type is only visible inside its own file, and the generated code is another one");
	}

	[Fact]
	public async Task WhenTypeIsNotAccessible_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			using aweXpect;
			public class Tests
			{
				public void Test() => Expect.That(new Hidden()).IsEquivalentTo(new Hidden());
				private sealed class Hidden
				{
					public int Value { get; set; }
				}
			}
			""",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("Hidden")
			.Because("generated code in a separate class cannot reach a private nested type");
	}

	[Fact]
	public async Task WhenTypeParameterIsMarked_ShouldRegisterTheTypeArgument()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			Call("Expect.That(new[] { new Models.Other(), }).Contains(new Models.Other()).Equivalent();"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the expected value reached the comparison through an earlier call, so only the type argument names it");
	}

	[Fact]
	public async Task WhenTypeReachesTheComparisonTwice_ShouldRegisterItOnce()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Other());"),
			Call("Expect.That(new Models.Subject()).IsEquivalentTo(new Models.Other());").Replace("class Tests",
				"class OtherTests"),
		]);

		await That(result.Errors).IsEmpty();
		await That(OtherRegistration().Matches(result.Generated).Count).IsEqualTo(1)
			.Because("the registry keeps one entry per type, so the registration is emitted once");
	}

	private static string Call(string statement)
		=> $$"""
		     using aweXpect;

		     public class Tests
		     {
		     	public void Test()
		     	{
		     		{{statement}}
		     	}
		     }
		     """;

	[GeneratedRegex("^\t// global::Models\\.Other\r?$", RegexOptions.Multiline)]
	private static partial Regex OtherRegistration();
}
