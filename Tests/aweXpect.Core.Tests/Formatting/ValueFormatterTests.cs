using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public class ValueFormatterTests
{
	[Fact]
	public async Task CustomFormatter_ShouldBeUsedDuringLifetime()
	{
		MyFormattableClass value = new()
		{
			Value = 42,
		};
		using (ValueFormatter.Register(new MyCustomFormatter("my-string")))
		{
			string customObjectResult = Formatter.Format(value);

			await That(customObjectResult).IsEqualTo("my-string");
		}

		string objectResult = Formatter.Format(value);

		await That(objectResult).IsEqualTo("""
		                                   ValueFormatterTests.MyFormattableClass {
		                                     Value = 42
		                                   }
		                                   """);
	}

	[Fact]
	public async Task CustomFormatter_WhenDisposedTwice_ShouldOnlyRemoveItsOwnRegistration()
	{
		MyFormattableClass value = new();
		MyCustomFormatter formatter = new("my-string");
		using IDisposable first = ValueFormatter.Register(formatter);
		IDisposable second = ValueFormatter.Register(formatter);

		second.Dispose();
		second.Dispose();

		await That(Formatter.Format(value)).IsEqualTo("my-string")
			.Because("each registration is removed on its own, even when the same formatter was registered twice");
	}

	[Fact]
	public async Task CustomFormatter_WhenItThrows_ShouldRenderAPlaceholderInTheFailureMessage()
	{
		MyThrowingFormattableClass subject = new();
		using IDisposable lifetime = ValueFormatter.Register(
			new MyThrowingCustomFormatter(new InvalidOperationException("formatter failed")));

		async Task Act()
			=> await That(subject).IsNull();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is null,
			             but it was [the formatter did throw an InvalidOperationException: formatter failed]
			             """)
			.Because("what the formatter appended before it threw is discarded");
	}

	[Fact]
	public async Task CustomFormatter_WhenMultipleAreRegistered_ShouldUseTheMostRecentOne()
	{
		MyFormattableClass value = new();
		using IDisposable first = ValueFormatter.Register(new MyCustomFormatter("first"));
		using IDisposable second = ValueFormatter.Register(new MyCustomFormatter("second"));
		using IDisposable third = ValueFormatter.Register(new MyCustomFormatter("third"));

		await That(Formatter.Format(value)).IsEqualTo("third")
			.Because("the most recently registered formatter takes precedence");
	}

	[Fact]
	public async Task CustomFormatter_WhenNull_ShouldUseDefaultNullString()
	{
		using IDisposable lifetime = ValueFormatter.Register(new MyCustomFormatter("my-string"));
		bool? value = null;

		string objectResult = Formatter.Format((object?)value);

		await That(objectResult).IsEqualTo(ValueFormatter.NullString);
	}

	[Fact]
	public async Task CustomFormatter_WhenTheMostRecentOneIsDisposed_ShouldFallBackToTheEarlierOne()
	{
		MyFormattableClass value = new();
		using IDisposable first = ValueFormatter.Register(new MyCustomFormatter("first"));
		using IDisposable second = ValueFormatter.Register(new MyCustomFormatter("second"));
		IDisposable third = ValueFormatter.Register(new MyCustomFormatter("third"));

		third.Dispose();

		await That(Formatter.Format(value)).IsEqualTo("second")
			.Because("disposing the most recent formatter restores the precedence of the earlier one");
	}

	[Fact]
	public async Task CustomFormatter_WhenTypeDoesNotMatch_ShouldDoNothing()
	{
		int value = 1;
		using (ValueFormatter.Register(new MyCustomFormatter("my-string")))
		{
			string customObjectResult = Formatter.Format((object?)value);

			await That(customObjectResult).IsEqualTo("1");
		}
	}

	private sealed class MyCustomFormatter(string formatString) : IValueFormatter
	{
		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
		{
			if (value is MyFormattableClass)
			{
				stringBuilder.Append(formatString);
				return true;
			}

			return false;
		}
	}

	private sealed class MyFormattableClass
	{
		public int Value { get; set; }
	}

	private sealed class MyThrowingCustomFormatter(Exception exception) : IValueFormatter
	{
		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
		{
			if (value is MyThrowingFormattableClass)
			{
				stringBuilder.Append("partial");
				throw exception;
			}

			return false;
		}
	}

	private sealed class MyThrowingFormattableClass;
}
