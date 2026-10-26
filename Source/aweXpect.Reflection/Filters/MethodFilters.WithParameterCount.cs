using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods with the specified number of parameters.
	/// </summary>
	public static Filtered.Methods WithParameterCount(this Filtered.Methods @this, int expected)
	{
		IFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.GetParameters().Length == expected,
			expected switch
			{
				0 => "without parameters ",
				1 => "with one parameter ",
				_ => $"with {expected} parameters ",
			});
		return @this.Which(filter);
	}
}
