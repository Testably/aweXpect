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
			public async Task WhenAnEarlierAttemptWasNotParsable_ShouldNotFailWithItsException()
			{
				int calls = 0;
				Func<string> subject = () => calls++ == 0 ? "abc" : "1";

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
			public async Task WhenCombinedWithAFailingExpectation_ShouldNotFailWithTheParseException()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsNotParsableInto<int>().And.IsEqualTo("xyz");

				XunitException exception = await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not parsable into int and is equal to "xyz",
					             but it was "abc", which differs at index 0:
					                ↓ (actual)
					               "abc"
					               "xyz"
					                ↑ (expected)
					             """);
				await That(exception.InnerException).IsNull()
					.Because("the parse exception is why the subject is not parsable, not why the expectation failed");
			}

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
					=> await That(subject).IsNotParsableInto<decimal>(formatProvider);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not parsable into decimal using {cultureName},
					              but it was "{subject}", which is parsable into 12.34
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenStringIsNotParsable_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotParsableInto<int>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is parsable into int,
					             but Parse of int did throw a FormatException:
					               The input string 'abc' was not in a correct format.
					             """);
			}
		}
	}
}
#endif
