#if NET8_0_OR_GREATER
using System.Globalization;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsNotParsableInto
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotParsableInto<int>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not parsable into int,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenStringIsNotParsable_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsNotParsableInto<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringIsParsable_ShouldFail()
			{
				string subject = "42";

				async Task Act()
					=> await That(subject).IsNotParsableInto<int>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not parsable into int,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("12,34", "de-AT")]
			[InlineData("12.34", "en-US")]
			public async Task WithFormatProvider_ShouldBeUsed(string subject, string cultureName)
			{
				IFormatProvider formatProvider = new CultureInfo(cultureName);

				async Task Act()
					=> await That(subject).IsNotParsableInto<decimal>(formatProvider);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not parsable into decimal using {cultureName},
					              but it was
					              """);
			}
		}
	}
}
#endif
