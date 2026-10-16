using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class IsNumber
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("0")]
			[InlineData("-4.2e3")]
			public async Task WhenJsonIsANumber_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNumber();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("[]", "an array")]
			[InlineData("\"2\"", "a string")]
			[InlineData("true", "true")]
			[InlineData("false", "false")]
			[InlineData("null", "null")]
			public async Task WhenJsonIsNoNumber_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNumber();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is a number,
					              but it was {kindString}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsNumber();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a number,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenJsonIsANumber_ShouldFail()
			{
				JsonElement? subject = FromString("2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a number,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("{}")]
			[InlineData("[]")]
			[InlineData("\"2\"")]
			[InlineData("true")]
			[InlineData("null")]
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
