using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class StringTests
	{
		[Fact]
		public async Task Strings_InCollection_ShouldKeepItemsApart()
		{
			string[] value = ["a\", \"b",];

			string result = Formatter.Format(value);

			await That(result).IsEqualTo("""
			                             ["a\", \"b"]
			                             """)
				.Because("a single item with quotes must not read like two items");
		}

		[Fact]
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

		[Theory]
		[InlineData("a\\nb", "\"a\\\\nb\"")]
		[InlineData("a\0b", "\"a\\0b\"")]
		[InlineData("a\u00A0b", "\"a\\u00A0b\"")]
		[InlineData("a\u200Bb", "\"a\\u200Bb\"")]
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

		[Fact]
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

		[Fact]
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

		[Fact]
		public async Task Strings_WhenUsingMultipleLines_ShouldUseNotEscapeNewlines()
		{
			string value = $"a{Environment.NewLine}b";
			string expectedResult = """
			                        "a
			                        b"
			                        """;
			StringBuilder sb = new();

			string result =
				Formatter.Format(value, FormattingOptions.MultipleLines);
			Formatter.Format(sb, value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
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

		[Fact]
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
