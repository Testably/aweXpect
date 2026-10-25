#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;
using StringEqualityOptions = aweXpect.Options.StringEqualityOptions;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     …are unique, i.e. they occur exactly once in the collection.
	/// </summary>
	public static StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreUnique(
		this IAsyncEnumerableItems<string?> items)
		=> AreUniqueCore(items, true);

	/// <summary>
	///     …are unique, i.e. they occur exactly once in the collection.
	/// </summary>
	public static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		AreUnique<TItem>(
			this IAsyncEnumerableItems<TItem> items)
		=> AreUniqueCore(items, true);

	/// <summary>
	///     …have unique members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TMember>
		AreUnique<TItem, TMember>(this IAsyncEnumerableItems<TItem> items,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, true);

	/// <summary>
	///     …have unique members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static StringEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>> AreUnique<TItem>(
		this IAsyncEnumerableItems<TItem> items,
		Func<TItem, string> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, true);

	/// <summary>
	///     …are not unique, i.e. they occur more than once in the collection.
	/// </summary>
	public static StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreNotUnique(
		this IAsyncEnumerableItems<string?> items)
		=> AreUniqueCore(items, false);

	/// <summary>
	///     …are not unique, i.e. they occur more than once in the collection.
	/// </summary>
	public static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		AreNotUnique<TItem>(
			this IAsyncEnumerableItems<TItem> items)
		=> AreUniqueCore(items, false);

	/// <summary>
	///     …have duplicate members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TMember>
		AreNotUnique<TItem, TMember>(this IAsyncEnumerableItems<TItem> items,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, false);

	/// <summary>
	///     …have duplicate members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static StringEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>> AreNotUnique<TItem>(
		this IAsyncEnumerableItems<TItem> items,
		Func<TItem, string> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, false);

	private static StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreUniqueCore(
		IAsyncEnumerableItems<string?> items,
		bool expectUnique)
	{
		OptionsSlot<StringEqualityOptions> options = new(new StringEqualityOptions("expected"));
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique, Options: options),
				static (state, it, grammars)
					=> new AsyncAreUniqueConstraint<string?, string?>(
						it, grammars,
						state.Quantifier,
						a => a,
						null,
						state.Options.Value,
						state.ExpectUnique)),
			items.Subject,
			options);
	}

	private static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		AreUniqueCore<TItem>(
			IAsyncEnumerableItems<TItem> items,
			bool expectUnique)
	{
		OptionsSlot<Options.ObjectEqualityOptions<TItem>> options = new(new ItemEqualityOptions<TItem>());
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique, Options: options),
				static (state, it, grammars)
					=> new AsyncAreUniqueConstraint<TItem, TItem>(
						it, grammars,
						state.Quantifier,
						a => a,
						null,
						state.Options.Value,
						state.ExpectUnique)),
			items.Subject,
			options);
	}

	private static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TMember>
		AreUniqueCore<TItem, TMember>(IAsyncEnumerableItems<TItem> items,
			Func<TItem, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
	{
		memberAccessor.ThrowIfNull();
		OptionsSlot<Options.ObjectEqualityOptions<TMember>> options = new(new ItemEqualityOptions<TMember>());
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TMember>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique,
					MemberAccessorExpression: memberAccessorExpression, Options: options,
					MemberAccessor: memberAccessor),
				static (state, it, grammars)
					=> new AsyncAreUniqueConstraint<TItem, TMember>(
						it, grammars,
						state.Quantifier,
						state.MemberAccessor,
						state.MemberAccessorExpression,
						state.Options.Value,
						state.ExpectUnique)),
			items.Subject,
			options);
	}

	private static StringEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>> AreUniqueCore<TItem>(
		IAsyncEnumerableItems<TItem> items,
		Func<TItem, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
	{
		memberAccessor.ThrowIfNull();
		OptionsSlot<StringEqualityOptions> options = new(new StringEqualityOptions("expected"));
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique,
					MemberAccessorExpression: memberAccessorExpression, Options: options,
					MemberAccessor: memberAccessor),
				static (state, it, grammars)
					=> new AsyncAreUniqueConstraint<TItem, string>(
						it, grammars,
						state.Quantifier,
						state.MemberAccessor,
						state.MemberAccessorExpression,
						state.Options.Value,
						state.ExpectUnique)),
			items.Subject,
			options);
	}
}
#endif
