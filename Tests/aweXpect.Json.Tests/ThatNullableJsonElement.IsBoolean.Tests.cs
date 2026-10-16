using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class IsBoolean
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("true")]
			[InlineData("false")]
			public async Task WhenJsonIsABoolean_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsBoolean();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("{}", "an object")]
			[InlineData("[]", "an array")]
			[InlineData("1", "a number")]
			[InlineData("\"true\"", "a string")]
			[InlineData("null", "null")]
			public async Task WhenJsonIsNoBoolean_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsBoolean();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is a boolean,
					              but it was {kindString}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsBoolean();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("true")]
			[InlineData("false")]
			public async Task WhenJsonIsABoolean_ShouldFail(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a boolean,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("{}")]
			[InlineData("[]")]
			[InlineData("1")]
			[InlineData("\"true\"")]
			[InlineData("null")]
			public async Task WhenJsonIsNoBoolean_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
