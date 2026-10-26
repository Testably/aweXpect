using System;
using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of a specific-type dependency assertion, allowing to widen the targeted types.
/// </summary>
public sealed class TypeDependencyResult<TType, TValue>
	: AndOrResult<TType, IThat<TValue>, TypeDependencyResult<TType, TValue>>
{
	private readonly OptionsHolder<TypeDependencyOptions> _options;

	internal TypeDependencyResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsHolder<TypeDependencyOptions> options)
		: base(expectationBuilder, subject)
	{
		_options = options;
	}

	/// <summary>
	///     Widens the expression by the type <typeparamref name="T" />.
	/// </summary>
	public TypeDependencyResult<TType, TValue> OrOn<T>()
	{
		_options.Value = _options.Value.OrOn(typeof(T));
		return this;
	}

	/// <summary>
	///     Widens the expression by the <paramref name="type" />.
	/// </summary>
	public TypeDependencyResult<TType, TValue> OrOn(Type type)
	{
		_options.Value = _options.Value.OrOn(type);
		return this;
	}
}
