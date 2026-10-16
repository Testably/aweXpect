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
	}
}
