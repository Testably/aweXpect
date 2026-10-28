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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain fields matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching field. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TSelf}(TSelf, Times)" />) to require a specific count.<br />
	///     The inherited fields are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. Restrict them with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.Directly" /> or add the members of these roots with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainFields(
		this IThat<IEnumerable<Type?>?> subject,
		Func<Filtered.Fields, Filtered.Fields> filter)
	{
		ContainMembersExpectation expectation = ContainFieldsExpectation(filter);
		return new(subject.ExpectAll(expectation), subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <summary>
	///     …contain fields matching the <paramref name="filter" />.
	/// </summary>
	/// <inheritdoc cref="ContainFields(IThat{IEnumerable{Type}}, Func{Filtered.Fields, Filtered.Fields})" path="/remarks" />
	public static TypeContainingMembersScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainFields(
		this IEnumerableItems<Type?> items,
		Func<Filtered.Fields, Filtered.Fields> filter)
	{
		ContainMembersExpectation expectation = ContainFieldsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <inheritdoc cref="ContainFields(IEnumerableItems{Type}, Func{Filtered.Fields, Filtered.Fields})" />
	public static TypeContainingMembersScopeResult<TCollection, TCollection> ContainFields<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Func<Filtered.Fields, Filtered.Fields> filter)
		where TCollection : struct, IEnumerable<Type?>
	{
		ContainMembersExpectation expectation = ContainFieldsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="ContainFields(IThat{IEnumerable{Type}}, Func{Filtered.Fields, Filtered.Fields})" />
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainFields(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Func<Filtered.Fields, Filtered.Fields> filter)
	{
		ContainMembersExpectation expectation = ContainFieldsExpectation(filter);
		return new(subject.ExpectAll(expectation), subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <inheritdoc cref="ContainFields(IEnumerableItems{Type}, Func{Filtered.Fields, Filtered.Fields})" />
	public static TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainFields(
		this IAsyncEnumerableItems<Type?> items,
		Func<Filtered.Fields, Filtered.Fields> filter)
	{
		ContainMembersExpectation expectation = ContainFieldsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}
#endif

	private static ContainMembersExpectation ContainFieldsExpectation(Func<Filtered.Fields, Filtered.Fields> filter)
		=> ContainMembersExpectation.WithScope<FieldInfo, Filtered.Fields>(
			(types, memberScope) => types.Fields(memberScope), filter);
}
