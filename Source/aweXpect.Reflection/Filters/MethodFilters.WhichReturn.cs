using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods which return type <typeparamref name="TReturn" />.
	/// </summary>
	public static MethodsWhichReturn WhichReturn<TReturn>(this Filtered.Methods @this)
		=> WhichReturn(@this, typeof(TReturn));

	/// <summary>
	///     Filter for methods which return type <paramref name="returnType" />.
	/// </summary>
	public static MethodsWhichReturn WhichReturn(this Filtered.Methods @this,
		Type returnType)
	{
		IChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.ReturnType.IsOrInheritsFrom(returnType),
			$"which return {Formatter.Format(returnType)} ");
		return new MethodsWhichReturn(@this.Which(filter), filter);
	}

	/// <summary>
	///     Additional filters on methods which return a specific type.
	/// </summary>
	public class MethodsWhichReturn(Filtered.Methods inner, IChangeableFilter<MethodInfo> filter)
		: Filtered.Methods(inner)
	{
		/// <summary>
		///     Allow an alternative return type <typeparamref name="TReturn" />.
		/// </summary>
		public MethodsWhichReturn OrReturn<TReturn>()
			=> OrReturn(typeof(TReturn));

		/// <summary>
		///     Allow an alternative return type <paramref name="returnType" />.
		/// </summary>
		public MethodsWhichReturn OrReturn(Type returnType)
		{
			filter.UpdateFilter(
				(result, methodInfo)
					=> result || methodInfo.ReturnType.IsOrInheritsFrom(returnType),
				description
					=> $"{description}or return {Formatter.Format(returnType)} ");
			return this;
		}
	}
}
