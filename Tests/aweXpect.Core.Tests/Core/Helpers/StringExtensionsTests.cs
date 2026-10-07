using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public class StringExtensionsTests
{
	public sealed class DisplayWhitespace
	{
		[Test]
		[Arguments("a\0b", @"a\0b")]
		[Arguments("\u0001\u001F", @"\u0001\u001F")]
		[Arguments("\u007F\u0085", @"\u007F\u0085")]
		[Arguments("a\u00A0b", @"a\u00A0b")]
		[Arguments("a\u200Bb", @"a\u200Bb")]
		[Arguments("\u00AD\u200D\u2060\uFEFF", @"\u00AD\u200D\u2060\uFEFF")]
		[Arguments("\u2007\u202F\u3000", @"\u2007\u202F\u3000")]
		[Arguments("\u2028\u2029", @"\u2028\u2029")]
		public async Task ShouldEscapeControlAndInvisibleCharacters(string input, string expected)
		{
			string result = input.DisplayWhitespace();

			await That(result).IsEqualTo(expected)
				.Because("characters that are invisible or look like a plain space must be told apart in a message");
		}

		[Test]
		public async Task ShouldEscapeNewlines()
		{
			string input = "\r,\n;\t ";
			string expected = @"\r,\n;\t ";

			string result = input.DisplayWhitespace();

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task ShouldKeepBackslashesQuotesAndVisibleCharacters()
		{
			string input = "C:\\temp \"a\" 'b' äß€😀";

			string result = input.DisplayWhitespace();

			await That(result).IsEqualTo(input)
				.Because("unquoted text like an exception message is not a literal, so a path keeps its backslashes");
		}

		[Test]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.DisplayWhitespace();

			await That(result).IsNull();
		}

		[Test]
		public async Task WhenTheNormalizationCannotBeChecked_ShouldEscapeCombiningMarks()
		{
			string input = new(['a', (char)0x0301, (char)0xD800,]);

			string result = input.DisplayWhitespace();

			await That(result).IsEqualTo("a\\u" + "0301\\uD800")
				.Because("a value whose normalization cannot be checked counts as not normalized");
		}
	}

	public sealed class Escape
	{
		[Test]
		public async Task ShouldEscapeBackslash()
		{
			string input = "a\\nb";
			string expected = @"a\\nb";

			string result = input.Escape();

			await That(result).IsEqualTo(expected)
				.Because("a backslash in the value must not be confused with an escaped newline");
		}

		[Test]
		public async Task ShouldEscapeControlAndInvisibleCharacters()
		{
			string input = "\r\n\t\0\u00A0\u200B";
			string expected = @"\r\n\t\0\u00A0\u200B";

			string result = input.Escape();

			await That(result).IsEqualTo(expected);
		}

		[Test]
		[Arguments('"', "a\"b'c", "a\\\"b'c")]
		[Arguments('\'', "a\"b'c", "a\"b\\'c")]
		public async Task ShouldOnlyEscapeTheGivenQuote(char quote, string input, string expected)
		{
			string result = input.Escape(quote);

			await That(result).IsEqualTo(expected)
				.Because("only the quote that encloses the value can end it early, like in a C# literal");
		}

		[Test]
		public async Task WhenNothingToEscape_ShouldReturnSameInstance()
		{
			string input = "foo bar äß😀";

			string result = input.Escape();

			await That(result).IsSameAs(input);
		}

		[Test]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.Escape();

			await That(result).IsNull();
		}
	}

	public sealed class IsSplitAt
	{
		[Test]
		[Arguments("", 0, false)]
		[Arguments("a\r\nb", 0, false)]
		[Arguments("a\r\nb", 1, false)]
		[Arguments("a\r\nb", 2, true)]
		[Arguments("a\r\nb", 3, false)]
		[Arguments("a\n\rb", 2, false)]
		[Arguments("a😀b", 1, false)]
		[Arguments("a😀b", 2, true)]
		[Arguments("a😀b", 3, false)]
		[Arguments("a😀", 3, false)]
		public async Task ShouldReturnExpectedResult(string input, int index, bool expected)
		{
			bool result = input.IsSplitAt(index);

			await That(result).IsEqualTo(expected);
		}
	}

	public sealed class RemoveIndentation
	{
		[Test]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.RemoveIndentation();

			await That(result).IsNull();
		}
	}

	public sealed class RemoveNewlineStyle
	{
		[Test]
		public async Task ShouldReplaceNewlinesWithSlashN()
		{
			string input = "\ra\r\nb\nc";
			string expected = "\na\nb\nc";

			string result = input.RemoveNewlineStyle();

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.RemoveNewlineStyle();

			await That(result).IsNull();
		}
	}

	public sealed class SubstringUntilFirst
	{
		[Test]
		public async Task WhenFirstCharacter_ShouldReturnEmptyString()
		{
			string input = "a,b,c";

			string result = input.SubstringUntilFirst('a');

			await That(result).IsEqualTo("");
		}


		[Test]
		public async Task WhenNotPresent_ShouldReturnString()
		{
			string input = "foo";

			string result = input.SubstringUntilFirst('X');

			await That(result).IsEqualTo(input);
		}

		[Test]
		public async Task WhenPresent_ShouldReturnSubstringUntilFirstOccurrence()
		{
			string input = "a,b,c";

			string result = input.SubstringUntilFirst(',');

			await That(result).IsEqualTo("a");
		}
	}

	public sealed class TruncateWithEllipsis
	{
		[Test]
		public async Task WhenCutWouldSplitALineBreak_ShouldCutBeforeTheLineBreak()
		{
			string input = "abcd\r\nefgh";

			string result = input.TruncateWithEllipsis(5);

			await That(result).IsEqualTo("abcd…")
				.Because("a lone \\r would suggest that the value contains no \\n");
		}

		[Test]
		public async Task WhenCutWouldSplitASurrogatePair_ShouldCutBeforeThePair()
		{
			string input = "abcd\U0001F600efgh";

			string result = input.TruncateWithEllipsis(5);

			await That(result).IsEqualTo("abcd…")
				.Because("a lone high surrogate is not a valid character");
		}

		[Test]
		public async Task WhenLonger_ShouldTruncateWithEllipsis()
		{
			string input = "12345678910";
			string expected = "1234567891…";

			string result = input.TruncateWithEllipsis(10);

			await That(result).IsEqualTo(expected);
		}


		[Test]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.TruncateWithEllipsis(10);

			await That(result).IsNull();
		}

		[Test]
		public async Task WhenShorter_ShouldReturnInput()
		{
			string input = "1234567890";

			string result = input.TruncateWithEllipsis(10);

			await That(result).IsEqualTo(input);
		}
	}

	public sealed class TruncateWithEllipsisOnWord
	{
		[Test]
		public async Task WhenCutWouldSplitALineBreak_ShouldCutBeforeTheLineBreak()
		{
			string input = $"{new string('a', 29)}\r\nb";

			string result = input.TruncateWithEllipsisOnWord(30);

			await That(result).IsEqualTo($"{new string('a', 29)}…")
				.Because("a lone \\r would suggest that the value contains no \\n");
		}

		[Test]
		public async Task WhenCutWouldSplitASurrogatePair_ShouldCutBeforeThePair()
		{
			string input = $"{new string('a', 29)}\U0001F600b";

			string result = input.TruncateWithEllipsisOnWord(30);

			await That(result).IsEqualTo($"{new string('a', 29)}…")
				.Because("a lone high surrogate is not a valid character");
		}

		[Test]
		public async Task WhenLongerWithoutWordBoundary_ShouldTruncateOnWordWithEllipsis()
		{
			string input = "some word boundary";
			string expected = "some word…";

			string result = input.TruncateWithEllipsisOnWord(11);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task WhenLongerWithoutWordBoundary_ShouldTruncateWithEllipsis()
		{
			string input = "12345678910";
			string expected = "1234567891…";

			string result = input.TruncateWithEllipsisOnWord(10);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task WhenNull_ShouldReturnNull()
		{
			string? input = null;

			string? result = input.TruncateWithEllipsisOnWord(10);

			await That(result).IsNull();
		}

		[Test]
		public async Task WhenShorter_ShouldReturnInput()
		{
			string input = "1234567890";

			string result = input.TruncateWithEllipsisOnWord(10);

			await That(result).IsEqualTo(input);
		}

		[Test]
		[Arguments("another word-boundary", "another wo…")]
		[Arguments("_another word-boundary", "_another…")]
		public async Task WhenWordBoundaryIsBelow80Percent_ShouldTruncateWithEllipsis(
			string input, string expected)
		{
			string result = input.TruncateWithEllipsisOnWord(10);

			await That(result).IsEqualTo(expected);
		}
	}
}
