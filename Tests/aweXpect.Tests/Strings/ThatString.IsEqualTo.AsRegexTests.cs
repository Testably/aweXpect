using System.Text.RegularExpressions;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class AsRegexTests
		{
			[Theory]
			[InlineData("some message", ".*me me.*", true)]
			[InlineData("some message", ".*ME ME.*", false)]
			public async Task ShouldDefaultToCaseSensitiveMatch(
				string subject, string pattern, bool expectMatch)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsRegex();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches regex {Formatter.Format(pattern)},
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (regex pattern)
					              """);
			}

			[Theory]
			[InlineData("b", "^b$", true)]
			[InlineData("a\nb", "^b$", false)]
			[InlineData("a\nb", "^a", true)]
			[InlineData("a\nb", "b$", true)]
			[InlineData("b\n", "^b$", true)]
			[InlineData("b\n", @"^b\z", false)]
			public async Task ShouldNotBindTheAnchorsToLineBoundaries(
				string subject, string pattern, bool expectMatch)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsRegex();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches regex {Formatter.Format(pattern)},
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (regex pattern)
					              """)
					.Because("the pattern is matched like Regex.IsMatch, where '$' also matches before a "
					         + "trailing newline, but never at an inner line boundary");
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenIgnoringCase_ShouldIgnoreCase(
				bool ignoreCase)
			{
				string subject = "some message";
				string pattern = ".*ME ME.*";

				async Task Act()
					=> await That(subject).IsEqualTo(pattern)
						.AsRegex().IgnoringCase(ignoreCase);

				await That(Act).Throws().OnlyIf(!ignoreCase)
					.WithMessage("""
					             Expected that subject
					             matches regex ".*ME ME.*",
					             but it did not match:
					               ↓ (actual)
					               "some message"
					               ".*ME ME.*"
					               ↑ (regex pattern)
					             """);
			}

			[Fact]
			public async Task WhenOptionsAreCombinedWithIgnoringCase_ShouldApplyBoth()
			{
				string subject = "a\nB";

				async Task Act()
					=> await That(subject).IsEqualTo("^b$").AsRegex(RegexOptions.Multiline).IgnoringCase();

				await That(Act).DoesNotThrow()
					.Because("the given options are combined with the ignored casing");
			}

			[Fact]
			public async Task WhenOptionsContainIgnoreCase_ShouldIgnoreCaseAlthoughIgnoringCaseIsDisabled()
			{
				string subject = "SOME";

				async Task Act()
					=> await That(subject).IsEqualTo("some").AsRegex(RegexOptions.IgnoreCase).IgnoringCase(false);

				await That(Act).DoesNotThrow()
					.Because("an explicitly given option is never taken away again");
			}

			[Fact]
			public async Task WhenOptionsContainMultiline_ShouldBindTheAnchorsToLineBoundaries()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).IsEqualTo("^b$").AsRegex(RegexOptions.Multiline);

				await That(Act).DoesNotThrow()
					.Because("the line anchors are opt-in via the explicit options");
			}

			[Fact]
			public async Task WhenPatternEnablesMultilineInline_ShouldBindTheAnchorsToLineBoundaries()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).IsEqualTo("(?m)^b$").AsRegex();

				await That(Act).DoesNotThrow()
					.Because("the line anchors are also available via the inline construct");
			}
		}
	}
}
