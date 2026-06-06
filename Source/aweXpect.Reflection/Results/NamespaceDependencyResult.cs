using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of a namespace-based type dependency assertion, allowing to widen the targeted/allowed namespaces
///     and to opt out of sub-namespace matching.
/// </summary>
public sealed class NamespaceDependencyResult<TThat>
	: AndOrResult<TThat, IThat<TThat>>
{
	private readonly NamespaceDependencyOptions _options;

	internal NamespaceDependencyResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		NamespaceDependencyOptions options)
		: base(expectationBuilder, subject)
	{
		_options = options;
	}

	/// <summary>
	///     Widens the expression by the given <paramref name="namespaces" /> (including sub-namespaces unless
	///     <see cref="ExcludingSubNamespaces" /> is used).
	/// </summary>
	public NamespaceDependencyResult<TThat> OrOn(params IEnumerable<string> namespaces)
	{
		_options.OrOn(namespaces);
		return this;
	}

	/// <summary>
	///     Excludes sub-namespaces from matching for the whole expression (including any <see cref="OrOn" /> additions).
	/// </summary>
	/// <remarks>
	///     Without this call, a namespace matches itself and all its sub-namespaces (so <c>Foo.Bar</c> includes
	///     <c>Foo.Bar.Baz</c> but not <c>Foo.BarBaz</c>).
	/// </remarks>
	public NamespaceDependencyResult<TThat> ExcludingSubNamespaces()
	{
		_options.ExcludingSubNamespaces();
		return this;
	}
}
