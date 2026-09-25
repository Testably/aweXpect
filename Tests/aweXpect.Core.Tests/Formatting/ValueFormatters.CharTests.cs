using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class CharTests
	{
		[Fact]
		public async Task NullableValue_ShouldAddSingleQuotes()
		{
			char? value = 'X';
			string expectedResult = "'X'";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task NullableValue_WithType_ShouldAddSingleQuotes()
		{
			char? value = 'X';
			string expectedResult = "char 'X'";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task Value_ShouldAddSingleQuotes()
		{
			char value = 'a';
			string expectedResult = "'a'";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Theory]
		[InlineData('\n', "'\\n'")]
		[InlineData('\r', "'\\r'")]
		[InlineData('\t', "'\\t'")]
		[InlineData('\'', "'''")]
		[InlineData('"', "'\"'")]
		public async Task Value_ShouldDisplayWhitespaceLikeSingleLineStrings(char value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			string withTypeResult = Formatter.Format(value, FormattingOptions.WithType);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("line breaks and tabs are escaped like in single-line strings, while quotes are not");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(withTypeResult).IsEqualTo($"char {expectedResult}");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task Value_WithType_ShouldAddSingleQuotes()
		{
			char value = 'a';
			string expectedResult = "char 'a'";
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
			char? value = null;
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
