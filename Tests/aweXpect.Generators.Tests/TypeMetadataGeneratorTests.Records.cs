namespace aweXpect.Generators.Tests;

public sealed partial class TypeMetadataGeneratorTests
{
	private const string Records = """
	                               namespace Models;

	                               public sealed record Order(int Id, System.Collections.Generic.List<string> Notes);

	                               public record struct Point(int X, int Y);

	                               public sealed record WithOwnToString(int Id)
	                               {
	                               	public override string ToString() => "custom";
	                               }

	                               public sealed record WithOwnPrintMembers(int Id)
	                               {
	                               	private bool PrintMembers(System.Text.StringBuilder builder) => true;
	                               }

	                               public record Box<T>(T Value);

	                               public static class Box
	                               {
	                               	public static Box<T> Of<T>(T value) => new(value);
	                               }

	                               public record Titled(string Title) : System.Collections.Generic.IEnumerable<int>
	                               {
	                               	public System.Collections.Generic.IEnumerator<int> GetEnumerator()
	                               		=> ((System.Collections.Generic.IEnumerable<int>)[]).GetEnumerator();

	                               	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	                               		=> GetEnumerator();
	                               }
	                               """;

	[Test]
	public async Task WhenCoreCannotRegisterRecords_ShouldStillRegisterTheirMembers()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"namespace Models { public sealed record Order(int Id); }",
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
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Order))]",
		], false);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.Order, int>(\"Id\", o => o.Id);");
		await That(result.Generated).DoesNotContain("RegisterRecord")
			.Because("generated code must keep compiling against an aweXpect.Core without the registration");
	}

	[Test]
	public async Task WhenRecordCannotBeNamed_ShouldMarkItThroughAProbe()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Records,
			Call("""
			     var box = Models.Box.Of(new { Id = 1 });
			     Expect.That(box).IsEquivalentTo(box);
			     """),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated).Contains("RegisterRecord(probe);")
			.Because("a record over an anonymous type cannot be named, so a helper infers it from the probe");
	}

	[Test]
	public async Task WhenRecordHasOwnPrintMembers_ShouldNotMarkIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Records,
			Call("Expect.That(new Models.WithOwnPrintMembers(1)).IsEquivalentTo(new Models.WithOwnPrintMembers(1));"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.WithOwnPrintMembers, int>");
		await That(result.Generated).DoesNotContain("RegisterRecord")
			.Because("a user-written PrintMembers is part of the rendering of the record, which the formatter keeps");
	}

	[Test]
	public async Task WhenRecordHasOwnToString_ShouldNotMarkIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Records,
			Call("Expect.That(new Models.WithOwnToString(1)).IsEquivalentTo(new Models.WithOwnToString(1));"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterProperty<global::Models.WithOwnToString, int>");
		await That(result.Generated).DoesNotContain("RegisterRecord")
			.Because("a user-written ToString is the rendering of the record, which the formatter keeps");
	}

	[Test]
	public async Task WhenRecordIsACollection_ShouldNotMarkIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Records,
			Call("Expect.That(new Models.Titled(\"a\")).IsEquivalentTo(new Models.Titled(\"a\"));"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("RegisterCollection<global::Models.Titled>();");
		await That(result.Generated).DoesNotContain("RegisterRecord")
			.Because("a collection is formatted by its items, whatever its own rendering");
	}

	[Test]
	[Arguments("new Models.Order(1, [])", "global::Models.Order")]
	[Arguments("new Models.Point(1, 2)", "global::Models.Point")]
	[Arguments("new Models.Box<int>(1)", "global::Models.Box<int>")]
	public async Task WhenRecordIsCompilerGenerated_ShouldMarkIt(string value, string typeName)
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Records,
			Call($"Expect.That({value}).IsEquivalentTo({value});"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated)
			.Contains($"global::aweXpect.Core.Metadata.TypeMetadataRegistry.RegisterRecord<{typeName}>();").Once()
			.Because("the formatter has to recognize the compiler-generated rendering without reflection");
	}
}
