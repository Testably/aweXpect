using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with the specified number of parameters.
	/// </summary>
	public static Filtered.Constructors WithParameterCount(this Filtered.Constructors @this, int expected)
	{
		IChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo => constructorInfo.GetParameters().Length == expected,
			expected switch
			{
				0 => "without parameters ",
				1 => "with one parameter ",
				_ => $"with {expected} parameters ",
			});
		return @this.Which(filter);
	}
}
