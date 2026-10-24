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

		public sealed class WithExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsAreMet_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString(s => s.StartsWith("let").IgnoringCase());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonElement subject = FromString("\"Yesterday\"");

				async Task Act()
					=> await That(subject).IsString(s => s.IsEqualTo("Let*").AsWildcard());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string which matches "Let*",
					             but it did not match:
					               ↓ (actual)
					               "Yesterday"
					               "Let*"
					               ↑ (wildcard pattern)
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("\"foo\"");

				async Task Act()
					=> await That(subject).IsString(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenJsonIsNoString_ShouldNotVerifyTheExpectations()
			{
				JsonElement subject = FromString("243");

				async Task Act()
					=> await That(subject).IsString(s => s.IsEqualTo("243"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string which is equal to "243",
					             but it was a number
					             """);
			}

			[Test]
			public async Task WhenNegatedAndExpectationsAreMet_ShouldFail()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString(s => s.StartsWith("Let")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a string which starts with "Let",
					             but it was a string
					             """);
			}
		}
	}
}
