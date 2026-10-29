using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("HaveName", "has name {expected}", "have name {expected}",
		CollectionOverload = true, NegatedName = "DoNotHaveName", FormattedItem = nameof(ToFormattable),
		NullItemFailsBothWays = true)]
	private static ValueTask<bool> HasName<TMember>(TMember member, [NegatedParameterName("unexpected")] string expected,
		[ExpectationOptions] StringEqualityOptions options)
		where TMember : MemberInfo
		=> options.AreConsideredEqual(member.Name, expected, nameof(expected));

	private static object? ToFormattable(MemberInfo? member) => FormattableMember.From(member);

	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> have the name
	///     returned by the <paramref name="expectedNameSelector" /> for the respective item.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<TMember?>, IThat<IEnumerable<TMember?>?>> HaveName<TMember>(
		this IThat<IEnumerable<TMember?>?> subject,
		Func<TMember, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
		where TMember : MemberInfo
	{
		HaveNameFromSelectorExpectation<TMember> expectation =
			new(expectedNameSelector, doNotPopulateThisValue, static member => member.Name);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …have the name returned by the <paramref name="expectedNameSelector" /> for the respective item.
	/// </summary>
	public static StringEqualityTypeResult<IEnumerable<TMember?>, IThat<IEnumerable<TMember?>?>> HaveName<TMember>(
		this IEnumerableItems<TMember?> items,
		Func<TMember, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
		where TMember : MemberInfo
	{
		HaveNameFromSelectorExpectation<TMember> expectation =
			new(expectedNameSelector, doNotPopulateThisValue, static member => member.Name);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="HaveName{TMember}(IEnumerableItems{TMember}, Func{TMember, string}, string)" />
	public static StringEqualityTypeResult<TCollection, IThat<TCollection>> HaveName<TCollection, TMember>(
		this IStructEnumerableItems<TCollection, TMember?> items,
		Func<TMember, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
		where TCollection : struct, IEnumerable<TMember?>
		where TMember : MemberInfo
	{
		HaveNameFromSelectorExpectation<TMember> expectation =
			new(expectedNameSelector, doNotPopulateThisValue, static member => member.Name);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveName{TMember}(IThat{IEnumerable{TMember}}, Func{TMember, string}, string)" />
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<TMember?>, IThat<IAsyncEnumerable<TMember?>?>>
		HaveName<TMember>(
			this IThat<IAsyncEnumerable<TMember?>?> subject,
			Func<TMember, string> expectedNameSelector,
			[CallerArgumentExpression(nameof(expectedNameSelector))]
			string doNotPopulateThisValue = "")
		where TMember : MemberInfo
	{
		HaveNameFromSelectorExpectation<TMember> expectation =
			new(expectedNameSelector, doNotPopulateThisValue, static member => member.Name);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="HaveName{TMember}(IEnumerableItems{TMember}, Func{TMember, string}, string)" />
	public static StringEqualityTypeResult<IAsyncEnumerable<TMember?>, IThat<IAsyncEnumerable<TMember?>?>>
		HaveName<TMember>(
			this IAsyncEnumerableItems<TMember?> items,
			Func<TMember, string> expectedNameSelector,
			[CallerArgumentExpression(nameof(expectedNameSelector))]
			string doNotPopulateThisValue = "")
		where TMember : MemberInfo
	{
		HaveNameFromSelectorExpectation<TMember> expectation =
			new(expectedNameSelector, doNotPopulateThisValue, static member => member.Name);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif
}
