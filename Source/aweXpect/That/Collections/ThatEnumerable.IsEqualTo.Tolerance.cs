using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

[CreateCollectionToleranceExpectations("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToWithTolerance),
	"System.Collections.Generic.IEnumerable",
	Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
	Summary = "Verifies that the collection matches the <paramref name=\"expected\" /> collection.",
	NegatedSummary = "Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection.")]
[CreateCollectionToleranceExpectations("Is{Not}EqualTo", nameof(ThatEnumerable.IsEqualToWithToleranceForEnumerable),
	"System.Collections.Immutable.ImmutableArray",
	Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
	ConditionalOn = "NET8_0_OR_GREATER",
	Summary = "Verifies that the collection matches the <paramref name=\"expected\" /> collection.",
	NegatedSummary = "Verifies that the collection does not match the <paramref name=\"unexpected\" /> collection.")]
public static partial class ThatEnumerable
{
	/// <remarks>
	///     Shared body of every tolerance overload of <c>IsEqualTo</c> / <c>IsNotEqualTo</c> whose subject is a
	///     reference collection. The generated overloads supply the concrete types and the options instance.
	/// </remarks>
	internal static ObjectCollectionMatchWithToleranceResult<TCollection, IThat<TCollection?>, TItem, TTolerance>
		IsEqualToWithTolerance<TCollection, TItem, TTolerance>(
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
					expectedExpression,
					expected,
					options,
					matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}

	/// <remarks>
	///     Counterpart of <see cref="IsEqualToWithTolerance{TCollection,TItem,TTolerance}" /> for a struct collection
	///     such as <c>ImmutableArray&lt;T&gt;</c>, whose subject is not nullable and which needs the enumerable
	///     constraint.
	/// </remarks>
	internal static ObjectCollectionMatchWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		IsEqualToWithToleranceForEnumerable<TCollection, TItem, TTolerance>(
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
					expectedExpression,
					expected,
					options,
					matchOptions);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options,
			matchOptions);
	}
}
