using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;
#if NET8_0_OR_GREATER
using System.Numerics;
#else
using System.Globalization;
#endif

namespace aweXpect.Options;

internal static class ObjectEqualityOptions
{
	internal static readonly IObjectMatchType EqualsMatch = new EqualsMatchType();

	/// <summary>
	///     Prepends the <paramref name="itemNoun" /> and the <paramref name="comparison" /> to the
	///     <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A match type that only formats the value names neither the item nor the comparison on its own, so a verb
	///     that needs both reads <c>contains an item equal to 3</c>.
	/// </remarks>
	internal static string GetItemExpectation(string expected, string? itemNoun, string? comparison)
		=> (itemNoun is null ? "" : itemNoun + " ") + (comparison is null ? "" : comparison + " ") + expected;

	private sealed class EqualsMatchType : IObjectMatchType
	{
		/// <inheritdoc cref="object.ToString()" />
		public override string ToString() => "";

		#region IEquality Members

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TSubject, TExpected}(TSubject, TExpected)" />
#if NET8_0_OR_GREATER
		public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
		{
			if (actual is null && expected is null)
			{
				return ValueTask.FromResult(true);
			}

			if (actual is null || expected is null)
			{
				return ValueTask.FromResult(false);
			}

			if (DateTimeKindComparison.AreKindsIncompatible(actual, expected))
			{
				return ValueTask.FromResult(false);
			}

			if (expected is TActual castedExpected &&
			    EqualityComparer<TActual>.Default.Equals(actual, castedExpected))
			{
				return ValueTask.FromResult(true);
			}

			if (typeof(TActual) == typeof(object) &&
			    AreNumericsEqual(actual, expected))
			{
				return ValueTask.FromResult(true);
			}

			return ValueTask.FromResult(Equals(actual, expected));
		}
#else
		public Task<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
		{
			if (actual is null && expected is null)
			{
				return Task.FromResult(true);
			}

			if (actual is null || expected is null)
			{
				return Task.FromResult(false);
			}

			if (DateTimeKindComparison.AreKindsIncompatible(actual, expected))
			{
				return Task.FromResult(false);
			}

			if (expected is TActual castedExpected &&
			    EqualityComparer<TActual>.Default.Equals(actual, castedExpected))
			{
				return Task.FromResult(true);
			}

			if (typeof(TActual) == typeof(object) &&
			    AreNumericsEqual(actual, expected))
			{
				return Task.FromResult(true);
			}

			return Task.FromResult(Equals(actual, expected));
		}
#endif

		private static bool AreNumericsEqual(object actual, object expected)
		{
			Type expectedType = expected.GetType();
			Type actualType = actual.GetType();

			return actualType != expectedType
			       && IsNumericType(actual)
			       && IsNumericType(expected)
			       && IsEqualWhenConverted(actual, expected)
			       && IsEqualWhenConverted(expected, actual);

			bool IsNumericType(object obj)
			{
				return obj is
#if NET8_0_OR_GREATER
					Int128 or
					UInt128 or
					nint or
					nuint or
					Half or
#endif
					int or
					long or
					float or
					double or
					decimal or
					sbyte or
					byte or
					short or
					ushort or
					uint or
					ulong;
			}
		}

