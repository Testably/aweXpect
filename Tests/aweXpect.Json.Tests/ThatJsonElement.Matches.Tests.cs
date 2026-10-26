using System.Collections.Generic;
using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class Matches
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement[] items = [FromString("1"), FromString("2"),];

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
				JsonElement subject = FromString("[1, 2]");

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
			public async Task WhenStringValueContainsAQuote_ShouldEscapeIt()
			{
				JsonElement subject = FromString("\"a\\\"b\"");

				async Task Act()
					=> await That(subject).Matches("\"c\"");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches "\"c\"",
					             but it differed as $ was "a\"b" instead of "c"

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("a string value is formatted like any other string");
			}

			[Test]
			[Arguments("true", true, true)]
			[Arguments("true", false, false)]
			[Arguments("false", true, false)]
			[Arguments("false", false, true)]
			public async Task BooleanValue_ShouldSucceedWhenMatching(string json, bool expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
			[Arguments("9007199254740993", 9007199254740993L, true)]
			[Arguments("9007199254740993", 9007199254740992L, false)]
			public async Task LongValue_ShouldSucceedWhenMatching(string json, long expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected, o => o.IgnoringAdditionalProperties());

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
			public async Task WhenSubjectIsUndefined_ShouldFail()
			{
				JsonElement subject = default;

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
				JsonElement[] items = [FromString("1"), FromString("2"),];

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
				JsonElement subject = default;

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
		}

		public sealed class ArrayTests
		{
			[Test]
			[MethodDataSource(nameof(MatchingArrayValues))]
			public async Task MatchingValues_ShouldSucceed(string[] expected, string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NotMatchingArrayValues))]
			public async Task NotMatchingValues_ShouldFail(string[] expected, string json, string errorMessage)
			{
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString("[1, 2]");

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
				JsonElement subject = FromString("[1, 2]");

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
				JsonElement titles = FromString("[\"Let It Be\", \"Yesterday\", \"Hey Jude\"]");

				async Task Act()
					=> await That(titles).Matches(["Let It Be", "Yesterday"]);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that titles
					             matches ["Let It Be", "Yesterday"],
					             but it differed as $[2] had unexpected "Hey Jude"

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("additional array elements are only ignored with IgnoringAdditionalArrayElements");
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringAdditionalArrayElements_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).Matches([1, 2,], o => o.IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenNotIgnoringAdditionalArrayElements_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).Matches([1, 2,], o => o.IgnoringAdditionalArrayElements(false));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,],
					             but it differed as $[2] had unexpected 3

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringAdditionalProperties_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).Matches([1, 2,], o => o.IgnoringAdditionalProperties());

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
			public async Task
				WhenSubjectContainsAdditionalElements_WhenOnlyIgnoringAdditionalArrayElements_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).Matches([1, 2,],
						o => o.IgnoringAdditionalProperties(false).IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow()
					.Because("additional array elements are ignored independently of additional properties");
			}

			[Test]
			public async Task WhenNestedArrayContainsAdditionalElements_ShouldFail()
			{
				JsonElement subject = FromString("""{"id": 1, "items": ["A", "B", "C"]}""");

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
			public async Task WhenNestedArrayContainsAdditionalElements_WhenIgnoringAdditionalArrayElements_ShouldSucceed()
			{
				JsonElement subject = FromString("""{"id": 1, "items": ["A", "B", "C"]}""");

				async Task Act()
					=> await That(subject).Matches(new
					{
						items = new[] { "A", "B", },
					}, o => o.IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenObjectsInArrayContainAdditionalProperties_ShouldSucceed()
			{
				JsonElement subject = FromString("""[{"id": 1, "name": "foo"}, {"id": 2, "name": "bar"}]""");

				async Task Act()
					=> await That(subject).Matches(new[]
					{
						new { id = 1, },
						new { id = 2, },
					});

				await That(Act).DoesNotThrow()
					.Because("additional properties of objects in an array are still ignored");
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalObject_ShouldFail()
			{
				JsonElement subject = FromString("""[{"id": 1, "name": "foo"}, {"id": 2}]""");

				async Task Act()
					=> await That(subject).Matches(new[]
					{
						new { id = 1, },
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new[]
					             {
					             	new { id = 1, },
					             },
					             but it differed as $[1] had unexpected {"id":2}

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenNotIgnoringAdditionalProperties_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).Matches([1, 2,], o => o.IgnoringAdditionalProperties(false));

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
				JsonElement subject = FromString("[4, 3, 2, 1]");

				async Task Act()
					=> await That(subject).Matches([1, 3,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 3,],
					             but it differed as
					               $[0] had unexpected 4 and
					               $[2] had unexpected 2

					             JSON options:
					              - ignore additional properties
					              - ignore array order
					             """)
					.Because("the unpaired elements are additional array elements, which are not ignored");
			}

			[Test]
			public async Task
				WhenSubjectContainsExpectedElementsInDifferentOrder_WhenIgnoringArrayOrderAndAdditionalArrayElements_ShouldSucceed()
			{
				JsonElement subject = FromString("[4, 3, 2, 1]");

				async Task Act()
					=> await That(subject).Matches([1, 3,],
						o => o.IgnoringArrayOrder().IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow()
					.Because("additional elements are ignored, so the expected elements are a subset in any order");
			}

			[Test]
			public async Task
				WhenSubjectMissesExpectedElements_WhenIgnoringArrayOrderAndAdditionalArrayElements_ShouldOnlyReportMissingElements()
			{
				JsonElement subject = FromString("[4, 3, 2, 1]");

				async Task Act()
					=> await That(subject).Matches([5, 1, 6,],
						o => o.IgnoringArrayOrder().IgnoringAdditionalArrayElements());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [5, 1, 6,],
					             but it differed as
					               $ had missing 5 and
					               $ had missing 6

					             JSON options:
					              - ignore additional properties
					              - ignore additional array elements
					              - ignore array order
					             """);
			}

			[Test]
			public async Task WhenItIsMatchesLaterElement_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("[{\"foo\": 1}, {\"foo\": 2}, {\"foo\": 3}]");

				async Task Act()
					=> await That(subject).Matches(new[]
					{
						new
						{
							foo = It.Is<int>().That.IsGreaterThan(2),
						},
					}, o => o.IgnoringArrayOrder().IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow();
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
				JsonElement subject = FromString("{\"id\": 1, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						id = 2,
						name = "a",
					}, o => o.IgnoringMember("id"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOtherPropertyDiffers_WhenIgnoringMember_ShouldFail()
			{
				JsonElement subject = FromString("{\"id\": 1, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						id = 2,
						name = "b",
					}, o => o.IgnoringMember("id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	id = 2,
					             	name = "b",
					             },
					             but it differed as $.name was "a" instead of "b"

					             JSON options:
					              - ignore additional properties
					              - ignore members: ["id"]
					             """);
			}

			[Test]
			[Arguments("{}", "$.foo had missing 2")]
			[Arguments("{\"foo\": 2}")]
			public async Task ShouldFailIfPropertyIsMissing(string json,
				string? errorMessage = null)
			{
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).Matches(new object());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_AndItIsExpectsAPrefix_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = It.Is<string>().That.StartsWith("a"),
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = It.Is<string>().That.StartsWith("a"),
					             },
					             but it differed as $.foo was <null>

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("a null value has no content that could start with the prefix");
			}

			[Test]
			public async Task WhenItIsWaitsLongerThanTheTimeout_ShouldStopAtTheTimeout()
			{
				JsonElement subject = FromString("{\"foo\": \"bar\"}");

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
			public async Task WhenPropertyHasDifferentValue_ShouldFail()
			{
				JsonElement subject = FromString("{\"bar\": 2}");

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
				JsonElement subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						bar = 2,
					}, o => o.IgnoringAdditionalProperties());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasAdditionalProperties_WhenNotIgnoringAdditionalProperties_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null, \"bar\": 2}");

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
				JsonElement subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						bar = 2,
					}, o => o.IgnoringAdditionalProperties(false).TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedNullPropertyIsMissing_ShouldFail()
			{
				JsonElement subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = (string?)null,
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = (string?)null,
					             },
					             but it differed as $.foo had missing <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectedNullPropertyIsMissing_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = (string?)null,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItIsExpectsNullOnNullProperty_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = It.Is<string?>().That.IsNull(),
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow()
					.Because("the null property is still compared with the expectation for it");
			}

			[Test]
			public async Task WhenItIsExpectsNullOnMissingProperty_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = It.Is<string?>().That.IsNull(),
					}, o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = It.Is<string?>().That.IsNull(),
					             },
					             but it differed as $.foo had missing is string

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """)
					.Because("only an expected null value accepts a missing property");
			}
		}

		public sealed class ExpectationTests
		{
			[Test]
			public async Task WhenElementWithExpectationIsMissing_ShouldDescribeTheExpectation()
			{
				JsonElement subject = FromString("[{\"foo\": 1}]");

				async Task Act()
					=> await That(subject).Matches(new object[]
					{
						new { foo = 1, },
						new { foo = It.Is<int>().That.IsGreaterThan(2), },
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new object[]
					             {
					             	new { foo = 1, },
					             	new { foo = It.Is<int>().That.IsGreaterThan(2), },
					             },
					             but it differed as $[1] had missing {"foo":is int}

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("an expectation that was never evaluated can only describe its type");
			}

			[Test]
			public async Task WhenElementWithExpectationIsMissing_WhenIgnoringArrayOrder_ShouldDescribeTheExpectation()
			{
				JsonElement subject = FromString("[{\"foo\": 1}, {\"foo\": 2}]");

				async Task Act()
					=> await That(subject).Matches(new object[]
					{
						new { foo = It.Is<int>().That.IsGreaterThan(2), },
						new { foo = 5, },
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new object[]
					             {
					             	new { foo = It.Is<int>().That.IsGreaterThan(2), },
					             	new { foo = 5, },
					             },
					             but it differed as
					               $ had missing {"foo":is int that is greater than 2} and
					               $ had missing {"foo":5} and
					               $[0] had unexpected {"foo":1} and
					               $[1] had unexpected {"foo":2}

					             JSON options:
					              - ignore additional properties
					              - ignore array order
					             """);
			}

			[Test]
			public async Task WhenExpectationElementIsMissing_ShouldDescribeTheExpectation()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).Matches(new object[]
					{
						1, It.Is<int>().That.IsGreaterThan(2),
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new object[]
					             {
					             	1, It.Is<int>().That.IsGreaterThan(2),
					             },
					             but it differed as $[1] had missing is int

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectedObjectWithExpectationHasDifferentKind_ShouldDescribeTheExpectation()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = new { bar = It.Is<int>().That.IsGreaterThan(2), },
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = new { bar = It.Is<int>().That.IsGreaterThan(2), },
					             },
					             but it differed as $.foo was number 1 instead of {"bar":is int}

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenNestedElementWithExpectationIsMissing_ShouldDescribeTheExpectation()
			{
				JsonElement subject = FromString("{\"items\": []}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						items = new[] { new { bar = new { foo = It.Is<int>().That.IsGreaterThan(2), }, }, },
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	items = new[] { new { bar = new { foo = It.Is<int>().That.IsGreaterThan(2), }, }, },
					             },
					             but it differed as $.items[0] had missing {"bar":{"foo":is int}}

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task
				WhenNestedElementWithExpectationIsMissing_WhenIgnoringArrayOrder_ShouldDescribeTheExpectation()
			{
				JsonElement subject = FromString("{\"items\": [{\"bar\": {\"foo\": 1}}, {\"bar\": {\"foo\": 2}}]}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						items = new object[]
						{
							new { bar = new { foo = It.Is<int>().That.IsGreaterThan(2), }, },
							new { bar = new { foo = 5, }, },
						},
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	items = new object[]
					             	{
					             		new { bar = new { foo = It.Is<int>().That.IsGreaterThan(2), }, },
					             		new { bar = new { foo = 5, }, },
					             	},
					             },
					             but it differed as
					               $.items had missing {"bar":{"foo":is int that is greater than 2}} and
					               $.items had missing {"bar":{"foo":5}} and
					               $.items[0] had unexpected {"bar":{"foo":1}} and
					               $.items[1] had unexpected {"bar":{"foo":2}}

					             JSON options:
					              - ignore additional properties
					              - ignore array order
					             """);
			}
		}

		public sealed class ComparingNumbersWithinTests
		{
			[Test]
			public async Task WhenNumberIsWithinTheTolerance_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"price\": 10.04, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						price = 10,
					}, o => o.ComparingNumbersWithin(0.05));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberIsOutsideTheTolerance_ShouldFail()
			{
				JsonElement subject = FromString("{\"price\": 10.06, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).Matches(new
					{
						price = 10,
					}, o => o.ComparingNumbersWithin(0.05));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	price = 10,
					             },
					             but it differed as $.price was 10.06 instead of 10 ± 0.05

					             JSON options:
					              - ignore additional properties
					              - compare numbers within ± 0.05
					             """);
			}
		}
	}
}
