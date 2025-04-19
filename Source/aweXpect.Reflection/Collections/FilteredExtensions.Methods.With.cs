using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Extensions;

namespace aweXpect.Reflection.Collections;

public static partial class FilteredExtensions
{
	/// <summary>
	///     Filter for methods with attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static MethodsWith With<TAttribute>(this Filtered.Methods @this, bool inherit = true)
		where TAttribute : Attribute
	{
		IChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.HasAttribute<TAttribute>(inherit: inherit),
			$"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} ");
		return new MethodsWith(@this.Which(filter), filter);
	}

	/// <summary>
	///     Filter for methods with attribute of type <typeparamref name="TAttribute" /> that
	///     match the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static MethodsWith With<TAttribute>(this Filtered.Methods @this,
		Func<TAttribute, bool>? predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		IChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.HasAttribute(predicate, inherit),
			$"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue} ");
		return new MethodsWith(@this.Which(filter), filter);
	}

	/// <summary>
	///     Additional filters on methods with an attribute.
	/// </summary>
	public class MethodsWith(Filtered.Methods inner, IChangeableFilter<MethodInfo> filter) : Filtered.Methods(inner)
	{
		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public MethodsWith OrWith<TAttribute>(bool inherit = true)
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
		public MethodsWith OrWith<TAttribute>(
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
