using System;
using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an expectation on the return types of a collection of methods, allowing to chain alternative
///     return types.
/// </summary>
public sealed class ReturnResult<TType>
	: AndOrResult<TType, IThat<TType?>>
{
	private readonly TypeFilterOptions _typeFilterOptions;

	internal ReturnResult(
		ExpectationBuilder expectationBuilder,
		IThat<TType?> subject,
		TypeFilterOptions typeFilterOptions)
		: base(expectationBuilder, subject)
	{
		_typeFilterOptions = typeFilterOptions;
	}

	/// <summary>
	///     Allow an alternative return type <typeparamref name="TReturn" />.
	/// </summary>
	public ReturnResult<TType> OrReturn<TReturn>()
		=> OrReturn(typeof(TReturn));

	/// <summary>
	///     Allow an alternative return type <paramref name="returnType" />.
	/// </summary>
	public ReturnResult<TType> OrReturn(Type returnType)
	{
		_typeFilterOptions.AddType(returnType, false);
		return this;
	}

	/// <summary>
	///     Allow an alternative exact return type <typeparamref name="TReturn" />.
	/// </summary>
	public ReturnResult<TType> OrReturnExactly<TReturn>()
		=> OrReturnExactly(typeof(TReturn));

	/// <summary>
	///     Allow an alternative exact return type <paramref name="returnType" />.
	/// </summary>
	public ReturnResult<TType> OrReturnExactly(Type returnType)
	{
		_typeFilterOptions.AddType(returnType, true);
		return this;
	}
}
