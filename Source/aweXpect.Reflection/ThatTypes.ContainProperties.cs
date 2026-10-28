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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain properties matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching property. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TSelf}(TSelf, Times)" />) to require a specific count.<br />
	///     The inherited properties are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. Restrict them with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.Directly" /> or add the members of these roots with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainProperties(
		this IThat<IEnumerable<Type?>?> subject,
		Func<Filtered.Properties, Filtered.Properties> filter)
	{
		ContainMembersExpectation expectation = ContainPropertiesExpectation(filter);
		return new(subject.ExpectAll(expectation), subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <summary>
	///     …contain properties matching the <paramref name="filter" />.
	/// </summary>
	/// <inheritdoc cref="ContainProperties(IThat{IEnumerable{Type}}, Func{Filtered.Properties, Filtered.Properties})" path="/remarks" />
	public static TypeContainingMembersScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainProperties(
		this IEnumerableItems<Type?> items,
		Func<Filtered.Properties, Filtered.Properties> filter)
	{
		ContainMembersExpectation expectation = ContainPropertiesExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <inheritdoc cref="ContainProperties(IEnumerableItems{Type}, Func{Filtered.Properties, Filtered.Properties})" />
	public static TypeContainingMembersScopeResult<TCollection, TCollection> ContainProperties<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Func<Filtered.Properties, Filtered.Properties> filter)
		where TCollection : struct, IEnumerable<Type?>
	{
		ContainMembersExpectation expectation = ContainPropertiesExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="ContainProperties(IThat{IEnumerable{Type}}, Func{Filtered.Properties, Filtered.Properties})" />
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainProperties(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Func<Filtered.Properties, Filtered.Properties> filter)
	{
		ContainMembersExpectation expectation = ContainPropertiesExpectation(filter);
		return new(subject.ExpectAll(expectation), subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <inheritdoc cref="ContainProperties(IEnumerableItems{Type}, Func{Filtered.Properties, Filtered.Properties})" />
	public static TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainProperties(
		this IAsyncEnumerableItems<Type?> items,
		Func<Filtered.Properties, Filtered.Properties> filter)
	{
		ContainMembersExpectation expectation = ContainPropertiesExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}
#endif

	private static ContainMembersExpectation ContainPropertiesExpectation(Func<Filtered.Properties, Filtered.Properties> filter)
		=> ContainMembersExpectation.WithScope<PropertyInfo, Filtered.Properties>(
			(types, memberScope) => types.Properties(memberScope), filter);
}
