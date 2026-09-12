﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

using CoreDelegate = global::aweXpect.Delegates.ThatDelegate;

public sealed class GuaranteesNotNullTests
{
	private static readonly Type[] TypeArgumentCandidates =
	[
		typeof(object), typeof(string), typeof(Exception), typeof(ArgumentException), typeof(EquatableSubject),
		typeof(NotifyingSubject), typeof(double), typeof(int), typeof(DayOfWeek), typeof(TimeSpan),
		typeof(DateTime), typeof(EnumerableStruct<object>), typeof(EnumerableStruct<string?>),
	];

	// Expectations whose call shape this test cannot construct faithfully: the result needs a
	// continuation the test cannot choose, or an argument it cannot invent. Each one is covered by
	// a hand-written null-subject test instead, which ExcludedExpectations_ShouldBeCovered verifies.
	private static readonly HashSet<string> ExcludedFromInvocation =
	[
		"ThatAsyncEnumerable.All(IThat<IAsyncEnumerable<String>>)",
		"ThatAsyncEnumerable.Any(IThat<IAsyncEnumerable<String>>)",
		"ThatAsyncEnumerable.AtLeast(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.AtMost(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.Between(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.Contains<TItem>(IThat<IAsyncEnumerable<TItem>>,Func<TItem,Boolean>,String)",
		"ThatAsyncEnumerable.DoesNotContain<TItem>(IThat<IAsyncEnumerable<TItem>>,Func<TItem,Boolean>,String)",
		"ThatAsyncEnumerable.Exactly(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.HasItem<TItem>(IThat<IAsyncEnumerable<TItem>>)",
		"ThatAsyncEnumerable.HasItemThat<TItem>(IThat<IAsyncEnumerable<TItem>>,Action<IThatSubject<TItem>>)",
		"ThatAsyncEnumerable.LessThan(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.MoreThan(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.None(IThat<IAsyncEnumerable<String>>)",
		"ThatEnumerable.All(IThat<IEnumerable<String>>)",
		"ThatEnumerable.Any(IThat<IEnumerable<String>>)",
		"ThatEnumerable.AreAllUnique(IThat<Nullable<ImmutableArray<String>>>)",
		"ThatEnumerable.AtLeast(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.AtMost(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.Between(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.Contains(IThat<IEnumerable>,Func<Object,Boolean>,String)",
		"ThatEnumerable.Contains<TItem>(IThat<IEnumerable<TItem>>,Func<TItem,Boolean>,String)",
		"ThatEnumerable.DoesNotContain(IThat<IEnumerable>,Func<Object,Boolean>,String)",
		"ThatEnumerable.DoesNotContain<TItem>(IThat<IEnumerable<TItem>>,Func<TItem,Boolean>,String)",
		"ThatEnumerable.Exactly(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.HasItem(IThat<IEnumerable>)",
		"ThatEnumerable.HasItem<TItem>(IThat<IEnumerable<TItem>>)",
		"ThatEnumerable.HasItemThat<TItem>(IThat<IEnumerable<TItem>>,Action<IThatSubject<TItem>>)",
		"ThatEnumerable.LessThan(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.MoreThan(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.None(IThat<IEnumerable<String>>)",
		"ThatEventRecording.DidNotTriggerPropertyChangedFor<TSubject,TProperty>(IThat<IEventRecording<TSubject>>,Expression<Func<TSubject,TProperty>>)",
		"ThatEventRecording.TriggeredPropertyChangedFor<TSubject,TProperty>(IThat<IEventRecording<TSubject>>,Expression<Func<TSubject,TProperty>>)",
		"ThatException.HasInner(IThat<Exception>,Type,Action<IThatSubject<Exception>>)",
		"ThatException.HasInner<TInnerException>(IThat<Exception>,Action<IThatSubject<TInnerException>>)",
		"ThatException.HasInnerException(IThat<Exception>,Action<IThatSubject<Exception>>)",
		"ThatException.HasRecursiveInnerExceptions(IThat<Exception>,Action<IThatSubject<IEnumerable<Exception>>>)",
		"ThatGeneric.DoesNotSatisfy<T>(IThat<T>,Func<T,Boolean>,String)",
		"ThatGeneric.Satisfies<T>(IThat<T>,Func<T,Boolean>,String)",
		"ThatString.HasLines(IThat<String>,Action<IThatSubject<IEnumerable<String>>>)",
	];

