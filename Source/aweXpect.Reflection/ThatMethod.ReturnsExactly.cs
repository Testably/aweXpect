using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the method returns exactly type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static MethodReturnResult<MethodInfo, IThat<MethodInfo?>> ReturnsExactly<TReturn>(
		this IThat<MethodInfo?> subject)
		=> ReturnsExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that the method returns exactly type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static MethodReturnResult<MethodInfo, IThat<MethodInfo?>> ReturnsExactly(
		this IThat<MethodInfo?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, true);
		return new MethodReturnResult<MethodInfo, IThat<MethodInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new ReturnsConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Verifies that the method does not return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<MethodInfo, IThat<MethodInfo?>> DoesNotReturnExactly<TReturn>(
		this IThat<MethodInfo?> subject)
		=> DoesNotReturnExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that the method does not return exactly type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<MethodInfo, IThat<MethodInfo?>> DoesNotReturnExactly(
		this IThat<MethodInfo?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, true);
		return new AndOrResult<MethodInfo, IThat<MethodInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new ReturnsConstraint(it, grammars, typeFilterOptions).Invert()),
			subject);
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
			_typeFilterOptions.RegisterType(returnType, true);
			return this;
		}
	}
}
