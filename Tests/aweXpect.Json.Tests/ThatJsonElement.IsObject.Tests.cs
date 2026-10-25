using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsObject
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				JsonElement result = await That(subject).IsObject();

				await That(result.GetRawText()).IsEqualTo("{\"foo\": 1}");
			}

			[Test]
			[Arguments("{}")]
			[Arguments("{\"foo\": 1}")]
			public async Task WhenJsonIsAnObject_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object,
					              but it was {kindString}
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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString("""
				                                 {"artist": "The Beatles", "title": "Hey Jude"}
				                                 """);

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.All().Satisfy(p => p.Value.ValueKind == JsonValueKind.String);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task All_WhenSomeValuesAreNoStrings_ShouldFormatThePropertiesAsJson()
			{
				JsonElement subject = FromString("""
				                                 {
				                                   "title": "Hey Jude",
				                                   "durationSeconds": 431,
				                                   "explicit": false,
				                                   "album": null,
				                                   "artist": {"name": "The Beatles", "members": [1, 2]},
				                                   "tags": []
				                                 }
				                                 """);

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.All().Satisfy(p => p.Value.ValueKind == JsonValueKind.String);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties satisfy p => p.Value.ValueKind == JsonValueKind.String for all items,
					             but only 1 of 6 did

					             Not matching items (properties):
					             [
					               "durationSeconds":431,
					               "explicit":false,
					               "album":null,
					               "artist":{"name":"The Beatles","members":[1,2]},
					               "tags":[]
					             ]

					             Collection (properties):
					             [
					               "title":"Hey Jude",
					               "durationSeconds":431,
					               "explicit":false,
					               "album":null,
					               "artist":{"name":"The Beatles","members":[1,2]},
					               "tags":[]
					             ]
					             """)
					.Because("the properties are formatted as JSON in a single line instead of with their raw text");
			}

			[Test]
			public async Task All_WithJsonExpectationOnTheValue_WhenAValueDoesNotComply_ShouldFail()
			{
				JsonElement subject = FromString("""
				                                 {"title": "Hey Jude", "durationSeconds": 431}
				                                 """);

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.All().ComplyWith(p => p.Whose(x => x.Value, v => v.IsString()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties have Value that is a string for all items,
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
			public async Task None_WhenAPropertyMatches_ShouldFail()
			{
				JsonElement subject = FromString("""
				                                 {"title": "Hey Jude", "password": "secret"}
				                                 """);

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.None().Satisfy(p => p.Name == "password");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties satisfy p => p.Name == "password" for no items,
					             but 1 of 2 did

					             Matching items (properties):
					             [
					               "password":"secret"
					             ]

					             Collection (properties):
					             [
					               "title":"Hey Jude",
					               "password":"secret"
					             ]
					             """);
			}

			[Test]
			public async Task None_WhenNoPropertyMatches_ShouldSucceed()
			{
				JsonElement subject = FromString("""
				                                 {"title": "Hey Jude"}
				                                 """);

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.None().Satisfy(p => p.Name == "password");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFollowingAnAlternative_ShouldOnlyApplyToTheObject()
			{
				JsonElement subject = FromString("{\"a\": 1}");

				async Task Act()
					=> await That(subject).IsArray().Or.IsObject().WhoseProperties.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an array or is an object whose properties have exactly 2 items,
					             but it was an object and properties had only 1 item

					             Collection (properties):
					             [
					               "a":1
					             ]
					             """);
			}

			[Test]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			[Arguments("null", "null")]
			public async Task WhenJsonIsNoObject_ShouldOnlyFailWithIsObject(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.All().Satisfy(p => p.Name.Length > 0);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object whose properties satisfy p => p.Name.Length > 0 for all items,
					              but it was {kindString}
					              """)
					.Because("the properties are only accessed when the subject is an object");
			}

			[Test]
			public async Task WhenNamesAreDuplicated_ShouldKeepAllProperties()
			{
				JsonElement subject = FromString("{\"a\": 1, \"b\": 2, \"a\": 3}");

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.Exactly(1).Satisfy(p => p.Name == "a");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties satisfy p => p.Name == "a" for exactly one item,
					             but 2 of 3 did

					             Collection (properties):
					             [
					               "a":1,
					               "b":2,
					               "a":3
					             ]
					             """)
					.Because("the properties keep a duplicate name in the order of the document");
			}
		}

		public sealed class WhosePropertiesNegatedTests
		{
			[Test]
			public async Task WhenPropertiesDoNotMeetTheExpectation_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"a\": 1}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject().WhoseProperties.HasCount(2));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesMeetTheExpectation_ShouldFail()
			{
				JsonElement subject = FromString("{\"a\": 1, \"b\": 2}");

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
			public async Task WhenSubjectIsNoObject_ShouldSucceed()
			{
				JsonElement subject = FromString("[]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject().WhoseProperties.HasCount(2));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
