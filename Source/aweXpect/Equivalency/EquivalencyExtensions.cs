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
		((IOptionsProvider<ObjectEqualityOptions<TElement>>)result).Options.SetMatchType(
			new EquivalencyComparer(EquivalencyOptionsExtensions.FromCallback(options)));
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
		((IOptionsProvider<ObjectEqualityOptions<TItem>>)result).Options.SetMatchType(
			new EquivalencyComparer(EquivalencyOptionsExtensions.FromCallback(options)));
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
		((IOptionsProvider<ObjectEqualityOptions<TElement>>)result).Options.SetMatchType(
			new EquivalencyComparer(EquivalencyOptionsExtensions.FromCallback(options)));
		return (TSelf)result;
	}

	/// <summary>
	///     Use equivalency to compare objects.
	/// </summary>
	internal static ObjectEqualityOptions<TSubject> Equivalent<TSubject>(this ObjectEqualityOptions<TSubject> options,
		EquivalencyOptions equivalencyOptions)
	{
		options.SetMatchType(new EquivalencyComparer(equivalencyOptions));
		return options;
	}

	internal static void AddEquivalencyContext(this ExpectationBuilder expectationBuilder,
		EquivalencyOptions equivalencyOptions)
		=> expectationBuilder.AddContext(
			new ResultContext.SyncCallback("Equivalency options",
				equivalencyOptions.ToString,
				int.MinValue));
}
