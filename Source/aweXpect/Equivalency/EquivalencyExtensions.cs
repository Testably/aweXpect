using System;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Metadata;
using aweXpect.Equivalency;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Extension methods for equivalency.
/// </summary>
/// <remarks>
///     Declared in the <c>aweXpect</c> namespace, so that switching an equality expectation to equivalency needs no
///     additional <c>using</c>.
/// </remarks>
public static class EquivalencyExtensions
{
	/// <summary>
	///     Use equivalency to compare objects.
	/// </summary>
	public static TSelf Equivalent<TSelf, [RequiresMemberMetadata] TElement>(
		this IObjectEqualityResult<TSelf, TElement> result,
		Func<EquivalencyOptions, EquivalencyOptions>? options = null)
	{
		result.Options.Equivalent(EquivalencyOptionsExtensions.FromCallback(options));
		return (TSelf)result;
	}

	/// <summary>
	///     Use equivalency to compare objects.
	/// </summary>
	/// <remarks>
	///     The options can be specified only once, so that later options can neither replace the ones that the
	///     failure lists nor be replaced by a custom comparer.
	/// </remarks>
	internal static ObjectEqualityOptions<TSubject> Equivalent<TSubject>(this ObjectEqualityOptions<TSubject> options,
		EquivalencyOptions equivalencyOptions)
	{
		options.SetMatchType(new EquivalencyComparer(equivalencyOptions), nameof(Equivalent));
		return options;
	}

	/// <summary>
	///     Adds the <paramref name="equivalencyOptions" /> as context.
	/// </summary>
	internal static void AddEquivalencyContext(this ResultContextCollector contexts,
		EquivalencyOptions equivalencyOptions)
		=> contexts.Add(new ResultContext.SyncCallback("Equivalency options", equivalencyOptions.ToString,
			int.MinValue));
}
