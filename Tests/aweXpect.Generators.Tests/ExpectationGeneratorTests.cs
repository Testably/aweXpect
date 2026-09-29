using System.Linq;
using aweXpect.SourceGenerators;
using Microsoft.CodeAnalysis;

namespace aweXpect.Generators.Tests;

public sealed class ExpectationGeneratorTests
{
	[Fact]
	public async Task WhenClassHasNoNamespace_ShouldEmitItWithoutANamespace()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""");

		await That(result.Generated).Contains("public static partial class ThatInt").Once();
		await That(result.Generated).DoesNotContain("namespace <global namespace>")
			.Because("a class in the global namespace has no namespace to declare");
	}

	[Fact]
	public async Task WhenClassIsDeclaredInSeveralParts_ShouldEmitEachExpectationOnce()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(new ExpectationGenerator(),
		[
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""",
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsOne", "{value} == 1", ExpectationText = "is one")]
			public static partial class ThatInt;
			""",
		], false);

		await That(result.Generated).Contains(" IsZero(this ").Once()
			.Because("every part only contributes the expectations of its own attributes");
		await That(result.Generated).Contains(" IsOne(this ").Once();
	}

	[Fact]
	public async Task WhenClassIsInternal_ShouldKeepItsAccessibility()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			internal static partial class ThatInt;
			""");

		await That(result.Generated).Contains("internal static partial class ThatInt").Once()
			.Because("the parts of a partial class must not declare conflicting accessibilities");
	}

	[Fact]
	public async Task WhenClassIsNested_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			public static partial class Outer
			{
				[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
				public static partial class ThatInt;
			}
			""");

		await That(result.Generated).DoesNotContain(" IsZero(this ")
			.Because("extension methods can only be declared in a top-level class");
		await That(result.GeneratorDiagnostics).HasSingle().Which
			.Satisfies(x => x.Id == "aweXpect3005" && x.Severity == DiagnosticSeverity.Error &&
			                x.GetMessage() ==
			                "'Lib.Outer.ThatInt' cannot hold the generated expectations, because extension methods need a top-level static class");
	}

	[Fact]
	public async Task WhenCompilationChanges_ShouldCacheTheExpectations()
	{
		GeneratorDriverRunResult result = GeneratorRunner.RunTwice(new ExpectationGenerator(),
		[
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero", Using = ["System.Text"])]
			public static partial class ThatInt;
			""",
		], "public class Unrelated;", false);

		IncrementalStepRunReason[] reasons = result.Results[0].TrackedSteps["Expectations"]
			.SelectMany(x => x.Outputs).Select(x => x.Reason).ToArray();
		await That(reasons).IsNotEmpty();
		await That(reasons).All()
			.Satisfy(x => x is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged)
			.Because("an unchanged declaration has to compare equal, so that its output is not produced again");
	}

	[Fact]
	public async Task WhenCompilationChanges_ShouldKeepTheGeneratedSources()
	{
		GeneratorDriverRunResult result = GeneratorRunner.RunTwice(new ExpectationGenerator(),
		[
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""",
		], "public class Unrelated;", false);

		await That(string.Concat(result.GeneratedTrees.Select(x => x.ToString()))).Contains(" IsZero(this ").Once()
			.Because("an edit elsewhere must not drop the output of an unchanged declaration");
	}

	[Fact]
	public async Task WhenExpectationTextContainsQuotesAndBackslashes_ShouldEscapeThem()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<string>("IsQuoted", "{value} == \"\\\"\"", ExpectationText = "is {not} \"\\\"")]
			public static partial class ThatString;
			""");

		await That(result.Generated).Contains("""Grammars.Verb("is \"\\\"", "are \"\\\"")""").Once()
			.Because("the expectation text is embedded in a string literal");
		await That(result.Generated).Contains("""Grammars.Verb("is not \"\\\"", "are not \"\\\"")""").Once();
	}

	[Fact]
	public async Task WhenExpectationTextContainsXmlCharacters_ShouldEscapeThemInTheSummary()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("Is{Not}Between", "{value} is > 0 and < 10", ExpectationText = "is {not} > 0 & < 10")]
			public static partial class ThatInt;
			""");

		await That(result.Generated).Contains("///     Verifies that the subject is &gt; 0 &amp; &lt; 10.").Once()
			.Because("the expectation text is plain text, but the summary is XML");
		await That(result.Generated).Contains("///     Verifies that the subject is not &gt; 0 &amp; &lt; 10.").Once();
	}

	[Fact]
	public async Task WhenSameNamedClassesAreInDifferentNamespaces_ShouldEmitBoth()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(new ExpectationGenerator(),
		[
			"""
			using aweXpect.SourceGenerators;

			namespace Lib.A;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""",
			"""
			using aweXpect.SourceGenerators;

			namespace Lib.B;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""",
		], false);

		await That(result.GeneratorDiagnostics).IsEmpty();
		await That(result.Generated).Contains(" IsZero(this ").Exactly(2)
			.Because("the hint name has to tell the two classes apart");
		await That(result.Generated).Contains("namespace Lib.A;").Once();
		await That(result.Generated).Contains("namespace Lib.B;").Once();
	}

	[Fact]
	public async Task WhenSummaryHasSeveralLines_ShouldContinueTheDocumentationComment()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOnNullable<string>("IsEmpty", "{value} == \"\"", ExpectationText = "is empty",
				Summary = "Verifies that the subject\nis empty.")]
			public static partial class ThatString;
			""");

		await That(result.Generated).Contains("\t///     is empty.").Once()
			.Because("every line of the summary has to stay inside the documentation comment");
	}

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
