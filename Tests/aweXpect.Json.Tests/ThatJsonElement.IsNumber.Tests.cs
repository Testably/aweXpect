using System.Numerics;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Json.Results;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsNumber
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("0")]
			[Arguments("-4.2e3")]
			[Arguments("123456789012345678901234567890")]
			public async Task WhenJsonIsANumber_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement[] items = [FromString("2"), FromString("\"2\""),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsNumber()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are a number for all items,
					             but only 1 of 2 were
					             *
					             """).AsWildcard()
					.Because("the items are the plural subject of the expectation");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenJsonIsANumber_ShouldFail()
			{
				JsonElement subject = FromString("2");

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString("4.52");

				async Task Act()
					=> await That(subject).IsNumber(n => n.IsEqualTo(4.5).Within(0.1));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonElement subject = FromString("4.75");

				async Task Act()
					=> await That(subject).IsNumber(n => n.IsEqualTo(4.5).Within(0.1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number which is equal to 4.5 ± 0.1,
					             but it was 4.75, which differs by 0.25
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("1");

				async Task Act()
					=> await That(subject).IsNumber(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectingAnIntegerLiteral_ShouldCompareTheNumberAsDouble()
			{
				JsonElement subject = FromString("5");

				async Task Act()
					=> await That(subject).IsNumber(n => n.IsEqualTo(5));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenJsonIsNoNumber_ShouldNotVerifyTheExpectations()
			{
				JsonElement subject = FromString("\"4.5\"");

				async Task Act()
					=> await That(subject).IsNumber(n => n.IsEqualTo(4.5));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number which is equal to 4.5,
					             but it was a string

					             Actual:
					             "4.5"
					             """);
			}

			[Test]
			public async Task WhenNegatedAndExpectationsAreMet_ShouldFail()
			{
				JsonElement subject = FromString("4.5");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(n => n.IsPositive()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a number or its value is not positive,
					             but it was 4.5
					             """);
			}

			[Test]
			public async Task WhenNegatedAndNumberDoesNotFitIntoADouble_ShouldFail()
			{
				JsonElement subject = FromString("1e400");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(n => n.IsPositive()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a number or its value is not positive,
					             but it was 1e400, which is not a double

					             Actual:
					             1e400
					             """)
					.Because("a number beyond the range of a double cannot be answered, so it fails both ways");
			}

			[Test]
			public async Task WhenNumberDoesNotFitIntoADouble_ShouldFail()
			{
				JsonElement subject = FromString("1e400");

				async Task Act()
					=> await That(subject).IsNumber(n => n.IsPositive());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number which is positive,
					             but it was 1e400, which is not a double

					             Actual:
					             1e400
					             """);
			}
		}

		public sealed class OfTypeTests
		{
			[Test]
			[Arguments("\"1\"", "a string")]
			[Arguments("null", "null")]
			[Arguments("{}", "an object")]
			public async Task WhenJsonIsNoNumber_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNumber<int>();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an int,
					              but it was {kindString}

					              Actual:
					              {json}
					              """);
			}

			[Test]
			[Arguments("256", "byte", "a byte")]
			[Arguments("128", "sbyte", "an sbyte")]
			[Arguments("32768", "short", "a short")]
			[Arguments("-1", "ushort", "a ushort")]
			[Arguments("1.5", "int", "an int")]
			[Arguments("4294967296", "uint", "a uint")]
			[Arguments("9223372036854775808", "long", "a long")]
			[Arguments("-1", "ulong", "a ulong")]
			[Arguments("1e39", "float", "a float")]
			[Arguments("1e400", "double", "a double")]
			[Arguments("1e30", "decimal", "a decimal")]
			[Arguments("1.5", "BigInteger", "a BigInteger")]
			[Arguments("1e2", "BigInteger", "a BigInteger")]
