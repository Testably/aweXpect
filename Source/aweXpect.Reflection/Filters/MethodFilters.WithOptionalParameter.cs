using System;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods with an optional parameter.
	/// </summary>
	public static Filtered.Methods WithOptionalParameter(this Filtered.Methods @this)
	{
		IChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.GetParameters().Any(p => p.IsOptionalParameter()),
			"with optional parameter ");
		return @this.Which(filter);
	}

	/// <summary>
	///     Filter for methods with an optional parameter of type <typeparamref name="T" />.
	/// </summary>
	public static MethodsWithParameter<T> WithOptionalParameter<T>(this Filtered.Methods @this)
		=> @this.WithParameter<T>().WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for methods with an optional parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static MethodsWithParameter<object?> WithOptionalParameter(this Filtered.Methods @this, Type parameterType)
		=> @this.WithParameter(parameterType).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for methods with an optional parameter of type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<T> WithOptionalParameter<T>(this Filtered.Methods @this, string expected)
		=> @this.WithParameter<T>(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for methods with an optional parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithOptionalParameter(this Filtered.Methods @this, Type parameterType, string expected)
		=> @this.WithParameter(parameterType, expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for methods with an optional parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithOptionalParameter(this Filtered.Methods @this, string expected)
		=> @this.WithParameter(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for methods with an optional parameter of exact type <typeparamref name="T" />.
	/// </summary>
	public static MethodsWithParameter<T> WithOptionalParameterExactly<T>(this Filtered.Methods @this)
		=> @this.WithParameterExactly<T>().WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for methods with an optional parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static MethodsWithParameter<object?> WithOptionalParameterExactly(this Filtered.Methods @this, Type parameterType)
		=> @this.WithParameterExactly(parameterType).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for methods with an optional parameter of exact type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<T> WithOptionalParameterExactly<T>(this Filtered.Methods @this, string expected)
		=> @this.WithParameterExactly<T>(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for methods with an optional parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithOptionalParameterExactly(this Filtered.Methods @this, Type parameterType, string expected)
		=> @this.WithParameterExactly(parameterType, expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");
}
