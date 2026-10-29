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
	///     Verifies that all events in the filtered collection have a handler of type <typeparamref name="THandler" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<EventInfo?>> AreOfType<THandler>(
		this IThat<IEnumerable<EventInfo?>?> subject)
		=> AreOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of type <paramref name="handlerType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<EventInfo?>> AreOfType(
		this IThat<IEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		OfTypeExpectation<EventInfo> expectation = new(static @event => @event.EventHandlerType, handlerType, false);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …have a handler of type <typeparamref name="THandler" /> (or a subtype).
	/// </summary>
	public static OfTypeResult<IEnumerable<EventInfo?>> AreOfType<THandler>(
		this IEnumerableItems<EventInfo?> items)
		=> AreOfType(items, typeof(THandler));

	/// <summary>
	///     …have a handler of type <paramref name="handlerType" /> (or a subtype).
	/// </summary>
	public static OfTypeResult<IEnumerable<EventInfo?>> AreOfType(
		this IEnumerableItems<EventInfo?> items, Type handlerType)
	{
		OfTypeExpectation<EventInfo> expectation = new(static @event => @event.EventHandlerType, handlerType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfType(IEnumerableItems{EventInfo}, Type)" />
	public static OfTypeResult<TCollection> AreOfType<TCollection>(
		this IStructEnumerableItems<TCollection, EventInfo?> items, Type handlerType)
		where TCollection : struct, IEnumerable<EventInfo?>
	{
		OfTypeExpectation<EventInfo> expectation = new(static @event => @event.EventHandlerType, handlerType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreOfType{THandler}(IThat{IEnumerable{EventInfo}})" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<EventInfo?>> AreOfType<THandler>(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		=> AreOfType(subject, typeof(THandler));

	/// <inheritdoc cref="AreOfType(IThat{IEnumerable{EventInfo}}, Type)" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<EventInfo?>> AreOfType(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		OfTypeExpectation<EventInfo> expectation = new(static @event => @event.EventHandlerType, handlerType, false);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfType{THandler}(IEnumerableItems{EventInfo})" />
	public static OfTypeResult<IAsyncEnumerable<EventInfo?>> AreOfType<THandler>(
		this IAsyncEnumerableItems<EventInfo?> items)
		=> AreOfType(items, typeof(THandler));

	/// <inheritdoc cref="AreOfType(IEnumerableItems{EventInfo}, Type)" />
	public static OfTypeResult<IAsyncEnumerable<EventInfo?>> AreOfType(
		this IAsyncEnumerableItems<EventInfo?> items, Type handlerType)
	{
		OfTypeExpectation<EventInfo> expectation = new(static @event => @event.EventHandlerType, handlerType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of type <typeparamref name="THandler" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>> AreNotOfType<THandler>(
		this IThat<IEnumerable<EventInfo?>?> subject)
		=> AreNotOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of type <paramref name="handlerType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>> AreNotOfType(
		this IThat<IEnumerable<EventInfo?>?> subject, Type handlerType)
		=> new(subject.ExpectNone(new OfTypeExpectation<EventInfo>(static @event => @event.EventHandlerType, handlerType, false)), subject);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreNotOfType{THandler}(IThat{IEnumerable{EventInfo}})" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>> AreNotOfType<THandler>(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		=> AreNotOfType(subject, typeof(THandler));

	/// <inheritdoc cref="AreNotOfType(IThat{IEnumerable{EventInfo}}, Type)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>> AreNotOfType(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject, Type handlerType)
		=> new(subject.ExpectNone(new OfTypeExpectation<EventInfo>(static @event => @event.EventHandlerType, handlerType, false)), subject);
#endif
}
