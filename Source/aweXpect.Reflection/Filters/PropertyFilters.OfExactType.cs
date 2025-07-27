using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Filter for properties of exact type <typeparamref name="TProperty" />.
	/// </summary>
	public static PropertiesOfType OfExactType<TProperty>(this Filtered.Properties @this)
		=> OfExactType(@this, typeof(TProperty));

	/// <summary>
	///     Filter for properties of exact type <paramref name="propertyType" />.
	/// </summary>
	public static PropertiesOfType OfExactType(this Filtered.Properties @this,
		Type propertyType)
	{
		IChangeableFilter<PropertyInfo> filter = Filter.Suffix<PropertyInfo>(
			propertyInfo => propertyInfo.PropertyType.IsOrInheritsFrom(propertyType, true),
			$"of exact type {Formatter.Format(propertyType)} ");
		return new PropertiesOfType(@this.Which(filter), filter);
	}

	public partial class PropertiesOfType
	{
		/// <summary>
		///     Allow an alternative type <typeparamref name="TProperty" /> exactly.
		/// </summary>
		public PropertiesOfType OrOfExactType<TProperty>()
			=> OrOfExactType(typeof(TProperty));

		/// <summary>
		///     Allow an alternative type <paramref name="propertyType" /> exactly.
		/// </summary>
		public PropertiesOfType OrOfExactType(Type propertyType)
		{
			filter.UpdateFilter(
				(result, propertyInfo)
					=> result || propertyInfo.PropertyType.IsOrInheritsFrom(propertyType, true),
				description
					=> $"{description}or of exact type {Formatter.Format(propertyType)} ");
			return this;
		}
	}
}
