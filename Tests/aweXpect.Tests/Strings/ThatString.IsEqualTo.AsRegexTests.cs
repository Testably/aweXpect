using System.Text.RegularExpressions;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class AsRegexTests
		{
			[Test]
			[Arguments("some message", ".*me me.*", true)]
			[Arguments("some message", ".*ME ME.*", false)]
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

			[Test]
			[Arguments("b", "^b$", true)]
			[Arguments("a\nb", "^b$", false)]
			[Arguments("a\nb", "^a", true)]
			[Arguments("a\nb", "b$", true)]
			[Arguments("b\n", "^b$", true)]
			[Arguments("b\n", @"^b\z", false)]
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

			[Test]
			public async Task WhenACustomComparerIsUsed_ShouldThrowInvalidOperationException()
			{
				string subject = "some message";

				async Task Act()
					=> await That(subject).IsEqualTo(".*").AsRegex().Using(StringComparer.Ordinal);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A custom comparer is not supported for regex or wildcard matching.")
					.Because("the regex engine cannot consult a comparer, so it used to be ignored silently");
			}

			[Test]
			[Arguments(true)]
			[Arguments(false)]
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

			[Test]
			[Arguments("tr-TR", "I", "i", true)]
			[Arguments("tr-TR", "İ", "i", false)]
			[Arguments("tr-TR", "ı", "I", false)]
			[Arguments("", "I", "i", true)]
			[Arguments("", "İ", "i", false)]
			[Arguments("", "ı", "I", false)]
			public async Task WhenIgnoringCase_ShouldIgnoreCaseIndependentOfTheCurrentCulture(
				string cultureName, string subject, string pattern, bool expectMatch)
			{
				using CultureOverride _ = new(cultureName);

				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsRegex().IgnoringCase();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches regex {Formatter.Format(pattern)} ignoring case,
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (regex pattern)
					              """)
					.Because("the dotted and dotless Turkish 'I' must not change which characters are considered equal");
			}

			[Test]
			[Arguments("forget", false)]
			[Arguments("get it", true)]
			[Arguments("for get", true)]
			public async Task WhenIgnoringLeadingWhiteSpace_ShouldOnlyIgnoreTheWhiteSpaceOfThePatternAtTheStartOfTheSubject(
				string subject, bool expectMatch)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(" g.t").AsRegex().IgnoringLeadingWhiteSpace();

				await That(Act).Throws<FailException>().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches regex " g.t" ignoring leading whitespace,
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                " g.t"
					                ↑ (regex pattern)
					              """)
					.Because("the regex may match any part of the subject, so its space is only optional at the start of the subject");
			}

			[Test]
			public async Task WhenNotIgnoringCase_ShouldMatchCaseSensitiveIndependentOfTheCurrentCulture()
			{
				string subject = "I";

				using CultureOverride _ = new("tr-TR");

				async Task Act()
					=> await That(subject).IsEqualTo("i").AsRegex();

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenOptionsAreCombinedWithIgnoringCase_ShouldApplyBoth()
			{
				string subject = "a\nB";

				async Task Act()
					=> await That(subject).IsEqualTo("^b$").AsRegex(RegexOptions.Multiline).IgnoringCase();

				await That(Act).DoesNotThrow()
					.Because("the given options are combined with the ignored casing");
			}

			[Test]
			[Arguments((RegexOptions)0x4000_0000)]
			[Arguments(RegexOptions.ECMAScript | RegexOptions.Singleline)]
			public async Task WhenOptionsAreInvalid_ShouldThrowArgumentOutOfRangeException(
				RegexOptions regexOptions)
			{
				async Task Act()
					=> await That("ABC").IsEqualTo("b").AsRegex(regexOptions);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage($"The regex options '{regexOptions}' are not a valid combination.").AsPrefix().And
					.WithParamName("regexOptions")
					.Because("invalid options used to name an internal parameter of the regex engine");
			}

			[Test]
			public async Task WhenOptionsContainIgnoreCase_ShouldIgnoreCaseAlthoughIgnoringCaseIsDisabled()
			{
				string subject = "SOME";

				async Task Act()
					=> await That(subject).IsEqualTo("some").AsRegex(RegexOptions.IgnoreCase).IgnoringCase(false);

				await That(Act).DoesNotThrow()
					.Because("an explicitly given option is never taken away again");
			}

			[Test]
			public async Task WhenOptionsContainIgnoreCase_ShouldIgnoreTheCultureWhenIgnoringCaseIsEnabled()
			{
				string subject = "İ";

				using CultureOverride _ = new("tr-TR");

				async Task Act()
					=> await That(subject).IsEqualTo("i").AsRegex(RegexOptions.IgnoreCase).IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              matches regex "i" ignoring case,
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                "i"
					                ↑ (regex pattern)
					              """)
					.Because("asking for the casing to be ignored adds the culture independence to the given options");
			}

			[Test]
			[Arguments("I", "i", false)]
			[Arguments("İ", "i", true)]
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

			[Test]
			public async Task WhenOptionsContainMultiline_ShouldBindTheAnchorsToLineBoundaries()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).IsEqualTo("^b$").AsRegex(RegexOptions.Multiline);

				await That(Act).DoesNotThrow()
					.Because("the line anchors are opt-in via the explicit options");
			}

			[Test]
			public async Task WhenPatternDoesNotCompleteInTime_ShouldThrowArgumentException()
			{
				string subject = new('a', 30);

				async Task Act()
					=> await That(subject + "!").IsEqualTo("(a+)+$").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"""The regex "(a+)+$" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
					.AsPrefix().And
					.WithParamName("expected")
					.Because("the timeout used to escape as a generic evaluation error without naming the pattern");
			}

			[Test]
			public async Task WhenPatternEnablesMultilineInline_ShouldBindTheAnchorsToLineBoundaries()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).IsEqualTo("(?m)^b$").AsRegex();

				await That(Act).DoesNotThrow()
					.Because("the line anchors are also available via the inline construct");
			}

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(".*").AsRegex();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches regex ".*",
					             but it was <null>
					             """);
			}
		}
	}
}
