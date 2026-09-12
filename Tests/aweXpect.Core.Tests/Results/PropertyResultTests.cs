using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	private sealed class Dummy : IExpectThat<string>
	{
		public ExpectationBuilder ExpectationBuilder { get; } = new ManualExpectationBuilder<string>(null);
	}

	private sealed class MyClass
	{
		public int IntValue { get; private init; }
		public long LongValue { get; private init; }
		public string? StringValue { get; private init; }
		public TimeSpan TimeSpanValue { get; private init; }

		public static PropertyResult.Int<MyClass?> HasIntValue(int intValue)
		{
			MyClass subject = new()
			{
				IntValue = intValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Int<MyClass?>(source, a => a?.IntValue, "int value");
		}

		public static PropertyResult.Int<MyClass?> HasIntValueOfNullSubject()
		{
			MyClass? subject = null;
#pragma warning disable aweXpect0001
			IThat<MyClass?> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Int<MyClass?>(source, a => a?.IntValue, "int value");
		}

		public static PropertyResult.Long<MyClass?> HasLongValue(long longValue)
		{
			MyClass subject = new()
			{
				LongValue = longValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Long<MyClass?>(source, a => a?.LongValue, "long value");
		}

		public static PropertyResult.String<MyClass?> HasStringValue(string stringValue)
		{
			MyClass subject = new()
			{
				StringValue = stringValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.String<MyClass?>(source, a => a?.StringValue, "string value");
		}

		public static PropertyResult.String<MyClass?> HasStringValueOfNullSubject()
		{
			MyClass? subject = null;
#pragma warning disable aweXpect0001
			IThat<MyClass?> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.String<MyClass?>(source, a => a?.StringValue, "string value");
		}

		public static PropertyResult.TimeSpan<MyClass?> HasTimeSpanValue(TimeSpan timeSpanValue)
		{
			MyClass subject = new()
			{
				TimeSpanValue = timeSpanValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.TimeSpan<MyClass?>(source, a => a?.TimeSpanValue, "TimeSpan value");
		}
	}
}
