using System.Collections.Generic;
using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class ExceptionTests
	{
		[Fact]
		public async Task AggregateException_ShouldNotBeFormattedAsCollection()
		{
			Exception value = new AggregateException("outer", new CustomException("inner"));
#if NETFRAMEWORK
			string expectedResult = "AggregateException: outer";
#else
			string expectedResult = "AggregateException: outer (inner)";
#endif
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task GenericException_ShouldUseFormattedTypeName()
		{
			Exception value = new GenericException<int>("foo");
			string expectedResult = "ValueFormatters.ExceptionTests.GenericException<int>: foo";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task InCollection_ShouldFormatEachExceptionWithTypeAndMessage()
		{
			List<Exception> value = [new CustomException("foo"), new ArgumentException("bar"),];
			string expectedResult = """
			                        [
			                          ValueFormatters.ExceptionTests.CustomException: foo,
			                          ArgumentException: bar
			                        ]
			                        """;

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult).IgnoringNewlineStyle();
		}

		[Fact]
		public async Task ShouldDefaultToSingleLine()
		{
			Exception value = new CustomException("a\nb");
			string expectedResult = @"ValueFormatters.ExceptionTests.CustomException: a\nb";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task ShouldIncludeTypeNameAndMessage()
		{
			Exception value = new CustomException("foo");
			string expectedResult = "ValueFormatters.ExceptionTests.CustomException: foo";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task ShouldNotIncludeInnerExceptionOrStackTrace()
		{
			Exception value;
			try
			{
				throw new InvalidOperationException("outer", new CustomException("inner"));
			}
			catch (Exception ex)
			{
				value = ex;
			}

			string expectedResult = "InvalidOperationException: outer";

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenMessageIsEmpty_ShouldOnlyIncludeTypeName()
		{
			Exception value = new CustomException("");
			string expectedResult = "ValueFormatters.ExceptionTests.CustomException";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenMessageIsTooLong_ShouldTruncateMessage()
		{
			Exception value = new CustomException(new string('a', 101));
			string expectedResult = $"ValueFormatters.ExceptionTests.CustomException: {new string('a', 100)}…";

			string result = Formatter.Format(value);

			await That(result).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenNull_ShouldUseDefaultNullString()
		{
			Exception? value = null;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo("<null>");
			await That(sb.ToString()).IsEqualTo("<null>");
		}

		[Fact]
		public async Task WhenUsingMultipleLines_ShouldIndentFollowingLinesOfTheMessage()
		{
			Exception value = new CustomException("a\nb");
			string expectedResult = """
			                        ValueFormatters.ExceptionTests.CustomException: a
			                          b
			                        """;
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.MultipleLines);
			Formatter.Format(sb, value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult).IgnoringNewlineStyle();
			await That(objectResult).IsEqualTo(expectedResult).IgnoringNewlineStyle();
			await That(sb.ToString()).IsEqualTo(expectedResult).IgnoringNewlineStyle();
		}

		private sealed class CustomException(string message) : Exception(message);

		private sealed class GenericException<T>(string message) : Exception(message);
	}
}
