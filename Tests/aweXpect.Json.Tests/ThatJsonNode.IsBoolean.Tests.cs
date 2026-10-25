using System.Text.Json.Nodes;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class IsBoolean
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenNodeIsABoolean_ShouldSucceed(bool value)
			{
				JsonValue subject = JsonValue.Create(value);

				async Task Act()
					=> await That(subject).IsBoolean();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("1", "a number")]
			[Arguments("\"true\"", "a string")]
			[Arguments("null", "null")]
			public async Task WhenNodeIsNoBoolean_ShouldFail(string json, string kindString)
			{
				JsonNode? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsBoolean();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is a boolean,
					              but it was {kindString}
					              """);
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonNode?[] items = [JsonValue.Create(true), JsonValue.Create(false), null,];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsBoolean()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are a boolean for all items,
					             but only 2 of 3 were
					             *
					             """).AsWildcard()
					.Because("the items are the plural subject of the expectation");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNodeIsABoolean_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(true);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a boolean,
					             but it was true
					             """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("1")]
			[Arguments("null")]
			public async Task WhenNodeIsNoBoolean_ShouldSucceed(string json)
			{
				JsonNode? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsAreMet_ShouldSucceed()
			{
				JsonNode? subject = FromString("{\"explicit\": false}");

				async Task Act()
					=> await That(subject).HasProperty("explicit", e => e.IsBoolean(b => b.IsFalse()));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(true);

				async Task Act()
					=> await That(subject).IsBoolean(b => b.IsFalse());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean which is False,
					             but it was True
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonValue subject = JsonValue.Create(true);

				async Task Act()
					=> await That(subject).IsBoolean(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}
		}

		public sealed class WithValueTests
		{
			[Test]
			public async Task WhenBooleanDiffers_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(true);

				async Task Act()
					=> await That(subject).IsBoolean(false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean equal to False,
					             but it was True
					             """);
			}

			[Test]
			public async Task WhenBooleanIsEqual_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create(true);

				async Task Act()
					=> await That(subject).IsBoolean(true);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNodeIsNoBoolean_ShouldFail()
			{
				JsonValue subject = JsonValue.Create("true");

				async Task Act()
					=> await That(subject).IsBoolean(true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean equal to True,
					             but it was a string
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).IsBoolean(true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean equal to True,
					             but it was null
					             """)
					.Because("a null node is the JSON null");
			}
		}

		public sealed class WithValueNegatedTests
		{
			[Test]
			public async Task WhenBooleanDiffers_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create(true);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean(false));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBooleanIsEqual_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(true);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean(true));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a boolean equal to True,
					             but it was True
					             """);
			}

			[Test]
			public async Task WhenNodeIsNoBoolean_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create("true");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean(true));

				await That(Act).DoesNotThrow()
					.Because("a value of another kind is no boolean equal to the expected value, like for IsBoolean()");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean(true));

				await That(Act).DoesNotThrow()
					.Because("a null node is the JSON null, which is no boolean");
			}
		}
	}
}
