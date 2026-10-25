using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Checks equality of objects with an optional tolerance.
/// </summary>
public class ObjectEqualityWithToleranceOptions<TSubject, TTolerance>(
	Func<TSubject, TSubject, TTolerance, bool> isWithinTolerance,
	Func<TTolerance, string>? toString = null)
	: ObjectEqualityOptions<TSubject>
{
	private Action<TTolerance>? _validateTolerance;

	/// <summary>
	///     Interprets the specified tolerance relative to the expected value.
	/// </summary>
	/// <param name="absoluteTolerance">
	///     Calculates the absolute tolerance from the specified tolerance and the expected value.
	/// </param>
	/// <param name="unit">The unit appended to the tolerance in the expectation text, e.g. <c>%</c>.</param>
	/// <remarks>
	///     Applies to the tolerance specified with <see cref="Within(TTolerance)" />, and only to expected values of type
	///     <typeparamref name="TTolerance" />.
	/// </remarks>
	public void UseRelativeTolerance(Func<TTolerance, TTolerance, TTolerance> absoluteTolerance, string unit)
	{
		if (MatchType is WithinMatchType matchType)
		{
			MatchType = matchType.WithRelativeTolerance(absoluteTolerance, unit);
		}
	}

	/// <summary>
	///     Specifies the <paramref name="tolerance" /> within which the actual value is considered equal to the expected
	///     value.
	/// </summary>
	/// <remarks>
	///     A later tolerance replaces the earlier one, including a relative tolerance.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     Another option already specified how two objects are compared.
	/// </exception>
	public ObjectEqualityWithToleranceOptions<TSubject, TTolerance> Within(TTolerance tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance);
		ThrowIfMatchTypeIsSpecified(nameof(Within));
		_validateTolerance?.Invoke(tolerance);
		SetMatchType(new WithinMatchType(() => tolerance, false, isWithinTolerance,
			toString ?? ToleranceHelpers.Format), nameof(Within));
		return this;
	}

	/// <summary>
	///     Specifies the <paramref name="validation" /> that rejects a tolerance passed to
	///     <see cref="Within(TTolerance)" /> which the comparison of <typeparamref name="TSubject" /> cannot honour.
	/// </summary>
	public ObjectEqualityWithToleranceOptions<TSubject, TTolerance> WithToleranceValidation(
		Action<TTolerance> validation)
	{
		_validateTolerance = validation;
		return this;
	}

	/// <summary>
	///     Specifies the <paramref name="defaultTolerance" /> that applies until a tolerance is specified with
	///     <see cref="Within(TTolerance)" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="defaultTolerance" /> is read when the comparison is described, and once per evaluation
	///     through <see cref="ObjectEqualityOptions{TSubject}.ForEvaluation(IEvaluationContext, CancellationToken)" />,
	///     and it is only named in the expectation text when it differs from the default value of
	///     <typeparamref name="TTolerance" />.
	/// </remarks>
	public ObjectEqualityWithToleranceOptions<TSubject, TTolerance> WithDefaultTolerance(
		Func<TTolerance> defaultTolerance)
	{
		MatchType = new WithinMatchType(defaultTolerance, true, isWithinTolerance,
			toString ?? ToleranceHelpers.Format);
		return this;
	}

	private sealed class WithinMatchType(
		Func<TTolerance> tolerance,
		bool isDefault,
		Func<TSubject, TSubject, TTolerance, bool> isWithinTolerance,
		Func<TTolerance, string> toString,
		Func<TTolerance, TTolerance, TTolerance>? absoluteTolerance = null,
		string unit = "")
		: ObjectMatchType<TSubject>
	{
		private WithinMatcher? _matcher;

		public WithinMatchType WithRelativeTolerance(Func<TTolerance, TTolerance, TTolerance> relativeTolerance,
			string relativeUnit)
			=> new(tolerance, isDefault, isWithinTolerance, toString, relativeTolerance, relativeUnit);

		#region IEquality Members

		/// <inheritdoc />
		/// <remarks>
		///     A default tolerance can be customized, so it is read once for each evaluation, and outside an evaluation
		///     for every comparison, while a specified tolerance is the same for every evaluation.
		/// </remarks>
		public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
		{
			if (!isDefault)
			{
				return _matcher ??= new WithinMatcher(this, tolerance());
			}

			return environment.Evaluation is null
				? _matcher ??= new WithinMatcher(this, default!, true)
				: new WithinMatcher(this, tolerance());
		}

		/// <inheritdoc />
		/// <remarks>
		///     A default tolerance can be customized, so the phrase is written whenever it is asked for, and names the
		///     tolerance only when it differs from the default value of <typeparamref name="TTolerance" />.
		/// </remarks>
		public override MatchPhrase Phrase
		{
			get
			{
				TTolerance value = tolerance();
				if (isDefault && EqualityComparer<TTolerance>.Default.Equals(value, default!))
				{
					return MatchPhrase.EqualTo;
				}

				string name = toString.Invoke(value) + unit;
				return MatchPhrase.EqualTo.WithSuffix(name).WithName(name);
			}
		}

		#endregion

		/// <remarks>
		///     A value that is not a <typeparamref name="TSubject" /> never matches. The matcher reads the
		///     <paramref name="tolerance" /> of the match type for every comparison when it
		///     <paramref name="readsTolerance" />.
		/// </remarks>
		private sealed class WithinMatcher(WithinMatchType matchType, TTolerance tolerance, bool readsTolerance = false)
			: ObjectMatcher<TSubject, TSubject>
		{
			/// <inheritdoc />
			public override ValueTask<MatchResult> Match(TSubject actual, TSubject expected)
				=> new(MatchResult.From(IsEqual(actual, expected)));

			private bool IsEqual(TSubject actual, TSubject expected)
			{
				if (actual is null || expected is null)
				{
					return actual is null && expected is null;
				}

				return matchType.IsWithinTolerance(actual, expected, ToleranceFor(expected));
			}

			private TTolerance ToleranceFor(TSubject expected)
			{
				TTolerance value = readsTolerance ? matchType.Tolerance : tolerance;
				return matchType.AbsoluteTolerance is not null && expected is TTolerance typedExpected
					? matchType.AbsoluteTolerance(value, typedExpected)
					: value;
			}
		}

		private TTolerance Tolerance => tolerance();

		private bool IsWithinTolerance(TSubject actual, TSubject expected, TTolerance value)
			=> isWithinTolerance(actual, expected, value);

		private Func<TTolerance, TTolerance, TTolerance>? AbsoluteTolerance => absoluteTolerance;
	}
}
