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
		IChangeableFilter<FieldInfo> filter = Filter.Suffix<FieldInfo>(
			fieldInfo => fieldInfo.HasAttribute<TAttribute>(),
			$"with {Formatter.Format(typeof(TAttribute))} ");
		return new FieldsWith(@this.Which(filter), filter);
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
		IChangeableFilter<FieldInfo> filter = Filter.Suffix<FieldInfo>(
			fieldInfo => fieldInfo.HasAttribute(predicate),
			$"with {Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} ");
		return new FieldsWith(@this.Which(filter), filter);
	}

	/// <summary>
	///     Additional filters on fields with an attribute.
	/// </summary>
	public class FieldsWith(Filtered.Fields inner, IChangeableFilter<FieldInfo> filter) : Filtered.Fields(inner)
	{
		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		public FieldsWith OrWith<TAttribute>()
			where TAttribute : Attribute
		{
			filter.UpdateFilter((result, fieldInfo) => result || fieldInfo.HasAttribute<TAttribute>(),
				description
					=> $"{description}or with {Formatter.Format(typeof(TAttribute))} ");
			return this;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		public FieldsWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
		{
			filter.UpdateFilter(
				(result, fieldInfo) => result || fieldInfo.HasAttribute(predicate),
				description
					=> $"{description}or with {Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} ");
			return this;
		}
	}
}
