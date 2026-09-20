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

			[Theory]
			[InlineData("tr-TR", "I", "i", true)]
			[InlineData("tr-TR", "İ", "i", false)]
			[InlineData("tr-TR", "ı", "I", false)]
			[InlineData("", "I", "i", true)]
			[InlineData("", "İ", "i", false)]
			[InlineData("", "ı", "I", false)]
			public async Task WhenIgnoringCase_ShouldIgnoreCaseIndependentOfTheCurrentCulture(
				string cultureName, string subject, string pattern, bool expectMatch)
			{
				using CultureOverride _ = new(cultureName);

				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsRegex().IgnoringCase();

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
					.Because("the dotted and dotless Turkish 'I' must not change which characters are considered equal");
			}

			[Fact]
			public async Task WhenNotIgnoringCase_ShouldMatchCaseSensitiveIndependentOfTheCurrentCulture()
			{
				string subject = "I";

				using CultureOverride _ = new("tr-TR");

				async Task Act()
					=> await That(subject).IsEqualTo("i").AsRegex();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches regex "i",
					             but it did not match:
					               ↓ (actual)
					               "I"
					               "i"
					               ↑ (regex pattern)
					             """)
					.Because("a case-sensitive match never looked at the culture");
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
			public async Task WhenOptionsContainIgnoreCase_ShouldIgnoreTheCultureWhenIgnoringCaseIsEnabled()
			{
				string subject = "İ";

				using CultureOverride _ = new("tr-TR");

				async Task Act()
					=> await That(subject).IsEqualTo("i").AsRegex(RegexOptions.IgnoreCase).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              matches regex "i",
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                "i"
					                ↑ (regex pattern)
					              """)
					.Because("asking for the casing to be ignored adds the culture independence to the given options");
			}

			[Theory]
			[InlineData("I", "i", false)]
			[InlineData("İ", "i", true)]
			public async Task WhenOptionsContainIgnoreCase_ShouldUseTheCurrentCulture(
				string subject, string pattern, bool expectMatch)
			{
				using CultureOverride _ = new("tr-TR");

				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsRegex(RegexOptions.IgnoreCase);

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
					.Because("an explicitly given option keeps the behaviour of Regex.IsMatch, which is culture-dependent");
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

			[Fact]
			public async Task WhenPatternIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "some message";

				async Task Act()
					=> await That(subject).IsEqualTo("").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("an empty pattern matches every subject, so the expectation could never fail");
			}

			[Fact]
			public async Task WhenPatternIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some message";

				async Task Act()
					=> await That(subject).IsEqualTo(null).AsRegex();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' regex pattern cannot be null.").AsPrefix()
					.Because("a missing pattern cannot express any expectation");
			}

			[Fact]
			public async Task WhenPatternIsProvidedAsNullVariable_ShouldThrowArgumentNullException()
			{
				string subject = "some message";
				string? pattern = null;

				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsRegex();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' regex pattern cannot be null.").AsPrefix()
					.Because("a pattern that only becomes null at runtime must be rejected just as a literal one");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(".*").AsRegex();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches regex ".*",
					             but it was <null>
					             """);
			}
		}
	}
}
