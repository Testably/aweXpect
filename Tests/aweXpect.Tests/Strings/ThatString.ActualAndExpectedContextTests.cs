using aweXpect.Customization;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed class ActualAndExpectedContextTests
	{
		[Fact]
		public async Task WhenActualExceedsTheMaximumStringLength_ShouldIncludeActual()
		{
			string subject = "a subject with more than twenty characters";

			async Task Act()
			{
				using (Customize.aweXpect.Formatting().MaximumStringLength.Set(20))
				{
					await That(subject).Contains("foo");
				}
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains "foo" at least once,
				             but it did not contain "foo" in "a subject with more …"

				             Actual:
				             a subject with more than twenty characters
				             """);
		}

		[Fact]
		public async Task WhenTheDiffShortensTheValues_ShouldIncludeActualAndExpected()
		{
			string subject = "The quick brown fox jumps over the lazy dog and keeps on running far away from here";
			string expected = "The quick brown fox jumps over the lazy dog and keeps on running far away from home";

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to "The quick brown fox jumps…",
				             but it was "The quick brown fox jumps…", which differs at index 80:
				                                ↓ (actual)
				               "…far away from here"
				               "…far away from home"
				                                ↑ (expected)

				             Actual:
				             The quick brown fox jumps over the lazy dog and keeps on running far away from here

				             Expected:
				             The quick brown fox jumps over the lazy dog and keeps on running far away from home
				             """);
		}

		[Fact]
		public async Task WhenTheDiffShowsTheWholeValue_ShouldOmitActual()
		{
			string subject = "this subject is longer than thirty characters";
			string expected = "short";

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to "short",
				             but it was "this subject is longer than…", which differs at index 0:
				                ↓ (actual)
				               "this subject is longer than thirty characters"
				               "short"
				                ↑ (expected)
				             """);
		}

		[Fact]
		public async Task WhenValuesAreShownCompletely_ShouldOmitActualAndExpected()
		{
			string subject = "foo\nbar";
			string expected = "foo\nbaz";

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to "foo\nbaz",
				             but it was "foo\nbar", which differs on line 2 and column 3:
				                       ↓ (actual)
				               "foo\nbar"
				               "foo\nbaz"
				                       ↑ (expected)
				             """);
		}
	}
}
