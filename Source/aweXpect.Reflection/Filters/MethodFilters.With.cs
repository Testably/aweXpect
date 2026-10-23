using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	private const string DirectText = "direct ";

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
		Filter.GenericFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.HasAttribute<TAttribute>(inherit: inherit),
			() => $"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} ");
		return new MethodsWith(@this, filter);
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
		Filter.GenericFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.HasAttribute(predicate, inherit),
			() => $"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} ");
		return new MethodsWith(@this, filter);
	}

	/// <summary>
	///     Additional filters on methods with an attribute.
	/// </summary>
	public class MethodsWith : Filtered.Methods
	{
		private readonly Filter.GenericFilter<MethodInfo> _filter;
		private readonly Filtered.Methods _source;

		internal MethodsWith(Filtered.Methods source, Filter.GenericFilter<MethodInfo> filter)
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
		public MethodsWith OrWith<TAttribute>(bool inherit = true)
			where TAttribute : Attribute
			=> new(_source, _filter.Extend((result, methodInfo) => result || methodInfo.HasAttribute<TAttribute>(inherit: inherit),
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
		public MethodsWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			bool inherit = true,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
			=> new(_source, _filter.Extend(
				(result, methodInfo) => result || methodInfo.HasAttribute(predicate, inherit),
				description
					=> $"{description}or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));
	}
}
