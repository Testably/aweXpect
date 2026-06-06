using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of a has-dependencies-outside assertion whose allowed targets are filtered collections of
///     types, allowing to widen the allowed collections and to opt out of the implicit allowance of the type's
///     own sub-namespaces.
/// </summary>
public sealed class TypeSetDependencyOutsideResult<TThat>
	: AndOrResult<TThat, IThat<TThat>>
{
	private readonly TypeSetDependencyOptions _options;

	internal TypeSetDependencyOutsideResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		TypeSetDependencyOptions options)
		: base(expectationBuilder, subject)
		=> _options = options;

	/// <summary>
	///     Widens the allowed set by the given <paramref name="targets" />, so that dependencies on their types no
	///     longer count as outside.
	/// </summary>
	public TypeSetDependencyOutsideResult<TThat> OrOn(params Filtered.Types[] targets)
	{
		_options.OrOn(targets);
		return this;
	}

	/// <summary>
	///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (so a <c>Foo</c> type
	///     referencing <c>Foo.Bar</c> has a dependency outside the allowed set unless <c>Foo.Bar</c> types are part
	///     of an allowed collection).
	/// </summary>
	/// <remarks>
	///     The type's own namespace itself never counts as outside.
	/// </remarks>
	public TypeSetDependencyOutsideResult<TThat> ExcludingOwnSubNamespaces()
	{
		_options.ExcludingOwnSubNamespaces();
		return this;
	}
}
