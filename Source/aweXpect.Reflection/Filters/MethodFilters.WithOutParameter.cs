using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods with an <see langword="out" /> parameter.
	/// </summary>
	public static Filtered.Methods WithOutParameter(this Filtered.Methods @this)
	{
		IChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.GetParameters().Any(p => p.IsOutParameter()),
			"with out parameter ");
		return @this.Which(filter);
	}

	/// <summary>
	///     Filter for methods with an <see langword="out" /> parameter of type <typeparamref name="T" />.
	/// </summary>
	public static MethodsWithParameter<T> WithOutParameter<T>(this Filtered.Methods @this)
		=> @this.WithParameter<T>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Filter for methods with an <see langword="out" /> parameter of type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<T> WithOutParameter<T>(this Filtered.Methods @this, string expected)
		=> @this.WithParameter<T>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Filter for methods with an <see langword="out" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithOutParameter(this Filtered.Methods @this, string expected)
		=> @this.WithParameter(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");
}
