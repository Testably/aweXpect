using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class HasProperty
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonObject subject = new() { ["foo"] = 1, };

				JsonObject result = await That(subject).HasProperty("foo");

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFailWithThePathOfTheProperty()
			{
				JsonNode? subject = FromString("{\"title\": \"Yesterday\"}");

				async Task Act()
					=> await That(subject).HasProperty("title", p => p.IsString(s => s.StartsWith("Let")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "title" which is a string which starts with "Let",
					             but $.title was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonNode? subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasProperty("foo", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				JsonNode? subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasProperty(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'name' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNodeIsNoObject_ShouldFail()
			{
				JsonArray subject = new(1, 2);

				async Task Act()
					=> await That(subject).HasProperty("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo",
					             but it was an array
					             """);
			}

			[Test]
			public async Task WhenNodeWasModifiedBeforeTheEvaluation_ShouldVerifyTheModifiedNode()
			{
				JsonObject subject = new();

				async Task Act()
				{
					IThat<JsonObject> that = That(subject);
					subject["foo"] = 1;
					await that.HasProperty("foo");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldNotVerifyTheExpectations()
			{
				JsonNode? subject = FromString("{\"bar\": 1}");

				async Task Act()
					=> await That(subject).HasProperty("foo", p => p.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo" which is a number,
					             but property $.foo did not exist
					             """);
			}

			[Test]
			public async Task WhenPropertyExists_ShouldPassTheNodeItself()
			{
				JsonArray tracks = new("Come Together");
				JsonObject subject = new() { ["tracks"] = tracks, };

				async Task Act()
					=> await That(subject).HasProperty("tracks", t => t.IsSameAs(tracks));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldPassANullNode()
			{
				JsonNode? subject = FromString("{\"label\": null}");

				async Task Act()
					=> await That(subject).HasProperty("label", l => l.IsNull());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonNode? subject = FromString("{\"label\": null}");

				async Task Act()
					=> await That(subject).HasProperty("label", l => l.IsNull()).TreatingNullAsAbsent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "label" which is null,
					             but property $.label did not exist

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).HasProperty("foo", p => p.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo" which is a number,
					             but it was null
					             """)
					.Because("a null node is the JSON null");
			}

			[Test]
			public async Task WhenValueHoldsAnObject_ShouldPassTheProperty()
			{
				JsonNode? subject = JsonValue.Create(new
				{
					artist = new
					{
						name = "The Beatles",
					},
				});

				async Task Act()
					=> await That(subject).HasProperty("artist", a => a
						.HasProperty("name", n => n.IsString(s => s.IsEqualTo("The Beatles"))));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyExists_ShouldFail()
			{
				JsonNode? subject = FromString("{\"foo\": \"bar\"}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProperty("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "foo",
					             but it had property "foo" with "bar"
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProperty("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "foo",
					             but it was null
					             """);
			}
		}
	}

	public sealed class DoesNotHaveProperty
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				JsonNode? subject = FromString("{}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'name' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldSucceed()
			{
				JsonNode? subject = FromString("{\"bar\": 1}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				JsonNode? subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "foo",
					             but it had property "foo" with <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonNode? subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("foo").TreatingNullAsAbsent();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
