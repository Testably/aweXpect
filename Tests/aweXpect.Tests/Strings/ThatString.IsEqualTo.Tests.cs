using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualAndExpectedAreNull_ShouldSucceed()
			{
				string? subject = null;
				string? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text",
					             but it was <null>
					             """);
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenCombiningIgnoringCaseWithAComparer_ShouldThrowInvalidOperationException(
				bool comparerFirst)
			{
				string subject = "ABC";

				async Task Act()
				{
					if (comparerFirst)
					{
						await That(subject).IsEqualTo("abc").Using(StringComparer.Ordinal).IgnoringCase();
					}
					else
					{
						await That(subject).IsEqualTo("abc").IgnoringCase().Using(StringComparer.Ordinal);
					}
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage(
						"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.")
					.Because("the comparer used to win silently, which also removed the casing from the message");
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenComparerIsNull_ShouldThrowArgumentNullException(bool negated)
			{
				string subject = "ABC";

				async Task Act()
				{
					if (negated)
					{
						await That(subject).IsNotEqualTo("abc").Using(null!);
					}
					else
					{
						await That(subject).IsEqualTo("abc").Using(null!);
					}
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("comparer").And
					.WithMessage("The 'comparer' cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenComparerIsSpecifiedTwice_ShouldThrowInvalidOperationException(bool negated)
			{
				string subject = "ABC";

				async Task Act()
				{
					if (negated)
					{
						await That(subject).IsNotEqualTo("abc")
							.Using(StringComparer.Ordinal).Using(StringComparer.OrdinalIgnoreCase);
					}
					else
					{
						await That(subject).IsEqualTo("abc")
							.Using(StringComparer.Ordinal).Using(StringComparer.OrdinalIgnoreCase);
					}
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Using cannot be specified more than once.")
					.Because("the second comparer would silently replace the first one");
			}

			[Test]
			public async Task WhenComparerThrowsForAPartOfTheValues_ShouldFailWithoutThePositionOfTheDifference()
			{
				string subject = "1.2.3";

				async Task Act()
					=> await That(subject).IsEqualTo("1.2.4").Using(new VersionComparer());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "1.2.4" using ThatString.IsEqualTo.Tests.VersionComparer,
					             but it was "1.2.3"
					             """)
					.Because("the comparer answered for the values and is only called for parts of them to locate the difference");
			}

			[Test]
			public async Task WhenComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsEqualTo("abc").Using(new ThrowingComparer(exception));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "abc" using ThatString.IsEqualTo.Tests.ThrowingComparer,
					             but the comparer did throw an InvalidOperationException:
					               comparer failed

					             Actual:
					             ABC
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Test]
			public async Task WhenCustomMatchTypeComparesByValue_AndBothAreNull_ShouldSucceed()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(null).AsCaseFolded();

				await That(Act).DoesNotThrow()
					.Because("a custom match type that does not inspect the subject compares a null subject as a value");
			}

			[Test]
			public async Task WhenCustomMatchTypeComparesByValue_AndOnlySubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo("abc").AsCaseFolded();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is case-folded equal to "abc",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldFail()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo("");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "",
					             but it was "some text" with a length of 9, which is longer than the expected length of 0 and has superfluous:
					               "some text"
					             """)
					.Because("comparing a string against the empty string is a legitimate expectation");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "some text";
				string? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to <null>,
					             but it was "some text"
					             """);
			}

			[Test]
			[Arguments("i", "I")]
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

			[Test]
			public async Task WhenStringHasMissingLeadingWhitespace_ShouldFail()
			{
				string subject = "some text";
				string expected = " \t some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to " \t some text",
					             but it was "some text", which misses some whitespace (" \t " at the beginning)
					             """);
			}

			[Test]
			public async Task WhenStringHasMissingTrailingWhitespace_ShouldFail()
			{
				string subject = "some text";
				string expected = "some text \t ";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text \t ",
					             but it was "some text", which misses some whitespace (" \t " at the end)
					             """);
			}

			[Test]
			public async Task WhenStringHasUnexpectedLeadingWhitespace_ShouldFail()
			{
				string subject = " \t some text";
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text",
					             but it was " \t some text", which has unexpected whitespace (" \t " at the beginning)
					             """);
			}

			[Test]
			public async Task WhenStringHasUnexpectedTrailingWhitespace_ShouldFail()
			{
				string subject = "some text \t ";
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text",
					             but it was "some text \t ", which has unexpected whitespace (" \t " at the end)
					             """);
			}

			[Test]
			public async Task WhenStringIsLonger_ShouldFail()
			{
				string subject = "some text without out";
				string expected = "some text with";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text with",
					             but it was "some text without out" with a length of 21, which is longer than the expected length of 14 and has superfluous:
					               "out out"
					             """);
			}

			[Test]
			public async Task WhenStringIsShorter_ShouldFail()
			{
				string subject = "some text with";
				string expected = "some text without out";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "some text without out",
					             but it was "some text with" with a length of 14, which is shorter than the expected length of 21 and misses:
					               "out out"
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenStringsAreTheSame_ShouldSucceed(string subject)
			{
				string expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringsDiffer_ShouldFail()
			{
				string subject = "actual text";
				string expected = "expected other text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "expected other text",
					             but it was "actual text", which differs at index 0:
					                ↓ (actual)
					               "actual text"
					               "expected other text"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenTheSameMatchTypeIsSpecifiedTwice_ShouldThrowInvalidOperationException()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsEqualTo("a.*").AsRegex().AsRegex(RegexOptions.IgnoreCase);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("AsRegex cannot be specified more than once.");
			}

			[Test]
			public async Task WhenTwoMatchTypesAreSpecified_ShouldThrowAtTheCall()
			{
				string subject = "abc";

				void Act()
#pragma warning disable aweXpect0001
					=> _ = That(subject).IsEqualTo("a*").AsWildcard().AsPrefix();
#pragma warning restore aweXpect0001

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("AsPrefix cannot be combined with AsWildcard.")
					.Because("the conflict is detected when the expectation is built, not when it is awaited");
			}

			[Test]
			public async Task WhenTwoMatchTypesAreSpecified_ShouldThrowInvalidOperationException()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsEqualTo("a*").AsWildcard().AsPrefix();

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("AsPrefix cannot be combined with AsWildcard.")
					.Because("the prefix would silently replace the wildcard");
			}

			private sealed class ThrowingComparer(Exception exception) : IEqualityComparer<string>
			{
				public bool Equals(string? x, string? y)
					=> throw exception;

				public int GetHashCode(string obj)
					=> obj.GetHashCode();
			}

			/// <summary>
			///     Compares strings as versions, and throws for a string that is not a complete version, like a part of one.
			/// </summary>
			private sealed class VersionComparer : IEqualityComparer<string>
			{
				public bool Equals(string? x, string? y)
					=> Version.Parse(x!).Equals(Version.Parse(y!));

				public int GetHashCode(string obj)
					=> Version.Parse(obj).GetHashCode();
			}
		}

		public sealed class IgnoringIndentationTests
		{
			[Test]
			public async Task ShouldIncludeCorrectLineAndColumnInMessage()
			{
				string subject = "foo\n    baz";
				string expected = "foo\nbar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringIndentation();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to "foo\nbar" ignoring indentation,
					              but it was "foo\nbaz", which differs on line 2 and column 7:
					                        ↓ (actual)
					                "foo\nbaz"
					                "foo\nbar"
					                        ↑ (expected)

					              Actual:
					              {subject}
					              """);
			}

			[Test]
			public async Task WhenLineOnlyConsistsOfWhiteSpace_ShouldBecomeEmpty()
			{
				string subject = "foo\n   \nbar";
				string expected = "foo\n\nbar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("    foo", "foo")]
			[Arguments("\tfoo", "foo")]
			[Arguments("class C\n{\n    Foo();\n}", "class C\n{\nFoo();\n}")]
			[Arguments("class C\r\n{\r\n    Foo();\r\n}", "class C\n{\nFoo();\n}")]
			public async Task WhenStringsDifferOnlyInIndentation_ShouldSucceed(
				string subject, string expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTrailingWhiteSpaceDiffers_ShouldFail()
			{
				string subject = "foo  \nbar";
				string expected = "foo\nbar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringIndentation();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo\nbar" ignoring indentation,
					             but it was "foo  \nbar", which differs on line 1 and column 4:
					                   ↓ (actual)
					               "foo  \nbar"
					               "foo\nbar"
					                   ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenUsedAsPrefix_ShouldIgnoreIndentation()
			{
				string subject = "    some arbitrary\n        text";
				string expected = "some arbitrary\ntext";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix().IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUsedAsRegex_ShouldIgnoreIndentation()
			{
				string subject = "    some arbitrary text";
				string expected = "some .* text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsRegex().IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUsedAsSuffix_ShouldIgnoreIndentation()
			{
				string subject = "some arbitrary\n    text";
				string expected = "arbitrary\ntext";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix().IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Test]
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
			[Test]
			[AutoArguments(" foo", "bar", 1)]
			[AutoArguments(" \tfoo", "bar", 2)]
			public async Task ShouldIncludeCorrectIndexInMessage(
				string subject, string expected, int index)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ignoring leading whitespace,
					              but it was {Formatter.Format(subject.TrimStart())}, which differs at index {index}:
					                 ↓ (actual)
					                "foo"
					                "bar"
					                 ↑ (expected)

					              Actual:
					              {subject}
					              """);
			}

			[Test]
			public async Task ShouldIncludeCorrectIndexInMessage_WhenOnlyExpectedHasLeadingWhiteSpace()
			{
				string subject = "foo";
				string expected = " bar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to " bar" ignoring leading whitespace,
					             but it was "foo", which differs at index 0:
					                ↓ (actual)
					               "foo"
					               "bar"
					                ↑ (expected)
					             """);
			}

			[Test]
			[AutoArguments(" \n\n foo", "bar", 3, 2)]
			[AutoArguments(" \r\n \tfoo", "bar", 2, 3)]
			public async Task ShouldIncludeCorrectLineAndColumnInMessage(
				string subject, string expected, int line, int column)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ignoring leading whitespace,
					              but it was {Formatter.Format(subject.TrimStart())}, which differs on line {line} and column {column}:
					                 ↓ (actual)
					                {Formatter.Format(subject.TrimStart())}
					                {Formatter.Format(expected.TrimStart())}
					                 ↑ (expected)

					              Actual:
					              {subject}
					              """);
			}

			[Test]
			public async Task ShouldIncludeCorrectLineAndColumnInMessage_WhenOnlyExpectedHasLeadingWhiteSpace()
			{
				string subject = "foo\nbar";
				string expected = " \n bar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to " \n bar" ignoring leading whitespace,
					             but it was "foo\nbar", which differs on line 1 and column 1:
					                ↓ (actual)
					               "foo\nbar"
					               "bar"
					                ↑ (expected)
					             """);
			}

			[Test]
			[AutoArguments(" foo", "foo")]
			[AutoArguments("foo", " foo")]
			[AutoArguments("\tfoo", "\nfoo")]
			[AutoArguments("\r\nfoo", "foo")]
			[AutoArguments("foo", "\tfoo")]
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
			[Test]
			public async Task ShouldIncludeCorrectIndexInMessage()
			{
				string subject = "foo\nbar";
				string expected = "foo\r\nbaz";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringNewlineStyle();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo\r\nbaz" ignoring newline style,
					             but it was "foo\nbar", which differs on line 2 and column 3:
					                       ↓ (actual)
					               "foo\nbar"
					               "foo\nbaz"
					                       ↑ (expected)
					             """);
			}

			[Test]
			[AutoArguments("foo\nbar", "foo\rbar")]
			[AutoArguments("foo\rbar", "foo\nbar")]
			[AutoArguments("foo\nbar", "foo\r\nbar")]
			[AutoArguments("foo\rbar", "foo\r\nbar")]
			[AutoArguments("foo\r\nbar", "foo\nbar")]
			[AutoArguments("foo\r\nbar", "foo\rbar")]
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
			[Test]
			public async Task ShouldConsiderNewlineForSwitchingToLineColumn()
			{
				string subject = "foo-boo\nbaz\t";
				string expected = "foo-bar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringTrailingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to "foo-bar" ignoring trailing whitespace,
					              but it was "foo-boo\nbaz", which differs on line 1 and column 6:
					                      ↓ (actual)
					                "foo-boo\nbaz"
					                "foo-bar"
					                      ↑ (expected)

					              Actual:
					              {subject}
					              """);
			}

			[Test]
			public async Task ShouldIncludeCorrectIndexInMessage()
			{
				string subject = "foo-boo\t";
				string expected = "foo-bar";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringTrailingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo-bar" ignoring trailing whitespace,
					             but it was "foo-boo", which differs at index 5:
					                     ↓ (actual)
					               "foo-boo"
					               "foo-bar"
					                     ↑ (expected)

					             Actual:
					             foo-boo	
					             """);
			}

			[Test]
			[AutoArguments("foo ", "foo")]
			[AutoArguments("foo", "foo ")]
			[AutoArguments("foo\t", "foo\n")]
			[AutoArguments("foo\r\n", "foo")]
			[AutoArguments("foo", "foo\t")]
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
