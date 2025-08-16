using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with the specified number of parameters.
	/// </summary>
	public static Filtered.Constructors WithParameterCount(this Filtered.Constructors @this, int expectedCount)
	{
		IChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo => constructorInfo.GetParameters().Length == expectedCount,
			expectedCount switch
			{
				0 => "without parameters ",
				1 => "with one parameter ",
				_ => $"with {expectedCount} parameters ",
			});
		return @this.Which(filter);
	}
}
