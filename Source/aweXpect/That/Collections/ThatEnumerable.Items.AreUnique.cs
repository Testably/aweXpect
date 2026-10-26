using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;
using StringEqualityOptions = aweXpect.Options.StringEqualityOptions;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     …are unique, i.e. they occur exactly once in the collection.
	/// </summary>
	/// <remarks>
	///     A set with a custom comparer never holds two items that its comparer considers equal, so its items count as
	///     unique without being compared, unless the comparison is changed, e.g. with <c>Using(…)</c>.
	/// </remarks>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreUnique(
		this IEnumerableItems<string?> items)
		=> AreUniqueCore(items, true);

	/// <summary>
	///     …are unique, i.e. they occur exactly once in the collection.
	/// </summary>
	/// <remarks>
	///     A set with a custom comparer never holds two items that its comparer considers equal, so its items count as
	///     unique without being compared, unless the comparison is changed, e.g. with <c>Using(…)</c>.
	/// </remarks>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem> AreUnique<TItem>(
		this IEnumerableItems<TItem> items)
		=> AreUniqueCore(items, true);

	/// <summary>
	///     …have unique members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember>
		AreUnique<TItem, TMember>(
			this IEnumerableItems<TItem> items,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, true);

	/// <summary>
	///     …have unique members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreUnique<TItem>(
		this IEnumerableItems<TItem> items,
		Func<TItem, string?> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, true);

	/// <summary>
	///     …are unique, i.e. they occur exactly once in the collection.
	/// </summary>
	/// <remarks>
	///     A set of <see langword="object" />s with a custom comparer never holds two items that its comparer considers
	///     equal, so its items count as unique without being compared, unless the comparison is changed, e.g. with
	///     <c>Using(…)</c>. The comparer of a set of another item type cannot be read without reflection and is not used.
	/// </remarks>
	public static ObjectEqualityResult<TCollection, IThat<TCollection?>, object?> AreUnique<TCollection>(
		this INonGenericEnumerableItems<TCollection> items)
		where TCollection : IEnumerable?
		=> AreUniqueCore(items, true);

	/// <summary>
	///     …have unique members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<TCollection, IThat<TCollection?>, TMember> AreUnique<TCollection, TMember>(
		this INonGenericEnumerableItems<TCollection> items,
		Func<object?, TMember> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		where TCollection : IEnumerable?
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, true);

	/// <summary>
	///     …have unique members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static StringEqualityResult<TCollection, IThat<TCollection?>> AreUnique<TCollection>(
		this INonGenericEnumerableItems<TCollection> items,
		Func<object?, string?> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		where TCollection : IEnumerable?
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, true);

	/// <summary>
	///     …are unique, i.e. they occur exactly once in the collection.
	/// </summary>
	public static ObjectEqualityResult<TCollection, IThat<TCollection>, TItem> AreUnique<TCollection, TItem>(
		this IStructEnumerableItems<TCollection, TItem> items)
		where TCollection : struct, IEnumerable<TItem>
		=> AreUniqueCore(items, true);

	/// <summary>
	///     …have unique members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<TCollection, IThat<TCollection>, TMember> AreUnique<TCollection, TItem, TMember>(
		this IStructEnumerableItems<TCollection, TItem> items,
		Func<TItem, TMember> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		where TCollection : struct, IEnumerable<TItem>
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, true);

	/// <summary>
	///     …have unique members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static StringEqualityResult<TCollection, IThat<TCollection>> AreUnique<TCollection, TItem>(
		this IStructEnumerableItems<TCollection, TItem> items,
		Func<TItem, string?> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		where TCollection : struct, IEnumerable<TItem>
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, true);

	/// <summary>
	///     …are unique, i.e. they occur exactly once in the collection.
	/// </summary>
	public static StringEqualityResult<TCollection, IThat<TCollection>> AreUnique<TCollection>(
		this IStructEnumerableItems<TCollection, string?> items)
		where TCollection : struct, IEnumerable<string?>
		=> AreUniqueCore(items, true);

	/// <summary>
	///     …are not unique, i.e. they occur more than once in the collection.
	/// </summary>
	/// <remarks>
	///     A set with a custom comparer never holds two items that its comparer considers equal, so its items count as
	///     unique without being compared, unless the comparison is changed, e.g. with <c>Using(…)</c>.
	/// </remarks>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreNotUnique(
		this IEnumerableItems<string?> items)
		=> AreUniqueCore(items, false);

	/// <summary>
	///     …are not unique, i.e. they occur more than once in the collection.
	/// </summary>
	/// <remarks>
	///     A set with a custom comparer never holds two items that its comparer considers equal, so its items count as
	///     unique without being compared, unless the comparison is changed, e.g. with <c>Using(…)</c>.
	/// </remarks>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem> AreNotUnique<TItem>(
		this IEnumerableItems<TItem> items)
		=> AreUniqueCore(items, false);

	/// <summary>
	///     …have duplicate members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember>
		AreNotUnique<TItem, TMember>(
			this IEnumerableItems<TItem> items,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, false);

	/// <summary>
	///     …have duplicate members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreNotUnique<TItem>(
		this IEnumerableItems<TItem> items,
		Func<TItem, string?> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, false);

	/// <summary>
	///     …are not unique, i.e. they occur more than once in the collection.
	/// </summary>
	/// <remarks>
	///     A set of <see langword="object" />s with a custom comparer never holds two items that its comparer considers
	///     equal, so its items count as unique without being compared, unless the comparison is changed, e.g. with
	///     <c>Using(…)</c>. The comparer of a set of another item type cannot be read without reflection and is not used.
	/// </remarks>
	public static ObjectEqualityResult<TCollection, IThat<TCollection?>, object?> AreNotUnique<TCollection>(
		this INonGenericEnumerableItems<TCollection> items)
		where TCollection : IEnumerable?
		=> AreUniqueCore(items, false);

	/// <summary>
	///     …have duplicate members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<TCollection, IThat<TCollection?>, TMember> AreNotUnique<TCollection, TMember>(
		this INonGenericEnumerableItems<TCollection> items,
		Func<object?, TMember> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		where TCollection : IEnumerable?
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, false);

	/// <summary>
	///     …have duplicate members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static StringEqualityResult<TCollection, IThat<TCollection?>> AreNotUnique<TCollection>(
		this INonGenericEnumerableItems<TCollection> items,
		Func<object?, string?> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		where TCollection : IEnumerable?
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, false);

	/// <summary>
	///     …are not unique, i.e. they occur more than once in the collection.
	/// </summary>
	public static ObjectEqualityResult<TCollection, IThat<TCollection>, TItem> AreNotUnique<TCollection, TItem>(
		this IStructEnumerableItems<TCollection, TItem> items)
		where TCollection : struct, IEnumerable<TItem>
		=> AreUniqueCore(items, false);

	/// <summary>
	///     …have duplicate members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static ObjectEqualityResult<TCollection, IThat<TCollection>, TMember>
		AreNotUnique<TCollection, TItem, TMember>(
			this IStructEnumerableItems<TCollection, TItem> items,
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
		where TCollection : struct, IEnumerable<TItem>
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, false);

	/// <summary>
	///     …have duplicate members specified by the <paramref name="memberAccessor" />.
	/// </summary>
	public static StringEqualityResult<TCollection, IThat<TCollection>> AreNotUnique<TCollection, TItem>(
		this IStructEnumerableItems<TCollection, TItem> items,
		Func<TItem, string?> memberAccessor,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
		where TCollection : struct, IEnumerable<TItem>
		=> AreUniqueCore(items, memberAccessor, doNotPopulateThisValue, false);

	/// <summary>
	///     …are not unique, i.e. they occur more than once in the collection.
	/// </summary>
	public static StringEqualityResult<TCollection, IThat<TCollection>> AreNotUnique<TCollection>(
		this IStructEnumerableItems<TCollection, string?> items)
		where TCollection : struct, IEnumerable<string?>
		=> AreUniqueCore(items, false);

	private static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreUniqueCore(
		IEnumerableItems<string?> items,
		bool expectUnique)
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique, Options: options),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<string?, string?> itemOptions =
						new(state.Options);
					return new AreUniqueConstraint<IEnumerable<string?>?, string?, string?>(
						it, grammars,
						state.Quantifier,
						a => a,
						null,
						state.Options,
						state.ExpectUnique,
						itemOptions);
				}),
			items.Subject,
			options);
	}

	private static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem> AreUniqueCore<TItem>(
		IEnumerableItems<TItem> items,
		bool expectUnique)
	{
		OptionsHolder<ObjectMatchType<TItem>> options = new(ItemEqualsMatchType<TItem>.Instance);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique, Options: options),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions =
						new(state.Options);
					return new AreUniqueConstraint<IEnumerable<TItem>?, TItem, TItem>(
						it, grammars,
						state.Quantifier,
						a => a,
						null,
						state.Options,
						state.ExpectUnique,
						itemOptions);
				}),
			items.Subject,
			options);
	}

	private static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember>
		AreUniqueCore<TItem, TMember>(
			IEnumerableItems<TItem> items,
			Func<TItem, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
	{
		memberAccessor.ThrowIfNull();
		OptionsHolder<ObjectMatchType<TMember>> options = new(ItemEqualsMatchType<TMember>.Instance);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique,
					MemberAccessorExpression: memberAccessorExpression, Options: options,
					MemberAccessor: memberAccessor),
				static (state, it, grammars)
					=> new AreUniqueConstraint<IEnumerable<TItem>?, TItem, TMember>(
						it, grammars,
						state.Quantifier,
						state.MemberAccessor,
						state.MemberAccessorExpression,
						state.Options,
						state.ExpectUnique)),
			items.Subject,
			options);
	}

	private static StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreUniqueCore<TItem>(
		IEnumerableItems<TItem> items,
		Func<TItem, string?> memberAccessor, string memberAccessorExpression, bool expectUnique)
	{
		memberAccessor.ThrowIfNull();
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique,
					MemberAccessorExpression: memberAccessorExpression, Options: options,
					MemberAccessor: memberAccessor),
				static (state, it, grammars)
					=> new AreUniqueConstraint<IEnumerable<TItem>?, TItem, string?>(
						it, grammars,
						state.Quantifier,
						state.MemberAccessor,
						state.MemberAccessorExpression,
						state.Options,
						state.ExpectUnique)),
			items.Subject,
			options);
	}

	private static ObjectEqualityResult<TCollection, IThat<TCollection?>, object?> AreUniqueCore<TCollection>(
		INonGenericEnumerableItems<TCollection> items, bool expectUnique)
		where TCollection : IEnumerable?
	{
		OptionsHolder<ObjectMatchType<object?>> options = new(ItemEqualsMatchType<object?>.Instance);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection?>, object?>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique, Options: options),
				static (state, it, grammars) =>
				{
					SubjectEqualityOptions<object?, object?> itemOptions =
						new(state.Options);
					return new AreUniqueConstraint<TCollection, object?, object?>(
						it, grammars,
						state.Quantifier,
						a => a,
						null,
						state.Options,
						state.ExpectUnique,
						itemOptions);
				}),
			items.Subject,
			options);
	}

	private static ObjectEqualityResult<TCollection, IThat<TCollection?>, TMember> AreUniqueCore<TCollection, TMember>(
		INonGenericEnumerableItems<TCollection> items,
		Func<object?, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		where TCollection : IEnumerable?
	{
		memberAccessor.ThrowIfNull();
		OptionsHolder<ObjectMatchType<TMember>> options = new(ItemEqualsMatchType<TMember>.Instance);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection?>, TMember>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique,
					MemberAccessorExpression: memberAccessorExpression, Options: options,
					MemberAccessor: memberAccessor),
				static (state, it, grammars)
					=> new AreUniqueConstraint<TCollection, object?, TMember>(
						it, grammars,
						state.Quantifier,
						state.MemberAccessor,
						state.MemberAccessorExpression,
						state.Options,
						state.ExpectUnique)),
			items.Subject,
			options);
	}

	private static StringEqualityResult<TCollection, IThat<TCollection?>> AreUniqueCore<TCollection>(
		INonGenericEnumerableItems<TCollection> items,
		Func<object?, string?> memberAccessor, string memberAccessorExpression, bool expectUnique)
		where TCollection : IEnumerable?
	{
		memberAccessor.ThrowIfNull();
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique,
					MemberAccessorExpression: memberAccessorExpression, Options: options,
					MemberAccessor: memberAccessor),
				static (state, it, grammars)
					=> new AreUniqueConstraint<TCollection, object?, string?>(
						it, grammars,
						state.Quantifier,
						state.MemberAccessor,
						state.MemberAccessorExpression,
						state.Options,
						state.ExpectUnique)),
			items.Subject,
			options);
	}

	private static ObjectEqualityResult<TCollection, IThat<TCollection>, TItem> AreUniqueCore<TCollection, TItem>(
		IStructEnumerableItems<TCollection, TItem> items, bool expectUnique)
		where TCollection : struct, IEnumerable<TItem>
	{
		OptionsHolder<ObjectMatchType<TItem>> options = new(ItemEqualsMatchType<TItem>.Instance);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique, Options: options),
				static (state, it, grammars)
					=> new AreUniqueConstraint<TCollection, object?, TItem>(
						it, grammars,
						state.Quantifier,
						a => (TItem)a!,
						null,
						state.Options,
						state.ExpectUnique)),
			items.Subject,
			options);
	}

	private static ObjectEqualityResult<TCollection, IThat<TCollection>, TMember>
		AreUniqueCore<TCollection, TItem, TMember>(
			IStructEnumerableItems<TCollection, TItem> items,
			Func<TItem, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		where TCollection : struct, IEnumerable<TItem>
	{
		memberAccessor.ThrowIfNull();
		OptionsHolder<ObjectMatchType<TMember>> options = new(ItemEqualsMatchType<TMember>.Instance);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection>, TMember>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique,
					MemberAccessorExpression: memberAccessorExpression, Options: options,
					MemberAccessor: memberAccessor),
				static (state, it, grammars)
					=> new AreUniqueConstraint<TCollection, object?, TMember>(
						it, grammars,
						state.Quantifier,
						a => state.MemberAccessor((TItem)a!),
						state.MemberAccessorExpression,
						state.Options,
						state.ExpectUnique)),
			items.Subject,
			options);
	}

	private static StringEqualityResult<TCollection, IThat<TCollection>> AreUniqueCore<TCollection, TItem>(
		IStructEnumerableItems<TCollection, TItem> items,
		Func<TItem, string?> memberAccessor, string memberAccessorExpression, bool expectUnique)
		where TCollection : struct, IEnumerable<TItem>
	{
		memberAccessor.ThrowIfNull();
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique,
					MemberAccessorExpression: memberAccessorExpression, Options: options,
					MemberAccessor: memberAccessor),
				static (state, it, grammars)
					=> new AreUniqueConstraint<TCollection, object?, string?>(
						it, grammars,
						state.Quantifier,
						a => state.MemberAccessor((TItem)a!),
						state.MemberAccessorExpression,
						state.Options,
						state.ExpectUnique)),
			items.Subject,
			options);
	}

	private static StringEqualityResult<TCollection, IThat<TCollection>> AreUniqueCore<TCollection>(
		IStructEnumerableItems<TCollection, string?> items, bool expectUnique)
		where TCollection : struct, IEnumerable<string?>
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		ExpectationBuilder expectationBuilder = items.Subject.Get().ExpectationBuilder;
		return new StringEqualityResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint(
				(Quantifier: items.Quantifier, ExpectUnique: expectUnique, Options: options),
				static (state, it, grammars)
					=> new AreUniqueConstraint<TCollection, object?, string?>(
						it, grammars,
						state.Quantifier,
						a => (string?)a,
						null,
						state.Options,
						state.ExpectUnique)),
			items.Subject,
			options);
	}
}
