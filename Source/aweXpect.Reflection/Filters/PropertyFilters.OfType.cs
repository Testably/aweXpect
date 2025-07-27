using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filter for properties of type <typeparamref name="TProperty" />.
	/// </summary>
	public static PropertiesOfType OfType<TProperty>(this Filtered.Properties @this)
		=> OfType(@this, typeof(TProperty));

	/// <summary>
	///     Filter for properties of type <paramref name="propertyType" />.
	/// </summary>
	public static PropertiesOfType OfType(this Filtered.Properties @this,
		Type propertyType)
	{
		IChangeableFilter<PropertyInfo> filter = Filter.Suffix<PropertyInfo>(
			propertyInfo => propertyInfo.PropertyType.IsOrInheritsFrom(propertyType),
			$"of type {Formatter.Format(propertyType)} ");
		return new PropertiesOfType(@this.Which(filter), filter);
	}

	public partial class PropertiesOfType
	{
		/// <summary>
		///     Allow an alternative type <typeparamref name="TProperty" />.
		/// </summary>
		public PropertiesOfType OrOfType<TProperty>()
			=> OrOfType(typeof(TProperty));

		/// <summary>
		///     Allow an alternative type <paramref name="propertyType" />.
		/// </summary>
		public PropertiesOfType OrOfType(Type propertyType)
		{
			filter.UpdateFilter(
				(result, propertyInfo)
					=> result || propertyInfo.PropertyType.IsOrInheritsFrom(propertyType),
				description
					=> $"{description}or of type {Formatter.Format(propertyType)} ");
			return this;
		}
	}
}
