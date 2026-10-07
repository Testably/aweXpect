#if NET8_0_OR_GREATER
using System.Numerics;
#else
using System.Globalization;
#endif
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Equivalency;

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

	/// <summary>
	///     Returns a cached result for the <paramref name="isMatch" /> outcome that explains a failure with the actual
	///     value, e.g. <c>it was 2</c>.
	/// </summary>
	internal static ValueTask<IObjectMatchResult> ExplainWithActualValue(bool isMatch)
		=> new(isMatch ? ActualValueResult.Match : ActualValueResult.NoMatch);

	private sealed class EqualsMatchType : IObjectMatchType
	{
		/// <inheritdoc cref="object.ToString()" />
		public override string ToString() => "";

		#region IEquality Members

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TSubject, TExpected}(TSubject, TExpected)" />
		public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> new(IsEqual(actual, expected));

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqualWithExplanation{TActual, TExpected}(TActual, TExpected)" />
		public ValueTask<IObjectMatchResult> AreConsideredEqualWithExplanation<TActual, TExpected>(TActual actual,
			TExpected expected)
			=> ExplainWithActualValue(IsEqual(actual, expected));

		private static bool IsEqual<TActual, TExpected>(TActual actual, TExpected expected)
		{
			if (actual is null && expected is null)
			{
				return true;
			}

			if (actual is null || expected is null)
			{
				return false;
			}

			if (DateTimeKindComparison.AreKindsIncompatible(actual, expected))
			{
				return false;
			}

			if (expected is TActual castedExpected &&
			    UserCode.Invoke(static values => EqualityComparer<TActual>.Default.Equals(values.Actual, values.Expected),
				    (Actual: actual, Expected: castedExpected), static values => UserCode.EqualsOf(values.Actual!)))
			{
				return true;
			}

			// A primitive's Equals(object) agrees with its typed Equals, so comparing the boxed values cannot differ.
			if (typeof(TActual).IsPrimitive && expected is TActual)
			{
				return false;
			}

			if (typeof(TActual) == typeof(object) &&
			    AreNumericsEqual(actual, expected))
			{
				return true;
			}

			return UserCode.Invoke(static values => Equals(values.Actual, values.Expected),
				(Actual: actual, Expected: expected), static values => UserCode.EqualsOf(values.Actual!));
		}

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
					Half or
#endif
					nint or
					nuint or
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
		///     its own. <see cref="Convert" /> cannot convert a native integer, so on the other target frameworks it is
		///     converted as a 64-bit integer instead, which holds every native integer.
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
				object widenedTarget = WidenNativeInteger(target);
				object? convertedNumber = Convert.ChangeType(WidenNativeInteger(source), widenedTarget.GetType(),
					CultureInfo.InvariantCulture);
				return widenedTarget.Equals(convertedNumber);
#endif
			}
			catch
			{
				return false;
			}
		}

#if !NET8_0_OR_GREATER
		private static object WidenNativeInteger(object number)
			=> number switch
			{
				IntPtr value => value.ToInt64(),
				UIntPtr value => value.ToUInt64(),
				_ => number,
			};
#endif

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

		/// <inheritdoc cref="IObjectMatchType.AppendContexts(ResultContextCollector)" />
		public void AppendContexts(ResultContextCollector contexts)
		{
			// A comparison by equality has no options to explain.
		}

		/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
		public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
			=> GetItemExpectation(expected, itemNoun, comparison);

		#endregion
	}

	/// <summary>
	///     A result that explains a failure only with the actual value, so that it holds no state of the comparison
	///     and one instance per outcome is shared.
	/// </summary>
	private sealed class ActualValueResult(bool isMatch) : IObjectMatchResult
	{
		public static readonly IObjectMatchResult Match = new ActualValueResult(true);
		public static readonly IObjectMatchResult NoMatch = new ActualValueResult(false);

		/// <inheritdoc cref="IObjectMatchResult.IsMatch" />
		public bool IsMatch => isMatch;

		/// <inheritdoc cref="IObjectMatchResult.GetExtendedFailure(string, ExpectationGrammars, object?, object?)" />
		public string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected)
			=> $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual, FormattingOptions.Indented())}";
	}
}

/// <summary>
///     Checks equality of objects.
/// </summary>
public partial class ObjectEqualityOptions<TSubject> : IOptionsEquality<TSubject>
{
	private string? _matchTypeOption;

