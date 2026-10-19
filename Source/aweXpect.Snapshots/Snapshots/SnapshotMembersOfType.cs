using System;

namespace aweXpect.Snapshots;

/// <summary>
///     The members of a type that a rule applies to, see <see cref="SnapshotMembers{TReturn, TValue}.OfType{T}" />, which
///     also allows alternative types.
/// </summary>
/// <remarks>
///     A value matches when it matches any of the types. As the selected values are then of different types,
///     <see cref="SnapshotMembers{TReturn, TValue}.Convert{TResult}" /> gets an <see cref="object" /> after an
///     alternative type.
/// </remarks>
/// <typeparam name="TReturn">What an action returns.</typeparam>
/// <typeparam name="TValue">The type of the selected values.</typeparam>
public sealed class SnapshotMembersOfType<TReturn, TValue> : SnapshotMembers<TReturn, TValue>
{
	private readonly Func<SnapshotRule, TReturn> _add;
	private readonly ValueSelection _selectionWithoutType;
	private readonly TypeFilter[] _types;

	internal SnapshotMembersOfType(Func<SnapshotRule, TReturn> add, ValueSelection selectionWithoutType,
		TypeFilter[] types)
		: base(add, selectionWithoutType.OfType(types))
	{
		_add = add;
		_selectionWithoutType = selectionWithoutType;
		_types = types;
	}

	/// <summary>
	///     Also selects the members of the type <typeparamref name="T" />, like
	///     <see cref="SnapshotMembers{TReturn, TValue}.OfType{T}" />.
	/// </summary>
	public SnapshotMembersOfType<TReturn, object> OrOfType<T>()
		=> new(_add, _selectionWithoutType, [.. _types, new TypeFilter(typeof(T), false),]);

	/// <summary>
	///     Also selects the members that are exactly of the type <typeparamref name="T" />, like
	///     <see cref="SnapshotMembers{TReturn, TValue}.OfExactType{T}" />.
	/// </summary>
	public SnapshotMembersOfType<TReturn, object> OrOfExactType<T>()
		=> new(_add, _selectionWithoutType, [.. _types, new TypeFilter(typeof(T), true),]);
}
