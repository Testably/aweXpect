#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     …are equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ToleranceEqualityResult<IAsyncEnumerable<double>, IThat<IAsyncEnumerable<double>?>, double, double>
		AreEqualTo(this Elements<double> elements, double expected)
	{
		IElements<double> iElements = elements;
		ObjectEqualityWithToleranceOptions<double, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDouble();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ToleranceEqualityResult<IAsyncEnumerable<double>, IThat<IAsyncEnumerable<double>?>, double, double>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<double>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	/// <summary>
	///     …are equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ToleranceEqualityResult<IAsyncEnumerable<double?>, IThat<IAsyncEnumerable<double?>?>, double?, double>
		AreEqualTo(this Elements<double?> elements, double? expected)
	{
		IElements<double?> iElements = elements;
		ObjectEqualityWithToleranceOptions<double?, double> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDouble();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ToleranceEqualityResult<IAsyncEnumerable<double?>, IThat<IAsyncEnumerable<double?>?>, double?,
			double>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<double?>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	/// <summary>
	///     …are equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ToleranceEqualityResult<IAsyncEnumerable<float>, IThat<IAsyncEnumerable<float>?>, float, float>
		AreEqualTo(this Elements<float> elements, float expected)
	{
		IElements<float> iElements = elements;
		ObjectEqualityWithToleranceOptions<float, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateFloat();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ToleranceEqualityResult<IAsyncEnumerable<float>, IThat<IAsyncEnumerable<float>?>, float, float>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<float>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	/// <summary>
	///     …are equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ToleranceEqualityResult<IAsyncEnumerable<float?>, IThat<IAsyncEnumerable<float?>?>, float?, float>
		AreEqualTo(this Elements<float?> elements, float? expected)
	{
		IElements<float?> iElements = elements;
		ObjectEqualityWithToleranceOptions<float?, float> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableFloat();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ToleranceEqualityResult<IAsyncEnumerable<float?>, IThat<IAsyncEnumerable<float?>?>, float?, float>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<float?>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	/// <summary>
	///     …are equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ToleranceEqualityResult<IAsyncEnumerable<decimal>, IThat<IAsyncEnumerable<decimal>?>, decimal,
			decimal>
		AreEqualTo(this Elements<decimal> elements, decimal expected)
	{
		IElements<decimal> iElements = elements;
		ObjectEqualityWithToleranceOptions<decimal, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDecimal();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ToleranceEqualityResult<IAsyncEnumerable<decimal>, IThat<IAsyncEnumerable<decimal>?>, decimal,
			decimal>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<decimal>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	/// <summary>
	///     …are equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ToleranceEqualityResult<IAsyncEnumerable<decimal?>, IThat<IAsyncEnumerable<decimal?>?>, decimal?,
			decimal>
		AreEqualTo(this Elements<decimal?> elements, decimal? expected)
	{
		IElements<decimal?> iElements = elements;
		ObjectEqualityWithToleranceOptions<decimal?, decimal> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDecimal();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ToleranceEqualityResult<IAsyncEnumerable<decimal?>, IThat<IAsyncEnumerable<decimal?>?>, decimal?,
			decimal>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<decimal?>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	/// <summary>
	///     …are equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ToleranceEqualityResult<IAsyncEnumerable<DateTime>, IThat<IAsyncEnumerable<DateTime>?>, DateTime,
			TimeSpan>
		AreEqualTo(this Elements<DateTime> elements, DateTime expected)
	{
		IElements<DateTime> iElements = elements;
		ObjectEqualityWithToleranceOptions<DateTime, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateDateTime();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ToleranceEqualityResult<IAsyncEnumerable<DateTime>, IThat<IAsyncEnumerable<DateTime>?>, DateTime,
			TimeSpan>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<DateTime>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	/// <summary>
	///     …are equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ToleranceEqualityResult<IAsyncEnumerable<DateTime?>, IThat<IAsyncEnumerable<DateTime?>?>, DateTime?,
			TimeSpan>
		AreEqualTo(this Elements<DateTime?> elements, DateTime? expected)
	{
		IElements<DateTime?> iElements = elements;
		ObjectEqualityWithToleranceOptions<DateTime?, TimeSpan> options =
			ObjectEqualityWithToleranceOptionsFactory.CreateNullableDateTime();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ToleranceEqualityResult<IAsyncEnumerable<DateTime?>, IThat<IAsyncEnumerable<DateTime?>?>, DateTime?,
			TimeSpan>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<DateTime?>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	/// <summary>
	///     …are equal to the <paramref name="expected" /> value.
	/// </summary>
	public static ObjectEqualityResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>, TItem>
		AreEqualTo<TItem>(this Elements<TItem> elements, TItem expected)
	{
		IElements<TItem> iElements = elements;
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>?, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<TItem>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}

	/// <summary>
	///     …are equal to the <paramref name="expected" /> value.
	/// </summary>
	public static StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreEqualTo(
		this Elements elements,
		string? expected)
	{
		IElements iElements = elements;
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = iElements.Subject.Get().ExpectationBuilder;
		return new StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionConstraint<string?>(
					expectationBuilder,
					it, grammars,
					iElements.Quantifier,
					g => ElementExpectations.IsEqualTo(g, Formatter.Format(expected), options),
					a => options.AreConsideredEqual(a, expected),
					"were")),
			iElements.Subject,
			options);
	}
}
#endif
