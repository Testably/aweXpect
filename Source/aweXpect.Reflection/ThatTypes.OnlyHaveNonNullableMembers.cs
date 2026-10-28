using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all fields, properties and events of all items in the filtered collection of <see cref="Type" />
	///     are non-nullable.
	/// </summary>
	/// <remarks>
	///     A member is considered nullable if its type is a <see cref="Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).<br />
	///     The inherited members are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. Restrict them with
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" /> or add the members of these roots with
	///     <see cref="MemberScopeResult{TType, TValue}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static MemberScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> OnlyHaveNonNullableMembers(
		this IThat<IEnumerable<Type?>?> subject)
	{
		OnlyHaveMembersExpectation expectation = new(false);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <summary>
	///     …only have non-nullable fields, properties and events.
	/// </summary>
	/// <inheritdoc cref="OnlyHaveNonNullableMembers(IThat{IEnumerable{Type}})" path="/remarks" />
	public static MemberScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> OnlyHaveNonNullableMembers(
		this IEnumerableItems<Type?> items)
	{
		OnlyHaveMembersExpectation expectation = new(false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <inheritdoc cref="OnlyHaveNonNullableMembers(IEnumerableItems{Type})" />
	public static MemberScopeResult<TCollection, TCollection> OnlyHaveNonNullableMembers<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items)
		where TCollection : struct, IEnumerable<Type?>
	{
		OnlyHaveMembersExpectation expectation = new(false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="OnlyHaveNonNullableMembers(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static MemberScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> OnlyHaveNonNullableMembers(
		this IThat<IAsyncEnumerable<Type?>?> subject)
	{
		OnlyHaveMembersExpectation expectation = new(false);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <inheritdoc cref="OnlyHaveNonNullableMembers(IEnumerableItems{Type})" />
	public static MemberScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> OnlyHaveNonNullableMembers(
		this IAsyncEnumerableItems<Type?> items)
	{
		OnlyHaveMembersExpectation expectation = new(false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}
#endif
}
