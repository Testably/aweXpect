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

namespace aweXpect.Options;

internal static class ObjectEqualityOptions
{
	/// <summary>
	///     Throws for a <paramref name="result" /> that could not compare the subject, so that every expectation and
	///     every collection fails for it in both polarities, like for an exception of the caller's code.
	/// </summary>
	internal static MatchResult ComparableOrThrow(MatchResult result)
		=> result.Outcome != MatchOutcome.NotComparable
			? result
			: throw new UserCodeException(new NotComparableException(result.NotComparableReason!, result.Cause));

	/// <summary>
	///     The comparison by <see cref="object.Equals(object)" />, which also considers numbers of different types equal
	///     when they have the same value.
	/// </summary>
	internal static class EqualsMatchType
	{
		internal static bool IsEqual<TActual, TExpected>(TActual actual, TExpected expected)
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

			if (HasTypedEqualsLikeObjectEquals<TActual>() && expected is TActual)
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

		/// <summary>
		///     Whether the <see cref="object.Equals(object)" /> of <typeparamref name="T" /> agrees with its typed
		///     <c>Equals</c>, so that comparing the boxed values cannot differ.
		/// </summary>
		private static bool HasTypedEqualsLikeObjectEquals<T>()
			=> typeof(T).IsPrimitive || typeof(T).IsEnum || typeof(T) == typeof(decimal) ||
			   typeof(T) == typeof(Guid) || typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTimeOffset) ||
			   typeof(T) == typeof(TimeSpan);

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
	}

	/// <summary>
	///     The match type that compares by <see cref="EqualsMatchType.IsEqual{TActual, TExpected}" />.
	/// </summary>
	internal sealed class EqualsMatchType<TSubject> : ObjectMatchType<TSubject>
	{
		internal static readonly EqualsMatchType<TSubject> Instance = new();
		private static readonly EqualsMatcher Matcher = new();

		/// <inheritdoc />
		public override MatchPhrase Phrase => MatchPhrase.EqualTo;

		/// <inheritdoc />
		public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
			=> Matcher;

		private sealed class EqualsMatcher : ObjectMatcher<TSubject>
		{
			/// <inheritdoc />
			public override ValueTask<MatchResult> Match<TExpected>(TSubject actual, TExpected expected)
				=> new(MatchResult.From(EqualsMatchType.IsEqual(actual, expected)));
		}
	}
}

/// <summary>
///     Checks equality of objects.
/// </summary>
public partial class ObjectEqualityOptions<TSubject> : IOptionsEquality<TSubject>
{
	/// <summary>
	///     The matcher of the evaluation for options returned by <see cref="ForEvaluation" />, otherwise the matcher for
	///     the comparisons outside an evaluation.
	/// </summary>
	/// <remarks>
	///     The options are configured once and serve every evaluation, so they never keep the matcher of an evaluation,
	///     which may hold its state, its context and its cancellation. A match type without state per evaluation returns
	///     this matcher for every evaluation, so the options are only copied for an evaluation that gets a matcher of
	///     its own.
	/// </remarks>
	private ObjectMatcher<TSubject>? _matcher;

	private ObjectMatchType<TSubject> _matchType = ObjectEqualityOptions.EqualsMatchType<TSubject>.Instance;
	private string? _matchTypeOption;

	/// <summary>
	///     The match type.
	/// </summary>
	/// <remarks>
	///     Only aweXpect.Core sets the match type, so that every option sets it through
	///     <see cref="SetMatchType(ObjectMatchType{TSubject}, string)" /> and is checked against a competing option.
	/// </remarks>
	protected ObjectMatchType<TSubject> MatchType
	{
		get => _matchType;
		private protected set
		{
			_matchType = value;
			_matcher = null;
		}
	}

	/// <summary>
	///     Whether the options compare by <see cref="object.Equals(object)" />.
	/// </summary>
	internal bool UsesEqualsMatch => ReferenceEquals(_matchType, ObjectEqualityOptions.EqualsMatchType<TSubject>.Instance);

	/// <inheritdoc />
	/// <remarks>
	///     The default comparison is called directly, because the generic virtual call to the matcher costs more than
	///     comparing most items.
	/// </remarks>
	public ValueTask<bool> AreConsideredEqual<TExpected>(TSubject actual, TExpected expected)
	{
		if (UsesEqualsMatch)
		{
			return new ValueTask<bool>(ObjectEqualityOptions.EqualsMatchType.IsEqual(actual, expected));
		}

		ValueTask<MatchResult> result = GetMatcher().Match(actual, expected);
		if (!result.IsCompletedSuccessfully)
		{
			return AwaitIsMatch(result);
		}

		return new ValueTask<bool>(ObjectEqualityOptions.ComparableOrThrow(result.Result).IsMatch);

		static async ValueTask<bool> AwaitIsMatch(ValueTask<MatchResult> result)
			=> ObjectEqualityOptions.ComparableOrThrow(await result).IsMatch;
	}

	/// <summary>
	///     Compares the <paramref name="actual" /> with the <paramref name="expected" /> value and returns the result,
	///     from which
	///     <see cref="GetExtendedFailure{TExpected}(string, ExpectationGrammars, TSubject, TExpected, MatchResult, string?)" />
	///     explains a failure.
	/// </summary>
	/// <remarks>
	///     A constraint keeps the result for its failure message, so that the options keep nothing of a comparison.<br />
	///     A subject that the match type cannot compare throws, so that the expectation fails in both polarities.
	/// </remarks>
	public ValueTask<MatchResult> MatchWithExplanation<TExpected>(TSubject actual, TExpected expected)
	{
		if (UsesEqualsMatch)
		{
			return new ValueTask<MatchResult>(
				MatchResult.From(ObjectEqualityOptions.EqualsMatchType.IsEqual(actual, expected)));
		}

		ValueTask<MatchResult> result = GetMatcher().MatchWithExplanation(actual, expected);
		if (!result.IsCompletedSuccessfully)
		{
			return AwaitComparable(result);
		}

		return new ValueTask<MatchResult>(ObjectEqualityOptions.ComparableOrThrow(result.Result));

		static async ValueTask<MatchResult> AwaitComparable(ValueTask<MatchResult> result)
			=> ObjectEqualityOptions.ComparableOrThrow(await result);
	}