#if NET8_0_OR_GREATER
			[Arguments("9223372036854775808", "nint", "an nint")]
			[Arguments("-1", "nuint", "an nuint")]
			[Arguments("170141183460469231731687303715884105728", "Int128", "an Int128")]
			[Arguments("-1", "UInt128", "a UInt128")]
			[Arguments("1.5", "Int128", "an Int128")]
			[Arguments("1e5", "Half", "a Half")]
#endif
			public async Task WhenNumberDoesNotFit_ShouldFail(string json, string type, string expected)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await VerifyIsNumberOfType(subject, type);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is {expected},
					              but it was {json}, which is not {expected}

					              Actual:
					              {json}
					              """);
			}

			[Test]
			[Arguments("255", "byte")]
			[Arguments("-128", "sbyte")]
			[Arguments("-32768", "short")]
			[Arguments("65535", "ushort")]
			[Arguments("2147483647", "int")]
			[Arguments("4294967295", "uint")]
			[Arguments("-9223372036854775808", "long")]
			[Arguments("18446744073709551615", "ulong")]
			[Arguments("1.5", "float")]
			[Arguments("1.5e300", "double")]
			[Arguments("79228162514264337593543950335", "decimal")]
			[Arguments("-123456789012345678901234567890123456789012345678901234567890", "BigInteger")]
#if NET8_0_OR_GREATER
			[Arguments("-9223372036854775808", "nint")]
			[Arguments("18446744073709551615", "nuint")]
			[Arguments("-170141183460469231731687303715884105728", "Int128")]
			[Arguments("340282366920938463463374607431768211455", "UInt128")]
			[Arguments("1.5e4", "Half")]
#endif
			public async Task WhenNumberFits_ShouldSucceed(string json, string type)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await VerifyIsNumberOfType(subject, type);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotSupported_ShouldThrowNotSupportedException()
			{
				JsonElement subject = FromString("1");

				async Task Act()
					=> await That(subject).IsNumber<char>();

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						$"The number type char is not supported. Use {ThatJsonElement.SupportedNumberTypes}.");
			}

			private static async Task VerifyIsNumberOfType(JsonElement subject, string type)
			{
				switch (type)
				{
					case "byte":
						await That(subject).IsNumber<byte>();
						break;
					case "sbyte":
						await That(subject).IsNumber<sbyte>();
						break;
					case "short":
						await That(subject).IsNumber<short>();
						break;
					case "ushort":
						await That(subject).IsNumber<ushort>();
						break;
					case "int":
						await That(subject).IsNumber<int>();
						break;
					case "uint":
						await That(subject).IsNumber<uint>();
						break;
					case "long":
						await That(subject).IsNumber<long>();
						break;
					case "ulong":
						await That(subject).IsNumber<ulong>();
						break;
					case "float":
						await That(subject).IsNumber<float>();
						break;
					case "double":
						await That(subject).IsNumber<double>();
						break;
					case "BigInteger":
						await That(subject).IsNumber<BigInteger>();
						break;
#if NET8_0_OR_GREATER
					case "nint":
						await That(subject).IsNumber<nint>();
						break;
					case "nuint":
						await That(subject).IsNumber<nuint>();
						break;
					case "Int128":
						await That(subject).IsNumber<Int128>();
						break;
					case "UInt128":
						await That(subject).IsNumber<UInt128>();
						break;
					case "Half":
						await That(subject).IsNumber<Half>();
						break;
#endif
					default:
						await That(subject).IsNumber<decimal>();
						break;
				}
			}
		}

		public sealed class OfTypeNegatedTests
		{
			[Test]
			public async Task WhenNumberDoesNotFit_ShouldSucceed()
			{
				JsonElement subject = FromString("1.5");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber<int>());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberFits_ShouldFail()
			{
				JsonElement subject = FromString("5");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber<int>());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an int,
					             but it was 5

					             Actual:
					             5
					             """);
			}
		}

		public sealed class OfTypeWithExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsAreMet_ShouldSucceed()
			{
				JsonElement subject = FromString("243");

				async Task Act()
					=> await That(subject).IsNumber<long>(n => n.IsBetween(120).And(600));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonElement subject = FromString("9.98");

				async Task Act()
					=> await That(subject).IsNumber<decimal>(n => n.IsEqualTo(9.99m));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a decimal which is equal to 9.99,
					             but it was 9.98, which differs by -0.01
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("1");

				async Task Act()
					=> await That(subject).IsNumber<int>(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNegatedAndExpectationsAreMet_ShouldFail()
			{
				JsonElement subject = FromString("243");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber<int>(n => n.IsEqualTo(243)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an int or its value is not equal to 243,
					             but it was 243
					             """);
			}

			[Test]
			public async Task WhenNegatedAndNumberDoesNotFit_ShouldSucceed()
			{
				JsonElement subject = FromString("243.5");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber<int>(n => n.IsEqualTo(243)));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberDoesNotFit_ShouldNotVerifyTheExpectations()
			{
				JsonElement subject = FromString("{\"durationSeconds\": 1.5}");

				async Task Act()
					=> await That(subject).HasProperty("durationSeconds", p => p.IsNumber<int>(n => n.IsPositive()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "durationSeconds" which is an int which is positive,
					             but $.durationSeconds was 1.5, which is not an int

					             Actual ($.durationSeconds):
					             1.5
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotSupported_ShouldThrowNotSupportedException()
			{
				JsonElement subject = FromString("1");

				async Task Act()
					=> await That(subject).IsNumber<char>(n => n.IsEqualTo('a'));

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						$"The number type char is not supported. Use {ThatJsonElement.SupportedNumberTypes}.");
			}
		}

		public sealed class WithOperatorTests
		{
			[Test]
			public async Task WhenBetween_ShouldVerifyTheRange()
			{
				JsonElement subject = FromString("3");

				async Task Act()
					=> await That(subject).IsNumber().Between(1).And(5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("99", true)]
			[Arguments("98", false)]
			public async Task WhenEqualTo_WithinPercent_ShouldApplyTheRelativeTolerance(string json, bool isWithin)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNumber().EqualTo(100).Within(1).Percent();

				await That(Act).Throws<FailException>().OnlyIf(!isWithin)
					.WithMessage($"""
					              Expected that subject
					              is a number equal to 100.0 ± 1.0%,
					              but it was {json}

					              Actual:
					              98
					              """);
			}

			[Test]
			[Arguments("4.55", true)]
			[Arguments("4.65", false)]
			public async Task WhenEqualTo_WithinTolerance_ShouldApplyTheTolerance(string json, bool isWithin)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNumber().EqualTo(4.5).Within(0.1);

				await That(Act).Throws<FailException>().OnlyIf(!isWithin)
					.WithMessage($"""
					              Expected that subject
					              is a number equal to 4.5 ± 0.1,
					              but it was {json}

					              Actual:
					              4.65
					              """);
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("3");

				async Task Act()
					=> await That(subject).IsNumber().LessThan(null);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expected' cannot be null.").AsPrefix()
					.Because("a JSON number is never null, so nothing can be compared with a null bound");
			}

			[Test]
			public async Task WhenGreaterThan_AndNumberIsNotGreater_ShouldFail()
			{
				JsonElement subject = FromString("2");

				async Task Act()
					=> await That(subject).IsNumber().GreaterThan(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number greater than 3.0,
					             but it was 2

					             Actual:
					             2
					             """);
			}

			[Test]
			[Arguments("\"3\"", "a string")]
			[Arguments("null", "null")]
			[Arguments("true", "true")]
			public async Task WhenJsonIsNoNumber_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsNumber().GreaterThan(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is a number greater than 1.0,
					              but it was {kindString}

					              Actual:
					              {json}
					              """);
			}

			[Test]
			public async Task WhenNestedInHasProperty_ShouldNameThePathOfTheProperty()
			{
				JsonElement subject = FromString("{\"rating\": 3.5}");

				async Task Act()
					=> await That(subject).HasProperty("rating", r => r.IsNumber().GreaterThan(4));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "rating" which is a number greater than 4.0,
					             but $.rating was 3.5

					             Actual ($.rating):
					             3.5
					             """);
			}

			[Test]
			public async Task WhenNotGreaterThan_AndNumberIsGreater_ShouldFail()
			{
				JsonElement subject = FromString("5");

				async Task Act()
					=> await That(subject).IsNumber().NotGreaterThan(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number not greater than 3.0,
					             but it was 5

					             Actual:
					             5
					             """);
			}

			[Test]
			public async Task WhenNumberDoesNotFitIntoADouble_ShouldFail()
			{
				JsonElement subject = FromString("1e400");

				async Task Act()
					=> await That(subject).IsNumber().GreaterThan(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a number greater than 1.0,
					             but it was 1e400, which is not a double

					             Actual:
					             1e400
					             """);
			}

			[Test]
			public async Task WhenNumberDoesNotFitIntoADoubleWithoutAnOperator_ShouldSucceed()
			{
				JsonElement subject = FromString("1e400");

				async Task Act()
					=> await That(subject).IsNumber();

				await That(Act).DoesNotThrow()
					.Because("the value is only read for a comparison");
			}

			[Test]
			public async Task WhenNumberMatches_ShouldContinueOnTheElement()
			{
				JsonElement subject = FromString("3");

				JsonElement result = await That(subject).IsNumber().GreaterThan(2).And.IsNumber().LessThan(4);

				await That(result.GetDouble()).IsEqualTo(3.0);
			}

			[Test]
			public async Task WhenOperatorIsCalledTwice_ShouldThrowInvalidOperationException()
			{
				JsonElement subject = FromString("3");

				async Task Act()
					=> await That(subject).CompliesWith(it =>
					{
						IsNumberResult<JsonElement, IThat<JsonElement>> result = it.IsNumber();
						result.GreaterThan(1);
						result.EqualTo(3);
					});

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("IsNumber() accepts only one comparison, so call only one operator on its result.");
			}
		}

		public sealed class WithOperatorNegatedTests
		{
			[Test]
			public async Task WhenJsonIsNoNumber_ShouldSucceed()
			{
				JsonElement subject = FromString("\"3\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber().GreaterThan(1));

				await That(Act).DoesNotThrow()
					.Because("a value of another kind is no number greater than the bound, like for IsNumber()");
			}

			[Test]
			public async Task WhenNumberDoesNotFitIntoADouble_ShouldFail()
			{
				JsonElement subject = FromString("1e400");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber().GreaterThan(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a number greater than 3.0,
					             but it was 1e400, which is not a double

					             Actual:
					             1e400
					             """)
					.Because("a number beyond the range of a double cannot be answered, so it fails both ways");
			}

			[Test]
			public async Task WhenNumberDoesNotMatch_ShouldSucceed()
			{
				JsonElement subject = FromString("2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber().GreaterThan(3));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberMatches_ShouldFail()
			{
				JsonElement subject = FromString("5");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber().GreaterThan(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a number greater than 3.0,
					             but it was 5

					             Actual:
					             5
					             """);
			}
		}

		public sealed class WithValueTests
		{
			[Test]
			[Arguments("sbyte", "-128", "an sbyte equal to 127 ± 1")]
			[Arguments("short", "-32768", "a short equal to 32767 ± 1")]
			[Arguments("int", "-2147483648", "an int equal to 2147483647 ± 1")]
			[Arguments("long", "-9223372036854775808", "a long equal to 9223372036854775807 ± 1")]
			[Arguments("decimal", "-79228162514264337593543950335", "a decimal equal to decimal.MaxValue ± 1.0")]
#if NET8_0_OR_GREATER
			[Arguments("nint", "-9223372036854775808", "an nint equal to 9223372036854775807 ± 1")]
			[Arguments("Int128", "-170141183460469231731687303715884105728",
				"an Int128 equal to 170141183460469231731687303715884105727 ± 1")]
#endif
			public async Task WhenDifferenceOverflows_ShouldFail(string type, string minValue, string expectation)
			{
				JsonElement subject = FromString(minValue);

				async Task Act()
					=> await VerifyIsMaxValueWithinOne(subject, type);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is {expectation},
					              but it was {minValue}

					              Actual:
					              {minValue}
					              """)
					.Because("a difference beyond the range of the number type is not within any tolerance");
			}

			[Test]
			public async Task WhenExpectedIsInfiniteAndToleranceIsRelative_ShouldFail()
			{
				JsonElement subject = FromString("42");

				async Task Act()
					=> await That(subject).IsNumber(double.PositiveInfinity).Within(5).Percent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a double equal to +∞ ± 5.0%,
					             but it was 42

					             Actual:
					             42
					             """)
					.Because("an infinite expected value makes the relative tolerance infinite as well, but a finite number still has no finite distance to it");
			}

			[Test]
			[Arguments("double", "PositiveInfinity", "a double equal to +∞ ± +∞")]
			[Arguments("double", "NegativeInfinity", "a double equal to -∞ ± +∞")]
			[Arguments("double", "NaN", "a double equal to NaN ± +∞")]
			[Arguments("float", "PositiveInfinity", "a float equal to +∞ ± +∞")]
			[Arguments("float", "NegativeInfinity", "a float equal to -∞ ± +∞")]
			[Arguments("float", "NaN", "a float equal to NaN ± +∞")]
#if NET8_0_OR_GREATER
			[Arguments("Half", "PositiveInfinity", "a Half equal to +∞ ± +∞")]
			[Arguments("Half", "NegativeInfinity", "a Half equal to -∞ ± +∞")]
			[Arguments("Half", "NaN", "a Half equal to NaN ± +∞")]
#endif
			public async Task WhenExpectedIsNotFinite_ShouldFail(string type, string value, string expectation)
			{
				JsonElement subject = FromString("42");

				async Task Act()
					=> await VerifyIsNotFiniteWithinInfinity(subject, type, value);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is {expectation},
					              but it was 42

					              Actual:
					              42
					              """)
					.Because("no tolerance can bridge the distance to a non-finite value, like for the number expectations");
			}

			[Test]
			public async Task WhenExpectingADoubleLiteral_ShouldAcceptAnInteger()
			{
				JsonElement subject = FromString("42");

				async Task Act()
					=> await That(subject).IsNumber(42.0);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectingAnIntegerLiteral_ShouldNotAcceptAFraction()
			{
				JsonElement subject = FromString("42.0");

				async Task Act()
					=> await That(subject).IsNumber(42);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int equal to 42,
					             but it was 42.0, which is not an int

					             Actual:
					             42.0
					             """)
					.Because("the number type follows the expected value");
			}

			[Test]
			public async Task WhenExpectingAnIntegerLiteralForAnExplicitType_ShouldCompareAsThatType()
			{
				JsonElement subject = FromString("42.0");

				async Task Act()
					=> await That(subject).IsNumber<double>(42);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenJsonIsNoNumber_ShouldFail()
			{
				JsonElement subject = FromString("\"42\"");

				async Task Act()
					=> await That(subject).IsNumber(42);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int equal to 42,
					             but it was a string

					             Actual:
					             "42"
					             """);
			}

			[Test]
			public async Task WhenLongDiffersBeyondThePrecisionOfADouble_ShouldFail()
			{
				JsonElement subject = FromString("9007199254740993");

				async Task Act()
					=> await That(subject).IsNumber(9007199254740992L);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a long equal to 9007199254740992,
					             but it was 9007199254740993

					             Actual:
					             9007199254740993
					             """)
					.Because("a long is compared as long and not as double, which cannot tell these values apart");
			}

			[Test]
			public async Task WhenNestedInHasProperty_ShouldNameThePathOfTheProperty()
			{
				JsonElement subject = FromString("{\"durationSeconds\": 243}");

				async Task Act()
					=> await That(subject).HasProperty("durationSeconds", d => d.IsNumber(242));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "durationSeconds" which is an int equal to 242,
					             but $.durationSeconds was 243

					             Actual ($.durationSeconds):
					             243
					             """);
			}

			[Test]
			public async Task WhenNumberDiffers_ShouldFail()
			{
				JsonElement subject = FromString("43");

				async Task Act()
					=> await That(subject).IsNumber(42);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an int equal to 42,
					             but it was 43

					             Actual:
					             43
					             """);
			}

			[Test]
			public async Task WhenNumberDoesNotFit_ShouldFail()
			{
				JsonElement subject = FromString("300");

				async Task Act()
					=> await That(subject).IsNumber((byte)42);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a byte equal to 42,
					             but it was 300, which is not a byte

					             Actual:
					             300
					             """);
			}

			[Test]
			public async Task WhenNumberIsEqual_ShouldSucceed()
			{
				JsonElement subject = FromString("42");

				async Task Act()
					=> await That(subject).IsNumber(42);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberIsOutsideTheTolerance_ShouldFail()
			{
				JsonElement subject = FromString("43.5");

				async Task Act()
					=> await That(subject).IsNumber(42.0).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a double equal to 42.0 ± 1.0,
					             but it was 43.5

					             Actual:
					             43.5
					             """);
			}

			[Test]
			public async Task WhenNumberIsWithinTheRelativeTolerance_ShouldSucceed()
			{
				JsonElement subject = FromString("209.5");

				async Task Act()
					=> await That(subject).IsNumber(200.0).Within(5).Percent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("byte")]
			[Arguments("sbyte")]
			[Arguments("short")]
			[Arguments("ushort")]
			[Arguments("int")]
			[Arguments("uint")]
			[Arguments("long")]
			[Arguments("ulong")]
			[Arguments("float")]
			[Arguments("double")]
			[Arguments("decimal")]
			[Arguments("BigInteger")]
