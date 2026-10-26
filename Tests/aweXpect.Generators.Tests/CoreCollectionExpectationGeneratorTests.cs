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
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableItems<int> items)");
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
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableItems<int> items)")
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
	public async Task CollectionOverload_ShouldGenerateTheExpectationThatAllItemsMeetOnTheCollection()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreEmpty", "is empty", "are empty", CollectionOverload = true, Remarks = "Ignores whitespace.")]
			private static bool IsEmpty(string value) => value.Trim().Length == 0;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				/// <summary>
				///     Verifies that all items are empty.
				/// </summary>
				/// <remarks>
				///     Ignores whitespace.
				/// </remarks>
				[global::aweXpect.Core.GuaranteesNotNull]
				public static global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IEnumerable<string?>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<string?>?>> AreEmpty(this global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<string?>?> subject)
				{
					return new global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IEnumerable<string?>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<string?>?>>(global::aweXpect.Core.Extending.ThatExtensions.Get(subject).ExpectationBuilder.AddConstraint((__it, __grammars) => new IsEmpty_GeneratedItemsConstraint<global::System.Collections.Generic.IEnumerable<string?>?>(__it, __grammars, global::aweXpect.Options.EnumerableQuantifier.All(), true)),
						subject);
				}
			""");
		await That(result.Generated)
			.Contains("new IsEmpty_GeneratedAsyncItemsConstraint(__it, __grammars, global::aweXpect.Options.EnumerableQuantifier.All(), true)");
		await That(result.Generated).Contains("(__it, __grammars, __state, false)").Exactly(3)
			.Because("the expectations on the elements are not on the collection");
		await That(result.Generated).Contains(": base(it, grammars, quantifier, isOnCollection)").Exactly(2);
		await That(result.Generated).DoesNotContain(" AreNotEmpty(")
			.Because("the negated overload on the collection is only generated with a negated name");
	}

	[Test]
	public async Task CollectionOverload_WithParameters_ShouldPassThemToTheConstraint()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreBetween", "is between {minimum} and {maximum}", "are between {minimum} and {maximum}",
				NullableOverload = false, CollectionOverload = true, NegatedName = "AreNotBetween")]
			private static bool IsBetween(int value, int minimum, int maximum) => value >= minimum && value <= maximum;
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains(" AreBetween(this global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<int>?> subject, int minimum, int maximum)");
		await That(result.Generated)
			.Contains("AddConstraint((minimum, maximum), (__state, __it, __grammars) => new IsBetween_GeneratedItemsConstraint<global::System.Collections.Generic.IEnumerable<int>?>(__it, __grammars, global::aweXpect.Options.EnumerableQuantifier.All(), true, __state.Item1, __state.Item2))");
		await That(result.Generated)
			.Contains("(__it, __grammars, global::aweXpect.Options.EnumerableQuantifier.None(global::aweXpect.Core.ExpectationGrammars.Nested), true, __state.Item1, __state.Item2)")
			.Exactly(2);
		await That(result.Generated).Contains("(__it, __grammars, __state.Item1, false, __state.Item2, __state.Item3)")
			.Exactly(3);
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
			.Contains(" AreAnyOf(this global::aweXpect.IEnumerableItems<int> items, params int[] values)");
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
	public async Task FormattedItem_OfANullableValueTypeItem_ShouldKeepNull()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero", FormattedItem = nameof(Describe))]
			private static bool IsZero(int value) => value == 0;

			private static string Describe(int value) => $"#{value}";
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains("item => (object?)global::Lib.ThatSample.Describe(item)").Exactly(2);
		await That(result.Generated)
			.Contains("item => item is null ? null : (object?)global::Lib.ThatSample.Describe(item.Value)").Exactly(2)
			.Because("the method takes the value type, so a null item of the nullable overload stays null");
	}

	[Test]
	public async Task FormattedItem_ShouldFormatTheItemsWithTheMethod()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreEmpty", "is empty", "are empty", FormattedItem = nameof(Describe))]
			private static bool IsEmpty(string value) => value.Length == 0;

			private static object? Describe(string? value) => value?.Length;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
					protected override string FormatItems(global::System.Collections.Generic.IEnumerable<string?> items, global::aweXpect.Formatting.FormattingOptions options)
						=> global::aweXpect.Formatting.ValueFormatters.Format<object?>(global::aweXpect.Formatting.Format.Formatter, global::System.Linq.Enumerable.Select<string?, object?>(items, item => (object?)global::Lib.ThatSample.Describe(item)), options);
			""");
	}

	[Test]
	[Arguments("private static object? Describe(string value) => value;")]
	[Arguments("private static void Describe(string? value) { }")]
	[Arguments("private static object? Describe(int value) => value;")]
	[Arguments("private object? Describe(string? value) => value;")]
	[Arguments("private static object? Describe(string? value, int other) => value;")]
	public async Task FormattedItemIsNotSuitable_ShouldReportADiagnostic(string method)
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			$$"""
			  [GenerateCollectionExpectation("AreEmpty", "is empty", "are empty", FormattedItem = "Describe")]
			  private static bool IsEmpty(string value) => value.Length == 0;

			  {{method}}
			  """);

		await ThatReports(result,
			"aweXpect4004: 'Describe' names no static method that returns a value and takes the item of 'ThatSample.IsEmpty(string)', which can be null for a reference type");
	}

	[Test]
	public async Task ItemTypeParameter_ShouldGenerateGenericExpectations()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("ArePublic", "is public", "are public",
				CollectionOverload = true, NegatedName = "AreNotPublic", FormattedItem = nameof(Describe))]
			private static bool IsPublic<TMember>(TMember member) where TMember : System.Reflection.MemberInfo
				=> member.Name.Length > 0;

			private static object? Describe(System.Reflection.MemberInfo? member) => member?.Name;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				public static global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IEnumerable<TMember?>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<TMember?>?>> ArePublic<TMember>(this global::aweXpect.IEnumerableItems<TMember?> items)
					where TMember : global::System.Reflection.MemberInfo
			""");
		await ThatContainsLines(result,
			"""
				public static global::aweXpect.Results.AndOrResult<TCollection, global::aweXpect.Core.IThat<TCollection>> ArePublic<TCollection, TMember>(this global::aweXpect.IStructEnumerableItems<TCollection, TMember?> items)
					where TCollection : struct, global::System.Collections.Generic.IEnumerable<TMember?>
					where TMember : global::System.Reflection.MemberInfo
			""");
		await That(result.Generated)
			.Contains(" AreNotPublic<TMember>(this global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<TMember?>?> subject)");
		await That(result.Generated)
			.Contains("new IsPublic_GeneratedItemsConstraint<global::System.Collections.Generic.IEnumerable<TMember?>?, TMember>(");
		await That(result.Generated).Contains("new IsPublic_GeneratedAsyncItemsConstraint<TMember>(");
		await ThatContainsLines(result,
			"""
				private sealed class IsPublic_GeneratedItemsConstraint<TCollection, TMember> : global::aweXpect.Core.Extending.GeneratedCollectionConstraint<TCollection, TMember?>
					where TCollection : global::System.Collections.Generic.IEnumerable<TMember?>?
					where TMember : global::System.Reflection.MemberInfo
			""");
		await That(result.Generated).Contains("!(item is null) && global::Lib.ThatSample.IsPublic<TMember>(item)")
			.Exactly(2);
		await That(result.Generated).Contains("item => (object?)global::Lib.ThatSample.Describe(item)").Exactly(2);
	}

	[Test]
	[Arguments("private static bool IsZero<T>(T value) => value is null;")]
	[Arguments("private static bool IsZero<T>(int value) where T : class => value == 0;")]
	[Arguments("private static bool IsZero<T, TOther>(T value) where T : class => value is null;")]
	[Arguments("private static bool IsZero<T>(T value, T other) where T : class => value == other;")]
	[Arguments("private static bool IsZero<TCollection>(TCollection value) where TCollection : class => value is null;")]
	public async Task ItemTypeParameterIsNotSupported_ShouldReportADiagnostic(string method)
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			$$"""
			  [GenerateCollectionExpectation("AreZero", "is zero", "are zero")]
			  {{method}}
			  """);

		await That(result.GeneratorDiagnostics.Select(x => $"{x.Id}: {x.GetMessage()}"))
			.HasSingle().Which
			.EndsWith("cannot generate an expectation, because it is generic, but not only in the type of the item, which a reference type constraint restricts");
		await That(result.Generated).IsEmpty();
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

					[GenerateCollectionExpectation("AreLong", "is long", "are long", CollectionOverload = true,
						NegatedName = "AreNotLong", FormattedItem = nameof(Describe))]
					private static bool IsLong(string value, int minimum) => value.Length > minimum;

					[GenerateCollectionExpectation("AreSmall", "is small", "are small", CollectionOverload = true,
						NegatedName = "AreNotSmall", FormattedItem = nameof(Describe))]
					private static bool IsSmall(int value) => value < 3;

					[GenerateCollectionExpectation("AreSet", "is set", "are set", CollectionOverload = true,
						NegatedName = "AreUnset", FormattedItem = nameof(Describe))]
					private static bool IsSet<TItem>(TItem? item) where TItem : class => item != null;

					private static object? Describe(string? value) => value?.Length;

					private static object Describe(int value) => value;

					private static object? Describe(object? value) => value;
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
	public async Task NegatedName_ShouldGenerateTheExpectationThatNoItemMeetsOnTheCollection()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreZero", "is zero", "are zero", NullableOverload = false, NegatedName = "AreNotZero")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				/// <summary>
				///     Verifies that no items are zero.
				/// </summary>
				[global::aweXpect.Core.GuaranteesNotNull]
				public static global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IEnumerable<int>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<int>?>> AreNotZero(this global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<int>?> subject)
				{
					return new global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IEnumerable<int>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<int>?>>(global::aweXpect.Core.Extending.ThatExtensions.Get(subject).ExpectationBuilder.AddConstraint((__it, __grammars) => new IsZero_GeneratedItemsConstraint<global::System.Collections.Generic.IEnumerable<int>?>(__it, __grammars, global::aweXpect.Options.EnumerableQuantifier.None(global::aweXpect.Core.ExpectationGrammars.Nested), true)),
						subject);
				}
			""");
		await That(result.Generated)
			.Contains(" AreNotZero(this global::aweXpect.Core.IThat<global::System.Collections.Generic.IAsyncEnumerable<int>?> subject)");
		await That(result.Generated).DoesNotContain(" AreZero(this global::aweXpect.Core.IThat<")
			.Because("only the negated name generates an expectation on the collection");
	}

	[Test]
	public async Task NegatedName_WithNegatedParameterName_ShouldNameTheParameterOfTheNegatedExpectation()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreNamed", "is named {expected}", "are named {expected}",
				CollectionOverload = true, NegatedName = "AreNotNamed",
				Remarks = "The <paramref name=\"expected\" /> name is compared exactly.")]
			private static bool IsNamed(string value, [NegatedParameterName("unexpected")] string expected) => value == expected;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				/// <summary>
				///     Verifies that no items are named <paramref name="unexpected" />.
				/// </summary>
				/// <remarks>
				///     The <paramref name="unexpected" /> name is compared exactly.
				/// </remarks>
				[global::aweXpect.Core.GuaranteesNotNull]
				public static global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IEnumerable<string?>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<string?>?>> AreNotNamed(this global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<string?>?> subject, string unexpected)
				{
					if (unexpected is null)
					{
						throw global::aweXpect.Core.Tracing.WriteException(new global::System.ArgumentNullException(nameof(unexpected), "The 'unexpected' value cannot be null."));
					}

					return new global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IEnumerable<string?>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<string?>?>>(global::aweXpect.Core.Extending.ThatExtensions.Get(subject).ExpectationBuilder.AddConstraint(unexpected, (__state, __it, __grammars) => new
			""");
		await That(result.Generated).Contains(" AreNotNamed(this global::aweXpect.Core.IThat<global::System.Collections.Generic.IAsyncEnumerable<string?>?> subject, string unexpected)");
		await That(result.Generated).Contains(" AreNamed(this global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<string?>?> subject, string expected)")
			.Because("the expectation that all items meet keeps the name of the parameter");
		await That(result.Generated).Contains("The <paramref name=\"expected\" /> name is compared exactly.").Exactly(5)
			.Because("only the remarks of the negated expectations follow the negated name");
	}

	[Test]
	[Arguments("AreZero")]
	[Arguments("Are{Not}Zero")]
	public async Task NegatedNameIsInvalid_ShouldReportADiagnostic(string negatedName)
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			$$"""
			  [GenerateCollectionExpectation("AreZero", "is zero", "are zero", NegatedName = "{{negatedName}}")]
			  private static bool IsZero(int value) => value == 0;
			  """);

		await ThatReports(result,
			$"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because the negated name '{negatedName}' is not a valid method name that differs from the name");
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
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableItems<int?> items)");
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
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableItems<int?> items)");
		await That(result.Generated).Contains("!(item is null) && global::Lib.ThatSample.IsZero(item.Value)").Exactly(2);
		await That(result.Generated).DoesNotContain("IEnumerableItems<int>");
	}

	[Test]
	[Arguments("private static bool IsZero(int value) => value == 0;",
		"=> item is null ? new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::aweXpect.Core.Extending.PredicateResult.FailureBothWays(\"was <null>\")) : new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::Lib.ThatSample.IsZero(item.Value));")]
	[Arguments("private static bool IsZero([DisallowNull] int? value) => value == 0;",
		"=> item is null ? new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::aweXpect.Core.Extending.PredicateResult.FailureBothWays(\"was <null>\")) : new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::Lib.ThatSample.IsZero(item.Value));")]
	[Arguments("private static Task<PredicateResult> IsZero(int value) => Task.FromResult<PredicateResult>(value == 0);",
		"=> item is null ? new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::aweXpect.Core.Extending.PredicateResult.FailureBothWays(\"was <null>\")) : new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::Lib.ThatSample.IsZero(item.Value));")]
	[Arguments("private static Task<bool> IsZero(int value) => Task.FromResult(value == 0);",
		"=> item is null ? global::aweXpect.Core.Extending.PredicateResult.FailureBothWays(\"was <null>\") : await global::Lib.ThatSample.IsZero(item.Value);")]
	[Arguments("private static ValueTask<PredicateResult> IsZero(int value) => new(value == 0);",
		"=> item is null ? new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::aweXpect.Core.Extending.PredicateResult.FailureBothWays(\"was <null>\")) : global::Lib.ThatSample.IsZero(item.Value);")]
	public async Task NullItemFailsBothWays_ShouldFailANullItemBothWays(string method, string expectedMatch)
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			$$"""
			  [GenerateCollectionExpectation("AreZero", "is zero", "are zero", NullItemFailsBothWays = true)]
			  {{method}}
			  """);

		await ThatCompiles(result);
		await That(result.Generated).Contains(expectedMatch).Exactly(2)
			.Because("the null items of the nullable type fail with the reason of a null subject");
	}

	[Test]
	[Arguments("int? value", "int?")]
	[Arguments("string? value", "string?")]
	public async Task NullItemFailsBothWays_WithANullableItemParameter_ShouldReportADiagnostic(string parameter,
		string type)
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			$$"""
			  [GenerateCollectionExpectation("AreEmpty", "is empty", "are empty", NullItemFailsBothWays = true)]
			  private static bool IsEmpty({{parameter}}) => value == default;
			  """);

		await ThatReports(result,
			$"aweXpect4002: 'ThatSample.IsEmpty({type})' cannot generate an expectation, because NullItemFailsBothWays requires an item parameter that is not nullable or is marked with [DisallowNull]");
	}

	[Test]
	public async Task NullItemFailsBothWays_WithAReferenceType_ShouldFailANullItemBothWays()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreEmpty", "is empty", "are empty", NullItemFailsBothWays = true)]
			private static bool IsEmpty(string value) => value.Length == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains("=> item is null ? new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::aweXpect.Core.Extending.PredicateResult.FailureBothWays(\"was <null>\")) : new global::System.Threading.Tasks.ValueTask<global::aweXpect.Core.Extending.PredicateResult>(global::Lib.ThatSample.IsEmpty(item));")
			.Exactly(2);
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
		await That(result.Generated).Contains("class IsZero_GeneratedItemsConstraint<").Once();
		await That(result.Generated).Contains("class IsZero_1_GeneratedItemsConstraint<").Once();
	}

	[Test]
	public async Task ParameterNamedItems_ShouldRenameTheExtendedItems()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreIn", "is in {items}", "are in {items}", NullableOverload = false)]
			private static bool IsIn(int value, int[] items) => Array.IndexOf(items, value) >= 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains(" AreIn(this global::aweXpect.IEnumerableItems<int> __items, int[] items)")
			.Because("the extended items must not collide with a parameter of the expectation");
		await That(result.Generated).Contains("AddConstraint((__items.Quantifier, items), ").Exactly(3);
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
			.Contains("AddConstraint((items.Quantifier, name, alias), (__state, __it, __grammars) => new ")
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
			.Contains(" AreAnyOf(this global::aweXpect.IEnumerableItems<int> items, params int[] values)");
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
			.Contains("public static global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IEnumerable<global::Lib.Track?>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IEnumerable<global::Lib.Track?>?>> AreLong(this global::aweXpect.IEnumerableItems<global::Lib.Track?> items)");
		await That(result.Generated)
			.Contains("public static global::aweXpect.Results.AndOrResult<TCollection, global::aweXpect.Core.IThat<TCollection>> AreLong<TCollection>(this global::aweXpect.IStructEnumerableItems<TCollection, global::Lib.Track?> items)");
		await That(result.Generated)
			.Contains("public static global::aweXpect.Results.AndOrResult<global::System.Collections.Generic.IAsyncEnumerable<global::Lib.Track?>, global::aweXpect.Core.IThat<global::System.Collections.Generic.IAsyncEnumerable<global::Lib.Track?>?>> AreLong(this global::aweXpect.IAsyncEnumerableItems<global::Lib.Track?> items)");
		await That(result.Generated).Contains("!(item is null) && global::Lib.ThatTrack.IsLong(item)").Exactly(2)
			.Because("a null item does not match");
	}

	[Test]
	public async Task String_ShouldExtendTheItemsOfNullableStrings()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateCollectionExpectation("AreEmpty", "is empty", "are empty")]
			private static bool IsEmpty(string value) => value.Length == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains(" AreEmpty(this global::aweXpect.IEnumerableItems<string?> items)")
			.Because("the covariant items of nullable strings also accept the items of not nullable strings");
		await That(result.Generated)
			.Contains(" AreEmpty<TCollection>(this global::aweXpect.IStructEnumerableItems<TCollection, string?> items)");
		await That(result.Generated).Contains(" AreEmpty(this global::aweXpect.IAsyncEnumerableItems<string?> items)");
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
		await That(result.Generated).Contains("=> await global::Lib.ThatSample.IsZero(item);").Exactly(2)
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
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableItems<int> items)");
		await That(result.Generated).Contains(" AreZero(this global::aweXpect.IEnumerableItems<int?> items)");
		await That(result.Generated)
			.Contains("=> item is null ? false : await global::Lib.ThatSample.IsZero(item.Value);")
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
