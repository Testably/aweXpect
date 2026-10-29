using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAssemblies
{
	[GenerateCollectionExpectation("HaveName", "has name {expected}", "have name {expected}",
		CollectionOverload = true, NegatedName = "DoNotHaveName", NullItemFailsBothWays = true)]
	private static ValueTask<bool> HasName(Assembly assembly, [NegatedParameterName("unexpected")] string expected,
		[ExpectationOptions] StringEqualityOptions options)
		=> options.AreConsideredEqual(assembly.GetName().Name, expected, nameof(expected));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have the name
	///     returned by the <paramref name="expectedNameSelector" /> for the respective item.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> HaveName(
		this IThat<IEnumerable<Assembly?>?> subject,
		Func<Assembly, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
	{
		HaveNameFromSelectorExpectation<Assembly> expectation =
			new(expectedNameSelector, doNotPopulateThisValue, static assembly => assembly.GetName().Name);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …have the name returned by the <paramref name="expectedNameSelector" /> for the respective item.
	/// </summary>
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> HaveName(
		this IEnumerableItems<Assembly?> items,
		Func<Assembly, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
	{
		HaveNameFromSelectorExpectation<Assembly> expectation =
			new(expectedNameSelector, doNotPopulateThisValue, static assembly => assembly.GetName().Name);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="HaveName(IEnumerableItems{Assembly?}, Func{Assembly, string}, string)" />
	public static StringEqualityTypeResult<TCollection, IThat<TCollection>> HaveName<TCollection>(
		this IStructEnumerableItems<TCollection, Assembly?> items,
		Func<Assembly, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
		where TCollection : struct, IEnumerable<Assembly?>
	{
		HaveNameFromSelectorExpectation<Assembly> expectation =
			new(expectedNameSelector, doNotPopulateThisValue, static assembly => assembly.GetName().Name);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveName(IThat{IEnumerable{Assembly?}}, Func{Assembly, string}, string)" />
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> HaveName(
		this IThat<IAsyncEnumerable<Assembly?>?> subject,
		Func<Assembly, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
	{
		HaveNameFromSelectorExpectation<Assembly> expectation =
			new(expectedNameSelector, doNotPopulateThisValue, static assembly => assembly.GetName().Name);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="HaveName(IEnumerableItems{Assembly?}, Func{Assembly, string}, string)" />
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> HaveName(
		this IAsyncEnumerableItems<Assembly?> items,
		Func<Assembly, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
	{
		HaveNameFromSelectorExpectation<Assembly> expectation =
			new(expectedNameSelector, doNotPopulateThisValue, static assembly => assembly.GetName().Name);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif
}
