using System;
using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an expectation on the return type of a single method, allowing to chain alternative return types.
/// </summary>
public sealed class ReturnsResult<TType>
	: AndOrResult<TType, IThat<TType?>>,
		IOptionsProvider<TypeFilterOptions>
{
	private readonly TypeFilterOptions _typeFilterOptions;

	internal ReturnsResult(
		ExpectationBuilder expectationBuilder,
		IThat<TType?> subject,
		TypeFilterOptions typeFilterOptions)
		: base(expectationBuilder, subject)
	{
		_typeFilterOptions = typeFilterOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TypeFilterOptions}.Options" />
	/// <remarks>
	///     A new slot around the options that the filter keeps itself.
	/// </remarks>
	OptionsSlot<TypeFilterOptions> IOptionsProvider<TypeFilterOptions>.Options
		=> new(_typeFilterOptions);

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
		_typeFilterOptions.RegisterType(returnType, false);
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
		_typeFilterOptions.RegisterType(returnType, true);
		return this;
	}
}
