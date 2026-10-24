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
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the filter to the
	///     attributes declared directly on the property with <see cref="PropertiesWith.Directly" />.
	/// </remarks>
	public static PropertiesWith With<TAttribute>(this Filtered.Properties @this)
		where TAttribute : Attribute
		=> new(@this, new AttributeFilter<PropertyInfo>(
			(propertyInfo, inherit) => propertyInfo.HasAttribute<TAttribute>(inherit: inherit),
			directly => $"with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} "));

	/// <summary>
	///     Filter for properties with attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the filter to the
	///     attributes declared directly on the property with <see cref="PropertiesWith.Directly" />.
	/// </remarks>
	public static PropertiesWith With<TAttribute>(this Filtered.Properties @this,
		Func<TAttribute, bool>? predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> new(@this, new AttributeFilter<PropertyInfo>(
			(propertyInfo, inherit) => propertyInfo.HasAttribute(predicate, inherit),
			directly => $"with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));

	/// <summary>
	///     Additional filters on properties with an attribute.
	/// </summary>
	public class PropertiesWith : Filtered.Properties
	{
		private readonly AttributeFilter<PropertyInfo> _filter;
		private readonly Filtered.Properties _source;

		internal PropertiesWith(Filtered.Properties source, AttributeFilter<PropertyInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		public PropertiesWith OrWith<TAttribute>()
			where TAttribute : Attribute
			=> new(_source, _filter.Or(
				(propertyInfo, inherit) => propertyInfo.HasAttribute<TAttribute>(inherit: inherit),
				directly => $"or with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} "));

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		public PropertiesWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
			=> new(_source, _filter.Or(
				(propertyInfo, inherit) => propertyInfo.HasAttribute(predicate, inherit),
				directly => $"or with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));

		/// <summary>
		///     Only considers the attributes declared directly on the property, not those inherited from a base type or an overridden member.
		/// </summary>
		/// <remarks>
		///     Applies to all attributes of the filter, also to those added via <see cref="OrWith{TAttribute}()" />.
		/// </remarks>
		public PropertiesWith Directly()
			=> new(_source, _filter.Directly());
	}
}
