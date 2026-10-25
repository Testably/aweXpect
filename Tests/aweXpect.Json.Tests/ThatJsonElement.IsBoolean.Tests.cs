using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsBoolean
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("true")]
			[Arguments("false")]
			public async Task WhenJsonIsABoolean_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

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
			public async Task WhenJsonIsNoBoolean_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

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
				JsonElement[] items = [FromString("true"), FromString("false"), FromString("null"),];

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
			[Arguments("true")]
			[Arguments("false")]
			public async Task WhenJsonIsABoolean_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not a boolean,
					              but it was {json}
					              """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("[]")]
			[Arguments("1")]
			[Arguments("\"true\"")]
			[Arguments("null")]
			public async Task WhenJsonIsNoBoolean_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString("false");

				async Task Act()
					=> await That(subject).IsBoolean(b => b.IsFalse());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonElement subject = FromString("true");

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
				JsonElement subject = FromString("true");

				async Task Act()
					=> await That(subject).IsBoolean(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenJsonIsNoBoolean_ShouldNotVerifyTheExpectations()
			{
				JsonElement subject = FromString("\"false\"");

				async Task Act()
					=> await That(subject).IsBoolean(b => b.IsFalse());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean which is False,
					             but it was a string
					             """);
			}
		}

		public sealed class WithValueTests
		{
			[Test]
			public async Task WhenBooleanDiffers_ShouldFail()
			{
				JsonElement subject = FromString("false");

				async Task Act()
					=> await That(subject).IsBoolean(true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean equal to True,
					             but it was False
					             """);
			}

			[Test]
			[Arguments("true", true)]
			[Arguments("false", false)]
			public async Task WhenBooleanIsEqual_ShouldSucceed(string json, bool expected)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsBoolean(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenJsonIsNoBoolean_ShouldFail()
			{
				JsonElement subject = FromString("\"true\"");

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
			public async Task WhenNestedInHasProperty_ShouldNameThePathOfTheProperty()
			{
				JsonElement subject = FromString("{\"explicit\": true}");

				async Task Act()
					=> await That(subject).HasProperty("explicit", e => e.IsBoolean(false));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "explicit" which is a boolean equal to False,
					             but $.explicit was True
					             """);
			}
		}

		public sealed class WithValueNegatedTests
		{
			[Test]
			public async Task WhenBooleanDiffers_ShouldSucceed()
			{
				JsonElement subject = FromString("false");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean(true));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBooleanIsEqual_ShouldFail()
			{
				JsonElement subject = FromString("true");

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
			public async Task WhenJsonIsNoBoolean_ShouldSucceed()
			{
				JsonElement subject = FromString("\"true\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean(true));

				await That(Act).DoesNotThrow()
					.Because("a value of another kind is no boolean equal to the expected value, like for IsBoolean()");
			}
		}
	}
}
