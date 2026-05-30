using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods with a <see langword="ref" /> parameter.
	/// </summary>
	public static Filtered.Methods WithRefParameter(this Filtered.Methods @this)
	{
		IChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.GetParameters().Any(p => p.IsRefParameter()),
			"with ref parameter ");
		return @this.Which(filter);
	}

	/// <summary>
	///     Filter for methods with a <see langword="ref" /> parameter of type <typeparamref name="T" />.
	/// </summary>
	public static MethodsWithParameter<T> WithRefParameter<T>(this Filtered.Methods @this)
		=> @this.WithParameter<T>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Filter for methods with a <see langword="ref" /> parameter of type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<T> WithRefParameter<T>(this Filtered.Methods @this, string expected)
		=> @this.WithParameter<T>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Filter for methods with a <see langword="ref" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithRefParameter(this Filtered.Methods @this, string expected)
		=> @this.WithParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");
}
