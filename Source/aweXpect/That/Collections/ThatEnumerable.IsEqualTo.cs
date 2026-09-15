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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<double>, IThat<IEnumerable<double>?>,
			double, double>
		IsEqualTo(
			this IThat<IEnumerable<double>?> subject,
			IEnumerable<double> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<double, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDouble();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<double>, IThat<IEnumerable<double>?>,
			double, double>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<double, double>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<double?>, IThat<IEnumerable<double?>?>,
			double?, double>
		IsEqualTo(
			this IThat<IEnumerable<double?>?> subject,
			IEnumerable<double?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<double?, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDouble();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<double?>, IThat<IEnumerable<double?>?>,
			double?, double>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<double?, double?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<double?>, IThat<IEnumerable<double?>?>,
			double?, double>
		IsEqualTo(
			this IThat<IEnumerable<double?>?> subject,
			IEnumerable<double> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<double?, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDouble();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<double?>, IThat<IEnumerable<double?>?>,
			double?, double>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<double?, double?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
					expected.Cast<double?>(),
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal>, IThat<IEnumerable<decimal>?>,
			decimal, decimal>
		IsEqualTo(
			this IThat<IEnumerable<decimal>?> subject,
			IEnumerable<decimal> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<decimal, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDecimal();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal>, IThat<IEnumerable<decimal>?>,
			decimal, decimal>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<decimal, decimal>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal?>, IThat<IEnumerable<decimal?>?>,
			decimal?, decimal>
		IsEqualTo(
			this IThat<IEnumerable<decimal?>?> subject,
			IEnumerable<decimal?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<decimal?, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDecimal();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal?>, IThat<IEnumerable<decimal?>?>,
			decimal?, decimal>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<decimal?, decimal?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal?>, IThat<IEnumerable<decimal?>?>,
			decimal?, decimal>
		IsEqualTo(
			this IThat<IEnumerable<decimal?>?> subject,
			IEnumerable<decimal> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<decimal?, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDecimal();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal?>, IThat<IEnumerable<decimal?>?>,
			decimal?, decimal>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<decimal?, decimal?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
					expected.Cast<decimal?>(),
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<float>, IThat<IEnumerable<float>?>,
			float, float>
		IsEqualTo(
			this IThat<IEnumerable<float>?> subject,
			IEnumerable<float> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<float, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateFloat();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<float>, IThat<IEnumerable<float>?>,
			float, float>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<float, float>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<float?>, IThat<IEnumerable<float?>?>,
			float?, float>
		IsEqualTo(
			this IThat<IEnumerable<float?>?> subject,
			IEnumerable<float?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<float?, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableFloat();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<float?>, IThat<IEnumerable<float?>?>,
			float?, float>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<float?, float?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<float?>, IThat<IEnumerable<float?>?>,
			float?, float>
		IsEqualTo(
			this IThat<IEnumerable<float?>?> subject,
			IEnumerable<float> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<float?, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableFloat();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<float?>, IThat<IEnumerable<float?>?>,
			float?, float>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<float?, float?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
					expected.Cast<float?>(),
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>,
			DateTime, TimeSpan>
		IsEqualTo(
			this IThat<IEnumerable<DateTime>?> subject,
			IEnumerable<DateTime> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<DateTime, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDateTime();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>,
			DateTime, TimeSpan>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<DateTime, DateTime>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>,
			DateTime?, TimeSpan>
		IsEqualTo(
			this IThat<IEnumerable<DateTime?>?> subject,
			IEnumerable<DateTime?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDateTime();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>,
			DateTime?, TimeSpan>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<DateTime?, DateTime?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>,
			DateTime?, TimeSpan>
		IsEqualTo(
			this IThat<IEnumerable<DateTime?>?> subject,
			IEnumerable<DateTime> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDateTime();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>,
			DateTime?, TimeSpan>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<DateTime?, DateTime?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
					expected.Cast<DateTime?>(),
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<double>, IThat<IEnumerable<double>?>,
			double, double>
		IsNotEqualTo(
			this IThat<IEnumerable<double>?> subject,
			IEnumerable<double> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<double, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDouble();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<double>, IThat<IEnumerable<double>?>,
			double, double>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<double, double>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<double?>, IThat<IEnumerable<double?>?>,
			double?, double>
		IsNotEqualTo(
			this IThat<IEnumerable<double?>?> subject,
			IEnumerable<double?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<double?, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDouble();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<double?>, IThat<IEnumerable<double?>?>,
			double?, double>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<double?, double?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<double?>, IThat<IEnumerable<double?>?>,
			double?, double>
		IsNotEqualTo(
			this IThat<IEnumerable<double?>?> subject,
			IEnumerable<double> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<double?, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDouble();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<double?>, IThat<IEnumerable<double?>?>,
			double?, double>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<double?, double?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
					unexpected.Cast<double?>(),
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal>, IThat<IEnumerable<decimal>?>,
			decimal, decimal>
		IsNotEqualTo(
			this IThat<IEnumerable<decimal>?> subject,
			IEnumerable<decimal> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<decimal, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDecimal();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal>, IThat<IEnumerable<decimal>?>,
			decimal, decimal>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<decimal, decimal>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal?>, IThat<IEnumerable<decimal?>?>,
			decimal?, decimal>
		IsNotEqualTo(
			this IThat<IEnumerable<decimal?>?> subject,
			IEnumerable<decimal?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<decimal?, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDecimal();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal?>, IThat<IEnumerable<decimal?>?>,
			decimal?, decimal>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<decimal?, decimal?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal?>, IThat<IEnumerable<decimal?>?>,
			decimal?, decimal>
		IsNotEqualTo(
			this IThat<IEnumerable<decimal?>?> subject,
			IEnumerable<decimal> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<decimal?, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDecimal();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<decimal?>, IThat<IEnumerable<decimal?>?>,
			decimal?, decimal>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<decimal?, decimal?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
					unexpected.Cast<decimal?>(),
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<float>, IThat<IEnumerable<float>?>,
			float, float>
		IsNotEqualTo(
			this IThat<IEnumerable<float>?> subject,
			IEnumerable<float> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<float, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateFloat();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<float>, IThat<IEnumerable<float>?>,
			float, float>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<float, float>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<float?>, IThat<IEnumerable<float?>?>,
			float?, float>
		IsNotEqualTo(
			this IThat<IEnumerable<float?>?> subject,
			IEnumerable<float?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<float?, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableFloat();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<float?>, IThat<IEnumerable<float?>?>,
			float?, float>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<float?, float?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<float?>, IThat<IEnumerable<float?>?>,
			float?, float>
		IsNotEqualTo(
			this IThat<IEnumerable<float?>?> subject,
			IEnumerable<float> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<float?, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableFloat();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<float?>, IThat<IEnumerable<float?>?>,
			float?, float>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<float?, float?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
					unexpected.Cast<float?>(),
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>,
			DateTime, TimeSpan>
		IsNotEqualTo(
			this IThat<IEnumerable<DateTime>?> subject,
			IEnumerable<DateTime> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<DateTime, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDateTime();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime>, IThat<IEnumerable<DateTime>?>,
			DateTime, TimeSpan>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<DateTime, DateTime>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>,
			DateTime?, TimeSpan>
		IsNotEqualTo(
			this IThat<IEnumerable<DateTime?>?> subject,
			IEnumerable<DateTime?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDateTime();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>,
			DateTime?, TimeSpan>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<DateTime?, DateTime?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
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
	public static ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>,
			DateTime?, TimeSpan>
		IsNotEqualTo(
			this IThat<IEnumerable<DateTime?>?> subject,
			IEnumerable<DateTime> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDateTime();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IEnumerable<DateTime?>, IThat<IEnumerable<DateTime?>?>,
			DateTime?, TimeSpan>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<DateTime?, DateTime?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue,
					unexpected.Cast<DateTime?>(),
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
