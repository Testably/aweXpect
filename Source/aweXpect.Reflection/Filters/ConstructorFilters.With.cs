using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static ConstructorsWith With<TAttribute>(this Filtered.Constructors @this)
		where TAttribute : Attribute
	{
		IChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo => constructorInfo.HasAttribute<TAttribute>(),
			$"with {Formatter.Format(typeof(TAttribute))} ");
		return new ConstructorsWith(@this.Which(filter), filter);
	}

	/// <summary>
	///     Filter for constructors with attribute of type <typeparamref name="TAttribute" /> that
	///     match the <paramref name="predicate" />.
	/// </summary>
	public static ConstructorsWith With<TAttribute>(this Filtered.Constructors @this,
		Func<TAttribute, bool>? predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		IChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo => constructorInfo.HasAttribute(predicate),
			$"with {Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} ");
		return new ConstructorsWith(@this.Which(filter), filter);
	}

	/// <summary>
	///     Additional filters on constructors with an attribute.
	/// </summary>
	public class ConstructorsWith(Filtered.Constructors inner, IChangeableFilter<ConstructorInfo> filter)
		: Filtered.Constructors(inner)
	{
		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		public ConstructorsWith OrWith<TAttribute>()
			where TAttribute : Attribute
		{
			filter.UpdateFilter((result, constructorInfo) => result || constructorInfo.HasAttribute<TAttribute>(),
				description
					=> $"{description}or with {Formatter.Format(typeof(TAttribute))} ");
			return this;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		public ConstructorsWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
		{
			filter.UpdateFilter(
				(result, constructorInfo) => result || constructorInfo.HasAttribute(predicate),
				description
					=> $"{description}or with {Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} ");
			return this;
		}
	}
}
