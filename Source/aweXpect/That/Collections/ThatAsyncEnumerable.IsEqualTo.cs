#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsEqualTo<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TItem, TItem>(expectationBuilder, it, grammars,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<double>, IThat<IAsyncEnumerable<double>?>,
			double, double>
		IsEqualTo(
			this IThat<IAsyncEnumerable<double>?> subject,
			IEnumerable<double> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<double, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDouble();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<double>, IThat<IAsyncEnumerable<double>?>,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<double?>, IThat<IAsyncEnumerable<double?>?>,
			double?, double>
		IsEqualTo(
			this IThat<IAsyncEnumerable<double?>?> subject,
			IEnumerable<double?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<double?, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDouble();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<double?>,
			IThat<IAsyncEnumerable<double?>?>, double?, double>(
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<decimal>, IThat<IAsyncEnumerable<decimal>?>,
			decimal, decimal>
		IsEqualTo(
			this IThat<IAsyncEnumerable<decimal>?> subject,
			IEnumerable<decimal> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<decimal, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDecimal();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<decimal>, IThat<IAsyncEnumerable<decimal>?>
			,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<decimal?>,
			IThat<IAsyncEnumerable<decimal?>?>,
			decimal?, decimal>
		IsEqualTo(
			this IThat<IAsyncEnumerable<decimal?>?> subject,
			IEnumerable<decimal?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<decimal?, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDecimal();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<decimal?>,
			IThat<IAsyncEnumerable<decimal?>?>,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<float>, IThat<IAsyncEnumerable<float>?>,
			float, float>
		IsEqualTo(
			this IThat<IAsyncEnumerable<float>?> subject,
			IEnumerable<float> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<float, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateFloat();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<float>, IThat<IAsyncEnumerable<float>?>,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<float?>, IThat<IAsyncEnumerable<float?>?>,
			float?, float>
		IsEqualTo(
			this IThat<IAsyncEnumerable<float?>?> subject,
			IEnumerable<float?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<float?, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableFloat();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<float?>, IThat<IAsyncEnumerable<float?>?>,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<DateTime>,
			IThat<IAsyncEnumerable<DateTime>?>,
			DateTime, TimeSpan>
		IsEqualTo(
			this IThat<IAsyncEnumerable<DateTime>?> subject,
			IEnumerable<DateTime> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<DateTime, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDateTime();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<DateTime>,
			IThat<IAsyncEnumerable<DateTime>?>,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<DateTime?>,
			IThat<IAsyncEnumerable<DateTime?>?>,
			DateTime?, TimeSpan>
		IsEqualTo(
			this IThat<IAsyncEnumerable<DateTime?>?> subject,
			IEnumerable<DateTime?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDateTime();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<DateTime?>,
			IThat<IAsyncEnumerable<DateTime?>?>,
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
	public static StringCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		IsEqualTo(
			this IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					options,
					matchOptions)),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection matches the <paramref name="expected" /> collection of predicates.
	/// </summary>
	public static CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsEqualTo<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			[CallerArgumentExpression("expected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
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
	public static CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsEqualTo<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			[CallerArgumentExpression("expected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
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
	public static ObjectCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsNotEqualTo<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TItem, TItem>(expectationBuilder, it, grammars,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<double>, IThat<IAsyncEnumerable<double>?>,
			double, double>
		IsNotEqualTo(
			this IThat<IAsyncEnumerable<double>?> subject,
			IEnumerable<double> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<double, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDouble();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<double>, IThat<IAsyncEnumerable<double>?>,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<double?>, IThat<IAsyncEnumerable<double?>?>,
			double?, double>
		IsNotEqualTo(
			this IThat<IAsyncEnumerable<double?>?> subject,
			IEnumerable<double?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<double?, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDouble();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<double?>, IThat<IAsyncEnumerable<double?>?>
			,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<decimal>, IThat<IAsyncEnumerable<decimal>?>,
			decimal, decimal>
		IsNotEqualTo(
			this IThat<IAsyncEnumerable<decimal>?> subject,
			IEnumerable<decimal> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<decimal, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDecimal();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<decimal>, IThat<IAsyncEnumerable<decimal>?>
			,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<decimal?>,
			IThat<IAsyncEnumerable<decimal?>?>,
			decimal?, decimal>
		IsNotEqualTo(
			this IThat<IAsyncEnumerable<decimal?>?> subject,
			IEnumerable<decimal?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<decimal?, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDecimal();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<decimal?>,
			IThat<IAsyncEnumerable<decimal?>?>,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<float>, IThat<IAsyncEnumerable<float>?>,
			float, float>
		IsNotEqualTo(
			this IThat<IAsyncEnumerable<float>?> subject,
			IEnumerable<float> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<float, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateFloat();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<float>, IThat<IAsyncEnumerable<float>?>,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<float?>, IThat<IAsyncEnumerable<float?>?>,
			float?, float>
		IsNotEqualTo(
			this IThat<IAsyncEnumerable<float?>?> subject,
			IEnumerable<float?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<float?, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableFloat();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<float?>, IThat<IAsyncEnumerable<float?>?>,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<DateTime>,
			IThat<IAsyncEnumerable<DateTime>?>,
			DateTime, TimeSpan>
		IsNotEqualTo(
			this IThat<IAsyncEnumerable<DateTime>?> subject,
			IEnumerable<DateTime> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<DateTime, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDateTime();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<DateTime>,
			IThat<IAsyncEnumerable<DateTime>?>,
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
	public static ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<DateTime?>,
			IThat<IAsyncEnumerable<DateTime?>?>,
			DateTime?, TimeSpan>
		IsNotEqualTo(
			this IThat<IAsyncEnumerable<DateTime?>?> subject,
			IEnumerable<DateTime?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDateTime();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectCollectionMatchWithToleranceResult<IAsyncEnumerable<DateTime?>,
			IThat<IAsyncEnumerable<DateTime?>?>,
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
	public static StringCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		IsNotEqualTo(
			this IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringCollectionMatchResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					options,
					matchOptions).Invert()),
			subject,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not match the <paramref name="unexpected" /> collection of predicates.
	/// </summary>
	public static CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsNotEqualTo<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Expression<Func<TItem, bool>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
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
	public static CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		IsNotEqualTo<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<Action<IThatSubject<TItem?>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new CollectionMatchResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions).Invert()),
			subject,
			matchOptions);
	}
}
#endif
