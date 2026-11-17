namespace aweXpect.Generators.Tests;

public sealed partial class TypeMetadataGeneratorTests
{
	private const string Converters = """
	                                  namespace Models;

	                                  using System.ComponentModel;

	                                  [TypeConverter(typeof(ReleaseDateConverter))]
	                                  public readonly struct ReleaseDate
	                                  {
	                                  }

	                                  public sealed class ReleaseDateConverter : TypeConverter
	                                  {
	                                  }

	                                  [TypeConverter(typeof(GuardedConverter))]
	                                  public readonly struct Guarded
	                                  {
	                                  }

	                                  public sealed class GuardedConverter : TypeConverter
	                                  {
	                                  	private GuardedConverter()
	                                  	{
	                                  	}
	                                  }

	                                  [TypeConverter("Models.ReleaseDateConverter")]
	                                  public readonly struct NamedByString
	                                  {
	                                  }

	                                  [TypeConverter(typeof(AbstractConverter))]
	                                  public readonly struct Abstract
	                                  {
	                                  }

	                                  public abstract class AbstractConverter : TypeConverter
	                                  {
	                                  }

	                                  [TypeConverter(typeof(GenericConverter<int>))]
	                                  public readonly struct Generic
	                                  {
	                                  }

	                                  public sealed class GenericConverter<T> : TypeConverter
	                                  {
	                                  }

	                                  [TypeConverter(typeof(Hidden.Converter))]
	                                  public readonly struct Hidden
	                                  {
	                                  	private sealed class Converter : TypeConverter
	                                  	{
	                                  	}
	                                  }
	                                  """;

	[Test]
	public async Task WhenConverterHasNoPublicParameterlessConstructor_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Converters,
			Call("Expect.That(new { Value = new Models.Guarded(), }).IsEquivalentTo(new { Value = new Models.Guarded(), });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("RegisterConverter")
			.Because("the generated code could not create the converter");
	}

	[Test]
	public async Task WhenConverterIsAbstract_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Converters,
			Call("Expect.That(new { Value = new Models.Abstract(), }).IsEquivalentTo(new { Value = new Models.Abstract(), });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("RegisterConverter")
			.Because("the generated code cannot create an abstract converter");
	}

	[Test]
	public async Task WhenConverterIsGeneric_ShouldRegisterTheConstructedConverter()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Converters,
			Call("Expect.That(new { Value = new Models.Generic(), }).IsEquivalentTo(new { Value = new Models.Generic(), });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains(
			"global::aweXpect.Core.Metadata.TypeMetadataRegistry.RegisterConverter<global::Models.Generic, global::Models.GenericConverter<int>>();");
	}


	[Test]
	public async Task WhenConverterIsNamedByString_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Converters,
			Call("Expect.That(new { Value = new Models.NamedByString(), }).IsEquivalentTo(new { Value = new Models.NamedByString(), });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("RegisterConverter")
			.Because("only a converter named by type can be resolved at compile time");
	}

	[Test]
	public async Task WhenConverterIsNotAccessible_ShouldNotRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Converters,
			Call("Expect.That(new { Value = new Models.Hidden(), }).IsEquivalentTo(new { Value = new Models.Hidden(), });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("RegisterConverter")
			.Because("the generated code cannot name a private converter");
	}

	[Test]
	public async Task WhenCoreCannotRegisterConverters_ShouldNotRegisterThem()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Converters,
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
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.ReleaseDate))]",
		], false);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("RegisterConverter")
			.Because("generated code must keep compiling against an aweXpect.Core without the registration");
	}

	[Test]
	public async Task WhenMemberTypeDeclaresConverter_ShouldRegisterIt()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Converters,
			Call("Expect.That(new { Value = new Models.ReleaseDate(), }).IsEquivalentTo(new { Value = new Models.ReleaseDate(), });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty();
		await That(result.Generated).Contains(
			"global::aweXpect.Core.Metadata.TypeMetadataRegistry.RegisterConverter<global::Models.ReleaseDate, global::Models.ReleaseDateConverter>();");
	}

	[Test]
	public async Task WhenTypeIsInFrameworkNamespace_ShouldNotRegisterItsConverter()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			"""
			namespace Microsoft.Models;

			using System.ComponentModel;

			[TypeConverter(typeof(FrameworkConverter))]
			public readonly struct Framework
			{
			}

			public sealed class FrameworkConverter : TypeConverter
			{
			}
			""",
			Call("Expect.That(new { Value = new Microsoft.Models.Framework(), }).IsEquivalentTo(new { Value = new Microsoft.Models.Framework(), });"),
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).DoesNotContain("RegisterConverter")
			.Because("the comparison never converts a type of the framework");
	}

	[Test]
	public async Task WhenTypeIsRegisteredByAttribute_ShouldRegisterItsConverter()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
		[
			Converters,
			"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.ReleaseDate))]",
		]);

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty()
			.Because("the converter is a registration, even for a type without members");
		await That(result.Generated).Contains(
			"RegisterConverter<global::Models.ReleaseDate, global::Models.ReleaseDateConverter>();");
	}
}
