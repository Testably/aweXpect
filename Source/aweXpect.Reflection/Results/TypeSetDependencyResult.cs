using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of a dependency assertion whose targets are filtered collections of types, allowing to widen
///     the targeted/allowed collections.
/// </summary>
public sealed class TypeSetDependencyResult<TType, TValue>
	: AndOrResult<TType, IThat<TValue>, TypeSetDependencyResult<TType, TValue>>
{
	private readonly OptionsHolder<TypeSetDependencyOptions> _options;

	internal TypeSetDependencyResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsHolder<TypeSetDependencyOptions> options)
		: base(expectationBuilder, subject)
	{
		_options = options;
	}

	/// <summary>
	///     Widens the expression by the given <paramref name="targets" />.
	/// </summary>
	public TypeSetDependencyResult<TType, TValue> OrOn(params Filtered.Types[] targets)
	{
		_options.Value = _options.Value.OrOn(targets);
		return this;
	}
}
