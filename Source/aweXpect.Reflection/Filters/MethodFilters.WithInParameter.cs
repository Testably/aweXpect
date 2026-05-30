using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods with an <see langword="in" /> parameter.
	/// </summary>
	public static Filtered.Methods WithInParameter(this Filtered.Methods @this)
	{
		IChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.GetParameters().Any(p => p.IsInParameter()),
			"with in parameter ");
		return @this.Which(filter);
	}

	/// <summary>
	///     Filter for methods with an <see langword="in" /> parameter of type <typeparamref name="T" />.
	/// </summary>
	public static MethodsWithParameter<T> WithInParameter<T>(this Filtered.Methods @this)
		=> @this.WithParameter<T>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Filter for methods with an <see langword="in" /> parameter of type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<T> WithInParameter<T>(this Filtered.Methods @this, string expected)
		=> @this.WithParameter<T>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Filter for methods with an <see langword="in" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithInParameter(this Filtered.Methods @this, string expected)
		=> @this.WithParameter(expected).WithModifier(p => p.IsInParameter(), "with in modifier");
}