	/// <summary>
	///     Get an extended failure text for the comparison of the <paramref name="actual" /> with the
	///     <paramref name="expected" /> object that returned the <paramref name="result" />.
	/// </summary>
	/// <param name="it">The name of the subject in the result text.</param>
	/// <param name="grammars">The grammars of the expectation.</param>
	/// <param name="actual">The actual object of the comparison.</param>
	/// <param name="expected">The expected object of the comparison.</param>
	/// <param name="result">
	///     The result of <see cref="MatchWithExplanation{TExpected}(TSubject, TExpected)" /> for the objects.
	/// </param>
	/// <param name="indentation">
	///     The indentation for the lines after the first, when the failure text is part of a nested result, e.g. of an
	///     expectation in <c>Expect.ThatAll</c>. Pass on the <c>indentation</c> of <c>AppendResult</c>.
	/// </param>
	public string GetExtendedFailure<TExpected>(string it, ExpectationGrammars grammars, TSubject? actual,
		TExpected expected, MatchResult result, string? indentation = null)
		=> GetMatcher().GetFailure(it, grammars, actual, expected, result, indentation);

	/// <summary>
	///     Returns the options to use for all comparisons of the evaluation in the <paramref name="context" />.
	/// </summary>
	/// <remarks>
	///     The match type creates the matcher of the evaluation, so that it can read a customized setting once instead of
	///     on every comparison and keep state across the items of a collection. A comparison that evaluates expectations
	///     of its own, i.e. the <c>It.Is…</c> in the expected object of an equivalency comparison, evaluates them as part
	///     of that evaluation: the <paramref name="cancellationToken" /> cancels them, and its timeout and time system
	///     apply to them.<br />
	///     A constraint compares with the returned options, and also appends its contexts and explains its failures with
	///     them, as only they know what the comparisons of the evaluation found. The options return themselves, unless
	///     the evaluation gets a matcher of its own; then the returned options only have the match type of these options.
	/// </remarks>
	public ObjectEqualityOptions<TSubject> ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (UsesEqualsMatch)
		{
			return this;
		}

		ObjectMatcher<TSubject> matcher = _matchType.ForEvaluation(new MatchEnvironment(context, cancellationToken));
		return ReferenceEquals(matcher, GetMatcher())
			? this
			: new ObjectEqualityOptions<TSubject>
			{
				_matchType = _matchType,
				_matchTypeOption = _matchTypeOption,
				_matcher = matcher,
			};
	}

	/// <summary>
	///     Returns the matcher of the evaluation, or for a comparison outside an evaluation, a matcher without one.
	/// </summary>
	private ObjectMatcher<TSubject> GetMatcher()
	{
		ObjectMatcher<TSubject>? matcher = _matcher;
		if (matcher is null)
		{
			matcher = _matchType.ForEvaluation(MatchEnvironment.WithoutEvaluation);
			_matcher = matcher;
		}

		return matcher;
	}

	/// <summary>
	///     Specifies the <paramref name="matchType" /> of the option named <paramref name="optionName" /> to use for
	///     matching two objects.
	/// </summary>
	/// <remarks>
	///     A match type of the same option replaces the earlier one.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared.
	/// </exception>
	public ObjectEqualityOptions<TSubject> SetMatchType(ObjectMatchType<TSubject> matchType, string optionName)
	{
		matchType.ThrowIfNull();
		ThrowIfMatchTypeIsSpecified(optionName);
		_matchTypeOption = optionName;
		MatchType = matchType;
		return this;
	}

	/// <summary>
	///     Rejects the option named <paramref name="optionName" /> when another option already specified how two objects
	///     are compared, so that a subclass can check this before it validates the value of the option.
	/// </summary>
	private protected void ThrowIfMatchTypeIsSpecified(string optionName)
		=> ThrowHelper.ThrowIfCompetingOption(_matchTypeOption, optionName);

	/// <summary>
	///     Returns the expectation string, e.g. <c>be equal to {expectedExpression}</c>.
	/// </summary>
	public string GetExpectation(string expectedExpression, ExpectationGrammars grammars)
		=> MatchType.Phrase.GetExpectation(expectedExpression, grammars, true);

	/// <summary>
	///     Get the expectation text for a single expected item, e.g. <c>an item equivalent to {expected}</c>.
	/// </summary>
	/// <remarks>
	///     A comparison other than the equality is always named after the <paramref name="itemNoun" />, so that a verb
	///     such as <c>contains</c> keeps a direct object.<br />
	///     The equality is named with the <paramref name="comparison" /> instead, or left out without one, so that a
	///     verb which already names the item reads <c>has item equal to 3</c>.
	/// </remarks>
	public string GetItemExpectation(string expected, string? itemNoun = null, string? comparison = null)
		=> MatchType.Phrase.GetItemExpectation(expected, itemNoun, comparison);

	/// <summary>
	///     Adds the contexts of the matcher to the <paramref name="contexts" />, e.g. the options of an equivalency
	///     comparison.
	/// </summary>
	public void AppendContexts(ResultContextCollector contexts)
		=> GetMatcher().AppendContexts(contexts);

	/// <inheritdoc />
	public override string ToString() => MatchType.Phrase.Name;
}
