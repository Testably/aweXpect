using System.Text.Json.Nodes;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class MatchesExactly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEnumerableHasAdditionalElements_ShouldFail()
			{
				JsonArray subject = new(1, 2, 3);

				async Task Act()
					=> await That(subject).MatchesExactly([1, 2,]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches [1, 2,] exactly,
					             but it differed as $[2] had unexpected 3
					             """);
			}

			[Fact]
			public async Task WhenNodeHasAdditionalStringProperty_ShouldFail()
			{
				JsonNode? track = FromString("""{"title": "Let It Be", "artist": "The Beatles"}""");

				async Task Act()
					=> await That(track).MatchesExactly(new { title = "Let It Be" });

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that track
					             matches new { title = "Let It Be" } exactly,
					             but it differed as $.artist had unexpected "The Beatles"
					             """);
			}

			[Fact]
			public async Task WhenObjectHasAdditionalProperties_ShouldFail()
			{
				JsonNode? subject = FromString("""{"foo": 1, "bar": 2}""");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = 1,
					});

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches new
					             {
					             	foo = 1,
					             } exactly,
					             but it differed as $.bar had unexpected 2
					             """);
			}

			[Fact]
			public async Task WhenObjectMatchesExactly_ShouldSucceed()
			{
				JsonObject subject = new()
				{
					["foo"] = 1,
					["bar"] = new JsonArray("a", null),
				};

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = 1,
						bar = new[] { "a", null, },
					});

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldMatchNull()
			{
				JsonArray? subject = null;

				async Task Act()
					=> await That(subject).MatchesExactly(null);

				await That(Act).DoesNotThrow()
					.Because("a null node is the JSON null");
			}

			[Fact]
			public async Task WithTreatingNullAsAbsent_ShouldIgnoreNullProperty()
			{
				JsonNode? subject = FromString("""{"foo": 1, "bar": null}""");

				async Task Act()
					=> await That(subject).MatchesExactly(new
					{
						foo = 1,
					}, o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenObjectHasAdditionalProperties_ShouldSucceed()
			{
				JsonNode? subject = FromString("""{"foo": 1, "bar": 2}""");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(new
					{
						foo = 1,
					}));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenObjectMatchesExactly_ShouldFail()
			{
				JsonNode? subject = FromString("""{"foo": 1}""");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesExactly(new
					{
						foo = 1,
					}));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match new
					             {
					             	foo = 1,
					             } exactly,
					             but it did

					             Actual:
					             {
					               "foo": 1
					             }
					             """);
			}
		}
	}
}
