using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToCore),
	"System.Collections.Generic.IEnumerable<{item}>",
	TypeParameters = ["TItem",],
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToForStringsCore),
	"System.Collections.Generic.IEnumerable<{item}>",
	ElementType = "string?",
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToForEnumerableCore),
	"System.Collections.IEnumerable",
	TypeParameters = ["TItem",], Priority = -1,
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToForObjectsCore),
	"System.Collections.IEnumerable",
	ElementType = "object?", ExpectedType = "System.Collections.IEnumerable", Priority = -2,
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch,
	Remarks = UntypedEnumerableRemarks)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToForStructCore),
	"System.Collections.Immutable.ImmutableArray<{item}>",
	TypeParameters = ["TItem",], ConditionalOn = Net8,
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToForStructCore),
	"System.Collections.Immutable.ImmutableArray<{item}>",
	TypeParameters = ["TItem",], ConditionalOn = Net8,
	ExpectedType = "System.Collections.Immutable.ImmutableArray<{item}>",
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch,
	Remarks = ImmutableArrayRemarks)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToForStructStringsCore),
	"System.Collections.Immutable.ImmutableArray<{item}>",
	ElementType = "string?", ConditionalOn = Net8,
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToForStructStringsCore),
	"System.Collections.Immutable.ImmutableArray<{item}>",
	ElementType = "string?", ConditionalOn = Net8,
	ExpectedType = "System.Collections.Immutable.ImmutableArray<{item}>",
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch,
	Remarks = ImmutableArrayRemarks)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToFromPredicatesCore),
	"System.Collections.Generic.IEnumerable<{item}>",
	TypeParameters = ["TItem",],
	ExpectedType =
		"System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression<System.Func<{item}, bool>>>",
	Summary = CollectionMatchesPredicates, NegatedSummary = CollectionDoesNotMatchPredicates)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToFromExpectationsCore),
	"System.Collections.Generic.IEnumerable<{item}>",
	TypeParameters = ["TItem",],
	ExpectedType = "System.Collections.Generic.IEnumerable<System.Action<aweXpect.Core.IThatSubject<{item}?>>>",
	Summary = CollectionMatchesExpectations, NegatedSummary = CollectionDoesNotMatchExpectations)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToWithToleranceCore),
	"System.Collections.Generic.IEnumerable<{item}>",
	Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToWithToleranceForStructCore),
	"System.Collections.Immutable.ImmutableArray<{item}>",
	Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), ConditionalOn = Net8,
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch)]
public static partial class ThatEnumerable
{
	private const string Net8 = "NET8_0_OR_GREATER";

	private const string CollectionMatches =
		"Verifies that the collection matches the <paramref name=\"expected\" /> collection.";

	private const string CollectionDoesNotMatch =
		"Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection.";

	private const string CollectionMatchesPredicates =
		"Verifies that the collection matches the <paramref name=\"expected\" /> collection of predicates.";

	private const string CollectionDoesNotMatchPredicates =
		"Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection of predicates.";

	private const string CollectionMatchesExpectations =
		"Verifies that the collection matches the <paramref name=\"expected\" /> collection of expectations.";

	private const string CollectionDoesNotMatchExpectations =
		"Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection of expectations.";

	private const string UntypedEnumerableRemarks =
		"Without this overload an untyped <see cref=\"System.Collections.IEnumerable\" /> would bind to the equality expectation for objects,\n" +
		"which compares the instances by reference. A multi-dimensional array has no shape as an\n" +
		"<see cref=\"System.Collections.IEnumerable\" />, so it is compared by its flattened content.";

	private const string ImmutableArrayRemarks =
		"Without this overload an <see cref=\"System.Collections.Immutable.ImmutableArray{T}\" /> or a collection expression would bind to the equality\n" +
		"expectation for structs, which compares the backing arrays by reference.";

