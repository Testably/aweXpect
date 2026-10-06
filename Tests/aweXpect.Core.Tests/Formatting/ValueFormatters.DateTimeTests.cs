using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class DateTimeTests
	{
		[Test]
		public async Task Nullable_ShouldUseRoundtripFormat()
		{
			DateTime? value = new(2024, 11, 2, 15, 42, 08, 123);
			string expectedResult = "2024-11-02T15:42:08.1230000";
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
			DateTime? value = new(2024, 11, 2, 15, 42, 08, 123);
			string expectedResult = "DateTime 2024-11-02T15:42:08.1230000";
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
			DateTime value = new(2024, 11, 2, 15, 42, 08, 123);
			string expectedResult = "2024-11-02T15:42:08.1230000";
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
			DateTime? value = null;
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
			DateTime value = new(2024, 11, 2, 15, 42, 08, 123);
			string expectedResult = "DateTime 2024-11-02T15:42:08.1230000";
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
