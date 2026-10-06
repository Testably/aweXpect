using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class GuidTests
	{
		[Test]
		public async Task Empty_ShouldUseDefaultFormat()
		{
			Guid value = Guid.Empty;
			string expectedResult = "00000000-0000-0000-0000-000000000000";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Nullable_Empty_ShouldUseDefaultFormat()
		{
			Guid? value = Guid.Empty;
			string expectedResult = "00000000-0000-0000-0000-000000000000";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Nullable_ShouldUseRoundtripFormat()
		{
			Guid? value = Guid.NewGuid();
			string? expectedResult = value.ToString();
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
			Guid? value = Guid.NewGuid();
			string expectedResult = $"Guid {value.ToString()}";
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
			Guid value = Guid.NewGuid();
			string expectedResult = value.ToString();
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
			Guid? value = null;
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
			Guid value = Guid.NewGuid();
			string expectedResult = $"Guid {value.ToString()}";
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
