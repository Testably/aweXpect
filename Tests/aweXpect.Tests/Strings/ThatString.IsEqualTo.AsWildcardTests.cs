namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class AsWildcardTests
		{
			[Theory]
			[InlineData("some message", "*me me*", true)]
			[InlineData("some message", "*ME ME*", false)]
			[InlineData("some message", "some?message", true)]
			[InlineData("some message", "some*message", true)]
			[InlineData("some message", "some me?age", false)]
			[InlineData("some message", "some me??age", true)]
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

			[Theory]
			[InlineData("a.b", "a.b", true)]
			[InlineData("axb", "a.b", false)]
			[InlineData("a+b", "a+b", true)]
			[InlineData("ab", "a+b", false)]
			[InlineData("a[b]c", "a[b]c", true)]
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

			[Theory]
			[InlineData("abc", "abc", true)]
			[InlineData("xyz\nabc", "abc", false)]
			[InlineData("abc\nxyz", "abc", false)]
			[InlineData("xyz\nabc\nqqq", "abc", false)]
			[InlineData("abc\n", "abc", false)]
			[InlineData("abc\n", "abc*", true)]
			[InlineData("", "", true)]
			[InlineData("a\n\nb", "", false)]
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

			[Theory]
			[InlineData("a\nb", "a?b", true)]
			[InlineData("\n", "?", true)]
			[InlineData("\nb", "?b", true)]
			[InlineData("a\n", "a?", true)]
			[InlineData("a\r\nb", "a??b", true)]
			[InlineData("a\nb", "a*b", true)]
			[InlineData("a\nb\nc", "a*c", true)]
			[InlineData("a\r\nb", "a?b", false)]
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

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
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
					=> await That(subject).IsEqualTo(pattern).AsWildcard().IgnoringCase();

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
					.Because("the dotted and dotless Turkish 'I' must not change which characters are considered equal");
			}

			[Fact]
			public async Task WhenIgnoringCase_ShouldStillRequireThePatternToCoverTheCompleteSubject()
			{
				string subject = "XYZ\nABC";

				async Task Act()
					=> await That(subject).IsEqualTo("abc").AsWildcard().IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches "abc",
					             but it did not match:
					               ↓ (actual)
					               "XYZ\nABC"
					               "abc"
					               ↑ (wildcard pattern)
					             """)
					.Because("ignoring the casing must not turn the anchors into line anchors");
			}

			[Fact]
			public async Task WhenIgnoringNewlineStyle_ShouldMatchAWindowsNewlineWithASingleQuestionMark()
			{
				string subject = "a\r\nb";

				async Task Act()
					=> await That(subject).IsEqualTo("a?b").AsWildcard().IgnoringNewlineStyle();

				await That(Act).DoesNotThrow()
					.Because("the normalized newline is a single character that '?' has to match");
			}

			[Fact]
			public async Task WhenIgnoringNewlineStyle_ShouldStillRequireThePatternToCoverTheCompleteSubject()
			{
				string subject = "xyz\r\nabc";

				async Task Act()
					=> await That(subject).IsEqualTo("abc").AsWildcard().IgnoringNewlineStyle();

				await That(Act).Throws<XunitException>()
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

			[Theory]
			[InlineData("tr-TR", "I", "I", true)]
			[InlineData("tr-TR", "I", "i", false)]
			[InlineData("tr-TR", "ı", "I", false)]
			[InlineData("", "I", "I", true)]
			[InlineData("", "I", "i", false)]
			[InlineData("", "ı", "I", false)]
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

			[Theory]
			[InlineData("", true)]
			[InlineData("a", false)]
			[InlineData("\n", false)]
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

			[Fact]
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

			[Fact]
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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo("p").AsWildcard();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches "p",
					             but it was <null>
					             """);
			}
		}
	}
}
