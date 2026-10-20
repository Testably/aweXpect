using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsNull
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenJsonIsNull_ShouldSucceed()
			{
				JsonElement subject = FromString("null");

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("0", "a number")]
			[Arguments("\"null\"", "a string")]
			[Arguments("true", "true")]
			[Arguments("false", "false")]
			public async Task WhenJsonIsNotNull_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is null,
					              but it was {kindString}
					              """);
			}

			[Test]
			public async Task WhenElementIsUndefined_ShouldFail()
			{
				JsonElement subject = default;

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is null,
					             but it was undefined
					             """)
					.Because("an undefined element has no value, which differs from the JSON value null");
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement[] items = [FromString("null"), FromString("{}"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsNull()));

				await That(Act).Throws<FailException>()
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
			[Test]
			public async Task WhenJsonIsNull_ShouldFail()
			{
				JsonElement subject = FromString("null");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNull());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not null,
					             but it was
					             """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("[]")]
			[Arguments("0")]
			[Arguments("\"null\"")]
			[Arguments("false")]
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
