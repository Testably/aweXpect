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
	public static PropertiesOfType WhichAreOfExactType<TProperty>(this Filtered.Properties @this)
		=> WhichAreOfExactType(@this, typeof(TProperty));

	/// <summary>
	///     Filter for properties of exact type <paramref name="propertyType" />.
	/// </summary>
	public static PropertiesOfType WhichAreOfExactType(this Filtered.Properties @this,
		Type propertyType)
	{
		Filter.GenericFilter<PropertyInfo> filter = Filter.Suffix<PropertyInfo>(
			propertyInfo => propertyInfo.PropertyType.IsOrInheritsFrom(propertyType, true),
			() => $"of exact type {Formatter.Format(propertyType)} ");
		return new PropertiesOfType(@this, filter);
	}

	/// <summary>
	///     Filter for properties not of exact type <typeparamref name="TProperty" />.
	/// </summary>
	public static Filtered.Properties WhichAreNotOfExactType<TProperty>(this Filtered.Properties @this)
		=> WhichAreNotOfExactType(@this, typeof(TProperty));

	/// <summary>
	///     Filter for properties not of exact type <paramref name="propertyType" />.
	/// </summary>
	public static Filtered.Properties WhichAreNotOfExactType(this Filtered.Properties @this,
		Type propertyType)
		=> @this.Which(Filter.Suffix<PropertyInfo>(
			propertyInfo => !propertyInfo.PropertyType.IsOrInheritsFrom(propertyType, true),
			() => $"not of exact type {Formatter.Format(propertyType)} "));

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
			=> new(_source, _filter.Extend(
				(result, propertyInfo)
					=> result || propertyInfo.PropertyType.IsOrInheritsFrom(propertyType, true),
				description
					=> $"{description}or of exact type {Formatter.Format(propertyType)} "));
	}
}