		/// <remarks>
		///     Every number is converted with the checked conversion of generic math, which needs no
		///     <see langword="dynamic" /> binder, unavailable when publishing with Native AOT enabled. A value that does
		///     not fit into the target type throws instead of wrapping around, so it counts as not equal, as with
		///     <see cref="Convert.ChangeType(object, Type, IFormatProvider)" /> on the other target frameworks. The caller
		///     converts in both directions, so a value that loses precision in one direction never counts as equal on
		///     its own.
		/// </remarks>
		private static bool IsEqualWhenConverted(object source, object target)
		{
			try
			{
#if NET8_0_OR_GREATER
				return source switch
				{
					int number => IsEqualWhenConverted(number, target),
					long number => IsEqualWhenConverted(number, target),
					float number => IsEqualWhenConverted(number, target),
					double number => IsEqualWhenConverted(number, target),
					decimal number => IsEqualWhenConverted(number, target),
					sbyte number => IsEqualWhenConverted(number, target),
					byte number => IsEqualWhenConverted(number, target),
					short number => IsEqualWhenConverted(number, target),
					ushort number => IsEqualWhenConverted(number, target),
					uint number => IsEqualWhenConverted(number, target),
					ulong number => IsEqualWhenConverted(number, target),
					Int128 number => IsEqualWhenConverted(number, target),
					UInt128 number => IsEqualWhenConverted(number, target),
					nint number => IsEqualWhenConverted(number, target),
					nuint number => IsEqualWhenConverted(number, target),
					Half number => IsEqualWhenConverted(number, target),
					_ => false,
				};
#else
				object? convertedNumber =
					Convert.ChangeType(source, target.GetType(), CultureInfo.InvariantCulture);
				return target.Equals(convertedNumber);
#endif
			}
			catch
			{
				return false;
			}
		}

#if NET8_0_OR_GREATER
		private static bool IsEqualWhenConverted<TSource>(TSource source, object target)
			where TSource : INumberBase<TSource>
			=> target switch
			{
				int number => number.Equals(int.CreateChecked(source)),
				long number => number.Equals(long.CreateChecked(source)),
				float number => number.Equals(float.CreateChecked(source)),
				double number => number.Equals(double.CreateChecked(source)),
				decimal number => number.Equals(decimal.CreateChecked(source)),
				sbyte number => number.Equals(sbyte.CreateChecked(source)),
				byte number => number.Equals(byte.CreateChecked(source)),
				short number => number.Equals(short.CreateChecked(source)),
				ushort number => number.Equals(ushort.CreateChecked(source)),
				uint number => number.Equals(uint.CreateChecked(source)),
				ulong number => number.Equals(ulong.CreateChecked(source)),
				Int128 number => number.Equals(Int128.CreateChecked(source)),
				UInt128 number => number.Equals(UInt128.CreateChecked(source)),
				nint number => number.Equals(nint.CreateChecked(source)),
				nuint number => number.Equals(nuint.CreateChecked(source)),
				Half number => number.Equals(Half.CreateChecked(source)),
				_ => false,
			};
#endif

		/// <inheritdoc cref="IObjectMatchType.GetExpectation(string, ExpectationGrammars)" />
		public string GetExpectation(string expected, ExpectationGrammars grammars)
			=> $"{grammars.Verb("is", "are")} {(grammars.IsNegated() ? "not " : "")}equal to {expected}";

		/// <inheritdoc cref="IObjectMatchType.GetExtendedFailure(string, ExpectationGrammars, object?, object?)" />
		public string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected)
			=> $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual, FormattingOptions.Indented())}";

		/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
		public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
			=> GetItemExpectation(expected, itemNoun, comparison);

		#endregion
	}
}

/// <summary>
///     Checks equality of objects.
/// </summary>
public partial class ObjectEqualityOptions<TSubject> : IOptionsEquality<TSubject>
{
	/// <summary>
	///     The match type.
	/// </summary>
	protected IObjectMatchType MatchType = ObjectEqualityOptions.EqualsMatch;

	/// <inheritdoc />
#if NET8_0_OR_GREATER
	public ValueTask<bool> AreConsideredEqual<TExpected>(TSubject actual, TExpected expected)
#else
	public Task<bool> AreConsideredEqual<TExpected>(TSubject actual, TExpected expected)
#endif
		=> MatchType.AreConsideredEqual(actual, expected);

	/// <summary>
	///     Returns the options to use for all comparisons of one evaluation.
	/// </summary>
	/// <remarks>
	///     Options that depend on a customized setting read it once here instead of on every comparison.
	/// </remarks>
	public virtual IOptionsEquality<TSubject> ForEvaluation() => this;

	/// <summary>
	///     Specifies a new <see cref="IObjectMatchType" /> to use for matching two objects.
	/// </summary>
	public void SetMatchType(IObjectMatchType matchType) => MatchType = matchType;

	/// <summary>
	///     Get an extended failure text.
	/// </summary>
	public string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected)
		=> MatchType.GetExtendedFailure(it, grammars, actual, expected);

	/// <summary>
	///     Returns the expectation string, e.g. <c>be equal to {expectedExpression}</c>.
	/// </summary>
	public string GetExpectation(string expectedExpression, ExpectationGrammars grammars)
		=> MatchType.GetExpectation(expectedExpression, grammars);


	/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
	public string GetItemExpectation(string expected, string? itemNoun = null, string? comparison = null)
		=> MatchType.PrependItemAndComparison(expected, itemNoun, comparison);

	/// <inheritdoc />
	public override string? ToString() => MatchType.ToString();
}