	// Expectations that do not fail for a null subject today, one entry per expectation name and
	// subject type. The rule is that a null subject fails, so every entry here is a deviation that
	// the next major version either removes or documents.
	private static readonly Dictionary<string, NullSubjectOutcome> ExpectationsThatDoNotFail =
		new(StringComparer.Ordinal)
		{
			["IThatSubject<T>.IsNot(IThatSubject<T>)"] = NullSubjectOutcome.Passes,
			["IThatSubject<T>.IsNotExactly(IThatSubject<T>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.DoesNotContain(IAsyncEnumerable<String>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.DoesNotContain(IAsyncEnumerable<TItem>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotContainedIn(IAsyncEnumerable<String>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotContainedIn(IAsyncEnumerable<TItem>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotEqualTo(IAsyncEnumerable<DateTime>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotEqualTo(IAsyncEnumerable<Decimal>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotEqualTo(IAsyncEnumerable<Double>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotEqualTo(IAsyncEnumerable<Nullable<DateTime>>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotEqualTo(IAsyncEnumerable<Nullable<Decimal>>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotEqualTo(IAsyncEnumerable<Nullable<Double>>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotEqualTo(IAsyncEnumerable<Nullable<Single>>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotEqualTo(IAsyncEnumerable<Single>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotEqualTo(IAsyncEnumerable<String>)"] = NullSubjectOutcome.Passes,
			["ThatAsyncEnumerable.IsNotEqualTo(IAsyncEnumerable<TItem>)"] = NullSubjectOutcome.Passes,
			["ThatBufferedStream.HasBufferSize(BufferedStream)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatEnumerable.AreAllUnique(Nullable<ImmutableArray<String>>)"] = NullSubjectOutcome.Throws,
			["ThatEnumerable.DoesNotContain(IEnumerable)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatEnumerable.DoesNotContain(IEnumerable<String>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatEnumerable.DoesNotContain(IEnumerable<TItem>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatEnumerable.DoesNotContain(String[])"] = NullSubjectOutcome.Throws,
			["ThatEnumerable.IsNotContainedIn(IEnumerable)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotContainedIn(IEnumerable<String>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotContainedIn(IEnumerable<TItem>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable<DateTime>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable<Decimal>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable<Double>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable<Nullable<DateTime>>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable<Nullable<Decimal>>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable<Nullable<Double>>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable<Nullable<Single>>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable<Single>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable<String>)"] = NullSubjectOutcome.Passes,
			["ThatEnumerable.IsNotEqualTo(IEnumerable<TItem>)"] = NullSubjectOutcome.Passes,
			["ThatEventRecording.DidNotTriggerPropertyChangedFor(IEventRecording<TSubject>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.NotInvocable,
			["ThatEventRecording.TriggeredPropertyChangedFor(IEventRecording<TSubject>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.NotInvocable,
			["ThatException.HasInner(Exception)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Throws,
			["ThatException.HasInnerException(Exception)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Throws,
			["ThatException.HasRecursiveInnerExceptions(Exception)"] = NullSubjectOutcome.Throws,
			["ThatGeneric.CompliesWith(T)"] = NullSubjectOutcome.Throws,
			["ThatGeneric.DoesNotComplyWith(T)"] = NullSubjectOutcome.Throws,
			["ThatGeneric.For(T)"] = NullSubjectOutcome.Throws,
			["ThatGeneric.IsNotEquatableTo(TEquatable)"] = NullSubjectOutcome.Passes,
			["ThatNullableBool.IsEqualTo(Nullable<Boolean>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableBool.IsNotEqualTo(Nullable<Boolean>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableBool.IsNotFalse(Nullable<Boolean>)"] = NullSubjectOutcome.Passes,
			["ThatNullableBool.IsNotTrue(Nullable<Boolean>)"] = NullSubjectOutcome.Passes,
			["ThatNullableBool.IsNull(Nullable<Boolean>)"] = NullSubjectOutcome.Passes,
			["ThatNullableChar.IsEqualTo(Nullable<Char>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableChar.IsNotEqualTo(Nullable<Char>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableChar.IsNotOneOf(Nullable<Char>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableChar.IsOneOf(Nullable<Char>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateOnly.HasDay(Nullable<DateOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateOnly.HasMonth(Nullable<DateOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateOnly.HasYear(Nullable<DateOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateOnly.IsEqualTo(Nullable<DateOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateOnly.IsNotEqualTo(Nullable<DateOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateOnly.IsNotOneOf(Nullable<DateOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateOnly.IsOneOf(Nullable<DateOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.HasDay(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.HasHour(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.HasMillisecond(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.HasMinute(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.HasMonth(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.HasSecond(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.HasYear(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.IsEqualTo(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.IsNotEqualTo(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.IsNotOneOf(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTime.IsOneOf(Nullable<DateTime>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.HasDay(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.HasHour(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.HasMillisecond(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.HasMinute(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.HasMonth(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.HasOffset(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.HasSecond(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.HasYear(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.IsEqualTo(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.IsNotEqualTo(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.IsNotOneOf(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableDateTimeOffset.IsOneOf(Nullable<DateTimeOffset>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableEnum.DoesNotHaveFlag(Nullable<TEnum>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableEnum.DoesNotHaveValue(Nullable<TEnum>)"] = NullSubjectOutcome.Passes,
			["ThatNullableEnum.HasFlag(Nullable<TEnum>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableEnum.IsEqualTo(Nullable<TEnum>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableEnum.IsNotEqualTo(Nullable<TEnum>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableEnum.IsNotOneOf(Nullable<TEnum>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableEnum.IsOneOf(Nullable<TEnum>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableGuid.IsEqualTo(Nullable<Guid>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableGuid.IsNotEqualTo(Nullable<Guid>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableGuid.IsNullOrEmpty(Nullable<Guid>)"] = NullSubjectOutcome.Passes,
			["ThatNullableTimeOnly.HasHour(Nullable<TimeOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeOnly.HasMillisecond(Nullable<TimeOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeOnly.HasMinute(Nullable<TimeOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeOnly.HasSecond(Nullable<TimeOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeOnly.IsEqualTo(Nullable<TimeOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeOnly.IsNotEqualTo(Nullable<TimeOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeOnly.IsNotOneOf(Nullable<TimeOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeOnly.IsOneOf(Nullable<TimeOnly>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeSpan.IsEqualTo(Nullable<TimeSpan>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeSpan.IsNotEqualTo(Nullable<TimeSpan>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeSpan.IsNotOneOf(Nullable<TimeSpan>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNullableTimeSpan.IsOneOf(Nullable<TimeSpan>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<TNumber>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<TNumber>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<TNumber>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotFinite(Nullable<TNumber>)"] = NullSubjectOutcome.Passes,
			["ThatNumber.IsNotInfinite(Nullable<TNumber>)"] = NullSubjectOutcome.Passes,
			["ThatNumber.IsNotNaN(Nullable<TNumber>)"] = NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<TNumber>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<TNumber>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatObject.IsEqualTo(Nullable<T>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatObject.IsEqualTo(Object)"] = NullSubjectOutcome.Passes,
			["ThatObject.IsEquivalentTo(TSubject)"] = NullSubjectOutcome.Passes,
			["ThatObject.IsNot(T)"] = NullSubjectOutcome.Passes,
			["ThatObject.IsNotEqualTo(Nullable<T>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatObject.IsNotExactly(Object)"] = NullSubjectOutcome.Passes,
			["ThatObject.IsNull(Nullable<T>)"] = NullSubjectOutcome.Passes,
			["ThatObject.IsNull(T)"] = NullSubjectOutcome.Passes,
			["ThatObject.IsOneOf(Object)"] = NullSubjectOutcome.Passes,
			["ThatSpan.IsNotParsableInto(SpanWrapper<Byte>)"] = NullSubjectOutcome.Passes,
			["ThatSpan.IsNotParsableInto(SpanWrapper<Char>)"] = NullSubjectOutcome.Passes,
			["ThatStream.HasLength(Stream)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatStream.HasPosition(Stream)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatString.DoesNotEndWith(String)"] = NullSubjectOutcome.Passes,
			["ThatString.DoesNotStartWith(String)"] = NullSubjectOutcome.Passes,
			["ThatString.HasLength(String)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatString.HasLineCount(String)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatString.HasLines(String)"] = NullSubjectOutcome.Throws,
			["ThatString.IsNotEmpty(String)"] = NullSubjectOutcome.Passes,
			["ThatString.IsNotEqualTo(String)"] = NullSubjectOutcome.Passes,
			["ThatString.IsNotOneOf(String)"] = NullSubjectOutcome.Passes,
			["ThatString.IsNotParsableInto(String)"] = NullSubjectOutcome.Passes,
			["ThatString.IsNull(String)"] = NullSubjectOutcome.Passes,
			["ThatString.IsNullOrEmpty(String)"] = NullSubjectOutcome.Passes,
			["ThatString.IsNullOrWhiteSpace(String)"] = NullSubjectOutcome.Passes,
			["WithValue<T>.Eventually(WithValue<T>)"] = NullSubjectOutcome.NotInvocable,
			// Without generic math, the numeric expectations exist once per numeric type instead of
			// once for `TNumber`, so they are separate entries on the older target frameworks.
#if !NET8_0_OR_GREATER
			["ThatNumber.IsEqualTo(Nullable<Byte>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<Decimal>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<Double>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<Int16>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<Int32>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<Int64>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<SByte>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<Single>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<UInt16>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<UInt32>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsEqualTo(Nullable<UInt64>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<Byte>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<Decimal>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<Double>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<Int16>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<Int32>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<Int64>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<SByte>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<Single>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<UInt16>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<UInt32>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotBetween(Nullable<UInt64>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<Byte>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<Decimal>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<Double>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<Int16>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<Int32>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<Int64>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<SByte>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<Single>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<UInt16>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<UInt32>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotEqualTo(Nullable<UInt64>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotFinite(Nullable<Double>)"] = NullSubjectOutcome.Passes,
			["ThatNumber.IsNotFinite(Nullable<Single>)"] = NullSubjectOutcome.Passes,
			["ThatNumber.IsNotInfinite(Nullable<Double>)"] = NullSubjectOutcome.Passes,
			["ThatNumber.IsNotInfinite(Nullable<Single>)"] = NullSubjectOutcome.Passes,
			["ThatNumber.IsNotNaN(Nullable<Double>)"] = NullSubjectOutcome.Passes,
			["ThatNumber.IsNotNaN(Nullable<Single>)"] = NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<Byte>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<Decimal>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<Double>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<Int16>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<Int32>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<Int64>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<SByte>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<Single>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<UInt16>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<UInt32>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsNotOneOf(Nullable<UInt64>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<Byte>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<Decimal>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<Double>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<Int16>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<Int32>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<Int64>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<SByte>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<Single>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<UInt16>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<UInt32>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
			["ThatNumber.IsOneOf(Nullable<UInt64>)"] = NullSubjectOutcome.Fails | NullSubjectOutcome.Passes,
#endif
		};

	private static Dictionary<string, NullSubjectOutcome>? _nullSubjectBehaviour;

	private static Dictionary<string, NullSubjectOutcome> NullSubjectBehaviour
		=> _nullSubjectBehaviour ??= DetermineNullSubjectBehaviour();

	public static TheoryData<string> MarkedExpectations
	{
		get
		{
			TheoryData<string> data = new();
			foreach (MethodInfo method in GetMarkedExpectations().Where(CanHaveNullSubject))
			{
				string identifier = GetIdentifier(method);
				if (!ExcludedFromInvocation.Contains(identifier))
				{
					data.Add(identifier);
				}
			}

			return data;
		}
	}

	[Fact]
	public async Task EveryExpectation_ShouldFailForANullSubject()
	{
		List<string> deviations = NullSubjectBehaviour
			.Where(entry => entry.Value != GetRecordedOutcome(entry.Key))
			.Select(entry => $"{entry.Key} = {entry.Value}")
			.OrderBy(entry => entry, StringComparer.Ordinal)
			.ToList();

		await That(deviations).IsEmpty()
			.Because(
				"a null subject must fail every expectation, unless ExpectationsThatDoNotFail records that it does not");
	}

	[Fact]
	public async Task ShouldFindTheMarkedExpectations()
	{
		List<MethodInfo> marked = GetMarkedExpectations().ToList();

		await That(marked.Select(GetIdentifier)).AreAllUnique()
			.Because("each marked expectation must map to exactly one test case");
		await That(marked.Where(CanHaveNullSubject)).IsNotEmpty()
			.Because("the reflection lookup must not silently degrade into an empty test set");
		await That(marked.Select(method => method.DeclaringType!.Assembly.GetName().Name).Distinct())
			.IsEqualTo(["aweXpect", "aweXpect.Core",]).InAnyOrder()
			.Because("both assemblies declare expectations that are marked");
	}

#if NET8_0_OR_GREATER
	// Both assertions are statements about this repository rather than about runtime behaviour, so it is
	// enough to make them where every expectation and every test class is present: neither the
	// `IAsyncEnumerable` expectations nor their tests exist on the older target frameworks.
	[Fact]
	public async Task ExcludedExpectations_ShouldBeCovered()
	{
		List<string> identifiers = GetMarkedExpectations().Where(CanHaveNullSubject).Select(GetIdentifier).ToList();

		await That(ExcludedFromInvocation.Where(excluded => !identifiers.Contains(excluded)).ToList()).IsEmpty()
			.Because("an exclusion that no longer matches a marked expectation is stale");
		await That(ExcludedFromInvocation.Where(excluded => GetCoveringTests(excluded).Length == 0).ToList()).IsEmpty()
			.Because("every excluded expectation needs a hand-written null-subject test instead");
	}

	[Fact]
	public async Task RecordedExpectations_ShouldStillExist()
	{
		await That(ExpectationsThatDoNotFail.Keys.Where(key => !NullSubjectBehaviour.ContainsKey(key)).ToList())
			.IsEmpty()
			.Because("a recorded expectation that no longer exists is stale");
	}
#endif

	[Fact]
	public async Task SkippedExpectations_ShouldHaveASubjectThatCannotBeNull()
	{
		List<Type> skipped = GetMarkedExpectations().Where(method => !CanHaveNullSubject(method))
			.Select(method => GetSubjectType(CloseMethod(method)))
			.Distinct().ToList();

		await That(skipped.Where(type => type.Assembly == typeof(GuaranteesNotNullTests).Assembly).ToList()).IsEmpty()
			.Because(
				"a skipped expectation is invisible — it is neither invoked nor excluded — so a subject that only looks non-nullable because it was closed to one of this test's own helper structs must not go unnoticed");
	}

	[Theory]
	[MemberData(nameof(MarkedExpectations))]
	public async Task WhenSubjectIsNull_ShouldFail(string identifier)
	{
		MethodInfo method = GetMarkedExpectations().Single(m => GetIdentifier(m) == identifier);

		void Act() => Evaluate(method);

		await That(Act).Throws<XunitException>()
			.Because($"{identifier} is marked with [GuaranteesNotNull]");
	}

	private static Dictionary<string, NullSubjectOutcome> DetermineNullSubjectBehaviour()
	{
		Dictionary<string, NullSubjectOutcome> behaviour = new(StringComparer.Ordinal);
		foreach (MethodInfo method in GetAllExpectations())
		{
			if (DetermineOutcome(method) is not { } determined)
			{
				continue;
			}

			string key = GetKey(method);
			behaviour.TryGetValue(key, out NullSubjectOutcome outcome);
			behaviour[key] = outcome | determined;
		}

		return behaviour;
	}

	private static NullSubjectOutcome GetRecordedOutcome(string key)
		=> ExpectationsThatDoNotFail.TryGetValue(key, out NullSubjectOutcome outcome)
			? outcome
			: NullSubjectOutcome.Fails;

	private static NullSubjectOutcome? DetermineOutcome(MethodInfo method)
	{
		MethodInfo closedMethod;
		Type subjectType;
		try
		{
			closedMethod = CloseMethod(method);
			if (!CanHaveNullSubject(closedMethod))
			{
				return null;
			}

			subjectType = GetSubjectType(closedMethod);
			Invoke(closedMethod, CreateNullSubject(subjectType));
		}
		catch (NotInvocableException)
		{
			return NullSubjectOutcome.NotInvocable;
		}
		catch (Exception exception) when (exception is not XunitException)
		{
			return NullSubjectOutcome.Throws;
		}

		List<MethodInfo[]> paths =
			DiscoverCompletions(() => Invoke(closedMethod, CreateNullSubject(subjectType)), [], 0);

		NullSubjectOutcome outcome = default;
		foreach (MethodInfo[] path in paths)
		{
			foreach (bool nullValues in new[] { false, true, })
			{
				try
				{
					Await(Follow(Invoke(closedMethod, CreateNullSubject(subjectType), nullValues), path, nullValues));
					outcome |= NullSubjectOutcome.Passes;
				}
				catch (XunitException)
				{
					outcome |= NullSubjectOutcome.Fails;
				}
				catch (Exception)
				{
					outcome |= NullSubjectOutcome.Throws;
				}
			}
		}

		return outcome == default ? NullSubjectOutcome.NotInvocable : outcome;
	}

	private static List<MethodInfo[]> DiscoverCompletions(Func<object> create, MethodInfo[] prefix, int depth)
	{
		object expectation;
		try
		{
			expectation = Follow(create(), prefix, false);
		}
		catch (Exception)
		{
			return [];
		}

		if (expectation.GetType().GetMethod("GetAwaiter") is not null)
		{
			return [prefix,];
		}

		if (depth >= 3)
		{
			return [];
		}

		List<MethodInfo[]> paths = [];
		foreach (MethodInfo continuation in GetContinuations(expectation))
		{
			paths.AddRange(DiscoverCompletions(create, [..prefix, continuation,], depth + 1));
		}

		return paths;
	}

	private static object Follow(object expectation, MethodInfo[] path, bool nullValues)
	{
		object current = expectation;
		foreach (MethodInfo continuation in path)
		{
			current = continuation.Invoke(current,
				continuation.GetParameters().Select(parameter => CreateContinuationArgument(parameter, nullValues)).ToArray())!;
		}

		return current;
	}

	private static IEnumerable<MethodInfo> GetAllExpectations()
		=> new[]
			{
				typeof(GuaranteesNotNullAttribute).Assembly, typeof(global::aweXpect.ThatString).Assembly,
			}
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsPublic || type.IsNestedPublic)
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static |
			                                    BindingFlags.Instance | BindingFlags.DeclaredOnly))
			.Where(IsExpectation);

	private static bool IsExpectation(MethodInfo method)
	{
		if (method.IsSpecialName || method.ReturnType == typeof(void) ||
		    method.GetCustomAttribute<EditorBrowsableAttribute>()?.State == EditorBrowsableState.Never)
		{
			return false;
		}

		Type? receiver = method.IsStatic
			? method.GetParameters().FirstOrDefault()?.ParameterType
			: method.DeclaringType;
		return receiver is not null && IsSupportedReceiver(receiver);
	}

	private static bool IsSupportedReceiver(Type receiver)
	{
		Type definition = receiver.IsGenericType ? receiver.GetGenericTypeDefinition() : receiver;
		return definition == typeof(IThat<>) || definition == typeof(IThatSubject<>) ||
		       definition == typeof(ThatSubject<>) || definition == typeof(CoreDelegate) ||
		       definition == typeof(CoreDelegate.WithoutValue) || definition == typeof(CoreDelegate.WithValue<>);
	}

	private static string GetKey(MethodInfo method)
		=> $"{FormatType(method.DeclaringType!)}.{method.Name}({FormatType(GetSubjectType(method))})";

	private static IEnumerable<MethodInfo> GetMarkedExpectations()
		=> new[]
			{
				typeof(GuaranteesNotNullAttribute).Assembly, typeof(global::aweXpect.ThatString).Assembly,
			}
			.SelectMany(assembly => assembly.GetTypes())
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static |
			                                    BindingFlags.Instance | BindingFlags.DeclaredOnly))
			.Where(method => method.GetCustomAttribute<GuaranteesNotNullAttribute>() is not null);

	private static MethodInfo[] GetCoveringTests(string identifier)
	{
		string declaringType = identifier.Substring(0, identifier.IndexOf('.'));
		string expectation = identifier.Substring(identifier.IndexOf('.') + 1).Split('<', '(')[0];
		return typeof(GuaranteesNotNullTests).Assembly.GetTypes()
			.Where(type => type.FullName?.Contains($"{declaringType}+{expectation}+") == true)
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance |
			                                    BindingFlags.DeclaredOnly))
			.Where(method => method.Name is nameof(WhenSubjectIsNull_ShouldFail) or "WhenActualIsNull_ShouldFail")
			.ToArray();
	}

	private static bool CanHaveNullSubject(MethodInfo method)
	{
		Type subjectType = GetSubjectType(CloseMethod(method));
		return !subjectType.IsValueType || Nullable.GetUnderlyingType(subjectType) is not null;
	}

	private static string GetIdentifier(MethodInfo method)
	{
		string declaringType = FormatType(method.DeclaringType!);
		string typeArguments = method.IsGenericMethodDefinition
			? "<" + string.Join(",", method.GetGenericArguments().Select(argument => argument.Name)) + ">"
			: "";
		string parameters = string.Join(",", method.GetParameters().Select(p => FormatType(p.ParameterType)));
		return $"{declaringType}.{method.Name}{typeArguments}({parameters})";
	}

	private static string FormatType(Type type)
	{
		if (type.IsArray)
		{
			return FormatType(type.GetElementType()!) + "[]";
		}

		string name = type.Name.Split('`')[0];
		return type.IsGenericType
			? name + "<" + string.Join(",", type.GetGenericArguments().Select(FormatType)) + ">"
			: name;
	}

	private static void Evaluate(MethodInfo method)
	{
		MethodInfo closedMethod = CloseMethod(method);
		object subject = CreateNullSubject(GetSubjectType(closedMethod));
		Await(Complete(Invoke(closedMethod, subject)));
	}

	private static object Invoke(MethodInfo method, object subject, bool nullValues = false)
	{
		object?[] arguments = method.GetParameters()
			.Select(parameter => parameter.Position == 0 && method.IsStatic
				? subject
				: CreateArgument(parameter, nullValues))
			.ToArray();
		try
		{
			return (method.IsStatic ? method.Invoke(null, arguments) : method.Invoke(subject, arguments))!;
		}
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			throw exception.InnerException;
		}
	}

	private static Type GetSubjectType(MethodInfo method)
	{
		Type receiver = method.IsStatic ? method.GetParameters()[0].ParameterType : method.DeclaringType!;
		return receiver.IsGenericType && receiver.GetGenericTypeDefinition() == typeof(IThat<>)
			? receiver.GetGenericArguments()[0]
			: receiver;
	}

	private static object CreateNullSubject(Type subjectType)
	{
		Type definition = subjectType.IsGenericType ? subjectType.GetGenericTypeDefinition() : subjectType;
#pragma warning disable aweXpect0001 // the expectation is awaited in Await after it was invoked reflectively
		if (definition == typeof(IThatSubject<>) || definition == typeof(ThatSubject<>))
		{
			return Expect.That((object?)null);
		}

		if (definition == typeof(CoreDelegate) || definition == typeof(CoreDelegate.WithoutValue))
		{
			return Expect.That((Action)null!);
		}
#pragma warning restore aweXpect0001

		if (definition == typeof(CoreDelegate.WithValue<>))
		{
			return InvokeThat(subjectType.GetGenericArguments()[0],
				parameter => parameter.ParameterType.IsGenericType &&
				             parameter.ParameterType.GetGenericTypeDefinition() == typeof(Func<>) &&
				             IsGenericMethodParameter(parameter.ParameterType.GetGenericArguments()[0]));
		}

		return InvokeThat(subjectType, parameter => IsGenericMethodParameter(parameter.ParameterType));
	}

	private static bool IsGenericMethodParameter(Type type)
		=> type.IsGenericParameter && type.DeclaringMethod is not null;

	private static object InvokeThat(Type typeArgument, Func<ParameterInfo, bool> subjectParameter)
	{
		MethodInfo that = typeof(Expect).GetMethods(BindingFlags.Public | BindingFlags.Static)
			.Single(candidate => candidate is { Name: nameof(Expect.That), IsGenericMethodDefinition: true, } &&
			                     candidate.GetGenericArguments().Length == 1 &&
			                     candidate.GetParameters().Length == 2 &&
			                     subjectParameter(candidate.GetParameters()[0]));
		return that.MakeGenericMethod(typeArgument).Invoke(null, [null, "subject",])!;
	}

	private static MethodInfo CloseMethod(MethodInfo method)
	{
		MethodInfo closedMethod = method;
		if (method.DeclaringType!.IsGenericTypeDefinition)
		{
			Type closedType = method.DeclaringType.MakeGenericType(
				CreateTypeArguments(method.DeclaringType.GetGenericArguments()));
			closedMethod = closedType
				.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				.Single(candidate => candidate.MetadataToken == method.MetadataToken);
		}

		return closedMethod.IsGenericMethodDefinition
			? closedMethod.MakeGenericMethod(CreateTypeArguments(closedMethod.GetGenericArguments()))
			: closedMethod;
	}

	private static Type[] CreateTypeArguments(Type[] genericArguments)
	{
		Dictionary<Type, Type> resolved = new();
		bool progressed = true;
		while (resolved.Count < genericArguments.Length && progressed)
		{
			progressed = false;
			foreach (Type genericArgument in genericArguments.Where(argument => !resolved.ContainsKey(argument)))
			{
				if (genericArgument.GetGenericParameterConstraints()
				    .Any(constraint => ReferencesUnresolved(constraint, genericArgument, resolved)))
				{
					continue;
				}

				if (TypeArgumentCandidates.FirstOrDefault(candidate =>
					    Satisfies(candidate, genericArgument, resolved)) is { } match)
				{
					resolved[genericArgument] = match;
					progressed = true;
				}
			}
		}

		Type? unresolved = genericArguments.FirstOrDefault(argument => !resolved.ContainsKey(argument));
		if (unresolved is not null)
		{
			throw new NotInvocableException(
				$"The test does not know which type argument satisfies '{unresolved}' of '{unresolved.DeclaringMethod?.ToString() ?? unresolved.DeclaringType?.ToString()}'. Extend {nameof(TypeArgumentCandidates)}.");
		}

		return genericArguments.Select(argument => resolved[argument]).ToArray();
	}

	private static bool ReferencesUnresolved(Type constraint, Type genericArgument, Dictionary<Type, Type> resolved)
	{
		if (constraint.IsGenericParameter)
		{
			return constraint != genericArgument && !resolved.ContainsKey(constraint);
		}

		return constraint.IsGenericType && constraint.GetGenericArguments()
			.Any(argument => ReferencesUnresolved(argument, genericArgument, resolved));
	}

	private static bool Satisfies(Type candidate, Type genericArgument, Dictionary<Type, Type> resolved)
	{
		GenericParameterAttributes attributes = genericArgument.GenericParameterAttributes;
		if (attributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint) && !candidate.IsValueType)
		{
			return false;
		}

		if (attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint) && candidate.IsValueType)
		{
			return false;
		}

		try
		{
			return genericArgument.GetGenericParameterConstraints()
				.Select(constraint => Substitute(constraint, candidate, genericArgument, resolved))
				.All(constraint => constraint.IsAssignableFrom(candidate));
		}
		catch (Exception exception) when (exception is ArgumentException or TypeLoadException)
		{
			return false;
		}
	}

	private static Type Substitute(Type constraint, Type candidate, Type genericArgument,
		Dictionary<Type, Type> resolved)
	{
		if (!constraint.ContainsGenericParameters || !constraint.IsGenericType)
		{
			return constraint == genericArgument ? candidate : constraint;
		}

		return constraint.GetGenericTypeDefinition().MakeGenericType(constraint.GetGenericArguments()
			.Select(argument => argument == genericArgument ? candidate :
				resolved.TryGetValue(argument, out Type? value) ? value : argument)
			.ToArray());
	}

	private static object? CreateArgument(ParameterInfo parameter, bool nullValues)
	{
		Type type = parameter.ParameterType;
		if (type == typeof(Type))
		{
			return typeof(Exception);
		}

		if (parameter.IsOptional)
		{
			return parameter.DefaultValue;
		}

		if (Nullable.GetUnderlyingType(type) is { } underlyingType)
		{
			return nullValues ? null : Activator.CreateInstance(underlyingType);
		}

		if (type.IsValueType)
		{
			return Activator.CreateInstance(type);
		}


		if (type == typeof(string))
		{
			return "a";
		}

		if (type.IsArray)
		{
			return CreateSingleElementArray(type.GetElementType()!);
		}

		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
		{
			return CreateSingleElementArray(type.GetGenericArguments()[0]);
		}

		if (typeof(Expression).IsAssignableFrom(type))
		{
			throw new NotInvocableException(
				$"The test cannot invent a '{type}' for parameter '{parameter.Name}'. Extend {nameof(CreateArgument)}.");
		}

		return typeof(Delegate).IsAssignableFrom(type) ? CreateDelegate(type) : null;
	}

	private static Delegate CreateDelegate(Type delegateType)
	{
		MethodInfo invoke = delegateType.GetMethod("Invoke")!;
		ParameterExpression[] parameters = invoke.GetParameters()
			.Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name))
			.ToArray();
		Expression body = invoke.ReturnType == typeof(void)
			? Expression.Empty()
			: Expression.Default(invoke.ReturnType);
		return Expression.Lambda(delegateType, body, parameters).Compile();
	}

	private static Array CreateSingleElementArray(Type elementType)
	{
		Array array = Array.CreateInstance(elementType, 1);
		array.SetValue(
			elementType == typeof(string) ? "a" :
			elementType.IsValueType ? Activator.CreateInstance(elementType) : null,
			0);
		return array;
	}

	private static object Complete(object expectation, int depth = 0)
	{
		if (expectation.GetType().GetMethod("GetAwaiter") is not null)
		{
			return expectation;
		}

		if (depth < 3)
		{
			foreach (MethodInfo continuation in GetContinuations(expectation))
			{
				object? result;
				try
				{
					result = continuation.Invoke(expectation,
						continuation.GetParameters().Select(parameter => CreateContinuationArgument(parameter, false)).ToArray());
				}
				catch (Exception)
				{
					continue;
				}

				if (result is not null)
				{
					return Complete(result, depth + 1);
				}
			}
		}

		throw new NotInvocableException(
			$"The test does not know how to await a '{expectation.GetType()}'. Extend {nameof(Complete)}.");
	}

	private static IEnumerable<MethodInfo> GetContinuations(object expectation)
		=> expectation.GetType()
			.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
			.Where(method => !method.IsSpecialName && !method.IsGenericMethodDefinition &&
			                 method.ReturnType != typeof(void))
			.OrderBy(method => method.Name, StringComparer.Ordinal);

	private static object? CreateContinuationArgument(ParameterInfo parameter, bool nullValues)
		=> parameter.ParameterType == typeof(TimeSpan)
			? TimeSpan.FromSeconds(1)
			: CreateArgument(parameter, nullValues);

	private static void Await(object expectation)
	{
		object awaiter = expectation.GetType().GetMethod("GetAwaiter")!.Invoke(expectation, null)!;
		try
		{
			awaiter.GetType().GetMethod("GetResult")!.Invoke(awaiter, null);
		}
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			throw exception.InnerException;
		}
	}

	[Flags]
	private enum NullSubjectOutcome
	{
		Fails = 1,
		Passes = 2,
		Throws = 4,
		NotInvocable = 8,
	}

	private sealed class NotInvocableException(string message) : Exception(message);

	private sealed class NotifyingSubject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged
		{
			add => throw new NotSupportedException();
			remove => throw new NotSupportedException();
		}
	}

	private readonly struct EnumerableStruct<T> : IEnumerable<T>
	{
		public IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private sealed class EquatableSubject : IEquatable<object>
	{
		public override bool Equals(object? other) => false;

		public override int GetHashCode() => 0;
	}
}
