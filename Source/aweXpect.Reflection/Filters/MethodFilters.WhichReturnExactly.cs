using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods which return exact type <typeparamref name="TReturn" />.
	/// </summary>
	public static MethodsWhichReturn WhichReturnExactly<TReturn>(this Filtered.Methods @this)
		=> WhichReturnExactly(@this, typeof(TReturn));

	/// <summary>
	///     Filter for methods which return exact type <paramref name="returnType" />.
	/// </summary>
	public static MethodsWhichReturn WhichReturnExactly(this Filtered.Methods @this,
		Type returnType)
	{
		IChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.ReturnType.IsOrInheritsFrom(returnType, true),
			$"which return exactly {Formatter.Format(returnType)} ");
		return new MethodsWhichReturn(@this.Which(filter), filter);
	}

	public partial class MethodsWhichReturn
	{
		/// <summary>
		///     Allow an alternative exact return type <typeparamref name="TReturn" />.
		/// </summary>
		public MethodsWhichReturn OrReturnExactly<TReturn>()
			=> OrReturnExactly(typeof(TReturn));

		/// <summary>
		///     Allow an alternative exact return type <paramref name="returnType" />.
		/// </summary>
		public MethodsWhichReturn OrReturnExactly(Type returnType)
		{
			filter.UpdateFilter(
				(result, methodInfo)
					=> result || methodInfo.ReturnType.IsOrInheritsFrom(returnType, true),
				description
					=> $"{description}or return exactly {Formatter.Format(returnType)} ");
			return this;
		}
	}
}
