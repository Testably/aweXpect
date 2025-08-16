using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the method returns exactly type <typeparamref name="TReturn" />.
	/// </summary>
	public static MethodReturnResult<MethodInfo?, IThat<MethodInfo?>> ReturnsExactly<TReturn>(
		this IThat<MethodInfo?> subject)
		=> ReturnsExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that the method returns exactly type <paramref name="returnType" />.
	/// </summary>
	public static MethodReturnResult<MethodInfo?, IThat<MethodInfo?>> ReturnsExactly(
		this IThat<MethodInfo?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, true);
		return new MethodReturnResult<MethodInfo?, IThat<MethodInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new ReturnsConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	public sealed partial class MethodReturnResult<TValue, TResult>
	{
		/// <summary>
		///     Allow an alternative exact return type <typeparamref name="TReturn" />.
		/// </summary>
		public MethodReturnResult<TValue, TResult> OrReturnsExactly<TReturn>()
			=> OrReturnsExactly(typeof(TReturn));

		/// <summary>
		///     Allow an alternative exact return type <paramref name="returnType" />.
		/// </summary>
		public MethodReturnResult<TValue, TResult> OrReturnsExactly(Type returnType)
		{
			typeFilterOptions.RegisterType(returnType, true);
			return this;
		}
	}
}
