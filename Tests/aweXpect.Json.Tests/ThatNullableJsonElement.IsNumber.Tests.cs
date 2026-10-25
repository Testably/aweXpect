using System.Numerics;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Json.Results;

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

		public sealed class WithExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsAreMet_ShouldSucceed()
			{
				JsonElement? subject = FromString("2.5");

				async Task Act()
					=> await That(subject).IsNumber(n => n.IsGreaterThan(2));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("1");

				async Task Act()
					=> await That(subject).IsNumber(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsNumber(n => n.IsGreaterThan(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number which is greater than 2.0,
					             but it was <null>
					             """);
			}
		}

		public sealed class OfTypeTests
		{
			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonElement? subject = FromString("300");

				async Task Act()
					=> await That(subject).IsNumber<short>(n => n.IsLessThan((short)200));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a short which is less than 200,
					             but it was 300, which differs by 100
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("1");

				async Task Act()
					=> await That(subject).IsNumber<int>(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNumberDoesNotFit_ShouldFail()
			{
				JsonElement? subject = FromString("300");

				async Task Act()
					=> await That(subject).IsNumber<byte>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a byte,
					             but it was 300, which is not a byte

					             Actual:
					             300
					             """);
			}

			[Test]
			public async Task WhenNumberFits_ShouldSucceed()
			{
				JsonElement? subject = FromString("300");

				async Task Act()
					=> await That(subject).IsNumber<ushort>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsNumber<int>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotSupported_ShouldThrowNotSupportedException()
			{
				JsonElement? subject = FromString("1");

				async Task Act()
					=> await That(subject).IsNumber<char>();

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						$"The number type char is not supported. Use {ThatJsonElement.SupportedNumberTypes}.");
			}
		}

		public sealed class WithOperatorTests
		{
			[Test]
			public async Task WhenJsonIsNoNumber_ShouldFail()
			{
				JsonElement? subject = FromString("\"4.5\"");

				async Task Act()
					=> await That(subject).IsNumber().EqualTo(4.5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number equal to 4.5,
					             but it was a string
					             """);
			}

			[Test]
			public async Task WhenNumberDoesNotMatch_ShouldFail()
			{
				JsonElement? subject = FromString("6");

				async Task Act()
					=> await That(subject).IsNumber().Between(1).And(5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number between 1.0 and 5.0,
					             but it was 6
					             """);
			}

			[Test]
			public async Task WhenNumberMatches_ShouldReturnTheNotNullableElement()
			{
				JsonElement? subject = FromString("4.55");

				JsonElement result = await That(subject).IsNumber().EqualTo(4.5).Within(0.1);

				await That(result.GetDouble()).IsEqualTo(4.55);
			}

			[Test]
			public async Task WhenOperatorIsCalledTwice_ShouldThrowInvalidOperationException()
			{
				JsonElement? subject = FromString("3");

				async Task Act()
					=> await That(subject).CompliesWith(it =>
					{
						IsNumberResult<JsonElement, IThat<JsonElement?>> result = it.IsNumber();
						result.NotEqualTo(2);
						result.LessThan(4);
					});

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("IsNumber() accepts only one comparison, so call only one operator on its result.");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsNumber().GreaterThan(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number greater than 1.0,
					             but it was <null>
					             """);
			}
		}

		public sealed class WithOperatorNegatedTests
		{
			[Test]
			public async Task WhenJsonIsNoNumber_ShouldSucceed()
			{
				JsonElement? subject = FromString("\"4.5\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber().EqualTo(4.5));

				await That(Act).DoesNotThrow()
					.Because("a value of another kind is no number equal to the expected value, like for IsNumber()");
			}

			[Test]
			public async Task WhenNumberMatches_ShouldFail()
			{
				JsonElement? subject = FromString("4.5");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber().NotEqualTo(4));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a number not equal to 4.0,
					             but it was 4.5
					             """);
			}
		}

		public sealed class WithValueTests
		{
			[Test]
			public async Task WhenNumberDoesNotFit_ShouldFail()
			{
				JsonElement? subject = FromString("1.5");

				async Task Act()
					=> await That(subject).IsNumber(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int equal to 2,
					             but it was 1.5, which is not an int

					             Actual:
					             1.5
					             """);
			}

			[Test]
			public async Task WhenNumberIsOutsideTheTolerance_ShouldFail()
			{
				JsonElement? subject = FromString("1.5");

				async Task Act()
					=> await That(subject).IsNumber(2.0).Within(0.25);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a double equal to 2.0 ± 0.25,
					             but it was 1.5

					             Actual:
					             1.5
					             """);
			}

			[Test]
			public async Task WhenNumberIsWithinTheTolerance_ShouldSucceed()
			{
				JsonElement? subject = FromString("1.5");

				async Task Act()
					=> await That(subject).IsNumber(2.0).Within(0.5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsNumber(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int equal to 1,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotSupported_ShouldThrowNotSupportedException()
			{
				JsonElement? subject = FromString("1");

				async Task Act()
					=> await That(subject).IsNumber('a');

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						$"The number type char is not supported. Use {ThatJsonElement.SupportedNumberTypes}.");
			}
		}

		public sealed class WithValueNegatedTests
		{
			[Test]
			public async Task WhenNumberDiffers_ShouldSucceed()
			{
				JsonElement? subject = FromString("1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(2));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberIsEqual_ShouldFail()
			{
				JsonElement? subject = FromString("1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an int equal to 1,
					             but it was 1

					             Actual:
					             1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an int equal to 1,
					             but it was <null>
					             """)
					.Because("a null subject fails both ways");
			}
		}
	}
}
