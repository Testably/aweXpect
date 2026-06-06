using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of a dependency assertion whose targets are filtered collections of types, allowing to widen
///     the targeted/allowed collections.
/// </summary>
public sealed class TypeSetDependencyResult<TThat>
	: AndOrResult<TThat, IThat<TThat>>
{
	private readonly TypeSetDependencyOptions _options;

	internal TypeSetDependencyResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		TypeSetDependencyOptions options)
		: base(expectationBuilder, subject)
		=> _options = options;

	/// <summary>
	///     Widens the expression by the given <paramref name="targets" />.
	/// </summary>
	public TypeSetDependencyResult<TThat> OrOn(params Filtered.Types[] targets)
	{
		_options.OrOn(targets);
		return this;
	}
}
