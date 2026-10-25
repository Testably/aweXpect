using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The predicate that selects the items of an expectation.
/// </summary>
/// <remarks>
///     An immutable value: <see cref="SetPredicate(Func{TItem, bool}, string)" /> returns new options, and
///     <see langword="default" /> matches every item, like <see cref="Default" />.
/// </remarks>
internal readonly struct PredicateOptions<TItem>
{
	private readonly Func<TItem, bool>? _predicate;
	private readonly string? _predicateDescription;

	private PredicateOptions(Func<TItem, bool> predicate, string predicateDescription)
	{
		_predicate = predicate;
		_predicateDescription = predicateDescription;
	}

	public static PredicateOptions<TItem> Default => default;

	public bool Matches(TItem item)
		=> _predicate is null || UserCode.Invoke(_predicate, item, "the predicate");

	public PredicateOptions<TItem> SetPredicate(Func<TItem, bool> predicate, string predicateDescription)
		=> new(predicate, predicateDescription);

	public string GetDescription() => _predicateDescription ?? "";
}
