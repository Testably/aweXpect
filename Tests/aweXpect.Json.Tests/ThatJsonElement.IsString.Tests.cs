using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsString
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("\"\"")]
			[Arguments("\"foo\"")]
			public async Task WhenJsonIsAString_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsString();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			[Arguments("true", "true")]
			[Arguments("false", "false")]
			[Arguments("null", "null")]
			public async Task WhenJsonIsNoString_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsString();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is a string,
					              but it was {kindString}
					              """);
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement[] items = [FromString("\"foo\""), FromString("2"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsString()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are a string for all items,
					             but only 1 of 2 were
					             *
					             """).AsWildcard()
					.Because("the items are the plural subject of the expectation");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenJsonIsAString_ShouldFail()
			{
				JsonElement subject = FromString("\"foo\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a string,
					             but it was a string
					             """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("[]")]
			[Arguments("2")]
			[Arguments("true")]
			[Arguments("null")]
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
