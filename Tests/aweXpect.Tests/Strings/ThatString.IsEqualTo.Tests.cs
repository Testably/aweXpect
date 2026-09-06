using System.Globalization;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenActualAndExpectedAreNull_ShouldSucceed()
			{
				string? subject = null;
				string? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "some text";
				string? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to <null>,
					             but it was "some text"

					             Actual:
					             some text
					             """);
			}

			[Theory]
			[InlineData("i", "I")]
			public async Task WhenIgnoringCase_UseInvariantCulture(string subject, string expected)
			{
				// .NET converts uppercase Turkish 'I' to lowercase 'ı'
				// https://stackoverflow.com/q/78724630
				using CultureOverride _ = new("tr-TR");
				bool isEqualInvariant = subject.Equals(expected.ToLower(CultureInfo.InvariantCulture),
					StringComparison.CurrentCulture);
				bool isEqualTurkish = subject.Equals(expected.ToLower(),
					StringComparison.CurrentCulture);
				await That(isEqualInvariant).IsNotEqualTo(isEqualTurkish);

				async Task Action()
					=> await That(subject).IsEqualTo(expected).IgnoringCase();

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringHasMissingLeadingWhitespace_ShouldFail()
			{
				string subject = "some text";
				string expected = " \t some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to " \t some text",
					             but it was "some text" which misses some whitespace (" \t " at the beginning)

					             Actual:
					             some text
					             
					             Expected:
					              	 some text
					             """);
			}

			[Fact]
			public async Task WhenStringHasMissingTrailingWhitespace_ShouldFail()
			{
				string subject = "some text";
				string expected = "some text \t ";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text \t ",
					             but it was "some text" which misses some whitespace (" \t " at the end)

					             Actual:
					             some text
					             
					             Expected:
					             some text 	 
					             """);
			}

			[Fact]
			public async Task WhenStringHasUnexpectedLeadingWhitespace_ShouldFail()
			{
				string subject = " \t some text";
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text",
					             but it was " \t some text" which has unexpected whitespace (" \t " at the beginning)

					             Actual:
					              	 some text
					             
					             Expected:
					             some text
					             """);
			}

			[Fact]
			public async Task WhenStringHasUnexpectedTrailingWhitespace_ShouldFail()
			{
				string subject = "some text \t ";
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text",
					             but it was "some text \t " which has unexpected whitespace (" \t " at the end)

					             Actual:
					             some text 	 
					             
					             Expected:
					             some text
					             """);
			}

			[Fact]
			public async Task WhenStringIsLonger_ShouldFail()
			{
				string subject = "some text without out";
				string expected = "some text with";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text with",
					             but it was "some text without out" with a length of 21 which is longer than the expected length of 14 and has superfluous:
					               "out out"

					             Actual:
					             some text without out
					             
					             Expected:
					             some text with
					             """);
			}

			[Fact]
			public async Task WhenStringIsShorter_ShouldFail()
			{
				string subject = "some text with";
				string expected = "some text without out";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text without out",
					             but it was "some text with" with a length of 14 which is shorter than the expected length of 21 and misses:
					               "out out"

					             Actual:
					             some text with
					             
					             Expected:
					             some text without out
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenStringsAreTheSame_ShouldSucceed(string subject)
			{
				string expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringsDiffer_ShouldFail()
			{
				string subject = "actual text";
				string expected = "expected other text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "expected other text",
					             but it was "actual text" which differs at index 0:
					                ↓ (actual)
					               "actual text"
					               "expected other text"
					                ↑ (expected)

					             Actual:
					             actual text
					             
					             Expected:
					             expected other text
					             """);
			}
		}

		public sealed class IgnoringIndentationTests
		{
			[Fact]
			public async Task ShouldIncludeCorrectLineAndColumnInMessage()
			{
				string subject = "foo\n    baz";
				string expected = "foo\nbar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringIndentation();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to "foo\nbar" ignoring indentation,
					              but it was "foo\nbaz" which differs on line 2 and column 7:
					                        ↓ (actual)
					                "foo\nbaz"
					                "foo\nbar"
					                        ↑ (expected)

					              Actual:
					              {subject}

					              Expected:
					              {expected}
					              """);
			}

			[Fact]
			public async Task WhenLineOnlyConsistsOfWhiteSpace_ShouldBecomeEmpty()
			{
				string subject = "foo\n   \nbar";
				string expected = "foo\n\nbar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("    foo", "foo")]
			[InlineData("\tfoo", "foo")]
			[InlineData("class C\n{\n    Foo();\n}", "class C\n{\nFoo();\n}")]
			[InlineData("class C\r\n{\r\n    Foo();\r\n}", "class C\n{\nFoo();\n}")]
			public async Task WhenStringsDifferOnlyInIndentation_ShouldSucceed(
				string subject, string expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTrailingWhiteSpaceDiffers_ShouldFail()
			{
				string subject = "foo  \nbar";
				string expected = "foo\nbar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringIndentation();

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenUsedAsPrefix_ShouldIgnoreIndentation()
			{
				string subject = "    some arbitrary\n        text";
				string expected = "some arbitrary\ntext";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix().IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedAsRegex_ShouldIgnoreIndentation()
			{
				string subject = "    some arbitrary text";
				string expected = "some .* text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsRegex().IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedAsSuffix_ShouldIgnoreIndentation()
			{
				string subject = "some arbitrary\n    text";
				string expected = "arbitrary\ntext";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix().IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsedAsWildcard_ShouldIgnoreIndentation()
			{
				string subject = "    some arbitrary text";
				string expected = "some * text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsWildcard().IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringLeadingWhiteSpaceTests
		{
			[Theory]
			[InlineAutoData("foo", " bar", 0)]
			[InlineAutoData(" foo", "bar", 1)]
			[InlineAutoData(" \tfoo", "bar", 2)]
			public async Task ShouldIncludeCorrectIndexInMessage(
				string subject, string expected, int index)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ignoring leading white-space,
					              but it was {Formatter.Format(subject.TrimStart())} which differs at index {index}:
					                 ↓ (actual)
					                "foo"
					                "bar"
					                 ↑ (expected)

					              Actual:
					              {subject}
					              
					              Expected:
					              {expected}
					              """);
			}

			[Theory]
			[InlineAutoData("foo\nbar", " \n bar", 1, 1)]
			[InlineAutoData(" \n\n foo", "bar", 3, 2)]
			[InlineAutoData(" \r\n \tfoo", "bar", 2, 3)]
			public async Task ShouldIncludeCorrectLineAndColumnInMessage(
				string subject, string expected, int line, int column)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ignoring leading white-space,
					              but it was {Formatter.Format(subject.TrimStart())} which differs on line {line} and column {column}:
					                 ↓ (actual)
					                {Formatter.Format(subject.TrimStart())}
					                {Formatter.Format(expected.TrimStart())}
					                 ↑ (expected)

					              Actual:
					              {subject}
					              
					              Expected:
					              {expected}
					              """);
			}

			[Theory]
			[InlineAutoData(" foo", "foo")]
			[InlineAutoData("foo", " foo")]
			[InlineAutoData("\tfoo", "\nfoo")]
			[InlineAutoData("\r\nfoo", "foo")]
			[InlineAutoData("foo", "\tfoo")]
			public async Task WhenStringsDifferOnlyInLeadingWhiteSpace_ShouldSucceed(
				string subject, string expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringLeadingWhiteSpace();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringNewlineStyleTests
		{
			[Fact]
			public async Task ShouldIncludeCorrectIndexInMessage()
			{
				string subject = "foo\nbar";
				string expected = "foo\r\nbaz";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringNewlineStyle();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to "foo\r\nbaz" ignoring newline style,
					              but it was "foo\nbar" which differs on line 2 and column 3:
					                        ↓ (actual)
					                "foo\nbar"
					                "foo\nbaz"
					                        ↑ (expected)

					              Actual:
					              {subject}
					              
					              Expected:
					              {expected}
					              """);
			}

			[Theory]
			[InlineAutoData("foo\nbar", "foo\rbar")]
			[InlineAutoData("foo\rbar", "foo\nbar")]
			[InlineAutoData("foo\nbar", "foo\r\nbar")]
			[InlineAutoData("foo\rbar", "foo\r\nbar")]
			[InlineAutoData("foo\r\nbar", "foo\nbar")]
			[InlineAutoData("foo\r\nbar", "foo\rbar")]
			public async Task WhenStringsDifferOnlyInNewlineStyle_ShouldSucceed(
				string subject, string expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringNewlineStyle();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringTrailingWhiteSpaceTests
		{
			[Fact]
			public async Task ShouldConsiderNewlineForSwitchingToLineColumn()
			{
				string subject = "foo-boo\nbaz\t";
				string expected = "foo-bar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringTrailingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to "foo-bar" ignoring trailing white-space,
					              but it was "foo-boo\nbaz" which differs on line 1 and column 6:
					                      ↓ (actual)
					                "foo-boo\nbaz"
					                "foo-bar"
					                      ↑ (expected)

					              Actual:
					              {subject}
					              
					              Expected:
					              foo-bar
					              """);
			}

			[Fact]
			public async Task ShouldIncludeCorrectIndexInMessage()
			{
				string subject = "foo-boo\t";
				string expected = "foo-bar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringTrailingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo-bar" ignoring trailing white-space,
					             but it was "foo-boo" which differs at index 5:
					                     ↓ (actual)
					               "foo-boo"
					               "foo-bar"
					                     ↑ (expected)

					             Actual:
					             foo-boo	
					             
					             Expected:
					             foo-bar
					             """);
			}

			[Theory]
			[InlineAutoData("foo ", "foo")]
			[InlineAutoData("foo", "foo ")]
			[InlineAutoData("foo\t", "foo\n")]
			[InlineAutoData("foo\r\n", "foo")]
			[InlineAutoData("foo", "foo\t")]
			public async Task WhenStringsDifferOnlyInTrailingWhiteSpace_ShouldSucceed(
				string subject, string expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringTrailingWhiteSpace();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
