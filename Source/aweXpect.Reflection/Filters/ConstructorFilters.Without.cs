using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors without attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static Filtered.Constructors Without<TAttribute>(this Filtered.Constructors @this)
		where TAttribute : Attribute
		=> @this.Which(Filter.Suffix<ConstructorInfo>(
			constructorInfo => !constructorInfo.HasAttribute<TAttribute>(),
			() => $"without {Formatter.Format(typeof(TAttribute))} "));

	/// <summary>
	///     Filter for constructors without attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Constructors Without<TAttribute>(this Filtered.Constructors @this,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> @this.Which(Filter.Suffix<ConstructorInfo>(
			constructorInfo => !constructorInfo.HasAttribute(predicate),
			() => $"without {Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));
}
