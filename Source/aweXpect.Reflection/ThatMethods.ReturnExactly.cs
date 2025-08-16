using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all methods in the filtered collection return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	public static MethodsReturnResult<IEnumerable<MethodInfo>, IThat<IEnumerable<MethodInfo>>> ReturnExactly<TReturn>(
		this IThat<IEnumerable<MethodInfo>> subject)
		=> ReturnExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that all methods in the filtered collection return exactly type <paramref name="returnType" />.
	/// </summary>
	public static MethodsReturnResult<IEnumerable<MethodInfo>, IThat<IEnumerable<MethodInfo>>> ReturnExactly(
		this IThat<IEnumerable<MethodInfo>> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, true);
		return new MethodsReturnResult<IEnumerable<MethodInfo>, IThat<IEnumerable<MethodInfo>>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new ReturnConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	public sealed partial class MethodsReturnResult<TValue, TResult>
	{
		/// <summary>
		///     Allow an alternative exact return type <typeparamref name="TReturn" />.
		/// </summary>
		public MethodsReturnResult<TValue, TResult> OrReturnExactly<TReturn>()
			=> OrReturnExactly(typeof(TReturn));

		/// <summary>
		///     Allow an alternative exact return type <paramref name="returnType" />.
		/// </summary>
		public MethodsReturnResult<TValue, TResult> OrReturnExactly(Type returnType)
		{
			typeFilterOptions.RegisterType(returnType, true);
			return this;
		}
	}
}
