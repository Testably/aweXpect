using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with a <see langword="params" /> parameter.
	/// </summary>
	public static Filtered.Constructors WithParamsParameter(this Filtered.Constructors @this)
	{
		IChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo => constructorInfo.GetParameters().Any(p => p.IsParamsParameter()),
			"with params parameter ");
		return @this.Which(filter);
	}

	/// <summary>
	///     Filter for constructors with a <see langword="params" /> parameter of type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithParamsParameter<T>(this Filtered.Constructors @this)
		=> @this.WithParameter<T>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Filter for constructors with a <see langword="params" /> parameter of type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithParamsParameter<T>(this Filtered.Constructors @this, string expected)
		=> @this.WithParameter<T>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Filter for constructors with a <see langword="params" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithParamsParameter(this Filtered.Constructors @this, string expected)
		=> @this.WithParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");
}
