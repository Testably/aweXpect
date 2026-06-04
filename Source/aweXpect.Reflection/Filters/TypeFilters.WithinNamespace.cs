using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types within the <paramref name="expected" /> namespace (including sub-namespaces).
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="WithNamespace(Filtered.Types, string)" /> with <c>AsPrefix()</c>, this matches the
	///     <paramref name="expected" /> namespace and its sub-namespaces, but not namespaces that merely start with the same
	///     text (e.g. <c>Foo.Bar</c> does not include <c>Foo.BarBaz</c>). The comparison is exact and case-sensitive.
	/// </remarks>
	public static Filtered.Types WithinNamespace(this Filtered.Types @this, string expected)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.IsWithinNamespace(expected),
			$"within namespace {Formatter.Format(expected)} "));

	/// <summary>
	///     Filter for types not within the <paramref name="expected" /> namespace (including sub-namespaces).
	/// </summary>
	/// <remarks>
	///     This is the negation of <see cref="WithinNamespace(Filtered.Types, string)" />: it matches all types that are
	///     neither in the <paramref name="expected" /> namespace nor in one of its sub-namespaces. The comparison is exact
	///     and case-sensitive.
	/// </remarks>
	public static Filtered.Types NotWithinNamespace(this Filtered.Types @this, string expected)
		=> @this.Which(Filter.Suffix<Type>(
			type => !type.IsWithinNamespace(expected),
			$"not within namespace {Formatter.Format(expected)} "));
}
