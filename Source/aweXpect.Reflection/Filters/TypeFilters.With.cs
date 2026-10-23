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
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static TypesWith With<TAttribute>(this Filtered.Types @this, bool inherit = true)
		where TAttribute : Attribute
	{
		Filter.GenericFilter<Type> filter = Filter.Suffix<Type>(type => type.HasAttribute<TAttribute>(inherit: inherit),
			() => $"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} ");
		return new TypesWith(@this, filter);
	}

	/// <summary>
	///     Filter for types with attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static TypesWith With<TAttribute>(this Filtered.Types @this,
		Func<TAttribute, bool>? predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		Filter.GenericFilter<Type> filter = Filter.Suffix<Type>(type => type.HasAttribute(predicate, inherit),
			() => $"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} ");
		return new TypesWith(@this, filter);
	}

	/// <summary>
	///     Additional filters on types with an attribute.
	/// </summary>
	public class TypesWith : Filtered.Types
	{
		private readonly Filter.GenericFilter<Type> _filter;
		private readonly Filtered.Types _source;

		internal TypesWith(Filtered.Types source, Filter.GenericFilter<Type> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public TypesWith OrWith<TAttribute>(bool inherit = true)
			where TAttribute : Attribute
			=> new(_source, _filter.Extend((result, type) => result || type.HasAttribute<TAttribute>(inherit: inherit),
				description
					=> $"{description}or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} "));

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public TypesWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			bool inherit = true,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
			=> new(_source, _filter.Extend(
				(result, type) => result || type.HasAttribute(predicate, inherit),
				description
					=> $"{description}or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));
	}
}
