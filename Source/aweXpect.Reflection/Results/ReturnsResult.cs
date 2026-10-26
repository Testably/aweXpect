using System;
using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an expectation on the return type of a single method, allowing to chain alternative return types.
/// </summary>
public sealed class ReturnsResult<TType>
	: AndOrResult<TType, IThat<TType?>>
{
	private readonly OptionsHolder<TypeFilterOptions> _typeFilterOptions;

	internal ReturnsResult(
		ExpectationBuilder expectationBuilder,
		IThat<TType?> subject,
		OptionsHolder<TypeFilterOptions> typeFilterOptions)
		: base(expectationBuilder, subject)
	{
		_typeFilterOptions = typeFilterOptions;
	}

	/// <summary>
	///     Allow an alternative return type <typeparamref name="TReturn" />.
	/// </summary>
	public ReturnsResult<TType> OrReturns<TReturn>()
		=> OrReturns(typeof(TReturn));

	/// <summary>
	///     Allow an alternative return type <paramref name="returnType" />.
	/// </summary>
	public ReturnsResult<TType> OrReturns(Type returnType)
	{
		_typeFilterOptions.Value = _typeFilterOptions.Value.WithType(returnType, false);
		return this;
	}

	/// <summary>
	///     Allow an alternative exact return type <typeparamref name="TReturn" />.
	/// </summary>
	public ReturnsResult<TType> OrReturnsExactly<TReturn>()
		=> OrReturnsExactly(typeof(TReturn));

	/// <summary>
	///     Allow an alternative exact return type <paramref name="returnType" />.
	/// </summary>
	public ReturnsResult<TType> OrReturnsExactly(Type returnType)
	{
		_typeFilterOptions.Value = _typeFilterOptions.Value.WithType(returnType, true);
		return this;
	}
}
