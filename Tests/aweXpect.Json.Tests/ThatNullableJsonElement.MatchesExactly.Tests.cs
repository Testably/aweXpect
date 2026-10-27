using System.Collections.Generic;
using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class MatchesExactly
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("true", true, true)]
			[Arguments("true", false, false)]
			[Arguments("false", true, false)]
			[Arguments("false", false, true)]
			public async Task BooleanValue_ShouldSucceedWhenMatching(string json, bool expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed as $ was {subject} instead of {expected}
					              """);
			}

			[Test]
			[Arguments("42.1", 42.1, true)]
			[Arguments("1.2", 2.1, false)]
			public async Task DoubleValue_ShouldSucceedWhenMatching(string json, double expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed as $ was {json} instead of *
					              """).AsWildcard();
			}

			[Test]
			[Arguments("42", 42, true)]
			[Arguments("1", 2, false)]
			public async Task IntegerValue_ShouldSucceedWhenMatching(string json, int expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed as $ was {json} instead of {expected}
					              """);
			}

			[Test]
			[Arguments("null", true)]
			[Arguments("{}", false)]
			public async Task NullValue_ShouldSucceedWhenMatching(string json, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(null);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches null exactly,
					              but it differed as $ was object {json} instead of <null>
					              """);
			}

			[Test]
			[Arguments("\"foo\"", "\"foo\"", true)]
			[Arguments("\"foo\"", "\"bar\"", false)]
			public async Task StringValue_ShouldSucceedWhenMatching(string json, string expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed as $ was {json} instead of {expected}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).MatchesExactly(new object());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new object() exactly,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("true", true, true)]
			[Arguments("true", false, false)]
			[Arguments("false", true, false)]
			[Arguments("false", false, true)]
			public async Task BooleanValue_ShouldFailWhenMatching(string json, bool expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(expected));

				await That(Act).Throws<FailException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected exactly,
					              but it did

					              Actual:
					              {json}
					              """);
			}

			[Test]
			[Arguments("42.1", 42.1, true)]
			[Arguments("1.2", 2.1, false)]
			public async Task DoubleValue_ShouldFailWhenMatching(string json, double expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(expected));

				await That(Act).Throws<FailException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected exactly,
					              but it did

					              Actual:
					              {json}
					              """);
			}

			[Test]
			[Arguments("42", 42, true)]
			[Arguments("1", 2, false)]
			public async Task IntegerValue_ShouldFailWhenMatching(string json, int expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(expected));

				await That(Act).Throws<FailException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected exactly,
					              but it did

					              Actual:
					              {json}
					              """);
			}

			[Test]
			[Arguments("null", true)]
			[Arguments("{}", false)]
			public async Task NullValue_ShouldFailWhenMatching(string json, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(null));

				await That(Act).Throws<FailException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match null exactly,
					              but it did

					              Actual:
					              {json}
					              """);
			}

			[Test]
			[Arguments("\"foo\"", "\"foo\"", true)]
			[Arguments("\"foo\"", "\"bar\"", false)]
			public async Task StringValue_ShouldFailWhenMatching(string json, string expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(expected));

				await That(Act).Throws<FailException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected exactly,
					              but it did

					              Actual:
					              {json}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(new object()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match new object() exactly,
					             but it was <null>
					             """);
			}
		}

		public sealed class ArrayTests
		{
			[Test]
			[MethodDataSource(nameof(MatchingArrayValues))]
			public async Task MatchingValues_ShouldSucceed(string[] expected, string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NotMatchingArrayValues))]
			public async Task NotMatchingValues_ShouldFail(string[] expected, string json, string errorMessage)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed {errorMessage}
					              """);
			}

			[Test]
			public async Task WhenElementsAreInDifferentOrder_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([2, 1,]);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [2, 1,] exactly,
					             but it differed as
					               $[0] was 1 instead of 2 and
					               $[1] was 2 instead of 1
					             """);
			}

			[Test]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2, 3,]);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2, 3,] exactly,
					             but it differed as $[2] had missing 3
					             """);
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,]);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,] exactly,
					             but it differed as $[2] had unexpected 3
					             """);
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringAdditionalArrayElements_ShouldSucceed()
			{
				JsonElement? subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringAdditionalProperties_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringAdditionalProperties());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,],
					             but it differed as $[2] had unexpected 3

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("ignoring additional properties does not affect the elements of arrays");
			}

			[Test]
			public async Task WhenElementsAreInDifferentOrder_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement? subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([3, 1, 2,], o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenElementsDiffer_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([4, 1, 5,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [4, 1, 5,] exactly,
					             but it differed as
					               $ had missing 4 and
					               $ had missing 5 and
					               $[1] had unexpected 2 and
					               $[2] had unexpected 3

					             JSON options:
					              - ignore array order
					             """);
			}

			public static IEnumerable<Func<(string[], string)>> MatchingArrayValues()
				=> [
					() => (
						[], "[]"
					),
					() => (
						[
							"foo", "bar",
						],
						"[\"foo\", \"bar\"]"
					),
				];

			public static IEnumerable<Func<(string[], string, string)>> NotMatchingArrayValues()
				=> [
					() => (
						[], "[\"foo\"]", "as $[0] had unexpected \"foo\""
					),
					() => (
						[], "[\"1\", \"2\"]", """
						                      as
						                        $[0] had unexpected "1" and
						                        $[1] had unexpected "2"
						                      """
					),
					() => (
						[
							"foo",
						],
						"[]", "as $[0] had missing \"foo\""
					),
					() => (
						[
							"bar", "foo",
						],
						"[\"foo\", \"bar\"]", """
						                      as
						                        $[0] was "foo" instead of "bar" and
						                        $[1] was "bar" instead of "foo"
						                      """
					),
				];
		}

		public sealed class ObjectTests
		{
			[Test]
			public async Task WhenIgnoredPropertyIsUnexpected_WhenIgnoringMember_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"id\": 1, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						name = "a",
					}, o => o.IgnoringMember("id"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "$.foo had missing 2")]
			[Arguments("{\"foo\": 2}")]
			public async Task ShouldFailIfPropertyIsMissing(string json,
				string? errorMessage = null)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = 2,
					});

				await That(Act).Throws<FailException>().OnlyIf(errorMessage != null)
					.WithMessage($$"""
					               Expected that subject
					               matches new
					               {
					               	foo = 2,
					               } exactly,
					               but it differed as {{errorMessage}}
					               """);
			}

			[Test]
			[Arguments("{}", true)]
			[Arguments("{\"foo\": 1}", false)]
			public async Task WhenExpectedIsEmpty_ShouldSucceedWhenJsonIsEmptyObject(string json, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(new object());

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage("""
					             Expected that subject
					             matches new object() exactly,
					             but it differed as $.foo had unexpected 1
					             """);
			}

			[Test]
			public async Task WhenPropertyHasDifferentValue_ShouldFail()
			{
				JsonElement? subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						bar = 3,
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	bar = 3,
					             } exactly,
					             but it differed as $.bar was 2 instead of 3
					             """);
			}

			[Test]
			public async Task WhenSubjectHasAdditionalProperties_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						bar = 2,
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	bar = 2,
					             } exactly,
					             but it differed as $.foo had unexpected <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectHasAdditionalProperties_WhenIgnoringAdditionalProperties_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						bar = 2,
					}, o => o.IgnoringAdditionalProperties());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasAdditionalNullProperty_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						bar = 2,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedNullPropertyIsMissing_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = (string?)null,
						bar = 2,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ComparingNumbersWithinTests
		{
			[Test]
			public async Task WhenNumberIsWithinTheTolerance_ShouldSucceed()
			{
				JsonElement? subject = FromString("[0.99, 2.01]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.ComparingNumbersWithin(0.02));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberIsOutsideTheTolerance_ShouldFail()
			{
				JsonElement? subject = FromString("[0.99, 2.03]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.ComparingNumbersWithin(0.02));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,] exactly,
					             but it differed as $[1] was 2.03 instead of 2 ± 0.02

					             JSON options:
					              - compare numbers within ± 0.02
					             """);
			}
		}
	}
}
