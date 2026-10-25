using System.Text.Json.Nodes;
using aweXpect.Core;

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
					=> await That(subject).IsString((Action<IThatSubject<string>>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNegatedAndExpectationsAreMet_ShouldFail()
			{
				JsonValue subject = JsonValue.Create("Let It Be");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString(s => s.IsEqualTo("Let It Be")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a string or its value is not equal to "Let It Be",
					             but it was "Let It Be"
					             """);
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

					             Actual:
					             1
					             """);
			}
		}

		public sealed class WithValueTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				JsonValue subject = JsonValue.Create("foo");

				async Task Act()
					=> await That(subject).IsString((string)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNestedInHasProperty_ShouldNameThePathOfTheProperty()
			{
				JsonNode subject = new JsonObject { ["title"] = "Yesterday", };

				async Task Act()
					=> await That(subject).HasProperty("title", t => t.IsString("Let").AsPrefix());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "title" which is a string starting with "Let",
					             but $.title was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)

					             Actual ($.title):
					             "Yesterday"
					             """);
			}

			[Test]
			public async Task WhenNodeIsNoString_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(1);

				async Task Act()
					=> await That(subject).IsString("1");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string equal to "1",
					             but it was a number

					             Actual:
					             1
					             """);
			}

			[Test]
			public async Task WhenStringDiffers_ShouldFail()
			{
				JsonValue subject = JsonValue.Create("foo");

				async Task Act()
					=> await That(subject).IsString("FOO");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string equal to "FOO",
					             but it was "foo", which differs at index 0:
					                ↓ (actual)
					               "foo"
					               "FOO"
					                ↑ (expected)

					             Actual:
					             "foo"
					             """);
			}

			[Test]
			public async Task WhenStringIsEqual_IgnoringCase_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create("foo");

				async Task Act()
					=> await That(subject).IsString("FOO").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonValue? subject = null;

				async Task Act()
					=> await That(subject).IsString("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string equal to "foo",
					             but it was null

					             Actual:
					             null
					             """)
					.Because("a null node is the JSON null");
			}
		}

		public sealed class WithValueNegatedTests
		{
			[Test]
			public async Task WhenStringDiffers_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create("foo");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString("bar"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringIsEqual_ShouldFail()
			{
				JsonValue subject = JsonValue.Create("foo");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a string equal to "foo",
					             but it was "foo"

					             Actual:
					             "foo"
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				JsonValue? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString("foo"));

				await That(Act).DoesNotThrow()
					.Because("a null node is the JSON null, which is no string");
			}
		}
	}
}
