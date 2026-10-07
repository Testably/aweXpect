using System;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect.Options;

internal class PredicateOptions<TItem>
{
	private Func<TItem, bool>? _predicate;
	private string? _predicateDescription;

	public bool Matches(TItem item)
		=> _predicate is null || UserCode.Invoke(_predicate, item, "the predicate");

	internal void SetPredicate(Func<TItem, bool> predicate, string predicateDescription)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_predicate is not null, "Matching");
		_predicate = predicate;
		_predicateDescription = predicateDescription;
	}

	public string GetDescription()
	{
		if (_predicateDescription is null)
		{
			return "";
		}

		return _predicateDescription;
	}
}
