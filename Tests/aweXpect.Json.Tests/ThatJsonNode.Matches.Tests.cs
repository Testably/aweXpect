using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class Matches
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldReturnTheSubject()
			{
				JsonObject subject = new()
				{
					["foo"] = 1,
				};

				JsonObject? result = await That(subject).Matches(new
				{
					foo = 1,
				});

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenEnumerableMatches_ShouldSucceed()
			{
				JsonArray subject = new(1, 2);

				async Task Act()
					=> await That(subject).Matches([1, 2,]);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNodeWasModifiedBeforeTheEvaluation_ShouldVerifyTheModifiedNode()
			{
				JsonObject subject = new()
				{
					["foo"] = 1,
				};
				AndOrResult<JsonObject?, IThat<JsonObject?>> expectation = That(subject).Matches(new
				{
					foo = 2,
				});

				subject["foo"] = 2;

				async Task Act()
					=> await expectation;

				await That(Act).DoesNotThrow()
					.Because("the expectation is only evaluated when it is awaited");
			}

			[Fact]
			public async Task WhenObjectDiffers_ShouldFail()
			{
				JsonNode? subject = FromString("""{"foo": 1, "bar": [1, 2]}""");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = 2,
						bar = new[] { 1, 3, },
					});

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = 2,
					             	bar = new[] { 1, 3, },
					             },
					             but it differed as
					               $.foo was 1 instead of 2 and
					               $.bar[1] was 2 instead of 3

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Fact]
			public async Task WhenObjectHasAdditionalProperties_ShouldSucceed()
			{
				JsonNode? subject = FromString("""{"foo": 1, "bar": 2}""");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = 1,
					});

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsJsonArray_ShouldFailWithDifference()
			{
				JsonArray subject = new(1, 2);

				async Task Act()
					=> await That(subject).Matches([1, 3,]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 3,],
					             but it differed as $[1] was 2 instead of 3

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsJsonValue_ShouldCompareTheValue()
			{
				JsonValue subject = JsonValue.Create("foo");

				async Task Act()
					=> await That(subject).Matches("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches "bar",
					             but it differed as $ was "foo" instead of "bar"

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldMatchNull()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).Matches(null);

				await That(Act).DoesNotThrow()
					.Because("a null node is the JSON null");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldNotMatchAnObject()
			{
				JsonObject? subject = null;

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = 1,
					});

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = 1,
					             },
					             but it differed as $ was <null> instead of {"foo":1}

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Fact]
			public async Task WithIgnoringArrayOrder_ShouldSucceedForDifferentOrder()
			{
				JsonNode? subject = FromString("[3, 1, 2]");

				async Task Act()
					=> await That(subject).Matches([1, 2, 3,], o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithIgnoringMember_ShouldListItInTheContext()
			{
				JsonNode? subject = FromString("""{"id": 1, "name": "a"}""");

				async Task Act()
					=> await That(subject).Matches(new
					{
						id = 2,
						name = "b",
					}, o => o.IgnoringMember("id").IgnoringMember("etag"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	id = 2,
					             	name = "b",
					             },
					             but it differed as $.name was "a" instead of "b"

					             JSON options:
					              - ignore additional properties
					              - ignore members: ["id", "etag"]
					                "etag" matched no member
					             """);
			}

			[Fact]
			public async Task WithTreatingNullAsAbsent_ShouldSucceedForAbsentProperty()
			{
				JsonNode? subject = FromString("""{"foo": 1}""");

				async Task Act()
					=> await That(subject).Matches(new
					{
						foo = 1,
						bar = (string?)null,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenNodeDiffers_ShouldSucceed()
			{
				JsonNode? subject = FromString("""{"foo": 1}""");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches(new
					{
						foo = 2,
					}));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNodeMatches_ShouldFailWithTheActualJson()
			{
				JsonObject subject = new()
				{
					["foo"] = 1,
					["bar"] = new JsonArray(1, 2),
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches(new
					{
						foo = 1,
					}));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match new
					             {
					             	foo = 1,
					             },
					             but it did

					             Actual:
					             {
					               "foo": 1,
					               "bar": [
					                 1,
					                 2
					               ]
					             }

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFailForNull()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Matches(null));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match null,
					             but it did

					             Actual:
					             null

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}
	}
}
