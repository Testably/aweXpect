using System.Collections.Generic;
using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class KeyValuePairTests
	{
		[Fact]
		public async Task ShouldFormatKeyAndValue()
		{
			string expectedResult = "[\"foo\"] = 42";
			KeyValuePair<string, int> value = new("foo", 42);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenBoxed_ShouldFormatKeyAndValue()
		{
			string expectedResult = "[\"foo\"] = 42";
			object value = new KeyValuePair<string, int>("foo", 42);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("a boxed pair has lost its type arguments, so the members are read through the registry or reflection");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenKeyAndValueAreNull_ShouldUseDefaultNullString()
		{
			string expectedResult = $"[{ValueFormatter.NullString}] = {ValueFormatter.NullString}";
			KeyValuePair<string?, object?> value = new(null, null);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WithType_ShouldNotIncludeTypeInformation()
		{
			string expectedResult = "[\"foo\"] = 42";
			KeyValuePair<string, int> value = new("foo", 42);
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult).Because("the brackets already convey the pair structure");
			await That(sb.ToString()).IsEqualTo(expectedResult)
				.Because("the brackets already convey the pair structure");
		}
	}
}
