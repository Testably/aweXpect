using System.Text.Json.Nodes;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class IsNumber
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNodeIsANumber_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create(42.5);

				async Task Act()
					=> await That(subject).IsNumber();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("[]", "an array")]
			[InlineData("\"1\"", "a string")]
			[InlineData("true", "true")]
			[InlineData("null", "null")]
			public async Task WhenNodeIsNoNumber_ShouldFail(string json, string kindString)
			{
				JsonNode? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNumber();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is a number,
					              but it was {kindString}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenNodeIsANumber_ShouldFail()
			{
				JsonNode? subject = FromString("1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a number,
					             but it was
					             """);
			}

			[Fact]
			public async Task WhenNodeIsNoNumber_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create("1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
