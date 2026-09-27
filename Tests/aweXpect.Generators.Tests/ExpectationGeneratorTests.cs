using aweXpect.SourceGenerators;

namespace aweXpect.Generators.Tests;

public sealed class ExpectationGeneratorTests
{
	[Fact]
	public async Task WhenTheNameHasNoNotPlaceholder_ShouldEmitOnlyThePositiveOverload()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""");

		await That(result.Generated).Contains(" IsZero(this ").Once();
		await That(result.Generated).DoesNotContain(".Invert()")
			.Because("only a name with {Not} declares a negated overload");
	}

	[Fact]
	public async Task WithNotPlaceholder_ShouldEmitBothPolarities()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("Is{Not}Zero", "{value} == 0", ExpectationText = "is {not} zero")]
			public static partial class ThatInt;
			""");

		await That(result.Generated).Contains(" IsZero(this ").Once();
		await That(result.Generated).Contains(" IsNotZero(this ").Once();
		await That(result.Generated).Contains("///     Verifies that the subject is not zero.").Once();
		await That(result.Generated).Contains("Grammars.Verb(\"is not zero\", \"are not zero\")").Once();
	}

	[Fact]
	public async Task WithSummary_ShouldUseItForThePositiveOverload()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOnNullable<string>("Is{Not}NullOrEmpty", "string.IsNullOrEmpty({value})",
				ExpectationText = "is {not} null or empty",
				Summary = "Verifies that the subject is <see langword=\"null\" /> or <see cref=\"string.Empty\" />.",
				FailOnNull = false,
				NegatedFailsOnNull = true)]
			public static partial class ThatString;
			""");

		await That(result.Generated)
			.Contains("///     Verifies that the subject is <see langword=\"null\" /> or <see cref=\"string.Empty\" />.")
			.Once();
		await That(result.Generated).Contains("///     Verifies that the subject is not null or empty.").Once()
			.Because("the summary only replaces the one of the positive overload");
	}

	private static GeneratorRunner.GeneratorResult Run(string source)
		=> GeneratorRunner.Run(new ExpectationGenerator(), [source,], false);
}
