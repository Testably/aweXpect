using System.Linq;
using aweXpect.Core.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.Generators.Tests;

public sealed class CoreExpectationGeneratorTests
{
	[Test]
	public async Task AsyncPredicate_ShouldPassTheCancellationToken()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static Task<bool> IsZero(int value, [EvaluationCancellationToken] CancellationToken cancellationToken) => Task.FromResult(value == 0);
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("global::aweXpect.Core.Constraints.IAsyncConstraint<int>");
		await That(result.Generated)
			.Contains("await global::Lib.ThatSample.IsZero(actual, cancellationToken) ? ");
		await That(result.Generated).Contains(" IsZero(this global::aweXpect.Core.IThat<int> subject)")
			.Because("the cancellation token is no parameter of the expectation");
	}

	[Test]
	public async Task AsyncPredicate_WithoutCancellationToken_ShouldCompile()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static ValueTask<bool> IsZero(int value) => new(value == 0);
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("await global::Lib.ThatSample.IsZero(actual) ? ");
	}

	[Test]
	public async Task ClassIsFileLocal_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.Core.Extending;

			namespace Lib;

			file static partial class ThatSample
			{
				[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
				private static bool IsZero(int value) => value == 0;
			}
			""");

		await ThatReports(result,
			"aweXpect4001: 'Lib.ThatSample' cannot hold the generated expectation, because extension methods need a top-level, non-generic, static partial class that is not file-local");
	}

	[Test]
	public async Task ClassIsGeneric_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.Core.Extending;

			namespace Lib;

			public static partial class ThatSample<T>
			{
				[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
				private static bool IsZero(int value) => value == 0;
			}
			""");

		await ThatReports(result,
			"aweXpect4001: 'Lib.ThatSample<T>' cannot hold the generated expectation, because extension methods need a top-level, non-generic, static partial class that is not file-local");
	}

	[Test]
	public async Task ClassIsNested_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.Core.Extending;

			namespace Lib;

			public static partial class Outer
			{
				public static partial class ThatSample
				{
					[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
					private static bool IsZero(int value) => value == 0;
				}
			}
			""");

		await ThatReports(result,
			"aweXpect4001: 'Lib.Outer.ThatSample' cannot hold the generated expectation, because extension methods need a top-level, non-generic, static partial class that is not file-local");
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
				[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
				private static bool IsZero(int value) => value == 0;
			}
			""");

		await ThatReports(result,
			"aweXpect4001: 'Lib.ThatSample' cannot hold the generated expectation, because extension methods need a top-level, non-generic, static partial class that is not file-local");
	}

	[Test]
	public async Task ClassIsNotStatic_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.Core.Extending;

			namespace Lib;

			public partial class ThatSample
			{
				[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
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
			[GenerateExpectation("Is{Not}AnyOf", "is {not} any of {values}", NullableOverload = false)]
			private static bool IsAnyOf(int value, [EvaluationCancellationToken] CancellationToken cancellationToken,
				params int[] values) => Array.IndexOf(values, value) >= 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains(" IsAnyOf(this global::aweXpect.Core.IThat<int> subject, params int[] values)");
		await That(result.Generated).Contains("global::Lib.ThatSample.IsAnyOf(actual, cancellationToken, _p0)");
	}

	[Test]
	public async Task EvaluationCancellationToken_OnAnotherType_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static bool IsZero(int value, [EvaluationCancellationToken] int other) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int, int)' cannot generate an expectation, because the parameter 'other' is marked with [EvaluationCancellationToken], but is no CancellationToken");
	}

	[Test]
	public async Task EvaluationCancellationToken_OnMoreThanOneParameter_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static bool IsZero(int value, [EvaluationCancellationToken] CancellationToken first,
				[EvaluationCancellationToken] CancellationToken second) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int, CancellationToken, CancellationToken)' cannot generate an expectation, because more than one parameter is marked with [EvaluationCancellationToken]");
	}

	[Test]
	public async Task EvaluationCancellationToken_OnTheSubject_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}None", "is {not} none")]
			private static bool IsNone([EvaluationCancellationToken] CancellationToken value) => value == CancellationToken.None;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsNone(CancellationToken)' cannot generate an expectation, because the subject cannot be marked with [EvaluationCancellationToken]");
	}

	[Test]
	public async Task ExpectationTextHasNoNot_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is zero")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because the expectation text has no '{not}' and no negated expectation text is given");
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
					[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
					private static bool IsZero(int value) => value == 0;

					[GenerateExpectation("Is{Not}Named", "is {not} named {name} or {alias}", Result = nameof(Describe))]
					private static bool IsNamed(string value, string name, string? alias) => value == name || value == alias;

					[GenerateExpectation("Is{Not}Empty", "is {not} empty")]
					private static bool IsEmpty([NotNullWhen(false)] string? value) => value == "";

					[GenerateExpectation("Is{Not}Ready", "is {not} ready")]
					private static Task<bool> IsReady(string value, [EvaluationCancellationToken] CancellationToken cancellationToken) => Task.FromResult(true);

					[GenerateExpectation("Is{Not}Known", "is {not} known")]
					private static PredicateResult IsKnown(string value)
						=> value.Length == 0 ? PredicateResult.FailureBothWays("was empty") : true;

					private static string Describe(string value) => "was " + value;
				}
			}
			""", LanguageVersion.CSharp8);

		await ThatCompiles(result);
	}

	[Test]
	public async Task MethodHasNoParameter_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static bool IsZero() => true;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero()' cannot generate an expectation, because it has no parameter for the subject");
	}

	[Test]
	public async Task MethodIsGeneric_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Default", "is {not} default")]
			private static bool IsDefault<T>(T value) => Equals(value, default(T));
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsDefault<T>(T)' cannot generate an expectation, because it is generic");
	}

	[Test]
	public async Task MethodIsNotStatic_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private bool IsZero(int value) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because it is not static");
	}

	[Test]
	public async Task MethodReturnsATaskOfAnotherNamespace_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			private sealed class Task<T>;

			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static Task<bool> IsZero(int value) => new();
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because it returns neither bool nor PredicateResult, nor a Task or ValueTask of them");
	}

	[Test]
	public async Task MethodReturnsNoBoolean_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static Task<int> IsZero(int value) => Task.FromResult(value);
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because it returns neither bool nor PredicateResult, nor a Task or ValueTask of them");
	}

	[Test]
	public async Task NameHasNoNot_ShouldOnlyGenerateTheExpectation()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("IsZero", "is {not} zero", NullableOverload = false)]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains(" IsZero(this ").Once();
		await That(result.Generated).Contains("\"is not zero\"")
			.Because("the negated text is still needed when the expectation is negated with DoesNotComplyWith");
	}

	[Test]
	public async Task NameIsInvalid_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is {Not} Zero", "is {not} zero")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because the name 'Is {Not} Zero' is not a valid method name");
	}

	[Test]
	public async Task NameIsNull_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation(null!, "is {not} zero")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsZero(int)' cannot generate an expectation, because the name and the expectation text are required");
	}

	[Test]
	public async Task NegatedExpectationText_ShouldReplaceTheDerivedText()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Has{Not}Level", "has level {level}", NullableOverload = false,
				NegatedExpectationText = "does not have level {level}")]
			private static bool HasLevel(int value, int level) => value == level;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("Verb(Grammars, \"has level \", \"have level \")");
		await That(result.Generated).Contains("Verb(Grammars, \"does not have level \", \"do not have level \")");
		await That(result.Generated)
			.Contains("Verifies that the subject does not have level <paramref name=\"level\" />.")
			.Because("the documentation of the negated expectation is derived from its text");
	}

	[Test]
	public async Task NegatedExpectationText_WithUnknownPlaceholder_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Has{Not}Level", "has level {level}",
				NegatedExpectationText = "does not have level {other}")]
			private static bool HasLevel(int value, int level) => value == level;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.HasLevel(int, int)' cannot generate an expectation, because the placeholder '{other}' names no parameter of the expectation");
	}

	[Test]
	public async Task NegatedName_ShouldNameTheNegatedExpectation()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("HasLevel", "has level {level}", NullableOverload = false,
				NegatedName = "DoesNotHaveLevel", NegatedExpectationText = "does not have level {level}")]
			private static bool HasLevel(int value, int level) => value == level;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains(" HasLevel(this ").Once();
		await That(result.Generated).Contains(" DoesNotHaveLevel(this ").Once();
	}

	[Test]
	public async Task NegatedName_WhenEqualToTheName_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Has{Not}Level", "has {not} level {level}", NegatedName = "HasLevel")]
			private static bool HasLevel(int value, int level) => value == level;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.HasLevel(int, int)' cannot generate an expectation, because the negated name 'HasLevel' is not a valid method name that differs from the name");
	}

	[Test]
	public async Task NegatedName_WhenInvalid_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("HasLevel", "has {not} level {level}", NegatedName = "Does Not Have Level")]
			private static bool HasLevel(int value, int level) => value == level;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.HasLevel(int, int)' cannot generate an expectation, because the negated name 'Does Not Have Level' is not a valid method name that differs from the name");
	}

	[Test]
	public async Task NotNullWhenFalse_ShouldOnlyGuaranteeNotNullForTheNegatedExpectation()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}NullOrEmpty", "is {not} null or empty")]
			private static bool IsNullOrEmpty([NotNullWhen(false)] string? value) => string.IsNullOrEmpty(value);
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				/// </summary>
				public static global::aweXpect.Results.AndOrResult<string?, global::aweXpect.Core.IThat<string?>> IsNullOrEmpty(this global::aweXpect.Core.IThat<string?> subject)
			""");
		await ThatContainsLines(result,
			"""
				[global::aweXpect.Core.GuaranteesNotNull]
				public static global::aweXpect.Results.AndOrResult<string, global::aweXpect.Core.IThat<string?>> IsNotNullOrEmpty(this global::aweXpect.Core.IThat<string?> subject)
			""");
		await That(result.Generated).Contains("ConstraintResult.WithValue<string?>")
			.Because("the predicate decides about a null subject");
	}

	[Test]
	public async Task NotNullWhenTrue_ShouldOnlyGuaranteeNotNullForTheExpectation()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Short", "is {not} short")]
			private static bool IsShort([NotNullWhen(true)] string? value) => value is { Length: < 3, };
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				[global::aweXpect.Core.GuaranteesNotNull]
				public static global::aweXpect.Results.AndOrResult<string, global::aweXpect.Core.IThat<string?>> IsShort(this global::aweXpect.Core.IThat<string?> subject)
			""");
		await ThatContainsLines(result,
			"""
				/// </summary>
				public static global::aweXpect.Results.AndOrResult<string?, global::aweXpect.Core.IThat<string?>> IsNotShort(this global::aweXpect.Core.IThat<string?> subject)
			""");
	}

	[Test]
	public async Task NullableOverloadIsFalse_ShouldOnlyExtendTheValueType()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero", NullableOverload = false)]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).DoesNotContain("IThat<int?>");
	}

	[Test]
	public async Task NullableValueTypeWithDisallowNull_ShouldFailOnNull()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static bool IsZero([DisallowNull] int? value) => value == 0;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				[global::aweXpect.Core.GuaranteesNotNull]
				public static global::aweXpect.Results.AndOrResult<int, global::aweXpect.Core.IThat<int?>> IsNotZero(this global::aweXpect.Core.IThat<int?> subject)
			""");
		await That(result.Generated).Contains("ConstraintResult.WithNotNullValue<int?>");
		await That(result.Generated).Contains("!(actual is null) && global::Lib.ThatSample.IsZero(actual)");
	}

	[Test]
	public async Task OverloadedPredicates_ShouldGenerateDistinctConstraints()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static bool IsZero(int value) => value == 0;

			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static bool IsZero(long value) => value == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("class IsZero_GeneratedConstraint ").Once();
		await That(result.Generated).Contains("class IsZero_1_GeneratedConstraint ").Once();
	}

	[Test]
	public async Task ParameterIsAKeyword_ShouldEscapeIt()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}OfClass", "is {not} of class {class}")]
			private static bool IsOfClass(string value, string @class) => value == @class;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("string @class)");
		await That(result.Generated).Contains("new global::System.ArgumentNullException(nameof(@class), \"The 'class' cannot be null.\")");
		await That(result.Generated).Contains("<paramref name=\"class\" />");
	}

	[Test]
	public async Task ParameterIsNamedSubject_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Equal", "is {not} equal to {subject}")]
			private static bool IsEqual(int value, int subject) => value == subject;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsEqual(int, int)' cannot generate an expectation, because the parameter 'subject' would clash with the subject of the expectation");
	}

	[Test]
	public async Task ParameterIsOptional_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Equal", "is {not} equal to {expected}")]
			private static bool IsEqual(int value, int expected = 0) => value == expected;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsEqual(int, int)' cannot generate an expectation, because the parameter 'expected' is optional");
	}

	[Test]
	public async Task ParameterIsPassedByReference_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Equal", "is {not} equal to {expected}")]
			private static bool IsEqual(int value, in int expected) => value == expected;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsEqual(int, in int)' cannot generate an expectation, because the parameter 'expected' is passed by reference");
	}

	[Test]
	public async Task PlaceholderNamesNoParameter_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Equal", "is {not} equal to {other}")]
			private static bool IsEqual(int value, int expected) => value == expected;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsEqual(int, int)' cannot generate an expectation, because the placeholder '{other}' names no parameter of the expectation");
	}

	[Test]
	public async Task PredicateResult_ShouldSetTheOutcomeAndTheReason()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero", NullableOverload = false)]
			private static PredicateResult IsZero(int value)
				=> value < 0 ? PredicateResult.FailureBothWays("was negative") : value == 0;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
						global::aweXpect.Core.Extending.PredicateResult result = global::Lib.ThatSample.IsZero(actual);
						_reason = result.Reason;
						Outcome = result.Outcome;
			""");
		await ThatContainsLines(result,
			"""
						if (!(_reason is null))
						{
							stringBuilder.Append(It).Append(' ').Append(_reason);
							return;
						}
			""");
	}

	[Test]
	public async Task PredicateResult_WhenAsync_ShouldAwaitIt()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero", NullableOverload = false)]
			private static ValueTask<PredicateResult> IsZero(int value) => new(value == 0);
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains("global::aweXpect.Core.Extending.PredicateResult result = await global::Lib.ThatSample.IsZero(actual);");
	}

	[Test]
	public async Task PredicateResult_WhenSubjectFailsOnNull_ShouldNotCallThePredicateForNull()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static PredicateResult IsZero(int value) => value == 0;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
						if (actual is null)
						{
							Outcome = global::aweXpect.Core.Constraints.Outcome.Failure;
							return this;
						}

						global::aweXpect.Core.Extending.PredicateResult result = global::Lib.ThatSample.IsZero(actual.Value);
			""");
	}

	[Test]
	public async Task ReferenceType_ShouldFailOnNull()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Empty", "is {not} empty")]
			private static bool IsEmpty(string value) => value.Length == 0;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				[global::aweXpect.Core.GuaranteesNotNull]
				public static global::aweXpect.Results.AndOrResult<string, global::aweXpect.Core.IThat<string?>> IsEmpty(this global::aweXpect.Core.IThat<string?> subject)
			""");
		await ThatContainsLines(result,
			"""
				[global::aweXpect.Core.GuaranteesNotNull]
				public static global::aweXpect.Results.AndOrResult<string, global::aweXpect.Core.IThat<string?>> IsNotEmpty(this global::aweXpect.Core.IThat<string?> subject)
			""");
		await That(result.Generated).Contains("ConstraintResult.WithNotNullValue<string?>");
	}

	[Test]
	public async Task Remarks_ShouldResolveReferencesWithTheUsingsOfTheDeclaringFile()
	{
		CSharpCompilation compilation = GeneratorRunner.CreateCompilation([
			"global using System;",
			"""
			using System;
			using aweXpect.Core.Extending;

			namespace Lib
			{
				using System.Text;

				/// <summary>
				///     The expectations on the sample.
				/// </summary>
				public static partial class ThatSample
				{
					[GenerateExpectation("Is{Not}Zero", "is {not} zero",
						Remarks = "Compare <see cref=\"StringBuilder\" /> and <see cref=\"TimeSpan\" />.")]
					private static bool IsZero(int value) => value == 0;
				}
			}
			""",
		]);
		GeneratorDriver driver = CSharpGeneratorDriver.Create([new ExpectationGenerator().AsSourceGenerator(),],
			parseOptions: new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.Diagnose));

		driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation output, out _);

		SyntaxTree generated = output.SyntaxTrees.Last();
		await That(generated.ToString()).Contains("using System.Text;");
		await That(output.GetDiagnostics()
				.Where(x => x.Severity >= DiagnosticSeverity.Warning && x.Location.SourceTree == generated)
				.Select(x => x.ToString()))
			.IsEmpty()
			.Because("the references in the documentation have to resolve like in the declaring file");
	}

	[Test]
	public async Task ResultMethodHasOtherParameters_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Equal", "is {not} equal to {expected}", Result = nameof(Describe))]
			private static bool IsEqual(int value, int expected) => value == expected;

			private static string Describe(int value, string expected) => "was " + value;
			""");

		await ThatReports(result,
			"aweXpect4003: 'Describe' names no static method that returns a string and takes the subject, optionally followed by the other parameters of 'ThatSample.IsEqual(int, int)' and by the string 'it' and the ExpectationGrammars");
	}

	[Test]
	public async Task ResultMethodIsMissing_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero", Result = "Describe")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4003: 'Describe' names no static method that returns a string and takes the subject, optionally followed by the other parameters of 'ThatSample.IsZero(int)' and by the string 'it' and the ExpectationGrammars");
	}

	[Test]
	public async Task ResultMethodTakesTheParameters_ShouldPassThem()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Equal", "is {not} equal to {expected}", Result = nameof(Describe))]
			private static bool IsEqual(int value, int expected) => value == expected;

			private static string Describe(int value, int expected) => "differed by " + (value - expected);
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("global::Lib.ThatSample.Describe(Actual, _p0)");
		await That(result.Generated).Contains("global::Lib.ThatSample.Describe(Actual.GetValueOrDefault(), _p0)")
			.Because("the nullable overload only describes a subject that is not null");
	}

	[Test]
	public async Task ResultMethodTakesOnlyTheGrammars_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero", Result = nameof(Describe))]
			private static bool IsZero(int value) => value == 0;

			private static string Describe(int value, aweXpect.Core.ExpectationGrammars grammars) => "was " + value;
			""");

		await ThatReports(result,
			"aweXpect4003: 'Describe' names no static method that returns a string and takes the subject, optionally followed by the other parameters of 'ThatSample.IsZero(int)' and by the string 'it' and the ExpectationGrammars");
	}

	[Test]
	public async Task ResultMethodTakesItAndTheGrammars_ShouldNotPrependIt()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Equal", "is {not} equal to {expected}", Result = nameof(Describe))]
			private static bool IsEqual(int value, int expected) => value == expected;

			private static string Describe(int value, int expected, string it, aweXpect.Core.ExpectationGrammars grammars)
				=> it + " was " + value;
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains("stringBuilder.Append(global::Lib.ThatSample.Describe(Actual, _p0, It, Grammars));");
	}

	[Test]
	public async Task ResultMethodTakesItAndTheGrammarsWithoutTheParameters_ShouldPassOnlyThem()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Named", "is {not} named {name}", Result = nameof(Describe))]
			private static bool IsNamed(string value, string name) => value == name;

			private static string Describe(string value, string it, aweXpect.Core.ExpectationGrammars grammars)
				=> it + " was " + value;
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains("stringBuilder.Append(global::Lib.ThatSample.Describe(Actual!, It, Grammars));");
	}

	[Test]
	public async Task ResultMethodTakesOnlyIt_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero", Result = nameof(Describe))]
			private static bool IsZero(int value) => value == 0;

			private static string Describe(int value, string it) => it + " was " + value;
			""");

		await ThatReports(result,
			"aweXpect4003: 'Describe' names no static method that returns a string and takes the subject, optionally followed by the other parameters of 'ThatSample.IsZero(int)' and by the string 'it' and the ExpectationGrammars");
	}

	[Test]
	public async Task SubjectIsAParamsArray_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Empty", "is {not} empty")]
			private static bool IsEmpty(params int[] values) => values.Length == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsEmpty(params int[])' cannot generate an expectation, because the subject cannot be a params array");
	}

	[Test]
	public async Task SummaryAndRemarks_ShouldReplaceTheDerivedDocumentation()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero", NullableOverload = false,
				Summary = "Checks for <c>0</c>.", NegatedSummary = "Checks for anything but <c>0</c>.",
				Remarks = "First line.\nSecond line.", NegatedRemarks = "Negated remark.")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				/// <summary>
				///     Checks for <c>0</c>.
				/// </summary>
				/// <remarks>
				///     First line.
				///     Second line.
				/// </remarks>
				public static
			""".TrimEnd());
		await ThatContainsLines(result,
			"""
				/// <summary>
				///     Checks for anything but <c>0</c>.
				/// </summary>
				/// <remarks>
				///     Negated remark.
				/// </remarks>
				public static
			""".TrimEnd());
	}

	[Test]
	public async Task SyncPredicate_ShouldPassTheCancellationToken()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static bool IsZero(int value, [EvaluationCancellationToken] CancellationToken cancellationToken) => value == 0;
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("global::aweXpect.Core.Constraints.IAsyncConstraint<int>");
		await That(result.Generated).Contains("global::Lib.ThatSample.IsZero(actual, cancellationToken)");
		await That(result.Generated).DoesNotContain("async ")
			.Because("the synchronous predicate completes the constraint at once");
		await That(result.Generated).Contains(" IsZero(this global::aweXpect.Core.IThat<int> subject)")
			.Because("the cancellation token is no parameter of the expectation");
	}

	[Test]
	public async Task UnmarkedCancellationToken_ShouldBeAParameterOfTheExpectation()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}CanceledBy", "is {not} canceled by {token}")]
			private static bool IsCanceledBy(string value, CancellationToken token) => token.IsCancellationRequested;
			""");

		await ThatCompiles(result);
		await That(result.Generated)
			.Contains(" IsCanceledBy(this global::aweXpect.Core.IThat<string?> subject, global::System.Threading.CancellationToken token)");
		await That(result.Generated).Contains("global::Lib.ThatSample.IsCanceledBy(actual, _p0)");
		await That(result.Generated).Contains("global::aweXpect.Core.Constraints.IValueConstraint<string?>")
			.Because("only a marked token makes the constraint receive the token of the evaluation");
	}

	[Test]
	public async Task ValueType_ShouldAlsoExtendTheNullableType()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
			private static bool IsZero(int value) => value == 0;
			""");

		await ThatCompiles(result);
		await ThatContainsLines(result,
			"""
				/// </summary>
				public static global::aweXpect.Results.AndOrResult<int, global::aweXpect.Core.IThat<int>> IsZero(this global::aweXpect.Core.IThat<int> subject)
			""");
		await ThatContainsLines(result,
			"""
				[global::aweXpect.Core.GuaranteesNotNull]
				public static global::aweXpect.Results.AndOrResult<int, global::aweXpect.Core.IThat<int?>> IsZero(this global::aweXpect.Core.IThat<int?> subject)
			""");
		await That(result.Generated).Contains("!(actual is null) && global::Lib.ThatSample.IsZero(actual.Value)");
	}

	[Test]
	public async Task WhenClassHasNoNamespace_ShouldCompile()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.Core.Extending;

			public static partial class ThatSample
			{
				[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
				private static bool IsZero(int value) => value == 0;
			}
			""");

		await ThatCompiles(result);
		await That(result.Generated).DoesNotContain("namespace ");
		await That(result.Generated).Contains("global::ThatSample.IsZero(actual)");
	}

	[Test]
	public async Task WhenClassIsInternal_ShouldKeepItsAccessibility()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.Core.Extending;

			namespace Lib;

			internal static partial class ThatSample
			{
				[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
				private static bool IsZero(int value) => value == 0;
			}
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("internal static partial class ThatSample").Once()
			.Because("the parts of a partial class must not declare conflicting accessibilities");
	}

	[Test]
	public async Task WhenClassIsPublicAndSubjectTypeIsInternal_ShouldGenerateInternalExpectations()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.Core.Extending;

			namespace Lib;

			internal sealed class Track(int duration)
			{
				public int Duration { get; } = duration;
			}

			public static partial class ThatTrack
			{
				[GenerateExpectation("Is{Not}Long", "is {not} long")]
				private static bool IsLong(Track track) => track.Duration > 1;
			}
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("public static partial class ThatTrack").Once();
		await That(result.Generated).Contains("internal static global::aweXpect.Results.AndOrResult<").Exactly(2);
		await That(result.Generated).DoesNotContain("public static global::aweXpect.Results.AndOrResult<");
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
				[GenerateExpectation("Is{Not}Zero", "is {not} zero")]
				private static bool IsZero(int value) => value == 0;
			}
			""",
		], "// unrelated");

		IncrementalStepRunReason[] reasons = result.Results[0].TrackedSteps["Expectations"]
			.SelectMany(x => x.Outputs).Select(x => x.Reason).ToArray();
		await That(reasons).IsNotEmpty();
		await That(reasons).All()
			.Satisfy(x => x is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged)
			.Because("the model is compared by value, so an unrelated edit does not regenerate the expectation");
	}

	[Test]
	public async Task WhenParameterTypeArgumentIsInternal_ShouldGenerateInternalExpectations()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using System.Collections.Generic;
			using System.Linq;
			using aweXpect.Core.Extending;

			namespace Lib;

			internal enum Level
			{
				Low,
				High,
			}

			public static partial class ThatSample
			{
				[GenerateExpectation("Is{Not}AnyOf", "is {not} any of {levels}")]
				private static bool IsAnyOf(int value, IEnumerable<Level[]> levels) => levels.Any(x => x.Length == value);
			}
			""");

		await ThatCompiles(result);
		await That(result.Generated).Contains("internal static global::aweXpect.Results.AndOrResult<").Exactly(4);
		await That(result.Generated).DoesNotContain("public static global::aweXpect.Results.AndOrResult<");
	}

	[Test]
	public async Task WhenParameterTypeIsPrivate_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = RunOn(
			"""
			private sealed class Secret;

			[GenerateExpectation("Is{Not}Secret", "is {not} secret {secret}")]
			private static bool IsSecret(int value, Secret secret) => value == 0;
			""");

		await ThatReports(result,
			"aweXpect4002: 'ThatSample.IsSecret(int, ThatSample.Secret)' cannot generate an expectation, because the type of the parameter 'secret' is not accessible outside of 'ThatSample'");
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
