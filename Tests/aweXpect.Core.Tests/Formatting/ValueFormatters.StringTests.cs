using System.Text;
using aweXpect.Customization;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class StringTests
	{
		[Test]
		public async Task InFailureMessage_WhenStringsDifferByACombiningMark_ShouldShowTheDifference()
		{
			string subject = "e\u0301x";

			async Task Act()
				=> await That(subject).IsEqualTo("\u00E9x");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to "éx",
				             but it was "e\u0301x", which differs at index 0:
				                ↓ (actual)
				               "e\u0301x"
				               "éx"
				                ↑ (expected)
				             """);
		}

		[Test]
		public async Task InFailureMessage_WhenStringContainsAnUnpairedSurrogate_ShouldEscapeIt()
		{
			string subject = "\uD83D";

			async Task Act()
				=> await That(subject).IsEqualTo("x");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to "x",
				             but it was "\uD83D", which differs at index 0:
				                ↓ (actual)
				               "\uD83D"
				               "x"
				                ↑ (expected)
				             """)
				.Because("an unpaired surrogate is no valid text and cannot be written by a strict encoder");
		}

		[Test]
		public async Task Strings_InCollection_ShouldKeepItemsApart()
		{
			string[] value = ["a\", \"b",];

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("""
			                             ["a\", \"b"]
			                             """)
				.Because("a single item with quotes must not read like two items");
		}

		[Test]
		public async Task Strings_ShouldDefaultToSingleLine()
		{
			string value = "a\nb";
			string expectedResult = """
			                        "a\nb"
			                        """;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		[Arguments("a\\nb", "\"a\\\\nb\"")]
		[Arguments("a\0b", "\"a\\0b\"")]
		[Arguments("a\u00A0b", "\"a\\u00A0b\"")]
		[Arguments("a\u200Bb", "\"a\\u200Bb\"")]
		[Arguments("e\u0301", "\"e\\u0301\"")]
		[Arguments("\u0301e", "\"\\u0301e\"")]
		public async Task Strings_ShouldEscapeBackslashesAndInvisibleCharacters(string value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			string withTypeResult = Formatter.Format(value, FormattingOptions.WithType);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("a backslash must not be confused with an escape and invisible characters must be visible");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(withTypeResult).IsEqualTo($"string {expectedResult}");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Strings_ShouldEscapeDoubleQuotationMarks()
		{
			string value = "a\"b'c";
			string expectedResult = """
			                        "a\"b'c"
			                        """;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("an unescaped double quote would end the string early, while a single quote cannot");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Strings_ShouldEscapeUnpairedSurrogatesButKeepSurrogatePairs()
		{
			string value = "a\uD83Db\uDE00c\uD83D\uDE00d\uD83D";
			string expectedResult = "\"a\\uD83Db\\uDE00c\uD83D\uDE00d\\uD83D\"";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("an unpaired surrogate is no valid text and cannot be written by a strict encoder");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Strings_ShouldUseDoubleQuotationMarks()
		{
			string value = "foo";
			string expectedResult = """
			                        "foo"
			                        """;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		[Arguments("\u0928\u092E\u0938\u094D\u0924\u0947")]
		[Arguments("\u2764\uFE0F")]
		public async Task Strings_WhenNormalized_ShouldKeepCombiningMarks(string value)
		{
			string result = Formatter.Format(value);

			await That(result).IsEqualTo($"\"{value}\"")
				.Because("a combining mark in normalized text cannot be confused with a precomposed character");
		}

		[Test]
		public async Task Strings_WhenTruncated_ShouldNotSplitEscapedWhitespace()
		{
			string value = "abcd\nefgh";
			string expectedResult = """
			                        "abcd\n…"
			                        """;
			StringBuilder sb = new();

			string result;
			string objectResult;
			string typeResult;
			using (Customize.aweXpect.Formatting().MaximumStringLength.Set(5))
			{
				result = Formatter.Format(value);
				objectResult = Formatter.Format((object?)value);
				typeResult = Formatter.Format(value, FormattingOptions.WithType);
				Formatter.Format(sb, value);
			}

			await That(result).IsEqualTo(expectedResult)
				.Because("the limit applies to the characters of the value, not to their escaped form");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(typeResult).IsEqualTo($"string {expectedResult}");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Strings_WhenTruncated_ShouldNotSplitSurrogatePairs()
		{
			string value = "abcd\U0001F600efgh";
			string expectedResult = "\"abcd…\"";
			StringBuilder sb = new();

			string result;
			string objectResult;
			using (Customize.aweXpect.Formatting().MaximumStringLength.Set(5))
			{
				result = Formatter.Format(value);
				objectResult = Formatter.Format((object?)value);
				Formatter.Format(sb, value);
			}

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Strings_WhenUsingMultipleLines_ShouldStillEscapeThemOnASingleLine()
		{
			string value = "say \"hi\"\r\nbye";
			string expectedResult = """
			                        "say \"hi\"\r\nbye"
			                        """;
			StringBuilder sb = new();
			StringBuilder typeSb = new();

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.MultipleLines);
			string typeResult = Formatter.Format(value, FormattingOptions.MultipleLines with
			{
				IncludeType = true,
			});
			Formatter.Format(sb, value, FormattingOptions.MultipleLines);
			Formatter.Format(typeSb, value, FormattingOptions.MultipleLines with
			{
				IncludeType = true,
			});

			await That(result).IsEqualTo(expectedResult)
				.Because("a raw quote or line break would break the layout of the message, just as for a nested string");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(typeResult).IsEqualTo($"string {expectedResult}");
			await That(sb.ToString()).IsEqualTo(expectedResult);
			await That(typeSb.ToString()).IsEqualTo($"string {expectedResult}");
		}

		[Test]
		public async Task Strings_WhenUsingMultipleLines_ShouldTruncateThem()
		{
			string value = "abcdefgh";
			string expectedResult = "\"abcde…\"";
			StringBuilder sb = new();

			string result;
			string objectResult;
			using (Customize.aweXpect.Formatting().MaximumStringLength.Set(5))
			{
				result = Formatter.Format(value, FormattingOptions.MultipleLines);
				objectResult = Formatter.Format((object?)value, FormattingOptions.MultipleLines);
				Formatter.Format(sb, value, FormattingOptions.MultipleLines);
			}

			await That(result).IsEqualTo(expectedResult)
				.Because("the maximum length applies to a string, however the caller wants it formatted");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Strings_WithType_ShouldIncludeTypeInformation()
		{
			string value = "foo";
			string expectedResult = "string \"foo\"";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenNull_ShouldUseDefaultNullString()
		{
			string? value = null;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(ValueFormatter.NullString);
			await That(objectResult).IsEqualTo(ValueFormatter.NullString);
			await That(sb.ToString()).IsEqualTo(ValueFormatter.NullString);
		}
	}
}
