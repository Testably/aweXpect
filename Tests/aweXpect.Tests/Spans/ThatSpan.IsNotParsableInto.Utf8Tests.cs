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
			[Test]
			public async Task WhenAnEarlierAttemptWasNotParsable_ShouldNotFailWithItsException()
			{
				int calls = 0;
				Func<SpanWrapper<byte>> subject = () => new SpanWrapper<byte>(calls++ == 0 ? "abc"u8 : "1"u8);

				async Task Act()
					=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
						.IsNotParsableInto<int>().And.IsParsableInto<int>();

				FailException exception = await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             eventually is not parsable into int and is parsable into int within 0:05,
					             but it was "1", which is parsable into 1
					             """);
				await That(exception.InnerException).IsNull()
					.Because("the last attempt parsed the subject without an exception");
			}

			[Test]
			public async Task WhenSpanIsNotParsable_ShouldSucceed()
			{
				byte[] subject = "abc"u8.ToArray();

				async Task Act()
					=> await That(subject.AsSpan()).IsNotParsableInto<int>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSpanIsParsable_ShouldFail()
			{
				byte[] subject = "42"u8.ToArray();

				async Task Act()
					=> await That(subject.AsSpan()).IsNotParsableInto<int>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject.AsSpan()
					             is not parsable into int,
					             but it was "42", which is parsable into 42
					             """);
			}

			[Test]
			[Arguments("12,34", "de-AT")]
			[Arguments("12.34", "en-US")]
			public async Task WithFormatProvider_ShouldBeUsed(string subjectString, string cultureName)
			{
				byte[] subject = Encoding.UTF8.GetBytes(subjectString);
				IFormatProvider formatProvider = new CultureInfo(cultureName);

				async Task Act()
					=> await That(subject.AsSpan()).IsNotParsableInto<decimal>(formatProvider);

				await That(Act).Throws<FailException>()
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
