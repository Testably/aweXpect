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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain constructors matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching constructor. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TSelf}(TSelf, Times)" />) to require a specific count.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainConstructors(
		this IThat<IEnumerable<Type?>?> subject,
		Func<Filtered.Constructors, Filtered.Constructors> filter)
	{
		ContainMembersExpectation expectation = ContainConstructorsExpectation(filter);
		return new(subject.ExpectAll(expectation), subject, expectation.Quantifier);
	}

	/// <summary>
	///     …contain constructors matching the <paramref name="filter" />.
	/// </summary>
	/// <inheritdoc cref="ContainConstructors(IThat{IEnumerable{Type}}, Func{Filtered.Constructors, Filtered.Constructors})" path="/remarks" />
	public static TypeContainingMembersResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainConstructors(
		this IEnumerableItems<Type?> items,
		Func<Filtered.Constructors, Filtered.Constructors> filter)
	{
		ContainMembersExpectation expectation = ContainConstructorsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier);
	}

	/// <inheritdoc cref="ContainConstructors(IEnumerableItems{Type}, Func{Filtered.Constructors, Filtered.Constructors})" />
	public static TypeContainingMembersResult<TCollection, TCollection> ContainConstructors<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Func<Filtered.Constructors, Filtered.Constructors> filter)
		where TCollection : struct, IEnumerable<Type?>
	{
		ContainMembersExpectation expectation = ContainConstructorsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="ContainConstructors(IThat{IEnumerable{Type}}, Func{Filtered.Constructors, Filtered.Constructors})" />
	[GuaranteesNotNull]
	public static TypeContainingMembersResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainConstructors(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Func<Filtered.Constructors, Filtered.Constructors> filter)
	{
		ContainMembersExpectation expectation = ContainConstructorsExpectation(filter);
		return new(subject.ExpectAll(expectation), subject, expectation.Quantifier);
	}

	/// <inheritdoc cref="ContainConstructors(IEnumerableItems{Type}, Func{Filtered.Constructors, Filtered.Constructors})" />
	public static TypeContainingMembersResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainConstructors(
		this IAsyncEnumerableItems<Type?> items,
		Func<Filtered.Constructors, Filtered.Constructors> filter)
	{
		ContainMembersExpectation expectation = ContainConstructorsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier);
	}
#endif

	private static ContainMembersExpectation ContainConstructorsExpectation(
		Func<Filtered.Constructors, Filtered.Constructors> filter)
		=> ContainMembersExpectation.WithoutScope<ConstructorInfo, Filtered.Constructors>(
			types => types.Constructors(), filter);
}
