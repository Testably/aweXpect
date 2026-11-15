using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filter for fields without attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static Filtered.Fields Without<TAttribute>(this Filtered.Fields @this)
		where TAttribute : Attribute
		=> @this.Which(Filter.Suffix<FieldInfo>(
			fieldInfo => !fieldInfo.HasAttribute<TAttribute>(),
			() => $"without {Formatter.Format(typeof(TAttribute))} "));

	/// <summary>
	///     Filter for fields without attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Fields Without<TAttribute>(this Filtered.Fields @this,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> @this.Which(Filter.Suffix<FieldInfo>(
			fieldInfo => !fieldInfo.HasAttribute(predicate),
			() => $"without {Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));
}
