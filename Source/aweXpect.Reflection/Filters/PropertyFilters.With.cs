using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	private const string DirectText = "direct ";

	/// <summary>
	///     Filter for properties with attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static PropertiesWith With<TAttribute>(this Filtered.Properties @this, bool inherit = true)
		where TAttribute : Attribute
	{
		IChangeableFilter<PropertyInfo> filter = Filter.Suffix<PropertyInfo>(
			propertyInfo => propertyInfo.HasAttribute<TAttribute>(inherit: inherit),
			$"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} ");
		return new PropertiesWith(@this.Which(filter), filter);
	}

	/// <summary>
	///     Filter for properties with attribute of type <typeparamref name="TAttribute" /> that
	///     match the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static PropertiesWith With<TAttribute>(this Filtered.Properties @this,
		Func<TAttribute, bool>? predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		IChangeableFilter<PropertyInfo> filter = Filter.Suffix<PropertyInfo>(
			propertyInfo => propertyInfo.HasAttribute(predicate, inherit),
			$"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue} ");
		return new PropertiesWith(@this.Which(filter), filter);
	}

	/// <summary>
	///     Additional filters on properties with an attribute.
	/// </summary>
	public class PropertiesWith(Filtered.Properties inner, IChangeableFilter<PropertyInfo> filter) : Filtered.Properties(inner)
	{
		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public PropertiesWith OrWith<TAttribute>(bool inherit = true)
			where TAttribute : Attribute
		{
			filter.UpdateFilter((result, propertyInfo) => result || propertyInfo.HasAttribute<TAttribute>(inherit: inherit),
				description
					=> $"{description}or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} ");
			return this;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public PropertiesWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			bool inherit = true,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
		{
			filter.UpdateFilter(
				(result, propertyInfo) => result || propertyInfo.HasAttribute(predicate, inherit),
				description
					=> $"{description}or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue} ");
			return this;
		}
	}
}
