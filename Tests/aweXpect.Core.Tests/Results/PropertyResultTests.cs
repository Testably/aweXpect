using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	/// <summary>
	///     Spelling the three type arguments out once keeps the string tests readable.
	/// </summary>
	private sealed class StringProperty(
		IThat<MyClass?> subject,
		Func<MyClass?, string?> mapper,
		string propertyExpression,
		Action<string?, string>? validation = null,
		ExpectationGrammars grammars = ExpectationGrammars.None,
		bool includeValueInContext = false)
		: PropertyResult.String<MyClass?, MyClass?, IThat<MyClass?>>(subject, mapper, propertyExpression, validation,
			grammars, includeValueInContext);

	private class MyBaseClass
	{
		public int IntValue { get; init; }
		public string? StringValue { get; init; }
	}

	private sealed class MyDerivedClass : MyBaseClass;

	private sealed class Dummy : IExpectThat<string>
	{
		public ExpectationBuilder ExpectationBuilder { get; } = new ManualExpectationBuilder<string>(null);
	}

	private sealed class MyClass
	{
		public DateTimeKind DateTimeKindValue { get; private init; }
		public int IntValue { get; private init; }
		public long LongValue { get; private init; }
		public string? StringValue { get; private init; }
		public TimeSpan TimeSpanValue { get; private init; }

		public static PropertyResult.DateTimeKind<MyClass?, MyClass?, IThat<MyClass?>> HasDateTimeKindValue(
			DateTimeKind dateTimeKindValue, ExpectationGrammars grammars)
		{
			MyClass subject = new()
			{
				DateTimeKindValue = dateTimeKindValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.DateTimeKind<MyClass?, MyClass?, IThat<MyClass?>>(
				source, a => a?.DateTimeKindValue, "kind value", grammars);
		}

		public static PropertyResult.DateTimeKind<MyClass?, MyClass?, IThat<MyClass?>> DateTimeKindValueOf(
			IThat<MyClass?> source, ExpectationGrammars grammars)
			=> new(source, a => a?.DateTimeKindValue, "kind value", grammars);

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

		public static PropertyResult.Int<MyClass?, MyClass?, IThat<MyClass?>> HasIntValue(int intValue,
			ExpectationGrammars grammars)
		{
			MyClass subject = new()
			{
				IntValue = intValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Int<MyClass?, MyClass?, IThat<MyClass?>>(
				source, a => a?.IntValue, "int value", grammars: grammars);
		}

		public static PropertyResult.Int<MyClass?, MyClass?, IThat<MyClass?>> IntValueOf(IThat<MyClass?> source,
			ExpectationGrammars grammars)
			=> new(source, a => a?.IntValue, "int value", grammars: grammars);

		/// <summary>
		///     The mapper is typed at <see cref="MyBaseClass" /> while the result keeps <see cref="MyDerivedClass" />,
		///     which is the shape a delegate produces when it narrows the exception type only at the result.
		/// </summary>
		public static PropertyResult.Int<MyBaseClass?, MyDerivedClass?, IThat<MyDerivedClass?>>
			HasIntValueOfNarrowedSubject(int intValue)
		{
			MyDerivedClass subject = new()
			{
				IntValue = intValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyDerivedClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Int<MyBaseClass?, MyDerivedClass?, IThat<MyDerivedClass?>>(
				source, a => a?.IntValue, "int value");
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

		public static PropertyResult.Long<MyClass?, MyClass?, IThat<MyClass?>> HasLongValue(long longValue,
			ExpectationGrammars grammars)
		{
			MyClass subject = new()
			{
				LongValue = longValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Long<MyClass?, MyClass?, IThat<MyClass?>>(
				source, a => a?.LongValue, "long value", grammars: grammars);
		}

		public static PropertyResult.Long<MyClass?, MyClass?, IThat<MyClass?>> LongValueOf(IThat<MyClass?> source,
			ExpectationGrammars grammars)
			=> new(source, a => a?.LongValue, "long value", grammars: grammars);

		public static StringProperty HasStringValue(string stringValue,
			ExpectationGrammars grammars = ExpectationGrammars.None)
		{
			MyClass subject = new()
			{
				StringValue = stringValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new StringProperty(source, a => a?.StringValue, "string value", null, grammars);
		}

		/// <summary>
		///     The mapper is typed at <see cref="MyBaseClass" /> while the result keeps <see cref="MyDerivedClass" />,
		///     which is the shape a delegate produces when it narrows the exception type only at the result.
		/// </summary>
		public static PropertyResult.String<MyBaseClass?, MyDerivedClass?, IThat<MyDerivedClass?>>
			HasStringValueOfNarrowedSubject(string stringValue)
		{
			MyDerivedClass subject = new()
			{
				StringValue = stringValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyDerivedClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.String<MyBaseClass?, MyDerivedClass?, IThat<MyDerivedClass?>>(
				source, a => a?.StringValue, "string value");
		}

		public static StringProperty HasStringValueOfNullSubject()
		{
			MyClass? subject = null;
#pragma warning disable aweXpect0001
			IThat<MyClass?> source = That(subject);
#pragma warning restore aweXpect0001
			return new StringProperty(source, a => a?.StringValue, "string value");
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

		public static PropertyResult.TimeSpan<MyClass?, MyClass?, IThat<MyClass?>> HasTimeSpanValue(
			TimeSpan timeSpanValue, ExpectationGrammars grammars)
		{
			MyClass subject = new()
			{
				TimeSpanValue = timeSpanValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.TimeSpan<MyClass?, MyClass?, IThat<MyClass?>>(
				source, a => a?.TimeSpanValue, "TimeSpan value", grammars: grammars);
		}

		public static PropertyResult.TimeSpan<MyClass?, MyClass?, IThat<MyClass?>> TimeSpanValueOf(
			IThat<MyClass?> source, ExpectationGrammars grammars)
			=> new(source, a => a?.TimeSpanValue, "TimeSpan value", grammars: grammars);

		public static StringProperty StringValueOf(IThat<MyClass?> source, bool includeValueInContext = false,
			ExpectationGrammars grammars = ExpectationGrammars.None)
			=> new(source, a => a?.StringValue, "string value", null, grammars, includeValueInContext);

		/// <summary>
		///     The source of a <see cref="StringValueOf" />, so that two properties can share one expectation builder.
		/// </summary>
		public static IThat<MyClass?> WithStringValue(string stringValue)
		{
			MyClass subject = new()
			{
				StringValue = stringValue,
			};
#pragma warning disable aweXpect0001
			return That(subject);
#pragma warning restore aweXpect0001
		}
	}
}
