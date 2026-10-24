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

		public sealed class WithExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsAreMet_ShouldSucceed()
			{
				JsonElement? subject = FromString("\"foo\"");

				async Task Act()
					=> await That(subject).IsString(s => s.IsEqualTo("foo"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonElement? subject = FromString("\"foo\"");

				async Task Act()
					=> await That(subject).IsString(s => s.HasLength(4));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string which has length equal to 4,
					             but it had length 3
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("\"foo\"");

				async Task Act()
					=> await That(subject).IsString(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsString(s => s.IsEqualTo("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string which is equal to "foo",
					             but it was <null>
					             """);
			}
		}
	}
}
