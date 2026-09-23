#if NET8_0_OR_GREATER
using System.Globalization;

namespace aweXpect.Tests;

public sealed partial class ThatSpan
{
	public sealed partial class IsNotParsableInto
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSpanIsNotParsable_ShouldSucceed()
			{
				async Task Act()
					=> await That("abc".AsSpan()).IsNotParsableInto<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSpanIsParsable_ShouldFail()
			{
				async Task Act()
					=> await That("42".AsSpan()).IsNotParsableInto<int>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that "42".AsSpan()
					             is not parsable into int,
					             but it was "42", which is parsable into 42
					             """);
			}

			[Theory]
			[InlineData("12,34", "de-AT")]
			[InlineData("12.34", "en-US")]
			public async Task WithFormatProvider_ShouldBeUsed(string subject, string cultureName)
			{
				IFormatProvider formatProvider = new CultureInfo(cultureName);

				async Task Act()
					=> await That(subject.AsSpan()).IsNotParsableInto<decimal>(formatProvider);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject.AsSpan()
					              is not parsable into decimal using {cultureName},
					              but it was "{subject}", which is parsable into 12.34
					              """);
			}
		}
	}
}
#endif
