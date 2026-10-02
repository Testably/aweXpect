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
	public static TSelf Equivalent<TType, TThat, [RequiresMemberMetadata] TElement, TSelf>(
		this ObjectEqualityResult<TType, TThat, TElement, TSelf> result,
		Func<EquivalencyOptions, EquivalencyOptions>? options = null)
		where TSelf : ObjectEqualityResult<TType, TThat, TElement, TSelf>
	{
		((IOptionsProvider<ObjectEqualityOptions<TElement>>)result).Options.Equivalent(
			EquivalencyOptionsExtensions.FromCallback(options), result);
		return (TSelf)result;
	}

	/// <summary>
	///     Use equivalency to compare objects.
	/// </summary>
	public static TSelf Equivalent<TCollection, [RequiresMemberMetadata] TItem, TSelf>(
		this ObjectHasItemResult<TCollection, TItem, TSelf> result,
		Func<EquivalencyOptions, EquivalencyOptions>? options = null)
		where TSelf : ObjectHasItemResult<TCollection, TItem, TSelf>
	{
		((IOptionsProvider<ObjectEqualityOptions<TItem>>)result).Options.Equivalent(
			EquivalencyOptionsExtensions.FromCallback(options), result);
		return (TSelf)result;
	}

	/// <summary>
	///     Use equivalency to compare objects.
	/// </summary>
	public static TSelf Equivalent<TType, TThat, [RequiresMemberMetadata] TElement, TSelf>(
		this ObjectCountResult<TType, TThat, TElement, TSelf> result,
		Func<EquivalencyOptions, EquivalencyOptions>? options = null)
		where TSelf : ObjectCountResult<TType, TThat, TElement, TSelf>
	{
		((IOptionsProvider<ObjectEqualityOptions<TElement>>)result).Options.Equivalent(
			EquivalencyOptionsExtensions.FromCallback(options), result);
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

	private static void Equivalent<TSubject>(this ObjectEqualityOptions<TSubject> options,
		EquivalencyOptions equivalencyOptions, IOptionsProvider<ExpectationBuilder> result)
	{
		options.Equivalent(equivalencyOptions);
		result.Options.AddEquivalencyContext(equivalencyOptions);
	}

	internal static void AddEquivalencyContext(this ExpectationBuilder expectationBuilder,
		EquivalencyOptions equivalencyOptions)
		=> expectationBuilder.AddContext(
			new ResultContext.SyncCallback("Equivalency options",
				equivalencyOptions.ToString,
				int.MinValue));
}
