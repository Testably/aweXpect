using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class IsObject
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonElement? subject = FromString("{\"foo\": 1}");

				JsonElement result = await That(subject).IsObject();

				await That(result.GetRawText()).IsEqualTo("{\"foo\": 1}");
			}

			[Test]
			[Arguments("{}")]
			[Arguments("{\"foo\": 1}")]
			public async Task WhenJsonIsAnObject_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoObject_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

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
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("{}")]
			[Arguments("{\"foo\": 1}")]
			public async Task WhenJsonIsAnObject_ShouldFail(string json)
			{
				JsonElement? subject = FromString(json);

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
			[Arguments("[]")]
			[Arguments("2")]
			[Arguments("\"foo\"")]
			public async Task WhenJsonIsNoObject_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WhosePropertiesTests
		{
			[Test]
			public async Task All_WhenAllValuesAreStrings_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"artist\": \"The Beatles\", \"title\": \"Hey Jude\"}");

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.All().Satisfy(p => p.Value.ValueKind == JsonValueKind.String);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task All_WhenSomeValuesAreNoStrings_ShouldFail()
			{
				JsonElement? subject = FromString("{\"title\": \"Hey Jude\", \"durationSeconds\": 431}");

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
			public async Task WhenJsonIsNoObject_ShouldOnlyFailWithIsObject(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

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
			public async Task WhenNamesAreDuplicated_ShouldKeepAllProperties()
			{
				JsonElement? subject = FromString("{\"a\": 1, \"a\": 2}");

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.All().Satisfy(p => p.Value.GetInt32() == 1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties satisfy p => p.Value.GetInt32() == 1 for all items,
					             but only 1 of 2 did

					             Not matching items (properties):
					             [
					               "a":2
					             ]

					             Collection (properties):
					             [
					               "a":1,
					               "a":2
					             ]
					             """)
					.Because("the properties keep a duplicate name in the order of the document");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldOnlyFailWithIsObject()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties have exactly 2 items,
					             but it was <null>
					             """)
					.Because("the properties are only accessed when the subject is an object");
			}
		}

		public sealed class WhosePropertiesNegatedTests
		{
			[Test]
			public async Task WhenPropertiesMeetTheExpectation_ShouldFail()
			{
				JsonElement? subject = FromString("{\"a\": 1, \"b\": 2}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject().WhoseProperties.HasCount(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object whose properties have exactly 2 items,
					             but it was an object
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject().WhoseProperties.HasCount(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object whose properties have exactly 2 items,
					             but it was <null>
					             """)
					.Because("a null subject fails IsObject also when it is negated");
			}
		}
	}
}
