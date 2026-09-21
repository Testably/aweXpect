using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsEqualTo<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	public static StringCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		IsEqualTo(this IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>
		IsEqualTo<TItem>(
			this IThat<IEnumerable?> subject,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToForEnumerableConstraint<IEnumerable, TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	/// <remarks>
	///     Without this overload an untyped <see cref="IEnumerable" /> would bind to the equality expectation for objects,
	///     which compares the instances by reference. A multi-dimensional array has no shape as an
	///     <see cref="IEnumerable" />, so it is compared by its flattened content.
	/// </remarks>
	[OverloadResolutionPriority(-2)]
	public static ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>
		IsEqualTo(
			this IThat<IEnumerable?> subject,
			IEnumerable expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<object?> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToForEnumerableConstraint<IEnumerable, object?, object?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected?.Cast<object?>(),
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		IsEqualTo<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	/// <remarks>
	///     Without this overload an <see cref="ImmutableArray{T}" /> or a collection expression would bind to the equality
	///     expectation for structs, which compares the backing arrays by reference.
	/// </remarks>
	public static ObjectCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		IsEqualTo<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			ImmutableArray<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	public static StringCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		IsEqualTo(this IThat<ImmutableArray<string?>> subject,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<string?>, string?, string?>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	/// <remarks>
	///     Without this overload an <see cref="ImmutableArray{T}" /> or a collection expression would bind to the equality
	///     expectation for structs, which compares the backing arrays by reference.
	/// </remarks>
	public static StringCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		IsEqualTo(this IThat<ImmutableArray<string?>> subject,
			ImmutableArray<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<string?>, string?, string?>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}
#endif

	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection of predicates.
	/// </summary>
	public static CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsEqualTo<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions)),
			subject,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection of expectations.
	/// </summary>
	public static CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsEqualTo<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions)),
			subject,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsNotEqualTo<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		IsNotEqualTo(this IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>
		IsNotEqualTo<TItem>(
			this IThat<IEnumerable?> subject,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToForEnumerableConstraint<IEnumerable, TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	/// <remarks>
	///     Without this overload an untyped <see cref="IEnumerable" /> would bind to the equality expectation for objects,
	///     which compares the instances by reference. A multi-dimensional array has no shape as an
	///     <see cref="IEnumerable" />, so it is compared by its flattened content.
	/// </remarks>
	[OverloadResolutionPriority(-2)]
	public static ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>
		IsNotEqualTo(
			this IThat<IEnumerable?> subject,
			IEnumerable unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<object?> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToForEnumerableConstraint<IEnumerable, object?, object?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected?.Cast<object?>(),
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		IsNotEqualTo<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	/// <remarks>
	///     Without this overload an <see cref="ImmutableArray{T}" /> or a collection expression would bind to the equality
	///     expectation for structs, which compares the backing arrays by reference.
	/// </remarks>
	public static ObjectCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		IsNotEqualTo<TItem>(
			this IThat<ImmutableArray<TItem>> subject,
			ImmutableArray<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<TItem>, TItem, TItem>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		IsNotEqualTo(this IThat<ImmutableArray<string?>> subject,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<string?>, string?, string?>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	/// <remarks>
	///     Without this overload an <see cref="ImmutableArray{T}" /> or a collection expression would bind to the equality
	///     expectation for structs, which compares the backing arrays by reference.
	/// </remarks>
	public static StringCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		IsNotEqualTo(this IThat<ImmutableArray<string?>> subject,
			ImmutableArray<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToForEnumerableConstraint<ImmutableArray<string?>, string?, string?>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}
#endif

	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection of predicates.
	/// </summary>
	public static CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsNotEqualTo<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions).Invert()),
			subject,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection of expectations.
	/// </summary>
	public static CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		IsNotEqualTo<TItem>(
			this IThat<IEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions).Invert()),
			subject,
			matchOptions);
	}
}
