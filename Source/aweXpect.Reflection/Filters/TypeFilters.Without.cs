using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types without attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type are considered as well; restrict the filter to the
	///     attributes declared directly on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult Without<TAttribute>(this Filtered.Types @this)
		where TAttribute : Attribute
		=> new(@this, memberScope =>
		{
			bool directly = memberScope.IsDirect();
			return Filter.Suffix<Type>(
				type => !type.HasAttribute<TAttribute>(inherit: !directly),
				() => $"without {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} ");
		});
}
