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
		: PropertyResult.String<MyClass?, IThat<MyClass?>, MyClass?>(subject, mapper, propertyExpression, validation,
			grammars, includeValueInContext);

	private class MyBaseClass
	{
		public int IntValue { get; init; }
		public string? StringValue { get; init; }
	}

	private sealed class MyDerivedClass : MyBaseClass;

	private sealed class Dummy : IExpectThat<string>
	{
		public ExpectationBuilder ExpectationBuilder { get; } = new ManualExpectationBuilder<string>();
	}

	private sealed class MyClass
	{
		public DateTimeKind DateTimeKindValue { get; private init; }
		public decimal DecimalValue { get; private init; }
		public double DoubleValue { get; private init; }
		public int IntValue { get; private init; }
		public long LongValue { get; private init; }
		public string? StringValue { get; private init; }
		public TimeSpan TimeSpanValue { get; private init; }

		public static PropertyResult.Enum<MyClass?, IThat<MyClass?>, MyClass?, DateTimeKind> HasDateTimeKindValue(
			DateTimeKind dateTimeKindValue, ExpectationGrammars grammars)
		{
			MyClass subject = new()
			{
				DateTimeKindValue = dateTimeKindValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Enum<MyClass?, IThat<MyClass?>, MyClass?, DateTimeKind>(
				source, a => a?.DateTimeKindValue, "kind value", grammars);
		}

		public static PropertyResult.Enum<MyClass?, IThat<MyClass?>, MyClass?, DateTimeKind> DateTimeKindValueOf(
			IThat<MyClass?> source, ExpectationGrammars grammars)
			=> new(source, a => a?.DateTimeKindValue, "kind value", grammars);

		/// <summary>
		///     The mapper returns <see langword="null" /> for a subject that is not <see langword="null" />, which is the
		///     shape of a nullable property.
		/// </summary>
		public static PropertyResult.Enum<MyClass?, DateTimeKind> HasNullDateTimeKindValue()
		{
			MyClass subject = new();
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Enum<MyClass?, DateTimeKind>(source, _ => null, "kind value");
		}

		/// <inheritdoc cref="HasNullDateTimeKindValue" />
		public static PropertyResult.Decimal<MyClass?> HasNullDecimalValue()
		{
			MyClass subject = new();
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Decimal<MyClass?>(source, _ => null, "decimal value");
		}

		/// <inheritdoc cref="HasNullDateTimeKindValue" />
		public static PropertyResult.Double<MyClass?> HasNullDoubleValue()
		{
			MyClass subject = new();
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Double<MyClass?>(source, _ => null, "double value");
		}

		/// <inheritdoc cref="HasNullDateTimeKindValue" />
		public static PropertyResult.Int<MyClass?> HasNullIntValue()
		{
			MyClass subject = new();
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Int<MyClass?>(source, _ => null, "int value");
		}

		/// <inheritdoc cref="HasNullDateTimeKindValue" />
		public static PropertyResult.Long<MyClass?> HasNullLongValue()
		{
			MyClass subject = new();
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Long<MyClass?>(source, _ => null, "long value");
		}

		/// <inheritdoc cref="HasNullDateTimeKindValue" />
		public static PropertyResult.TimeSpan<MyClass?> HasNullTimeSpanValue()
		{
			MyClass subject = new();
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.TimeSpan<MyClass?>(source, _ => null, "TimeSpan value");
		}

		public static PropertyResult.Decimal<MyClass?> HasDecimalValue(decimal decimalValue)
		{
			MyClass subject = new()
			{
				DecimalValue = decimalValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Decimal<MyClass?>(source, a => a?.DecimalValue, "decimal value");
		}

		public static PropertyResult.Decimal<MyClass?> HasDecimalValueOfNullSubject()
		{
			MyClass? subject = null;
#pragma warning disable aweXpect0001
			IThat<MyClass?> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Decimal<MyClass?>(source, a => a?.DecimalValue, "decimal value");
		}

		public static PropertyResult.Decimal<MyClass?, IThat<MyClass?>, MyClass?> DecimalValueOf(
			IThat<MyClass?> source, ExpectationGrammars grammars)
			=> new(source, a => a?.DecimalValue, "decimal value", grammars: grammars);

		public static PropertyResult.Double<MyClass?> HasDoubleValue(double doubleValue)
		{
			MyClass subject = new()
			{
				DoubleValue = doubleValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Double<MyClass?>(source, a => a?.DoubleValue, "double value");
		}

		public static PropertyResult.Double<MyClass?> HasDoubleValueOfNullSubject()
		{
			MyClass? subject = null;
#pragma warning disable aweXpect0001
			IThat<MyClass?> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Double<MyClass?>(source, a => a?.DoubleValue, "double value");
		}

		public static PropertyResult.Double<MyClass?, IThat<MyClass?>, MyClass?> DoubleValueOf(
			IThat<MyClass?> source, ExpectationGrammars grammars)
			=> new(source, a => a?.DoubleValue, "double value", grammars: grammars);

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

		public static PropertyResult.Int<MyClass?, IThat<MyClass?>, MyClass?> HasIntValue(int intValue,
			ExpectationGrammars grammars)
		{
			MyClass subject = new()
			{
				IntValue = intValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Int<MyClass?, IThat<MyClass?>, MyClass?>(
				source, a => a?.IntValue, "int value", grammars: grammars);
		}

		public static PropertyResult.Int<MyClass?, IThat<MyClass?>, MyClass?> IntValueOf(IThat<MyClass?> source,
			ExpectationGrammars grammars)
			=> new(source, a => a?.IntValue, "int value", grammars: grammars);

		/// <summary>
		///     The mapper is typed at <see cref="MyBaseClass" /> while the result keeps <see cref="MyDerivedClass" />,
		///     which is the shape a delegate produces when it narrows the exception type only at the result.
		/// </summary>
		public static PropertyResult.Int<MyDerivedClass?, IThat<MyDerivedClass?>, MyBaseClass?>
			HasIntValueOfNarrowedSubject(int intValue)
		{
			MyDerivedClass subject = new()
			{
				IntValue = intValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyDerivedClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Int<MyDerivedClass?, IThat<MyDerivedClass?>, MyBaseClass?>(
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

		public static PropertyResult.Long<MyClass?, IThat<MyClass?>, MyClass?> HasLongValue(long longValue,
			ExpectationGrammars grammars)
		{
			MyClass subject = new()
			{
				LongValue = longValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Long<MyClass?, IThat<MyClass?>, MyClass?>(
				source, a => a?.LongValue, "long value", grammars: grammars);
		}

		public static PropertyResult.Long<MyClass?, IThat<MyClass?>, MyClass?> LongValueOf(IThat<MyClass?> source,
			ExpectationGrammars grammars)
			=> new(source, a => a?.LongValue, "long value", grammars: grammars);

		public static PropertyResult.Long<MyClass?> HasLongValueOfNullSubject()
		{
			MyClass? subject = null;
#pragma warning disable aweXpect0001
			IThat<MyClass?> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Long<MyClass?>(source, a => a?.LongValue, "long value");
		}

		/// <summary>
		///     The mapper throws the <paramref name="exception" />, which is the shape of a property that cannot be read,
		///     e.g. the length of a disposed stream.
		/// </summary>
		public static PropertyResult.Long<MyClass?> HasThrowingLongValue(Exception exception)
		{
			MyClass subject = new();
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.Long<MyClass?>(source, _ => throw exception, "long value");
		}

		/// <summary>
		///     The mapper throws the <paramref name="exception" />, which is the shape of a property that cannot be read,
		///     e.g. a <see cref="Exception.Message" /> that throws itself.
		/// </summary>
		public static StringProperty HasThrowingStringValue(Exception exception)
		{
			MyClass subject = new();
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new StringProperty(source, _ => throw exception, "string value");
		}

		public static StringProperty HasStringValue(string? stringValue,
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
		public static PropertyResult.String<MyDerivedClass?, IThat<MyDerivedClass?>, MyBaseClass?>
			HasStringValueOfNarrowedSubject(string stringValue)
		{
			MyDerivedClass subject = new()
			{
				StringValue = stringValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyDerivedClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.String<MyDerivedClass?, IThat<MyDerivedClass?>, MyBaseClass?>(
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

		public static PropertyResult.TimeSpan<MyClass?, IThat<MyClass?>, MyClass?> HasTimeSpanValue(
			TimeSpan timeSpanValue, ExpectationGrammars grammars)
		{
			MyClass subject = new()
			{
				TimeSpanValue = timeSpanValue,
			};
#pragma warning disable aweXpect0001
			IThat<MyClass> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.TimeSpan<MyClass?, IThat<MyClass?>, MyClass?>(
				source, a => a?.TimeSpanValue, "TimeSpan value", grammars: grammars);
		}

		public static PropertyResult.TimeSpan<MyClass?> HasTimeSpanValueOfNullSubject()
		{
			MyClass? subject = null;
#pragma warning disable aweXpect0001
			IThat<MyClass?> source = That(subject);
#pragma warning restore aweXpect0001
			return new PropertyResult.TimeSpan<MyClass?>(source, a => a?.TimeSpanValue, "TimeSpan value");
		}

		public static PropertyResult.TimeSpan<MyClass?, IThat<MyClass?>, MyClass?> TimeSpanValueOf(
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
