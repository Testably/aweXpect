using System;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Extensions;

namespace aweXpect.Reflection.Collections;

public static partial class FilteredExtensions
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
		IChangeableFilter<Type> filter = Filter.Suffix<Type>(type => type.HasAttribute<TAttribute>(inherit: inherit),
			$"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} ");
		return new TypesWith(@this.Which(filter), filter);
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
		IChangeableFilter<Type> filter = Filter.Suffix<Type>(type => type.HasAttribute(predicate, inherit),
			$"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue} ");
		return new TypesWith(@this.Which(filter), filter);
	}

	/// <summary>
	///     Additional filters on types with an attribute.
	/// </summary>
	public class TypesWith(Filtered.Types inner, IChangeableFilter<Type> filter) : Filtered.Types(inner)
	{
		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public TypesWith OrWith<TAttribute>(bool inherit = true)
			where TAttribute : Attribute
		{
			filter.UpdateFilter((result, type) => result || type.HasAttribute<TAttribute>(inherit: inherit),
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
		public TypesWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			bool inherit = true,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
		{
			filter.UpdateFilter(
				(result, type) => result || type.HasAttribute(predicate, inherit),
				description
					=> $"{description}or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue} ");
			return this;
		}
	}
}
