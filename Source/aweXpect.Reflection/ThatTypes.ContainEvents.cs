using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain events matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching event. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TSelf}(TSelf, Times)" />) to require a specific count.<br />
	///     The inherited events are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. Restrict them with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.Directly" /> or add the members of these roots with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainEvents(
		this IThat<IEnumerable<Type?>?> subject,
		Func<Filtered.Events, Filtered.Events> filter)
	{
		ContainMembersExpectation expectation = ContainEventsExpectation(filter);
		return new(subject.ExpectAll(expectation), subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <summary>
	///     …contain events matching the <paramref name="filter" />.
	/// </summary>
	/// <inheritdoc cref="ContainEvents(IThat{IEnumerable{Type}}, Func{Filtered.Events, Filtered.Events})" path="/remarks" />
	public static TypeContainingMembersScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainEvents(
		this IEnumerableItems<Type?> items,
		Func<Filtered.Events, Filtered.Events> filter)
	{
		ContainMembersExpectation expectation = ContainEventsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <inheritdoc cref="ContainEvents(IEnumerableItems{Type}, Func{Filtered.Events, Filtered.Events})" />
	public static TypeContainingMembersScopeResult<TCollection, TCollection> ContainEvents<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Func<Filtered.Events, Filtered.Events> filter)
		where TCollection : struct, IEnumerable<Type?>
	{
		ContainMembersExpectation expectation = ContainEventsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="ContainEvents(IThat{IEnumerable{Type}}, Func{Filtered.Events, Filtered.Events})" />
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainEvents(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Func<Filtered.Events, Filtered.Events> filter)
	{
		ContainMembersExpectation expectation = ContainEventsExpectation(filter);
		return new(subject.ExpectAll(expectation), subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <inheritdoc cref="ContainEvents(IEnumerableItems{Type}, Func{Filtered.Events, Filtered.Events})" />
	public static TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainEvents(
		this IAsyncEnumerableItems<Type?> items,
		Func<Filtered.Events, Filtered.Events> filter)
	{
		ContainMembersExpectation expectation = ContainEventsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}
#endif

	private static ContainMembersExpectation ContainEventsExpectation(Func<Filtered.Events, Filtered.Events> filter)
		=> ContainMembersExpectation.WithScope<EventInfo, Filtered.Events>(
			(types, memberScope) => types.Events(memberScope), filter);
}