#if NET8_0_OR_GREATER
			[Arguments("nint")]
			[Arguments("nuint")]
			[Arguments("Int128")]
			[Arguments("UInt128")]
			[Arguments("Half")]
#endif
			public async Task WhenNumberIsWithinTheTolerance_ShouldSucceed(string type)
			{
				JsonElement subject = FromString("43");

				async Task Act()
					=> await VerifyIsNumberWithin(subject, type);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotSupported_ShouldThrowNotSupportedException()
			{
				JsonElement subject = FromString("1");

				async Task Act()
					=> await That(subject).IsNumber('a');

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						$"The number type char is not supported. Use {ThatJsonElement.SupportedNumberTypes}.");
			}

			private static async Task VerifyIsMaxValueWithinOne(JsonElement subject, string type)
			{
				switch (type)
				{
					case "sbyte":
						await That(subject).IsNumber(sbyte.MaxValue).Within(1);
						break;
					case "short":
						await That(subject).IsNumber(short.MaxValue).Within(1);
						break;
					case "int":
						await That(subject).IsNumber(int.MaxValue).Within(1);
						break;
					case "long":
						await That(subject).IsNumber(long.MaxValue).Within(1);
						break;
#if NET8_0_OR_GREATER
					case "nint":
						await That(subject).IsNumber(nint.MaxValue).Within(1);
						break;
					case "Int128":
						await That(subject).IsNumber(Int128.MaxValue).Within(1);
						break;
#endif
					default:
						await That(subject).IsNumber(decimal.MaxValue).Within(1);
						break;
				}
			}

			private static async Task VerifyIsNotFiniteWithinInfinity(JsonElement subject, string type, string value)
			{
				switch (type)
				{
					case "float":
						await That(subject).IsNumber(value switch
						{
							"PositiveInfinity" => float.PositiveInfinity,
							"NegativeInfinity" => float.NegativeInfinity,
							_ => float.NaN,
						}).Within(float.PositiveInfinity);
						break;
#if NET8_0_OR_GREATER
					case "Half":
						await That(subject).IsNumber(value switch
						{
							"PositiveInfinity" => Half.PositiveInfinity,
							"NegativeInfinity" => Half.NegativeInfinity,
							_ => Half.NaN,
						}).Within(Half.PositiveInfinity);
						break;
#endif
					default:
						await That(subject).IsNumber(value switch
						{
							"PositiveInfinity" => double.PositiveInfinity,
							"NegativeInfinity" => double.NegativeInfinity,
							_ => double.NaN,
						}).Within(double.PositiveInfinity);
						break;
				}
			}

			private static async Task VerifyIsNumberWithin(JsonElement subject, string type)
			{
				switch (type)
				{
					case "byte":
						await That(subject).IsNumber((byte)42).Within(1);
						break;
					case "sbyte":
						await That(subject).IsNumber((sbyte)42).Within(1);
						break;
					case "short":
						await That(subject).IsNumber((short)42).Within(1);
						break;
					case "ushort":
						await That(subject).IsNumber((ushort)42).Within(1);
						break;
					case "int":
						await That(subject).IsNumber(42).Within(1);
						break;
					case "uint":
						await That(subject).IsNumber(42U).Within(1);
						break;
					case "long":
						await That(subject).IsNumber(42L).Within(1);
						break;
					case "ulong":
						await That(subject).IsNumber(42UL).Within(1);
						break;
					case "float":
						await That(subject).IsNumber(42F).Within(1);
						break;
					case "double":
						await That(subject).IsNumber(42.0).Within(1);
						break;
					case "BigInteger":
						await That(subject).IsNumber(new BigInteger(42)).Within(1);
						break;
#if NET8_0_OR_GREATER
					case "nint":
						await That(subject).IsNumber((nint)42).Within(1);
						break;
					case "nuint":
						await That(subject).IsNumber((nuint)42).Within(1);
						break;
					case "Int128":
						await That(subject).IsNumber((Int128)42).Within(1);
						break;
					case "UInt128":
						await That(subject).IsNumber((UInt128)42).Within(1);
						break;
					case "Half":
						await That(subject).IsNumber((Half)42).Within((Half)1);
						break;
#endif
					default:
						await That(subject).IsNumber(42M).Within(1);
						break;
				}
			}
		}

		public sealed class WithValueNegatedTests
		{
			[Test]
			public async Task WhenJsonIsNoNumber_ShouldSucceed()
			{
				JsonElement subject = FromString("\"42\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(42));

				await That(Act).DoesNotThrow()
					.Because("a value of another kind is no number equal to the expected value, like for IsNumber()");
			}

			[Test]
			public async Task WhenNumberDiffers_ShouldSucceed()
			{
				JsonElement subject = FromString("43");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(42));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNumberDoesNotFit_ShouldSucceed()
			{
				JsonElement subject = FromString("42.5");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(42));

				await That(Act).DoesNotThrow()
					.Because("a number that does not fit into the number type is no int equal to it, like for IsNumber<int>()");
			}

			[Test]
			public async Task WhenNumberIsEqual_ShouldFail()
			{
				JsonElement subject = FromString("42");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(42));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an int equal to 42,
					             but it was 42

					             Actual:
					             42
					             """);
			}

			[Test]
			public async Task WhenNumberIsWithinTheTolerance_ShouldFail()
			{
				JsonElement subject = FromString("43");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNumber(42).Within(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an int equal to 42 ± 1,
					             but it was 43

					             Actual:
					             43
					             """);
			}
		}
	}
}
