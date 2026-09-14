namespace aweXpect.Generators.Tests;

public sealed class TypeMetadataGeneratorTests
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
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(sources:
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Other());"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("[System.Runtime.CompilerServices.ModuleInitializer]")
			.Because("the metadata has to be registered before any code of the assembly compares anything");
	}

	[Fact]
	public async Task WhenArgumentIsAnonymous_ShouldRegisterThroughAProbe()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(sources:
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
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(sources:
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
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(sources:
			[Models, Call("Expect.That(new Models.Other()).IsEquivalentTo(new Models.Subject());"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the comparison fetches the members of the subject by name, so it needs the subject registered");
	}

	[Fact]
	public async Task WhenCoreIsNotReferenced_ShouldNotEmit()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(false, Models);

		await That(result.Generated).IsEmpty()
			.Because("a registration against a Core without the registry would not compile");
	}

	[Fact]
	public async Task WhenGenerateMetadataAttributeIsApplied_ShouldRegisterTheType()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(sources:
			[Models, "[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Other))]",]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the attribute is the escape hatch for a type no marked call site reveals");
	}

	[Fact]
	public async Task WhenNothingIsMarked_ShouldNotEmit()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(sources:
			[Models, Call("Expect.That(new Models.Other()).IsNotNull();"),]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).IsEmpty();
	}

	[Fact]
	public async Task WhenTypeIsEnumerable_ShouldRegisterTheElementTypeInstead()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(sources:
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
	public async Task WhenTypeIsNotAccessible_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(sources:
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
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(sources:
		[
			Models,
			Call("Expect.That(new[] { new Models.Other(), }).Contains(new Models.Other()).Equivalent();"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("RegisterProperty<global::Models.Other, int>(\"Count\", o => o.Count);")
			.Because("the expected value reached the comparison through an earlier call, so only the type argument names it");
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
}
