using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Filter for fields with attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static FieldsWith With<TAttribute>(this Filtered.Fields @this)
		where TAttribute : Attribute
	{
		Filter.GenericFilter<FieldInfo> filter = Filter.Suffix<FieldInfo>(
			fieldInfo => fieldInfo.HasAttribute<TAttribute>(),
			() => $"with {Formatter.Format(typeof(TAttribute))} ");
		return new FieldsWith(@this, filter);
	}

	/// <summary>
	///     Filter for fields with attribute of type <typeparamref name="TAttribute" /> that
	///     match the <paramref name="predicate" />.
	/// </summary>
	public static FieldsWith With<TAttribute>(this Filtered.Fields @this,
		Func<TAttribute, bool>? predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		Filter.GenericFilter<FieldInfo> filter = Filter.Suffix<FieldInfo>(
			fieldInfo => fieldInfo.HasAttribute(predicate),
			() => $"with {Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} ");
		return new FieldsWith(@this, filter);
	}

	/// <summary>
	///     Additional filters on fields with an attribute.
	/// </summary>
	public class FieldsWith : Filtered.Fields
	{
		private readonly Filter.GenericFilter<FieldInfo> _filter;
		private readonly Filtered.Fields _source;

		internal FieldsWith(Filtered.Fields source, Filter.GenericFilter<FieldInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		public FieldsWith OrWith<TAttribute>()
			where TAttribute : Attribute
			=> new(_source, _filter.Extend((result, fieldInfo) => result || fieldInfo.HasAttribute<TAttribute>(),
				description
					=> $"{description}or with {Formatter.Format(typeof(TAttribute))} "));

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		public FieldsWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
			=> new(_source, _filter.Extend(
				(result, fieldInfo) => result || fieldInfo.HasAttribute(predicate),
				description
					=> $"{description}or with {Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));
	}
}
