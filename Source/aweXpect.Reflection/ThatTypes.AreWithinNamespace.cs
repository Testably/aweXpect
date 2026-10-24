using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	[GenerateCollectionExpectation("AreWithinNamespace", "is within namespace {expected}",
		"are within namespace {expected}",
		CollectionOverload = true,
		Remarks = "Sub-namespaces of the <paramref name=\"expected\" /> namespace are included.")]
	private static PredicateResult IsWithinNamespaceType(Type? type, string expected)
		=> type is null ? ItemResults.Null : type.IsWithinNamespace(expected);

	/// <summary>
	///     Verifies that no items are within namespace <paramref name="unexpected" />.
	/// </summary>
	/// <remarks>
	///     Sub-namespaces of the <paramref name="unexpected" /> namespace are included.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreNotWithinNamespace(
		this IThat<IEnumerable<Type?>?> subject, string unexpected)
	{
		ThrowHelper.ThrowIfNull(unexpected, nameof(unexpected));
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>(subject.Get().ExpectationBuilder
				.AddConstraint(unexpected, static (s, it, grammars)
					=> new IsWithinNamespaceType_GeneratedItemsConstraint<IEnumerable<Type?>?>(it, grammars,
						EnumerableQuantifier.None(ExpectationGrammars.Nested), true, s)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that no items are within namespace <paramref name="unexpected" />.
	/// </summary>
	/// <remarks>
	///     Sub-namespaces of the <paramref name="unexpected" /> namespace are included.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreNotWithinNamespace(
		this IThat<IAsyncEnumerable<Type?>?> subject, string unexpected)
	{
		ThrowHelper.ThrowIfNull(unexpected, nameof(unexpected));
		return new AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>>(subject.Get()
				.ExpectationBuilder
				.AddConstraint(unexpected, static (s, it, grammars)
					=> new IsWithinNamespaceType_GeneratedAsyncItemsConstraint(it, grammars,
						EnumerableQuantifier.None(ExpectationGrammars.Nested), true, s)),
			subject);
	}
#endif
}
