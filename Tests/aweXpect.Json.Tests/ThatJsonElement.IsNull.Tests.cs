using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsNull
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenJsonIsNull_ShouldSucceed()
			{
				JsonElement subject = FromString("null");

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("[]", "an array")]
			[InlineData("0", "a number")]
			[InlineData("\"null\"", "a string")]
			[InlineData("true", "true")]
			[InlineData("false", "false")]
			public async Task WhenJsonIsNotNull_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is null,
					              but it was {kindString}
					              """);
			}

			[Fact]
			public async Task WhenElementIsUndefined_ShouldFail()
			{
				JsonElement subject = default;

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is null,
					             but it was undefined
					             """)
					.Because("an undefined element has no value, which differs from the JSON value null");
			}

			[Fact]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement[] items = [FromString("null"), FromString("{}"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsNull()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are null for all items,
					             but only 1 of 2 were
					             *
					             """).AsWildcard()
					.Because("the items are the plural subject of the expectation");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenJsonIsNull_ShouldFail()
			{
				JsonElement subject = FromString("null");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNull());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not null,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("{}")]
			[InlineData("[]")]
			[InlineData("0")]
			[InlineData("\"null\"")]
			[InlineData("false")]
			public async Task WhenJsonIsNotNull_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNull());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
