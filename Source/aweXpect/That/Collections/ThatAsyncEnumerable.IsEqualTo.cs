#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatAsyncEnumerable.IsEqualToCore),
	"System.Collections.Generic.IAsyncEnumerable<{item}>",
	TypeParameters = ["TItem",], ConditionalOn = Net8,
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatAsyncEnumerable.IsEqualToForStringsCore),
	"System.Collections.Generic.IAsyncEnumerable<{item}>",
	ElementType = "string?", ConditionalOn = Net8,
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatAsyncEnumerable.IsEqualToFromPredicatesCore),
	"System.Collections.Generic.IAsyncEnumerable<{item}>",
	TypeParameters = ["TItem",], ConditionalOn = Net8,
	ExpectedType =
		"System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression<System.Func<{item}, bool>>>",
	Summary = CollectionMatchesPredicates, NegatedSummary = CollectionDoesNotMatchPredicates)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatAsyncEnumerable.IsEqualToFromExpectationsCore),
	"System.Collections.Generic.IAsyncEnumerable<{item}>",
	TypeParameters = ["TItem",], ConditionalOn = Net8,
	ExpectedType = "System.Collections.Generic.IEnumerable<System.Action<aweXpect.Core.IThatSubject<{item}?>>>",
	Summary = CollectionMatchesExpectations, NegatedSummary = CollectionDoesNotMatchExpectations)]
[CreateCollectionExpectation("Is{Not}EqualTo", nameof(ThatAsyncEnumerable.IsEqualToWithToleranceCore),
	"System.Collections.Generic.IAsyncEnumerable<{item}>",
	Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), ConditionalOn = Net8,
	Summary = CollectionMatches, NegatedSummary = CollectionDoesNotMatch)]
public static partial class ThatAsyncEnumerable
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

	/// <remarks>
	///     Shared body of the value overloads.
	/// </remarks>
	internal static ObjectCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsEqualToCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string expectedExpression,
			bool negated)
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IAsyncEnumerable<TItem>?>((it, grammars) =>
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
	///     Counterpart of <see cref="IsEqualToCore{TItem}" /> for a collection of strings, which carries the string
	///     equality options instead.
	/// </remarks>
	internal static StringCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		IsEqualToForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string expectedExpression,
			bool negated)
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint<IAsyncEnumerable<string?>?>((it, grammars) =>
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
	///     Shared body of the overloads whose expected collection is a collection of predicates, which carry no
	///     equality options.
	/// </remarks>
	internal static CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsEqualToFromPredicatesCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			string expectedExpression,
			bool negated)
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IAsyncEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToFromPredicateConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			matchOptions);
	}

	/// <remarks>
	///     Counterpart of <see cref="IsEqualToFromPredicatesCore{TItem}" /> for a collection of expectations.
	/// </remarks>
	internal static CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsEqualToFromExpectationsCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			string expectedExpression,
			bool negated)
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IAsyncEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToFromExpectationsConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(), expected, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			matchOptions);
	}

	/// <remarks>
	///     Shared body of the tolerance overloads. The generated overloads supply the concrete types and the options
	///     instance.
	/// </remarks>
	internal static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<TItem>,
			IThat<IAsyncEnumerable<TItem>?>, TItem, TTolerance>
		IsEqualToWithToleranceCore<TItem, TTolerance>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string expectedExpression,
			bool negated)
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>,
			TItem, TTolerance>(
			expectationBuilder.AddConstraint<IAsyncEnumerable<TItem>?>((it, grammars) =>
			{
				IsEqualToConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression, expected, options, matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}
}
#endif
