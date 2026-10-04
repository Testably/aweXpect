#if NET8_0_OR_GREATER
using System.Globalization;
using System.Text;
using aweXpect.Core;

namespace aweXpect.Tests;

public sealed partial class ThatSpan
{
	public sealed partial class IsNotParsableInto
	{
		public sealed class Utf8Tests
		{
			[Fact]
			public async Task WhenAnEarlierAttemptWasNotParsable_ShouldNotFailWithItsException()
			{
				int calls = 0;
				Func<SpanWrapper<byte>> subject = () => new SpanWrapper<byte>(calls++ == 0 ? "abc"u8 : "1"u8);

				async Task Act()
					=> await That(subject).Eventually().Within(200.Milliseconds()).CheckEvery(10.Milliseconds())
						.IsNotParsableInto<int>().And.IsParsableInto<int>();

				XunitException exception = await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             eventually is not parsable into int and is parsable into int within 0:00.200,
					             but it was "1", which is parsable into 1
					             """);
				await That(exception.InnerException).IsNull()
					.Because("the last attempt parsed the subject without an exception");
			}

			[Fact]
			public async Task WhenSpanIsNotParsable_ShouldSucceed()
			{
				byte[] subject = "abc"u8.ToArray();

				async Task Act()
					=> await That(subject.AsSpan()).IsNotParsableInto<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSpanIsParsable_ShouldFail()
			{
				byte[] subject = "42"u8.ToArray();

				async Task Act()
					=> await That(subject.AsSpan()).IsNotParsableInto<int>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject.AsSpan()
					             is not parsable into int,
					             but it was "42", which is parsable into 42
					             """);
			}

			[Theory]
			[InlineData("12,34", "de-AT")]
			[InlineData("12.34", "en-US")]
			public async Task WithFormatProvider_ShouldBeUsed(string subjectString, string cultureName)
			{
				byte[] subject = Encoding.UTF8.GetBytes(subjectString);
				IFormatProvider formatProvider = new CultureInfo(cultureName);

				async Task Act()
					=> await That(subject.AsSpan()).IsNotParsableInto<decimal>(formatProvider);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject.AsSpan()
					              is not parsable into decimal using {cultureName},
					              but it was "{subjectString}", which is parsable into 12.34
					              """);
			}
		}
	}
}
#endif
