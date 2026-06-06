using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of a namespace-based depends-only-on assertion, allowing to widen the allowed namespaces
///     and to opt out of sub-namespace matching — for the targeted namespaces and for the type's own namespace.
/// </summary>
public sealed class NamespaceDependencyOnlyOnResult<TThat>
	: AndOrResult<TThat, IThat<TThat>>
{
	private readonly NamespaceDependencyOptions _options;

	internal NamespaceDependencyOnlyOnResult(
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
	public NamespaceDependencyOnlyOnResult<TThat> OrOn(params IEnumerable<string> namespaces)
	{
		_options.OrOn(namespaces);
		return this;
	}

	/// <summary>
	///     Excludes sub-namespaces of the allowed namespaces from matching for the whole expression (including any
	///     <see cref="OrOn" /> additions).
	/// </summary>
	/// <remarks>
	///     Without this call, a namespace matches itself and all its sub-namespaces (so <c>Foo.Bar</c> includes
	///     <c>Foo.Bar.Baz</c> but not <c>Foo.BarBaz</c>).
	///     <para />
	///     The type's own namespace is always allowed, and its sub-namespaces stay allowed unless
	///     <see cref="ExcludingOwnSubNamespaces" /> is also used.
	/// </remarks>
	public NamespaceDependencyOnlyOnResult<TThat> ExcludingSubNamespaces()
	{
		_options.ExcludingSubNamespaces();
		return this;
	}

	/// <summary>
	///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (so a <c>Foo</c> type
	///     referencing <c>Foo.Bar</c> becomes a violation unless <c>Foo.Bar</c> is explicitly allowed).
	/// </summary>
	/// <remarks>
	///     The type's own namespace itself is always allowed.
	/// </remarks>
	public NamespaceDependencyOnlyOnResult<TThat> ExcludingOwnSubNamespaces()
	{
		_options.ExcludingOwnSubNamespaces();
		return this;
	}
}
