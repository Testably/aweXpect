namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class AsWildcardTests
		{
			[Test]
			[Arguments("some message", "*me me*", true)]
			[Arguments("some message", "*ME ME*", false)]
			[Arguments("some message", "some?message", true)]
			[Arguments("some message", "some*message", true)]
			[Arguments("some message", "some me?age", false)]
			[Arguments("some message", "some me??age", true)]
			public async Task ShouldDefaultToCaseSensitiveMatch(
				string subject, string pattern, bool expectMatch)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsWildcard();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches {Formatter.Format(pattern)},
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (wildcard pattern)
					              """);
			}

			[Test]
			[Arguments("a.b", "a.b", true)]
			[Arguments("axb", "a.b", false)]
			[Arguments("a+b", "a+b", true)]
			[Arguments("ab", "a+b", false)]
			[Arguments("a[b]c", "a[b]c", true)]
			public async Task ShouldEscapeRegexMetacharactersInThePattern(
				string subject, string pattern, bool expectMatch)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsWildcard();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches {Formatter.Format(pattern)},
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (wildcard pattern)
					              """)
					.Because("only '*' and '?' are wildcards, every other regex metacharacter is a literal");
			}

			[Test]
			public async Task ShouldNotMatchASurrogatePairWithTwoQuestionMarks()
			{
				string subject = "\U0001F600";

				async Task Act()
					=> await That(subject).IsEqualTo("??").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches "??",
					             but it did not match:
					               ↓ (actual)
					               "😀"
					               "??"
					               ↑ (wildcard pattern)
					             """)
					.Because("the emoji is one character, so it cannot match two question marks");
			}

			[Test]
			[Arguments("abc", "abc", true)]
			[Arguments("xyz\nabc", "abc", false)]
			[Arguments("abc\nxyz", "abc", false)]
			[Arguments("xyz\nabc\nqqq", "abc", false)]
			[Arguments("abc\n", "abc", false)]
			[Arguments("abc\n", "abc*", true)]
			[Arguments("", "", true)]
			[Arguments("a\n\nb", "", false)]
			public async Task ShouldRequireThePatternToCoverTheCompleteSubject(
				string subject, string pattern, bool expectMatch)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsWildcard();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches {Formatter.Format(pattern)},
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (wildcard pattern)
					              """)
					.Because("the pattern is anchored to the whole string, so it may not match a single line "
					         + "and a trailing newline is part of the subject");
			}

			[Test]
			[Arguments("\U0001F600", "?")]
			[Arguments("a\U0001F600b", "a?b")]
			[Arguments("\U0001F600\U0001F601", "??")]
			public async Task ShouldTreatASurrogatePairAsOneCharacter(string subject, string pattern)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsWildcard();

				await That(Act).DoesNotThrow()
					.Because("the emoji is one character, although it consists of two UTF-16 code units");
			}

			[Test]
			[Arguments("a\nb", "a?b", true)]
			[Arguments("\n", "?", true)]
			[Arguments("\nb", "?b", true)]
			[Arguments("a\n", "a?", true)]
			[Arguments("a\r\nb", "a??b", true)]
			[Arguments("a\nb", "a*b", true)]
			[Arguments("a\nb\nc", "a*c", true)]
			[Arguments("a\r\nb", "a?b", false)]
			public async Task ShouldTreatNewlinesLikeAnyOtherCharacter(
				string subject, string pattern, bool expectMatch)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsWildcard();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches {Formatter.Format(pattern)},
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (wildcard pattern)
					              """)
					.Because("a newline is a character, so both '*' and '?' have to match it");
			}

			[Test]
			public async Task WhenACustomComparerIsUsed_ShouldThrowInvalidOperationException()
			{
				string subject = "some message";

				async Task Act()
					=> await That(subject).IsEqualTo("*").AsWildcard().Using(StringComparer.Ordinal);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A custom comparer is not supported for regex or wildcard matching.")
					.Because("the wildcard is translated into a regex, which cannot consult a comparer");
			}

			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenIgnoringCase_ShouldIgnoreCase(
				bool ignoreCase)
			{
				string subject = "some message";
				string pattern = "*ME ME*";

				async Task Act()
					=> await That(subject).IsEqualTo(pattern)
						.AsWildcard().IgnoringCase(ignoreCase);

				await That(Act).Throws().OnlyIf(!ignoreCase)
					.WithMessage("""
					             Expected that subject
					             matches "*ME ME*",
					             but it did not match:
					               ↓ (actual)
					               "some message"
					               "*ME ME*"
					               ↑ (wildcard pattern)
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
					=> await That(subject).IsEqualTo(pattern).AsWildcard().IgnoringCase();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches {Formatter.Format(pattern)} ignoring case,
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (wildcard pattern)
					              """)
					.Because("the dotted and dotless Turkish 'I' must not change which characters are considered equal");
			}

			[Test]
			public async Task WhenIgnoringCase_ShouldStillRequireThePatternToCoverTheCompleteSubject()
			{
				string subject = "XYZ\nABC";

				async Task Act()
					=> await That(subject).IsEqualTo("abc").AsWildcard().IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches "abc" ignoring case,
					             but it did not match:
					               ↓ (actual)
					               "XYZ\nABC"
					               "abc"
					               ↑ (wildcard pattern)
					             """)
					.Because("ignoring the casing must not turn the anchors into line anchors");
			}

			[Test]
			[Arguments("k", "K")]
			[Arguments("K", "k")]
			[Arguments("xk", "*K")]
			public async Task WhenIgnoringCase_ShouldTreatTheKelvinSignLikeThePlainComparison(
				string subject, string pattern)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsWildcard().IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              matches {Formatter.Format(pattern)} ignoring case,
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (wildcard pattern)
					              """)
					.Because("the casing is ignored like the plain comparison with OrdinalIgnoreCase, which does not consider the Kelvin sign equal to 'k'");
			}

			[Test]
			public async Task WhenIgnoringNewlineStyle_ShouldMatchAWindowsNewlineWithASingleQuestionMark()
			{
				string subject = "a\r\nb";

				async Task Act()
					=> await That(subject).IsEqualTo("a?b").AsWildcard().IgnoringNewlineStyle();

				await That(Act).DoesNotThrow()
					.Because("the normalized newline is a single character that '?' has to match");
			}

			[Test]
			public async Task WhenIgnoringNewlineStyle_ShouldStillRequireThePatternToCoverTheCompleteSubject()
			{
				string subject = "xyz\r\nabc";

				async Task Act()
					=> await That(subject).IsEqualTo("abc").AsWildcard().IgnoringNewlineStyle();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches "abc" ignoring newline style,
					             but it did not match:
					               ↓ (actual)
					               "xyz\nabc"
					               "abc"
					               ↑ (wildcard pattern)

					             Actual:
					             xyz
					             abc
					             """).IgnoringNewlineStyle()
					.Because("normalizing the newline style does not remove the first line from the subject");
			}

			[Test]
			[Arguments("tr-TR", "I", "I", true)]
			[Arguments("tr-TR", "I", "i", false)]
			[Arguments("tr-TR", "ı", "I", false)]
			[Arguments("", "I", "I", true)]
			[Arguments("", "I", "i", false)]
			[Arguments("", "ı", "I", false)]
			public async Task WhenNotIgnoringCase_ShouldMatchCaseSensitiveIndependentOfTheCurrentCulture(
				string cultureName, string subject, string pattern, bool expectMatch)
			{
				using CultureOverride _ = new(cultureName);

				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsWildcard();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches {Formatter.Format(pattern)},
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                {Formatter.Format(pattern)}
					                ↑ (wildcard pattern)
					              """)
					.Because("a case-sensitive match never looked at the culture");
			}

			[Test]
			public async Task WhenPatternDoesNotCompleteInTime_ShouldThrowArgumentException()
			{
				string subject = new('a', 100);
				string pattern = "*a*a*a*a*a*a*a*a*a*ab";

				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsWildcard();

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						$"""The wildcard pattern "{pattern}" did not complete within 0:01. Simplify the pattern to avoid catastrophic backtracking.""")
					.AsPrefix().And
					.WithParamName("expected")
					.Because("the message must name the wildcard pattern, not the regex it is translated into");
			}

			[Test]
			[Arguments("", true)]
			[Arguments("a", false)]
			[Arguments("\n", false)]
			public async Task WhenPatternIsEmpty_ShouldMatchOnlyTheEmptySubject(
				string subject, bool expectMatch)
			{
				async Task Act()
					=> await That(subject).IsEqualTo("").AsWildcard();

				await That(Act).Throws().OnlyIf(!expectMatch)
					.WithMessage($"""
					              Expected that subject
					              matches "",
					              but it did not match:
					                ↓ (actual)
					                {Formatter.Format(subject)}
					                ""
					                ↑ (wildcard pattern)
					              """)
					.Because("an empty wildcard pattern has the well-defined meaning of the empty string");
			}

			[Test]
			public async Task WhenPatternIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some message";

				async Task Act()
					=> await That(subject).IsEqualTo(null).AsWildcard();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' wildcard pattern cannot be null.").AsPrefix()
					.Because("a missing pattern cannot express any expectation");
			}

			[Test]
			public async Task WhenPatternIsProvidedAsNullVariable_ShouldThrowArgumentNullException()
			{
				string subject = "some message";
				string? pattern = null;

				async Task Act()
					=> await That(subject).IsEqualTo(pattern).AsWildcard();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' wildcard pattern cannot be null.").AsPrefix()
					.Because("a pattern that only becomes null at runtime must be rejected just as a literal one");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo("p").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches "p",
					             but it was <null>
					             """);
			}
		}
	}
}
