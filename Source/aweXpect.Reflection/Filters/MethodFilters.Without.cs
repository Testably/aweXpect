using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods without attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the filter to the
	///     attributes declared directly on the method with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult Without<TAttribute>(this Filtered.Methods @this)
		where TAttribute : Attribute
		=> new(@this, new AttributeFilter<MethodInfo>(
			(methodInfo, inherit) => !methodInfo.HasAttribute<TAttribute>(inherit: inherit),
			directly => $"without {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} "));
}
