namespace aweXpect.Generators.Tests;

public sealed partial class TypeMetadataGeneratorTests
{
	private const string Collections = """
	                                   namespace Models;

	                                   public class NamedList : System.Collections.Generic.List<Other>
	                                   {
	                                   	public string Name { get; set; } = "";
	                                   	public Address? Origin;
	                                   }

	                                   public class PlainList : System.Collections.Generic.List<Other>;

	                                   public class Page<T>(params T[] items) : System.Collections.Generic.IReadOnlyCollection<T>
	                                   {
	                                   	public int Total { get; set; }
	                                   	public int Count => items.Length;

	                                   	public System.Collections.Generic.IEnumerator<T> GetEnumerator()
	                                   		=> ((System.Collections.Generic.IEnumerable<T>)items).GetEnumerator();

	                                   	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	                                   		=> GetEnumerator();
	                                   }

	                                   public static class Page
	                                   {
	                                   	public static Page<T> Of<T>(params T[] items) => new(items);
	                                   }
	                                   """;

	[Test]
	public async Task WhenCollectionCannotBeNamed_ShouldRegisterItsOwnMembersThroughAProbe()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models, Collections,
			Call("""
			     var page = Models.Page.Of(new { Id = 1 });
			     Expect.That(page).IsEquivalentTo(page);
			     """),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty(probe, \"Total\", o => o.Total);");
		await That(result.Generated).Contains("RegisterCollection(probe);")
			.Because("a collection over an anonymous type cannot be named, so a helper infers it from the probe");
		await That(result.Generated).DoesNotContain("\"Count\"");
	}

	[Test]
	public async Task WhenCollectionDeclaresMembersItself_ShouldRegisterThemAsACollection()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models, Collections,
			Call("Expect.That(new Models.NamedList()).IsEquivalentTo(new Models.NamedList());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.NamedList, string>(\"Name\", o => o.Name);").And
			.Contains("RegisterField<global::Models.NamedList, global::Models.Address>(\"Origin\", o => o.Origin);")
			.Because("the comparison of the items does not cover the members that the collection declares itself");
		await That(result.Generated)
			.Contains("global::aweXpect.Core.Metadata.TypeMetadataRegistry.RegisterCollection<global::Models.NamedList>();")
			.Once()
			.Because("the comparison has to know that only the own members of the collection are registered");
		await That(result.Generated)
			.DoesNotContain("\"Capacity\"").And
			.DoesNotContain("RegisterProperty<global::Models.NamedList, int>(\"Count\"")
			.Because("the members of the framework list describe the collection, which the items cover");
		await That(result.Generated)
			.Contains("RegisterField<global::Models.Address, string>(\"Street\", o => o.Street);")
			.Because("the types of the own members are compared by their members as well");
		await That(result.Generated)
			.Contains("typeof(global::Models.NamedList)")
			.Because("the collection still keeps the interfaces that select the comparison");
	}

	[Test]
	public async Task WhenCollectionDeclaresNoMembersItself_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models, Collections,
			Call("Expect.That(new Models.PlainList()).IsEquivalentTo(new Models.PlainList());"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.DoesNotContain("RegisterCollection").And
			.DoesNotContain("RegisterProperty<global::Models.PlainList")
			.Because("a collection without members of its own is compared by its items alone");
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);");
	}

	[Test]
	public async Task WhenCollectionImplementsAMemberOfAFrameworkInterface_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models, Collections,
			Call("Expect.That(new Models.Page<int>(1)).IsEquivalentTo(new Models.Page<int>(1));"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Page<int>, int>(\"Total\", o => o.Total);").And
			.Contains("RegisterCollection<global::Models.Page<int>>();");
		await That(result.Generated).DoesNotContain("\"Count\"")
			.Because("the Count implements the one of the framework interface, although the type declares it");
	}

	[Test]
	public async Task WhenCoreCannotRegisterCollections_ShouldNotRegisterTheirMembers()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public class NamedList : System.Collections.Generic.List<int> { public string Name { get; set; } = \"\"; } }",
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
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.NamedList))]",
		], false);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("RegisterProperty")
			.Because("an aweXpect.Core without the registration would take the own members for all members of the type");
	}

	[Test]
	public async Task WhenGenerateMetadataAttributeNamesACollectionWithOwnMembers_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models, Collections,
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.NamedList))]",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.GeneratorDiagnostics).IsEmpty();
		await That(result.Generated).Contains("RegisterCollection<global::Models.NamedList>();");
	}

	[Test]
	public async Task WhenGenerateMetadataAttributeNamesACollectionWithoutOwnMembers_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Models, Collections,
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.PlainList))]",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.GeneratorDiagnostics).HasSingle().Which
			.Satisfies(x => x.Id == "aweXpect2001" && x.GetMessage().Contains("'Models.PlainList'"));
	}
}
