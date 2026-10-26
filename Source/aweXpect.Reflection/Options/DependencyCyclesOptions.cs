namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for a namespace dependency cycle assertion.
/// </summary>
/// <remarks>
///     An immutable value: <see cref="ExcludingSubNamespaces" /> returns new options. The expectation shares them in an
///     <see cref="Core.OptionsHolder{TOptions}" /> with its result, which replaces them, and its constraint, which reads
///     them when it evaluates.
/// </remarks>
internal sealed class DependencyCyclesOptions(string? sliceRoot, bool excludeSubNamespaces = false)
{
	/// <summary>
	///     The slice root below which namespaces are grouped into one slice each, or <see langword="null" /> when every
	///     distinct namespace is its own node.
	/// </summary>
	public string? SliceRoot { get; } = sliceRoot;

	/// <summary>
	///     Indicates whether each namespace forms its own node, so that a reference between a namespace and one of its
	///     sub-namespaces also creates an edge (and can form a cycle).
	/// </summary>
	/// <remarks>
	///     By default a namespace and its sub-namespaces are treated as one family, so references within a family never
	///     create an edge — consistent with the own-sub-namespace allowance of the other dependency assertions.
	/// </remarks>
	public bool ExcludeSubNamespaces { get; } = excludeSubNamespaces;

	/// <summary>
	///     Returns options that treat every namespace as its own node, so that a reference between a namespace and one of
	///     its sub-namespaces also creates an edge.
	/// </summary>
	public DependencyCyclesOptions ExcludingSubNamespaces()
		=> new(SliceRoot, true);
}
