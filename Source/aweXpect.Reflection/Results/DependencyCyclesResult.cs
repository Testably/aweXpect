using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of a namespace dependency cycle assertion, allowing to opt out of treating a namespace and its
///     sub-namespaces as one family.
/// </summary>
public sealed class DependencyCyclesResult<TThat>
	: AndOrResult<TThat, IThat<TThat>>
{
	private readonly DependencyCyclesOptions _options;

	internal DependencyCyclesResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		DependencyCyclesOptions options)
		: base(expectationBuilder, subject)
	{
		_options = options;
	}

	/// <summary>
	///     Treats every namespace as its own node, so that a reference between a namespace and one of its sub-namespaces
	///     also creates an edge (and can form a cycle).
	/// </summary>
	/// <remarks>
	///     Without this call, a namespace and its sub-namespaces are treated as one family, so references within a family
	///     (a node and its ancestor or descendant) never create an edge — only references between unrelated namespaces do.
	/// </remarks>
	public DependencyCyclesResult<TThat> ExcludingSubNamespaces()
	{
		_options.ExcludingSubNamespaces();
		return this;
	}
}
