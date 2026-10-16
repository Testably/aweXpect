using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class IsArray
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldReturnTheSubject()
			{
				JsonArray subject = new(1);

				JsonArray? result = await That(subject).IsArray();

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenElementsDiffer_ShouldFail()
			{
				JsonArray subject = new(1, "a", new JsonArray(3));

				async Task Act()
					=> await That(subject).IsArray(a => a
						.At(1).Matching("b").And
						.At(2).AnArray(b => b.WithElements(4)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an array and $[1] matches "b" and $[2] is an array and $[2][0] matches 4,
					             but it differed as
					               $[1] was "a" instead of "b" and
					               $[2][0] was 3 instead of 4

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Fact]
			public async Task WhenExpectationIsNull_ShouldThrowArgumentNullException()
			{
				JsonNode? subject = FromString("[]");

				async Task Act()
					=> await That(subject).IsArray(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectation").And
					.WithMessage("The 'expectation' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenNodeIsAnArray_ShouldSucceed()
			{
				JsonNode? subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).IsArray(a => a.With(2).Elements());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("2", "a number")]
			[InlineData("\"foo\"", "a string")]
			public async Task WhenNodeIsNoArray_ShouldFail(string json, string kindString)
			{
				JsonNode? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an array,
					              but it was {kindString}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonArray? subject = null;

				async Task Act()
					=> await That(subject).IsArray();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an array,
					             but it was null
					             """)
					.Because("a null node is the JSON null");
			}

			[Fact]
			public async Task WithIgnoringMember_ShouldListItInTheContext()
			{
				JsonNode? subject = FromString("""[{"id": 1, "name": "a"}]""");

				async Task Act()
					=> await That(subject).IsArray(a => a
							.At(0).Matching(new
							{
								id = 2,
								name = "b",
							}),
						o => o.IgnoringMember("id"));

				await That(Act).Throws<XunitException>()
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
					              - ignore members: ["id"]
					             """);
			}

			[Fact]
			public async Task WithIgnoringArrayOrder_ShouldPairNestedArraysRegardlessOfOrder()
			{
				JsonNode? subject = FromString("""[{"tags": ["b", "a"]}]""");

				async Task Act()
					=> await That(subject).IsArray(a => a
							.At(0).Matching(new
							{
								tags = new[] { "a", "b", },
							}),
						o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenNodeIsAnArray_ShouldFail()
			{
				JsonArray subject = new(1, 2);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsArray(a => a.With(2).Elements()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an array or not with 2 elements,
					             but it was

					             Actual:
					             [
					               1,
					               2
					             ]

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Fact]
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
			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenNodeWasModifiedBeforeTheEvaluation_ShouldVerifyTheModifiedElements()
			{
				JsonArray subject = new(1, 2);
				IThat<IEnumerable<JsonElement>> expectation = That(subject).IsArray().WhoseElements;

				subject.Add(3);

				async Task Act()
					=> await expectation.HasCount(3);

				await That(Act).DoesNotThrow()
					.Because("the expectation is only evaluated when it is awaited");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.IsEmpty();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an array whose elements are empty,
					             but it was null
					             """);
			}
		}
	}
}
