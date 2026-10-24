using System.Text.Json.Nodes;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class IsString
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenNodeIsAString_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create("foo");

				async Task Act()
					=> await That(subject).IsString();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("1", "a number")]
			[Arguments("false", "false")]
			[Arguments("null", "null")]
			public async Task WhenNodeIsNoString_ShouldFail(string json, string kindString)
			{
				JsonNode? subject = FromString(json);

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
				JsonValue? subject = null;

				async Task Act()
					=> await That(subject).IsString();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string,
					             but it was null
					             """)
					.Because("a null node is the JSON null");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNodeIsAString_ShouldFail()
			{
				JsonNode? subject = FromString("\"foo\"");

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
			public async Task WhenNodeIsNoString_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create(1);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonValue subject = JsonValue.Create("foo");

				async Task Act()
					=> await That(subject).IsString(s => s.IsEqualTo("FOO"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string which is equal to "FOO",
					             but it was "foo", which differs at index 0:
					                ↓ (actual)
					               "foo"
					               "FOO"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonValue subject = JsonValue.Create("foo");

				async Task Act()
					=> await That(subject).IsString(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNodeIsNoString_ShouldNotVerifyTheExpectations()
			{
				JsonValue subject = JsonValue.Create(1);

				async Task Act()
					=> await That(subject).IsString(s => s.IsEqualTo("1"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string which is equal to "1",
					             but it was a number
					             """);
			}
		}
	}
}
