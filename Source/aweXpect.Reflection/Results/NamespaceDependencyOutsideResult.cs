using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of a namespace-based has-dependencies-outside assertion, allowing to widen the allowed
///     namespaces and to opt out of sub-namespace matching — for the allowed namespaces and for the type's own
///     namespace.
/// </summary>
public sealed class NamespaceDependencyOutsideResult<TThat>
	: AndOrResult<TThat, IThat<TThat>>
{
	private readonly NamespaceDependencyOptions _options;

	internal NamespaceDependencyOutsideResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		NamespaceDependencyOptions options)
		: base(expectationBuilder, subject)
	{
		_options = options;
	}

	/// <summary>
	///     Widens the allowed set by the given <paramref name="namespaces" /> (including sub-namespaces unless
	///     <see cref="ExcludingSubNamespaces" /> is used), so that dependencies on them no longer count as outside.
	/// </summary>
	public NamespaceDependencyOutsideResult<TThat> OrOn(params IEnumerable<string> namespaces)
	{
		_options.OrOn(namespaces);
		return this;
	}

	/// <summary>
	///     Excludes sub-namespaces of the allowed namespaces from matching for the whole expression (including any
	///     <see cref="OrOn" /> additions), so that dependencies on them count as outside.
	/// </summary>
	/// <remarks>
	///     Without this call, a namespace matches itself and all its sub-namespaces (so <c>Foo.Bar</c> includes
	///     <c>Foo.Bar.Baz</c> but not <c>Foo.BarBaz</c>).
	///     <para />
	///     The type's own namespace never counts as outside, and neither do its sub-namespaces unless
	///     <see cref="ExcludingOwnSubNamespaces" /> is also used.
	/// </remarks>
	public NamespaceDependencyOutsideResult<TThat> ExcludingSubNamespaces()
	{
		_options.ExcludingSubNamespaces();
		return this;
	}

	/// <summary>
	///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (so a <c>Foo</c> type
	///     referencing <c>Foo.Bar</c> has a dependency outside the allowed set unless <c>Foo.Bar</c> is explicitly
	///     allowed).
	/// </summary>
	/// <remarks>
	///     The type's own namespace itself never counts as outside.
	/// </remarks>
	public NamespaceDependencyOutsideResult<TThat> ExcludingOwnSubNamespaces()
	{
		_options.ExcludingOwnSubNamespaces();
		return this;
	}
}
