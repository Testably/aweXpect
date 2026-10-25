using System;
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
		where TSelf : IObjectEqualityResult<TSelf, TElement>
	{
		result.Options.Equivalent(EquivalencyOptionsExtensions.FromCallback(options));
		return (TSelf)result;
	}

	/// <summary>
	///     Use equivalency to compare objects.
	/// </summary>
	/// <remarks>
	///     A later call replaces the options, while another comparison, e.g. a custom comparer, is rejected, so that
	///     the failure describes the comparison that the expectation reads.
	/// </remarks>
	internal static ObjectEqualityOptions<TSubject> Equivalent<TSubject>(this ObjectEqualityOptions<TSubject> options,
		EquivalencyOptions equivalencyOptions)
	{
		options.SetMatchType(new EquivalencyMatchType<TSubject>(equivalencyOptions), nameof(Equivalent));
		return options;
	}

	/// <inheritdoc cref="Equivalent{TSubject}(ObjectEqualityOptions{TSubject}, EquivalencyOptions)" />
	internal static OptionsSlot<ObjectEqualityOptions<TSubject>> Equivalent<TSubject>(
		this OptionsSlot<ObjectEqualityOptions<TSubject>> slot, EquivalencyOptions equivalencyOptions)
	{
		slot.Value = slot.Value.Equivalent(equivalencyOptions);
		return slot;
	}
}
