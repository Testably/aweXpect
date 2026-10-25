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
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the filter to the
	///     attributes declared directly on the method with <see cref="MethodsWith.Directly" />.
	/// </remarks>
	public static MethodsWith With<TAttribute>(this Filtered.Methods @this)
		where TAttribute : Attribute
		=> new(@this, new AttributeFilter<MethodInfo>(
			(methodInfo, inherit) => methodInfo.HasAttribute<TAttribute>(inherit: inherit),
			directly => $"with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} "));

	/// <summary>
	///     Filter for methods with attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the filter to the
	///     attributes declared directly on the method with <see cref="MethodsWith.Directly" />.
	/// </remarks>
	public static MethodsWith With<TAttribute>(this Filtered.Methods @this,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> new(@this, new AttributeFilter<MethodInfo>(
			(methodInfo, inherit) => methodInfo.HasAttribute(predicate, inherit),
			directly => $"with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));

	/// <summary>
	///     Additional filters on methods with an attribute.
	/// </summary>
	public class MethodsWith : Filtered.Methods
	{
		private readonly AttributeFilter<MethodInfo> _filter;
		private readonly Filtered.Methods _source;

		internal MethodsWith(Filtered.Methods source, AttributeFilter<MethodInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		public MethodsWith OrWith<TAttribute>()
			where TAttribute : Attribute
			=> new(_source, _filter.Or(
				(methodInfo, inherit) => methodInfo.HasAttribute<TAttribute>(inherit: inherit),
				directly => $"or with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} "));

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		public MethodsWith OrWith<TAttribute>(
			Func<TAttribute, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
			=> new(_source, _filter.Or(
				(methodInfo, inherit) => methodInfo.HasAttribute(predicate, inherit),
				directly => $"or with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));

		/// <summary>
		///     Only considers the attributes declared directly on the method, not those inherited from a base type or an overridden member.
		/// </summary>
		/// <remarks>
		///     Applies to all attributes of the filter, also to those added via <see cref="OrWith{TAttribute}()" />.
		/// </remarks>
		public MethodsWith Directly()
			=> new(_source, _filter.Directly());
	}
}
