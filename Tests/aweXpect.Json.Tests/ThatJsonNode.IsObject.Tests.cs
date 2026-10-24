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
			public async Task WhenExpectationIsNull_ShouldThrowArgumentNullException()
			{
				JsonNode? subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
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
			public async Task WhenPropertyDiffers_ShouldFail()
			{
				JsonObject subject = new()
				{
					["foo"] = 1,
					["bar"] = new JsonObject
					{
						["baz"] = "a",
					},
				};

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").Matching(2).And
						.With("bar").AnObject(b => b.With("baz").Matching("b")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2 and $.bar is an object and $.bar.baz matches "b",
					             but it differed as
					               $.foo was 1 instead of 2 and
					               $.bar.baz was "a" instead of "b"

					             JSON options:
					              - ignore additional properties
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

			[Test]
			public async Task WhenSubjectIsNull_WithExpectations_ShouldFail()
			{
				JsonObject? subject = null;

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(true));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches true,
					             but it was null

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WithIgnoringMember_ShouldListItInTheContext()
			{
				JsonNode? subject = FromString("""{"id": 1}""");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("id").Matching(2), o => o.IgnoringMember("id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.id matches 2,
					             but it differed as $.id was 1 instead of 2

					             JSON options:
					              - ignore additional properties
					              - ignore members: ["id"]
					             """)
					.Because("an explicit expectation wins over an ignored member");
			}

			[Test]
			public async Task WithTreatingNullAsAbsent_ShouldTreatNullPropertyAsAbsent()
			{
				JsonNode? subject = FromString("""{"foo": null}""");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AString(), o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is a string,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
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
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.With("foo").Matching(1)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.foo does not match 1,
					             but it was

					             Actual:
					             {
					               "foo": 1
					             }

					             JSON options:
					              - ignore additional properties
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
						.All().Satisfy(p => p.Value.ValueKind == JsonValueKind.String);

				await That(Act).DoesNotThrow();
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
						.All().Satisfy(p => p.Value.ValueKind == JsonValueKind.String);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties satisfy p => p.Value.ValueKind == JsonValueKind.String for all items,
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
				IThat<IEnumerable<JsonProperty>> expectation = That(subject).IsObject().WhoseProperties;

				subject["b"] = 2;

				async Task Act()
					=> await expectation.HasCount(2);

				await That(Act).DoesNotThrow()
					.Because("the expectation is only evaluated when it is awaited");
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
