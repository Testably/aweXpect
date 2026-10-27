using System;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;

namespace aweXpect.Snapshots;

/// <summary>
///     A type that a value in an object snapshot is filtered by, with <c>OfType</c> or <c>OfExactType</c>.
/// </summary>
internal sealed class TypeFilter
{
	private readonly bool _isExact;
	private readonly Type _type;

	public TypeFilter(Type type, bool isExact)
	{
		_type = type;
		_isExact = isExact;
	}

	/// <summary>
	///     The name of a numbered placeholder for a value of this type, e.g. <c>Guid</c>.
	/// </summary>
	public string PlaceholderName => ValueFilters.GetPlaceholderName(_type);

	/// <summary>
	///     Describes the filter for the failure, e.g. <c>of type Guid</c>.
	/// </summary>
	public string Description => (_isExact ? "of exact type " : "of type ") + Formatter.Format(_type);

	public bool Matches(FormattedValue value)
		=> _isExact ? ValueFilters.IsOfExactType(value, _type) : ValueFilters.IsOfType(value, _type);
}
