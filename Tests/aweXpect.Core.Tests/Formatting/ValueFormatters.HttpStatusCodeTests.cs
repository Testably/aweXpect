#if NET8_0_OR_GREATER
using System.Net;
using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class HttpStatusCodeTests
	{
		[Test]
		[Arguments(HttpStatusCode.OK, "200 OK")]
		[Arguments(HttpStatusCode.BadRequest, "400 BadRequest")]
		[Arguments(null, "<null>")]
		public async Task Nullable_ShouldIncludeNumberAndDescription(HttpStatusCode? value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		[Arguments(HttpStatusCode.OK, "HttpStatusCode 200 OK")]
		[Arguments(HttpStatusCode.BadRequest, "HttpStatusCode 400 BadRequest")]
		[Arguments(null, "<null>")]
		public async Task Nullable_WithType_ShouldIncludeNumberAndDescription(HttpStatusCode? value,
			string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		[Arguments(HttpStatusCode.OK, "200 OK")]
		[Arguments(HttpStatusCode.BadRequest, "400 BadRequest")]
		public async Task ShouldIncludeNumberAndDescription(HttpStatusCode value, string expectedResult)
		{
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
			HttpStatusCode? value = null;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(ValueFormatter.NullString);
			await That(objectResult).IsEqualTo(ValueFormatter.NullString);
			await That(sb.ToString()).IsEqualTo(ValueFormatter.NullString);
		}

		[Test]
		public async Task WhenUndefined_ShouldOnlyIncludeNumber()
		{
			HttpStatusCode value = (HttpStatusCode)499;
			StringBuilder sb = new();
			StringBuilder sbWithType = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			string resultWithType = Formatter.Format(value, FormattingOptions.WithType);
			string objectResultWithType = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value);
			Formatter.Format(sbWithType, value, FormattingOptions.WithType);

			await That(result).IsEqualTo("499");
			await That(objectResult).IsEqualTo("499");
			await That(resultWithType).IsEqualTo("HttpStatusCode 499");
			await That(objectResultWithType).IsEqualTo("HttpStatusCode 499");
			await That(sb.ToString()).IsEqualTo("499");
			await That(sbWithType.ToString()).IsEqualTo("HttpStatusCode 499");
		}

		[Test]
		[Arguments(HttpStatusCode.OK, "HttpStatusCode 200 OK")]
		[Arguments(HttpStatusCode.BadRequest, "HttpStatusCode 400 BadRequest")]
		public async Task WithType_ShouldIncludeNumberAndDescription(HttpStatusCode value, string expectedResult)
		{
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
#endif
