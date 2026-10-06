using System.Text;
using aweXpect.Chronology;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class DateTimeOffsetTests
	{
		[Test]
		public async Task Nullable_ShouldUseRoundtripFormat()
		{
			DateTimeOffset? value = 2.November(2024).At(15, 42, 08, 123).WithOffset(3.Hours());
			string expectedResult = "2024-11-02T15:42:08.1230000+03:00";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Nullable_WithType_ShouldUseRoundtripFormat()
		{
			DateTimeOffset? value = 2.November(2024).At(15, 42, 08, 123).WithOffset(3.Hours());
			string expectedResult = "DateTimeOffset 2024-11-02T15:42:08.1230000+03:00";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task ShouldUseRoundtripFormat()
		{
			DateTimeOffset value = 2.November(2024).At(15, 42, 08, 123).WithOffset(3.Hours());
			string expectedResult = "2024-11-02T15:42:08.1230000+03:00";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenNull_ShouldUseDefaultNullString()
		{
			DateTimeOffset? value = null;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(ValueFormatter.NullString);
			await That(objectResult).IsEqualTo(ValueFormatter.NullString);
			await That(sb.ToString()).IsEqualTo(ValueFormatter.NullString);
		}

		[Test]
		public async Task WithType_ShouldUseRoundtripFormat()
		{
			DateTimeOffset value = 2.November(2024).At(15, 42, 08, 123).WithOffset(3.Hours());
			string expectedResult = "DateTimeOffset 2024-11-02T15:42:08.1230000+03:00";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}
	}
}