	/// <remarks>
	///     Shared body of the value overloads whose subject is a reference collection.
	/// </remarks>
	internal static ObjectCollectionMatchResult<TCollection, IThat<TCollection?>, TItem>
		IsEqualToCore<TCollection, TItem>(
			IThat<TCollection?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
		where TCollection : class, IEnumerable<TItem>
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<TCollection, IThat<TCollection?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	/// <remarks>
	///     Counterpart of <see cref="IsEqualToCore{TCollection,TItem}" /> for a collection of strings, which carries
	///     the string equality options instead.
	/// </remarks>
	internal static StringCollectionMatchResult<TCollection, IThat<TCollection?>>
		IsEqualToForStringsCore<TCollection>(
			IThat<TCollection?> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
		where TCollection : class, IEnumerable<string?>
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint<IEnumerable<string?>?>((it, grammars) =>
			{
				IsEqualToConstraint<string?, string?> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	/// <remarks>
	///     Counterpart of <see cref="IsEqualToCore{TCollection,TItem}" /> for an untyped subject, which needs the
	///     enumerable constraint because the subject is not an <see cref="IEnumerable{T}" />.
	/// </remarks>
	internal static ObjectCollectionMatchResult<TCollection, IThat<TCollection?>, TItem>
		IsEqualToForEnumerableCore<TCollection, TItem>(
			IThat<TCollection?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
		where TCollection : class, IEnumerable
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<TCollection, IThat<TCollection?>, TItem>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<TCollection, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	/// <remarks>
	///     Counterpart of <see cref="IsEqualToForEnumerableCore{TCollection,TItem}" /> for an untyped expected
	///     collection, whose elements are flattened to <see cref="object" /> before they are compared.
	/// </remarks>
	internal static ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>
		IsEqualToForObjectsCore(
			IThat<IEnumerable?> subject,
			IEnumerable expected,
			string expectedExpression,
			bool negated)
	{
		ObjectEqualityOptions<object?> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>(
			expectationBuilder.AddConstraint<IEnumerable?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<IEnumerable, object?, object?> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected?.Cast<object?>(), options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	/// <remarks>
	///     Counterpart of <see cref="IsEqualToForEnumerableCore{TCollection,TItem}" /> for a struct collection such as
	///     <c>ImmutableArray&lt;T&gt;</c>, whose subject is not nullable.
	/// </remarks>
	internal static ObjectCollectionMatchResult<TCollection, IThat<TCollection>, TItem>
		IsEqualToForStructCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable<TItem>
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<TCollection, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	/// <remarks>
	///     Counterpart of <see cref="IsEqualToForStructCore{TCollection,TItem}" /> for a collection of strings.
	/// </remarks>
	internal static StringCollectionMatchResult<TCollection, IThat<TCollection>>
		IsEqualToForStructStringsCore<TCollection>(
			IThat<TCollection> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable<string?>
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<TCollection, string?, string?> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	/// <remarks>
	///     Shared body of the overloads whose expected collection is a collection of predicates, which carry no
	///     equality options.
	/// </remarks>
	internal static CollectionMatchResult<TCollection, IThat<TCollection?>, TItem>
		IsEqualToFromPredicatesCore<TCollection, TItem>(
			IThat<TCollection?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			string expectedExpression,
			bool negated)
		where TCollection : class, IEnumerable<TItem>
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<TCollection, IThat<TCollection?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToFromPredicateConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			matchOptions);
	}

	/// <remarks>
	///     Counterpart of <see cref="IsEqualToFromPredicatesCore{TCollection,TItem}" /> for a collection of
	///     expectations.
	/// </remarks>
	internal static CollectionMatchResult<TCollection, IThat<TCollection?>, TItem>
		IsEqualToFromExpectationsCore<TCollection, TItem>(
			IThat<TCollection?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			string expectedExpression,
			bool negated)
		where TCollection : class, IEnumerable<TItem>
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<TCollection, IThat<TCollection?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToFromExpectationsConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			matchOptions);
	}

	/// <remarks>
	///     Shared body of the tolerance overloads whose subject is a reference collection. The generated overloads
	///     supply the concrete types and the options instance.
	/// </remarks>
	internal static ObjectCollectionMatchWithToleranceResult<TCollection, IThat<TCollection?>, TItem, TTolerance>
		IsEqualToWithToleranceCore<TCollection, TItem, TTolerance>(
			IThat<TCollection?> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string expectedExpression,
			bool negated)
		where TCollection : class, IEnumerable<TItem>
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<TCollection, IThat<TCollection?>, TItem, TTolerance>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression, expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	/// <remarks>
	///     Counterpart of <see cref="IsEqualToWithToleranceCore{TCollection,TItem,TTolerance}" /> for a struct
	///     collection, whose subject is not nullable and which needs the enumerable constraint.
	/// </remarks>
	internal static ObjectCollectionMatchWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		IsEqualToWithToleranceForStructCore<TCollection, TItem, TTolerance>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable<TItem>
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				IsEqualToForEnumerableConstraint<TCollection, TItem, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression, expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}
}
