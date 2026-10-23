using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class CollectionMatchOptionsExtensions
{
	/// <summary>
	///     Checks if the <paramref name="actual" /> values match the <paramref name="expected" /> values according to the
	///     <paramref name="matchOptions" />, comparing each value with the <paramref name="options" />.
	/// </summary>
	public static async ValueTask<bool> Matches(this CollectionMatchOptions matchOptions, string it,
		string?[] actual, string[] expected, StringEqualityOptions options)
	{
		int maximumNumber = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
		ICollectionMatcher<string?, string?> matcher = matchOptions.GetCollectionMatcher<string?, string?>(expected);
		foreach (string? value in actual)
		{
			if ((await matcher.Verify(it, value, options, maximumNumber)).IsFailure)
			{
				return false;
			}

			if (matcher.IsDetermined)
			{
				break;
			}
		}

		return !(await matcher.VerifyComplete(it, options, maximumNumber)).IsFailure;
	}
}
