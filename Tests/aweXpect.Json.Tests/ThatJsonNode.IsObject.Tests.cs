using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class IsObject
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonObject subject = new();

				JsonObject? result = await That(subject).IsObject();

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenNodeIsAnObject_ShouldSucceed()
			{
				JsonNode? subject = FromString("""{"foo": 1}""");

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenNodeIsNoObject_ShouldFail(string json, string kindString)
			{
				JsonNode? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object,
					              but it was {kindString}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsDeclaredAsNonNullableJsonNode_ShouldSucceed()
			{
				JsonNode subject = new JsonObject();

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? node = FromString("null");

				async Task Act()
					=> await That(node).IsObject();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that node
					             is an object,
					             but it was null
					             """)
					.Because("a null node is the JSON null");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNodeIsAnObject_ShouldFail()
			{
				JsonObject subject = new()
				{
					["foo"] = 1,
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object,
					             but it was an object
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject());

				await That(Act).DoesNotThrow()
					.Because("a null node is the JSON null, which is no object");
			}
		}

		public sealed class WhosePropertiesTests
		{
			[Test]
			public async Task All_WhenAllValuesAreStrings_ShouldSucceed()
			{
				JsonNode? subject = FromString("""{"artist": "The Beatles", "title": "Hey Jude"}""");

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.All().Satisfy(p => p.Value?.GetValueKind() == JsonValueKind.String);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task All_WhenSomeNamesDoNotSatisfyThePredicate_ShouldFail()
			{
				JsonObject subject = new()
				{
					["x-title"] = "Hey Jude",
					["artist"] = new JsonObject
					{
						["name"] = "The Beatles",
					},
					["label"] = null,
				};

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.All().Satisfy(p => p.Key.StartsWith("x-"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties satisfy p => p.Key.StartsWith("x-") for all items,
					             but only 1 of 3 did

					             Not matching items (properties):
					             [
					               "artist":{"name":"The Beatles"},
					               "label":null
					             ]

					             Collection (properties):
					             [
					               "x-title":"Hey Jude",
					               "artist":{"name":"The Beatles"},
					               "label":null
					             ]
					             """);
			}

			[Test]
			public async Task All_WhenSomeValuesAreNoStrings_ShouldFail()
			{
				JsonObject subject = new()
				{
					["title"] = "Hey Jude",
					["durationSeconds"] = 431,
				};

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.All().Satisfy(p => p.Value?.GetValueKind() == JsonValueKind.String);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties satisfy p => p.Value?.GetValueKind() == JsonValueKind.String for all items,
					             but only 1 of 2 did

					             Not matching items (properties):
					             [
					               "durationSeconds":431
					             ]

					             Collection (properties):
					             [
					               "title":"Hey Jude",
					               "durationSeconds":431
					             ]
					             """);
			}

			[Test]
			public async Task Contains_WhenPropertiesContainTheNameWithTheNode_ShouldSucceed()
			{
				JsonObject subject = new()
				{
					["title"] = "Let It Be",
				};
				JsonNode? title = subject["title"];

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.Contains(new KeyValuePair<string, JsonNode?>("title", title));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HasItemThat_WhenValueIsTheSameNode_ShouldSucceed()
			{
				JsonObject subject = new()
				{
					["artist"] = "The Beatles",
					["title"] = "Let It Be",
				};
				JsonNode? title = subject["title"];

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.HasItemThat(p => p.Whose(x => x.Value, v => v.IsSameAs(title))).AtIndex(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenNodeIsNoObject_ShouldOnlyFailWithIsObject(string json, string kindString)
			{
				JsonNode? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object whose properties have exactly 2 items,
					              but it was {kindString}
					              """)
					.Because("the properties are only accessed when the subject is an object");
			}

			[Test]
			public async Task WhenNodeWasModifiedBeforeTheEvaluation_ShouldVerifyTheModifiedProperties()
			{
				JsonObject subject = new()
				{
					["a"] = 1,
				};
				IThat<IEnumerable<KeyValuePair<string, JsonNode?>>> expectation = That(subject).IsObject().WhoseProperties;

				subject["b"] = 2;

				async Task Act()
					=> await expectation.HasCount(2);

				await That(Act).DoesNotThrow()
					.Because("the expectation is only evaluated when it is awaited");
			}

			[Test]
			public async Task WhenObjectContainsANameTwice_ShouldKeepBothProperties()
			{
				JsonNode? subject = FromString("""{"title": "Let It Be", "title": "Hey Jude"}""");

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.HasCount(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties have exactly one item,
					             but properties had 2 items

					             Collection (properties):
					             [
					               "title":"Let It Be",
					               "title":"Hey Jude"
					             ]
					             """)
					.Because("the parsed object keeps every property of the document");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties are empty,
					             but it was null
					             """);
			}
		}

		public sealed class WhosePropertiesNegatedTests
		{
			[Test]
			public async Task WhenPropertiesMeetTheExpectation_ShouldFail()
			{
				JsonObject subject = new()
				{
					["a"] = 1,
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject().WhoseProperties.HasCount(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object whose properties have exactly one item,
					             but it was an object
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject().WhoseProperties.HasCount(1));

				await That(Act).DoesNotThrow()
					.Because("a null node is the JSON null, which is no object");
			}
		}
	}
}
