using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
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
				JsonElement? subject = FromString(json);

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
				JsonElement? subject = FromString(json);

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
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsBoolean();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("true")]
			[Arguments("false")]
			public async Task WhenJsonIsABoolean_ShouldFail(string json)
			{
				JsonElement? subject = FromString(json);

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
				JsonElement? subject = FromString(json);

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
				JsonElement? subject = FromString("true");

				async Task Act()
					=> await That(subject).IsBoolean(b => b.IsTrue());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("true");

				async Task Act()
					=> await That(subject).IsBoolean(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsBoolean(b => b.IsTrue());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean which is True,
					             but it was <null>
					             """);
			}
		}

		public sealed class WithValueTests
		{
			[Test]
			public async Task WhenBooleanDiffers_ShouldFail()
			{
				JsonElement? subject = FromString("true");

				async Task Act()
					=> await That(subject).IsBoolean(false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean equal to False,
					             but it was True

					             Actual:
					             true
					             """);
			}

			[Test]
			public async Task WhenBooleanIsEqual_ShouldSucceed()
			{
				JsonElement? subject = FromString("false");

				async Task Act()
					=> await That(subject).IsBoolean(false);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsBoolean(true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a boolean equal to True,
					             but it was <null>
					             """);
			}
		}

		public sealed class WithValueNegatedTests
		{
			[Test]
			public async Task WhenBooleanDiffers_ShouldSucceed()
			{
				JsonElement? subject = FromString("true");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean(false));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBooleanIsEqual_ShouldFail()
			{
				JsonElement? subject = FromString("false");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean(false));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a boolean equal to False,
					             but it was False

					             Actual:
					             false
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBoolean(true));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a boolean equal to True,
					             but it was <null>
					             """)
					.Because("a null subject fails both ways");
			}
		}
	}
}
