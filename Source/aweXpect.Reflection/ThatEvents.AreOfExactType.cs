using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of exactly type <typeparamref name="THandler" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<EventInfo?>> AreOfExactType<THandler>(
		this IThat<IEnumerable<EventInfo?>?> subject)
		=> AreOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of exactly type <paramref name="handlerType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<EventInfo?>> AreOfExactType(
		this IThat<IEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		OfTypeExpectation<EventInfo> expectation = new(static @event => @event.EventHandlerType, handlerType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …have a handler of exactly type <typeparamref name="THandler" />.
	/// </summary>
	public static OfTypeResult<IEnumerable<EventInfo?>> AreOfExactType<THandler>(
		this IEnumerableItems<EventInfo?> items)
		=> AreOfExactType(items, typeof(THandler));

	/// <summary>
	///     …have a handler of exactly type <paramref name="handlerType" />.
	/// </summary>
	public static OfTypeResult<IEnumerable<EventInfo?>> AreOfExactType(
		this IEnumerableItems<EventInfo?> items, Type handlerType)
	{
		OfTypeExpectation<EventInfo> expectation = new(static @event => @event.EventHandlerType, handlerType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfExactType(IEnumerableItems{EventInfo}, Type)" />
	public static OfTypeResult<TCollection> AreOfExactType<TCollection>(
		this IStructEnumerableItems<TCollection, EventInfo?> items, Type handlerType)
		where TCollection : struct, IEnumerable<EventInfo?>
	{
		OfTypeExpectation<EventInfo> expectation = new(static @event => @event.EventHandlerType, handlerType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreOfExactType{THandler}(IThat{IEnumerable{EventInfo}})" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<EventInfo?>> AreOfExactType<THandler>(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		=> AreOfExactType(subject, typeof(THandler));

	/// <inheritdoc cref="AreOfExactType(IThat{IEnumerable{EventInfo}}, Type)" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<EventInfo?>> AreOfExactType(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		OfTypeExpectation<EventInfo> expectation = new(static @event => @event.EventHandlerType, handlerType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfExactType{THandler}(IEnumerableItems{EventInfo})" />
	public static OfTypeResult<IAsyncEnumerable<EventInfo?>> AreOfExactType<THandler>(
		this IAsyncEnumerableItems<EventInfo?> items)
		=> AreOfExactType(items, typeof(THandler));

	/// <inheritdoc cref="AreOfExactType(IEnumerableItems{EventInfo}, Type)" />
	public static OfTypeResult<IAsyncEnumerable<EventInfo?>> AreOfExactType(
		this IAsyncEnumerableItems<EventInfo?> items, Type handlerType)
	{
		OfTypeExpectation<EventInfo> expectation = new(static @event => @event.EventHandlerType, handlerType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of exactly type <typeparamref name="THandler" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>> AreNotOfExactType<THandler>(
		this IThat<IEnumerable<EventInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of exactly type <paramref name="handlerType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>> AreNotOfExactType(
		this IThat<IEnumerable<EventInfo?>?> subject, Type handlerType)
		=> new(subject.ExpectNone(new OfTypeExpectation<EventInfo>(static @event => @event.EventHandlerType, handlerType, true)), subject);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreNotOfExactType{THandler}(IThat{IEnumerable{EventInfo}})" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>> AreNotOfExactType<THandler>(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(THandler));

	/// <inheritdoc cref="AreNotOfExactType(IThat{IEnumerable{EventInfo}}, Type)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>> AreNotOfExactType(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject, Type handlerType)
		=> new(subject.ExpectNone(new OfTypeExpectation<EventInfo>(static @event => @event.EventHandlerType, handlerType, true)), subject);
#endif
}
