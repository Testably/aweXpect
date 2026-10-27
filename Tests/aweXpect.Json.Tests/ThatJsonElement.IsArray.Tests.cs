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
					             whose Items are an array for all items,
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
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement[] items = [FromString("[]"), FromString("{}"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.DoesNotComplyWith(it => it.IsArray())));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are not an array for all items,
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
					             but it was an array
					             """);
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
						.AtLeast(2).ComplyWith(t => t.HasProperty("durationSeconds", d => d.IsNumber()));

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
						.AtLeast(2).ComplyWith(t => t.HasProperty("durationSeconds", d => d.IsNumber()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements have property "durationSeconds" which is a number for at least 2 items,
					             but only 1 of 3 did

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
					             is an array whose elements are a number for at most one item,
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
					             is an array whose elements are a number for between 1 and 2 items,
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
						.None().ComplyWith(t => t.HasProperty("explicit", e => e.IsBoolean(b => b.IsTrue())));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements have property "explicit" which is a boolean which is True for no items,
					             but 1 of 2 did

					             Matching items (elements):
					             [
					               {"explicit":true}
					             ]

					             Collection (elements):
					             [
					               {"explicit":false},
					               {"explicit":true}
					             ]
					             """);
			}

			[Test]
			public async Task None_WhenNoElementComplies_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 [{"explicit": false}, {}]
				                                 """);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements
						.None().ComplyWith(t => t.HasProperty("explicit", e => e.IsBoolean(b => b.IsTrue())));

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
					             is an array whose elements have exactly 3 items and are a string for all items,
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
					              is an array whose elements are a number for all items,
					              but it was {kindString}
					              """)
					.Because("the elements are only accessed when the subject is an array");
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
					             is not an array whose elements are a number for all items,
					             but it was an array
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

			[Test]
			public async Task WhenElementsContainTheExpectedElement_ShouldFormatItAsJson()
			{
				JsonElement subject = FromString("""[{"title": "Let It Be"}, "rock"]""");
				JsonElement first = subject[0];
				JsonElement second = subject[1];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsArray().WhoseElements.Contains(first).And.Contains(second));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an array whose elements contain an item equal to {"title":"Let It Be"} at least once and contain an item equal to "rock" at least once,
					             but it was an array
					             """)
					.Because("the expected elements are formatted like in the failure message of the expectation");
			}
		}
	}
}
