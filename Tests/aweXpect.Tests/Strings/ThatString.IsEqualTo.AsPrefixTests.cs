namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class AsPrefixTests
		{
			[Fact]
			public async Task WhenActualAndExpectedAreNull_ShouldSucceed()
			{
				string? subject = null;
				string? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "some text",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "some text";
				string? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with <null>,
					             but it was "some text"
					             """);
			}

			[Fact]
			public async Task WhenStringHasAdditionalTrailingWhitespace_ShouldSucceed()
			{
				string subject = "some text \t ";
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringHasMissingLeadingWhitespace_ShouldFail()
			{
				string subject = "some text";
				string expected = " \t some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with " \t some text",
					             but it was "some text" which misses some whitespace (" \t " at the beginning)
					             """);
			}

			[Fact]
			public async Task WhenStringHasMissingTrailingWhitespace_ShouldFail()
			{
				string subject = "some text";
				string expected = "some text \t ";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "some text \t ",
					             but it was "some text" with a length of 9 which is shorter than the expected length of 12 and misses:
					               " 	 "
					             """);
			}

			[Fact]
			public async Task WhenStringHasUnexpectedLeadingWhitespace_ShouldFail()
			{
				string subject = " \t some text and more";
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "some text",
					             but it was " \t some text and more" which has unexpected whitespace (" \t " at the beginning)
					             """);
			}

			[Fact]
			public async Task WhenStringIsShorter_ShouldFail()
			{
				string subject = "some text with";
				string expected = "some text without out";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "some text without out",
					             but it was "some text with" with a length of 14 which is shorter than the expected length of 21 and misses:
					               "out out"
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenStringsAreTheSame_ShouldSucceed(string subject)
			{
				string expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringsDiffer_ShouldFail()
			{
				string subject = "actual text";
				string expected = "expected other text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "expected other text",
					             but it was "actual text" which differs at index 0:
					                ↓ (actual)
					               "actual text"
					               "expected other text"
					                ↑ (expected prefix)
					             """);
			}

			[Fact]
			public async Task WhenStringStartsWithExpected_ShouldSucceed()
			{
				string subject = "some text without out";
				string expected = "some text with";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsPrefixNegatedTests
		{
			[Fact]
			public async Task WhenStringEndsWithExpected_ShouldFail()
			{
				string subject = "some text without out";
				string expected = "some text without";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(expected).AsPrefix().IgnoringCase());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "some text without" ignoring case,
					             but it was "some text without out"
					             """);
			}

			[Fact]
			public async Task WhenStringEndsWithExpected_UsingCustomComparer_ShouldFail()
			{
				string subject = "some text without out";
				string expected = "sOmE text wIthOUt";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsEqualTo(expected).AsPrefix().Using(new IgnoreCaseForVocalsComparer()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "sOmE text wIthOUt" using IgnoreCaseForVocalsComparer,
					             but it was "some text without out"
					             """);
			}
		}
	}
}
