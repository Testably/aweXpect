using System.Collections.Generic;
using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed as $ was {json} instead of {expected}
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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenSubjectHasAdditionalNullProperty_ShouldFailWhenTreatingNullAsAbsent(bool treatNullAsAbsent)
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(new object(),
						o => o.TreatingNullAsAbsent(treatNullAsAbsent)));

				await That(Act).Throws<FailException>().OnlyIf(treatNullAsAbsent)
					.WithMessage("""
					             Expected that subject
					             does not match new object() exactly,
					             but it did

					             Actual:
					             {
					               "foo": null
					             }

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenElementsAreInDifferentOrder_ShouldFailWhenIgnoringArrayOrder(bool ignoreArrayOrder)
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly([2, 1,],
						o => o.IgnoringArrayOrder(ignoreArrayOrder)));

				await That(Act).Throws<FailException>().OnlyIf(ignoreArrayOrder)
					.WithMessage("""
					             Expected that subject
					             does not match [2, 1,] exactly,
					             but it did

					             Actual:
					             [
					               1,
					               2
					             ]

					             JSON options:
					              - ignore array order
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
					=> await That(subject).MatchesExactly(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NotMatchingArrayValues))]
			public async Task NotMatchingValues_ShouldFail(string[] expected, string json, string errorMessage)
			{
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString("[1, 2]");

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
				JsonElement subject = FromString("[1, 2]");

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
				JsonElement subject = FromString("[1, 2, 3]");

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
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringAdditionalProperties_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, 3]");

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
			public async Task WhenNestedArrayContainsAdditionalElements_WhenIgnoringAdditionalArrayElements_ShouldOnlyReportAdditionalProperties()
			{
				JsonElement subject = FromString("""{"id": 1, "items": ["A", "B", "C"]}""");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						items = new[] { "A", "B", },
					}, o => o.IgnoringAdditionalArrayElements());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	items = new[] { "A", "B", },
					             },
					             but it differed as $.id had unexpected 1

					             JSON options:
					              - ignore additional array elements
					             """)
					.Because("ignoring additional array elements does not affect the properties of objects");
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalNullElement_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("[1, null]");

				async Task Act()
					=> await That(subject).MatchesExactly([1,], o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1,] exactly,
					             but it differed as $[1] had unexpected <null>

					             JSON options:
					              - treat null as absent
					             """)
					.Because("null elements of an array are not affected by the option");
			}

			[Test]
			public async Task WhenExpectedContainsAdditionalNullElement_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, (int?)null,], o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, (int?)null,] exactly,
					             but it differed as $[1] had missing <null>

					             JSON options:
					              - treat null as absent
					             """)
					.Because("null elements of an array are not affected by the option");
			}

			[Test]
			[Arguments("[1, 2, 3]")]
			[Arguments("[3, 1, 2]")]
			[Arguments("[2, 3, 1]")]
			public async Task WhenElementsAreInAnyOrder_WhenIgnoringArrayOrder_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2, 3,], o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBothArraysAreEmpty_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("[]");

				async Task Act()
					=> await That(subject).MatchesExactly(Array.Empty<int>(), o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenElementsAreInDifferentOrder_WhenNotIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([2, 1,], o => o.IgnoringArrayOrder(false));

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
			public async Task WhenDuplicateElementsAreInDifferentOrder_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([2, 1, 1,], o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedContainsDuplicateElement_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 1,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 1,] exactly,
					             but it differed as $[1] was 2 instead of 1

					             JSON options:
					              - ignore array order
					             """)
					.Because("each element of the subject can only be paired with one expected element");
			}

			[Test]
			public async Task WhenSubjectContainsDuplicateElement_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[2, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2, 3,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2, 3,] exactly,
					             but it differed as $[1] was 2 instead of 3

					             JSON options:
					              - ignore array order
					             """)
					.Because("the single unpaired elements are compared with each other");
			}

			[Test]
			public async Task WhenElementsAreMissingAndUnexpected_WhenIgnoringArrayOrder_ShouldListThem()
			{
				JsonElement titles = FromString("[\"Let It Be\", \"Yesterday\", \"Hey Jude\"]");

				async Task Act()
					=> await That(titles).MatchesExactly(["Hey Jude", "Something", "Help!",],
						o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that titles
					             matches ["Hey Jude", "Something", "Help!",] exactly,
					             but it differed as
					               $ had missing "Something" and
					               $ had missing "Help!" and
					               $[0] had unexpected "Let It Be" and
					               $[1] had unexpected "Yesterday"

					             JSON options:
					              - ignore array order
					             """)
					.Because("missing elements have no index in the subject, while unexpected elements keep theirs");
			}

			[Test]
			public async Task WhenSingleElementDiffers_WhenIgnoringArrayOrder_ShouldCompareTheUnpairedElements()
			{
				JsonElement titles = FromString("[\"Let It Be\", \"Yesterday\", \"Hey Jude\"]");

				async Task Act()
					=> await That(titles).MatchesExactly(["Hey Jude", "Let It Be", "Something",],
						o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that titles
					             matches ["Hey Jude", "Let It Be", "Something",] exactly,
					             but it differed as $[1] was "Yesterday" instead of "Something"

					             JSON options:
					              - ignore array order
					             """)
					.Because("the only unpaired elements on both sides are compared with each other");
			}

			[Test]
			public async Task WhenExpectedContainsAdditionalElements_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2, 3, 4,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2, 3, 4,] exactly,
					             but it differed as
					               $ had missing 3 and
					               $ had missing 4

					             JSON options:
					              - ignore array order
					             """);
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[3, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,] exactly,
					             but it differed as $[0] had unexpected 3

					             JSON options:
					              - ignore array order
					             """);
			}

			[Test]
			[Arguments("[]", "$ had missing 1")]
			[Arguments("[1]", null)]
			[Arguments("[2]", "$[0] was 2 instead of 1")]
			public async Task WhenExpectingSingleElement_WhenIgnoringArrayOrder_ShouldFailUnlessItMatches(
				string json, string? errorMessage)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly([1,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>().OnlyIf(errorMessage != null)
					.WithMessage($"""
					              Expected that subject
					              matches [1,] exactly,
					              but it differed as {errorMessage}

					              JSON options:
					               - ignore array order
					              """);
			}

			[Test]
			public async Task WhenExpectedIsEmpty_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).MatchesExactly(Array.Empty<int>(), o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches Array.Empty<int>() exactly,
					             but it differed as $[0] had unexpected 1

					             JSON options:
					              - ignore array order
					             """);
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringArrayOrderAndAdditionalArrayElements_ShouldSucceed()
			{
				JsonElement subject = FromString("[4, 3, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 3,],
						o => o.IgnoringArrayOrder().IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow()
					.Because("the expected elements must only be a subset of the subject in any order");
			}

			[Test]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringArrayOrderAndAdditionalProperties_ShouldFail()
			{
				JsonElement subject = FromString("[4, 3, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 3,],
						o => o.IgnoringArrayOrder().IgnoringAdditionalProperties());

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
					.Because("ignoring additional properties does not affect the elements of arrays");
			}

			[Test]
			public async Task WhenElementIsMissing_WhenIgnoringArrayOrderAndAdditionalArrayElements_ShouldOnlyReportMissingElement()
			{
				JsonElement subject = FromString("[4, 3, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 5,],
						o => o.IgnoringArrayOrder().IgnoringAdditionalArrayElements());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 5,],
					             but it differed as $ had missing 5

					             JSON options:
					              - ignore additional array elements
					              - ignore array order
					             """);
			}

			[Test]
			public async Task WhenElementsOfEveryKindAreInAnyOrder_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [null, true, false, "a", 1, [1, 2, 3], [1, 2], {"a": 1, "b": 2}, {"a": 2, "b": 2}]
				                                 """);
				JsonElement expected = FromString("""
				                                  [{"a": 2, "b": 2}, {"a": 1, "b": 2}, [2, 1], [3, 2, 1], 1, "a", false, true, null]
				                                  """);

				async Task Act()
					=> await That(subject).MatchesExactly(expected, o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow()
					.Because("each expected element is paired with the element of any kind that matches it");
			}

			[Test]
			public async Task WhenElementsOfEveryKindDiffer_WhenIgnoringArrayOrder_ShouldReportThemAsMissingAndUnexpected()
			{
				JsonElement subject = FromString("""
				                                 [null, true, "a", [1, 2, 3], {"a": 1, "b": 2}]
				                                 """);
				JsonElement expected = FromString("""
				                                  [{"a": 1, "b": 3}, [3, 1], "b", false, 1]
				                                  """);

				async Task Act()
					=> await That(subject).MatchesExactly(expected, o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches expected exactly,
					             but it differed as
					               $ had missing {"a":1,"b":3} and
					               $ had missing [3,1] and
					               $ had missing "b" and
					               $ had missing False and
					               $ had missing 1 and
					               $[0] had unexpected <null> and
					               $[1] had unexpected True and
					               $[2] had unexpected "a" and
					               $[3] had unexpected [1,2,3] and
					               $[4] had unexpected {"a":1,"b":2}

					             JSON options:
					              - ignore array order
					             """)
					.Because("a candidate of another kind, with another value or with an additional element does not match");
			}

			[Test]
			public async Task WhenNestedArraysAreInDifferentOrder_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("[[1, 2], [3, 4, 5]]");

				async Task Act()
					=> await That(subject).MatchesExactly(new[]
					{
						new[] { 5, 3, 4, },
						new[] { 2, 1, },
					}, o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNestedArrayDiffers_WhenIgnoringArrayOrder_ShouldReportTheNestedDifference()
			{
				JsonElement subject = FromString("[[1, 2], [3, 4]]");

				async Task Act()
					=> await That(subject).MatchesExactly(new[]
					{
						new[] { 4, 3, },
						new[] { 2, 5, },
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new[]
					             {
					             	new[] { 4, 3, },
					             	new[] { 2, 5, },
					             } exactly,
					             but it differed as $[0][0] was 1 instead of 5

					             JSON options:
					              - ignore array order
					             """);
			}

			[Test]
			public async Task WhenObjectsAreInDifferentOrder_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   {"id": 1, "tags": ["a", "b"]},
				                                   {"id": 2, "tags": []}
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).MatchesExactly(new object[]
					{
						new
						{
							id = 2,
							tags = Array.Empty<string>(),
						},
						new
						{
							id = 1,
							tags = new[] { "b", "a", },
						},
					}, o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenObjectDiffers_WhenIgnoringArrayOrder_ShouldReportTheDifferentProperty()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   {"id": 1, "name": "foo"},
				                                   {"id": 2, "name": "bar"}
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).MatchesExactly(new object[]
					{
						new
						{
							id = 2,
							name = "bar",
						},
						new
						{
							id = 1,
							name = "baz",
						},
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new object[]
					             {
					             	new
					             	{
					             		id = 2,
					             		name = "bar",
					             	},
					             	new
					             	{
					             		id = 1,
					             		name = "baz",
					             	},
					             } exactly,
					             but it differed as $[0].name was "foo" instead of "baz"

					             JSON options:
					              - ignore array order
					             """);
			}

			[Test]
			public async Task WhenObjectsDiffer_WhenIgnoringArrayOrder_ShouldReportThemAsMissingAndUnexpected()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   {"id": 1},
				                                   {"id": 2},
				                                   {"id": 3}
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).MatchesExactly(new object[]
					{
						new
						{
							id = 4,
						},
						new
						{
							id = 2,
						},
						new
						{
							id = 5,
						},
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new object[]
					             {
					             	new
					             	{
					             		id = 4,
					             	},
					             	new
					             	{
					             		id = 2,
					             	},
					             	new
					             	{
					             		id = 5,
					             	},
					             } exactly,
					             but it differed as
					               $ had missing {"id":4} and
					               $ had missing {"id":5} and
					               $[0] had unexpected {"id":1} and
					               $[2] had unexpected {"id":3}

					             JSON options:
					              - ignore array order
					             """);
			}

			[Test]
			public async Task WhenArrayInObjectIsInDifferentOrder_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": [1, 2, 3]}}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = new
						{
							bar = new[] { 3, 2, 1, },
						},
					}, o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenArrayInObjectDiffers_WhenIgnoringArrayOrder_ShouldReportItsPath()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": [1, 2, 3]}}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = new
						{
							bar = new[] { 5, 4, 3, },
						},
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = new
					             	{
					             		bar = new[] { 5, 4, 3, },
					             	},
					             } exactly,
					             but it differed as
					               $.foo.bar had missing 5 and
					               $.foo.bar had missing 4 and
					               $.foo.bar[0] had unexpected 1 and
					               $.foo.bar[1] had unexpected 2

					             JSON options:
					              - ignore array order
					             """);
			}

			[Test]
			public async Task WhenItIsMatchesLaterElement_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   {"id": 1, "name": "foo"},
				                                   {"id": 2, "name": "bar"}
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).MatchesExactly(new object[]
					{
						new
						{
							id = It.Is<int>().That.IsGreaterThan(1),
							name = It.Is<string>().That.StartsWith("b"),
						},
						new
						{
							id = 1,
							name = "foo",
						},
					}, o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow()
					.Because("the expectations fail for the first element, but are met by the second");
			}

			[Test]
			public async Task WhenItIsOverlapsWithLaterElement_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly(new object[]
					{
						It.Is<int>().That.IsGreaterThan(0),
						1,
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new object[]
					             {
					             	It.Is<int>().That.IsGreaterThan(0),
					             	1,
					             } exactly,
					             but it differed as $[1] was 2 instead of 1

					             JSON options:
					              - ignore array order
					             """)
					.Because("the greedy pairing pairs the first element with the expectation, although only the other pairing succeeds");
			}

			[Test]
			public async Task WhenItIsDoesNotMatchRemainingElement_WhenIgnoringArrayOrder_ShouldOnlyReportItsFailure()
			{
				JsonElement subject = FromString("[\"foo\", \"bar\"]");

				async Task Act()
					=> await That(subject).MatchesExactly(new[]
					{
						It.Is<string>().That.StartsWith("b"),
						It.Is<string>().That.StartsWith("x"),
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new[]
					             {
					             	It.Is<string>().That.StartsWith("b"),
					             	It.Is<string>().That.StartsWith("x"),
					             } exactly,
					             but it differed as $[0] was "foo", which differs at index 0:
					                ↓ (actual)
					               "foo"
					               "x"
					                ↑ (expected prefix)

					             JSON options:
					              - ignore array order
					             """)
					.Because("the failures of the candidates that were not paired must not be reported");
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
			[Arguments("{}", "$.foo had missing 2")]
			[Arguments("{\"foo\": 2}")]
			public async Task ShouldFailIfPropertyIsMissing(string json,
				string? errorMessage = null)
			{
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString("{\"bar\": 2}");

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
				JsonElement subject = FromString("{\"foo\": null, \"bar\": 2}");

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
				JsonElement subject = FromString("{\"foo\": null, \"bar\": 2}");

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
				JsonElement subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						bar = 2,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasAdditionalNonNullProperty_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						bar = 2,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	bar = 2,
					             } exactly,
					             but it differed as $.foo had unexpected 1

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenExpectedNullPropertyIsMissing_ShouldFail()
			{
				JsonElement subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = (string?)null,
						bar = 2,
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = (string?)null,
					             	bar = 2,
					             } exactly,
					             but it differed as $.foo had missing <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedNullPropertyIsMissing_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = (string?)null,
						bar = 2,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedNullPropertyHasValue_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = (int?)null,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = (int?)null,
					             } exactly,
					             but it differed as $.foo was number 1 instead of <null>

					             JSON options:
					              - treat null as absent
					             """)
					.Because("only a missing property is treated like a property with a null value");
			}

			[Test]
			public async Task WhenPropertyIsNullInsteadOfValue_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = 2,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = 2,
					             } exactly,
					             but it differed as $.foo was <null> instead of 2

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenNestedNullPropertiesDiffer_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": null}, \"baz\": {}}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = new object(),
						baz = new
						{
							bar = (int?)null,
						},
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNullPropertiesOfObjectsInArrayDiffer_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"items\": [{\"a\": 1, \"b\": null}, {\"a\": 2}]}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						items = new object[]
						{
							new
							{
								a = 1,
							},
							new
							{
								a = 2,
								b = (int?)null,
							},
						},
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringMemberTests
		{
			[Test]
			public async Task WhenGeneratedValuesDiffer_ShouldSucceedAsDocumentedInTheReadme()
			{
				JsonElement track = FromString(
					"""{"id": 42, "title": "Let It Be", "meta": {"createdAt": "2026-10-16", "etag": "abc"}}""");

				async Task Act()
					=> await That(track).MatchesExactly(new { id = 1, title = "Let It Be", meta = new { createdAt = "2026-01-01" } },
						o => o.IgnoringMember("id").IgnoringMember("createdAt").IgnoringMembers(path => path.EndsWith(".etag")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNameIsMisspelled_ShouldFailAsDocumentedInTheReadme()
			{
				JsonElement track = FromString("""{"title": "Let It Be", "createdAt": "2026-10-16"}""");

				async Task Act()
					=> await That(track).MatchesExactly(new { title = "Yesterday", createdAt = "2026-01-01" },
						o => o.IgnoringMember("craetedAt"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that track
					             matches new { title = "Yesterday", createdAt = "2026-01-01" } exactly,
					             but it differed as
					               $.title was "Let It Be" instead of "Yesterday" and
					               $.createdAt was "2026-10-16" instead of "2026-01-01"

					             JSON options:
					              - ignore members: ["craetedAt"]
					                "craetedAt" matched no member (did you mean "createdAt"?)
					             """);
			}

			[Test]
			public async Task WhenPropertiesDifferAtAnyDepth_ShouldIgnoreThem()
			{
				JsonElement subject = FromString(
					"{\"id\": 1, \"inner\": {\"id\": 2, \"name\": \"a\"}, \"items\": [{\"id\": 3}]}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						id = 9,
						inner = new
						{
							id = 8,
							name = "a",
						},
						items = new[]
						{
							new
							{
								id = 7,
							},
						},
					}, o => o.IgnoringMember("id"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOtherPropertyDiffers_ShouldStillReportIt()
			{
				JsonElement subject = FromString("{\"id\": 1, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
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
					             } exactly,
					             but it differed as $.name was "a" instead of "b"

					             JSON options:
					              - ignore members: ["id"]
					             """);
			}

			[Test]
			public async Task WhenNameMatchesOnlyPartOfASegment_ShouldNotIgnoreIt()
			{
				JsonElement subject = FromString("{\"parentId\": 1}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						parentId = 2,
					}, o => o.IgnoringMember("id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	parentId = 2,
					             } exactly,
					             but it differed as $.parentId was 1 instead of 2

					             JSON options:
					              - ignore members: ["id"]
					                "id" matched no member
					             """)
					.Because("the name must cover whole segments of the path");
			}

			[Test]
			public async Task WhenNameDiffersInCasing_ShouldNotIgnoreThePropertyButSuggestIt()
			{
				JsonElement subject = FromString("{\"Id\": 1}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						Id = 2,
					}, o => o.IgnoringMember("id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	Id = 2,
					             } exactly,
					             but it differed as $.Id was 1 instead of 2

					             JSON options:
					              - ignore members: ["id"]
					                "id" matched no member (did you mean "Id"?)
					             """)
					.Because("JSON property names are case-sensitive, so a name in the wrong casing only gets a hint");
			}

			[Test]
			public async Task WhenParentNameContainsABracket_ShouldIgnoreTheProperty()
			{
				JsonElement subject = FromString("{\"a[b\": {\"id\": 1}}");

				async Task Act()
					=> await That(subject).MatchesExactly(new Dictionary<string, object>
					{
						["a[b"] = new
						{
							id = 2,
						},
					}, o => o.IgnoringMember("id"));

				await That(Act).DoesNotThrow()
					.Because("a JSON path writes property names after a '.', never between brackets");
			}

			[Test]
			public async Task WhenPropertiesDifferOnlyInCasing_ShouldOnlyIgnoreTheMatchingOne()
			{
				JsonElement subject = FromString("{\"id\": 1, \"ID\": \"abbey-road\"}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						id = 7,
						ID = "let-it-be",
					}, o => o.IgnoringMember("id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	id = 7,
					             	ID = "let-it-be",
					             } exactly,
					             but it differed as $.ID was "abbey-road" instead of "let-it-be"

					             JSON options:
					              - ignore members: ["id"]
					             """);
			}

			[Test]
			public async Task WhenNameHasSeveralSegments_ShouldOnlyIgnoreMatchingPaths()
			{
				JsonElement subject = FromString("{\"inner\": {\"id\": 1}, \"other\": {\"id\": 2}}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						inner = new
						{
							id = 9,
						},
						other = new
						{
							id = 8,
						},
					}, o => o.IgnoringMember("inner.id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	inner = new
					             	{
					             		id = 9,
					             	},
					             	other = new
					             	{
					             		id = 8,
					             	},
					             } exactly,
					             but it differed as $.other.id was 2 instead of 8

					             JSON options:
					              - ignore members: ["inner.id"]
					             """);
			}

			[Test]
			public async Task WhenNameStartsWithTheRoot_ShouldOnlyIgnoreThePropertyOfTheRoot()
			{
				JsonElement subject = FromString("{\"id\": 1, \"inner\": {\"id\": 2}}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						id = 9,
						inner = new
						{
							id = 8,
						},
					}, o => o.IgnoringMember("$.id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	id = 9,
					             	inner = new
					             	{
					             		id = 8,
					             	},
					             } exactly,
					             but it differed as $.inner.id was 2 instead of 8

					             JSON options:
					              - ignore members: ["$.id"]
					             """);
			}

			[Test]
			public async Task WhenNameIsAFullPath_ShouldIgnoreThePath()
			{
				JsonElement subject = FromString("{\"items\": [{\"etag\": \"a\"}, {\"etag\": \"b\"}]}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						items = new[]
						{
							new
							{
								etag = "x",
							},
							new
							{
								etag = "b",
							},
						},
					}, o => o.IgnoringMember("$.items[0].etag"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNameIsAnIndex_ShouldIgnoreTheElementOfEveryArray()
			{
				JsonElement subject = FromString("{\"a\": [1, 2], \"b\": [3, 4]}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						a = new[]
						{
							9, 2,
						},
						b = new[]
						{
							8, 4,
						},
					}, o => o.IgnoringMember("[0]"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIgnoredPropertyIsMissing_ShouldNotReportIt()
			{
				JsonElement subject = FromString("{\"name\": \"a\"}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						id = 1,
						name = "a",
					}, o => o.IgnoringMember("id"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIgnoredPropertyIsUnexpected_ShouldNotReportIt()
			{
				JsonElement subject = FromString("{\"id\": 1, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						name = "a",
					}, o => o.IgnoringMember("id"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIgnoredElementIsMissing_ShouldNotReportIt()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringMember("[1]"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIgnoredElementIsUnexpected_ShouldNotReportIt()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([1,], o => o.IgnoringMember("[1]"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIgnoredValueHasADifferentKind_ShouldNotReportIt()
			{
				JsonElement subject = FromString("{\"meta\": [1, 2]}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						meta = "x",
					}, o => o.IgnoringMember("meta"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenChildrenOfIgnoredPathDiffer_ShouldNotReportThem()
			{
				JsonElement subject = FromString("{\"meta\": {\"a\": 1, \"b\": 2}, \"name\": \"x\"}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						meta = new
						{
							a = 9,
							c = 3,
						},
						name = "x",
					}, o => o.IgnoringMember("meta"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPredicateMatchesThePath_ShouldIgnoreIt()
			{
				JsonElement subject = FromString("{\"items\": [{\"etag\": \"a\", \"name\": \"x\"}], \"etag\": 1}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						items = new[]
						{
							new
							{
								etag = "b",
								name = "x",
							},
						},
						etag = 2,
					}, o => o.IgnoringMembers(path => path.StartsWith("$.items[") && path.EndsWith(".etag")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	items = new[]
					             	{
					             		new
					             		{
					             			etag = "b",
					             			name = "x",
					             		},
					             	},
					             	etag = 2,
					             } exactly,
					             but it differed as $.etag was 1 instead of 2

					             JSON options:
					              - ignore members: [path => path.StartsWith("$.items[") && path.EndsWith(".etag")]
					             """);
			}

			[Test]
			public async Task WhenIgnoringSeveralMembers_ShouldIgnoreAllOfThem()
			{
				JsonElement subject = FromString("{\"id\": 1, \"createdAt\": \"today\", \"etag\": \"a\", \"name\": \"x\"}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						id = 2,
						createdAt = "yesterday",
						etag = "b",
						name = "x",
					}, o => o
						.IgnoringMember("id")
						.IgnoringMember("createdAt")
						.IgnoringMembers(path => path == "$.etag"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenElementsDifferInIgnoredMember_WhenIgnoringArrayOrder_ShouldPairThem()
			{
				JsonElement subject = FromString("[{\"id\": 1, \"name\": \"a\"}, {\"id\": 2, \"name\": \"b\"}]");

				async Task Act()
					=> await That(subject).MatchesExactly(new[]
					{
						new
						{
							id = 8,
							name = "b",
						},
						new
						{
							id = 9,
							name = "a",
						},
					}, o => o.IgnoringMember("id").IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIndexIsIgnored_WhenIgnoringArrayOrder_ShouldLeaveTheElementOfTheSubjectOutOfThePairing()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringMember("[0]").IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,] exactly,
					             but it differed as $[2] was 3 instead of 1

					             JSON options:
					              - ignore array order
					              - ignore members: ["[0]"]
					             """)
					.Because("the index refers to the element of the subject, so the expected 1 cannot pair with it");
			}

			[Test]
			public async Task WhenIndexIsIgnored_WhenIgnoringArrayOrder_ShouldNotReportTheElementAsUnexpected()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([3, 2,], o => o.IgnoringMember("[0]").IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNameOnlyMatchedWhilePairing_WhenIgnoringArrayOrder_ShouldNotNameIt()
			{
				JsonElement subject = FromString("[{\"id\": 1, \"a\": 1}, {\"id\": 2, \"a\": 2}]");

				async Task Act()
					=> await That(subject).MatchesExactly(new[]
					{
						new
						{
							id = 9,
							a = 3,
						},
						new
						{
							id = 9,
							a = 4,
						},
					}, o => o.IgnoringMember("id").IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new[]
					             {
					             	new
					             	{
					             		id = 9,
					             		a = 3,
					             	},
					             	new
					             	{
					             		id = 9,
					             		a = 4,
					             	},
					             } exactly,
					             but it differed as
					               $ had missing {"id":9,"a":3} and
					               $ had missing {"id":9,"a":4} and
					               $[0] had unexpected {"id":1,"a":1} and
					               $[1] had unexpected {"id":2,"a":2}

					             JSON options:
					              - ignore array order
					              - ignore members: ["id"]
					             """)
					.Because("the name matched while the elements were paired");
			}
		}

		public sealed class ComparingNumbersWithinTests
		{
			[Test]
			public async Task WhenNumbersAreClose_ShouldSucceedAsDocumentedInTheReadme()
			{
				JsonElement invoice = FromString("""{"total": 0.30000000000000004, "tax": 0.0575}""");

				async Task ActAbsolute()
					=> await That(invoice).MatchesExactly(new { total = 0.3, tax = 0.057m },
						o => o.ComparingNumbersWithin(0.001));

				async Task ActRelative()
					=> await That(invoice).MatchesExactly(new { tax = 0.057m },
						o => o.ComparingNumbersWithin(1).Percent().IgnoringAdditionalProperties());

				await That(ActAbsolute).DoesNotThrow();
				await That(ActRelative).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberIsOutsideTheRelativeTolerance_ShouldFailAsDocumentedInTheReadme()
			{
				JsonElement product = FromString("""{"name": "Guitar", "price": 110}""");

				async Task Act()
					=> await That(product).MatchesExactly(new { name = "Guitar", price = 100 },
						o => o.ComparingNumbersWithin(5).Percent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that product
					             matches new { name = "Guitar", price = 100 } exactly,
					             but it differed as $.price was 110 instead of 100 ± 5%

					             JSON options:
					              - compare numbers within ± 5%
					             """);
			}

			[Test]
			[Arguments("10.4", true)]
			[Arguments("10.5", true)]
			[Arguments("9.5", true)]
			[Arguments("10.6", false)]
			[Arguments("9.4", false)]
			public async Task WithAbsoluteTolerance_ShouldIncludeTheBoundaries(string json, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(10, o => o.ComparingNumbersWithin(0.5));

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches 10 exactly,
					              but it differed as $ was {json} instead of 10 ± 0.5

					              JSON options:
					               - compare numbers within ± 0.5
					              """);
			}

			[Test]
			[Arguments("104", true)]
			[Arguments("105", true)]
			[Arguments("95", true)]
			[Arguments("105.1", false)]
			[Arguments("94.9", false)]
			public async Task WithRelativeTolerance_ShouldIncludeTheBoundaries(string json, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(100, o => o.ComparingNumbersWithin(5).Percent());

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches 100 exactly,
					              but it differed as $ was {json} instead of 100 ± 5%

					              JSON options:
					               - compare numbers within ± 5%
					              """);
			}

			[Test]
			[Arguments("-104", true)]
			[Arguments("-96", true)]
			[Arguments("-106", false)]
			public async Task WithRelativeTolerance_WhenExpectedIsNegative_ShouldUseItsMagnitude(string json,
				bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(-100, o => o.ComparingNumbersWithin(5).Percent());

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches -100 exactly,
					              but it differed as $ was {json} instead of -100 ± 5%

					              JSON options:
					               - compare numbers within ± 5%
					              """);
			}

			[Test]
			[Arguments("0", true)]
			[Arguments("0.0", true)]
			[Arguments("0.0001", false)]
			public async Task WithRelativeTolerance_WhenExpectedIsZero_ShouldOnlyMatchAnEqualNumber(string json,
				bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(0, o => o.ComparingNumbersWithin(50).Percent());

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches 0 exactly,
					              but it differed as $ was {json} instead of 0 ± 50%

					              JSON options:
					               - compare numbers within ± 50%
					              """)
					.Because("a percentage of zero is zero, like in aweXpect");
			}

			[Test]
			public async Task WhenIntegersDiffer_ShouldAlsoApplyTheTolerance()
			{
				JsonElement subject = FromString("{\"id\": 41, \"count\": 3}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						id = 42,
						count = 2,
					}, o => o.ComparingNumbersWithin(1));

				await That(Act).DoesNotThrow()
					.Because("the tolerance applies to all numbers, also to integers such as ids or counts");
			}

			[Test]
			public async Task WhenToleranceIsZero_ShouldOnlyMatchEqualNumbers()
			{
				JsonElement subject = FromString("[1.0, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 3,], o => o.ComparingNumbersWithin(0));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 3,] exactly,
					             but it differed as $[1] was 2 instead of 3 ± 0

					             JSON options:
					              - compare numbers within ± 0
					             """);
			}

			[Test]
			public async Task WhenNestedNumbersDiffer_ShouldApplyTheToleranceToEachOfThem()
			{
				JsonElement subject = FromString(
					"{\"items\": [{\"price\": 10.04, \"tags\": [1.98]}, {\"price\": 20.1, \"tags\": [3]}]}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						items = new[]
						{
							new
							{
								price = 10,
								tags = new[] { 2, },
							},
							new
							{
								price = 20,
								tags = new[] { 3, },
							},
						},
					}, o => o.ComparingNumbersWithin(0.05));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	items = new[]
					             	{
					             		new
					             		{
					             			price = 10,
					             			tags = new[] { 2, },
					             		},
					             		new
					             		{
					             			price = 20,
					             			tags = new[] { 3, },
					             		},
					             	},
					             } exactly,
					             but it differed as $.items[1].price was 20.1 instead of 20 ± 0.05

					             JSON options:
					              - compare numbers within ± 0.05
					             """);
			}

			[Test]
			public async Task WhenIgnoringArrayOrder_ShouldPairTheElementsWithinTheTolerance()
			{
				JsonElement subject = FromString("[2.01, 0.99]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringArrayOrder().ComparingNumbersWithin(0.02));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIgnoringArrayOrder_WhenAnElementIsOutsideTheTolerance_ShouldCompareTheUnpairedElements()
			{
				JsonElement subject = FromString("[2.5, 0.99]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.ComparingNumbersWithin(0.02).IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,] exactly,
					             but it differed as $[0] was 2.5 instead of 2 ± 0.02

					             JSON options:
					              - ignore array order
					              - compare numbers within ± 0.02
					             """);
			}

			[Test]
			public async Task WhenOtherOptionsAreChainedAfterPercent_ShouldApplyAllOfThem()
			{
				JsonElement subject = FromString("{\"values\": [104, 3, 9], \"extra\": true}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						values = new[] { 100, },
					}, o => o.ComparingNumbersWithin(5).Percent().IgnoringAdditionalProperties().IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberIsNotExpected_ShouldStillReportTheDifferentKind()
			{
				JsonElement subject = FromString("{\"value\": \"1\"}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						value = 1,
					}, o => o.ComparingNumbersWithin(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	value = 1,
					             } exactly,
					             but it differed as $.value was string "1" instead of 1

					             JSON options:
					              - compare numbers within ± 1
					             """)
					.Because("the tolerance only applies to numbers");
			}

			[Test]
			public async Task WhenUsingAnExpectation_ShouldNotApplyTheTolerance()
			{
				JsonElement subject = FromString("{\"value\": 1.5}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						value = It.Is<double>().That.IsEqualTo(1),
					}, o => o.ComparingNumbersWithin(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	value = It.Is<double>().That.IsEqualTo(1),
					             } exactly,
					             but it differed as $.value was 1.5, which differs by 0.5

					             JSON options:
					              - compare numbers within ± 1
					             """)
					.Because("an expectation has its own tolerance");
			}
		}

		public sealed class ComparingNumbersWithinNegatedTests
		{
			[Test]
			[Arguments("10.4", true)]
			[Arguments("10.6", false)]
			public async Task WhenNumberIsWithinTheTolerance_ShouldFail(string json, bool isWithin)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(10, o => o.ComparingNumbersWithin(0.5)));

				await That(Act).Throws<FailException>().OnlyIf(isWithin)
					.WithMessage($"""
					              Expected that subject
					              does not match 10 exactly,
					              but it did

					              Actual:
					              {json}

					              JSON options:
					               - compare numbers within ± 0.5
					              """);
			}
		}
	}
}
