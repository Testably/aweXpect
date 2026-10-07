using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.Generators.Tests;

public sealed partial class TypeMetadataGeneratorTests
{
	private const string Library = "namespace Lib { public class Foo { public int Id { get; set; } } }";

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

	[Test]
	public async Task ShouldEmitAModuleInitializer()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Other());"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("[global::System.Runtime.CompilerServices.ModuleInitializer]")
			.Because("the metadata has to be registered before any code of the assembly compares anything");
	}

	[Test]
	public async Task ShouldRegisterEverythingInOneBatch()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Subject());"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains(string.Join(Environment.NewLine,
				"\t\t\tglobal::aweXpect.Core.Metadata.TypeMetadataRegistry.RegisterBatch(static () =>",
				"\t\t\t{",
				"\t\t\t\tRegister0();",
				"\t\t\t\tRegister1();",
				"\t\t\t\tRegister2();",
				"\t\t\t});"))
			.Because("a comparison on another thread must not see a type with only some of its members");
	}

	[Test]
	public async Task WhenAnAttributeOnlyHasTheNameOfTheMarker_ShouldNotRegister()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			"""
			namespace Lookalike
			{
				public sealed class RequiresMemberMetadataAttribute : System.Attribute;
			}

			[Lookalike.RequiresMemberMetadata]
			public class Tests
			{
				public static void Check([Lookalike.RequiresMemberMetadata] object expected) { }

				public void Test() => Check(new Models.Other());
			}
			""",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).IsEmpty()
			.Because("only the marker of aweXpect.Core seeds the walk, whatever another attribute is called");
	}

	[Test]
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

	[Test]
	[Arguments("values")]
	[Arguments("new Models.Outer<T>.Inner()")]
	public async Task WhenAnonymousTypeCarriesATypeParameterIndirectly_ShouldNotRegisterIt(string value)
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class Outer<T> { public class Inner { public int Id { get; set; } } } }",
			$$"""
			  using aweXpect;
			  public class Tests
			  {
			  	public void Test<T>(T[] values) => Expect.That(new { Value = {{value}} }).IsEquivalentTo(new { Value = {{value}} });
			  }
			  """,
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("probe")
			.Because("an array of the type parameter or a type nested in a generic over it cannot be named either");
	}

	[Test]
	public async Task WhenAnonymousTypeHasAGenericMemberOverAnAnonymousType_ShouldRegisterItThroughAProbe()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Call("""
			     var item = new { Id = 1 };
			     var items = System.Linq.Enumerable.Select(new[] { 1 }, i => new { Id = i });
			     var expected = new
			     {
			     	Items = items,
			     	List = System.Linq.Enumerable.ToList(items),
			     	Map = System.Linq.Enumerable.ToDictionary(items, x => "k"),
			     	Pair = (1, item),
			     	Grid = new[,] { { item } },
			     	Jagged = new[] { new[,] { { item } } },
			     };
			     Expect.That(expected).IsEquivalentTo(expected);
			     """),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains(
				"var probe = new { Items = Probe0(new { Id = default(int), }), List = Probe1(new { Id = default(int), }), Map = Probe2(new { Id = default(int), }), Pair = Probe3(new { Id = default(int), }), Grid = Probe4(new { Id = default(int), }), Jagged = new[] { Probe5(new { Id = default(int), }), }, };")
			.Because("a generic over an anonymous type cannot be named either, so a helper infers it from the probe");
		await That(result.Generated)
			.Contains("static global::System.Collections.Generic.IEnumerable<T0> Probe0<T0>(T0 p0) where T0 : class => default;");
		await That(result.Generated)
			.Contains("static global::System.Collections.Generic.Dictionary<string, T0> Probe2<T0>(T0 p0) where T0 : class => default;");
		await That(result.Generated)
			.Contains("static global::System.ValueTuple<int, T0> Probe3<T0>(T0 p0) where T0 : class => default;");
		await That(result.Generated).Contains("static T0[,] Probe4<T0>(T0 p0) where T0 : class => default;");
		await That(result.Generated).Contains("RegisterProperty(probe, \"Map\", o => o.Map);");
		await That(result.Generated).Contains("RegisterField(probe, \"Item2\", o => o.Item2);")
			.Because("the tuple over the anonymous type gets its own registration through a helper as well");
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenArgumentIsMarked_ShouldRegisterTheSubjectType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Subject());"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the comparison fetches the members of the subject by name, so it needs the subject registered");
	}

	[Test]
	public async Task WhenArgumentIsNamed_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(expected: new Models.Subject());"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Subject, int>(\"Id\", o => o.Id);")
			.Because("a named argument is matched by its name instead of its position");
	}

	[Test]
	public async Task WhenAssemblyAliasesIncludeGlobal_ShouldRegisterItsTypes()
	{
		MetadataReference library = GeneratorRunner.CompileToReference("Lib", Library);

		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Call("Expect.That(new Lib.Foo()).IsEquivalentTo(new Lib.Foo());"),],
			additionalReferences: [library.WithAliases(["A", "global",]),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Lib.Foo, int>(\"Id\", o => o.Id);")
			.Because("the global alias makes the type reachable through global::");
	}

	[Test]
	public async Task WhenAssemblyIsReferencedByAliasAndGlobally_ShouldRegisterItsTypes()
	{
		MetadataReference library = GeneratorRunner.CompileToReference("Lib", Library);

		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Call("Expect.That(new Lib.Foo()).IsEquivalentTo(new Lib.Foo());"),],
			additionalReferences: [library.WithAliases(["A",]), library,]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Lib.Foo, int>(\"Id\", o => o.Id);")
			.Because("one global reference is enough, whatever other aliases the assembly is known under");
	}

	[Test]
	public async Task WhenAssemblyIsReferencedOnlyByAlias_ShouldNotRegisterItsTypes()
	{
		MetadataReference library = GeneratorRunner.CompileToReference("Lib", Library);

		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			extern alias A;
			using aweXpect;

			public class Tests
			{
				public void Test() => Expect.That(new A::Lib.Foo()).IsEquivalentTo(new A::Lib.Foo());
			}
			""",
		], additionalReferences: [library.WithAliases(["A",]),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("Lib.Foo")
			.Because("a type that is only reachable through an extern alias cannot be named with global::");
	}

	[Test]
	public async Task WhenBasePropertyIsDynamicAndHiddenByAnObjectOne_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public class WithDynamic
			{
				public dynamic Value { get; set; } = 1;
			}

			public class HidingDynamic : WithDynamic
			{
				public int Own { get; set; }
				private new object Value { get; set; } = 2;
			}
			""",
			Call("Expect.That(new Models.HidingDynamic()).IsEquivalentTo(new Models.HidingDynamic());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("(\"Own\", o => o.Own);");
		await That(result.Generated).DoesNotContain("\"Value\"")
			.Because("dynamic is object in metadata, so the private object property hides the dynamic base one");
	}

	[Test]
	public async Task WhenBasePropertyIsHiddenByAByReferenceOne_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public class WithValue
			{
				public int Value { get; set; }
			}

			public class HidingByReference : WithValue
			{
				private int _value;
				private new ref int Value => ref _value;
			}
			""",
			Call("Expect.That(new Models.HidingByReference()).IsEquivalentTo(new Models.HidingByReference());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("(\"Value\", o => ((global::Models.WithValue)o).Value);")
			.Because("a by-reference return is part of the metadata signature, so the base property is not hidden");
	}

	[Test]
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
			.Because("the runtime drops a base property hidden by name and type when the private hider sits on the reflected type itself");
	}

	[Test]
	public async Task WhenBasePropertyIsHiddenPrivatelyOnAnIntermediateType_ShouldRegisterIt()
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
				private new int Value { get; set; }
			}

			public class Leaf : HidingPrivately
			{
				public int Own { get; set; }
			}
			""",
			Call("Expect.That(new Models.Leaf()).IsEquivalentTo(new Models.Leaf());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Leaf, int>(\"Value\", o => ((global::Models.WithValue)o).Value);")
			.Because("the runtime never returns private members of a base type, so a private hider on an intermediate type does not hide anything");
	}

	[Test]
	public async Task WhenBasePropertyOfANestedGenericIsHiddenPrivately_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public class GenericBase<T>
			{
				public T Value { get; set; } = default!;
			}

			public class Outer<T>
			{
				public class Inner<U> : GenericBase<T>
				{
					public int Own { get; set; }
					private new U Value { get; set; } = default!;
				}
			}
			""",
			Call("Expect.That(new Models.Outer<int>.Inner<string>()).IsEquivalentTo(new Models.Outer<int>.Inner<string>());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("(\"Value\", o => ((global::Models.GenericBase<int>)o).Value);")
			.Because("metadata numbers U as the second type parameter of the nested type, so it differs from the base's T");
	}

	[Test]
	public async Task WhenBasePropertyOnAGenericBaseIsHiddenPrivately_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public class GenericBase<T>
			{
				public T Value { get; set; } = default!;
			}

			public class HidingGenerically : GenericBase<int>
			{
				public int Own { get; set; }
				private new int Value { get; set; }
			}
			""",
			Call("Expect.That(new Models.HidingGenerically()).IsEquivalentTo(new Models.HidingGenerically());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("(\"Value\", o => ((global::Models.GenericBase<int>)o).Value);")
			.Because("the runtime compares the declared signature, in which the base property is typed by the type parameter, not by int");
	}

	[Test]
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

	[Test]
	public async Task WhenCompilationChanges_ShouldCacheTheCallSites()
	{
		GeneratorDriverRunResult result = GeneratorRunner.RunTwice(new TypeMetadataGenerator(),
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Subject());"),],
			"public class Unrelated;");

		IncrementalStepRunReason[] reasons = result.Results[0].TrackedSteps["CallSites"]
			.SelectMany(x => x.Outputs).Select(x => x.Reason).ToArray();
		await That(reasons).IsNotEmpty();
		await That(reasons).All()
			.Satisfy(x => x is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged)
			.Because("an unchanged call site has to compare equal, so that the registrations are not collected again");
	}

	[Test]
	public async Task WhenCompilationChanges_ShouldWalkTheChangedTypeAgain()
	{
		CSharpParseOptions parseOptions = new(LanguageVersion.Latest);
		SyntaxTree models = CSharpSyntaxTree.ParseText(Models, parseOptions);
		CSharpCompilation compilation = GeneratorRunner.CreateCompilation(
			[Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Other());"),]).AddSyntaxTrees(models);
		GeneratorDriver driver = CSharpGeneratorDriver.Create([new TypeMetadataGenerator().AsSourceGenerator(),],
			parseOptions: parseOptions);
		driver = driver.RunGenerators(compilation);

		driver = driver.RunGenerators(compilation.ReplaceSyntaxTree(models, CSharpSyntaxTree.ParseText(
			Models.Replace("public int Count { get; set; }", "public int Count { get; set; } public int Total { get; set; }"),
			parseOptions)));

		await That(string.Concat(driver.GetRunResult().GeneratedTrees.Select(x => x.ToString())))
			.Contains("RegisterProperty<global::Models.Other, int>(\"Total\", o => o.Total);")
			.Because("the walks are shared between the call sites of one compilation only");
	}

	[Test]
	public async Task WhenConsumerDeclaresNamespacesThatShadowTheSystemNamespace_ShouldCompile()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace aweXpect.System { internal class Shadow { } }",
			"namespace aweXpect.Generators.System { internal class Shadow { } }",
			Models,
			Call("Expect.That(new System.Collections.Generic.List<Models.Other>()).IsEquivalentTo(new Models.Other[0]);"),
		]);

		await That(result.Errors).IsEmpty()
			.Because("an extension package could declare a namespace under `aweXpect.` that shadows the BCL");
		await That(result.Generated).Contains("typeof(global::System.Collections.Generic.List<global::Models.Other>)");
	}

	[Test]
	public async Task WhenConsumerDeclaresTheModuleInitializerAsPolyfill_ShouldEmit()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace System.Runtime.CompilerServices { internal sealed class ModuleInitializerAttribute : Attribute; }",
			Models,
			Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Other());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("an internal polyfill in the consumer's own assembly can be applied by the generated code");
	}

	[Test]
	public async Task WhenConsumerIsExperimental_ShouldRegisterItsTypes()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"[assembly: System.Diagnostics.CodeAnalysis.Experimental(\"OWN001\")]",
			Models,
			Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Other());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the compiler reports an experimental assembly only to the assemblies that reference it");
	}

	[Test]
	public async Task WhenConsumerUsesCSharp8_ShouldNotEmit()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class Other { public int Count { get; set; } } }",
			Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Other());"),
		], languageVersion: LanguageVersion.CSharp8);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).IsEmpty()
			.Because("a module initializer needs C# 9, so emitting one would break the consumer's build");
	}

	[Test]
	public async Task WhenConsumerUsesCSharp9_ShouldCompile()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class Other { public int Count { get; set; } } }",
			Call("Expect.That(new Models.Other()).IsEquivalentTo(new { Count = 1 });"),
		], languageVersion: LanguageVersion.CSharp9);

		await That(result.Errors).IsEmpty()
			.Because("the generated file must not use anything newer than the module initializer itself");
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);");
	}

	[Test]
	public async Task WhenCoreCannotRegisterABatch_ShouldRegisterEachTypeDirectly()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class Plain { public int Own { get; set; } } }",
			"""
			namespace aweXpect.Core.Metadata
			{
				public sealed class GenerateMetadataAttribute(System.Type type) : System.Attribute
				{
					public System.Type Type { get; } = type;
				}

				public static class TypeMetadataRegistry
				{
					public static void RegisterField<T, TMember>(string name, System.Func<T, TMember> getValue) { }
					public static void RegisterProperty<T, TMember>(string name, System.Func<T, TMember> getValue) { }
					public static void RegisterProperty<T, TMember>(T probe, string name, System.Func<T, TMember> getValue) { }
				}
			}
			""",
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Plain))]",
		], false);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains(string.Join(Environment.NewLine,
			"\t\tinternal static void Register()",
			"\t\t{",
			"\t\t\tRegister0();",
			"\t\t}"));
		await That(result.Generated).DoesNotContain("RegisterBatch")
			.Because("an aweXpect.Core without the batch would not compile it");
	}

	[Test]
	public async Task WhenCoreCannotRegisterDictionaries_ShouldNotRegisterThem()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithMap { public System.Collections.Generic.Dictionary<string, int> Map { get; set; } = new(); } }",
			"""
			namespace aweXpect.Core.Metadata
			{
				public sealed class GenerateMetadataAttribute(System.Type type) : System.Attribute
				{
					public System.Type Type { get; } = type;
				}

				public static class TypeMetadataRegistry
				{
					public static void RegisterField<T, TMember>(string name, System.Func<T, TMember> getValue) { }
					public static void RegisterProperty<T, TMember>(string name, System.Func<T, TMember> getValue) { }
					public static void RegisterProperty<T, TMember>(T probe, string name, System.Func<T, TMember> getValue) { }
				}
			}
			""",
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.WithMap))]",
		], false);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains(
				"RegisterProperty<global::Models.WithMap, global::System.Collections.Generic.Dictionary<string, int>>(\"Map\", o => o.Map);");
		await That(result.Generated).DoesNotContain("RegisterDictionary")
			.Because("an aweXpect.Core without the registration would not compile it, and reads the key comparer by reflection instead");
	}

	[Test]
	public async Task WhenCoreCannotRegisterExplicitProperties_ShouldRegisterThePublicMembersOnly()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public interface IHasValue { int Value { get; } } public class WithExplicit : IHasValue { public int Own { get; set; } int IHasValue.Value => Own; } }",
			"""
			namespace aweXpect.Core.Metadata
			{
				public sealed class GenerateMetadataAttribute(System.Type type) : System.Attribute
				{
					public System.Type Type { get; } = type;
				}

				public static class TypeMetadataRegistry
				{
					public static void RegisterField<T, TMember>(string name, System.Func<T, TMember> getValue) { }
					public static void RegisterProperty<T, TMember>(string name, System.Func<T, TMember> getValue) { }
					public static void RegisterProperty<T, TMember>(T probe, string name, System.Func<T, TMember> getValue) { }
				}
			}
			""",
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.WithExplicit))]",
		], false);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.WithExplicit, int>(\"Own\", o => o.Own);");
		await That(result.Generated).DoesNotContain("RegisterExplicitProperty")
			.Because("an aweXpect.Core without the registration would not compile it, and it lacks the fallback that reads it");
	}

	[Test]
	public async Task WhenCoreHasAnIncompatibleRegistry_ShouldNotEmit()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			"""
			namespace aweXpect.Core.Metadata
			{
				[System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
				public sealed class GenerateMetadataAttribute(System.Type type) : System.Attribute
				{
					public System.Type Type { get; } = type;
				}

				public class TypeMetadataRegistry
				{
					public void RegisterProperty<T, TMember>(T probe, string name, System.Func<T, TMember> getValue) { }
					internal static void RegisterProperty<T, TMember>(string name, System.Func<T, TMember> getValue, bool other) { }
					public static void RegisterProperty<T, TMember>(string name, System.Func<T, TMember> getValue) { }
					public void RegisterBatch(System.Action register) { }
					internal static void RegisterBatch(System.Func<int> register) { }
					public static void RegisterBatch(System.Action register, bool other) { }
					public void RegisterDictionary<TKey, TValue>() { }
					internal static void RegisterDictionary<TKey, TValue>(int other) { }
					public void RegisterSet<T>() { }
					internal static void RegisterSet<T>(int other) { }
					public void RegisterCollection<T>() { }
					internal static void RegisterCollection<T>(int other) { }
					public void RegisterExplicitProperty<T, TMember>(string name, System.Func<T, TMember> getValue) { }
					internal static void RegisterExplicitProperty<T, TMember>(string name, System.Func<T, TMember> getValue, int other) { }
				}
			}
			""",
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Other))]",
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(System.Collections.Generic.List<Models.Other>))]",
		], false);

		await That(result.Errors).IsEmpty();
		await That(result.GeneratorDiagnostics).IsEmpty();
		await That(result.Generated).IsEmpty()
			.Because("only public static registration methods can be called from the generated code");
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenDiagnosticIdIsNumeric_ShouldNotRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithNumericObsolete { [System.Obsolete(\"gone\", DiagnosticId = \"0618\")] public int Old { get; set; } } }",
			Call("Expect.That(new Models.WithNumericObsolete()).IsEquivalentTo(new Models.WithNumericObsolete());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated).DoesNotContain("WithNumericObsolete")
			.Because("the compiler reads a numeric id in a pragma as a CS code, so the warning would stay");
	}

	[Test]
	public async Task WhenDictionaryHasDynamicValues_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Call("""
			     var expected = new { Map = new System.Collections.Generic.Dictionary<string, dynamic>(), Value = (dynamic)1 };
			     Expect.That(expected).IsEquivalentTo(expected);
			     """),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("global::aweXpect.Core.Metadata.TypeMetadataRegistry.RegisterDictionary<string, dynamic>();")
			.Because("dynamic can be named in the generated code");
		await That(result.Generated).Contains("Value = default(dynamic)");
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenGenerateMetadataAttributeIsSurroundedByAPragma_ShouldSuppressTheDiagnostic()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class Generic<T> { public T Value { get; set; } = default!; } }",
			"""
			#pragma warning disable aweXpect2001
			[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Generic<>))]
			#pragma warning restore aweXpect2001
			""",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.GeneratorDiagnostics.Where(x => !x.IsSuppressed)).IsEmpty()
			.Because("a warning on the attribute can be suppressed locally, like any other warning");
	}

	[Test]
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

	[Test]
	[Arguments("Models.Point?")]
	[Arguments("(int Left, Models.Point Right)")]
	public async Task WhenGenerateMetadataAttributeNamesANullableOrATuple_ShouldRegisterItWithoutADiagnostic(
		string type)
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public struct Point { public int X; } }",
			$"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof({type}))]",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.GeneratorDiagnostics).IsEmpty()
			.Because("the diagnostic looks for the registration under the key the walk uses");
		await That(result.Generated).Contains("RegisterField<global::Models.Point, int>(\"X\", o => o.X);");
	}

	[Test]
	public async Task WhenGenerateMetadataAttributeNamesAnUnregistrableType_ShouldLinkToTheDocumentation()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class Generic<T> { public T Value { get; set; } = default!; } }",
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Generic<>))]",
		]);

		await That(result.GeneratorDiagnostics).HasSingle().Which
			.Satisfies(x => x.Descriptor.HelpLinkUri == "https://docs.testably.org/aweXpect/analyzers#metadata-generator" &&
			                x.Descriptor.Description.ToString().Length > 0)
			.Because("every rule links to its section on the analyzers page and explains itself in the IDE");
	}

	[Test]
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

	[Test]
	public async Task WhenGenerateMetadataAttributeNamesNoType_ShouldIgnoreIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			["[assembly: aweXpect.Core.Metadata.GenerateMetadata(null!)]",]);

		await That(result.Errors).IsEmpty();
		await That(result.GeneratorDiagnostics).IsEmpty();
		await That(result.Generated).IsEmpty()
			.Because("there is no type to register or to report");
	}

	[Test]
	public async Task WhenGenericOverAnAnonymousTypeIsNestedOrGlobal_ShouldRegisterItThroughAProbe()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			public class Box<T>
			{
				public T Value { get; set; } = default!;
			}

			namespace Models
			{
				public class Outer<T>
				{
					public class Inner
					{
						public int Id { get; set; }
					}
				}
			}
			""",
			Call("""
			     var item = new { A = 1 };
			     Expect.That(Make(item)).IsEquivalentTo(Make(item));

			     static (Box<T>, Models.Outer<T>.Inner) Make<T>(T value) => (new Box<T> { Value = value, }, new Models.Outer<T>.Inner());
			     """),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("static global::Box<T0> Probe0<T0>(T0 p0) where T0 : class => default;")
			.Because("a type in the global namespace is spelled with global:: alone");
		await That(result.Generated)
			.Contains("static global::Models.Outer<T0>.Inner Probe0<T0>(T0 p0) where T0 : class => default;")
			.Because("a nested type is spelled through its generic container");
	}

	[Test]
	[Arguments("RequiresDynamicCode(\"emits\")")]
	[Arguments("RequiresAssemblyFiles")]
	public async Task WhenGetterRequiresDynamicCodeOrAssemblyFiles_ShouldNotRegisterTheType(string attribute)
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			$$"""
			  namespace Models
			  {
			  	public class WithTrimmedGetter
			  	{
			  		public int Id { get; set; }
			  		public string Name
			  		{
			  			[System.Diagnostics.CodeAnalysis.{{attribute}}]
			  			get => "";
			  		}
			  	}
			  }
			  """,
			Call("Expect.That(new Models.WithTrimmedGetter()).IsEquivalentTo(new Models.WithTrimmedGetter());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("WithTrimmedGetter")
			.Because("calling such a getter would make the registration itself a warning on publish");
	}

	[Test]
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

	[Test]
	public async Task WhenInheritedGetterRequiresUnreferencedCode_ShouldNotRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models
			{
				public class WithTrimmedGetter
				{
					public virtual string Name
					{
						[System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("reflects")]
						get => "";
						set { }
					}
				}

				public class OverridingTheSetter : WithTrimmedGetter
				{
					public int Id { get; set; }
					public override string Name
					{
						set { }
					}
				}
			}
			""",
			Call("Expect.That(new Models.OverridingTheSetter()).IsEquivalentTo(new Models.OverridingTheSetter());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated).DoesNotContain("OverridingTheSetter")
			.Because("an override of the setter alone is read through the inherited getter, which would make the registration a trimming warning on publish");
	}

	[Test]
	public async Task WhenInvocationDoesNotNameTheMethod_ShouldStillRegister()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			"""
			public delegate void Compare([aweXpect.Core.Metadata.RequiresMemberMetadata] object expected);

			public class Tests
			{
				public void Test(System.Func<Compare> getCompare) => getCompare()(new Models.Other());
			}
			""",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("an invocation that does not reveal the name of its method has to be bound to be judged");
	}

	[Test]
	public async Task WhenMarkedArgumentIsOmitted_ShouldRegisterTheParameterType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			"""
			public class Tests
			{
				public static void Check(int first = 0,
					[aweXpect.Core.Metadata.RequiresMemberMetadata] Models.Other? expected = null, int last = 0) { }

				public void Test()
				{
					Check();
					Check(last: 1, first: 2);
				}
			}
			""",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("without an argument, the declared type of the parameter is all that reaches the comparison");
	}

	[Test]
	public async Task WhenMarkerIsAppliedThroughANamespaceAlias_ShouldRegister()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			"""
			using M = aweXpect.Core.Metadata;

			public class Tests
			{
				public static void Check<[M::RequiresMemberMetadata] T>(T expected) { }

				public void Test() => Check(new Models.Other());
			}
			""",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("an alias-qualified marker carries the name of the marker as well");
	}

	[Test]
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

	[Test]
	public async Task WhenMemberIsANumberOrAStringBuilder_ShouldNotRegisterItsMembers()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Call("""
			     Expect.That(new { Number = System.Numerics.BigInteger.One, Text = new System.Text.StringBuilder() })
			     	.IsEquivalentTo(new { Number = System.Numerics.BigInteger.One, Text = new System.Text.StringBuilder() });
			     """),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("\"IsPowerOfTwo\"").And.DoesNotContain("\"Capacity\"")
			.Because("numbers and string builders are compared by value");
	}

	[Test]
	public async Task WhenMemberIsARefStruct_ShouldNotRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithSpan { public int Id { get; set; } public System.Span<byte> Bytes => default; } }",
			Call("Expect.That(new Models.WithSpan()).IsEquivalentTo(new Models.WithSpan());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("WithSpan")
			.Because("a ref struct cannot be a type argument, and reflection cannot read such a member either, so the type is compared through reflection where it fails as before");
	}

	[Test]
	public async Task WhenMemberIsATask_ShouldNotRegisterItsMembers()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Call("""
			     Expect.That(new { Task = System.Threading.Tasks.Task.FromResult(1), ValueTask = new System.Threading.Tasks.ValueTask<int>(1) })
			     	.IsEquivalentTo(new { Task = System.Threading.Tasks.Task.FromResult(1), ValueTask = new System.Threading.Tasks.ValueTask<int>(1) });
			     """),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("\"Result\"").And.DoesNotContain("\"IsCompletedSuccessfully\"")
			.Because("tasks and value tasks are compared by value");
	}

	[Test]
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

	[Test]
	public async Task WhenMemberIsAnExpectation_ShouldNotRegisterItsMembers()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			Call("Expect.That(new Models.Other()).IsEquivalentTo(new { Count = aweXpect.Equivalency.It.Is<int>() });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("\"ExpectationBuilder\"")
			.Because("an expectation is evaluated against the actual value instead of being compared member by member");
	}

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenMemberIsObsoleteWithoutError_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class WithOld { [System.Obsolete(\"gone\", false)] public int Old { get; set; } } }",
			Call("Expect.That(new Models.WithOld()).IsEquivalentTo(new Models.WithOld());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.WithOld, int>(\"Old\", o => o.Old);")
			.Because("only an obsolete member that is an error cannot be referenced");
	}

	[Test]
	public async Task WhenMemberTypeIsNotReferenced_ShouldNotRegisterTheType()
	{
		MetadataReference dependency = GeneratorRunner.CompileToReference("Dependency",
			"namespace Dependency { public class Dep { public int Value { get; set; } } }");
		MetadataReference owner = GeneratorRunner.CompileToReference("Owner",
			"namespace Owner { public class Root { public Dependency.Dep D { get; set; } = new(); public int Id { get; set; } } }",
			dependency);

		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Call("Expect.That(new Owner.Root()).IsEquivalentTo(new Owner.Root());"),],
			additionalReferences: [owner,]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("Owner.Root")
			.Because("the generated code cannot name a type from an assembly the consumer does not reference");
	}

	[Test]
	public async Task WhenNothingIsMarked_ShouldNotEmit()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[Models, Call("Expect.That(new Models.Other()).IsNotNull();"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).IsEmpty();
	}

	[Test]
	public async Task WhenPropertyIsAPointer_ShouldNotRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public unsafe class WithPointer { public int Id { get; set; } public int* Pointer => null; public delegate*<void> Function => null; } }",
			Call("Expect.That(new Models.WithPointer()).IsEquivalentTo(new Models.WithPointer());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("WithPointer")
			.Because("a pointer cannot be a type argument of a registration, and reflection cannot box it either");
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task WhenTypeIsADictionary_ShouldRegisterItsKeyAndValueTypesOnce()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			Call(
				"Expect.That(new System.Collections.Generic.Dictionary<string, Models.Other>()).IsEquivalentTo(new System.Collections.Generic.Dictionary<string, Models.Other>());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains(
				"global::aweXpect.Core.Metadata.TypeMetadataRegistry.RegisterDictionary<string, global::Models.Other>();")
			.Once()
			.Because("the comparison reads the key comparer through a reader for the type arguments of both dictionary interfaces");
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the values are still compared by their members");
	}

	[Test]
	public async Task WhenTypeIsASet_ShouldRegisterItsItemTypeOnce()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models,
			Call(
				"Expect.That(new System.Collections.Generic.HashSet<Models.Other>()).IsEquivalentTo(new System.Collections.Generic.HashSet<Models.Other>());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("global::aweXpect.Core.Metadata.TypeMetadataRegistry.RegisterSet<global::Models.Other>();")
			.Once()
			.Because("the comparison reads the comparer through a reader for the type argument of both set interfaces");
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the items are still compared by their members when the comparer does not find them");
	}

	[Test]
	public async Task WhenTypeIsAnUnnameableDictionary_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			using aweXpect;

			public class Tests
			{
				private class Hidden { public int Id { get; set; } }

				public void Test()
					=> Expect.That(new System.Collections.Generic.Dictionary<int, Hidden>())
						.IsEquivalentTo(new System.Collections.Generic.Dictionary<int, Hidden>());
			}
			""",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("RegisterDictionary")
			.Because("the generated code cannot name the private value type");
	}

	[Test]
	public async Task WhenTypeIsAnUnnameableSet_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			using aweXpect;

			public class Tests
			{
				private class Hidden { public int Id { get; set; } }

				public void Test()
					=> Expect.That(new System.Collections.Generic.HashSet<Hidden>())
						.IsEquivalentTo(new System.Collections.Generic.HashSet<Hidden>());
			}
			""",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("RegisterSet")
			.Because("the generated code cannot name the private item type");
	}

	[Test]
	public async Task WhenTypeIsEnumerable_ShouldKeepItsInterfacesWithoutAWarningOfTheNet8AotCompiler()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Call("Expect.That(new System.Collections.Generic.List<string>()).IsEquivalentTo(new string[0]);"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains(string.Join(Environment.NewLine,
				"\t\t// interfaces of global::System.Collections.Generic.List<string>",
				"\t#if NET5_0_OR_GREATER",
				"\t\t[global::System.Diagnostics.CodeAnalysis.DynamicDependency(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.Interfaces, typeof(global::System.Collections.Generic.List<string>))]",
				"\t#if !NET9_0_OR_GREATER",
				"\t\t// ILC 8 cannot resolve interfaces from a dependency, but ILLink 8 honours it when trimming.",
				"\t\t[global::System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage(\"Trimming\", \"IL2037\")]",
				"\t#endif",
				"\t#endif",
				"\t\tprivate static void Register"))
			.Because("ILC 8 reports IL2037 for the dependency, which fails a publish that treats warnings as errors");
	}

	[Test]
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
		await That(result.Generated).DoesNotContain("RegisterProperty<global::System.Collections.Generic.List<")
			.Because("the comparison enumerates a collection instead of comparing its members");
		await That(result.Generated)
			.Contains("typeof(global::System.Collections.Generic.List<global::Models.Other>)")
			.Because(
				"the collection keeps the interfaces that select the comparison, which the trimmer would drop");
	}

	[Test]
	[Arguments("", "", "[Experimental(\"LIBEXP001\")]")]
	[Arguments("", "[Experimental(\"LIBEXP001\")]", "")]
	[Arguments("[assembly: Experimental(\"LIBEXP001\")]", "", "")]
	[Arguments("[module: Experimental(\"LIBEXP001\")]", "", "")]
	public async Task WhenTypeIsExperimental_ShouldNotRegisterIt(string assemblyAttribute, string outerAttribute,
		string typeAttribute)
	{
		MetadataReference library = GeneratorRunner.CompileToReference("Lib", $$"""
		                                                                        using System.Diagnostics.CodeAnalysis;
		                                                                        {{assemblyAttribute}}
		                                                                        namespace Lib
		                                                                        {
		                                                                        	{{outerAttribute}}
		                                                                        	public class Outer
		                                                                        	{
		                                                                        		{{typeAttribute}}
		                                                                        		public class Foo { public int Id { get; set; } }
		                                                                        	}
		                                                                        }
		                                                                        """);

		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			["#pragma warning disable LIBEXP001\n" + Call("Expect.That(new Lib.Outer.Foo()).IsEquivalentTo(new Lib.Outer.Foo());"),],
			additionalReferences: library);

		await That(result.Errors).IsEmpty()
			.Because("using an experimental type is an error by default, which the consumer can only suppress in its own files");
		await That(result.Generated).DoesNotContain("Lib.Outer.Foo");
	}

	[Test]
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

	[Test]
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

