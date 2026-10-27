using System.Numerics;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Json.Results;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class IsNumber
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenNodeIsANumber_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create(42.5);

				async Task Act()
					=> await That(subject).IsNumber();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("\"1\"", "a string")]
			[Arguments("true", "true")]
			[Arguments("null", "null")]
			public async Task WhenNodeIsNoNumber_ShouldFail(string json, string kindString)
			{
				JsonNode? subject = FromString(json);

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
			public async Task WhenNodeTypeIsGivenExplicitly_ShouldVerifyTheKind()
			{
				JsonNode? subject = FromString("\"1\"");

				async Task Act()
					=> await That(subject).IsNumber<JsonNode>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number,
					             but it was a string
					             """)
					.Because("a node type is no number type, so that the overload for the kind of the node applies");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNodeIsANumber_ShouldFail()
			{
				JsonNode? subject = FromString("1");

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
			public async Task WhenNodeIsNoNumber_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create("1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(42.5);

				async Task Act()
					=> await That(subject).IsNumber(n => n.IsLessThan(10));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number which is less than 10.0,
					             but it was 42.5, which differs by 32.5
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonValue subject = JsonValue.Create(1);

				async Task Act()
					=> await That(subject).IsNumber(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).IsNumber(n => n.IsLessThan(10));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number which is less than 10.0,
					             but it was null

					             Actual:
					             null
					             """);
			}
		}

		public sealed class OfTypeTests
		{
			[Test]
			public async Task WhenExpectationsAreMet_ShouldSucceed()
			{
				JsonNode? subject = FromString("{\"durationSeconds\": 243}");

				async Task Act()
					=> await That(subject).HasProperty("durationSeconds", d => d.IsNumber<long>(n => n.IsEqualTo(243)));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonValue subject = JsonValue.Create(1);

				async Task Act()
					=> await That(subject).IsNumber<int>(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNodeHoldsAnotherNumberType_ShouldReadTheValue()
			{
				JsonValue subject = JsonValue.Create(5L);

				async Task Act()
					=> await That(subject).IsNumber<int>(n => n.IsEqualTo(5));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberDoesNotFit_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(1.5);

				async Task Act()
					=> await That(subject).IsNumber<int>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int,
					             but it was 1.5, which is not an int

					             Actual:
					             1.5
					             """);
			}

			[Test]
			public async Task WhenSubjectIsAJsonValue_ShouldKeepItsType()
			{
				JsonValue duration = JsonValue.Create(243);

				JsonValue result = await That(duration).IsNumber<int>().And.IsNumber<long>(n => n.IsPositive());

				await That(result).IsSameAs(duration);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).IsNumber<int>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int,
					             but it was null

					             Actual:
					             null
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotSupported_ShouldThrowNotSupportedException()
			{
				JsonValue subject = JsonValue.Create(1);

				async Task Act()
					=> await That(subject).IsNumber<char>();

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						$"The number type char is not supported. Use {ThatJsonElement.SupportedNumberTypes}.");
			}
		}

		public sealed class OfTypeNegatedTests
		{
			[Test]
			public async Task WhenNumberDoesNotFit_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create(1.5);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber<int>());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberFits_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(243);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber<int>());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an int,
					             but it was 243

					             Actual:
					             243
					             """);
			}
		}

		public sealed class WithOperatorTests
		{
			[Test]
			public async Task WhenNodeIsNoNumber_ShouldFail()
			{
				JsonNode? subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsNumber().GreaterThanOrEqualTo(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number greater than or equal to 0.0,
					             but it was an object

					             Actual:
					             {}
					             """);
			}

			[Test]
			public async Task WhenNumberDoesNotMatch_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(98);

				async Task Act()
					=> await That(subject).IsNumber().EqualTo(100).Within(1).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number equal to 100.0 ± 1.0%,
					             but it was 98

					             Actual:
					             98
					             """);
			}

			[Test]
			public async Task WhenNumberMatches_ShouldReturnTheNode()
			{
				JsonValue subject = JsonValue.Create(4.5);

				JsonValue result = await That(subject).IsNumber().NotBetween(1).And(4);

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenOperatorIsCalledTwice_ShouldThrowInvalidOperationException()
			{
				JsonValue subject = JsonValue.Create(3);

				async Task Act()
					=> await That(subject).CompliesWith(it =>
					{
						IsNumberResult<JsonValue, IThat<JsonValue?>> result = it.IsNumber();
						result.NotLessThan(1);
						result.NotLessThanOrEqualTo(2);
					});

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("IsNumber() accepts only one comparison, so call only one operator on its result.");
			}
		}

		public sealed class WithOperatorNegatedTests
		{
			[Test]
			public async Task WhenNodeIsNoNumber_ShouldSucceed()
			{
				JsonNode? subject = FromString("\"3\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber().LessThanOrEqualTo(3));

				await That(Act).DoesNotThrow()
					.Because("a value of another kind is no number less than or equal to the bound, like for IsNumber()");
			}

			[Test]
			public async Task WhenNumberMatches_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber().NotGreaterThanOrEqualTo(4));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a number not greater than or equal to 4.0,
					             but it was 3

					             Actual:
					             3
					             """);
			}
		}

		public sealed class WithValueTests
		{
			[Test]
			public async Task WhenNestedInHasProperty_ShouldNameThePathOfTheProperty()
			{
				JsonNode subject = new JsonObject { ["durationSeconds"] = 243, };

				async Task Act()
					=> await That(subject).HasProperty("durationSeconds", d => d.IsNumber(240).Within(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "durationSeconds" which is an int equal to 240 ± 2,
					             but $.durationSeconds was 243

					             Actual ($.durationSeconds):
					             243
					             """);
			}

			[Test]
			public async Task WhenNodeHoldsAnotherNumberType_ShouldReadTheValue()
			{
				JsonValue subject = JsonValue.Create(5L);

				async Task Act()
					=> await That(subject).IsNumber(5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNodeIsNoNumber_ShouldFail()
			{
				JsonValue subject = JsonValue.Create("5");

				async Task Act()
					=> await That(subject).IsNumber(5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int equal to 5,
					             but it was a string

					             Actual:
					             "5"
					             """);
			}

			[Test]
			public async Task WhenNumberDoesNotFit_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(1.5);

				async Task Act()
					=> await That(subject).IsNumber(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int equal to 1,
					             but it was 1.5, which is not an int

					             Actual:
					             1.5
					             """);
			}

			[Test]
			public async Task WhenSubjectIsAJsonValue_ShouldKeepItsType()
			{
				JsonValue duration = JsonValue.Create(243);

				JsonValue result = await That(duration).IsNumber<double>(243).Within(0.5).And.IsNumber(243);

				await That(result).IsSameAs(duration);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).IsNumber(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int equal to 1,
					             but it was null

					             Actual:
					             null
					             """)
					.Because("a null node is the JSON null");
			}

			[Test]
			public async Task WhenTypeIsNotSupported_ShouldThrowNotSupportedException()
			{
				JsonValue subject = JsonValue.Create(1);

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
				JsonValue subject = JsonValue.Create(1);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(2));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberDoesNotFit_ShouldSucceed()
			{
				JsonValue subject = JsonValue.Create(1.5);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(1));

				await That(Act).DoesNotThrow()
					.Because("a number that does not fit into the number type is no int equal to it, like for IsNumber<int>()");
			}

			[Test]
			public async Task WhenNumberIsEqual_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(1);

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
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(1));

				await That(Act).DoesNotThrow()
					.Because("a null node is the JSON null, which is no number");
			}
		}
	}
}
