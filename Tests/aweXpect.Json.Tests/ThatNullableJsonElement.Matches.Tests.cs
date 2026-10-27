using System.Collections.Generic;
using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class Matches
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
					=> await That(subject).Matches(expected);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected,
					              but it differed as $ was {subject} instead of {expected}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("42.1", 42.1, true)]
			[Arguments("1.2", 2.1, false)]
			public async Task DoubleValue_ShouldSucceedWhenMatching(string json, double expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected,
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
					=> await That(subject).Matches(expected);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected,
					              but it differed as $ was {json} instead of {expected}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("null", true)]
			[Arguments("{}", false)]
			public async Task NullValue_ShouldSucceedWhenMatching(string json, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(null);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches null,
					              but it differed as $ was object {json} instead of <null>

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("\"foo\"", "\"foo\"", true)]
			[Arguments("\"foo\"", "\"bar\"", false)]
			public async Task StringValue_ShouldSucceedWhenMatching(string json, string expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected,
					              but it differed as $ was {json} instead of {expected}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement?[] items = [FromString("1"), FromString("2"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.Matches(1)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items match 1 for all items,
					             but only 1 of 2 did

					             Not matching items (Items):
					             [
					               2
					             ]

					             Collection (Items):
					             [
					               1,
					               2
					             ]

					             JSON options (Items[1]):
					              - ignore additional properties
					             """)
					.Because("the items are the plural subject of the expectation");
			}

			[Test]
			public async Task WhenNestedInThatAll_ShouldIndentTheDifferences()
			{
				JsonElement? subject = FromString("[1, 2]");

				async Task Act()
					=> await ThatAll(That(subject).Matches([2, 1,]));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected all of the following to succeed:
					              [01] Expected that subject matches [2, 1,]
					             but
					              [01] it differed as
					                     $[0] was 1 instead of 2 and
					                     $[1] was 2 instead of 1

					             [01] JSON options:
					              - ignore additional properties
					             """)
					.Because("every line of the differences belongs to the nested result");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).Matches(new object(), o => o.IgnoringAdditionalProperties());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new object(),
					             but it was <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenSubjectIsUndefined_ShouldFail()
			{
				JsonElement? subject = default(JsonElement);

				async Task Act()
					=> await That(subject).Matches(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches 1,
					             but it was undefined

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement?[] items = [FromString("1"), FromString("2"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.DoesNotComplyWith(it => it.Matches(1))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items do not match 1 for all items,
					             but only 1 of 2 did

					             Not matching items (Items):
					             [
					               1
					             ]

					             Collection (Items):
					             [
					               1,
					               2
					             ]

					             Actual (Items[0]):
					             1

					             JSON options (Items[0]):
					              - ignore additional properties
					             """)
					.Because("the items are the plural subject of the expectation");
			}

			[Test]
			public async Task WhenSubjectIsUndefined_ShouldFail()
			{
				JsonElement? subject = default(JsonElement);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match 1,
					             but it was undefined

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("an undefined element cannot be compared, so its negation must not be met either");
			}

			[Test]
			[Arguments("true", true, true)]
			[Arguments("true", false, false)]
			[Arguments("false", true, false)]
			[Arguments("false", false, true)]
			public async Task BooleanValue_ShouldFailWhenMatching(string json, bool expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches(expected));

				await That(Act).Throws<FailException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected,
					              but it did

					              Actual:
					              {json}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("42.1", 42.1, true)]
			[Arguments("1.2", 2.1, false)]
			public async Task DoubleValue_ShouldFailWhenMatching(string json, double expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches(expected));

				await That(Act).Throws<FailException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected,
					              but it did

					              Actual:
					              {json}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("42", 42, true)]
			[Arguments("1", 2, false)]
			public async Task IntegerValue_ShouldFailWhenMatching(string json, int expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches(expected));

				await That(Act).Throws<FailException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected,
					              but it did

					              Actual:
					              {json}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("null", true)]
			[Arguments("{}", false)]
			public async Task NullValue_ShouldFailWhenMatching(string json, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches(null));

				await That(Act).Throws<FailException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match null,
					              but it did

					              Actual:
					              {json}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("\"foo\"", "\"foo\"", true)]
			[Arguments("\"foo\"", "\"bar\"", false)]
			public async Task StringValue_ShouldFailWhenMatching(string json, string expected, bool isMatch)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches(expected));

				await That(Act).Throws<FailException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected,
					              but it did

					              Actual:
					              {json}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Matches(new object(), o => o.IgnoringAdditionalProperties()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match new object(),
					             but it was <null>

					             JSON options:
					              - ignore additional properties
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
					=> await That(subject).Matches(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NotMatchingArrayValues))]
			public async Task NotMatchingValues_ShouldFail(string[] expected, string json, string errorMessage)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              matches expected,
					              but it differed {errorMessage}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			public async Task WhenElementsAreInDifferentOrder_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).Matches([2, 1,]);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [2, 1,],
					             but it differed as
					               $[0] was 1 instead of 2 and
					               $[1] was 2 instead of 1

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).Matches([1, 2, 3,]);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2, 3,],
					             but it differed as $[2] had missing 3

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).Matches([1, 2,]);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,],
					             but it differed as $[2] had unexpected 3

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("additional array elements are only ignored with IgnoringAdditionalArrayElements");
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringAdditionalArrayElements_ShouldSucceed()
			{
				JsonElement? subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).Matches([1, 2,], o => o.IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNestedArrayContainsAdditionalElements_ShouldFail()
			{
				JsonElement? subject = FromString("""{"id": 1, "items": ["A", "B", "C"]}""");

				async Task Act()
					=> await That(subject).Matches(new
					{
						items = new[] { "A", "B", },
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	items = new[] { "A", "B", },
					             },
					             but it differed as $.items[2] had unexpected "C"

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("the additional property is ignored, but the additional array element is not");
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenNotIgnoringAdditionalProperties_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringAdditionalProperties(false));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,] exactly,
					             but it differed as $[2] had unexpected 3
					             """);
			}

			[Test]
			public async Task WhenSubjectContainsExpectedElementsInDifferentOrder_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement? subject = FromString("[3, 2, 1]");

				async Task Act()
					=> await That(subject).Matches([1, 2,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,],
					             but it differed as $[0] had unexpected 3

					             JSON options:
					              - ignore additional properties
					              - ignore array order
					             """)
					.Because("the unpaired element is an additional array element, which is not ignored");
			}

			[Test]
			public async Task
				WhenSubjectContainsExpectedElementsInDifferentOrder_WhenIgnoringArrayOrderAndAdditionalArrayElements_ShouldSucceed()
			{
				JsonElement? subject = FromString("[3, 2, 1]");

				async Task Act()
					=> await That(subject).Matches([1, 2,],
						o => o.IgnoringArrayOrder().IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectMissesExpectedElement_WhenIgnoringArrayOrderAndAdditionalArrayElements_ShouldFail()
			{
				JsonElement? subject = FromString("[3, 2, 1]");

				async Task Act()
					=> await That(subject).Matches([1, 4,],
						o => o.IgnoringArrayOrder().IgnoringAdditionalArrayElements());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 4,],
					             but it differed as $ had missing 4

					             JSON options:
					              - ignore additional properties
					              - ignore additional array elements
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
						[
							"foo",
						],
						"[]", "as $[0] had missing \"foo\""
					),
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
			public async Task WhenIgnoredPropertyDiffers_WhenIgnoringMember_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"id\": 1, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						id = 2,
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
					=> await That(subject).Matches(new
					{
						foo = 2,
					});

				await That(Act).Throws<FailException>().OnlyIf(errorMessage != null)
					.WithMessage($$"""
					               Expected that subject
					               matches new
					               {
					               	foo = 2,
					               },
					               but it differed as {{errorMessage}}

					               JSON options:
					                - ignore additional properties
					               """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("{\"foo\": 1}")]
			public async Task WhenExpectedIsEmpty_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(new object());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItIsWaitsLongerThanTheTimeout_ShouldStopAtTheTimeout()
			{
				JsonElement? subject = FromString("{\"foo\": \"bar\"}");

				async Task Act()
					=> await That(subject).Matches(new
						{
							foo = It.Is<string>().That.Satisfies(_ => false).Within(TimeSpan.FromSeconds(30)),
						})
						.WithTimeout(TimeSpan.FromMilliseconds(100));

				await That(Act).Throws<FailException>().Within(TimeSpan.FromSeconds(10))
					.WithMessage("""
					             *but it did not finish within 0:00.100

					             JSON options:
					              - ignore additional properties
					             """).AsWildcard()
					.Because("the nested expectation must be canceled by the timeout of the evaluation");
			}

			[Test]
			public async Task WhenPropertyIsNull_AndItIsExpectsAPrefix_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = It.Is<string>().That.StartsWith("a"),
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             *but it differed as $.foo was <null>

					             JSON options:
					              - ignore additional properties
					             """).AsWildcard()
					.Because("a null value has no content that could start with the prefix");
			}

			[Test]
			public async Task WhenPropertyHasDifferentValue_ShouldFail()
			{
				JsonElement? subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						bar = 3,
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	bar = 3,
					             },
					             but it differed as $.bar was 2 instead of 3

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenSubjectHasAdditionalProperties_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						bar = 2,
					});

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasAdditionalProperties_WhenNotIgnoringAdditionalProperties_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						bar = 2,
					}, o => o.IgnoringAdditionalProperties(false));

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
			public async Task WhenSubjectHasAdditionalNullProperty_WhenNotIgnoringAdditionalProperties_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						bar = 2,
					}, o => o.IgnoringAdditionalProperties(false).TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedNullPropertyIsMissing_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = (string?)null,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ComparingNumbersWithinTests
		{
			[Test]
			public async Task WhenNumberIsWithinTheTolerance_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"price\": 104, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						price = 100,
					}, o => o.ComparingNumbersWithin(5).Percent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberIsOutsideTheTolerance_ShouldFail()
			{
				JsonElement? subject = FromString("{\"price\": 106, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						price = 100,
					}, o => o.ComparingNumbersWithin(5).Percent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	price = 100,
					             },
					             but it differed as $.price was 106 instead of 100 ± 5%

					             JSON options:
					              - ignore additional properties
					              - compare numbers within ± 5%
					             """);
			}
		}
	}
}
