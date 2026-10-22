using System.Text.Json.Nodes;

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
	}
}
