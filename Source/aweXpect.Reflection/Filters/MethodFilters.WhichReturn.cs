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
			() => $"which return {Formatter.Format(returnType)} ");
		return new MethodsWhichReturn(@this.Which(filter), filter);
	}

	/// <summary>
	///     Filter for methods which do not return type <typeparamref name="TReturn" />.
	/// </summary>
	public static Filtered.Methods WhichDoNotReturn<TReturn>(this Filtered.Methods @this)
		=> WhichDoNotReturn(@this, typeof(TReturn));

	/// <summary>
	///     Filter for methods which do not return type <paramref name="returnType" />.
	/// </summary>
	public static Filtered.Methods WhichDoNotReturn(this Filtered.Methods @this,
		Type returnType)
		=> @this.Which(Filter.Suffix<MethodInfo>(
			methodInfo => !methodInfo.ReturnType.IsOrInheritsFrom(returnType),
			() => $"which do not return {Formatter.Format(returnType)} "));

	/// <summary>
	///     Additional filters on methods which return a specific type.
	/// </summary>
	public partial class MethodsWhichReturn : Filtered.Methods
	{
		private readonly IChangeableFilter<MethodInfo> _filter;

		internal MethodsWhichReturn(Filtered.Methods inner, IChangeableFilter<MethodInfo> filter) : base(inner)
		{
			_filter = filter;
		}

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
			_filter.UpdateFilter(
				(result, methodInfo)
					=> result || methodInfo.ReturnType.IsOrInheritsFrom(returnType),
				description
					=> $"{description}or return {Formatter.Format(returnType)} ");
			return this;
		}
	}
}
