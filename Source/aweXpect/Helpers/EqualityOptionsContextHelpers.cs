using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class EqualityOptionsContextHelpers
{
	/// <summary>
	///     Adds the contexts of the match type of the <paramref name="options" />, e.g. the equivalency options.
	/// </summary>
	public static void AddOptionsContexts<T>(this ResultContextCollector contexts, IOptionsEquality<T> options)
	{
		if (options is IOptionsProvider<IOptionsEquality<T>> provider)
		{
			contexts.AddOptionsContexts(provider.Options);
		}
		else if (options is ObjectEqualityOptions<T> objectEqualityOptions)
		{
			objectEqualityOptions.AppendContexts(contexts);
		}
	}
}
