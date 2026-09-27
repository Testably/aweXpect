using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Tests;

public static class HasItemResultExtensions
{
	public static AndOrResult<TCollection, IThat<TCollection?>> WithInvalidMatch<TCollection>(
		this HasItemResult<TCollection> hasItemResult)
	{
		(hasItemResult as IOptionsProvider<CollectionIndexOptions>).Options.SetMatch(new InvalidMatch());
		return hasItemResult;
	}

	private class InvalidMatch : CollectionIndexOptions.IMatch
	{
		public string GetDescription() => " with invalid match";

		public bool OnlySingleIndex() => false;
	}
}
