using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with a <see langword="ref" /> parameter.
	/// </summary>
	public static Filtered.Constructors WithRefParameter(this Filtered.Constructors @this)
	{
		IChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo => constructorInfo.GetParameters().Any(p => p.IsRefParameter()),
			"with ref parameter ");
		return @this.Which(filter);
	}

	/// <summary>
	///     Filter for constructors with a <see langword="ref" /> parameter of type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithRefParameter<T>(this Filtered.Constructors @this)
		=> @this.WithParameter<T>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Filter for constructors with a <see langword="ref" /> parameter of type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithRefParameter<T>(this Filtered.Constructors @this, string expected)
		=> @this.WithParameter<T>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Filter for constructors with a <see langword="ref" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithRefParameter(this Filtered.Constructors @this, string expected)
		=> @this.WithParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");
}
