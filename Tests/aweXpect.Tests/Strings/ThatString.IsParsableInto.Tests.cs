#if NET8_0_OR_GREATER
using System.Globalization;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsParsableInto
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsParsableInto<int>().Because("null should fail");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is parsable into int, because null should fail,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenStringIsNotParsable_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsParsableInto<int>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is parsable into int,
					             but Parse of int did throw a FormatException:
					               The input string 'abc' was not in a correct format.
					             """).And
					.Whose(e => e.InnerException, i => i.Is<FormatException>())
					.Because("the exception of the parser tells why the string is not parsable");
			}

			[Fact]
			public async Task WhenStringIsParsable_ShouldSucceed()
			{
				string subject = "42";

				async Task Act()
					=> await That(subject).IsParsableInto<int>();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("-12.34", "de-AT")]
			[InlineData("-12,34", "en-US")]
			public async Task WithFormatProvider_WhenFormatDoesNotMatch_ShouldFail(string subject, string cultureName)
			{
				IFormatProvider formatProvider = new CultureInfo(cultureName);

				async Task Act()
					=> await That(subject).IsParsableInto<uint>(formatProvider);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is parsable into uint using {cultureName},
					              but Parse of uint did throw a FormatException:
					                The input string '{subject}' was not in a correct format.
					              """);
			}

			[Theory]
			[InlineData("12,34", "de-AT")]
			[InlineData("12.34", "en-US")]
			public async Task WithFormatProvider_WhenFormatMatches_ShouldSucceed(string subject, string cultureName)
			{
				IFormatProvider formatProvider = new CultureInfo(cultureName);

				async Task Act()
					=> await That(subject).IsParsableInto<decimal>(formatProvider);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WhichTests
		{
			[Fact]
			public async Task WhenNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsParsableInto<int>().Which.IsGreaterThan(2).And.IsLessThan(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is parsable into int that is greater than 2 and is less than 3,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenStringIsNotParsable_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsParsableInto<TimeSpan>().Which.IsLessThan(10.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is parsable into TimeSpan that is less than 0:10,
					             but Parse of TimeSpan did throw a FormatException:
					               String 'abc' was not recognized as a valid TimeSpan.
					             """);
			}

			[Fact]
			public async Task WhenStringIsParsable_ShouldSucceed()
			{
				string subject = "42";

				async Task Act()
					=> await That(subject).IsParsableInto<int>().Which.IsBetween(41).And(43);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("12,34", "de-AT")]
			[InlineData("12.34", "en-US")]
			public async Task WithFormatProvider_ShouldBeUsed(string subject, string cultureName)
			{
				IFormatProvider formatProvider = new CultureInfo(cultureName);

				async Task Act()
					=> await That(subject).IsParsableInto<decimal>(formatProvider).Which.IsEqualTo(12.34M);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
