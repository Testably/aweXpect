namespace aweXpect.Equivalency;

/// <summary>
///     Extension methods for <see cref="EquivalencyOptions" />.
/// </summary>
internal static class EquivalencyOptionsExtensions
{
	/// <summary>
	///     Returns the options that a type without a registration inherits from the enclosing type.
	/// </summary>
	/// <remarks>
	///     The <paramref name="defaultValue" /> of the enclosing type applies, except for its
	///     <see cref="EquivalencyTypeOptions.ComparisonType" />, which describes the enclosing type only: comparing a
	///     string member by members because its owner is would reduce it to its characters. The comparison type of
	///     the top-level options is kept instead, because those apply to the whole graph.
	/// </remarks>
	internal static EquivalencyTypeOptions GetInheritedOptions(this EquivalencyOptions @this,
		EquivalencyTypeOptions defaultValue)
	{
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
