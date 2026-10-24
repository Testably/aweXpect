using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
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
				JsonElement? subject = FromString(json);

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
				JsonElement? subject = FromString(json);

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
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsString();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenJsonIsAString_ShouldFail()
			{
				JsonElement? subject = FromString("\"foo\"");

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
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
