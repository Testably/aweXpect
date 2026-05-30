using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with an optional parameter.
	/// </summary>
	public static Filtered.Constructors WithOptionalParameter(this Filtered.Constructors @this)
	{
		IChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo => constructorInfo.GetParameters().Any(p => p.IsOptionalParameter()),
			"with optional parameter ");
		return @this.Which(filter);
	}

	/// <summary>
	///     Filter for constructors with an optional parameter of type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithOptionalParameter<T>(this Filtered.Constructors @this)
		=> @this.WithParameter<T>().WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for constructors with an optional parameter of type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithOptionalParameter<T>(this Filtered.Constructors @this, string expected)
		=> @this.WithParameter<T>(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for constructors with an optional parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithOptionalParameter(this Filtered.Constructors @this, string expected)
		=> @this.WithParameter(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for constructors with an optional parameter of exact type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithOptionalParameterExactly<T>(this Filtered.Constructors @this)
		=> @this.WithParameterExactly<T>().WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Filter for constructors with an optional parameter of exact type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithOptionalParameterExactly<T>(this Filtered.Constructors @this, string expected)
		=> @this.WithParameterExactly<T>(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");
}
