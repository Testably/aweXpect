using System;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	private const string DirectText = "direct ";

	/// <summary>
	///     Filter for types with attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type are considered as well; restrict the filter to the
	///     attributes declared directly on the type with <see cref="TypesWith.Directly" />.
	/// </remarks>
	public static TypesWith With<TAttribute>(this Filtered.Types @this)
		where TAttribute : Attribute
		=> new(@this, new AttributeFilter<Type>(
			(type, inherit) => type.HasAttribute<TAttribute>(inherit: inherit),
			directly => $"with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} "));

	/// <summary>
	///     Filter for types with attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type are considered as well; restrict the filter to the
	///     attributes declared directly on the type with <see cref="TypesWith.Directly" />.
	/// </remarks>
	public static TypesWith With<TAttribute>(this Filtered.Types @this,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> new(@this, new AttributeFilter<Type>(
			(type, inherit) => type.HasAttribute(predicate, inherit),
			directly => $"with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));

	/// <summary>
	///     Additional filters on types with an attribute.
	/// </summary>
	public class TypesWith : Filtered.Types
	{
		private readonly AttributeFilter<Type> _filter;
		private readonly Filtered.Types _source;

		internal TypesWith(Filtered.Types source, AttributeFilter<Type> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		public TypesWith OrWith<TAttribute>()
			where TAttribute : Attribute
			=> new(_source, _filter.Or(
				(type, inherit) => type.HasAttribute<TAttribute>(inherit: inherit),
				directly => $"or with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} "));

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		public TypesWith OrWith<TAttribute>(
			Func<TAttribute, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
			=> new(_source, _filter.Or(
				(type, inherit) => type.HasAttribute(predicate, inherit),
				directly => $"or with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));

		/// <summary>
		///     Only considers the attributes declared directly on the type, not those inherited from a base type.
		/// </summary>
		/// <remarks>
		///     Applies to all attributes of the filter, also to those added via <see cref="OrWith{TAttribute}()" />.
		/// </remarks>
		public TypesWith Directly()
			=> new(_source, _filter.Directly());
	}
}
