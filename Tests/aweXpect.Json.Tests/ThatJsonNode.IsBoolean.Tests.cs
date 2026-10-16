using System.Text.Json.Nodes;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class IsBoolean
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenNodeIsABoolean_ShouldSucceed(bool value)
			{
				JsonValue subject = JsonValue.Create(value);

				async Task Act()
					=> await That(subject).IsBoolean();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("[]", "an array")]
			[InlineData("1", "a number")]
			[InlineData("\"true\"", "a string")]
			[InlineData("null", "null")]
			public async Task WhenNodeIsNoBoolean_ShouldFail(string json, string kindString)
			{
				JsonNode? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsBoolean();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is a boolean,
					              but it was {kindString}
					              """);
			}

			[Fact]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonNode?[] items = [JsonValue.Create(true), JsonValue.Create(false), null,];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsBoolean()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are booleans for all items,
					             but only 2 of 3 were
					             *
					             """).AsWildcard()
					.Because("the items are the plural subject of the expectation");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenNodeIsABoolean_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(true);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a boolean,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("{}")]
			[InlineData("1")]
			[InlineData("null")]
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
