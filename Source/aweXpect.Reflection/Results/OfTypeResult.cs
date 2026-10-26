using System;
using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an expectation on the handler type of events or the type of fields or properties, allowing to
///     chain alternative types.
/// </summary>
public sealed class OfTypeResult<TType>
	: AndOrResult<TType, IThat<TType?>>
{
	private readonly TypeFilterOptions _typeFilterOptions;

	internal OfTypeResult(
		ExpectationBuilder expectationBuilder,
		IThat<TType?> subject,
		TypeFilterOptions typeFilterOptions)
		: base(expectationBuilder, subject)
	{
		_typeFilterOptions = typeFilterOptions;
	}

	/// <summary>
	///     Allow an alternative type <typeparamref name="T" /> (or a subtype).
	/// </summary>
	public OfTypeResult<TType> OrOfType<T>()
		=> OrOfType(typeof(T));

	/// <summary>
	///     Allow an alternative type <paramref name="type" /> (or a subtype).
	/// </summary>
	public OfTypeResult<TType> OrOfType(Type type)
	{
		_typeFilterOptions.AddType(type, false);
		return this;
	}

	/// <summary>
	///     Allow an alternative exact type <typeparamref name="T" />.
	/// </summary>
	public OfTypeResult<TType> OrOfExactType<T>()
		=> OrOfExactType(typeof(T));

	/// <summary>
	///     Allow an alternative exact type <paramref name="type" />.
	/// </summary>
	public OfTypeResult<TType> OrOfExactType(Type type)
	{
		_typeFilterOptions.AddType(type, true);
		return this;
	}
}
