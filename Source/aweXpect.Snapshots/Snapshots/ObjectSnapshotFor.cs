using System;

namespace aweXpect.Snapshots;

/// <summary>
///     Selects what a rule of an object snapshot applies to: the members or values of the object, or text in the
///     written snapshot.
/// </summary>
/// <remarks>
///     A rule is written as a selection, optional filters, then exactly one action, e.g.
///     <c>.For.Members().WithName("Id").Mask()</c>. A selection only offers the actions that fit it.<br />
///     <see cref="Members" /> and <see cref="Values" /> apply while the object is written, and the other selections to
///     the written text.
/// </remarks>
/// <typeparam name="TResult">What an action returns, the result of the expectation.</typeparam>
public sealed class ObjectSnapshotFor<TResult> : SnapshotTextFor<TResult>
{
	internal ObjectSnapshotFor(Func<SnapshotRule, TResult> add) : base(add)
	{
	}

	/// <summary>
	///     Selects the members of the objects in an object snapshot, at any depth, e.g. to leave one out.
	/// </summary>
	/// <remarks>
	///     The items of a collection, the values of a dictionary and the items of a tuple are not members, so that e.g.
	///     <c>.For.Members().OfType&lt;DateTime&gt;().Ignore()</c> does not write a <c>List&lt;DateTime&gt;</c> as an
	///     empty list; select them with <see cref="Values" />.<br />
	///     On a generic subject whose type argument is <see cref="string" />, which has no members, the action throws an
	///     <see cref="InvalidOperationException" />.
	/// </remarks>
	public SnapshotMembers<TResult, object> Members()
		=> new(Add, ValueSelection.Members);

	/// <summary>
	///     Selects the values in an object snapshot: the members, and the items of tuples, collections and dictionaries.
	/// </summary>
	/// <remarks>
	///     On a generic subject whose type argument is <see cref="string" />, which has no members, the action throws an
	///     <see cref="InvalidOperationException" />.
	/// </remarks>
	public SnapshotValues<TResult, object> Values()
		=> new(Add, ValueSelection.Values);
}
