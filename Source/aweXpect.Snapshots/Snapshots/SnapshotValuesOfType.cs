using System;

namespace aweXpect.Snapshots;

/// <summary>
///     The values of a type that a rule applies to, see <see cref="SnapshotValues{TReturn, TValue}.OfType{T}" />, which
///     also allows alternative types.
/// </summary>
/// <remarks>
///     A value matches when it matches any of the types. As the selected values are then of different types,
///     <see cref="SnapshotValues{TReturn, TValue}.Convert{TResult}" /> gets an <see cref="object" /> after an
///     alternative type.
/// </remarks>
/// <typeparam name="TReturn">What an action returns.</typeparam>
/// <typeparam name="TValue">The type of the selected values.</typeparam>
public sealed class SnapshotValuesOfType<TReturn, TValue> : SnapshotValues<TReturn, TValue>
{
	private readonly Func<SnapshotRule, TReturn> _add;
	private readonly ValueSelection _selectionWithoutType;
	private readonly TypeFilter[] _types;

	internal SnapshotValuesOfType(Func<SnapshotRule, TReturn> add, ValueSelection selectionWithoutType,
		TypeFilter[] types)
		: base(add, selectionWithoutType.OfType(types))
	{
		_add = add;
		_selectionWithoutType = selectionWithoutType;
		_types = types;
	}

	/// <summary>
	///     Also selects the values of the type <typeparamref name="T" />, like
	///     <see cref="SnapshotValues{TReturn, TValue}.OfType{T}" />.
	/// </summary>
	public SnapshotValuesOfType<TReturn, object> OrOfType<T>()
		=> new(_add, _selectionWithoutType, [.. _types, new TypeFilter(typeof(T), false),]);

	/// <summary>
	///     Also selects the values that are exactly of the type <typeparamref name="T" />, like
	///     <see cref="SnapshotValues{TReturn, TValue}.OfExactType{T}" />.
	/// </summary>
	public SnapshotValuesOfType<TReturn, object> OrOfExactType<T>()
		=> new(_add, _selectionWithoutType, [.. _types, new TypeFilter(typeof(T), true),]);
}
