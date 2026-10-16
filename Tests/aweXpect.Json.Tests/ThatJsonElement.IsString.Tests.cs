using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsString
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("\"\"")]
			[InlineData("\"foo\"")]
			public async Task WhenJsonIsAString_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsString();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("[]", "an array")]
			[InlineData("2", "a number")]
			[InlineData("true", "true")]
			[InlineData("false", "false")]
			[InlineData("null", "null")]
			public async Task WhenJsonIsNoString_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsString();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is a string,
					              but it was {kindString}
					              """);
			}

			[Fact]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement[] items = [FromString("\"foo\""), FromString("2"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsString()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are strings for all items,
					             but only 1 of 2 were
					             *
					             """).AsWildcard()
					.Because("the items are the plural subject of the expectation");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenJsonIsAString_ShouldFail()
			{
				JsonElement subject = FromString("\"foo\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a string,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("{}")]
			[InlineData("[]")]
			[InlineData("2")]
			[InlineData("true")]
			[InlineData("null")]
			public async Task WhenJsonIsNoString_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
