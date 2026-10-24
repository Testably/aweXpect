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
	}
}
