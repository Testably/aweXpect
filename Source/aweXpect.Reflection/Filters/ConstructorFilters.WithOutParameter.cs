using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with an <see langword="out" /> parameter.
	/// </summary>
	public static Filtered.Constructors WithOutParameter(this Filtered.Constructors @this)
	{
		IChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo => constructorInfo.GetParameters().Any(p => p.IsOutParameter()),
			"with out parameter ");
		return @this.Which(filter);
	}

	/// <summary>
	///     Filter for constructors with an <see langword="out" /> parameter of type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithOutParameter<T>(this Filtered.Constructors @this)
		=> @this.WithParameter<T>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="out" /> parameter of type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithOutParameter<T>(this Filtered.Constructors @this, string expected)
		=> @this.WithParameter<T>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="out" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithOutParameter(this Filtered.Constructors @this, string expected)
		=> @this.WithParameter(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="out" /> parameter of exact type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithOutParameterExactly<T>(this Filtered.Constructors @this)
		=> @this.WithParameterExactly<T>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Filter for constructors with an <see langword="out" /> parameter of exact type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithOutParameterExactly<T>(this Filtered.Constructors @this, string expected)
		=> @this.WithParameterExactly<T>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");
}