#if DEBUG
	[Test]
	public async Task WhenTypeOnlyImplementsPropertiesExplicitly_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public interface IHasValue { int Value { get; } } public class OnlyExplicit : IHasValue { int IHasValue.Value => 1; } }",
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.OnlyExplicit))]",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.GeneratorDiagnostics).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterExplicitProperty<global::Models.OnlyExplicit, int>(\"Models.IHasValue.Value\", o => ((global::Models.IHasValue)o).Value);")
			.Because("reflection would find the explicit implementation, so the type must not be left to a reflection that is unavailable under AOT");
	}
#endif

	[Test]
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

	[Test]
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

	[GeneratedRegex("^\t+// global::Models\\.Other\r?$", RegexOptions.Multiline)]
	private static partial Regex OtherRegistration();

#if DEBUG
	[Test]
	public async Task WhenPropertyIsImplementedExplicitly_ForAnInaccessibleInterface_ShouldLeaveItOut()
	{
		MetadataReference library = GeneratorRunner.CompileToReference("Lib",
			"namespace Lib { internal interface IHidden { int Value { get; } } public class WithHidden : IHidden { public int Own { get; set; } int IHidden.Value => Own; } }");
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			["[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Lib.WithHidden))]",],
			additionalReferences: library);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Lib.WithHidden, int>(\"Own\", o => o.Own);");
		await That(result.Generated).DoesNotContain("RegisterExplicitProperty")
			.Because("the generated code cannot cast to an interface it cannot see, and leaving out one fallback must not cost the type its registration");
	}

	[Test]
	public async Task WhenPropertyIsImplementedExplicitly_ForAnInterfaceOverAnAnonymousType_ShouldNotRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public interface IHasItem<T>
			{
				T Item { get; }
			}

			public class Box<T> : IHasItem<T>
			{
				public T Value { get; set; } = default!;
				T IHasItem<T>.Item => Value;
			}

			public static class Box
			{
				public static Box<T> Of<T>(T value) => new() { Value = value, };
			}
			""",
			Call("Expect.That(Models.Box.Of(new { A = 1 })).IsEquivalentTo(new { Value = new { A = 1 }, Item = new { A = 1 } });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("global::Models.Box<")
			.Because("the generated code cannot cast to an interface over an anonymous type, and registering the other members alone would answer the explicit lookup differently than reflection");
	}

	[Test]
	public async Task WhenPropertyIsImplementedExplicitly_OnABaseType_ShouldRegisterItOnce()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public interface IHasValue
			{
				int Value { get; }
			}

			public class Base : IHasValue
			{
				public int Own { get; set; }
				int IHasValue.Value => 1;
			}

			public class Derived : Base, IHasValue
			{
				int IHasValue.Value => 2;
			}
			""",
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Derived))]",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterExplicitProperty<global::Models.Derived, int>(\"Models.IHasValue.Value\", o => ((global::Models.IHasValue)o).Value);")
			.Exactly(1)
			.Because("the re-implementation hides the one on the base, as it does for reflection");
	}

	[Test]
	public async Task WhenPropertyIsImplementedExplicitly_OnAGenericOverAnAnonymousType_ShouldRegisterItThroughAProbe()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public interface IHasId
			{
				int Id { get; }
			}

			public class Box<T> : IHasId
			{
				public T Value { get; set; } = default!;
				int IHasId.Id => 42;
			}

			public static class Box
			{
				public static Box<T> Of<T>(T value) => new() { Value = value, };
			}
			""",
			Call("Expect.That(Models.Box.Of(new { A = 1 })).IsEquivalentTo(new { Value = new { A = 1 }, Id = 42 });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty(probe, \"Value\", o => o.Value);");
		await That(result.Generated)
			.Contains("RegisterExplicitProperty(probe, \"Models.IHasId.Id\", o => ((global::Models.IHasId)o).Id);")
			.Because("a registered type answers the explicit lookup from its registration only, so leaving the implementation out would report it as missing although reflection finds it");
	}

	[Test]
	public async Task WhenPropertyIsImplementedExplicitly_ShouldRegisterItThroughItsInterface()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Models;

			public interface IHasValue
			{
				int Value { get; }
			}

			public interface IHasItems<T>
			{
				T Value { get; }
			}

			public class WithExplicit : IHasValue, IHasItems<string>
			{
				public int Own { get; set; }
				int IHasValue.Value => Own;
				string IHasItems<string>.Value => "";
			}
			""",
			Call("Expect.That(new Models.WithExplicit()).IsEquivalentTo(new { Value = 1 });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterExplicitProperty<global::Models.WithExplicit, int>(\"Models.IHasValue.Value\", o => ((global::Models.IHasValue)o).Value);");
		await That(result.Generated)
			.Contains("RegisterExplicitProperty<global::Models.WithExplicit, string>(\"Models.IHasItems<System.String>.Value\", o => ((global::Models.IHasItems<string>)o).Value);")
			.Because("each implementation is registered under its own qualified name, so that the comparison can tell an ambiguous short name apart");
	}
#endif
}
