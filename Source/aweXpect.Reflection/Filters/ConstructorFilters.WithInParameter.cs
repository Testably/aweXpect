using System;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with an <see langword="in" /> parameter.
	/// </summary>
	public static Filtered.Constructors WithInParameter(this Filtered.Constructors @this)
	{
		IChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo => constructorInfo.GetParameters().Any(p => p.IsInParameter()),
			"with in parameter ");
		return @this.Which(filter);
	}

	/// <summary>
	///     Filter for constructors with an <see langword="in" /> parameter of type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithInParameter<T>(this Filtered.Constructors @this)
		=> @this.WithParameter<T>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="in" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ConstructorsWithParameter<object?> WithInParameter(this Filtered.Constructors @this, Type parameterType)
		=> @this.WithParameter(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="in" /> parameter of type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithInParameter<T>(this Filtered.Constructors @this, string expected)
		=> @this.WithParameter<T>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="in" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithInParameter(this Filtered.Constructors @this, Type parameterType, string expected)
		=> @this.WithParameter(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="in" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithInParameter(this Filtered.Constructors @this, string expected)
		=> @this.WithParameter(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="in" /> parameter of exact type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithInParameterExactly<T>(this Filtered.Constructors @this)
		=> @this.WithParameterExactly<T>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="in" /> parameter of exact type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithInParameterExactly<T>(this Filtered.Constructors @this, string expected)
		=> @this.WithParameterExactly<T>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="in" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ConstructorsWithParameter<object?> WithInParameterExactly(this Filtered.Constructors @this, Type parameterType)
		=> @this.WithParameterExactly(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="in" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithInParameterExactly(this Filtered.Constructors @this, Type parameterType, string expected)
		=> @this.WithParameterExactly(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");
}
