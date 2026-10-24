using System.Text.Json;

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
					             """);
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
					=> await That(subject).IsNumber<string>();

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						"The number type string is not supported. Use byte, sbyte, short, ushort, int, uint, long, ulong, float, double or decimal.");
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
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotSupported_ShouldThrowNotSupportedException()
			{
				JsonElement subject = FromString("1");

				async Task Act()
					=> await That(subject).IsNumber<object>(n => n.IsNotNull());

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						"The number type object is not supported. Use byte, sbyte, short, ushort, int, uint, long, ulong, float, double or decimal.");
			}
		}
	}
}
