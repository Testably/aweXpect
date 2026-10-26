using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Snapshots.Tests.TestHelpers;

internal static class InPlaceStringOption
{
	/// <remarks>
	///     A custom option that changes the holder of the result directly instead of returning a copy, as an extension
	///     without <see cref="ICopyOnOption{TOptions, TResult}" /> would.
	/// </remarks>
	public static TResult IgnoringCaseInPlace<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		result.Options.IgnoringCase();
		return result;
	}
}
