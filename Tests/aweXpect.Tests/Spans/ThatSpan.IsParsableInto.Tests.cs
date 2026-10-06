#if NET8_0_OR_GREATER
using System.Globalization;

namespace aweXpect.Tests;

public sealed partial class ThatSpan
{
	public sealed partial class IsParsableInto
	{
		public sealed class Tests
		{
			[Test]
			public async Task InAndChain_WhenLaterOperandsFail_ShouldExplainEachOfThem()
			{
				async Task Act()
					=> await That("5".AsSpan()).IsParsableInto<int>().And.IsParsableInto<Guid>().And.IsParsableInto<DateTime>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that "5".AsSpan()
					             is parsable into int and is parsable into Guid and is parsable into DateTime,
					             but Parse of Guid did throw a FormatException:
					               Unrecognized Guid format.
					             and Parse of DateTime did throw a FormatException:
					               String '5' was not recognized as a valid DateTime.
					             """)
					.Because("a successful parse must not hide the failing operands that follow it");
			}

			[Test]
			public async Task InOr_WhenBothOperandsFail_ShouldExplainBoth()
			{
				async Task Act()
					=> await That("abc".AsSpan()).IsParsableInto<int>().Or.IsParsableInto<Guid>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that "abc".AsSpan()
					             is parsable into int or is parsable into Guid,
					             but Parse of int did throw a FormatException:
					               The input string 'abc' was not in a correct format.
					             and Parse of Guid did throw a FormatException:
					               Unrecognized Guid format.
					             """)
					.Because("each alternative failed on its own, so both explain the failure");
			}

			[Test]
			public async Task WhenSpanIsNotParsable_ShouldFail()
			{
				async Task Act()
					=> await That("abc".AsSpan()).IsParsableInto<int>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that "abc".AsSpan()
					             is parsable into int,
					             but Parse of int did throw a FormatException:
					               The input string 'abc' was not in a correct format.
					             """).And
					.Whose(e => e.InnerException, i => i.Is<FormatException>())
					.Because("the exception of the parser tells why the span is not parsable");
			}

			[Test]
			public async Task WhenSpanIsParsable_ShouldSucceed()
			{
				async Task Act()
					=> await That("42".AsSpan()).IsParsableInto<int>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-12.34", "de-AT")]
			[Arguments("-12,34", "en-US")]
			public async Task WithFormatProvider_WhenFormatDoesNotMatch_ShouldFail(string subject, string cultureName)
			{
				IFormatProvider formatProvider = new CultureInfo(cultureName);

				async Task Act()
					=> await That(subject.AsSpan()).IsParsableInto<uint>(formatProvider);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject.AsSpan()
					              is parsable into uint using {cultureName},
					              but Parse of uint did throw a FormatException:
					                The input string '{subject}' was not in a correct format.
					              """);
			}

			[Test]
			[Arguments("12,34", "de-AT")]
			[Arguments("12.34", "en-US")]
			public async Task WithFormatProvider_WhenFormatMatches_ShouldSucceed(string subject, string cultureName)
			{
				IFormatProvider formatProvider = new CultureInfo(cultureName);

				async Task Act()
					=> await That(subject.AsSpan()).IsParsableInto<decimal>(formatProvider);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WhichTests
		{
			[Test]
			public async Task WhenSpanIsNotParsable_ShouldFail()
			{
				async Task Act()
					=> await That("abc".AsSpan()).IsParsableInto<TimeSpan>().Which.IsLessThan(10.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that "abc".AsSpan()
					             is parsable into TimeSpan that is less than 0:10,
					             but Parse of TimeSpan did throw a FormatException:
					               String 'abc' was not recognized as a valid TimeSpan.
					             """);
			}

			[Test]
			public async Task WhenSpanIsParsable_ShouldSucceed()
			{
				async Task Act()
					=> await That("42".AsSpan()).IsParsableInto<int>().Which.IsBetween(41).And(43);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("12,34", "de-AT")]
			[Arguments("12.34", "en-US")]
			public async Task WithFormatProvider_ShouldBeUsed(string subject, string cultureName)
			{
				IFormatProvider formatProvider = new CultureInfo(cultureName);

				async Task Act()
					=> await That(subject.AsSpan()).IsParsableInto<decimal>(formatProvider).Which.IsEqualTo(12.34M);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
