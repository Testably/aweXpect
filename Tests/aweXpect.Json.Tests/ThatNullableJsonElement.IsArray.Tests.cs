using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class IsArray
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("[]")]
			[Arguments("[1, 2]")]
			public async Task WhenJsonIsAnArray_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoArray_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an array,
					              but it was {kindString}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement?[] items = [FromString("[]"), FromString("{}"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsArray()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are arrays for all items,
					             but only 1 of 2 were

					             Not matching items (Items):
					             [
					               {}
					             ]

					             Collection (Items):
					             [
					               [],
					               {}
					             ]
					             """)
					.Because("the items are the plural subject of the expectation");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task IsArray_ShouldBeChainable()
			{
				string json = "[1, 2, 3]";
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsArray(o => o.At(0).Matching(1)).And.IsArray(o => o.At(2).Matching(3)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array or $[0] does not match 1 or is not an array or $[2] does not match 3,
					             but it was

					             Actual:
					             [
					               1,
					               2,
					               3
					             ]

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("[]")]
			[Arguments("[1, 2]")]
			public async Task WhenJsonIsAnArray_ShouldFail(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array,
					             but it was
					             """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("2")]
			[Arguments("\"foo\"")]
			public async Task WhenJsonIsNoArray_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithExpectationTests
		{
			[Test]
			public async Task WhenExpectationIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("[]");

				async Task Act()
					=> await That(subject).IsArray(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectation").And
					.WithMessage("The 'expectation' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectationThrows_ShouldFail()
			{
				JsonElement? subject = FromString("[1]");

				async Task Act()
					=> await That(subject).IsArray(_ => throw new InvalidOperationException("Yesterday"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array,
					             but the expectation did throw an InvalidOperationException:
					               Yesterday

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenMatchingItIsWaits_ShouldNotBlockTheCaller()
			{
				JsonElement? subject = FromString("[\"bar\"]");
				bool isSatisfied = false;

				async Task Act()
					=> await That(subject).IsArray(a => a.At(0)
						.Matching(It.Is<string>().That.Satisfies(_ => isSatisfied).Within(TimeSpan.FromSeconds(5))));

				Task evaluation = Act();
				isSatisfied = true;

				await That(() => evaluation).DoesNotThrow()
					.Because("the evaluation must return to the caller while it waits instead of blocking it");
			}

			[Test]
			[Arguments("{\"foo\":{}}", "an object")]
			[Arguments("{\"foo\":2}", "a number")]
			[Arguments("{\"foo\":\"foo\"}", "a string")]
			public async Task WhenInnerJsonIsNoArray_AndExpectElementAt_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnArray(a => a.At(0).Matching(null)));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is an array and $.foo[0] matches null,
					              but it differed as $.foo was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("{\"foo\":{}}", "an object")]
			[Arguments("{\"foo\":2}", "a number")]
			[Arguments("{\"foo\":\"foo\"}", "a string")]
			public async Task WhenInnerJsonIsNoArray_AndExpectElementCount_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnArray(a => a.With(0).Elements()));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is an array with 0 elements,
					              but it differed as $.foo was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("{\"foo\":{}}", "an object")]
			[Arguments("{\"foo\":2}", "a number")]
			[Arguments("{\"foo\":\"foo\"}", "a string")]
			public async Task WhenInnerJsonIsNoArray_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnArray());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is an array,
					              but it differed as $.foo was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenJsonIsNoArray_WithExpectations_ShouldConsiderIgnoreAdditionalProperties(
				bool ignoreAdditionalProperties)
			{
				string json = "[{\"foo\": 1, \"bar\": 2}, {\"foo\": 2, \"bar\": 4}]";
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o.At(0).Matching(new
					{
						foo = 1,
					}), o => o with
					{
						IgnoreAdditionalProperties = ignoreAdditionalProperties,
					});

				await That(Act).Throws<FailException>().OnlyIf(!ignoreAdditionalProperties)
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches new
					             {
					             	foo = 1,
					             },
					             but it differed as $[0].bar had unexpected 2
					             """);
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoArray_WithExpectations_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o.At(0).Matching(true));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an array and $[0] matches true,
					              but it was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_WithExpectations_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsArray(o => o.At(0).Matching(true));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches true,
					             but it was <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}

		public sealed class WhoseElementsTests
		{
			[Test]
			public async Task AtLeast_WhenEnoughElementsComply_ShouldSucceed()
			{
				JsonElement? subject = FromString("[{\"title\": \"Hey Jude\"}, \"Yesterday\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.AtLeast(1).ComplyWith(t => t.IsObject());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewElementsComply_ShouldFail()
			{
				JsonElement? subject = FromString("[{\"title\": \"Hey Jude\"}, \"Yesterday\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.AtLeast(2).ComplyWith(t => t.IsObject());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements are objects for at least 2 items,
					             but only 1 of 2 were

					             Not matching items (elements):
					             [
					               "Yesterday"
					             ]

					             Collection (elements):
					             [
					               {"title":"Hey Jude"},
					               "Yesterday"
					             ]
					             """);
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoArray_ShouldOnlyFailWithIsArray(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an array whose elements have exactly 2 items,
					              but it was {kindString}
					              """)
					.Because("the elements are only accessed when the subject is an array");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldOnlyFailWithIsArray()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements have exactly 2 items,
					             but it was <null>
					             """)
					.Because("the elements are only accessed when the subject is an array");
			}

			[Test]
			public async Task WithExpectation_WhenElementsDoNotComply_ShouldFail()
			{
				JsonElement? subject = FromString("[2, 1]");

				async Task Act()
					=> await That(subject).IsArray(a => a.With(2).Elements(), o => o.IgnoringArrayOrder())
						.WhoseElements.Contains(e => e.GetInt32() == 3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array with 2 elements whose elements contain an item matching e => e.GetInt32() == 3 at least once,
					             but elements did not contain it

					             Collection (elements):
					             [
					               2,
					               1
					             ]
					             """);
			}

			[Test]
			public async Task WithExpectation_WhenSubjectIsNull_ShouldOnlyFailWithIsArray()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsArray(a => a.With(2).Elements())
						.WhoseElements.Contains(e => e.GetInt32() == 3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array with 2 elements whose elements contain an item matching e => e.GetInt32() == 3 at least once,
					             but it was <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}

		public sealed class WhoseElementsNegatedTests
		{
			[Test]
			public async Task WhenElementsMeetTheExpectation_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray().WhoseElements.HasCount(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array whose elements have exactly 2 items,
					             but it was
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray().WhoseElements.HasCount(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array whose elements have exactly 2 items,
					             but it was <null>
					             """)
					.Because("a null subject fails IsArray also when it is negated");
			}
		}

		public sealed class WithNumberOfElementsTests
		{
			[Test]
			public async Task WhenNull_ShouldNotAddFailureMessage()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnArray(a => a
							.With(1).Elements()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an array with 1 element,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenNumberDiffers_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).IsArray(o => o
						.With(3).Elements());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array with 3 elements,
					             but it differed as $ had 2 elements

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("[]", 0)]
			[Arguments("[\"foo\"]", 1)]
			[Arguments("[1, 2, 3]", 3)]
			public async Task WhenNumberMatches_ShouldSucceed(string json, int expected)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o
						.With(expected).Elements());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithArraysTests
		{
			[Test]
			public async Task WhenArrayIsNull_ShouldNotAddFailureMessage()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnArray(a => a
							.WithArrays(p => p.With(0).Elements())));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an array and $.foo[0] is an array with 0 elements,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationIsNull_ShouldSkipIndex()
			{
				JsonElement? subject = FromString("[[1],[2],[3]]");

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithArrays(o => o.At(0).Matching(1), null, o => o.At(0).Matching(2)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an array and $[0][0] matches 1 and $[2] is an array and $[2][0] matches 2,
					             but it differed as $[2][0] was 3 instead of 2

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationsMatchInDifferentOrder_ShouldFail()
			{
				JsonElement? subject = FromString("""
				                                  [
				                                    [1],
				                                    [3],
				                                    [2],
				                                    [4]
				                                  ]
				                                  """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithArrays(
							o => o.At(0).Matching(1),
							o => o.At(0).Matching(2),
							o => o.At(0).Matching(3)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an array and $[0][0] matches 1 and $[1] is an array and $[1][0] matches 2 and $[2] is an array and $[2][0] matches 3,
					             but it differed as
					               $[1][0] was 3 instead of 2 and
					               $[2][0] was 2 instead of 3

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationsMatchInOrder_ShouldSucceed()
			{
				JsonElement? subject = FromString("""
				                                  [
				                                    [1],
				                                    [2],
				                                    [3],
				                                    [4]
				                                  ]
				                                  """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithArrays(
							o => o.At(0).Matching(1),
							o => o.At(0).Matching(2),
							o => o.At(0).Matching(3)));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithElementsTests
		{
			[Test]
			public async Task WhenArrayIsNull_ShouldNotAddFailureMessage()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnArray(a => a
							.WithElements(2, 3)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an array and $.foo[0] matches 2 and $.foo[1] matches 3,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationsMatchInDifferentOrder_ShouldFail()
			{
				JsonElement? subject = FromString("""
				                                  [
				                                    3, null, true, 1.2, false, "bar"
				                                  ]
				                                  """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithElements(3, true, null, 1.2, false, "bar"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches 3 and $[1] matches True and $[2] matches <null> and $[3] matches 1.2 and $[4] matches False and $[5] matches "bar",
					             but it differed as
					               $[1] was <null> instead of True and
					               $[2] was boolean True instead of <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationsMatchInOrder_ShouldSucceed()
			{
				JsonElement? subject = FromString("""
				                                  [
				                                    3, true, null, 1.2, false, "bar"
				                                  ]
				                                  """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithElements(3, true, null, 1.2, false, "bar"));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithObjectsTests
		{
			[Test]
			public async Task WhenArrayIsNull_ShouldNotAddFailureMessage()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnArray(a => a
							.WithObjects(p => p.With(0).Properties())));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an array and $.foo[0] is an object with 0 properties,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationIsNull_ShouldSkipIndex()
			{
				JsonElement? subject = FromString("[{},{},{\"foo\":true}]");

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithObjects(o => o.With(0).Properties(), null, o => o.With(0).Properties()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an object with 0 properties and $[2] is an object with 0 properties,
					             but it differed as $[2] had 1 property

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationsMatchInDifferentOrder_ShouldFail()
			{
				JsonElement? subject = FromString("""
				                                  [
				                                    {"foo":1},
				                                    {"foo":3},
				                                    {"bar":2},
				                                    {"baz":4}
				                                  ]
				                                  """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithObjects(
							o => o.With("foo").Matching(1),
							o => o.With("bar").Matching(2),
							o => o.With("foo").Matching(3)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an object and $[0].foo matches 1 and $[1] is an object and $[1].bar matches 2 and $[2] is an object and $[2].foo matches 3,
					             but it differed as
					               property $[1].bar did not exist and
					               property $[2].foo did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationsMatchInOrder_ShouldSucceed()
			{
				JsonElement? subject = FromString("""
				                                  [
				                                    {"foo":1},
				                                    {"bar":2},
				                                    {"foo":3},
				                                    {"baz":4}
				                                  ]
				                                  """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithObjects(
							o => o.With("foo").Matching(1),
							o => o.With("bar").Matching(2),
							o => o.With("foo").Matching(3)));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NumbersWithinTests
		{
			[Test]
			public async Task WhenMatchingNumberIsWithinTheTolerance_ShouldSucceed()
			{
				JsonElement? subject = FromString("[1, 104]");

				async Task Act()
					=> await That(subject).IsArray(a => a.At(1).Matching(100), o => o.NumbersWithin(5).Percent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMatchingNumberIsOutsideTheTolerance_ShouldFail()
			{
				JsonElement? subject = FromString("[1, 106]");

				async Task Act()
					=> await That(subject).IsArray(a => a.At(1).Matching(100), o => o.NumbersWithin(5).Percent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[1] matches 100,
					             but it differed as $[1] was 106 instead of 100 ± 5%

					             JSON options:
					              - ignore additional properties
					              - compare numbers within ± 5%
					             """);
			}
		}
	}
}
