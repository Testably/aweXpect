using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Tests;

public static class HasItemResultExtensions
{
	public static TResult WithInvalidMatch<TResult>(this TResult hasItemResult)
		where TResult : IOptionsProvider<CollectionIndexOptions>
	{
		hasItemResult.Options.SetMatch(new InvalidMatch());
		return hasItemResult;
	}

	private class InvalidMatch : CollectionIndexOptions.IMatch
	{
		public string GetDescription() => " with invalid match";

		public bool OnlySingleIndex() => false;
	}
}
