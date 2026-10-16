using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class MatchesExactly
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("true", true, true)]
			[InlineData("true", false, false)]
			[InlineData("false", true, false)]
			[InlineData("false", false, true)]
			public async Task BooleanValue_ShouldSucceedWhenMatching(string json, bool expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed as $ was {subject} instead of {expected}
					              """);
			}

			[Theory]
			[InlineData("42.1", 42.1, true)]
			[InlineData("1.2", 2.1, false)]
			public async Task DoubleValue_ShouldSucceedWhenMatching(string json, double expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed as $ was {json} instead of *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData("42", 42, true)]
			[InlineData("1", 2, false)]
			public async Task IntegerValue_ShouldSucceedWhenMatching(string json, int expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed as $ was {json} instead of {expected}
					              """);
			}

			[Theory]
			[InlineData("null", true)]
			[InlineData("{}", false)]
			public async Task NullValue_ShouldSucceedWhenMatching(string json, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(null);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches null exactly,
					              but it differed as $ was object {json} instead of <null>
					              """);
			}

			[Theory]
			[InlineData("\"foo\"", "foo", true)]
			[InlineData("\"foo\"", "bar", false)]
			public async Task StringValue_ShouldSucceedWhenMatching(string json, string expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed as $ was {json} instead of "{expected}"
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("true", true, true)]
			[InlineData("true", false, false)]
			[InlineData("false", true, false)]
			[InlineData("false", false, true)]
			public async Task BooleanValue_ShouldFailWhenMatching(string json, bool expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(expected));

				await That(Act).Throws<XunitException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected exactly,
					              but it did

					              Actual:
					              {json}
					              """);
			}

			[Theory]
			[InlineData("42.1", 42.1, true)]
			[InlineData("1.2", 2.1, false)]
			public async Task DoubleValue_ShouldFailWhenMatching(string json, double expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(expected));

				await That(Act).Throws<XunitException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected exactly,
					              but it did

					              Actual:
					              {json}
					              """);
			}

			[Theory]
			[InlineData("42", 42, true)]
			[InlineData("1", 2, false)]
			public async Task IntegerValue_ShouldFailWhenMatching(string json, int expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(expected));

				await That(Act).Throws<XunitException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected exactly,
					              but it did

					              Actual:
					              {json}
					              """);
			}

			[Theory]
			[InlineData("null", true)]
			[InlineData("{}", false)]
			public async Task NullValue_ShouldFailWhenMatching(string json, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(null));

				await That(Act).Throws<XunitException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match null exactly,
					              but it did

					              Actual:
					              {json}
					              """);
			}

			[Theory]
			[InlineData("\"foo\"", "foo", true)]
			[InlineData("\"foo\"", "bar", false)]
			public async Task StringValue_ShouldFailWhenMatching(string json, string expected, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(expected));

				await That(Act).Throws<XunitException>().OnlyIf(isMatch)
					.WithMessage($"""
					              Expected that subject
					              does not match expected exactly,
					              but it did

					              Actual:
					              {json}
					              """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenSubjectHasAdditionalNullProperty_ShouldFailWhenTreatingNullAsAbsent(bool treatNullAsAbsent)
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(new object(),
						o => o.TreatingNullAsAbsent(treatNullAsAbsent)));

				await That(Act).Throws<XunitException>().OnlyIf(treatNullAsAbsent)
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

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenElementsAreInDifferentOrder_ShouldFailWhenIgnoringArrayOrder(bool ignoreArrayOrder)
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly([2, 1,],
						o => o.IgnoringArrayOrder(ignoreArrayOrder)));

				await That(Act).Throws<XunitException>().OnlyIf(ignoreArrayOrder)
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
			[Theory]
			[MemberData(nameof(MatchingArrayValues))]
			public async Task MatchingValues_ShouldSucceed(string[] expected, string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NotMatchingArrayValues))]
			public async Task NotMatchingValues_ShouldFail(string[] expected, string json, string errorMessage)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches expected exactly,
					              but it differed {errorMessage}
					              """);
			}

			[Fact]
			public async Task WhenElementsAreInDifferentOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([2, 1,]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [2, 1,] exactly,
					             but it differed as
					               $[0] was 1 instead of 2 and
					               $[1] was 2 instead of 1
					             """);
			}

			[Fact]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2, 3,]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2, 3,] exactly,
					             but it differed as $[2] had missing 3
					             """);
			}

			[Fact]
			public async Task WhenSubjectContainsAdditionalElements_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,] exactly,
					             but it differed as $[2] had unexpected 3
					             """);
			}

			[Fact]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringAdditionalArrayElements_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringAdditionalProperties_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringAdditionalProperties());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,],
					             but it differed as $[2] had unexpected 3

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("ignoring additional properties does not affect the elements of arrays");
			}

			[Fact]
			public async Task WhenNestedArrayContainsAdditionalElements_WhenIgnoringAdditionalArrayElements_ShouldOnlyReportAdditionalProperties()
			{
				JsonElement subject = FromString("""{"id": 1, "items": ["A", "B", "C"]}""");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						items = new[] { "A", "B", },
					}, o => o.IgnoringAdditionalArrayElements());

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenSubjectContainsAdditionalNullElement_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("[1, null]");

				async Task Act()
					=> await That(subject).MatchesExactly([1,], o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1,] exactly,
					             but it differed as $[1] had unexpected <null>

					             JSON options:
					              - treat null as absent
					             """)
					.Because("null elements of an array are not affected by the option");
			}

			[Fact]
			public async Task WhenExpectedContainsAdditionalNullElement_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, (int?)null,], o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, (int?)null,] exactly,
					             but it differed as $[1] had missing <null>

					             JSON options:
					              - treat null as absent
					             """)
					.Because("null elements of an array are not affected by the option");
			}

			[Theory]
			[InlineData("[1, 2, 3]")]
			[InlineData("[3, 1, 2]")]
			[InlineData("[2, 3, 1]")]
			public async Task WhenElementsAreInAnyOrder_WhenIgnoringArrayOrder_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2, 3,], o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBothArraysAreEmpty_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("[]");

				async Task Act()
					=> await That(subject).MatchesExactly(Array.Empty<int>(), o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenElementsAreInDifferentOrder_WhenNotIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([2, 1,], o => o.IgnoringArrayOrder(false));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [2, 1,] exactly,
					             but it differed as
					               $[0] was 1 instead of 2 and
					               $[1] was 2 instead of 1
					             """);
			}

			[Fact]
			public async Task WhenDuplicateElementsAreInDifferentOrder_WhenIgnoringArrayOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([2, 1, 1,], o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedContainsDuplicateElement_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 1,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 1,] exactly,
					             but it differed as $[1] was 2 instead of 1

					             JSON options:
					              - ignore array order
					             """)
					.Because("each element of the subject can only be paired with one expected element");
			}

			[Fact]
			public async Task WhenSubjectContainsDuplicateElement_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[2, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2, 3,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2, 3,] exactly,
					             but it differed as $[1] was 2 instead of 3

					             JSON options:
					              - ignore array order
					             """)
					.Because("the single unpaired elements are compared with each other");
			}

			[Fact]
			public async Task WhenElementsAreMissingAndUnexpected_WhenIgnoringArrayOrder_ShouldListThem()
			{
				JsonElement titles = FromString("[\"Let It Be\", \"Yesterday\", \"Hey Jude\"]");

				async Task Act()
					=> await That(titles).MatchesExactly(["Hey Jude", "Something", "Help!",],
						o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenSingleElementDiffers_WhenIgnoringArrayOrder_ShouldCompareTheUnpairedElements()
			{
				JsonElement titles = FromString("[\"Let It Be\", \"Yesterday\", \"Hey Jude\"]");

				async Task Act()
					=> await That(titles).MatchesExactly(["Hey Jude", "Let It Be", "Something",],
						o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that titles
					             matches ["Hey Jude", "Let It Be", "Something",] exactly,
					             but it differed as $[1] was "Yesterday" instead of "Something"

					             JSON options:
					              - ignore array order
					             """)
					.Because("the only unpaired elements on both sides are compared with each other");
			}

			[Fact]
			public async Task WhenExpectedContainsAdditionalElements_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2, 3, 4,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[3, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,] exactly,
					             but it differed as $[0] had unexpected 3

					             JSON options:
					              - ignore array order
					             """);
			}

			[Theory]
			[InlineData("[]", "$ had missing 1")]
			[InlineData("[1]", null)]
			[InlineData("[2]", "$[0] was 2 instead of 1")]
			public async Task WhenExpectingSingleElement_WhenIgnoringArrayOrder_ShouldFailUnlessItMatches(
				string json, string? errorMessage)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly([1,], o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>().OnlyIf(errorMessage != null)
					.WithMessage($"""
					              Expected that subject
					              matches [1,] exactly,
					              but it differed as {errorMessage}

					              JSON options:
					               - ignore array order
					              """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).MatchesExactly(Array.Empty<int>(), o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches Array.Empty<int>() exactly,
					             but it differed as $[0] had unexpected 1

					             JSON options:
					              - ignore array order
					             """);
			}

			[Fact]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringArrayOrderAndAdditionalArrayElements_ShouldSucceed()
			{
				JsonElement subject = FromString("[4, 3, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 3,],
						o => o.IgnoringArrayOrder().IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow()
					.Because("the expected elements must only be a subset of the subject in any order");
			}

			[Fact]
			public async Task WhenSubjectContainsAdditionalElements_WhenIgnoringArrayOrderAndAdditionalProperties_ShouldFail()
			{
				JsonElement subject = FromString("[4, 3, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 3,],
						o => o.IgnoringArrayOrder().IgnoringAdditionalProperties());

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenElementIsMissing_WhenIgnoringArrayOrderAndAdditionalArrayElements_ShouldOnlyReportMissingElement()
			{
				JsonElement subject = FromString("[4, 3, 2, 1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 5,],
						o => o.IgnoringArrayOrder().IgnoringAdditionalArrayElements());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 5,],
					             but it differed as $ had missing 5

					             JSON options:
					              - ignore additional array elements
					              - ignore array order
					             """);
			}

			[Fact]
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

			[Fact]
			public async Task WhenNestedArrayDiffers_WhenIgnoringArrayOrder_ShouldReportTheNestedDifference()
			{
				JsonElement subject = FromString("[[1, 2], [3, 4]]");

				async Task Act()
					=> await That(subject).MatchesExactly(new[]
					{
						new[] { 4, 3, },
						new[] { 2, 5, },
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

			[Fact]
			public async Task WhenItIsOverlapsWithLaterElement_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly(new object[]
					{
						It.Is<int>().That.IsGreaterThan(0),
						1,
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenItIsDoesNotMatchRemainingElement_WhenIgnoringArrayOrder_ShouldOnlyReportItsFailure()
			{
				JsonElement subject = FromString("[\"foo\", \"bar\"]");

				async Task Act()
					=> await That(subject).MatchesExactly(new[]
					{
						It.Is<string>().That.StartsWith("b"),
						It.Is<string>().That.StartsWith("x"),
					}, o => o.IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
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

			public static TheoryData<string[], string> MatchingArrayValues
				=> new()
				{
					{
						[], "[]"
					},
					{
						[
							"foo", "bar",
						],
						"[\"foo\", \"bar\"]"
					},
				};

			public static TheoryData<string[], string, string> NotMatchingArrayValues
				=> new()
				{
					{
						[], "[\"foo\"]", "as $[0] had unexpected \"foo\""
					},
					{
						[], "[\"1\", \"2\"]", """
						                      as
						                        $[0] had unexpected "1" and
						                        $[1] had unexpected "2"
						                      """
					},
					{
						[
							"foo",
						],
						"[]", "as $[0] had missing \"foo\""
					},
					{
						[
							"bar", "foo",
						],
						"[\"foo\", \"bar\"]", """
						                      as
						                        $[0] was "foo" instead of "bar" and
						                        $[1] was "bar" instead of "foo"
						                      """
					},
				};
		}

		public sealed class ObjectTests
		{
			[Theory]
			[InlineData("{}", "$.foo was missing")]
			[InlineData("{\"foo\": 2}")]
			public async Task ShouldFailIfPropertyIsMissing(string json,
				string? errorMessage = null)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = 2,
					});

				await That(Act).Throws<XunitException>().OnlyIf(errorMessage != null)
					.WithMessage($$"""
					               Expected that subject
					               matches new
					               {
					               	foo = 2,
					               } exactly,
					               but it differed as {{errorMessage}}
					               """);
			}

			[Theory]
			[InlineData("{}", true)]
			[InlineData("{\"foo\": 1}", false)]
			public async Task WhenExpectedIsEmpty_ShouldSucceedWhenJsonIsEmptyObject(string json, bool isMatch)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).MatchesExactly(new object());

				await That(Act).Throws<XunitException>().OnlyIf(!isMatch)
					.WithMessage("""
					             Expected that subject
					             matches new object() exactly,
					             but it differed as $.foo had unexpected 1
					             """);
			}

			[Fact]
			public async Task WhenPropertyHasDifferentValue_ShouldFail()
			{
				JsonElement subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						bar = 3,
					});

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	bar = 3,
					             } exactly,
					             but it differed as $.bar was 2 instead of 3
					             """);
			}

			[Fact]
			public async Task WhenSubjectHasAdditionalProperties_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null, \"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						bar = 2,
					});

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	bar = 2,
					             } exactly,
					             but it differed as $.foo had unexpected <null>
					             """);
			}

			[Fact]
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

			[Fact]
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

			[Fact]
			public async Task WhenSubjectHasAdditionalNonNullProperty_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						bar = 2,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenExpectedNullPropertyIsMissing_ShouldFail()
			{
				JsonElement subject = FromString("{\"bar\": 2}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = (string?)null,
						bar = 2,
					});

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = (string?)null,
					             	bar = 2,
					             } exactly,
					             but it differed as $.foo was missing
					             """);
			}

			[Fact]
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

			[Fact]
			public async Task WhenExpectedNullPropertyHasValue_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = (int?)null,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenPropertyIsNullInsteadOfValue_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = 2,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

			[Fact]
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
			[Fact]
			public async Task WhenGeneratedValuesDiffer_ShouldSucceedAsDocumentedInTheReadme()
			{
				JsonElement track = FromString(
					"""{"id": 42, "title": "Let It Be", "meta": {"createdAt": "2026-10-16", "etag": "abc"}}""");

				async Task Act()
					=> await That(track).MatchesExactly(new { id = 1, title = "Let It Be", meta = new { createdAt = "2026-01-01" } },
						o => o.IgnoringMember("id").IgnoringMember("createdAt").Ignoring(path => path.EndsWith(".etag")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNameIsMisspelled_ShouldFailAsDocumentedInTheReadme()
			{
				JsonElement track = FromString("""{"title": "Let It Be", "createdAt": "2026-10-16"}""");

				async Task Act()
					=> await That(track).MatchesExactly(new { title = "Yesterday", createdAt = "2026-01-01" },
						o => o.IgnoringMember("craetedAt"));

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

			[Fact]
			public async Task WhenOtherPropertyDiffers_ShouldStillReportIt()
			{
				JsonElement subject = FromString("{\"id\": 1, \"name\": \"a\"}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						id = 2,
						name = "b",
					}, o => o.IgnoringMember("id"));

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenNameMatchesOnlyPartOfASegment_ShouldNotIgnoreIt()
			{
				JsonElement subject = FromString("{\"parentId\": 1}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						parentId = 2,
					}, o => o.IgnoringMember("id"));

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenNameDiffersInCasing_ShouldIgnoreTheProperty()
			{
				JsonElement subject = FromString("{\"Id\": 1}");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						Id = 2,
					}, o => o.IgnoringMember("id"));

				await That(Act).DoesNotThrow()
					.Because("names are matched case-insensitively, like in aweXpect");
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

			[Fact]
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

			[Fact]
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

			[Fact]
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

			[Fact]
			public async Task WhenIgnoredElementIsMissing_ShouldNotReportIt()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringMember("[1]"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenIgnoredElementIsUnexpected_ShouldNotReportIt()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).MatchesExactly([1,], o => o.IgnoringMember("[1]"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

			[Fact]
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

			[Fact]
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
					}, o => o.Ignoring(path => path.StartsWith("$.items[") && path.EndsWith(".etag")));

				await That(Act).Throws<XunitException>()
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

			[Fact]
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
						.Ignoring(path => path == "$.etag"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

			[Fact]
			public async Task WhenIndexIsIgnored_WhenIgnoringArrayOrder_ShouldLeaveTheElementOfTheSubjectOutOfThePairing()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringMember("[0]").IgnoringArrayOrder());

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenIndexIsIgnored_WhenIgnoringArrayOrder_ShouldNotReportTheElementAsUnexpected()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).MatchesExactly([3, 2,], o => o.IgnoringMember("[0]").IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
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
	}
}
