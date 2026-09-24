using System;

namespace aweXpect.Equivalency;

/// <summary>
///     Extension methods for <see cref="EquivalencyOptions" />.
/// </summary>
internal static class EquivalencyOptionsExtensions
{
	/// <summary>
	///     Returns type-specific <see cref="EquivalencyTypeOptions" />.
	/// </summary>
	/// <remarks>
	///     The lookup walks the base types, most derived first: the <paramref name="type" /> is the runtime type of a
	///     value, which can never be an abstract type the user registered options for. A <see cref="Type" /> member is
	///     a <c>RuntimeType</c> at runtime, a type that cannot even be named, so an exact match alone would make
	///     <see cref="EquivalencyOptions.For{TMember}" /> unreachable for it. Interfaces are not considered, because
	///     several of them can match without an order that decides between them.<br />
	///     Without a registration, the <paramref name="defaultValue" /> of the enclosing type applies, except for its
	///     <see cref="EquivalencyTypeOptions.ComparisonType" />, which describes the enclosing type only: comparing a
	///     string member by members because its owner is would reduce it to its characters. The comparison type of
	///     the top-level options is kept instead, because those apply to the whole graph.
	/// </remarks>
	internal static EquivalencyTypeOptions GetTypeOptions(this EquivalencyOptions @this, Type? type,
		EquivalencyTypeOptions defaultValue)
	{
		for (Type? candidate = type; candidate != null; candidate = candidate.BaseType)
		{
			if (@this.CustomOptions.TryGetValue(candidate, out EquivalencyTypeOptions? customOptions))
			{
				return customOptions;
			}
		}

		if (defaultValue.ComparisonType == @this.ComparisonType)
		{
			return defaultValue;
		}

		return defaultValue with
		{
			ComparisonType = @this.ComparisonType,
		};
	}
}
