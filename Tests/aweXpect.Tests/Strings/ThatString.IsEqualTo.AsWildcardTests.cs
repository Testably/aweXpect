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

			[Fact]
			public async Task WhenIgnoringNewlineStyle_ShouldMatchAWindowsNewlineWithASingleQuestionMark()
			{
				string subject = "a\r\nb";

				async Task Act()
					=> await That(subject).IsEqualTo("a?b").AsWildcard().IgnoringNewlineStyle();

				await That(Act).DoesNotThrow()
					.Because("the normalized newline is a single character that '?' has to match");
			}
		}
	}
}
