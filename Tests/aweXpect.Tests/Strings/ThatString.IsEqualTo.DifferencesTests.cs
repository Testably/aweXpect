﻿using System.Collections.Generic;
using aweXpect.Customization;

namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class DifferencesTests
		{
			[Test]
			public async Task WhenMultiLineValuesDifferInOnePlace_ShouldNotListTheDifferences()
			{
				string subject = "[server]\nhost = localhost\nport = 8080\ntimeout = 30";
				string expected = "[server]\nhost = localhost\nport = 8081\ntimeout = 30";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to "[server]\nhost = localhost\nport…",
					              but it was "[server]\nhost = localhost\nport…", which differs on line 3 and column 11:
					                       ↓ (actual)
					                "…= 8080\ntimeout = 30"
					                "…= 8081\ntimeout = 30"
					                       ↑ (expected)

					              Actual:
					              {subject}

					              Expected:
					              {expected}
					              """);
			}

			[Test]
			public async Task WhenMultiLineValuesDifferInSeveralPlaces_ShouldListTheDifferencesBeforeActualAndExpected()
			{
				string subject =
					"[server]\nhost = localhost\nport = 8080\ntimeout = 30\n[client]\nretries = 3\nverbose = true\nlog = file";
				string expected =
					"[server]\nhost = localhost\nport = 8081\ntimeout = 30\n[client]\nretries = 3\nlog = console";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					             Expected that subject
					             is equal to "[server]\nhost = localhost\nport…",
					             but it was "[server]\nhost = localhost\nport…", which differs on line 3 and column 11:
					                      ↓ (actual)
					               "…= 8080\ntimeout = 30\n[client]\nretries = 3\nverbose = true\nlog =…"
					               "…= 8081\ntimeout = 30\n[client]\nretries = 3\nlog = console"
					                      ↑ (expected)

					             Differences:
					                  2   2    host = localhost
					               -  3        port = 8080
					               +      3    port = 8081
					                  4   4    timeout = 30
					                  5   5    [client]
					                  6   6    retries = 3
					               -  7        verbose = true
					               -  8        log = file
					               +      7    log = console

					             Actual:
					             {subject}

					             Expected:
					             {expected}
					             """);
			}

			[Test]
			public async Task WhenDifferencesAreFarApart_ShouldSeparateThemWithAnEllipsis()
			{
				string subject = "a\nb\nc\nd\ne\nf\ng";
				string expected = "X\nb\nc\nd\ne\nf\nY";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "X\nb\nc\nd\ne\nf\nY",
					             but it was "a\nb\nc\nd\ne\nf\ng", which differs on line 1 and column 1:
					                ↓ (actual)
					               "a\nb\nc\nd\ne\nf\ng"
					               "X\nb\nc\nd\ne\nf\nY"
					                ↑ (expected)

					             Differences:
					               -  1        a
					               +      1    X
					                  2   2    b
					               …
					                  6   6    f
					               -  7        g
					               +      7    Y
					             """);
			}

			[Test]
			public async Task WhenLinesAreOnlyAddedOrRemoved_ShouldListThem()
			{
				string subject = "a\nb\nc\nd\ne\nf\ng\nh";
				string expected = "X\na\nb\nc\nd\ne\nf\ng";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "X\na\nb\nc\nd\ne\nf\ng",
					             but it was "a\nb\nc\nd\ne\nf\ng\nh", which differs on line 1 and column 1:
					                ↓ (actual)
					               "a\nb\nc\nd\ne\nf\ng\nh"
					               "X\na\nb\nc\nd\ne\nf\ng"
					                ↑ (expected)

					             Differences:
					               +      1    X
					                  1   2    a
					               …
					                  7   8    g
					               -  8        h
					             """);
			}

			[Test]
			public async Task WhenThereAreMoreDifferencesThanTheMaximumNumberOfCollectionItems_ShouldIndicateTheRest()
			{
				string subject = "a1\nb\nc\nd\ne\na2\nb\nc\nd\ne\na3\nb\nc\nd\ne\na4";
				string expected = "x1\nb\nc\nd\ne\nx2\nb\nc\nd\ne\nx3\nb\nc\nd\ne\nx4";

				async Task Act()
				{
					using (Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(2))
					{
						await That(subject).IsEqualTo(expected);
					}
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "x1\nb\nc\nd\ne\nx2\nb\nc\nd\ne\nx3\nb\nc\nd…",
					             but it was "a1\nb\nc\nd\ne\na2\nb\nc\nd\ne\na3\nb\nc\nd…", which differs on line 1 and column 1:
					                ↓ (actual)
					               "a1\nb\nc\nd\ne\na2\nb\nc\nd\ne\na3\nb\nc\nd\ne\na4"
					               "x1\nb\nc\nd\ne\nx2\nb\nc\nd\ne\nx3\nb\nc\nd\ne\nx4"
					                ↑ (expected)

					             Differences:
					               -  1        a1
					               +      1    x1
					                  2   2    b
					                  3   3    c
					                  4   4    d
					                  5   5    e
					               -  6        a2
					               +      6    x2
					                  7   7    b
					               (… and 2 more)
					             """);
			}

			[Test]
			public async Task WhenLinesOnlyDifferInTheirLineBreak_ShouldListThem()
			{
				string subject = "a\r\nb\nc\nd\ne\r\nf";
				string expected = "a\nb\nc\nd\ne\nf";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "a\nb\nc\nd\ne\nf",
					             but it was "a\r\nb\nc\nd\ne\r\nf", which differs on line 1 and column 2:
					                 ↓ (actual)
					               "a\r\nb\nc\nd\ne\r\nf"
					               "a\nb\nc\nd\ne\nf"
					                 ↑ (expected)
					             (but the values match when ignoring the newline style)

					             Differences:
					               -  1        "a\r\n"
					               +      1    "a\n"
					                  2   2    b
					                  3   3    c
					                  4   4    d
					               -  5        "e\r\n"
					               +      5    "e\n"
					                  6   6    f
					             """);
			}

			[Test]
			public async Task WhenOnlyTheLastLineHasNoLineBreak_ShouldOnlyListTheAddedLine()
			{
				string subject = "a\nb\nc\nd\ne";
				string expected = "X\nb\nc\nd\ne\n";

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "X\nb\nc\nd\ne\n",
					             but it was "a\nb\nc\nd\ne", which differs on line 1 and column 1:
					                ↓ (actual)
					               "a\nb\nc\nd\ne"
					               "X\nb\nc\nd\ne\n"
					                ↑ (expected)

					             Differences:
					               -  1        a
					               +      1    X
					                  2   2    b
					                  3   3    c
					                  4   4    d
					                  5   5    e
					               +      6
					             """);
			}

			[Test]
			public async Task IgnoringCase_ShouldNotListLinesThatOnlyDifferInCase()
			{
				string subject = "A\nb\nC\nd\nE\nf";
				string expected = "a\nX\nc\nd\ne\nY";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "a\nX\nc\nd\ne\nY" ignoring case,
					             but it was "A\nb\nC\nd\nE\nf", which differs on line 2 and column 1:
					                   ↓ (actual)
					               "A\nb\nC\nd\nE\nf"
					               "a\nX\nc\nd\ne\nY"
					                   ↑ (expected)

					             Differences:
					                  1   1    A
					               -  2        b
					               +      2    X
					                  3   3    C
					                  4   4    d
					                  5   5    E
					               -  6        f
					               +      6    Y
					             """);
			}

			[Test]
			public async Task IgnoringNewlineStyle_ShouldNotListLinesThatOnlyDifferInTheirLineBreak()
			{
				string subject = "a\r\nb\r\nc\r\nd\r\ne\r\nf";
				string expected = "a\nX\nc\rd\ne\nY";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringNewlineStyle();

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).Contains("""
				                                       Differences:
				                                            1   1    a
				                                         -  2        b
				                                         +      2    X
				                                            3   3    c
				                                            4   4    d
				                                            5   5    e
				                                         -  6        f
				                                         +      6    Y
				                                       """);
			}

			[Test]
			public async Task IgnoringIndentation_ShouldNotListLinesThatOnlyDifferInTheirIndentation()
			{
				string subject = "a\n  b\r\n  c\n d\ne";
				string expected = "a\nX\nc\nd\nY";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringIndentation();

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).Contains("""
				                                       Differences:
				                                            1   1    a
				                                         -  2          b
				                                         +      2    X
				                                            3   3      c
				                                            4   4     d
				                                         -  5        e
				                                         +      5    Y
				                                       """);
			}

			[Test]
			public async Task IgnoringLeadingWhiteSpace_ShouldNotListTheLeadingWhiteSpace()
			{
				string subject = "\n  \n  a\nb\nc\nd";
				string expected = "a\nX\nc\nY";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringLeadingWhiteSpace();

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).Contains("""
				                                       Differences:
				                                            3   1      a
				                                         -  4        b
				                                         +      2    X
				                                            5   3    c
				                                         -  6        d
				                                         +      4    Y
				                                       """);
			}

			[Test]
			public async Task IgnoringTrailingWhiteSpace_ShouldNotListTheTrailingWhiteSpace()
			{
				string subject = "a\nb\nc\nd\n  \n";
				string expected = "X\nb\nY\nd";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringTrailingWhiteSpace();

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).Contains("""
				                                       Differences:
				                                         -  1        a
				                                         +      1    X
				                                            2   2    b
				                                         -  3        c
				                                         +      3    Y
				                                            4   4    d

				                                       Actual:
				                                       """);
			}

			[Test]
			public async Task Using_ShouldNotListTheDifferences()
			{
				string subject = "ab\nc\nX\nde\nf";
				string expected = "a\nbc\nY\nd\nef";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Using(new IgnoringLineBreaksComparer());

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).DoesNotContain("Differences:")
					.Because("the comparer compares whole values, which need not be equal line by line");
			}

			[Test]
			public async Task WhenNegated_ShouldNotListTheDifferences()
			{
				string subject = "ab\nc\nX\nde\nf";
				string unexpected = "a\nbc\nX\nd\nef";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Using(new IgnoringLineBreaksComparer());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to "a\nbc\nX\nd\nef" using ThatString.IsEqualTo.DifferencesTests.IgnoringLineBreaksComparer,
					             but it was "ab\nc\nX\nde\nf"
					             """);
			}

			[Test]
			public async Task AsPrefix_ShouldNotListTheDifferences()
			{
				string subject = "a\nb\nc\nd\ne\nf";
				string expected = "X\nb\nc\nY";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).DoesNotContain("Differences:");
			}

			[Test]
			public async Task AsSuffix_ShouldNotListTheDifferences()
			{
				string subject = "a\nb\nc\nd\ne\nf";
				string expected = "X\nd\ne\nY";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).DoesNotContain("Differences:");
			}

			[Test]
			public async Task AsWildcard_ShouldNotListTheDifferences()
			{
				string subject = "a\nb\nc\nd\ne";
				string expected = "X\nb\nc\nd\nY";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsWildcard();

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).DoesNotContain("Differences:");
			}

			[Test]
			public async Task AsRegex_ShouldNotListTheDifferences()
			{
				string subject = "a\nb\nc\nd\ne";
				string expected = "X\nb\nc\nd\nY";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsRegex();

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).DoesNotContain("Differences:");
			}

			[Test]
			public async Task StartsWith_ShouldNotListTheDifferences()
			{
				string subject = "a\nb\nc\nd\ne\nf";
				string expected = "X\nb\nc\nY";

				async Task Act()
					=> await That(subject).StartsWith(expected);

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).DoesNotContain("Differences:");
			}

			[Test]
			public async Task EndsWith_ShouldNotListTheDifferences()
			{
				string subject = "a\nb\nc\nd\ne\nf";
				string expected = "X\nd\ne\nY";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				FailException exception = await That(Act).Throws<FailException>();

				await That(exception.Message).DoesNotContain("Differences:");
			}

			/// <summary>
			///     Compares strings ordinally after removing their line breaks.
			/// </summary>
			private sealed class IgnoringLineBreaksComparer : IEqualityComparer<string>
			{
				public bool Equals(string? x, string? y)
					=> string.Equals(x!.Replace("\n", ""), y!.Replace("\n", ""), StringComparison.Ordinal);

				public int GetHashCode(string obj)
					=> obj.Replace("\n", "").GetHashCode();
			}
		}
	}
}
