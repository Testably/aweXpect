using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNumber
{
#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<TNumber, IThat<TNumber>>, TNumber?> IsBetween<TNumber>(
		this IThat<TNumber> source, TNumber? minimum)
		where TNumber : struct, INumber<TNumber>
		=> new(maximum =>
		{
			NumberTolerance<TNumber> options = new(CalculateDifference);
			return new NumberToleranceResult<TNumber, IThat<TNumber>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<TNumber>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<TNumber, IThat<TNumber?>>, TNumber?> IsBetween<TNumber>(
		this IThat<TNumber?> source, TNumber? minimum)
		where TNumber : struct, INumber<TNumber>
		=> new(maximum =>
		{
			NumberTolerance<TNumber> options = new(CalculateDifference);
			return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<TNumber>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<TNumber, IThat<TNumber>>, TNumber?> IsNotBetween<TNumber>(
		this IThat<TNumber> source, TNumber? minimum)
		where TNumber : struct, INumber<TNumber>
		=> new(maximum =>
		{
			NumberTolerance<TNumber> options = new(CalculateDifference);
			return new NumberToleranceResult<TNumber, IThat<TNumber>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<TNumber>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<TNumber, IThat<TNumber?>>, TNumber?> IsNotBetween<TNumber>(
		this IThat<TNumber?> source, TNumber? minimum)
		where TNumber : struct, INumber<TNumber>
		=> new(maximum =>
		{
			NumberTolerance<TNumber> options = new(CalculateDifference);
			return new NullableNumberToleranceResult<TNumber, IThat<TNumber?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<TNumber>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	private sealed class IsInRangeConstraint<TNumber> : ConstraintResult.WithValue<TNumber>,
		IValueConstraint<TNumber>
		where TNumber : struct, INumber<TNumber>
	{
		private readonly string _it;
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;
		private readonly NumberTolerance<TNumber> _options;

		public IsInRangeConstraint(string it,
			ExpectationGrammars grammars,
			TNumber? minimum,
			TNumber? maximum,
			NumberTolerance<TNumber> options) : base(grammars)
		{
			if (maximum < minimum)
			{
				// ReSharper disable once LocalizableElement
				throw new ArgumentOutOfRangeException(nameof(maximum),
					"The maximum must be greater than or equal to the minimum.");
			}

			_it = it;
			_minimum = minimum;
			_maximum = maximum;
			_options = options;
		}

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = _options.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is between ");
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(_it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not between ");
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsInRangeConstraint<TNumber> : ConstraintResult.WithEqualToValue<TNumber?>,
		IValueConstraint<TNumber?>
		where TNumber : struct, INumber<TNumber>
	{
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;
		private readonly NumberTolerance<TNumber> _options;

		public NullableIsInRangeConstraint(string it,
			ExpectationGrammars grammars,
			TNumber? minimum,
			TNumber? maximum,
			NumberTolerance<TNumber> options) : base(it, grammars, minimum is null)
		{
			if (maximum < minimum)
			{
				// ReSharper disable once LocalizableElement
				throw new ArgumentOutOfRangeException(nameof(maximum),
					"The maximum must be greater than or equal to the minimum.");
			}

			_minimum = minimum;
			_maximum = maximum;
			_options = options;
		}

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = _options.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is between ");
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not between ");
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#else
	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<byte, IThat<byte>>, byte?> IsBetween(
		this IThat<byte> source,
		byte? minimum)
		=> new(maximum =>
		{
			NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
			return new NumberToleranceResult<byte, IThat<byte>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<byte>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<sbyte, IThat<sbyte>>, sbyte?> IsBetween(
		this IThat<sbyte> source,
		sbyte? minimum)
		=> new(maximum =>
		{
			NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
			return new NumberToleranceResult<sbyte, IThat<sbyte>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<sbyte>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<short, IThat<short>>, short?> IsBetween(
		this IThat<short> source,
		short? minimum)
		=> new(maximum =>
		{
			NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
			return new NumberToleranceResult<short, IThat<short>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<short>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<ushort, IThat<ushort>>, ushort?> IsBetween(
		this IThat<ushort> source,
		ushort? minimum)
		=> new(maximum =>
		{
			NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
			return new NumberToleranceResult<ushort, IThat<ushort>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<ushort>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<int, IThat<int>>, int?> IsBetween(
		this IThat<int> source,
		int? minimum)
		=> new(maximum =>
		{
			NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NumberToleranceResult<int, IThat<int>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<int>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<uint, IThat<uint>>, uint?> IsBetween(
		this IThat<uint> source,
		uint? minimum)
		=> new(maximum =>
		{
			NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NumberToleranceResult<uint, IThat<uint>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<uint>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<long, IThat<long>>, long?> IsBetween(
		this IThat<long> source,
		long? minimum)
		=> new(maximum =>
		{
			NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NumberToleranceResult<long, IThat<long>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<long>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<ulong, IThat<ulong>>, ulong?> IsBetween(
		this IThat<ulong> source,
		ulong? minimum)
		=> new(maximum =>
		{
			NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NumberToleranceResult<ulong, IThat<ulong>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<ulong>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<float, IThat<float>>, float?> IsBetween(
		this IThat<float> source,
		float? minimum)
		=> new(maximum =>
		{
			NumberTolerance<float> options = new((a, e) =>
			{
				if (float.IsNaN(a) || float.IsNaN(e))
				{
					return null;
				}

				return a > e ? a - e : e - a;
			});
			return new NumberToleranceResult<float, IThat<float>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<float>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<double, IThat<double>>, double?> IsBetween(
		this IThat<double> source,
		double? minimum)
		=> new(maximum =>
		{
			NumberTolerance<double> options = new((a, e) =>
			{
				if (double.IsNaN(a) || double.IsNaN(e))
				{
					return null;
				}

				return a > e ? a - e : e - a;
			});
			return new NumberToleranceResult<double, IThat<double>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<double>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NumberToleranceResult<decimal, IThat<decimal>>, decimal?> IsBetween(
		this IThat<decimal> source,
		decimal? minimum)
		=> new(maximum =>
		{
			NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NumberToleranceResult<decimal, IThat<decimal>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<decimal>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<byte, IThat<byte?>>, byte?> IsBetween(
		this IThat<byte?> source,
		byte? minimum)
		=> new(maximum =>
		{
			NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
			return new NullableNumberToleranceResult<byte, IThat<byte?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<byte>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<sbyte, IThat<sbyte?>>, sbyte?> IsBetween(
		this IThat<sbyte?> source,
		sbyte? minimum)
		=> new(maximum =>
		{
			NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
			return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<sbyte>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<short, IThat<short?>>, short?> IsBetween(
		this IThat<short?> source,
		short? minimum)
		=> new(maximum =>
		{
			NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
			return new NullableNumberToleranceResult<short, IThat<short?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<short>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<ushort, IThat<ushort?>>, ushort?> IsBetween(
		this IThat<ushort?> source,
		ushort? minimum)
		=> new(maximum =>
		{
			NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
			return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<ushort>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<int, IThat<int?>>, int?> IsBetween(
		this IThat<int?> source,
		int? minimum)
		=> new(maximum =>
		{
			NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NullableNumberToleranceResult<int, IThat<int?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<int>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<uint, IThat<uint?>>, uint?> IsBetween(
		this IThat<uint?> source,
		uint? minimum)
		=> new(maximum =>
		{
			NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NullableNumberToleranceResult<uint, IThat<uint?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<uint>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<long, IThat<long?>>, long?> IsBetween(
		this IThat<long?> source,
		long? minimum)
		=> new(maximum =>
		{
			NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NullableNumberToleranceResult<long, IThat<long?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<long>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<ulong, IThat<ulong?>>, ulong?> IsBetween(
		this IThat<ulong?> source,
		ulong? minimum)
		=> new(maximum =>
		{
			NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<ulong>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<float, IThat<float?>>, float?> IsBetween(
		this IThat<float?> source,
		float? minimum)
		=> new(maximum =>
		{
			NumberTolerance<float> options = new((a, e) =>
			{
				if (float.IsNaN(a) || float.IsNaN(e))
				{
					return null;
				}

				return a > e ? a - e : e - a;
			});
			return new NullableNumberToleranceResult<float, IThat<float?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<float>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<double, IThat<double?>>, double?> IsBetween(
		this IThat<double?> source,
		double? minimum)
		=> new(maximum =>
		{
			NumberTolerance<double> options = new((a, e) =>
			{
				if (double.IsNaN(a) || double.IsNaN(e))
				{
					return null;
				}

				return a > e ? a - e : e - a;
			});
			return new NullableNumberToleranceResult<double, IThat<double?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<double>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is in the range between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<NullableNumberToleranceResult<decimal, IThat<decimal?>>, decimal?> IsBetween(
		this IThat<decimal?> source,
		decimal? minimum)
		=> new(maximum =>
		{
			NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<decimal>(it, grammars, minimum, maximum, options)),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<byte, IThat<byte>>, byte?> IsNotBetween(
		this IThat<byte> source,
		byte? minimum)
		=> new(maximum =>
		{
			NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
			return new NumberToleranceResult<byte, IThat<byte>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<byte>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<sbyte, IThat<sbyte>>, sbyte?> IsNotBetween(
		this IThat<sbyte> source,
		sbyte? minimum)
		=> new(maximum =>
		{
			NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
			return new NumberToleranceResult<sbyte, IThat<sbyte>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<sbyte>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<short, IThat<short>>, short?> IsNotBetween(
		this IThat<short> source,
		short? minimum)
		=> new(maximum =>
		{
			NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
			return new NumberToleranceResult<short, IThat<short>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<short>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<ushort, IThat<ushort>>, ushort?> IsNotBetween(
		this IThat<ushort> source,
		ushort? minimum)
		=> new(maximum =>
		{
			NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
			return new NumberToleranceResult<ushort, IThat<ushort>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<ushort>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<int, IThat<int>>, int?> IsNotBetween(
		this IThat<int> source,
		int? minimum)
		=> new(maximum =>
		{
			NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NumberToleranceResult<int, IThat<int>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<int>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<uint, IThat<uint>>, uint?> IsNotBetween(
		this IThat<uint> source,
		uint? minimum)
		=> new(maximum =>
		{
			NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NumberToleranceResult<uint, IThat<uint>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<uint>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<long, IThat<long>>, long?> IsNotBetween(
		this IThat<long> source,
		long? minimum)
		=> new(maximum =>
		{
			NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NumberToleranceResult<long, IThat<long>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<long>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<ulong, IThat<ulong>>, ulong?> IsNotBetween(
		this IThat<ulong> source,
		ulong? minimum)
		=> new(maximum =>
		{
			NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NumberToleranceResult<ulong, IThat<ulong>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<ulong>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<float, IThat<float>>, float?> IsNotBetween(
		this IThat<float> source,
		float? minimum)
		=> new(maximum =>
		{
			NumberTolerance<float> options = new((a, e) =>
			{
				if (float.IsNaN(a) || float.IsNaN(e))
				{
					return null;
				}

				return a > e ? a - e : e - a;
			});
			return new NumberToleranceResult<float, IThat<float>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<float>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<double, IThat<double>>, double?> IsNotBetween(
		this IThat<double> source,
		double? minimum)
		=> new(maximum =>
		{
			NumberTolerance<double> options = new((a, e) =>
			{
				if (double.IsNaN(a) || double.IsNaN(e))
				{
					return null;
				}

				return a > e ? a - e : e - a;
			});
			return new NumberToleranceResult<double, IThat<double>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<double>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NumberToleranceResult<decimal, IThat<decimal>>, decimal?> IsNotBetween(
		this IThat<decimal> source,
		decimal? minimum)
		=> new(maximum =>
		{
			NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NumberToleranceResult<decimal, IThat<decimal>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsInRangeConstraint<decimal>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<byte, IThat<byte?>>, byte?> IsNotBetween(
		this IThat<byte?> source,
		byte? minimum)
		=> new(maximum =>
		{
			NumberTolerance<byte> options = new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });
			return new NullableNumberToleranceResult<byte, IThat<byte?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<byte>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<sbyte, IThat<sbyte?>>, sbyte?> IsNotBetween(
		this IThat<sbyte?> source,
		sbyte? minimum)
		=> new(maximum =>
		{
			NumberTolerance<sbyte> options = new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });
			return new NullableNumberToleranceResult<sbyte, IThat<sbyte?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<sbyte>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<short, IThat<short?>>, short?> IsNotBetween(
		this IThat<short?> source,
		short? minimum)
		=> new(maximum =>
		{
			NumberTolerance<short> options = new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });
			return new NullableNumberToleranceResult<short, IThat<short?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<short>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<ushort, IThat<ushort?>>, ushort?> IsNotBetween(
		this IThat<ushort?> source,
		ushort? minimum)
		=> new(maximum =>
		{
			NumberTolerance<ushort> options = new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });
			return new NullableNumberToleranceResult<ushort, IThat<ushort?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<ushort>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<int, IThat<int?>>, int?> IsNotBetween(
		this IThat<int?> source,
		int? minimum)
		=> new(maximum =>
		{
			NumberTolerance<int> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NullableNumberToleranceResult<int, IThat<int?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<int>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<uint, IThat<uint?>>, uint?> IsNotBetween(
		this IThat<uint?> source,
		uint? minimum)
		=> new(maximum =>
		{
			NumberTolerance<uint> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NullableNumberToleranceResult<uint, IThat<uint?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<uint>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<long, IThat<long?>>, long?> IsNotBetween(
		this IThat<long?> source,
		long? minimum)
		=> new(maximum =>
		{
			NumberTolerance<long> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NullableNumberToleranceResult<long, IThat<long?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<long>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<ulong, IThat<ulong?>>, ulong?> IsNotBetween(
		this IThat<ulong?> source,
		ulong? minimum)
		=> new(maximum =>
		{
			NumberTolerance<ulong> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NullableNumberToleranceResult<ulong, IThat<ulong?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<ulong>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<float, IThat<float?>>, float?> IsNotBetween(
		this IThat<float?> source,
		float? minimum)
		=> new(maximum =>
		{
			NumberTolerance<float> options = new((a, e) =>
			{
				if (float.IsNaN(a) || float.IsNaN(e))
				{
					return null;
				}

				return a > e ? a - e : e - a;
			});
			return new NullableNumberToleranceResult<float, IThat<float?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<float>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<double, IThat<double?>>, double?> IsNotBetween(
		this IThat<double?> source,
		double? minimum)
		=> new(maximum =>
		{
			NumberTolerance<double> options = new((a, e) =>
			{
				if (double.IsNaN(a) || double.IsNaN(e))
				{
					return null;
				}

				return a > e ? a - e : e - a;
			});
			return new NullableNumberToleranceResult<double, IThat<double?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<double>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	/// <summary>
	///     Verifies that the subject is not in the range between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<NullableNumberToleranceResult<decimal, IThat<decimal?>>, decimal?> IsNotBetween(
		this IThat<decimal?> source,
		decimal? minimum)
		=> new(maximum =>
		{
			NumberTolerance<decimal> options = new((a, e) => { checked { return a > e ? a - e : e - a; } });
			return new NullableNumberToleranceResult<decimal, IThat<decimal?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new NullableIsInRangeConstraint<decimal>(it, grammars, minimum, maximum, options).Invert()),
				source,
				options);
		});

	private sealed class IsInRangeConstraint<TNumber> : ConstraintResult.WithValue<TNumber>,
		IValueConstraint<TNumber>
		where TNumber : struct, IComparable<TNumber>
	{
		private readonly string _it;
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;
		private readonly NumberTolerance<TNumber> _options;

		public IsInRangeConstraint(string it,
			ExpectationGrammars grammars,
			TNumber? minimum,
			TNumber? maximum,
			NumberTolerance<TNumber> options) : base(grammars)
		{
			if (maximum != null && minimum != null &&
			    maximum.Value.CompareTo(minimum.Value) < 0)
			{
				// ReSharper disable once LocalizableElement
				throw new ArgumentOutOfRangeException(nameof(maximum),
					"The maximum must be greater than or equal to the minimum.");
			}

			_it = it;
			_minimum = minimum;
			_maximum = maximum;
			_options = options;
		}

		public ConstraintResult IsMetBy(TNumber actual)
		{
			Actual = actual;
			Outcome = _options.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is between ");
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(_it).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not between ");
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class NullableIsInRangeConstraint<TNumber> : ConstraintResult.WithEqualToValue<TNumber?>,
		IValueConstraint<TNumber?>
		where TNumber : struct, IComparable<TNumber>
	{
		private readonly TNumber? _maximum;
		private readonly TNumber? _minimum;
		private readonly NumberTolerance<TNumber> _options;

		public NullableIsInRangeConstraint(string it,
			ExpectationGrammars grammars,
			TNumber? minimum,
			TNumber? maximum,
			NumberTolerance<TNumber> options) : base(it, grammars, minimum is null)
		{
			if (maximum != null && minimum != null &&
			    maximum.Value.CompareTo(minimum.Value) < 0)
			{
				// ReSharper disable once LocalizableElement
				throw new ArgumentOutOfRangeException(nameof(maximum),
					"The maximum must be greater than or equal to the minimum.");
			}

			_minimum = minimum;
			_maximum = maximum;
			_options = options;
		}

		public ConstraintResult IsMetBy(TNumber? actual)
		{
			Actual = actual;
			Outcome = _options.IsInRange(actual, _minimum, _maximum) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is between ");
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not between ");
			Formatter.Format(stringBuilder, _minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
#endif
}
