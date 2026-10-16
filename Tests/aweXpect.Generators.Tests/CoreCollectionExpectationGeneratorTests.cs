using System.Linq;
using aweXpect.Core.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.Generators.Tests;

public sealed class CoreCollectionExpectationGeneratorTests
{
	[Test]
	public async Task BothAttributes_ShouldGenerateBothExpectations()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains(" IsZero(this global::aweXpect.Core.IThat<int> subject)");
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableElements<int> elements)");
	}

	[Test]
	[Arguments("is {not} zero", "is zero")]
	[Arguments("is zero", "is zero")]
	public async Task BothAttributes_WithoutExpectationText_ShouldUseTheExpectationTextWithoutNot(
		string expectationText, string expectedText)
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			$$"""
			  [GenerateCollectionExpectation("AreZero", "are zero")]
			  [GenerateExpectation("Is{Not}Zero", "{{expectationText}}", NegatedExpectationText = "is not zero")]
			  private static bool IsZero(int value) => value == 0;
			  """);

		await ThatCompiles(result);
		await That(result.Generated).Contains($"stringBuilder.Append(\"{expectedText}\");").Exactly(4);
		await That(result.Generated).Contains("stringBuilder.Append(\"are zero\");").Exactly(4);
	}

	[Test]
	public async Task CancellationToken_ShouldBePassedToThePredicate()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero", NullableOverload = false)]
			private static bool IsZero(int value, [EvaluationCancellationToken] CancellationToken cancellationToken) => value == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("global::Lib.ThatSample.IsZero(item, cancellationToken)");
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableElements<int> elements)")
			.Because("the cancellation token is no parameter of the expectation");
	}

	[Test]
	public async Task ClassIsNotPartial_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.Core.Extending;

			namespace Lib;

			public static class ThatSample
			{
				[GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
				private static bool IsZero(int value) => value == 0;
			}
			""");

		await ThatReports(result,
			"aweXpect4001: 'Lib.ThatSample' cannot hold the generated expectation, because extension methods need a top-level, non-generic, static partial class that is not file-local");
	}

	[Test]
	public async Task EvaluationCancellationToken_BeforeAParamsParameter_ShouldBePassedAtItsPosition()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreAnyOf", "is any of {values}", "are any of {values}", NullableOverload = false)]
			private static bool IsAnyOf(int value, [EvaluationCancellationToken] CancellationToken cancellationToken,
				params int[] values) => Array.IndexOf(values, value) >= 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains(" AreAnyOf(this global::aweXpect.IEnumerableElements<int> elements, params int[] values)");
		await That(result.Generated).Contains("global::Lib.ThatSample.IsAnyOf(item, cancellationToken, _p0)").Exactly(2);
	}

	[Test]
	public async Task ExpectationTextContainsNot_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is {not} zero", "are {not} zero")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because the expectation text contains '{not}', but the quantifier carries the negation");
	}

	[Test]
	public async Task ExpectationTextIsMissing_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "are zero")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because the expectation text is required, unless the method is also marked with [GenerateExpectation]");
	}

	[Test]
	public async Task LanguageVersionIsCSharp8_ShouldCompile()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using System.Diagnostics.CodeAnalysis;
			using System.Threading;
			using System.Threading.Tasks;
			using aweXpect.Core.Extending;

			namespace Lib
			{
				public static partial class ThatSample
				{
					[GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
					private static bool IsZero(int value) => value == 0;

					[GenerateCollectionExpectation("AreNamed", "is named {name}", "are named {name}")]
					private static bool IsNamed(string value, string name) => value == name;

					[GenerateCollectionExpectation("AreEmpty", "is empty", "are empty")]
					private static bool IsEmpty(string? value) => value == "";

					[GenerateCollectionExpectation("AreReady", "is ready", "are ready")]
					private static Task<bool> IsReady(string value, [EvaluationCancellationToken] CancellationToken cancellationToken) => Task.FromResult(true);

					[GenerateCollectionExpectation("AreOdd", "is odd", "are odd")]
					private static ValueTask<bool> IsOdd([DisallowNull] int? value) => new ValueTask<bool>(value % 2 == 1);

					[GenerateCollectionExpectation("AreKnown", "is known", "are known")]
					private static Task<PredicateResult> IsKnown(string value)
						=> Task.FromResult(value.Length == 0 ? PredicateResult.FailureBothWays("was empty") : true);
				}
			}
			""", LanguageVersion.CSharp8);

		await ThatCompiles(result);
	}

	[Test]
	public async Task MethodReturnsNoBoolean_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
			private static int IsZero(int value) => value;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because it returns neither bool nor PredicateResult, nor a Task or ValueTask of them");
	}

	[Test]
	public async Task NameIsInvalid_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("Are{Not}Zero", "is zero", "are zero")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because the name 'Are{Not}Zero' is not a valid method name");
	}

	[Test]
	public async Task NameIsNull_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation(null!, "are zero")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because the name and the plural expectation text are required");
	}

	[Test]
	public async Task NullableOverloadIsFalse_ShouldOnlyExtendTheValueType()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero", NullableOverload = false)]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).DoesNotContain("int?");
	}

	[Test]
	public async Task NullableReferenceType_ShouldLetThePredicateDecide()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreEmpty", "is empty", "are empty")]
			private static bool IsEmpty(string? value) => value == "";
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains("=> new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::Lib.ThatSample.IsEmpty(item));")
			.Exactly(2);
	}

	[Test]
	public async Task NullableValueType_ShouldLetThePredicateDecide()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
			private static bool IsZero(int? value) => value == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableElements<int?> elements)");
		await That(result.Generated)
			.Contains("=> new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::Lib.ThatSample.IsZero(item));")
			.Exactly(2);
	}

	[Test]
	public async Task NullableValueTypeWithDisallowNull_ShouldNotMatchNull()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
			private static bool IsZero([DisallowNull] int? value) => value == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableElements<int?> elements)");
		await That(result.Generated).Contains("!(item is null) && global::Lib.ThatSample.IsZero(item.Value)").Exactly(2);
		await That(result.Generated).DoesNotContain("IEnumerableElements<int>");
	}

	[Test]
	public async Task OverloadedPredicates_ShouldGenerateDistinctConstraints()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero", NullableOverload = false)]
			private static bool IsZero(int value) => value == 0;

			[GenerateCollectionExpectation("AreZero", "is zero", "are zero", NullableOverload = false)]
			private static bool IsZero(long value) => value == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("class IsZero_GeneratedElementsConstraint<").Once();
		await That(result.Generated).Contains("class IsZero_1_GeneratedElementsConstraint<").Once();
	}

	[Test]
	public async Task Parameters_ShouldBeCheckedAndPassedToThePredicate()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreNamed", "is named {name} or {alias}", "are named {name} or {alias}")]
			private static bool IsNamed(string value, string name, string? alias) => value == name || value == alias;
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains("AddConstraint((elements.Quantifier, name, alias), (__state, __it, __grammars) => new ")
			.Exactly(3);
		await That(result.Generated).Contains("(__it, __grammars, __state.Item1, __state.Item2, __state.Item3)")
			.Exactly(3);
		await That(result.Generated).Contains("throw global::aweXpect.Core.Tracing.WriteException(new global::System.ArgumentNullException(nameof(name)")
			.Exactly(3);
		await That(result.Generated).DoesNotContain("nameof(alias)")
			.Because("a nullable parameter accepts null");
		await That(result.Generated).Contains("global::Lib.ThatSample.IsNamed(item, _p0, _p1)");
	}

	[Test]
	public async Task ParamsParameter_ShouldStayParams()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreAnyOf", "is any of {values}", "are any of {values}", NullableOverload = false)]
			private static bool IsAnyOf(int value, params int[] values) => Array.IndexOf(values, value) >= 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains(" AreAnyOf(this global::aweXpect.IEnumerableElements<int> elements, params int[] values)");
	}

	[Test]
	[Arguments("is equal to {expected}", "are equal to {other}")]
	[Arguments("is equal to {other}", "are equal to {expected}")]
	public async Task PlaceholderNamesNoParameter_ShouldReportADiagnostic(string text, string pluralText)
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			$$"""
			  [GenerateCollectionExpectation("AreEqual", "{{text}}", "{{pluralText}}")]
			  private static bool IsEqual(int value, int expected) => value == expected;
			  """);

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsEqual(int, int)' cannot generate an expectation, because the placeholder '{other}' names no parameter of the expectation");
	}

	[Test]
	public async Task PredicateResult_ShouldBeWrapped()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreEmpty", "is empty", "are empty")]
			private static PredicateResult IsEmpty(string value)
				=> value.Length > 9 ? PredicateResult.FailureBothWays("was too long") : value.Length == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains("=> item is null ? new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(false) : new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::Lib.ThatSample.IsEmpty(item));")
			.Exactly(2);
	}

	[Test]
	public async Task ReferenceType_ShouldExtendEveryKindOfCollection()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.Core.Extending;

			namespace Lib;

			public sealed class Track(int duration)
			{
				public int Duration { get; } = duration;
			}

			public static partial class ThatTrack
			{
				[GenerateCollectionExpectation("AreLong", "is long", "are long")]
				private static bool IsLong(Track track) => track.Duration > 1;
			}
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains("public static global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IEnumerable<global::Lib.Track?>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<global::Lib.Track?>?>> AreLong(this global::aweXpect.IEnumerableElements<global::Lib.Track?> elements)");
		await That(result.Generated)
			.Contains("public static global::aweXpect.Results.AndOrResult<TEnumerable, global::aweXpect.Core.IThat<TEnumerable>> AreLong<TEnumerable>(this global::aweXpect.IStructEnumerableElements<TEnumerable, global::Lib.Track?> elements)");
		await That(result.Generated)
			.Contains("public static global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IAsyncEnumerable<global::Lib.Track?>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IAsyncEnumerable<global::Lib.Track?>?>> AreLong(this global::aweXpect.IAsyncEnumerableElements<global::Lib.Track?> elements)");
		await That(result.Generated).Contains("!(item is null) && global::Lib.ThatTrack.IsLong(item)").Exactly(2)
			.Because("a null item does not match");
	}

	[Test]
	public async Task String_ShouldExtendTheStringElements()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreEmpty", "is empty", "are empty")]
			private static bool IsEmpty(string value) => value.Length == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains(" AreEmpty(this global::aweXpect.IEnumerableStringElements elements)");
		await That(result.Generated)
			.Contains(" AreEmpty<TEnumerable>(this global::aweXpect.IStructEnumerableStringElements<TEnumerable> elements)");
		await That(result.Generated).Contains(" AreEmpty(this global::aweXpect.IAsyncEnumerableStringElements elements)");
		await That(result.Generated).DoesNotContain("IEnumerableElements<");
	}

	[Test]
	public async Task Summary_ShouldBeDerivedFromThePluralText()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreAbove", "is above {minimum}", "are above {minimum}",
				NullableOverload = false, Remarks = "Compares the values.")]
			private static bool IsAbove(int value, int minimum) => value > minimum;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				/// <summary>
				///     …are above <paramref name="minimum" />.
				/// </summary>
				/// <remarks>
				///     Compares the values.
				/// </remarks>
			""");
	}

	[Test]
	public async Task Texts_ShouldBeUsedForTheGrammaticalNumber()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreSingleDigits", "is a single digit", "are single digits", NullableOverload = false)]
			private static bool IsSingleDigit(int value) => value < 10;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
						if (global::aweXpect.Core.ExpectationGrammarsExtensions.IsPlural(grammars))
						{
							stringBuilder.Append("are single digits");
						}
						else
						{
							stringBuilder.Append("is a single digit");
						}
			""");
	}

	[Test]
	public async Task ValueTaskOfBoolPredicate_ShouldBeAwaited()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero", NullableOverload = false)]
			private static ValueTask<bool> IsZero(int value) => new(value == 0);
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("protected override async global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult> IsMatch(").Exactly(2);
		await That(result.Generated).Contains("=> await global::Lib.ThatSample.IsZero(item).ConfigureAwait(false);").Exactly(2)
			.Because("only the awaited bool converts to a PredicateResult");
	}

	[Test]
	public async Task ValueTaskOfPredicateResultPredicate_ShouldBeReturnedDirectly()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
			private static ValueTask<PredicateResult> IsZero(int value) => new(value == 0);
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("=> global::Lib.ThatSample.IsZero(item);").Exactly(2);
		await That(result.Generated)
			.Contains("=> item is null ? new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(false) : global::Lib.ThatSample.IsZero(item.Value);")
			.Exactly(2)
			.Because("a null item of the nullable overload does not match");
		await That(result.Generated).DoesNotContain("async ");
	}

	[Test]
	public async Task ValueType_ShouldAlsoExtendTheNullableType()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
			private static Task<bool> IsZero(int value) => Task.FromResult(value == 0);
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableElements<int> elements)");
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableElements<int?> elements)");
		await That(result.Generated)
			.Contains("=> item is null ? false : await global::Lib.ThatSample.IsZero(item.Value).ConfigureAwait(false);")
			.Exactly(2);
	}

	[Test]
	[Arguments("is zero", "were")]
	[Arguments("are zero", "were")]
	[Arguments("has zero", "did")]
	[Arguments("was zero", "were")]
	[Arguments("were zero", "were")]
	[Arguments("zeroes", "did")]
	[Arguments("{value} is zero", "did")]
	public async Task Verb_ShouldDependOnTheLeadingVerb(string text, string expectedVerb)
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			$$"""
			  [GenerateCollectionExpectation("AreZero", "{{text}}", "{{text}}", NullableOverload = false)]
			  private static bool IsZero(int item, int value) => item == value;
			  """);

		await ThatCompiles(result);
		await That(result.Generated).Contains($"protected override string Verb => \"{expectedVerb}\";").Exactly(2);
	}

	[Test]
	public async Task WhenClassHasNoNamespace_ShouldCompile()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.Core.Extending;

			public static partial class ThatSample
			{
				[GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
				private static bool IsZero(int value) => value == 0;
			}
			""");

		await ThatCompiles(result);
		await That(result.Generated).DoesNotContain("namespace ");
		await That(result.Generated).Contains("global::ThatSample.IsZero(item)");
	}

	[Test]
	public async Task WhenOnlyTouched_ShouldReuseTheCachedExpectation()
	{
		GeneratorDriverRunResult result = GeneratorRunner.RunTwice(new ExpectationGenerator(),
		[
			"""
			using aweXpect.Core.Extending;

			namespace Lib;

			public static partial class ThatSample
			{
				[GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
				private static bool IsZero(int value) => value == 0;
			}
			""",
		], "// unrelated");

		IncrementalStepRunReason[] reasons = result.Results[0].TrackedSteps["CollectionExpectations"]
			.SelectMany(x => x.Outputs).Select(x => x.Reason).ToArray();
		await That(reasons).IsNotEmpty();
		await That(reasons).All()
			.Satisfy(x => x is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged)
			.Because("the model is compared by value, so an unrelated edit does not regenerate the expectation");
	}

	private static GeneratorRunner.GeneratorResult Run(string source,
		LanguageVersion languageVersion = LanguageVersion.Latest)
		=> GeneratorRunner.Run(new ExpectationGenerator(), [source,], true, languageVersion);

	private static GeneratorRunner.GeneratorResult RunOn(string members)
		=> Run($$"""
		         using System;
		         using System.Diagnostics.CodeAnalysis;
		         using System.Threading;
		         using System.Threading.Tasks;
		         using aweXpect.Core.Extending;

		         namespace Lib;

		         public static partial class ThatSample
		         {
		         {{members}}
		         }
		         """);

	private static async Task ThatCompiles(GeneratorRunner.GeneratorResult result)
	{
		await That(result.GeneratorDiagnostics).IsEmpty();
		await That(result.Errors.Select(x => x.ToString())).IsEmpty();
		await That(result.Warnings.Select(x => x.ToString())).IsEmpty()
			.Because("the generated code has to compile without warnings in projects that treat them as errors");
	}

	private static async Task ThatContainsLines(GeneratorRunner.GeneratorResult result, string expected)
		=> await That(result.Generated.Replace("\r\n", "\n")).Contains(expected.Replace("\r\n", "\n"));

	private static async Task ThatReports(GeneratorRunner.GeneratorResult result, string diagnostic)
	{
		await That(result.GeneratorDiagnostics.Select(x => $"{x.Id}: {x.GetMessage()}")).IsEqualTo([diagnostic,]);
		await That(result.Generated).IsEmpty();
	}
}
