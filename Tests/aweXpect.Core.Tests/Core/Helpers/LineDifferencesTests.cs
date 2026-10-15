using System.Linq;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public class LineDifferencesTests
{
	public sealed class FormatTests
	{
		[Test]
		public async Task ShouldListTheDifferencesWithOneLineOfContext()
		{
			string actual = "[server]\nhost = localhost\nport = 8080\ntimeout = 30\n[client]\nretries = 3\nverbose = true\nlog = file";
			string expected = "[server]\nhost = localhost\nport = 8081\ntimeout = 30\n[client]\nretries = 3\nlog = console";

			string? result = Format(actual, expected);

			await That(result).IsEqualTo("""
			                                  2   2    host = localhost
			                               -  3        port = 8080
			                               +      3    port = 8081
			                                  4   4    timeout = 30
			                                  5   5    [client]
			                                  6   6    retries = 3
			                               -  7        verbose = true
			                               -  8        log = file
			                               +      7    log = console
			                             """);
		}

		[Test]
		public async Task WhenDifferencesAreFarApart_ShouldSeparateThemWithAnEllipsis()
		{
			string actual = "a\nb\nc\nd\ne\nf\ng";
			string expected = "X\nb\nc\nd\ne\nf\nY";

			string? result = Format(actual, expected);

			await That(result).IsEqualTo("""
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
		public async Task WhenAtMostTwoEqualLinesAreBetweenTheShownLines_ShouldShowThemTogether()
		{
			string actual = "a\nb\nc\nd\ne\nf";
			string expected = "X\nb\nc\nd\ne\nY";

			string? result = Format(actual, expected);

			await That(result).IsEqualTo("""
			                               -  1        a
			                               +      1    X
			                                  2   2    b
			                                  3   3    c
			                                  4   4    d
			                                  5   5    e
			                               -  6        f
			                               +      6    Y
			                             """);
		}

		[Test]
		public async Task WhenLinesAreOnlyAddedOrRemoved_ShouldLeaveTheOtherLineNumberBlank()
		{
			string actual = "a\nb\nc\nd\ne\nf";
			string expected = "a\nX\nb\nc\nd\nf";

			string? result = Format(actual, expected);

			await That(result).IsEqualTo("""
			                                  1   1    a
			                               +      2    X
			                                  2   3    b
			                                  3   4    c
			                                  4   5    d
			                               -  5        e
			                                  6   6    f
			                             """);
		}

		[Test]
		public async Task WhenLineNumbersHaveSeveralDigits_ShouldAlignThem()
		{
			string actual = string.Join("\n", Enumerable.Range(1, 12).Select(i => $"line {i}"));
			string expected = actual.Replace("line 8", "line X").Replace("line 12", "line Y");

			string? result = Format(actual, expected);

			await That(result).IsEqualTo("""
			                                   7    7    line 7
			                               -   8         line 8
			                               +        8    line X
			                                   9    9    line 9
			                                  10   10    line 10
			                                  11   11    line 11
			                               -  12         line 12
			                               +       12    line Y
			                             """);
		}

		[Test]
		public async Task WhenLinesAreReplacedByMoreLines_ShouldListTheRemovedLinesFirst()
		{
			string actual = "a\nb\nc\nd\ne\nf";
			string expected = "a\nB1\nB2\nB3\nc\nd\nE\nf";

			string? result = Format(actual, expected);

			await That(result).IsEqualTo("""
			                                  1   1    a
			                               -  2        b
			                               +      2    B1
			                               +      3    B2
			                               +      4    B3
			                                  3   5    c
			                                  4   6    d
			                               -  5        e
			                               +      7    E
			                                  6   8    f
			                             """);
		}

		[Test]
		public async Task WhenALineIsEmpty_ShouldNotAddTrailingWhiteSpace()
		{
			string actual = "a\n\nb\nc";
			string expected = "a\nb\nC\n";

			string? result = Format(actual, expected);

			await That(result).IsEqualTo("""
			                                  1   1    a
			                               -  2
			                                  3   2    b
			                               -  4        c
			                               +      3    C
			                               +      4
			                             """);
		}

		[Test]
		public async Task WhenThereAreMoreDifferencesThanTheMaximum_ShouldIndicateTheNumberOfMissingDifferences()
		{
			string actual = "a\nb\nc\nd\ne\nf\ng\nh\ni\nj";
			string expected = "X\nb\nc\nd\nY\nf\ng\nh\nZ\nj";

			string? result = Format(actual, expected, 1);

			await That(result).IsEqualTo("""
			                               -  1        a
			                               +      1    X
			                                  2   2    b
			                               (… and 2 more)
			                             """);
		}

		[Test]
		public async Task WhenTheMaximumIsZero_ShouldOnlyIndicateTheNumberOfDifferences()
		{
			string? result = Format("a\nb\nc", "X\nb\nY", 0);

			await That(result).IsEqualTo("  (… and 2 more)");
		}

		[Test]
		[Arguments("a\nb\nc", "a\nb\nc")]
		[Arguments("a\nb\nc", "a\nX\nc")]
		[Arguments("a\nb\nc", "X\nY\nZ")]
		[Arguments("a\nb\nc", "a\nb\nc\nd\ne")]
		[Arguments("", "a\nb")]
		public async Task WhenThereIsAtMostOneDifference_ShouldReturnNull(string actual, string expected)
		{
			string? result = Format(actual, expected);

			await That(result).IsNull();
		}

		[Test]
		public async Task WhenTooManyLinesDiffer_ShouldReturnNull()
		{
			int count = (LineDifferences.MaximumNumberOfChangedLines / 4) + 1;
			string actual = Block("a") + "\nsame\n" + Block("c");
			string expected = Block("b") + "\nsame\n" + Block("d");

			string? result = Format(actual, expected);

			await That(result).IsNull()
				.Because("the two differences change more lines than the diff compares");

			string Block(string prefix)
				=> string.Join("\n", Enumerable.Range(0, count).Select(i => $"{prefix}{i}"));
		}

		[Test]
		public async Task WhenManyLinesDiffer_ShouldListTheMaximumNumberOfDifferences()
		{
			string actual = string.Join("\n", Enumerable.Range(0, 20000).Select(i => $"line {i}"));
			string expected = string.Join("\n",
				Enumerable.Range(0, 20000).Select(i => i % 100 == 50 ? $"changed {i}" : $"line {i}"));

			string? result = Format(actual, expected, 2);

			await That(result).IsEqualTo("""
			                                   50    50    line 49
			                               -   51          line 50
			                               +         51    changed 50
			                                   52    52    line 51
			                               …
			                                  150   150    line 149
			                               -  151          line 150
			                               +        151    changed 150
			                                  152   152    line 151
			                               (… and 198 more)
			                             """);
		}

		private static string? Format(string actual, string expected, int maximumNumberOfDifferences = 10)
			=> LineDifferences.Format(LineDifferences.Split(actual), LineDifferences.Split(expected),
				(actualLine, expectedLine) => actualLine.ComparedText == expectedLine.ComparedText,
				maximumNumberOfDifferences);
	}

	public sealed class SplitTests
	{
		[Test]
		public async Task ShouldSplitAtEveryLineBreakStyle()
		{
			string value = "a\nb\r\nc\rd";

			LineDifferences.Line[] result = LineDifferences.Split(value).ToArray();

			await That(result.Select(line => line.Text)).IsEqualTo(["a", "b", "c", "d",]);
			await That(result.Select(line => line.LineBreak)).IsEqualTo(["\n", "\r\n", "\r", "",]);
			await That(result.Select(line => line.Number)).IsEqualTo([1, 2, 3, 4,]);
		}

		[Test]
		[Arguments("", 1)]
		[Arguments("a\n", 2)]
		[Arguments("\r\n\r\n", 3)]
		[Arguments("\n\r", 3)]
		public async Task ShouldEndWithTheTextAfterTheLastLineBreak(string value, int expectedCount)
		{
			LineDifferences.Line[] result = LineDifferences.Split(value).ToArray();

			await That(result).HasCount(expectedCount);
			await That(result[^1].Text).IsEmpty();
			await That(result[^1].LineBreak).IsEmpty();
		}
	}
}
