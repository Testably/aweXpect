using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class IsNumber
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("0")]
			[Arguments("-4.2e3")]
			public async Task WhenJsonIsANumber_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNumber();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("\"2\"", "a string")]
			[Arguments("true", "true")]
			[Arguments("false", "false")]
			[Arguments("null", "null")]
			public async Task WhenJsonIsNoNumber_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNumber();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is a number,
					              but it was {kindString}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsNumber();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenJsonIsANumber_ShouldFail()
			{
				JsonElement? subject = FromString("2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a number,
					             but it was a number
					             """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("[]")]
			[Arguments("\"2\"")]
			[Arguments("true")]
			[Arguments("null")]
			public async Task WhenJsonIsNoNumber_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
