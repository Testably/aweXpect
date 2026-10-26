using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using aweXpect.Core;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class IsArray
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonArray subject = new(1);

				JsonArray? result = await That(subject).IsArray();

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenNodeIsAnArray_ShouldSucceed()
			{
				JsonNode? subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenNodeIsNoArray_ShouldFail(string json, string kindString)
			{
				JsonNode? subject = FromString(json);

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
				JsonArray? subject = null;

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array,
					             but it was null
					             """)
					.Because("a null node is the JSON null");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenElementsDoNotContainTheNode_ShouldSucceed()
			{
				JsonArray subject = new(new JsonObject
				{
					["title"] = "Let It Be",
				});
				JsonObject other = new()
				{
					["title"] = "Let It Be",
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray().WhoseElements.Contains(other));

				await That(Act).DoesNotThrow()
					.Because("the elements are the nodes of the array, which have no value equality");
			}

			[Test]
			public async Task WhenNodeIsAnArray_ShouldFail()
			{
				JsonArray subject = new(1, 2);

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
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray());

				await That(Act).DoesNotThrow()
					.Because("a null node is the JSON null, which is no array");
			}
		}

		public sealed class WhoseElementsTests
		{
			[Test]
			public async Task All_WhenAllElementsComply_ShouldSucceed()
			{
				JsonNode? subject = FromString("""
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
				JsonArray subject = new(
					new JsonObject
					{
						["artist"] = "The Beatles",
					},
					new JsonObject
					{
						["artist"] = "Queen",
					});

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
					               {"artist":"Queen"}
					             ]

					             Collection (elements):
					             [
					               {"artist":"The Beatles"},
					               {"artist":"Queen"}
					             ]

					             JSON options (elements[1]):
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task Contains_WhenElementsContainTheNode_ShouldSucceed()
			{
				JsonArray subject = new(new JsonObject
				{
					["title"] = "Let It Be",
				});
				JsonNode first = subject[0]!;

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.Contains(first);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Contains_WhenElementsDoNotContainTheNode_ShouldFail()
			{
				JsonArray subject = new(new JsonObject
				{
					["title"] = "Let It Be",
				});
				JsonObject other = new()
				{
					["title"] = "Let It Be",
				};

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.Contains(other);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements contain an item equal to {"title":"Let It Be"} at least once,
					             but elements did not contain it

					             Collection (elements):
					             [
					               {"title":"Let It Be"}
					             ]
					             """)
					.Because("the elements are the nodes of the array, which have no value equality");
			}

			[Test]
			public async Task HasItemThat_WhenElementIsTheSameNode_ShouldSucceed()
			{
				JsonArray subject = new("Hey Jude", null, "Let It Be");
				JsonNode? last = subject[2];

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.HasItemThat(e => e.IsSameAs(last)).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNodeIsAValueThatRepresentsAnArray_ShouldVerifyItsElementsAsNodes()
			{
				JsonTypeInfo<string?[]> typeInfo = (JsonTypeInfo<string?[]>)new JsonSerializerOptions
				{
					TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
				}.GetTypeInfo(typeof(string?[]));
				JsonValue? subject = JsonValue.Create(["Hey Jude", null, "Let It Be",], typeInfo);

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.HasItemThat(e => e.Is<JsonValue>()).AtIndex(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements have an item that is of type JsonValue at index 1,
					             but for the item at index 1, it was <null>

					             Collection (elements):
					             [
					               "Hey Jude",
					               <null>,
					               "Let It Be"
					             ]
					             """);
			}

			[Test]
			public async Task WhenNodeWasModifiedBeforeTheEvaluation_ShouldVerifyTheModifiedElements()
			{
				JsonArray subject = new(1, 2);
				IThat<IEnumerable<JsonNode?>> expectation = That(subject).IsArray().WhoseElements;

				subject.Add(3);

				async Task Act()
					=> await expectation.HasCount(3);

				await That(Act).DoesNotThrow()
					.Because("the expectation is only evaluated when it is awaited");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements are empty,
					             but it was null
					             """);
			}
		}
	}
}
