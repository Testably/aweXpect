using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement[] items = [FromString("[]"), FromString("{}"),];

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

			[Test]
			public async Task WhenItemsArePlural_WithExpectation_ShouldUsePluralFormOnlyForTheItems()
			{
				JsonElement[] items = [FromString("[[1]]"), FromString("[{}]"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsArray(a => a.WithArrays(_ => { }))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are arrays and $[0] is an array for all items,
					             but only 1 of 2 were

					             Not matching items (Items):
					             [
					               [{}]
					             ]

					             Collection (Items):
					             [
					               [[1]],
					               [{}]
					             ]

					             JSON options (Items[1]):
					              - ignore additional properties
					             """)
					.Because("the path of a nested element is a singular subject");
			}

			[Test]
			public async Task WhenMemberIsPlural_WithExpectation_ShouldUsePluralFormInResult()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await WhosePluralItems(That(subject), items => items.IsArray(a => a.At(0).Matching(1)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Items are arrays and $[0] matches 1,
					             but Items were an object

					             JSON options (Items):
					              - ignore additional properties
					             """)
					.Because("the plural member is the subject of the result");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement[] items = [FromString("[]"), FromString("{}"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.DoesNotComplyWith(it => it.IsArray())));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are not arrays for all items,
					             but only 1 of 2 were

					             Not matching items (Items):
					             [
					               []
					             ]

					             Collection (Items):
					             [
					               [],
					               {}
					             ]
					             """)
					.Because("the items are the plural subject of the expectation");
			}

			[Test]
			public async Task WhenElementIsExpectedToBeAnObjectWithoutExpectation_ShouldNegateIt()
			{
				JsonElement subject = FromString("[{}]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray(a => a.At(0).AnObject()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array or $[0] is not an object,
					             but it was

					             Actual:
					             [
					               {}
					             ]

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenElementIsExpectedToHaveAValueKind_ShouldNegateIt()
			{
				JsonElement subject = FromString("[\"foo\"]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray(a => a.At(0).AString()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array or $[0] is not a string,
					             but it was

					             Actual:
					             [
					               "foo"
					             ]

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task IsArray_ShouldBeChainable()
			{
				string json = "[1, 2, 3]";
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
			public async Task WhenExpectationThrows_ShouldFail()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsArray(_ => throw new InvalidOperationException("Yesterday")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array,
					             but the expectation did throw an InvalidOperationException:
					               Yesterday

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("an expectation that throws answered nothing, so its negation must not be met either");
			}

			[Test]
			[Arguments("{}")]
			[Arguments("2")]
			[Arguments("\"foo\"")]
			public async Task WhenJsonIsNoArray_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ExpectationTests
		{
			[Test]
			public async Task IsArray_ShouldBeChainable()
			{
				string json = "[1, 2, 3]";
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o.At(0).Matching(1)).And.IsArray(o => o.At(2).Matching(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches 1 and is an array and $[2] matches 2,
					             but it differed as $[2] was 3 instead of 2

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task IsArray_ShouldSupportExpectationsOnElements()
			{
				string json = "[[], [1], {}, {\"foo\": {}}]";
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(elements => elements
						.At(0).AnArray().And
						.At(1).AnArray(a => a.With(1).Elements()).And
						.At(2).AnObject().And
						.At(3).AnObject(o => o.With("foo").AnObject()));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IsArray_ShouldSupportValueKindExpectationsOnElements()
			{
				JsonElement subject = FromString("[\"foo\", 1.5, true, false, null]");

				async Task Act()
					=> await That(subject).IsArray(elements => elements
						.At(0).AString().And
						.At(1).ANumber().And
						.At(2).ABoolean().And
						.At(3).ABoolean().And
						.At(4).Null());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenElementsHaveOtherValueKinds_ShouldListAllFailuresInOrder()
			{
				JsonElement subject = FromString("[1, \"1\", null, [], {}]");

				async Task Act()
					=> await That(subject).IsArray(elements => elements
						.At(0).AString().And
						.At(1).ANumber().And
						.At(2).ABoolean().And
						.At(3).Null().And
						.At(5).AString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is a string and $[1] is a number and $[2] is a boolean and $[3] is null and $[5] is a string,
					             but it differed as
					               $[0] was a number and
					               $[1] was a string and
					               $[2] was null and
					               $[3] was an array and
					               index $[5] did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenElementCannotBeSerialized_ShouldThrowNotSupportedException()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).IsArray(a => a.WithElements(typeof(int)));

				await That(Act).Throws<NotSupportedException>()
					.Because("an expected value that cannot be serialized is thrown as it is instead of failing the expectation");
			}

			[Test]
			public async Task WhenElementExpectationWaits_ShouldNotBlockTheCaller()
			{
				JsonElement subject = FromString("[\"bar\"]");
				bool isSatisfied = false;

				async Task Act()
					=> await That(subject).IsArray(a => a.WithElements(
						It.Is<string>().That.Satisfies(_ => isSatisfied).Within(TimeSpan.FromSeconds(5))));

				Task evaluation = Act();
				isSatisfied = true;

				await That(() => evaluation).DoesNotThrow()
					.Because("the evaluation must return to the caller while it waits instead of blocking it");
			}

			[Test]
			public async Task WhenExpectationIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("[]");

				async Task Act()
					=> await That(subject).IsArray(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectation").And
					.WithMessage("The 'expectation' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectationThrows_ShouldFail()
			{
				JsonElement subject = FromString("[1]");

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
			public async Task WhenIgnoringArrayOrder_ShouldStillVerifyTheElementAtTheIndex()
			{
				JsonElement subject = FromString("[[1, 2], 3]");

				async Task Act()
					=> await That(subject).IsArray(a => a
							.At(0).Matching(new[] { 2, 1, }).And
							.At(1).Matching(new[] { 2, 1, }),
						o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches new[] { 2, 1, } and $[1] matches new[] { 2, 1, },
					             but it differed as $[1] was number 3 instead of [2,1]

					             JSON options:
					              - ignore additional properties
					              - ignore array order
					             """)
					.Because("the option only ignores the order within the matched value, not the index of the element");
			}

			[Test]
			public async Task WhenIndexIsIgnored_ShouldStillVerifyTheElementAtTheIndex()
			{
				JsonElement subject = FromString("[{\"id\": 1, \"name\": \"a\"}]");

				async Task Act()
					=> await That(subject).IsArray(a => a
							.At(0).Matching(new
							{
								id = 2,
								name = "b",
							}),
						o => o.IgnoringMember("[0]").IgnoringMember("id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches new
					             {
					             	id = 2,
					             	name = "b",
					             },
					             but it differed as $[0].name was "a" instead of "b"

					             JSON options:
					              - ignore additional properties
					              - ignore members: ["[0]", "id"]
					                "[0]" matched no member
					             """)
					.Because("an explicit expectation wins over an ignored member, so \"[0]\" ignored nothing, but the members below it are still ignored");
			}

			[Test]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).IsArray(a => a.At(-1).Matching(1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("index").And
					.WithMessage("The index must not be negative.*").AsWildcard()
					.Because("an invalid argument is thrown as it is instead of failing the expectation");
			}

			[Test]
			public async Task WhenIndexDoesNotExist_ShouldFail()
			{
				string json = "[1, 2, 3]";
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o.At(3).Matching(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[3] matches 3,
					             but it differed as index $[3] did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("{\"foo\":{}}", "an object")]
			[Arguments("{\"foo\":2}", "a number")]
			[Arguments("{\"foo\":\"foo\"}", "a string")]
			[Arguments("{\"foo\":true}", "true")]
			[Arguments("{\"foo\":false}", "false")]
			[Arguments("{\"foo\":null}", "null")]
			public async Task WhenInnerJsonIsNoArray_AndExpectElementAt_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
			[Arguments("{}", "an object")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoArray_WithExpectations_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o.At(0).AnArray());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an array and $[0] is an array,
					              but it was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			public async Task WhenJsonIsNoArray_WithElements_ShouldFail()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsArray(a => a.WithElements(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches 1,
					             but it was an object

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenNestedExpectationThrows_ShouldFail()
			{
				JsonElement subject = FromString("[[1]]");

				async Task Act()
					=> await That(subject).IsArray(a => a
						.At(0).AnArray(_ => throw new InvalidOperationException("Yesterday")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an array,
					             but the expectation did throw an InvalidOperationException:
					               Yesterday

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WithExpectations_ShouldConsiderIgnoreAdditionalProperties(
				bool ignoreAdditionalProperties)
			{
				string json = "[{\"foo\": 1, \"bar\": 2}, {\"foo\": 2, \"bar\": 4}]";
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o.At(0).Matching(new
					{
						foo = 1,
					}), o => o.IgnoringAdditionalProperties(ignoreAdditionalProperties));

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
			[Arguments(true)]
			[Arguments(false)]
			public async Task WithExpectations_ShouldConsiderIgnoreAdditionalArrayElements(
				bool ignoreAdditionalArrayElements)
			{
				string json = "[[1, 2, 3], [4]]";
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o.At(0).Matching(new[] { 1, 2, }),
						o => o.IgnoringAdditionalArrayElements(ignoreAdditionalArrayElements));

				await That(Act).Throws<FailException>().OnlyIf(!ignoreAdditionalArrayElements)
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches new[] { 1, 2, },
					             but it differed as $[0][2] had unexpected 3

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("the positional members do not verify the length of the array, but the matched value does");
			}

			[Test]
			public async Task WithElements_ShouldNotVerifyAdditionalElements()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).IsArray(o => o.WithElements(1, 2));

				await That(Act).DoesNotThrow()
					.Because("the positional members only verify the elements at the given indices");
			}
		}

		public sealed class WhoseElementsTests
		{
			[Test]
			public async Task All_WhenAllElementsComply_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   {"artist": "The Beatles", "title": "Hey Jude"},
				                                   {"artist": "The Beatles", "title": "Yesterday"}
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements
						.All().ComplyWith(t => t.Matches(new { artist = "The Beatles", }));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task All_WhenSomeElementsDoNotComply_ShouldFail()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   {"artist": "The Beatles", "title": "Hey Jude"},
				                                   {"artist": "Queen", "title": "Bohemian Rhapsody"}
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements
						.All().ComplyWith(t => t.Matches(new { artist = "The Beatles", }));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements match new { artist = "The Beatles", } for all items,
					             but only 1 of 2 did

					             Not matching items (elements):
					             [
					               {"artist":"Queen","title":"Bohemian Rhapsody"}
					             ]

					             Collection (elements):
					             [
					               {"artist":"The Beatles","title":"Hey Jude"},
					               {"artist":"Queen","title":"Bohemian Rhapsody"}
					             ]

					             JSON options (elements[1]):
					              - ignore additional properties
					             """)
					.Because("the options belong to the expectation on the first element that does not comply");
			}

			[Test]
			public async Task All_WhenElementsAreNoStrings_ShouldFormatThemAsJson()
			{
				JsonElement subject = FromString("""
				                                 ["foo", 1.5, true, false, null, {"a": [1, {"b": "c"}]}, []]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.All().Satisfy(e => e.ValueKind == JsonValueKind.String);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements satisfy e => e.ValueKind == JsonValueKind.String for all items,
					             but only 1 of 7 did

					             Not matching items (elements):
					             [
					               1.5,
					               true,
					               false,
					               null,
					               {"a":[1,{"b":"c"}]},
					               []
					             ]

					             Collection (elements):
					             [
					               "foo",
					               1.5,
					               true,
					               false,
					               null,
					               {"a":[1,{"b":"c"}]},
					               []
					             ]
					             """)
					.Because("the elements are formatted as JSON instead of with their ToString()");
			}

			[Test]
			public async Task AtLeast_WhenEnoughElementsComply_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [{"durationSeconds": 431}, {"durationSeconds": 125}, {}]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements
						.AtLeast(2).ComplyWith(t => t.IsObject(o => o.With("durationSeconds").ANumber()));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewElementsComply_ShouldFail()
			{
				JsonElement subject = FromString("""
				                                 [{"durationSeconds": 431}, {"durationSeconds": "2:05"}, {}]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements
						.AtLeast(2).ComplyWith(t => t.IsObject(o => o.With("durationSeconds").ANumber()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements are objects and $.durationSeconds is a number for at least 2 items,
					             but only 1 of 3 were

					             Not matching items (elements):
					             [
					               {"durationSeconds":"2:05"},
					               {}
					             ]

					             Collection (elements):
					             [
					               {"durationSeconds":431},
					               {"durationSeconds":"2:05"},
					               {}
					             ]

					             JSON options (elements[1]):
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task AtMost_WhenFewEnoughElementsComply_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, \"2\", \"3\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.AtMost(1).ComplyWith(e => e.IsNumber());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtMost_WhenTooManyElementsComply_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, \"3\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.AtMost(1).ComplyWith(e => e.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements are numbers for at most one item,
					             but 2 of 3 were

					             Matching items (elements):
					             [
					               1,
					               2
					             ]

					             Collection (elements):
					             [
					               1,
					               2,
					               "3"
					             ]
					             """);
			}

			[Test]
			public async Task Between_WhenNumberOfComplyingElementsIsInRange_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2, \"3\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.Between(1).And(2).ComplyWith(e => e.IsNumber());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Between_WhenNumberOfComplyingElementsIsOutOfRange_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.Between(1).And(2).ComplyWith(e => e.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements are numbers for between 1 and 2 items,
					             but 3 of 3 were

					             Collection (elements):
					             [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Test]
			public async Task Contains_WithExpectations_WhenAnElementComplies_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [{"title": "Yesterday"}, {"title": "Hey Jude"}]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements
						.Contains([t => t.Matches(new { title = "Hey Jude", })]);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Contains_WithExpectations_WhenNoElementComplies_ShouldFail()
			{
				JsonElement subject = FromString("""
				                                 [{"title": "Yesterday"}, {"title": "Let It Be"}]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements
						.Contains([t => t.Matches(new { title = "Hey Jude", })]);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements contain collection [t => t.Matches(new { title = "Hey Jude", })] in order and contiguous,
					             but elements lacked the one expected item

					             Collection (elements):
					             [
					               {"title":"Yesterday"},
					               {"title":"Let It Be"}
					             ]

					             Expected (elements):
					             [
					               an item that matches new { title = "Hey Jude", }
					             ]
					             """);
			}

			[Test]
			public async Task Contains_WithPredicate_WhenAnElementMatches_ShouldSucceed()
			{
				JsonElement subject = FromString("[\"pop\", \"rock\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.Contains(e => e.GetString() == "rock");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Contains_WithPredicate_WhenNoElementMatches_ShouldFail()
			{
				JsonElement subject = FromString("[\"pop\", \"jazz\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.Contains(e => e.GetString() == "rock");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements contain an item matching e => e.GetString() == "rock" at least once,
					             but elements did not contain it

					             Collection (elements):
					             [
					               "pop",
					               "jazz"
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenNumberOfComplyingElementsDiffers_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, 2]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.Exactly(1).ComplyWith(e => e.Matches(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements match 2 for exactly one item,
					             but 2 of 3 did

					             Collection (elements):
					             [
					               1,
					               2,
					               2
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenNumberOfComplyingElementsMatches_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.Exactly(1).ComplyWith(e => e.Matches(2));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HasCount_WhenCountDiffers_ShouldFail()
			{
				JsonElement subject = FromString("[\"pop\", \"rock\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.HasCount(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements have exactly 3 items,
					             but elements had only 2 items

					             Collection (elements):
					             [
					               "pop",
					               "rock"
					             ]
					             """);
			}

			[Test]
			public async Task HasCount_WhenCountMatches_ShouldSucceed()
			{
				JsonElement subject = FromString("[\"pop\", \"rock\", \"jazz\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.HasCount(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IsEmpty_WhenArrayHasElements_ShouldFail()
			{
				JsonElement subject = FromString("[\"pop\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements are empty,
					             but elements were [
					               "pop"
					             ]
					             """);
			}

			[Test]
			public async Task IsEmpty_WhenArrayIsEmpty_ShouldSucceed()
			{
				JsonElement subject = FromString("[]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.IsEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnElementComplies_ShouldFail()
			{
				JsonElement subject = FromString("""
				                                 [{"explicit": false}, {"explicit": true}]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements
						.None().ComplyWith(t => t.IsObject(o => o.With("explicit").Matching(true)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements are objects and $.explicit matches true for no items,
					             but 1 of 2 were

					             Matching items (elements):
					             [
					               {"explicit":true}
					             ]

					             Collection (elements):
					             [
					               {"explicit":false},
					               {"explicit":true}
					             ]

					             JSON options (elements[1]):
					              - ignore additional properties
					             """)
					.Because("the options belong to the expectation on the first element that complies");
			}

			[Test]
			public async Task None_WhenNoElementComplies_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [{"explicit": false}, {}]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements
						.None().ComplyWith(t => t.IsObject(o => o.With("explicit").Matching(true)));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCombinedWithAnd_ShouldVerifyBothOnTheElements()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.HasCount(3).And.All().ComplyWith(e => e.IsString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements have exactly 3 items and are strings for all items,
					             but none of 3 were

					             Not matching items (elements):
					             [
					               1,
					               2,
					               3
					             ]

					             Collection (elements):
					             [
					               1,
					               2,
					               3
					             ]
					             """)
					.Because("the chain continues on the elements after WhoseElements");
			}

			[Test]
			public async Task WhenCombinedWithOr_ShouldSucceedWhenEitherIsMet()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.HasCount(2).Or.HasCount(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFollowingAnAlternative_ShouldOnlyApplyToTheArray()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).IsObject().Or.IsArray().WhoseElements.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object or is an array whose elements have exactly 2 items,
					             but it was an array and elements had 3 items

					             Collection (elements):
					             [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			[Arguments("null", "null")]
			public async Task WhenJsonIsNoArray_ShouldOnlyFailWithIsArray(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.All().ComplyWith(e => e.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an array whose elements are numbers for all items,
					              but it was {kindString}
					              """)
					.Because("the elements are only accessed when the subject is an array");
			}

			[Test]
			public async Task WithExpectation_WhenElementsDoNotComply_ShouldFailWithoutJsonOptions()
			{
				JsonElement subject = FromString("[2, 1]");

				async Task Act()
					=> await That(subject).IsArray(a => a.With(2).Elements(), o => o.IgnoringArrayOrder())
						.WhoseElements.All().Satisfy(e => e.GetInt32() > 1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array with 2 elements whose elements satisfy e => e.GetInt32() > 1 for all items,
					             but only 1 of 2 did

					             Not matching items (elements):
					             [
					               1
					             ]

					             Collection (elements):
					             [
					               2,
					               1
					             ]
					             """)
					.Because("the JSON options only belong to the met expectation on the array");
			}

			[Test]
			public async Task WithExpectation_WhenExpectationIsNotMet_ShouldNotVerifyTheElements()
			{
				JsonElement subject = FromString("[2, 1]");

				async Task Act()
					=> await That(subject).IsArray(a => a.With(3).Elements(), o => o.IgnoringArrayOrder())
						.WhoseElements.All().Satisfy(e => e.GetInt32() > 1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array with 3 elements whose elements satisfy e => e.GetInt32() > 1 for all items,
					             but it differed as $ had 2 elements

					             JSON options:
					              - ignore additional properties
					              - ignore array order
					             """);
			}

			[Test]
			public async Task WithExpectation_WhenExpectationAndElementsAreMet_ShouldSucceed()
			{
				JsonElement subject = FromString("[2, 3]");

				async Task Act()
					=> await That(subject).IsArray(a => a.With(2).Elements(), o => o.IgnoringArrayOrder())
						.WhoseElements.All().Satisfy(e => e.GetInt32() > 1);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WhoseElementsNegatedTests
		{
			[Test]
			public async Task WhenSubjectIsNoArray_ShouldSucceed()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray().WhoseElements.HasCount(3));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenElementsDoNotMeetTheExpectation_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray().WhoseElements.HasCount(3));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenElementsMeetTheExpectation_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray().WhoseElements.All().ComplyWith(e => e.IsNumber()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array whose elements are numbers for all items,
					             but it was
					             """);
			}

			[Test]
			public async Task WhenElementsAreExpectedNotToComply_ShouldFailForComplyingElements()
			{
				JsonElement subject = FromString("[1, 2, 3]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements
						.All().ComplyWith(e => e.DoesNotComplyWith(x => x.Matches(2)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements do not match 2 for all items,
					             but only 2 of 3 did

					             Not matching items (elements):
					             [
					               2
					             ]

					             Collection (elements):
					             [
					               1,
					               2,
					               3
					             ]

					             Actual (elements[1]):
					             2

					             JSON options (elements[1]):
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenElementsAreExpectedNotToContainAnItem_ShouldFailWhenTheyDo()
			{
				JsonElement subject = FromString("[\"pop\", \"rock\"]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.DoesNotContain(e => e.GetString() == "rock");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements do not contain an item matching e => e.GetString() == "rock",
					             but elements contained it at least once

					             Collection (elements):
					             [
					               "pop",
					               "rock"
					             ]
					             """);
			}
		}

		public sealed class WithNumberOfElementsTests
		{
			[Test]
			public async Task WhenNull_ShouldNotAddFailureMessage()
			{
				JsonElement subject = FromString("{}");

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
				JsonElement subject = FromString("[1, 2]");

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
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray(o => o
						.With(expected).Elements());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenElementsAreNull_WhenTreatingNullAsAbsent_ShouldCountThem()
			{
				JsonElement subject = FromString("[1, null]");

				async Task Act()
					=> await That(subject).IsArray(e => e
							.With(2).Elements(),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow()
					.Because("null elements of an array are not affected by the option");
			}
		}

		public sealed class WithArraysTests
		{
			[Test]
			public async Task WhenArrayIsNull_ShouldNotAddFailureMessage()
			{
				JsonElement subject = FromString("{}");

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
			public async Task WhenElementsArNoArrays_ShouldFail()
			{
				JsonElement subject = FromString("[{},{}]");

				async Task Act()
					=> await That(subject).IsArray(a => a
						.WithArrays(p => p.With(2).Elements()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an array with 2 elements,
					             but it differed as $[0] was an object

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationIsNull_ShouldSkipIndex()
			{
				JsonElement subject = FromString("[[1],[2],[3]]");

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
				JsonElement subject = FromString("""
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
				JsonElement subject = FromString("""
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
				JsonElement subject = FromString("{}");

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
				JsonElement subject = FromString("""
				                                 [
				                                   3, null, false, 1.2, false, "bar"
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithElements(3, false, null, 1.2, false, "bar"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches 3 and $[1] matches False and $[2] matches <null> and $[3] matches 1.2 and $[4] matches False and $[5] matches "bar",
					             but it differed as
					               $[1] was <null> instead of False and
					               $[2] was boolean False instead of <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationsMatchInOrder_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   3, true, null, 1.2, false, "bar"
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithElements(3, true, null, 1.2, false, "bar"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNullElementIsMissing_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).IsArray(e => e
							.WithElements(1, null),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches 1 and $[1] matches <null>,
					             but it differed as $[1] did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """)
					.Because("null elements of an array are not affected by the option");
			}

			[Test]
			public async Task WhenElementsAreInDifferentOrder_WhenIgnoringArrayOrder_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).IsArray(e => e
							.WithElements(2, 1),
						o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] matches 2 and $[1] matches 1,
					             but it differed as
					               $[0] was 1 instead of 2 and
					               $[1] was 2 instead of 1

					             JSON options:
					              - ignore additional properties
					              - ignore array order
					             """)
					.Because("the elements are still verified at their index");
			}
		}

		public sealed class WithObjectsTests
		{
			[Test]
			public async Task WhenArrayIsNull_ShouldNotAddFailureMessage()
			{
				JsonElement subject = FromString("{}");

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
			public async Task WhenElementsArNoObjects_ShouldFail()
			{
				JsonElement subject = FromString("[ [], [] ]");

				async Task Act()
					=> await That(subject).IsArray(a => a
						.WithObjects(p => p.With("foo").Matching(2)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an object and $[0].foo matches 2,
					             but it differed as $[0] was an array

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationIsNull_ShouldSkipIndex()
			{
				JsonElement subject = FromString("[{},{},{\"foo\":true}]");

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
				JsonElement subject = FromString("""
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
				JsonElement subject = FromString("""
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

			[Test]
			public async Task WhenExpectingAbsentProperties_ShouldReportOnlyTheElementsContainingThem()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   {"foo":1},
				                                   {"bar":2},
				                                   {"foo":null}
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray(e => e
						.WithObjects(
							o => o.Without("foo"),
							o => o.Without("foo"),
							o => o.Without("foo")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an object and $[0].foo does not exist and $[1] is an object and $[1].foo does not exist and $[2] is an object and $[2].foo does not exist,
					             but it differed as
					               $[0].foo was 1 and
					               $[2].foo was <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectingAbsentProperties_WhenTreatingNullAsAbsent_ShouldNotReportNullProperties()
			{
				JsonElement subject = FromString("""
				                                 [
				                                   {"foo":1},
				                                   {"bar":2},
				                                   {"foo":null}
				                                 ]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray(e => e
							.WithObjects(
								o => o.Without("foo"),
								o => o.Without("foo"),
								o => o.Without("foo")),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[0] is an object and $[0].foo does not exist and $[1] is an object and $[1].foo does not exist and $[2] is an object and $[2].foo does not exist,
					             but it differed as $[0].foo was 1

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenMatchingNullOnMissingProperty_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("[{\"foo\":1},{\"foo\":null},{}]");

				async Task Act()
					=> await That(subject).IsArray(e => e
							.WithObjects(
								o => o.With("foo").Matching(1),
								o => o.With("foo").Matching(null),
								o => o.With("foo").Matching(null)),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
