using System;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Equality options with a tolerance for the items of a collection that tell whether their match type is still the
///     default one.
/// </summary>
/// <remarks>
///     A set with a custom comparer keeps deciding which items are the same until an option, e.g. <c>Within(…)</c>,
///     changes the comparison, as it does for the element types without a tolerance.
/// </remarks>
internal sealed class ItemEqualityWithToleranceOptions<TSubject, TTolerance>
	: ObjectEqualityWithToleranceOptions<TSubject, TTolerance>, IHasDefaultMatchType
{
	private readonly IObjectMatchType _defaultMatchType;

	public ItemEqualityWithToleranceOptions(Func<TSubject, TSubject, TTolerance, bool> isWithinTolerance,
		Func<TTolerance, string> toString, Func<TTolerance>? defaultTolerance = null)
		: base(isWithinTolerance, toString)
	{
		if (defaultTolerance is not null)
		{
			WithDefaultTolerance(defaultTolerance);
		}

		_defaultMatchType = MatchType;
	}

	public bool HasDefaultMatchType => ReferenceEquals(MatchType, _defaultMatchType);
}
