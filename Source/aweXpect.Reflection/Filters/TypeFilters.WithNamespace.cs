using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types with the <paramref name="expected" /> namespace.
	/// </summary>
	public static Filtered.Types.StringEqualityResult WithNamespace(this Filtered.Types @this, string expected)
	{
		return new Filtered.Types.StringEqualityResult(@this, options => Filter.Suffix<Type>(
				type => options.Value.AreConsideredEqual(type.Namespace, expected, nameof(expected)),
				() => $"with namespace {options.Value.GetExpectation(expected, ExpectationGrammars.None)} "),
			new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default));
	}

	/// <summary>
	///     Filter for types without the <paramref name="unexpected" /> namespace.
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="WhichAreNotWithinNamespace(Filtered.Types, string)" />, this compares the whole namespace as a
	///     string with the string options (e.g. <c>AsPrefix()</c> or <c>IgnoringCase()</c>), instead of excluding a
	///     namespace and its sub-namespaces.
	/// </remarks>
	public static Filtered.Types.StringEqualityResult WithoutNamespace(this Filtered.Types @this, string unexpected)
	{
		return new Filtered.Types.StringEqualityResult(@this, options => Filter.Suffix<Type>(
				async type => !await options.Value.AreConsideredEqual(type.Namespace, unexpected, nameof(unexpected)),
				() => $"without namespace {options.Value.GetExpectation(unexpected, ExpectationGrammars.None)} "),
			new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default));
	}
}
