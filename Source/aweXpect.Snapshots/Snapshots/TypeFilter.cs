using System;
using aweXpect.Formatting;

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

	public bool Matches(FormattedValue value)
		=> _isExact ? ValueFilters.IsOfExactType(value, _type) : ValueFilters.IsOfType(value, _type);
}
