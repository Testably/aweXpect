using System;
using System.Runtime.CompilerServices;
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

	/// <summary>
	///     Filter for types without attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type are considered as well; restrict the filter to the
	///     attributes declared directly on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult Without<TAttribute>(this Filtered.Types @this,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> new(@this, memberScope =>
		{
			bool directly = memberScope.IsDirect();
			return Filter.Suffix<Type>(
				type => !type.HasAttribute(predicate, inherit: !directly),
				() => $"without {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} ");
		});
}
