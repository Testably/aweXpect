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
	public static PropertiesOfType WhichAreOfType<TProperty>(this Filtered.Properties @this)
		=> WhichAreOfType(@this, typeof(TProperty));

	/// <summary>
	///     Filter for properties of type <paramref name="propertyType" />.
	/// </summary>
	public static PropertiesOfType WhichAreOfType(this Filtered.Properties @this,
		Type propertyType)
	{
		Filter.GenericFilter<PropertyInfo> filter = Filter.Suffix<PropertyInfo>(
			propertyInfo => propertyInfo.PropertyType.IsOrInheritsFrom(propertyType),
			() => $"of type {Formatter.Format(propertyType)} ");
		return new PropertiesOfType(@this, filter);
	}

	/// <summary>
	///     Filter for properties not of type <typeparamref name="TProperty" />.
	/// </summary>
	public static Filtered.Properties WhichAreNotOfType<TProperty>(this Filtered.Properties @this)
		=> WhichAreNotOfType(@this, typeof(TProperty));

	/// <summary>
	///     Filter for properties not of type <paramref name="propertyType" />.
	/// </summary>
	public static Filtered.Properties WhichAreNotOfType(this Filtered.Properties @this,
		Type propertyType)
		=> @this.Which(Filter.Suffix<PropertyInfo>(
			propertyInfo => !propertyInfo.PropertyType.IsOrInheritsFrom(propertyType),
			() => $"not of type {Formatter.Format(propertyType)} "));

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
			=> new(_source, _filter.Extend(
				(result, propertyInfo)
					=> result || propertyInfo.PropertyType.IsOrInheritsFrom(propertyType),
				description
					=> $"{description}or of type {Formatter.Format(propertyType)} "));
	}
}