	/// <summary>
	///     The match type.
	/// </summary>
	protected IObjectMatchType MatchType = ObjectEqualityOptions.EqualsMatch;

	/// <summary>
	///     Whether the options compare by <see cref="object.Equals(object)" />.
	/// </summary>
	internal bool UsesEqualsMatch => MatchType == ObjectEqualityOptions.EqualsMatch;

	/// <inheritdoc />
	public ValueTask<bool> AreConsideredEqual<TExpected>(TSubject actual, TExpected expected)
		=> MatchType.AreConsideredEqual(actual, expected);

	/// <summary>
	///     Compares the <paramref name="actual" /> with the <paramref name="expected" /> value and returns a result that
	///     can explain a failure.
	/// </summary>
	/// <remarks>
	///     The result is only valid until the next comparison with
	///     <see cref="AreConsideredEqualWithExplanation{TExpected}(TSubject, TExpected)" />, so read it right away.
	/// </remarks>
	public ValueTask<IObjectMatchResult> AreConsideredEqualWithExplanation<TExpected>(TSubject actual,
		TExpected expected)
		=> MatchType.AreConsideredEqualWithExplanation(actual, expected);

	/// <summary>
	///     Returns the options to use for all comparisons of one evaluation.
	/// </summary>
	/// <remarks>
	///     Options that depend on a customized setting read it once here instead of on every comparison.
	/// </remarks>
	public virtual IOptionsEquality<TSubject> ForEvaluation() => this;

	/// <summary>
	///     Returns the options to use for all comparisons of the evaluation in the <paramref name="context" />.
	/// </summary>
	/// <remarks>
	///     A comparison that evaluates expectations of its own, i.e. the <c>It.Is…</c> in the expected object of an
	///     equivalency comparison, evaluates them as part of that evaluation: the <paramref name="cancellationToken" />
	///     cancels them, and its timeout and time system apply to them. The returned options are only valid for that
	///     evaluation. Options whose comparison evaluates nothing return themselves.<br />
	///     Without this, such expectations are evaluated on their own, without a timeout and without cancellation.
	/// </remarks>
	public ObjectEqualityOptions<TSubject> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (MatchType is not EquivalencyMatchType matchType)
		{
			return this;
		}

		return new ObjectEqualityOptions<TSubject>
		{
			MatchType = matchType.ForEvaluation(context, cancellationToken),
		};
	}

	/// <summary>
	///     Specifies a new <see cref="IObjectMatchType" /> to use for matching two objects.
	/// </summary>
	public void SetMatchType(IObjectMatchType matchType) => MatchType = matchType;

	/// <summary>
	///     Specifies the <paramref name="matchType" /> of the option named <paramref name="optionName" /> to use for
	///     matching two objects.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared, or the same option was already specified.
	/// </exception>
	public void SetMatchType(IObjectMatchType matchType, string optionName)
	{
		ThrowIfMatchTypeIsSpecified(optionName);
		_matchTypeOption = optionName;
		MatchType = matchType;
	}

	/// <summary>
	///     Rejects the option named <paramref name="optionName" /> when an option already specified how two objects are
	///     compared, so that a subclass can check this before it validates the value of the option.
	/// </summary>
	private protected void ThrowIfMatchTypeIsSpecified(string optionName)
		=> ThrowHelper.ThrowIfOptionIsAlreadySpecified(_matchTypeOption, optionName);

	/// <summary>
	///     Returns the expectation string, e.g. <c>be equal to {expectedExpression}</c>.
	/// </summary>
	public string GetExpectation(string expectedExpression, ExpectationGrammars grammars)
		=> MatchType.GetExpectation(expectedExpression, grammars);


	/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
	public string GetItemExpectation(string expected, string? itemNoun = null, string? comparison = null)
		=> MatchType.PrependItemAndComparison(expected, itemNoun, comparison);

	/// <summary>
	///     Adds the contexts of the match type to the <paramref name="contexts" />, e.g. the options of an equivalency
	///     comparison.
	/// </summary>
	public void AppendContexts(ResultContextCollector contexts)
		=> MatchType.AppendContexts(contexts);

	/// <inheritdoc />
	public override string? ToString() => MatchType.ToString();
}
